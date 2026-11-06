using Mutagen.Bethesda.Papyrus.Syntax.Lexer;
namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

/// <summary>
/// A token in the syntax tree, carrying its text, exact span, and attached trivia.
/// </summary>
public sealed class SyntaxToken : PapyrusSyntaxNode
{
    public string Text { get; }
    public bool IsMissing { get; }

    private readonly LexedTrivia[] _leading;
    private readonly LexedTrivia[] _trailing;
    private readonly int _leadingWidth;
    private readonly int _trailingWidth;

    internal SyntaxToken(
        SyntaxKind kind,
        string text,
        TextSpan fullSpan,
        LexedTrivia[] leading,
        LexedTrivia[] trailing,
        int leadingWidth,
        int trailingWidth,
        bool isMissing = false)
        : base(kind, fullSpan)
    {
        Text = text;
        IsMissing = isMissing;
        _leading = leading;
        _trailing = trailing;
        _leadingWidth = leadingWidth;
        _trailingWidth = trailingWidth;
    }

    public override int LeadingTriviaWidth => _leadingWidth;
    public override int TrailingTriviaWidth => _trailingWidth;

    public override IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens() => [];

    public IEnumerable<SyntaxTrivia> LeadingTrivia
    {
        get
        {
            var position = FullSpan.Start;
            foreach (var t in _leading)
            {
                yield return new SyntaxTrivia(t.Kind, t.Text, new TextSpan(position, t.Width));
                position += t.Width;
            }
        }
    }

    public IEnumerable<SyntaxTrivia> TrailingTrivia
    {
        get
        {
            var position = FullSpan.End - _trailingWidth;
            foreach (var t in _trailing)
            {
                yield return new SyntaxTrivia(t.Kind, t.Text, new TextSpan(position, t.Width));
                position += t.Width;
            }
        }
    }

    internal static SyntaxToken Missing(SyntaxKind kind, int position) => new(
        kind,
        string.Empty,
        new TextSpan(position, 0),
        Array.Empty<LexedTrivia>(),
        Array.Empty<LexedTrivia>(),
        leadingWidth: 0,
        trailingWidth: 0,
        isMissing: true);
}

/// <summary>
/// A piece of trivia (whitespace, comment) with its absolute span in the source.
/// </summary>
public readonly struct SyntaxTrivia
{
    public SyntaxKind Kind { get; }
    public string? Text { get; }
    public TextSpan Span { get; }

    internal SyntaxTrivia(SyntaxKind kind, string? text, TextSpan span)
    {
        Kind = kind;
        Text = text;
        Span = span;
    }

    public override string ToString() => Text ?? Kind.ToString();
}
