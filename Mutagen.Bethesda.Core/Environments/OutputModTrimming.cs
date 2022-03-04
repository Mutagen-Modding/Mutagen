namespace Mutagen.Bethesda.Environments;

public enum OutputModTrimming
{
    /// <summary>
    /// Do no trimming
    /// </summary>
    NoTrimming,
    /// <summary>
    /// Trim mod with the same FormKey from being read from disk as an input mod
    /// </summary>
    Self,
    /// <summary>
    /// Trim mod with the same FormKey from being read from disk as an input mod.
    /// Additionally, any mod found to be after the given output mod in the load order will be excluded
    /// </summary>
    SelfAndPast
}
