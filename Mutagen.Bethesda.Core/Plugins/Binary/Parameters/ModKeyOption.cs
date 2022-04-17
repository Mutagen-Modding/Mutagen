namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to keep a mod's ModKey in sync with its path
/// </summary>
public enum ModKeyOption
{
    /// <summary>
    /// Do no check
    /// </summary>
    NoCheck,

    /// <summary>
    /// If a mod's master flag does not match the path being exported to, throw
    /// </summary>
    ThrowIfMisaligned,

    /// <summary>
    /// If a mod's master flag does not match the path being exported to, modify it to match the path
    /// </summary>
    CorrectToPath,
}