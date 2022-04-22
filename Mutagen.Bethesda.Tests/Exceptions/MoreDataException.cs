using Loqui;
using System;

namespace Mutagen.Bethesda.Tests;

public class MoreDataException : Exception, IPrintable
{
    public readonly string Path;
    public readonly long Position;

    public MoreDataException(string path, long pos)
    {
        this.Position = pos;
        this.Path = path;
    }

    public override string ToString()
    {
        return $"{Path} had more data past position 0x{Position:X}";
    }

    public void ToString(FileGeneration fg, string name)
    {
        fg.AppendLine(Path);
        using (new DepthWrapper(fg))
        {
            fg.AppendLine($"had more data past position 0x{Position:X}");
        }
    }
}