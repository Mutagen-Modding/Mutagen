using System.IO.Abstractions;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

public record BinaryReadParameters
{
    public static BinaryReadParameters Default => new();
    public StringsReadParameters? StringsParam { get; init; }
    public bool Parallel { get; init; } = true;
    public bool ThrowOnUnknownSubrecord { get; init; } = false;
    public IFileSystem? FileSystem { get; init; }
}