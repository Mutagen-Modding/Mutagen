namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

public sealed class CompilationUnitSyntax : PapyrusSyntaxNode
{
    public ScriptHeaderSyntax Header { get; }
    public SyntaxList<ScriptMemberSyntax> Members { get; }
    public SyntaxToken EndOfFileToken { get; }

    private SyntaxTree? _tree;

    /// <summary>The owning tree; assigned by the tree after parsing (this is the root).</summary>
    public override SyntaxTree SyntaxTree => _tree!;

    internal void AttachTree(SyntaxTree tree) => _tree = tree;

    internal CompilationUnitSyntax(ScriptHeaderSyntax header, SyntaxList<ScriptMemberSyntax> members, SyntaxToken endOfFileToken)
        : base(SyntaxKind.CompilationUnit, FullSpanBetween(header, endOfFileToken))
    {
        Header = header;
        Members = members;
        EndOfFileToken = endOfFileToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Header;
        foreach (var member in Members) yield return member;
        yield return EndOfFileToken;
    }
}

public sealed class ScriptHeaderSyntax : PapyrusSyntaxNode
{
    public SyntaxToken ScriptNameKeyword { get; }
    public SyntaxToken Name { get; }
    public ExtendsClauseSyntax? ExtendsClause { get; }
    public SyntaxTokenList Flags { get; }

    internal ScriptHeaderSyntax(SyntaxToken scriptNameKeyword, SyntaxToken name, ExtendsClauseSyntax? extendsClause, SyntaxTokenList flags)
        : base(SyntaxKind.ScriptHeader, FullSpanBetween(scriptNameKeyword, LastOf(name, extendsClause, flags.Count > 0 ? flags[flags.Count - 1] : null)))
    {
        ScriptNameKeyword = scriptNameKeyword;
        Name = name;
        ExtendsClause = extendsClause;
        Flags = flags;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ScriptNameKeyword;
        yield return Name;
        if (ExtendsClause != null) yield return ExtendsClause;
        foreach (var flag in Flags) yield return flag;
    }
}

public sealed class ExtendsClauseSyntax : PapyrusSyntaxNode
{
    public SyntaxToken ExtendsKeyword { get; }
    public TypeSyntax BaseType { get; }

    internal ExtendsClauseSyntax(SyntaxToken extendsKeyword, TypeSyntax baseType)
        : base(SyntaxKind.ExtendsClause, FullSpanBetween(extendsKeyword, baseType))
    {
        ExtendsKeyword = extendsKeyword;
        BaseType = baseType;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ExtendsKeyword;
        yield return BaseType;
    }
}

public sealed class ImportDirectiveSyntax : ScriptMemberSyntax
{
    public SyntaxToken ImportKeyword { get; }
    public SyntaxToken ScriptName { get; }

    internal ImportDirectiveSyntax(SyntaxToken importKeyword, SyntaxToken scriptName)
        : base(SyntaxKind.ImportDirective, FullSpanBetween(importKeyword, scriptName))
    {
        ImportKeyword = importKeyword;
        ScriptName = scriptName;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ImportKeyword;
        yield return ScriptName;
    }
}

public abstract class ScriptMemberSyntax(SyntaxKind kind, TextSpan fullSpan) : PapyrusSyntaxNode(kind, fullSpan);

public sealed class VariableDeclarationSyntax : ScriptMemberSyntax
{
    public TypeSyntax Type { get; }
    public SyntaxToken Identifier { get; }
    public EqualsValueClauseSyntax? Initializer { get; }
    public SyntaxTokenList Flags { get; }

    internal VariableDeclarationSyntax(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax? initializer, SyntaxTokenList flags)
        : base(SyntaxKind.VariableDeclaration, FullSpanBetween(type, LastOf(identifier, initializer, flags.Count > 0 ? flags[flags.Count - 1] : null)))
    {
        Type = type;
        Identifier = identifier;
        Initializer = initializer;
        Flags = flags;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Type;
        yield return Identifier;
        if (Initializer != null) yield return Initializer;
        foreach (var flag in Flags) yield return flag;
    }
}

public sealed class PropertyDeclarationSyntax : ScriptMemberSyntax
{
    public TypeSyntax Type { get; }
    public SyntaxToken PropertyKeyword { get; }
    public SyntaxToken Identifier { get; }
    public EqualsValueClauseSyntax? Initializer { get; }
    public SyntaxTokenList Flags { get; }
    public FunctionDeclarationSyntax? Getter { get; }
    public FunctionDeclarationSyntax? Setter { get; }
    public SyntaxToken? EndPropertyToken { get; }

