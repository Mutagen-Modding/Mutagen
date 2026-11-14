namespace Mutagen.Bethesda.Papyrus.Syntax.Lexer;

/// <summary>
/// A piece of non-token source content (whitespace, comments) attached to a token.
/// Text is only materialized for comments; whitespace and end-of-line trivia are
/// reconstructed from the source span when needed. Positions are not stored: the
/// owning token's full span plus the trivia widths determine them, which also lets
/// trivia arrays be interned by content.
/// </summary>
public readonly struct LexedTrivia(SyntaxKind kind, int width, string? text)
{
    public SyntaxKind Kind { get; } = kind;
    public int Width { get; } = width;
    public string? Text { get; } = text;

    public override string ToString() => Text ?? Kind.ToString();
}
