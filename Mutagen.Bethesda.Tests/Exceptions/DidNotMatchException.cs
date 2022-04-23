using Loqui;
using Noggog;

namespace Mutagen.Bethesda.Tests;

public class DidNotMatchException : Exception, IPrintable
{
    public readonly string Path;
    public readonly RangeInt64[] Errors;
    public const int ErrorBytePrefixLen = 8;
    public readonly byte[][] ErrorBytes;

    public DidNotMatchException(string path, RangeInt64[] errs, Stream stream)
    {
        Errors = errs;
        Path = path;
        //this.ErrorBytes = new byte[Errors.Length][];
        //for (int i = 0; i < Errors.Length; i++)
        //{
        //    var err = this.Errors[i];
        //    var pos = err.Min - ErrorBytePrefixLen;
        //    if (pos < 0)
        //    {
        //        pos = 0;
        //    }
        //    stream.Position = pos;
        //    var len = Math.Min(ErrorBytePrefixLen, err.Width);
        //    byte[] bytes = new byte[ErrorBytePrefixLen + len];
        //    stream.Read(bytes.AsSpan());
        //    this.ErrorBytes[i] = bytes;
        //}
    }

    public override string ToString()
    {
        var posStr = string.Join(" ", Errors.Select((r) =>
        {
            return r.ToString("X");
        }));
        return $"{Path} Bytes did not match at positions: {posStr}";
    }

    //public static string ToDisplay(ReadOnlyMemorySlice<byte> bytes)
    //{
    //    return BinaryStringUtility.ToZString(bytes);
    //}

    public void ToString(FileGeneration fg, string name)
    {
        fg.AppendLine($"{Path} Bytes did not match at positions:");
        using (new DepthWrapper(fg))
        {
            for (int i = 0; i < Errors.Length; i++)
            {
                var err = Errors[i];
                fg.AppendLine($"{err.ToString("X")}");
                //using (new DepthWrapper(fg))
                //{
                //    var bytes = this.ErrorBytes[i];
                //    fg.AppendLine($"--->{ToDisplay(bytes)}<---");
                //}
            }
        }
    }
}