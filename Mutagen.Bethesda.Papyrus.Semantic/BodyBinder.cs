using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Mutagen.Bethesda.Papyrus.Semantic.Nodes;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;

namespace Mutagen.Bethesda.Papyrus.Semantic;

/// <summary>
/// Binds the statements of a single function or event body: resolves identifiers to
/// symbols, computes expression types, and reports body-level diagnostics such as
/// duplicate locals. Every bound expression is registered in the semantic model's
/// expression map so it can be queried by syntax node later.
/// </summary>
internal sealed class BodyBinder(PapyrusSemanticModel model)
{
    private StateSymbol? _containingState;
    private Scope _scope = null!;

    private ScriptSymbol Script => model.Script;
    private List<PapyrusDiagnostic> Diagnostics => model.BodyDiagnostics;

    public FunctionBody BindFunctionBody(FunctionSymbol function, FunctionBodySyntax body)
    {
        _containingState = function.ContainingSymbol as StateSymbol;
        _scope = new Scope(parent: null);

        foreach (var parameter in function.Parameters)
        {
            _scope.TryDeclare(parameter.Name, parameter);
        }

        return new FunctionBody(body, BindStatements(body.Statements, function));
    }

    public FunctionBody BindEventBody(EventSymbol @event, FunctionBodySyntax body)
    {
        _containingState = @event.ContainingSymbol as StateSymbol;
        _scope = new Scope(parent: null);

        foreach (var parameter in @event.Parameters)
        {
            _scope.TryDeclare(parameter.Name, parameter);
        }

        return new FunctionBody(body, BindStatements(body.Statements, @event));
    }

    private ImmutableArray<PapyrusStatement> BindStatements(SyntaxList<StatementSyntax> statements, ISymbol owner)
    {
        var bound = ImmutableArray.CreateBuilder<PapyrusStatement>(statements.Count);
        foreach (var statement in statements)
        {
            switch (statement)
            {
                case LocalDeclarationStatementSyntax local:
                    bound.Add(BindLocalDeclaration(local, owner));
                    break;
                case ExpressionStatementSyntax expressionStatement:
                    bound.Add(new ExpressionStatement(expressionStatement, BindExpression(expressionStatement.Expression)));
                    break;
                case ReturnStatementSyntax returnStatement:
                    var returned = returnStatement.Expression != null ? BindExpression(returnStatement.Expression) : null;
                    bound.Add(new ReturnStatement(returnStatement, returned));
                    break;
                case IfStatementSyntax ifStatement:
                    bound.Add(BindIfStatement(ifStatement, owner));
                    break;
                case WhileStatementSyntax whileStatement:
                    var whileCondition = BindExpression(whileStatement.Condition);
                    var whileBody = BindBlock(whileStatement.Statements, owner);
                    bound.Add(new WhileStatement(whileStatement, whileCondition, whileBody));
                    break;
                default:
                    Diagnostics.Add(new PapyrusDiagnostic(
                        DiagnosticId.UnexpectedToken,
                        $"Unsupported statement kind '{statement.Kind}'.",
                        statement.FullSpan));
                    break;
            }
        }
        return bound.ToImmutable();
    }

    /// <summary>Binds a nested statement block, which opens a child scope that may shadow outer names.</summary>
    private ImmutableArray<PapyrusStatement> BindBlock(SyntaxList<StatementSyntax> statements, ISymbol owner)
    {
        var outer = _scope;
        _scope = new Scope(outer);
        var bound = BindStatements(statements, owner);
        _scope = outer;
        return bound;
    }

    private LocalDeclarationStatement BindLocalDeclaration(LocalDeclarationStatementSyntax local, ISymbol owner)
    {
        var localSymbol = new VariableSymbol(
            local.Identifier.Text,
            TypeSymbol.FromSyntax(local.Type),
            isConditional: false,
            local.Initializer?.Value,
            owner,
            new DeclaringSyntaxReference(model.SyntaxTree, local.Identifier.Span));

        // Parameters and locals share one namespace.
        if (!_scope.TryDeclare(local.Identifier.Text, localSymbol))
        {
            Diagnostics.Add(new PapyrusDiagnostic(
                DiagnosticId.DuplicateLocal,
                $"Duplicate local variable '{local.Identifier.Text}'.",
                local.Identifier.Span));
        }

        switch (owner)
        {
            case FunctionSymbol function:
                function.AddLocal(localSymbol);
                break;
            case EventSymbol @event:
                @event.AddLocal(localSymbol);
                break;
        }

        var initializer = local.Initializer != null ? BindExpression(local.Initializer.Value) : null;
        return new LocalDeclarationStatement(local, localSymbol, initializer);
    }

