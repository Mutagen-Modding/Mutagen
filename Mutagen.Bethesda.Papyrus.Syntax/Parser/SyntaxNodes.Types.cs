namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

public abstract class TypeSyntax(SyntaxKind kind, TextSpan fullSpan) : PapyrusSyntaxNode(kind, fullSpan);

public sealed class PredefinedTypeSyntax : TypeSyntax
{
    public SyntaxToken Token { get; }

    internal PredefinedTypeSyntax(SyntaxToken token)
        : base(SyntaxKind.PredefinedType, token.FullSpan)
    {
        Token = token;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Token;
    }
}

public sealed class NamedTypeSyntax : TypeSyntax
{
    public SyntaxToken Identifier { get; }

    internal NamedTypeSyntax(SyntaxToken identifier)
        : base(SyntaxKind.NamedType, identifier.FullSpan)
    {
        Identifier = identifier;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return Identifier;
    }
}

public sealed class ArrayTypeSyntax : TypeSyntax
{
    public TypeSyntax ElementType { get; }
    public SyntaxToken OpenBracketToken { get; }
    public SyntaxToken CloseBracketToken { get; }

    internal ArrayTypeSyntax(TypeSyntax elementType, SyntaxToken openBracketToken, SyntaxToken closeBracketToken)
        : base(SyntaxKind.ArrayType, FullSpanBetween(elementType, closeBracketToken))
    {
        ElementType = elementType;
        OpenBracketToken = openBracketToken;
        CloseBracketToken = closeBracketToken;
    }

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens()
    {
        yield return ElementType;
        yield return OpenBracketToken;
        yield return CloseBracketToken;
    }
}
