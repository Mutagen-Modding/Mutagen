namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to ensure a mod's formIDs are compacted according to the
/// flags set in the mod's header.
/// </summary>
public enum FormIDCompactionOption
{
    /// <summary>
    /// Do no check
    /// </summary>
    NoCheck,

    /// <summary>
    /// Iterate source mod, and throw if compaction header flags do not allow contents
    /// </summary>
    Iterate,
}