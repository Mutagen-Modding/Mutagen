namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to ensure a mod's overridden form list in the mod header is accurate
/// </summary>
public enum OverriddenFormsOption
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