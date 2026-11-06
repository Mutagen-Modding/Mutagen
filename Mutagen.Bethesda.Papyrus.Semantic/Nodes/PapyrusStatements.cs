using System.Collections.Immutable;
using Mutagen.Bethesda.Papyrus.Semantic.Symbols;
using Mutagen.Bethesda.Papyrus.Syntax.Parser;
namespace Mutagen.Bethesda.Papyrus.Semantic.Nodes;

/// <summary>The body of a function or event: its executable statements.</summary>
public sealed class FunctionBody(FunctionBodySyntax? syntax, ImmutableArray<PapyrusStatement> statements) : PapyrusNode(syntax)
{
    public ImmutableArray<PapyrusStatement> Statements { get; } = statements;
}

public sealed class LocalDeclarationStatement(LocalDeclarationStatementSyntax syntax, VariableSymbol local, PapyrusExpression? initializer) : PapyrusStatement(syntax)
{
    public VariableSymbol Local { get; } = local;
    public PapyrusExpression? Initializer { get; } = initializer;
}

public sealed class ExpressionStatement(ExpressionStatementSyntax syntax, PapyrusExpression expression) : PapyrusStatement(syntax)
{
    public PapyrusExpression Expression { get; } = expression;
}

public sealed class ReturnStatement(ReturnStatementSyntax syntax, PapyrusExpression? expression) : PapyrusStatement(syntax)
{
    public PapyrusExpression? Expression { get; } = expression;
}

public sealed class IfStatement(
    IfStatementSyntax syntax,
    PapyrusExpression condition,
    ImmutableArray<PapyrusStatement> thenStatements,
    ImmutableArray<ElseIfClause> elseIfClauses,
    ImmutableArray<PapyrusStatement> elseStatements)
    : PapyrusStatement(syntax)
{
    public PapyrusExpression Condition { get; } = condition;
    public ImmutableArray<PapyrusStatement> ThenStatements { get; } = thenStatements;
    public ImmutableArray<ElseIfClause> ElseIfClauses { get; } = elseIfClauses;
    public ImmutableArray<PapyrusStatement> ElseStatements { get; } = elseStatements;
}

public sealed class ElseIfClause(ElseIfClauseSyntax syntax, PapyrusExpression condition, ImmutableArray<PapyrusStatement> statements) : PapyrusNode(syntax)
{
    public PapyrusExpression Condition { get; } = condition;
    public ImmutableArray<PapyrusStatement> Statements { get; } = statements;
}

public sealed class WhileStatement(WhileStatementSyntax syntax, PapyrusExpression condition, ImmutableArray<PapyrusStatement> statements) : PapyrusStatement(syntax)
{
    public PapyrusExpression Condition { get; } = condition;
    public ImmutableArray<PapyrusStatement> Statements { get; } = statements;
}
