namespace Mutagen.Bethesda.Papyrus.Syntax;

/// <summary>
/// A span of text within a <see cref="SourceText"/>, expressed as a start offset and length.
/// </summary>
public readonly record struct TextSpan(int Start, int Length)
{
    public int End => Start + Length;

    public static TextSpan FromBounds(int start, int end) => new(start, end - start);

    public override string ToString() => $"[{Start}..{End})";
}
