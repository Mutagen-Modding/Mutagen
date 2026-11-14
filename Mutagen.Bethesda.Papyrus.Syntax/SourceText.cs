namespace Mutagen.Bethesda.Papyrus.Syntax;

/// <summary>
/// The text of a single Papyrus source file.
/// </summary>
public sealed class SourceText(string text, string? filePath = null)
{
    public string Text { get; } = text;
    public string? FilePath { get; } = filePath;
    public int Length => Text.Length;

    public static SourceText From(string text, string? filePath = null) => new(text, filePath);

    public ReadOnlySpan<char> AsSpan() => Text.AsSpan();

    public ReadOnlySpan<char> AsSpan(int start, int length) => Text.AsSpan(start, length);

    public override string ToString() => Text;
}
