namespace Mutagen.Bethesda.Pex;

internal record PexWriteMeta(
    GameCategory Category,
    BinaryWriter Writer)
{
    public readonly Dictionary<string, ushort> Strings = new();
    private ushort _next;

    public ushort RegisterString(string str)
    {
        if (Strings.TryGetValue(str, out var index)) return index;
        index = _next++;
        Strings[str] = index;
        return index;
    }

    public void WriteString(string? str)
    {
        if (str == null) return;
        Writer.Write(RegisterString(str));
    }
}