    private IfStatement BindIfStatement(IfStatementSyntax ifStatement, ISymbol owner)
    {
        var condition = BindExpression(ifStatement.Condition);
        var thenStatements = BindBlock(ifStatement.Statements, owner);
        var elseIfClauses = ImmutableArray.CreateBuilder<ElseIfClause>(ifStatement.ElseIfClauses.Count);
        foreach (var elseIf in ifStatement.ElseIfClauses)
        {
            var elseIfCondition = BindExpression(elseIf.Condition);
            var elseIfStatements = BindBlock(elseIf.Statements, owner);
            elseIfClauses.Add(new ElseIfClause(elseIf, elseIfCondition, elseIfStatements));
        }
        var elseStatements = ifStatement.ElseClause != null
            ? BindBlock(ifStatement.ElseClause.Statements, owner)
            : [];
        return new IfStatement(ifStatement, condition, thenStatements, elseIfClauses.ToImmutable(), elseStatements);
    }

    private PapyrusExpression BindExpression(ExpressionSyntax node)
    {
        var bound = node switch
        {
            LiteralExpressionSyntax literal => BindLiteral(literal),
            IdentifierNameSyntax identifier => BindIdentifier(identifier),
            ParenthesizedExpressionSyntax parenthesized => BindExpression(parenthesized.Expression),
            PrefixUnaryExpressionSyntax unary => BindUnary(unary),
            BinaryExpressionSyntax binary => BindBinary(binary),
            CastExpressionSyntax cast => new CastExpression(cast, TypeSymbol.FromSyntax(cast.Type), BindExpression(cast.Expression)),
            MemberAccessExpressionSyntax memberAccess => BindMemberAccess(memberAccess),
            ElementAccessExpressionSyntax elementAccess => BindElementAccess(elementAccess),
            InvocationExpressionSyntax invocation => BindInvocation(invocation),
            ArrayCreationExpressionSyntax arrayCreation => BindArrayCreation(arrayCreation),
            AssignmentExpressionSyntax assignment => BindAssignment(assignment),
            _ => new BadExpression(node),
        };
        node.SemanticData = model.AddBoundExpression(bound);
        return bound;
    }

    private PapyrusExpression BindLiteral(LiteralExpressionSyntax literal)
    {
        switch (literal.Token.Kind)
        {
            case SyntaxKind.IntegerLiteralToken:
                return new LiteralExpression(literal, TypeSymbol.Int, ConstantValue.Int(ParseInteger(literal.Token.Text)));
            case SyntaxKind.FloatLiteralToken:
                var floatValue = double.TryParse(literal.Token.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedFloat)
                    ? parsedFloat
                    : 0;
                return new LiteralExpression(literal, TypeSymbol.Float, ConstantValue.Float(floatValue));
            case SyntaxKind.StringLiteralToken:
                return new LiteralExpression(literal, TypeSymbol.String, ConstantValue.String(UnescapeString(literal.Token.Text)));
            case SyntaxKind.TrueKeyword:
                return new LiteralExpression(literal, TypeSymbol.Bool, ConstantValue.Bool(true));
            case SyntaxKind.FalseKeyword:
                return new LiteralExpression(literal, TypeSymbol.Bool, ConstantValue.Bool(false));
            case SyntaxKind.NoneKeyword:
                return new NoneExpression(literal);
            case SyntaxKind.SelfKeyword:
                return new SelfExpression(literal, Script, model.ScriptType);
            case SyntaxKind.ParentKeyword:
                return Script.BaseScript is { } baseScript
                    ? new ScriptExpression(literal, baseScript, TypeSymbol.ScriptType(baseScript.Name), isParent: true)
                    : new BadExpression(literal);
            default:
                return new BadExpression(literal);
        }
    }

