using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Tests;

public class UnexpectedlyMoreData : Exception, IPrintable
{
    public readonly string Path;
    public readonly long Position;

    public UnexpectedlyMoreData(string path, long pos)
    {
        Position = pos;
        Path = path;
    }

    public override string ToString()
    {
        return $"{Path} had more data past position 0x{Position} than source stream.";
    }

    public void Print(StructuredStringBuilder sb, string name)
    {
        sb.AppendLine(Path);
        using (sb.IncreaseDepth())
        {
            sb.AppendLine($"had more data past position 0x{Position} than source stream.");
        }
    }
}