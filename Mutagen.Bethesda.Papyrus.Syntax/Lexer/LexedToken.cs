namespace Mutagen.Bethesda.Papyrus.Syntax.Lexer;

/// <summary>
/// A lexical token with its exact position in the source and the trivia attached to it.
/// </summary>
public readonly struct LexedToken(
    SyntaxKind kind,
    int start,
    int width,
    string text,
    LexedTrivia[] leadingTrivia,
    LexedTrivia[] trailingTrivia,
    int leadingTriviaWidth,
    int trailingTriviaWidth)
{
    public SyntaxKind Kind { get; } = kind;
    public int Start { get; } = start;
    public int Width { get; } = width;
    public int End => Start + Width;
    public string Text { get; } = text;
    public LexedTrivia[] LeadingTrivia { get; } = leadingTrivia;
    public LexedTrivia[] TrailingTrivia { get; } = trailingTrivia;
    public TextSpan Span => new(Start, Width);
    public bool IsEof => Kind == SyntaxKind.EndOfFileToken;

    public int LeadingTriviaWidth { get; } = leadingTriviaWidth;

    public int TrailingTriviaWidth { get; } = trailingTriviaWidth;

    /// <summary>Start of the token including its leading trivia.</summary>
    public int FullStart => Start - LeadingTriviaWidth;

    /// <summary>End of the token including its trailing trivia.</summary>
    public int FullEnd => Start + Width + TrailingTriviaWidth;

    public override string ToString() => Text;
}
