namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to ensure no record has a null FormKey of its own
/// </summary>
public enum NullFormKeyOption
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
