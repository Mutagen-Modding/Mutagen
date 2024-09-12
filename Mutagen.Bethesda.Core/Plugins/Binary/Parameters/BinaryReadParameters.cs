using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Parameter object for customizing binary import job instructions
/// </summary>
public record BinaryReadParameters
{
    public static BinaryReadParameters Default => new();
    
    public StringsReadParameters? StringsParam { get; init; }
    
    /// <summary>
    /// Required for games with Separated Load Order lists per master type
    /// </summary>
    public ILoadOrderGetter<IModMasterStyled>? MasterFlagsLookup { get; init; }

    /// <summary>
    /// Whether to use multithreading when possible
    /// </summary>
    public bool Parallel { get; init; } = true;
    
    /// <summary>
    /// Whether to treat unknown records as errors and throw an exception
    /// </summary>
    public bool ThrowOnUnknownSubrecord { get; init; } = false;
    
    /// <summary>
    /// FileSystem to read from
    /// </summary>
    public IFileSystem? FileSystem { get; init; }
}