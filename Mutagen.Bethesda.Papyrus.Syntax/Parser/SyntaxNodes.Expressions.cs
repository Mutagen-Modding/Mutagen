namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

/// <summary>
/// Opaque handle to semantic-layer data attached to this expression: an index into
/// the owning semantic model's bound-expression list, or -1 when not bound. An int
/// is used rather than a reference so binding does not write references into the
/// (old-generation) syntax nodes, which would dirty GC card tables.
/// </summary>
public abstract class ExpressionSyntax(SyntaxKind kind, TextSpan fullSpan) : PapyrusSyntaxNode(kind, fullSpan)
{
    internal int SemanticData { get; set; } = -1;
}

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken Token { get; }

    internal LiteralExpressionSyntax(SyntaxToken token)
        : base(SyntaxKind.LiteralExpression, token.FullSpan)
    {
        Token = token;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Token;
    }
}

public sealed class IdentifierNameSyntax : ExpressionSyntax
{
    public SyntaxToken Identifier { get; }

    internal IdentifierNameSyntax(SyntaxToken identifier)
        : base(SyntaxKind.IdentifierName, identifier.FullSpan)
    {
        Identifier = identifier;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Identifier;
    }
}

public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken OpenParenToken { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken CloseParenToken { get; }

    internal ParenthesizedExpressionSyntax(SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        : base(SyntaxKind.ParenthesizedExpression, FullSpanBetween(openParenToken, closeParenToken))
    {
        OpenParenToken = openParenToken;
        Expression = expression;
        CloseParenToken = closeParenToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return OpenParenToken;
        yield return Expression;
        yield return CloseParenToken;
    }
}

public sealed class PrefixUnaryExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Operand { get; }

    internal PrefixUnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        : base(SyntaxKind.PrefixUnaryExpression, FullSpanBetween(operatorToken, operand))
    {
        OperatorToken = operatorToken;
        Operand = operand;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    internal BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        : base(SyntaxKind.BinaryExpression, FullSpanBetween(left, right))
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}

public sealed class CastExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Expression { get; }
    public SyntaxToken AsKeyword { get; }
    public TypeSyntax Type { get; }

    internal CastExpressionSyntax(ExpressionSyntax expression, SyntaxToken asKeyword, TypeSyntax type)
        : base(SyntaxKind.CastExpression, FullSpanBetween(expression, type))
    {
        Expression = expression;
        AsKeyword = asKeyword;
        Type = type;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Expression;
        yield return AsKeyword;
        yield return Type;
    }
}

public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Expression { get; }
    public SyntaxToken DotToken { get; }
    public SyntaxToken Name { get; }

    internal MemberAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken dotToken, SyntaxToken name)
        : base(SyntaxKind.MemberAccessExpression, FullSpanBetween(expression, name))
    {
        Expression = expression;
        DotToken = dotToken;
        Name = name;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Expression;
        yield return DotToken;
        yield return Name;
    }
}

public sealed class ElementAccessExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Expression { get; }
    public SyntaxToken OpenBracketToken { get; }
    public ExpressionSyntax Index { get; }
    public SyntaxToken CloseBracketToken { get; }

    internal ElementAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken openBracketToken, ExpressionSyntax index, SyntaxToken closeBracketToken)
        : base(SyntaxKind.ElementAccessExpression, FullSpanBetween(expression, closeBracketToken))
    {
        Expression = expression;
        OpenBracketToken = openBracketToken;
        Index = index;
        CloseBracketToken = closeBracketToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Expression;
        yield return OpenBracketToken;
        yield return Index;
        yield return CloseBracketToken;
    }
}

public sealed class InvocationExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Expression { get; }
    public ArgumentListSyntax ArgumentList { get; }

    internal InvocationExpressionSyntax(ExpressionSyntax expression, ArgumentListSyntax argumentList)
        : base(SyntaxKind.InvocationExpression, FullSpanBetween(expression, argumentList))
    {
        Expression = expression;
        ArgumentList = argumentList;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Expression;
        yield return ArgumentList;
    }
}

public sealed class ArgumentListSyntax : PapyrusSyntaxNode
{
    public SyntaxToken OpenParenToken { get; }
    public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }
    public SyntaxToken CloseParenToken { get; }

    internal ArgumentListSyntax(SyntaxToken openParenToken, SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeParenToken)
        : base(SyntaxKind.ArgumentList, FullSpanBetween(openParenToken, closeParenToken))
    {
        OpenParenToken = openParenToken;
        Arguments = arguments;
        CloseParenToken = closeParenToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return OpenParenToken;
        for (var i = 0; i < Arguments.Count; i++)
        {
            yield return Arguments[i];
            if (i < Arguments.SeparatorCount) yield return Arguments.GetSeparator(i);
        }
        yield return CloseParenToken;
    }
}

public sealed class ArgumentSyntax : PapyrusSyntaxNode
{
    public SyntaxToken? Name { get; }
    public SyntaxToken? EqualsToken { get; }
    public ExpressionSyntax Expression { get; }

    internal ArgumentSyntax(SyntaxToken? name, SyntaxToken? equalsToken, ExpressionSyntax expression)
        : base(SyntaxKind.Argument, FullSpanBetween(FirstOf(name, equalsToken, expression), expression))
    {
        Name = name;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        if (Name != null) yield return Name;
        if (EqualsToken != null) yield return EqualsToken;
        yield return Expression;
    }
}

public sealed class ArrayCreationExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken NewKeyword { get; }
    public TypeSyntax ElementType { get; }
    public SyntaxToken OpenBracketToken { get; }
    public ExpressionSyntax SizeExpression { get; }
    public SyntaxToken CloseBracketToken { get; }

    internal ArrayCreationExpressionSyntax(
        SyntaxToken newKeyword,
        TypeSyntax elementType,
        SyntaxToken openBracketToken,
        ExpressionSyntax sizeExpression,
        SyntaxToken closeBracketToken)
        : base(SyntaxKind.ArrayCreationExpression, FullSpanBetween(newKeyword, closeBracketToken))
    {
        NewKeyword = newKeyword;
        ElementType = elementType;
        OpenBracketToken = openBracketToken;
        SizeExpression = sizeExpression;
        CloseBracketToken = closeBracketToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return NewKeyword;
        yield return ElementType;
        yield return OpenBracketToken;
        yield return SizeExpression;
        yield return CloseBracketToken;
    }
}

public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    internal AssignmentExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        : base(SyntaxKind.AssignmentExpression, FullSpanBetween(left, right))
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}
