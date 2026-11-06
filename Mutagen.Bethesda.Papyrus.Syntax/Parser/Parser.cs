using Mutagen.Bethesda.Papyrus.Syntax.Lexer;
namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

/// <summary>
/// Recursive-descent parser producing a lossless syntax tree. On errors it inserts
/// missing tokens and skips unexpected tokens into <c>SkippedTokens</c> nodes so the
/// tree always round-trips to the original source.
/// </summary>
internal sealed class Parser
{
    // Keywords that terminate any block; statement parsing stops at these.
    private static readonly SyntaxKind[] BlockEnders =
    [
        SyntaxKind.EndFunctionKeyword,
        SyntaxKind.EndEventKeyword,
        SyntaxKind.EndStateKeyword,
        SyntaxKind.EndPropertyKeyword,
        SyntaxKind.EndIfKeyword,
        SyntaxKind.ElseIfKeyword,
        SyntaxKind.ElseKeyword,
        SyntaxKind.EndWhileKeyword,
        SyntaxKind.ImportKeyword,
        SyntaxKind.EventKeyword,
        SyntaxKind.StateKeyword,
    ];

    private readonly Lexer.Lexer _lexer;
    private readonly List<LexedToken> _tokens = new();
    private int _pos;

    public List<PapyrusDiagnostic> Diagnostics { get; } = new();
    public List<PapyrusDiagnostic> LexerDiagnostics => _lexer.Diagnostics;

    internal Parser(SourceText source)
    {
        _lexer = new Lexer.Lexer(source);
    }

    private LexedToken Peek(int offset)
    {
        while (_tokens.Count <= _pos + offset)
        {
            if (_tokens.Count > 0 && _tokens[^1].Kind == SyntaxKind.EndOfFileToken)
            {
                return _tokens[^1];
            }
            _tokens.Add(_lexer.LexNext());
        }
        return _tokens[_pos + offset];
    }

    private LexedToken Current => Peek(0);

    private SyntaxToken Consume() => ToSyntaxToken(_tokens[_pos++]);

    private static SyntaxToken ToSyntaxToken(LexedToken token) => new(
        token.Kind,
        token.Text,
        TextSpan.FromBounds(token.FullStart, token.FullEnd),
        token.LeadingTrivia,
        token.TrailingTrivia,
        token.LeadingTriviaWidth,
        token.TrailingTriviaWidth);

    private SyntaxToken Eat(SyntaxKind kind)
    {
        if (Current.Kind == kind) return Consume();
        AddUnexpectedDiagnostic($"Expected {kind},");
        return SyntaxToken.Missing(kind, Current.FullStart);
    }

    private SyntaxToken EatIdentifier()
    {
        if (Current.Kind == SyntaxKind.IdentifierToken) return Consume();
        AddUnexpectedDiagnostic("Expected identifier,");
        return SyntaxToken.Missing(SyntaxKind.IdentifierToken, Current.FullStart);
    }

    private SyntaxToken EatMemberName()
    {
        if (Current.Kind is SyntaxKind.IdentifierToken or SyntaxKind.LengthKeyword) return Consume();
        AddUnexpectedDiagnostic("Expected member name,");
        return SyntaxToken.Missing(SyntaxKind.IdentifierToken, Current.FullStart);
    }

    private void AddUnexpectedDiagnostic(string prefix)
    {
        var found = Current.Kind == SyntaxKind.EndOfFileToken ? "end of file" : $"'{Current.Text}'";
        Diagnostics.Add(new PapyrusDiagnostic(
            DiagnosticId.UnexpectedToken,
            $"{prefix} found {found}.",
            new TextSpan(Current.Start, Current.Width)));
    }

    public CompilationUnitSyntax ParseCompilationUnit()
    {
        var header = ParseScriptHeader();
        var members = new List<ScriptMemberSyntax>();
        while (Current.Kind != SyntaxKind.EndOfFileToken)
        {
            members.Add(ParseMember());
        }
        var endOfFile = Eat(SyntaxKind.EndOfFileToken);
        return new CompilationUnitSyntax(header, new SyntaxList<ScriptMemberSyntax>(members.ToArray()), endOfFile);
    }

