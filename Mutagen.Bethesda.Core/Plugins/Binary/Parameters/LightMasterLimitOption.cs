namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Flag to specify what logic to use to ensure a light master mod does not go over its FormID count
/// </summary>
public enum LightMasterLimitOption
{
    /// <summary>
    /// Do no check
    /// </summary>
    NoCheck,
    /// <summary>
    /// Will throw an exception if a light master 
    /// </summary>
    ExceptionOnOverflow,
}