    private PapyrusExpression BindIdentifier(IdentifierNameSyntax identifier)
    {
        var name = identifier.Identifier.Text;

        // Papyrus resolves names flat: script-level variables and properties take
        // priority over parameters and locals, even when a local shares the name.
        foreach (var variable in Script.Variables)
        {
            if (string.Equals(variable.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return new VariableExpression(identifier, variable, variable.Type);
            }
        }

        foreach (var property in Script.Properties)
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return new VariableExpression(identifier, property, property.Type);
            }
        }

        // Inherited members: unqualified names resolve through the base chain
        for (var current = Script.BaseScript as ScriptSymbol; current != null; current = current.BaseScript as ScriptSymbol)
        {
            foreach (var variable in current.Variables)
            {
                if (string.Equals(variable.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return new VariableExpression(identifier, variable, variable.Type);
                }
            }
            foreach (var property in current.Properties)
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return new VariableExpression(identifier, property, property.Type);
                }
            }
        }

        if (_scope.TryLookup(name, out var local))
        {
            return new VariableExpression(identifier, local, local.Type);
        }

        // A script name used as a receiver, e.g. `Utility` in `Utility.Wait()`.
        if (model.GetScriptByName(name) is { } script)
        {
            return new ScriptExpression(identifier, script, TypeSymbol.ScriptType(script.Name));
        }

        return new BadExpression(identifier);
    }

    private PapyrusExpression BindUnary(PrefixUnaryExpressionSyntax unary)
    {
        var operand = BindExpression(unary.Operand);
        var type = unary.OperatorToken.Kind switch
        {
            SyntaxKind.ExclamationToken => TypeSymbol.Bool,
            SyntaxKind.MinusToken => operand.Type,
            _ => null,
        };
        return new UnaryExpression(unary, unary.OperatorToken.Kind, operand, type);
    }

    private PapyrusExpression BindBinary(BinaryExpressionSyntax binary)
    {
        var left = BindExpression(binary.Left);
        var right = BindExpression(binary.Right);
        var type = binary.OperatorToken.Kind switch
        {
            SyntaxKind.EqualsEqualsToken
                or SyntaxKind.ExclamationEqualsToken
                or SyntaxKind.LessThanToken
                or SyntaxKind.GreaterThanToken
                or SyntaxKind.LessThanEqualsToken
                or SyntaxKind.GreaterThanEqualsToken
                or SyntaxKind.AmpersandAmpersandToken
                or SyntaxKind.PipePipeToken => TypeSymbol.Bool,
            SyntaxKind.PlusToken
                or SyntaxKind.MinusToken
                or SyntaxKind.StarToken
                or SyntaxKind.SlashToken
                or SyntaxKind.PercentToken => ArithmeticResult(left.Type, right.Type),
            _ => null,
        };
        return new BinaryExpression(binary, binary.OperatorToken.Kind, left, right, type);
    }

    private PapyrusExpression BindMemberAccess(MemberAccessExpressionSyntax memberAccess)
    {
        var receiver = BindExpression(memberAccess.Expression);
        if (IsLengthAccess(memberAccess, receiver))
        {
            return new ArrayLengthExpression(memberAccess, receiver);
        }
        var member = ResolveScriptMember(receiver.Type, memberAccess.Name.Text);
        var type = member switch
        {
            IVariableSymbol variable => variable.Type,
            FunctionSymbol function => function.ReturnType,
            _ => null,
        };
        return new MemberExpression(memberAccess, receiver, member, memberAccess.Name.Text, type);
    }

    private PapyrusExpression BindElementAccess(ElementAccessExpressionSyntax elementAccess)
    {
        var array = BindExpression(elementAccess.Expression);
        var index = BindExpression(elementAccess.Index);
        var type = array.Type is { IsArray: true } arrayType ? arrayType.ElementType : null;
        return new ElementAccessExpression(elementAccess, array, index, type);
    }

    private PapyrusExpression BindInvocation(InvocationExpressionSyntax invocation)
    {
        var arguments = BindArguments(invocation.ArgumentList);
        switch (invocation.Expression)
        {
            case IdentifierNameSyntax callee:
            {
                // Unqualified calls resolve against the containing state first, then
                // the script's own functions.
                var function = ResolveFunction(callee.Identifier.Text);
                var call = new CallExpression(invocation, null, function, callee.Identifier.Text, arguments, function?.ReturnType);
                callee.SemanticData = model.AddBoundExpression(call);
                return call;
            }
            case MemberAccessExpressionSyntax memberAccess:
                return BindMemberInvocation(invocation, memberAccess, arguments);
            default:
                BindExpression(invocation.Expression);
                return new CallExpression(invocation, null, null, null, arguments, null);
        }
    }

    private PapyrusExpression BindMemberInvocation(InvocationExpressionSyntax invocation, MemberAccessExpressionSyntax memberAccess, ImmutableArray<PapyrusExpression> arguments)
    {
        var receiver = BindExpression(memberAccess.Expression);
        if (IsLengthAccess(memberAccess, receiver))
        {
            return new ArrayLengthExpression(memberAccess, receiver);
        }
        var member = ResolveScriptMember(receiver.Type, memberAccess.Name.Text);
        if (member is IFunctionSymbol function)
        {
            var call = new CallExpression(invocation, receiver, function, memberAccess.Name.Text, arguments, function.ReturnType);
            memberAccess.SemanticData = model.AddBoundExpression(call);
            return call;
        }

        // Unresolved (foreign receiver) or a non-function member being invoked; the
        // member access itself stays structured so analyzers can still inspect it.
        var memberType = member is IVariableSymbol variable ? variable.Type : null;
        memberAccess.SemanticData = model.AddBoundExpression(
            new MemberExpression(memberAccess, receiver, member, memberAccess.Name.Text, memberType));
        return new CallExpression(invocation, receiver, null, memberAccess.Name.Text, arguments, null);
    }

    private ImmutableArray<PapyrusExpression> BindArguments(ArgumentListSyntax argumentList)
    {
        var arguments = ImmutableArray.CreateBuilder<PapyrusExpression>(argumentList.Arguments.Count);
        foreach (var argument in argumentList.Arguments)
        {
            arguments.Add(BindExpression(argument.Expression));
        }
        return arguments.ToImmutable();
    }

    private PapyrusExpression BindArrayCreation(ArrayCreationExpressionSyntax arrayCreation)
    {
        var elementType = TypeSymbol.FromSyntax(arrayCreation.ElementType);
        var size = BindExpression(arrayCreation.SizeExpression);
        return new ArrayCreationExpression(arrayCreation, TypeSymbol.MakeArray(elementType), size);
    }

    private PapyrusExpression BindAssignment(AssignmentExpressionSyntax assignment)
    {
        var target = BindExpression(assignment.Left);
        var value = BindExpression(assignment.Right);
        return new AssignmentExpression(assignment, target, value, target.Type);
    }

    private static bool IsLengthAccess(MemberAccessExpressionSyntax memberAccess, PapyrusExpression receiver) =>
        receiver.Type is { IsArray: true }
        && memberAccess.Name.Kind == SyntaxKind.LengthKeyword;

    private IFunctionSymbol? ResolveFunction(string name)
    {
        // Unqualified calls resolve against the containing state first, then the
        // script's own (inherited) functions and events, then imported scripts'
        // globals, and finally the ScriptObject natives every script inherits.
        if (_containingState != null)
        {
            foreach (var member in _containingState.Members)
            {
                if (member is IFunctionSymbol stateFunction
                    && string.Equals(stateFunction.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return stateFunction;
                }
            }
        }

        if (ResolveInChain(Script, name, requireGlobal: false) is IFunctionSymbol function)
        {
            return function;
        }

        foreach (var importName in Script.Imports)
        {
            if (model.GetScriptByName(importName) is { } imported
                && ResolveInChain(imported, name, requireGlobal: true) is IFunctionSymbol importedFunction)
            {
                return importedFunction;
            }
        }

        return PapyrusBuiltins.FindScriptObjectFunction(name);
    }

    private ISymbol? ResolveScriptMember(TypeSymbol? receiverType, string name)
    {
        if (receiverType == null)
        {
            return null;
        }

        // Arrays only expose their builtin functions (and Length, handled earlier).
        if (receiverType.IsArray)
        {
            return PapyrusBuiltins.FindArrayFunction(receiverType, name);
        }

        if (receiverType.Equals(model.ScriptType))
        {
            // Own script: state functions shadow script-level ones.
            if (_containingState != null)
            {
                foreach (var member in _containingState.Members)
                {
                    if (member is IFunctionSymbol stateFunction
                        && string.Equals(stateFunction.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return stateFunction;
                    }
                }
            }

            if (ResolveInChain(Script, name, requireGlobal: false) is { } own)
            {
                return own;
            }
        }
        else if (model.GetScriptByName(receiverType.Name) is { } script)
        {
            // A foreign script type: resolve against that script and its base chain.
            if (ResolveInChain(script, name, requireGlobal: false) is { } foreign)
            {
                return foreign;
            }
        }

        // ScriptObject natives (GetState, GoToState) are available on every script
        // instance regardless of type, but not on predefined values.
        if (!receiverType.IsPredefined)
        {
            return PapyrusBuiltins.FindScriptObjectFunction(name);
        }

        return null;
    }

    private static ISymbol? ResolveInChain(IScriptSymbol start, string name, bool requireGlobal)
    {
        for (var current = start as ScriptSymbol; current != null; current = current.BaseScript as ScriptSymbol)
        {
            foreach (var function in current.Functions)
            {
                if ((!requireGlobal || function.IsGlobal)
                    && string.Equals(function.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return function;
                }
            }

            if (requireGlobal)
            {
                continue;
            }

            // Events are function-like and can be called like functions.
            foreach (var @event in current.Events)
            {
                if (string.Equals(@event.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return @event;
                }
            }

            foreach (var variable in current.Variables)
            {
                if (string.Equals(variable.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return variable;
                }
            }

            foreach (var property in current.Properties)
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return property;
                }
            }
        }

        return null;
    }

    private static TypeSymbol? ArithmeticResult(TypeSymbol? left, TypeSymbol? right)
    {
        if (left == null || right == null) return null;
        if (left.Equals(TypeSymbol.Float) || right.Equals(TypeSymbol.Float)) return TypeSymbol.Float;
        if (left.Equals(TypeSymbol.String) || right.Equals(TypeSymbol.String)) return TypeSymbol.String;
        if (left.Equals(TypeSymbol.Int) || right.Equals(TypeSymbol.Int)) return TypeSymbol.Int;
        return null;
    }

    private static long ParseInteger(string text)
    {
        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return long.TryParse(text.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hex) ? hex : 0;
        }

        return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    private static string UnescapeString(string text)
    {
        // The token text includes the surrounding quotes and raw escape sequences.
        var inner = text.Substring(1, text.Length - 2);
        if (inner.IndexOf('\\') < 0)
        {
            return inner;
        }

        var builder = new System.Text.StringBuilder(inner.Length);
        for (var i = 0; i < inner.Length; i++)
        {
            var c = inner[i];
            if (c == '\\' && i + 1 < inner.Length)
            {
                i++;
                builder.Append(inner[i] switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '"' => '"',
                    '\\' => '\\',
                    var other => other,
                });
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    private sealed class Scope(Scope? parent)
    {
        private readonly Scope? _parent = parent;

        // Most scopes declare nothing; the dictionary is only created on first use.
        private Dictionary<string, IVariableSymbol>? _variables;

        public bool TryDeclare(string name, IVariableSymbol variable)
        {
            _variables ??= new Dictionary<string, IVariableSymbol>(StringComparer.OrdinalIgnoreCase);
            return _variables.TryAdd(name, variable);
        }

        public bool TryLookup(string name, [MaybeNullWhen(false)] out IVariableSymbol variable)
        {
            for (var scope = this; scope != null; scope = scope._parent)
            {
                if (scope._variables != null && scope._variables.TryGetValue(name, out variable))
                {
                    return true;
                }
            }
            variable = null;
            return false;
        }
    }
}
