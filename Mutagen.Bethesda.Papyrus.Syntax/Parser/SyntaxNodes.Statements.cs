namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

public abstract class StatementSyntax(SyntaxKind kind, TextSpan fullSpan) : PapyrusSyntaxNode(kind, fullSpan);

public sealed class LocalDeclarationStatementSyntax : StatementSyntax
{
    public TypeSyntax Type { get; }
    public SyntaxToken Identifier { get; }
    public EqualsValueClauseSyntax? Initializer { get; }

    internal LocalDeclarationStatementSyntax(TypeSyntax type, SyntaxToken identifier, EqualsValueClauseSyntax? initializer)
        : base(SyntaxKind.LocalDeclarationStatement, FullSpanBetween(type, LastOf(identifier, initializer)))
    {
        Type = type;
        Identifier = identifier;
        Initializer = initializer;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Type;
        yield return Identifier;
        if (Initializer != null) yield return Initializer;
    }
}

public sealed class ExpressionStatementSyntax : StatementSyntax
{
    public ExpressionSyntax Expression { get; }

    internal ExpressionStatementSyntax(ExpressionSyntax expression)
        : base(SyntaxKind.ExpressionStatement, expression.FullSpan)
    {
        Expression = expression;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Expression;
    }
}

public sealed class ReturnStatementSyntax : StatementSyntax
{
    public SyntaxToken ReturnKeyword { get; }
    public ExpressionSyntax? Expression { get; }

    internal ReturnStatementSyntax(SyntaxToken returnKeyword, ExpressionSyntax? expression)
        : base(SyntaxKind.ReturnStatement, FullSpanBetween(returnKeyword, LastOf(returnKeyword, expression)))
    {
        ReturnKeyword = returnKeyword;
        Expression = expression;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ReturnKeyword;
        if (Expression != null) yield return Expression;
    }
}

public sealed class IfStatementSyntax : StatementSyntax
{
    public SyntaxToken IfKeyword { get; }
    public ExpressionSyntax Condition { get; }
    public SyntaxList<StatementSyntax> Statements { get; }
    public SyntaxList<ElseIfClauseSyntax> ElseIfClauses { get; }
    public ElseClauseSyntax? ElseClause { get; }
    public SyntaxToken EndIfToken { get; }

    internal IfStatementSyntax(
        SyntaxToken ifKeyword,
        ExpressionSyntax condition,
        SyntaxList<StatementSyntax> statements,
        SyntaxList<ElseIfClauseSyntax> elseIfClauses,
        ElseClauseSyntax? elseClause,
        SyntaxToken endIfToken)
        : base(SyntaxKind.IfStatement, FullSpanBetween(ifKeyword, endIfToken))
    {
        IfKeyword = ifKeyword;
        Condition = condition;
        Statements = statements;
        ElseIfClauses = elseIfClauses;
        ElseClause = elseClause;
        EndIfToken = endIfToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return IfKeyword;
        yield return Condition;
        foreach (var statement in Statements) yield return statement;
        foreach (var elseIf in ElseIfClauses) yield return elseIf;
        if (ElseClause != null) yield return ElseClause;
        yield return EndIfToken;
    }
}

public sealed class ElseIfClauseSyntax : PapyrusSyntaxNode
{
    public SyntaxToken ElseIfKeyword { get; }
    public ExpressionSyntax Condition { get; }
    public SyntaxList<StatementSyntax> Statements { get; }

    internal ElseIfClauseSyntax(SyntaxToken elseIfKeyword, ExpressionSyntax condition, SyntaxList<StatementSyntax> statements)
        : base(SyntaxKind.ElseIfClause, FullSpanBetween(elseIfKeyword, statements.Count > 0 ? statements[statements.Count - 1] : condition))
    {
        ElseIfKeyword = elseIfKeyword;
        Condition = condition;
        Statements = statements;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ElseIfKeyword;
        yield return Condition;
        foreach (var statement in Statements) yield return statement;
    }
}

public sealed class ElseClauseSyntax : PapyrusSyntaxNode
{
    public SyntaxToken ElseKeyword { get; }
    public SyntaxList<StatementSyntax> Statements { get; }

    internal ElseClauseSyntax(SyntaxToken elseKeyword, SyntaxList<StatementSyntax> statements)
        : base(SyntaxKind.ElseClause, FullSpanBetween(elseKeyword, statements.Count > 0 ? statements[statements.Count - 1] : elseKeyword))
    {
        ElseKeyword = elseKeyword;
        Statements = statements;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ElseKeyword;
        foreach (var statement in Statements) yield return statement;
    }
}

public sealed class WhileStatementSyntax : StatementSyntax
{
    public SyntaxToken WhileKeyword { get; }
    public ExpressionSyntax Condition { get; }
    public SyntaxList<StatementSyntax> Statements { get; }
    public SyntaxToken EndWhileToken { get; }

    internal WhileStatementSyntax(
        SyntaxToken whileKeyword,
        ExpressionSyntax condition,
        SyntaxList<StatementSyntax> statements,
        SyntaxToken endWhileToken)
        : base(SyntaxKind.WhileStatement, FullSpanBetween(whileKeyword, endWhileToken))
    {
        WhileKeyword = whileKeyword;
        Condition = condition;
        Statements = statements;
        EndWhileToken = endWhileToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return WhileKeyword;
        yield return Condition;
        foreach (var statement in Statements) yield return statement;
        yield return EndWhileToken;
    }
}