    internal PropertyDeclarationSyntax(
        TypeSyntax type,
        SyntaxToken propertyKeyword,
        SyntaxToken identifier,
        EqualsValueClauseSyntax? initializer,
        SyntaxTokenList flags,
        FunctionDeclarationSyntax? getter,
        FunctionDeclarationSyntax? setter,
        SyntaxToken? endPropertyToken)
        : base(SyntaxKind.PropertyDeclaration, FullSpanBetween(type, LastOf(identifier, initializer, flags.Count > 0 ? flags[flags.Count - 1] : null, getter, setter, endPropertyToken)))
    {
        Type = type;
        PropertyKeyword = propertyKeyword;
        Identifier = identifier;
        Initializer = initializer;
        Flags = flags;
        Getter = getter;
        Setter = setter;
        EndPropertyToken = endPropertyToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Type;
        yield return PropertyKeyword;
        yield return Identifier;
        if (Initializer != null) yield return Initializer;
        foreach (var flag in Flags) yield return flag;
        if (Getter != null) yield return Getter;
        if (Setter != null) yield return Setter;
        if (EndPropertyToken != null) yield return EndPropertyToken;
    }
}

public sealed class FunctionDeclarationSyntax : ScriptMemberSyntax
{
    public TypeSyntax? ReturnType { get; }
    public SyntaxToken FunctionKeyword { get; }
    public SyntaxToken Identifier { get; }
    public ParameterListSyntax ParameterList { get; }
    public SyntaxTokenList Flags { get; }
    public FunctionBodySyntax? Body { get; }

    internal FunctionDeclarationSyntax(
        TypeSyntax? returnType,
        SyntaxToken functionKeyword,
        SyntaxToken identifier,
        ParameterListSyntax parameterList,
        SyntaxTokenList flags,
        FunctionBodySyntax? body)
        : base(SyntaxKind.FunctionDeclaration, FullSpanBetween(FirstOf(returnType, functionKeyword), LastOf(parameterList, body)))
    {
        ReturnType = returnType;
        FunctionKeyword = functionKeyword;
        Identifier = identifier;
        ParameterList = parameterList;
        Flags = flags;
        Body = body;
    }

    public bool IsNative => Flags.Any(f => f.Kind == SyntaxKind.NativeKeyword);

    public bool IsGlobal => Flags.Any(f => f.Kind == SyntaxKind.GlobalKeyword);

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        if (ReturnType != null) yield return ReturnType;
        yield return FunctionKeyword;
        yield return Identifier;
        yield return ParameterList;
        foreach (var flag in Flags) yield return flag;
        if (Body != null) yield return Body;
    }
}

public sealed class FunctionBodySyntax : PapyrusSyntaxNode
{
    public SyntaxList<StatementSyntax> Statements { get; }
    public SyntaxToken EndToken { get; }

    internal FunctionBodySyntax(SyntaxList<StatementSyntax> statements, SyntaxToken endToken)
        : base(SyntaxKind.FunctionBody, FullSpanBetween(statements.Count > 0 ? statements[0] : endToken, endToken))
    {
        Statements = statements;
        EndToken = endToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        foreach (var statement in Statements) yield return statement;
        yield return EndToken;
    }
}

public sealed class EventDeclarationSyntax : ScriptMemberSyntax
{
    public SyntaxToken EventKeyword { get; }
    public SyntaxToken Identifier { get; }
    public ParameterListSyntax ParameterList { get; }
    public SyntaxTokenList Flags { get; }
    public FunctionBodySyntax? Body { get; }

    internal EventDeclarationSyntax(
        SyntaxToken eventKeyword,
        SyntaxToken identifier,
        ParameterListSyntax parameterList,
        SyntaxTokenList flags,
        FunctionBodySyntax? body)
        : base(SyntaxKind.EventDeclaration, FullSpanBetween(eventKeyword, LastOf(parameterList, body)))
    {
        EventKeyword = eventKeyword;
        Identifier = identifier;
        ParameterList = parameterList;
        Flags = flags;
        Body = body;
    }

