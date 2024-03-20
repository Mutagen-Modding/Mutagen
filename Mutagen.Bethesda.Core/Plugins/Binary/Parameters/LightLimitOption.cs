namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to ensure a light mod does not go over its FormID count
/// </summary>
public enum LightLimitOption
{
    /// <summary>
    /// Do no check
    /// </summary>
    NoCheck,
    /// <summary>
    /// Will throw an exception if a light mod goes over its limit 
    /// </summary>
    ExceptionOnOverflow,
}