namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to keep a mod's master list keys in order<br/>
/// This setting is just used to sync the order of the list, not the content
/// </summary>
public enum MastersListOrderingOption
{
    /// <summary>
    /// Do no check
    /// </summary>
    NoCheck,

    /// <summary>
    /// Simply confirms masters come before other mod types
    /// </summary>
    MastersFirst,
}