    public bool IsNative => Flags.Any(f => f.Kind == SyntaxKind.NativeKeyword);

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return EventKeyword;
        yield return Identifier;
        yield return ParameterList;
        foreach (var flag in Flags) yield return flag;
        if (Body != null) yield return Body;
    }
}

public sealed class StateDeclarationSyntax : ScriptMemberSyntax
{
    public SyntaxToken? AutoKeyword { get; }
    public SyntaxToken StateKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxList<ScriptMemberSyntax> Members { get; }
    public SyntaxToken EndStateToken { get; }

    internal StateDeclarationSyntax(
        SyntaxToken? autoKeyword,
        SyntaxToken stateKeyword,
        SyntaxToken identifier,
        SyntaxList<ScriptMemberSyntax> members,
        SyntaxToken endStateToken)
        : base(SyntaxKind.StateDeclaration, FullSpanBetween(FirstOf(autoKeyword, stateKeyword), endStateToken))
    {
        AutoKeyword = autoKeyword;
        StateKeyword = stateKeyword;
        Identifier = identifier;
        Members = members;
        EndStateToken = endStateToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        if (AutoKeyword != null) yield return AutoKeyword;
        yield return StateKeyword;
        yield return Identifier;
        foreach (var member in Members) yield return member;
        yield return EndStateToken;
    }
}

public sealed class ParameterListSyntax : PapyrusSyntaxNode
{
    public SyntaxToken OpenParenToken { get; }
    public SeparatedSyntaxList<ParameterSyntax> Parameters { get; }
    public SyntaxToken CloseParenToken { get; }

    internal ParameterListSyntax(SyntaxToken openParenToken, SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenToken)
        : base(SyntaxKind.ParameterList, FullSpanBetween(openParenToken, closeParenToken))
    {
        OpenParenToken = openParenToken;
        Parameters = parameters;
        CloseParenToken = closeParenToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return OpenParenToken;
        for (var i = 0; i < Parameters.Count; i++)
        {
            yield return Parameters[i];
            if (i < Parameters.SeparatorCount) yield return Parameters.GetSeparator(i);
        }
        yield return CloseParenToken;
    }
}

public sealed class ParameterSyntax : PapyrusSyntaxNode
{
    public TypeSyntax Type { get; }
    public SyntaxToken Identifier { get; }
    public EqualsValueClauseSyntax? Default { get; }

    internal ParameterSyntax(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax? @default)
        : base(SyntaxKind.Parameter, FullSpanBetween(type, LastOf(identifier, @default)))
    {
        Type = type;
        Identifier = identifier;
        Default = @default;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Type;
        yield return Identifier;
        if (Default != null) yield return Default;
    }
}

public sealed class EqualsValueClauseSyntax : PapyrusSyntaxNode
{
    public SyntaxToken EqualsToken { get; }
    public ExpressionSyntax Value { get; }

    internal EqualsValueClauseSyntax(SyntaxToken equalsToken, ExpressionSyntax value)
        : base(SyntaxKind.EqualsValueClause, FullSpanBetween(equalsToken, value))
    {
        EqualsToken = equalsToken;
        Value = value;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return EqualsToken;
        yield return Value;
    }
}

/// <summary>Tokens the parser skipped during error recovery, kept so the tree stays lossless.</summary>
public sealed class SkippedTokensSyntax : StatementSyntax
{
    private readonly SyntaxToken[] _tokens;

    internal SkippedTokensSyntax(SyntaxToken[] tokens, int fallbackPosition)
        : base(SyntaxKind.SkippedTokens, tokens.Length > 0 ? FullSpanBetween(tokens[0], tokens[^1]) : new TextSpan(fallbackPosition, 0))
    {
        _tokens = tokens;
    }

    public int Count => _tokens.Length;

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens() => _tokens;
}

/// <summary>Tokens the parser skipped at member level during error recovery.</summary>
public sealed class SkippedMemberTokensSyntax : ScriptMemberSyntax
{
    private readonly SyntaxToken[] _tokens;

    internal SkippedMemberTokensSyntax(SyntaxToken[] tokens, int fallbackPosition)
        : base(SyntaxKind.SkippedTokens, tokens.Length > 0 ? FullSpanBetween(tokens[0], tokens[^1]) : new TextSpan(fallbackPosition, 0))
    {
        _tokens = tokens;
    }

    public int Count => _tokens.Length;

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens() => _tokens;
}
