using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

/// <summary>
/// Class containing all the extra meta bits for writing
/// </summary>
public sealed record WritingBundle(GameConstants Constants)
{
    /// <summary>
    /// Game constants meta object to reference for header length measurements
    /// </summary>
    public GameConstants Constants { get; } = Constants;

    /// <summary>
    /// Optional master references for easy access during write operations
    /// </summary>
    public IReadOnlyMasterReferenceCollection? MasterReferences { get; set; }

    /// <summary>
    /// Optional strings writer for easy access during write operations
    /// </summary>
    public StringsWriter? StringsWriter { get; set; }

    /// <summary>
    /// Optional RecordInfoCache to reference while reading
    /// </summary>
    public RecordTypeInfoCacheReader? RecordInfoCache { get; set; }

    /// <summary>
    /// Tracker of current major record version
    /// </summary>
    public ushort? FormVersion { get; set; }

    /// <summary>
    /// If a FormID has all zeros for the ID, but a non-zero mod index, then set mod index to zero as well.
    /// </summary>
    public bool CleanNulls { get; set; } = true;
    
    public Language? TargetLanguageOverride { get; set; }

    public EncodingBundle Encodings { get; set; } = Constants.Encodings;
}