    private ScriptHeaderSyntax ParseScriptHeader()
    {
        if (Current.Kind == SyntaxKind.EndOfFileToken)
        {
            // Empty file: no header required
            var missingKeyword = SyntaxToken.Missing(SyntaxKind.ScriptNameKeyword, Current.FullStart);
            var missingName = SyntaxToken.Missing(SyntaxKind.IdentifierToken, Current.FullStart);
            return new ScriptHeaderSyntax(missingKeyword, missingName, null, SyntaxTokenList.Empty);
        }
        var scriptNameKeyword = Eat(SyntaxKind.ScriptNameKeyword);
        var name = EatIdentifier();
        ExtendsClauseSyntax? extendsClause = null;
        if (Current.Kind == SyntaxKind.ExtendsKeyword)
        {
            var extendsKeyword = Consume();
            var baseType = ParseType();
            extendsClause = new ExtendsClauseSyntax(extendsKeyword, baseType);
        }
        var flags = new List<SyntaxToken>();
        while (Current.Kind is SyntaxKind.HiddenKeyword or SyntaxKind.ConditionalKeyword)
        {
            flags.Add(Consume());
        }
        return new ScriptHeaderSyntax(scriptNameKeyword, name, extendsClause, new SyntaxTokenList(flags.ToArray()));
    }

