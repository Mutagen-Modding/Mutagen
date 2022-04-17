namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to keep a mod header's next formID field in sync
/// </summary>
public enum NextFormIDOption
{
    /// <summary>
    /// Do no check
    /// </summary>
    NoCheck,

    /// <summary>
    /// Iterate source mod
    /// </summary>
    Iterate,
}