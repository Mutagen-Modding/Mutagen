namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

/// <summary>
/// Base class of all syntax tree nodes and tokens. Nodes are immutable once built;
/// <see cref="Parent"/> is assigned by the tree after parsing, and the owning tree is
/// derived by walking to the root — no per-node reference is stored.
/// </summary>
public abstract class PapyrusSyntaxNode(SyntaxKind kind, TextSpan fullSpan)
{
    public SyntaxKind Kind { get; } = kind;

    /// <summary>The tree this node belongs to; derived from the root, which stores it.</summary>
    public virtual SyntaxTree? SyntaxTree => Parent?.SyntaxTree;

    public PapyrusSyntaxNode? Parent { get; internal set; }
    public TextSpan FullSpan { get; } = fullSpan;

    /// <summary>The span of this node without its leading and trailing trivia.</summary>
    public TextSpan Span => TextSpan.FromBounds(FullSpan.Start + LeadingTriviaWidth, FullSpan.End - TrailingTriviaWidth);

    public virtual int LeadingTriviaWidth => ChildNodesAndTokens().FirstOrDefault()?.LeadingTriviaWidth ?? 0;

    public virtual int TrailingTriviaWidth => ChildNodesAndTokens().LastOrDefault()?.TrailingTriviaWidth ?? 0;

    /// <summary>The direct children of this node, tokens included, in source order.</summary>
    public abstract IEnumerable<PapyrusSyntaxNode> ChildNodesAndTokens();

    public IEnumerable<PapyrusSyntaxNode> ChildNodes() => ChildNodesAndTokens().Where(n => n is not SyntaxToken);

    public IEnumerable<PapyrusSyntaxNode> DescendantNodesAndSelf()
    {
        yield return this;
        foreach (var child in ChildNodesAndTokens())
        {
            foreach (var descendant in child.DescendantNodesAndSelf())
            {
                yield return descendant;
            }
        }
    }

    public IEnumerable<PapyrusSyntaxNode> DescendantNodes() => DescendantNodesAndSelf().Skip(1);

    public IEnumerable<PapyrusSyntaxNode> Ancestors()
    {
        var current = Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public string ToFullString() => SyntaxTree is { } tree
        ? tree.Source.Text.Substring(FullSpan.Start, FullSpan.Length)
        : string.Empty;

    public override string ToString() => SyntaxTree is { } tree
        ? tree.Source.Text.Substring(Span.Start, Span.Length)
        : string.Empty;

    protected static TextSpan FullSpanBetween(PapyrusSyntaxNode first, PapyrusSyntaxNode last) =>
        TextSpan.FromBounds(first.FullSpan.Start, last.FullSpan.End);

    /// <summary>Returns the last non-null argument; arguments must be given in source order.</summary>
    protected static PapyrusSyntaxNode LastOf(params PapyrusSyntaxNode?[] nodes)
    {
        for (var i = nodes.Length - 1; i >= 0; i--)
        {
            if (nodes[i] != null) return nodes[i]!;
        }
        throw new ArgumentException("At least one node must be non-null.");
    }

    /// <summary>Returns the first non-null argument; arguments must be given in source order.</summary>
    protected static PapyrusSyntaxNode FirstOf(params PapyrusSyntaxNode?[] nodes)
    {
        foreach (var node in nodes)
        {
            if (node != null) return node;
        }
        throw new ArgumentException("At least one node must be non-null.");
    }
}