    private ScriptMemberSyntax ParseMember()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.ImportKeyword:
                return ParseImport();
            case SyntaxKind.FunctionKeyword:
                return ParseFunctionRest(null);
            case SyntaxKind.EventKeyword:
                return ParseEvent();
            case SyntaxKind.StateKeyword:
                return ParseState(null);
            case SyntaxKind.AutoKeyword when Peek(1).Kind == SyntaxKind.StateKeyword:
            {
                var autoKeyword = Consume();
                return ParseState(autoKeyword);
            }
        }

        if (IsTypeToken(Current.Kind))
        {
            var type = ParseType();
            switch (Current.Kind)
            {
                case SyntaxKind.PropertyKeyword:
                    return ParsePropertyRest(type);
                case SyntaxKind.FunctionKeyword:
                    return ParseFunctionRest(type);
                case SyntaxKind.IdentifierToken:
                    return ParseVariableRest(type);
            }
        }

        AddUnexpectedDiagnostic("Unexpected token,");
        return SkipMemberTokens();
    }

    private ImportDirectiveSyntax ParseImport()
    {
        var importKeyword = Consume();
        var scriptName = EatIdentifier();
        return new ImportDirectiveSyntax(importKeyword, scriptName);
    }

    private VariableDeclarationSyntax ParseVariableRest(TypeSyntax type)
    {
        var identifier = Consume();
        var initializer = Current.Kind == SyntaxKind.EqualsToken ? ParseEqualsValue() : null;
        var flags = new List<SyntaxToken>();
        while (Current.Kind == SyntaxKind.ConditionalKeyword)
        {
            flags.Add(Consume());
        }
        return new VariableDeclarationSyntax(type, identifier, initializer, new SyntaxTokenList(flags.ToArray()));
    }

    private PropertyDeclarationSyntax ParsePropertyRest(TypeSyntax type)
    {
        var propertyKeyword = Eat(SyntaxKind.PropertyKeyword);
        var identifier = EatIdentifier();
        var initializer = Current.Kind == SyntaxKind.EqualsToken ? ParseEqualsValue() : null;
        var flags = new List<SyntaxToken>();
        while (Current.Kind is SyntaxKind.AutoKeyword or SyntaxKind.AutoReadOnlyKeyword
            or SyntaxKind.HiddenKeyword or SyntaxKind.ConditionalKeyword)
        {
            // `Auto State` begins the next member (an auto state), not a duplicate
            // property flag; without this guard the state's Auto is stolen here.
            if (Current.Kind == SyntaxKind.AutoKeyword && Peek(1).Kind == SyntaxKind.StateKeyword)
            {
                break;
            }
            flags.Add(Consume());
        }
        FunctionDeclarationSyntax? getter = null;
        FunctionDeclarationSyntax? setter = null;
        SyntaxToken? endPropertyToken = null;
        var isAutoLike = flags.Any(f => f.Kind is SyntaxKind.AutoKeyword or SyntaxKind.AutoReadOnlyKeyword);
        if (!isAutoLike)
        {
            var functions = new List<FunctionDeclarationSyntax>();
            while (functions.Count < 2 && IsFunctionStart())
            {
                TypeSyntax? returnType = null;
                if (Current.Kind != SyntaxKind.FunctionKeyword)
                {
                    returnType = ParseType();
                }
                functions.Add(ParseFunctionRest(returnType));
            }
            if (functions.Count > 0)
            {
                getter = functions[0];
                setter = functions.Count > 1 ? functions[1] : null;
                endPropertyToken = Eat(SyntaxKind.EndPropertyKeyword);
            }
        }
        return new PropertyDeclarationSyntax(
            type,
            propertyKeyword,
            identifier,
            initializer,
            new SyntaxTokenList(flags.ToArray()),
            getter,
            setter,
            endPropertyToken);
    }

    private FunctionDeclarationSyntax ParseFunctionRest(TypeSyntax? returnType)
    {
        var functionKeyword = Eat(SyntaxKind.FunctionKeyword);
        var identifier = EatIdentifier();
        var parameterList = ParseParameterList();
        var flags = new List<SyntaxToken>();
        while (Current.Kind is SyntaxKind.NativeKeyword or SyntaxKind.GlobalKeyword)
        {
            flags.Add(Consume());
        }
        var isNative = flags.Any(f => f.Kind == SyntaxKind.NativeKeyword);
        FunctionBodySyntax? body = null;
        if (!isNative)
        {
            if (Current.Kind == SyntaxKind.EndFunctionKeyword || IsStatementStart())
            {
                body = ParseFunctionBody(SyntaxKind.EndFunctionKeyword);
            }
            else
            {
                AddUnexpectedDiagnostic("Expected function body,");
            }
        }
        return new FunctionDeclarationSyntax(
            returnType,
            functionKeyword,
            identifier,
            parameterList,
            new SyntaxTokenList(flags.ToArray()),
            body);
    }

    private FunctionBodySyntax ParseFunctionBody(SyntaxKind endKind)
    {
        var statements = ParseStatements();
        var endToken = Eat(endKind);
        return new FunctionBodySyntax(new SyntaxList<StatementSyntax>(statements.ToArray()), endToken);
    }

    private EventDeclarationSyntax ParseEvent()
    {
        var eventKeyword = Eat(SyntaxKind.EventKeyword);
        var identifier = EatIdentifier();
        var parameterList = ParseParameterList();
        var flags = new List<SyntaxToken>();
        while (Current.Kind is SyntaxKind.NativeKeyword or SyntaxKind.GlobalKeyword)
        {
            flags.Add(Consume());
        }
        var isNative = flags.Any(f => f.Kind == SyntaxKind.NativeKeyword);
        FunctionBodySyntax? body = null;
        if (!isNative)
        {
            if (Current.Kind == SyntaxKind.EndEventKeyword || IsStatementStart())
            {
                body = ParseFunctionBody(SyntaxKind.EndEventKeyword);
            }
            else
            {
                AddUnexpectedDiagnostic("Expected event body,");
            }
        }
        return new EventDeclarationSyntax(
            eventKeyword,
            identifier,
            parameterList,
            new SyntaxTokenList(flags.ToArray()),
            body);
    }

    private StateDeclarationSyntax ParseState(SyntaxToken? autoKeyword)
    {
        var stateKeyword = Eat(SyntaxKind.StateKeyword);
        var identifier = EatIdentifier();
        var members = new List<ScriptMemberSyntax>();
        while (Current.Kind is not (SyntaxKind.EndStateKeyword or SyntaxKind.EndOfFileToken))
        {
            members.Add(ParseMember());
        }
        var endStateToken = Eat(SyntaxKind.EndStateKeyword);
        return new StateDeclarationSyntax(autoKeyword, stateKeyword, identifier, new SyntaxList<ScriptMemberSyntax>(members.ToArray()), endStateToken);
    }

    private ParameterListSyntax ParseParameterList()
    {
        var openParen = Eat(SyntaxKind.OpenParenToken);
        var parameters = new List<ParameterSyntax>();
        var separators = new List<SyntaxToken>();
        while (true)
        {
            if (Current.Kind is SyntaxKind.CloseParenToken or SyntaxKind.EndOfFileToken) break;
            if (IsTypeToken(Current.Kind))
            {
                parameters.Add(ParseParameter());
            }
            else
            {
                AddUnexpectedDiagnostic("Unexpected token in parameter list,");
                var skipped = SkipUntil(SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
                if (skipped == 0 && Current.Kind != SyntaxKind.CommaToken) break;
            }
            if (Current.Kind == SyntaxKind.CommaToken)
            {
                separators.Add(Consume());
            }
            else
            {
                break;
            }
        }
        var closeParen = Eat(SyntaxKind.CloseParenToken);
        return new ParameterListSyntax(openParen, new SeparatedSyntaxList<ParameterSyntax>(parameters.ToArray(), separators.ToArray()), closeParen);
    }

    private ParameterSyntax ParseParameter()
    {
        var type = ParseType();
        var identifier = EatIdentifier();
        var @default = Current.Kind == SyntaxKind.EqualsToken ? ParseEqualsValue() : null;
        return new ParameterSyntax(type, identifier, @default);
    }

    private EqualsValueClauseSyntax ParseEqualsValue()
    {
        var equalsToken = Consume();
        var value = ParseExpression();
        return new EqualsValueClauseSyntax(equalsToken, value);
    }

    private TypeSyntax ParseType()
    {
        TypeSyntax element;
        if (Current.Kind is SyntaxKind.BoolKeyword or SyntaxKind.IntKeyword
            or SyntaxKind.FloatKeyword or SyntaxKind.StringKeyword)
        {
            element = new PredefinedTypeSyntax(Consume());
        }
        else if (Current.Kind == SyntaxKind.IdentifierToken)
        {
            element = new NamedTypeSyntax(Consume());
        }
        else
        {
            AddUnexpectedDiagnostic("Expected type,");
            element = new NamedTypeSyntax(SyntaxToken.Missing(SyntaxKind.IdentifierToken, Current.FullStart));
        }
        if (Current.Kind == SyntaxKind.OpenBracketToken && Peek(1).Kind == SyntaxKind.CloseBracketToken)
        {
            var openBracket = Consume();
            var closeBracket = Consume();
            return new ArrayTypeSyntax(element, openBracket, closeBracket);
        }
        return element;
    }

    private List<StatementSyntax> ParseStatements()
    {
        var statements = new List<StatementSyntax>();
        while (Current.Kind != SyntaxKind.EndOfFileToken && !BlockEnders.Contains(Current.Kind))
        {
            statements.Add(ParseStatement());
        }
        return statements;
    }

    private StatementSyntax ParseStatement()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.IfKeyword: return ParseIf();
            case SyntaxKind.WhileKeyword: return ParseWhile();
            case SyntaxKind.ReturnKeyword: return ParseReturn();
        }

        if (IsLocalDeclarationStart()) return ParseLocalDeclaration();
        if (IsExpressionStart(Current.Kind)) return ParseExpressionStatement();

        AddUnexpectedDiagnostic("Unexpected token,");
        return SkipStatementTokens();
    }

    private LocalDeclarationStatementSyntax ParseLocalDeclaration()
    {
        var type = ParseType();
        var identifier = EatIdentifier();
        var initializer = Current.Kind == SyntaxKind.EqualsToken ? ParseEqualsValue() : null;
        return new LocalDeclarationStatementSyntax(type, identifier, initializer);
    }

    private ExpressionStatementSyntax ParseExpressionStatement()
    {
        var expression = ParseExpression();
        if (IsAssignmentOperator(Current.Kind))
        {
            var operatorToken = Consume();
            var right = ParseExpression();
            expression = new AssignmentExpressionSyntax(expression, operatorToken, right);
        }
        return new ExpressionStatementSyntax(expression);
    }

    private ReturnStatementSyntax ParseReturn()
    {
        var returnKeyword = Consume();
        var expression = IsExpressionStart(Current.Kind) ? ParseExpression() : null;
        return new ReturnStatementSyntax(returnKeyword, expression);
    }

    private IfStatementSyntax ParseIf()
    {
        var ifKeyword = Eat(SyntaxKind.IfKeyword);
        var condition = ParseExpression();
        var statements = ParseStatements();
        var elseIfClauses = new List<ElseIfClauseSyntax>();
        while (Current.Kind == SyntaxKind.ElseIfKeyword)
        {
            var elseIfKeyword = Consume();
            var elseIfCondition = ParseExpression();
            var elseIfStatements = ParseStatements();
            elseIfClauses.Add(new ElseIfClauseSyntax(elseIfKeyword, elseIfCondition, new SyntaxList<StatementSyntax>(elseIfStatements.ToArray())));
        }
        ElseClauseSyntax? elseClause = null;
        if (Current.Kind == SyntaxKind.ElseKeyword)
        {
            var elseKeyword = Consume();
            var elseStatements = ParseStatements();
            elseClause = new ElseClauseSyntax(elseKeyword, new SyntaxList<StatementSyntax>(elseStatements.ToArray()));
        }
        var endIfToken = Eat(SyntaxKind.EndIfKeyword);
        return new IfStatementSyntax(
            ifKeyword,
            condition,
            new SyntaxList<StatementSyntax>(statements.ToArray()),
            new SyntaxList<ElseIfClauseSyntax>(elseIfClauses.ToArray()),
            elseClause,
            endIfToken);
    }

    private WhileStatementSyntax ParseWhile()
    {
        var whileKeyword = Eat(SyntaxKind.WhileKeyword);
        var condition = ParseExpression();
        var statements = ParseStatements();
        var endWhileToken = Eat(SyntaxKind.EndWhileKeyword);
        return new WhileStatementSyntax(whileKeyword, condition, new SyntaxList<StatementSyntax>(statements.ToArray()), endWhileToken);
    }

    private ExpressionSyntax ParseExpression() => ParseBinaryExpression(1);

    private ExpressionSyntax ParseBinaryExpression(int minPrecedence)
    {
        var left = ParseUnaryExpression();
        while (true)
        {
            var precedence = GetBinaryPrecedence(Current.Kind);
            if (precedence < minPrecedence) break;
            var operatorToken = Consume();
            var right = ParseBinaryExpression(precedence + 1);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        return left;
    }

    private static int GetBinaryPrecedence(SyntaxKind kind) => kind switch
    {
        SyntaxKind.PipePipeToken => 1,
        SyntaxKind.AmpersandAmpersandToken => 2,
        SyntaxKind.EqualsEqualsToken or SyntaxKind.ExclamationEqualsToken
            or SyntaxKind.LessThanToken or SyntaxKind.GreaterThanToken
            or SyntaxKind.LessThanEqualsToken or SyntaxKind.GreaterThanEqualsToken => 3,
        SyntaxKind.PlusToken or SyntaxKind.MinusToken => 4,
        SyntaxKind.StarToken or SyntaxKind.SlashToken or SyntaxKind.PercentToken => 5,
        _ => 0,
    };

    private ExpressionSyntax ParseUnaryExpression()
    {
        if (Current.Kind is SyntaxKind.MinusToken or SyntaxKind.ExclamationToken)
        {
            var operatorToken = Consume();
            var operand = ParseCastExpression();
            return new PrefixUnaryExpressionSyntax(operatorToken, operand);
        }
        return ParseCastExpression();
    }

    private ExpressionSyntax ParseCastExpression()
    {
        var expression = ParsePostfixExpression();
        while (Current.Kind == SyntaxKind.AsKeyword)
        {
            var asKeyword = Consume();
            var type = ParseType();
            expression = new CastExpressionSyntax(expression, asKeyword, type);
        }
        return expression;
    }

    private ExpressionSyntax ParsePostfixExpression()
    {
        var expression = ParsePrimaryExpression();
        while (true)
        {
            switch (Current.Kind)
            {
                case SyntaxKind.DotToken:
                {
                    var dotToken = Consume();
                    var name = EatMemberName();
                    expression = new MemberAccessExpressionSyntax(expression, dotToken, name);
                    break;
                }
                case SyntaxKind.OpenBracketToken when !StartsOnNewLine():
                {
                    var openBracket = Consume();
                    var index = ParseExpression();
                    var closeBracket = Eat(SyntaxKind.CloseBracketToken);
                    expression = new ElementAccessExpressionSyntax(expression, openBracket, index, closeBracket);
                    break;
                }
                case SyntaxKind.OpenParenToken when !StartsOnNewLine() && expression is IdentifierNameSyntax or MemberAccessExpressionSyntax:
                {
                    var argumentList = ParseArgumentList();
                    expression = new InvocationExpressionSyntax(expression, argumentList);
                    break;
                }
                default:
                    return expression;
            }
        }
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.IntegerLiteralToken or SyntaxKind.FloatLiteralToken or SyntaxKind.StringLiteralToken
                or SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword or SyntaxKind.NoneKeyword
                or SyntaxKind.SelfKeyword or SyntaxKind.ParentKeyword:
                return new LiteralExpressionSyntax(Consume());
            case SyntaxKind.IdentifierToken or SyntaxKind.LengthKeyword:
                return new IdentifierNameSyntax(Consume());
            case SyntaxKind.OpenParenToken:
            {
                var openParen = Consume();
                var expression = ParseExpression();
                var closeParen = Eat(SyntaxKind.CloseParenToken);
                return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
            }
            case SyntaxKind.NewKeyword:
            {
                var newKeyword = Consume();
                var elementType = ParseType();
                var openBracket = Eat(SyntaxKind.OpenBracketToken);
                var sizeExpression = ParseExpression();
                var closeBracket = Eat(SyntaxKind.CloseBracketToken);
                return new ArrayCreationExpressionSyntax(newKeyword, elementType, openBracket, sizeExpression, closeBracket);
            }
            default:
                AddUnexpectedDiagnostic("Expected expression,");
                return new IdentifierNameSyntax(SyntaxToken.Missing(SyntaxKind.IdentifierToken, Current.FullStart));
        }
    }

    private ArgumentListSyntax ParseArgumentList()
    {
        var openParen = Eat(SyntaxKind.OpenParenToken);
        var arguments = new List<ArgumentSyntax>();
        var separators = new List<SyntaxToken>();
        while (true)
        {
            if (Current.Kind is SyntaxKind.CloseParenToken or SyntaxKind.EndOfFileToken) break;
            if (IsExpressionStart(Current.Kind) || Current.Kind == SyntaxKind.IdentifierToken)
            {
                arguments.Add(ParseArgument());
            }
            else
            {
                AddUnexpectedDiagnostic("Unexpected token in argument list,");
                var skipped = SkipUntil(SyntaxKind.CommaToken, SyntaxKind.CloseParenToken);
                if (skipped == 0 && Current.Kind != SyntaxKind.CommaToken) break;
            }
            if (Current.Kind == SyntaxKind.CommaToken)
            {
                separators.Add(Consume());
            }
            else
            {
                break;
            }
        }
        var closeParen = Eat(SyntaxKind.CloseParenToken);
        return new ArgumentListSyntax(openParen, new SeparatedSyntaxList<ArgumentSyntax>(arguments.ToArray(), separators.ToArray()), closeParen);
    }

    private ArgumentSyntax ParseArgument()
    {
        SyntaxToken? name = null;
        SyntaxToken? equalsToken = null;
        if (Current.Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.EqualsToken)
        {
            name = Consume();
            equalsToken = Consume();
        }
        var expression = ParseExpression();
        return new ArgumentSyntax(name, equalsToken, expression);
    }

    private int SkipUntil(params SyntaxKind[] stopKinds)
    {
        var count = 0;
        while (Current.Kind != SyntaxKind.EndOfFileToken && !stopKinds.Contains(Current.Kind))
        {
            Consume();
            count++;
        }
        return count;
    }

    private SkippedMemberTokensSyntax SkipMemberTokens()
    {
        var tokens = new List<SyntaxToken>();
        while (Current.Kind != SyntaxKind.EndOfFileToken && !IsMemberStart())
        {
            tokens.Add(Consume());
        }
        return new SkippedMemberTokensSyntax(tokens.ToArray(), Current.FullStart);
    }

    private SkippedTokensSyntax SkipStatementTokens()
    {
        var tokens = new List<SyntaxToken>();
        while (Current.Kind != SyntaxKind.EndOfFileToken
            && !BlockEnders.Contains(Current.Kind)
            && !IsStatementStart())
        {
            tokens.Add(Consume());
        }
        return new SkippedTokensSyntax(tokens.ToArray(), Current.FullStart);
    }

    /// <summary>
    /// Papyrus statements are newline-delimited, so a paren or bracket that starts a new
    /// line does not continue the previous expression as a call or element access.
    /// A line continuation explicitly joins the lines back together.
    /// </summary>
    private bool StartsOnNewLine()
    {
        if (_pos > 0)
        {
            foreach (var trivia in _tokens[_pos - 1].TrailingTrivia)
            {
                if (trivia.Kind == SyntaxKind.EndOfLineTrivia) return true;
            }
        }
        var sawContinuation = false;
        foreach (var trivia in Current.LeadingTrivia)
        {
            if (trivia.Kind == SyntaxKind.LineContinuationTrivia) sawContinuation = true;
            if (trivia.Kind == SyntaxKind.EndOfLineTrivia && !sawContinuation) return true;
        }
        return false;
    }

    private bool IsFunctionStart()
    {
        if (Current.Kind == SyntaxKind.FunctionKeyword) return true;
        if (!IsTypeToken(Current.Kind)) return false;
        if (Current.Kind == SyntaxKind.IdentifierToken
            && Peek(1).Kind == SyntaxKind.OpenBracketToken
            && Peek(2).Kind == SyntaxKind.CloseBracketToken)
        {
            return Peek(3).Kind == SyntaxKind.FunctionKeyword;
        }
        return Peek(1).Kind == SyntaxKind.FunctionKeyword;
    }

    private bool IsMemberStart()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.ImportKeyword or SyntaxKind.FunctionKeyword or SyntaxKind.EventKeyword or SyntaxKind.StateKeyword:
                return true;
            case SyntaxKind.AutoKeyword:
                return Peek(1).Kind == SyntaxKind.StateKeyword;
            case SyntaxKind.BoolKeyword or SyntaxKind.IntKeyword or SyntaxKind.FloatKeyword or SyntaxKind.StringKeyword:
                return true;
            case SyntaxKind.IdentifierToken:
                return true;
            default:
                return false;
        }
    }

    private static bool IsTypeToken(SyntaxKind kind) => kind switch
    {
        SyntaxKind.BoolKeyword or SyntaxKind.IntKeyword or SyntaxKind.FloatKeyword or SyntaxKind.StringKeyword => true,
        SyntaxKind.IdentifierToken => true,
        _ => false,
    };

    private bool IsLocalDeclarationStart()
    {
        if (Current.Kind is SyntaxKind.BoolKeyword or SyntaxKind.IntKeyword
            or SyntaxKind.FloatKeyword or SyntaxKind.StringKeyword)
        {
            if (Peek(1).Kind == SyntaxKind.IdentifierToken) return true;
            return Peek(1).Kind == SyntaxKind.OpenBracketToken
                && Peek(2).Kind == SyntaxKind.CloseBracketToken
                && Peek(3).Kind == SyntaxKind.IdentifierToken;
        }
        if (Current.Kind == SyntaxKind.IdentifierToken)
        {
            if (Peek(1).Kind == SyntaxKind.IdentifierToken) return true;
            return Peek(1).Kind == SyntaxKind.OpenBracketToken
                && Peek(2).Kind == SyntaxKind.CloseBracketToken
                && Peek(3).Kind == SyntaxKind.IdentifierToken;
        }
        return false;
    }

    private static bool IsExpressionStart(SyntaxKind kind) => kind switch
    {
        SyntaxKind.IntegerLiteralToken or SyntaxKind.FloatLiteralToken or SyntaxKind.StringLiteralToken
            or SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword or SyntaxKind.NoneKeyword
            or SyntaxKind.SelfKeyword or SyntaxKind.ParentKeyword
            or SyntaxKind.IdentifierToken or SyntaxKind.LengthKeyword
            or SyntaxKind.OpenParenToken or SyntaxKind.NewKeyword
            or SyntaxKind.MinusToken or SyntaxKind.ExclamationToken => true,
        _ => false,
    };

    private bool IsStatementStart() =>
        Current.Kind is SyntaxKind.IfKeyword or SyntaxKind.WhileKeyword or SyntaxKind.ReturnKeyword
        || IsLocalDeclarationStart()
        || IsExpressionStart(Current.Kind);

    private static bool IsAssignmentOperator(SyntaxKind kind) => kind switch
    {
        SyntaxKind.EqualsToken or SyntaxKind.PlusEqualsToken or SyntaxKind.MinusEqualsToken
            or SyntaxKind.StarEqualsToken or SyntaxKind.SlashEqualsToken or SyntaxKind.PercentEqualsToken => true,
        _ => false,
    };
}
