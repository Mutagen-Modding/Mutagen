namespace Mutagen.Bethesda.Plugins.Records;

public interface IModFlagsGetter : IModMasterFlagsGetter, IModKeyed
{
    /// <summary>
    /// Whether a mod supports localization features
    /// </summary>
    bool CanUseLocalization { get; }

    /// <summary>
    /// Whether a mod has localization enabled
    /// </summary>
    bool UsingLocalization { get; }
    
    /// <summary>
    /// Whether a mod lists overridden forms in its header
    /// </summary>
    bool ListsOverriddenForms { get; }
}

public interface IModMasterFlagsGetter : IModKeyed
{
    /// <summary>
    /// Whether a mod supports Small Master features
    /// </summary>
    bool CanBeSmallMaster { get; }

    /// <summary>
    /// Whether a mod has Small Master flag enabled
    /// </summary>
    bool IsSmallMaster { get; }
    
    /// <summary>
    /// Whether a mod supports Medium Master features
    /// </summary>
    bool CanBeMediumMaster { get; }

    /// <summary>
    /// Whether a mod has Medium Master flag enabled
    /// </summary>
    bool IsMediumMaster { get; }

    /// <summary>
    /// Whether a mod has Master flag enabled
    /// </summary>
    bool IsMaster { get; }
}

public record ModFlags : IModFlagsGetter
{
    public ModKey ModKey { get; init; }
    public bool CanUseLocalization { get; init; }
    public bool UsingLocalization { get; init; }
    public bool CanBeSmallMaster { get; init; }
    public bool IsSmallMaster { get; init; }
    public bool CanBeMediumMaster { get; init; }
    public bool IsMediumMaster { get; init; }
    public bool IsMaster { get; init; }
    public bool ListsOverriddenForms { get; init; }

    public ModFlags(ModKey modKey)
    {
        ModKey = modKey;
    }

    public ModFlags(IModFlagsGetter flags)
    {
        ModKey = flags.ModKey;
        CanUseLocalization = flags.CanUseLocalization;
        UsingLocalization = flags.UsingLocalization;
        CanBeSmallMaster = flags.CanBeSmallMaster;
        IsSmallMaster = flags.IsSmallMaster;
        CanBeMediumMaster = flags.CanBeMediumMaster;
        IsMediumMaster = flags.IsMediumMaster;
        IsMaster = flags.IsMaster;
        ListsOverriddenForms = flags.ListsOverriddenForms;
    }
}