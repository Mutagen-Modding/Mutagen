using System.Collections;
namespace Mutagen.Bethesda.Papyrus.Syntax.Parser;

/// <summary>An ordered list of child nodes, owned by a parent node.</summary>
public sealed class SyntaxList<TNode> : IReadOnlyList<TNode>
    where TNode : PapyrusSyntaxNode
{
    public static SyntaxList<TNode> Empty { get; } = [with([])];

    private readonly TNode[] _nodes;

    internal SyntaxList(TNode[] nodes) => _nodes = nodes;

    public int Count => _nodes.Length;
    public TNode this[int index] => _nodes[index];

    public Enumerator GetEnumerator() => new(_nodes);

    IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => ((IEnumerable<TNode>)_nodes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TNode>)_nodes).GetEnumerator();

    public struct Enumerator(TNode[] nodes)
    {
        private int _index = -1;

        public readonly TNode Current => nodes[_index];
        public bool MoveNext() => ++_index < nodes.Length;
    }
}

/// <summary>An ordered list of tokens, owned by a parent node.</summary>
public sealed class SyntaxTokenList : IReadOnlyList<SyntaxToken>
{
    public static SyntaxTokenList Empty { get; } = [with([])];

    private readonly SyntaxToken[] _tokens;

    internal SyntaxTokenList(SyntaxToken[] tokens) => _tokens = tokens;

    public int Count => _tokens.Length;
    public SyntaxToken this[int index] => _tokens[index];

    public Enumerator GetEnumerator() => new(_tokens);

    IEnumerator<SyntaxToken> IEnumerable<SyntaxToken>.GetEnumerator() => ((IEnumerable<SyntaxToken>)_tokens).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<SyntaxToken>)_tokens).GetEnumerator();

    public struct Enumerator(SyntaxToken[] tokens)
    {
        private int _index = -1;

        public readonly SyntaxToken Current => tokens[_index];
        public bool MoveNext() => ++_index < tokens.Length;
    }
}

/// <summary>A list of elements separated by tokens (e.g. parameters separated by commas).</summary>
public sealed class SeparatedSyntaxList<TNode> : IReadOnlyList<TNode>
    where TNode : PapyrusSyntaxNode
{
    public static SeparatedSyntaxList<TNode> Empty { get; } = [with([], [])];

    private readonly TNode[] _nodes;
    private readonly SyntaxToken[] _separators;

    internal SeparatedSyntaxList(TNode[] nodes, SyntaxToken[] separators)
    {
        _nodes = nodes;
        _separators = separators;
    }

    public int Count => _nodes.Length;
    public TNode this[int index] => _nodes[index];
    public int SeparatorCount => _separators.Length;

    public SyntaxToken GetSeparator(int index) => _separators[index];

    public Enumerator GetEnumerator() => new(_nodes);

    IEnumerator<TNode> IEnumerable<TNode>.GetEnumerator() => ((IEnumerable<TNode>)_nodes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TNode>)_nodes).GetEnumerator();

    public struct Enumerator(TNode[] nodes)
    {
        private int _index = -1;

        public readonly TNode Current => nodes[_index];
        public bool MoveNext() => ++_index < nodes.Length;
    }
}
