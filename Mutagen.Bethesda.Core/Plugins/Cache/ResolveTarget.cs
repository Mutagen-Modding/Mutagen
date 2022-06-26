namespace Mutagen.Bethesda.Plugins.Cache;

/// <summary>
/// What target to look up during a resolution
/// </summary>
public enum ResolveTarget
{
    /// <summary>
    /// Locate the winning override
    /// </summary>
    Winner,
        
    /// <summary>
    /// Locate the original definition as it was initially defined
    /// </summary>
    Origin
}