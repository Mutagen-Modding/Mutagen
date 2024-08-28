using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Parameters;

/// <summary>
/// Parameter object for customizing binary export job instructions
/// </summary>
public sealed record BinaryWriteParameters
{
    public static BinaryWriteParameters Default => new();

    /// <summary>
    /// Flag to specify what logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    public ModKeyOption ModKey = ModKeyOption.ThrowIfMisaligned;

    /// <summary>
    /// Logic to use to keep a mod's master list content in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    public AMastersListContentOption MastersListContent = MastersListContentOption.Iterate;

    /// <summary>
    /// Specifies a list of masters to include if they are not included naturally
    /// </summary>
    public IEnumerable<ModKey>? ExtraIncludeMasters { get; init; }

    /// <summary>
    /// Specifies a list of masters to override 
    /// </summary>
    public IEnumerable<ModKey>? OverrideMasters { get; init; }

    /// <summary>
    /// Logic to use to keep a mod's record count in sync
    /// </summary>
    public RecordCountOption RecordCount = RecordCountOption.Iterate;

    /// <summary>
    /// Logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content
    /// </summary>
    public AMastersListOrderingOption MastersListOrdering { get; init; } = new MastersListOrderingEnumOption();

    /// <summary>
    /// Logic to use to locate a mod header's next formID to use based on the record content
    /// </summary>
    public NextFormIDOption NextFormID { get; init; } = NextFormIDOption.Iterate;

    /// <summary>
    /// Logic to use to determine minimum allowed FormID.  Null is default behavior, which checks header version
    /// </summary>
    public AMinimumFormIdOption MinimumFormID { get; init; } = new AutomaticLowerFormIdRangeOption();

    /// <summary>
    /// Logic to use to ensure a mod's formIDs are unique.<br/>
    /// If there is a collision, an ArgumentException will be thrown.
    /// </summary>
    public FormIDUniquenessOption FormIDUniqueness { get; init; } = FormIDUniquenessOption.Iterate;

    /// <summary>
    /// Logic to use to ensure a mod's formIDs are compacted according to a mod's header flags.<br/>
    /// If there is a record outside the allowed setting, an ArgumentException will be thrown.
    /// </summary>
    public FormIDCompactionOption FormIDCompaction { get; init; } = FormIDCompactionOption.Iterate;

    /// <summary>
    /// Optional StringsWriter override, for mods that are able to localize.
    /// </summary>
    public StringsWriter? StringsWriter { get; init; }
        
    /// <summary>
    /// If writing a localizable mod that has localization off, which language to output as the embedded strings.
    /// If left null, each individual TranslatedString will use its current TargetLanguage
    /// </summary>
    public Language? TargetLanguageOverride { get; init; }

    /// <summary>
    /// If a FormID has all zeros for the ID, but a non-zero mod index, then set mod index to zero as well.
    /// </summary>
    public bool CleanNulls { get; init; } = true;

    /// <summary>
    /// Encoding overrides to use for embedded strings
    /// </summary>
    public EncodingBundle? Encodings { get; init; }

    /// <summary>
    /// A class representing how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present.
    /// By default
    /// </summary>
    public ALowerRangeDisallowedHandlerOption LowerRangeDisallowedHandler { get; init; } = new AddPlaceholderMasterIfLowerRangeDisallowed();

    /// <summary>
    /// Load order.  Required for games with Separated Load Order lists per master type
    /// </summary>
    public ILoadOrderGetter<IModFlagsGetter>? LoadOrder { get; init; }

    /// <summary>
    /// Whether to use multithreading when possible
    /// </summary>
    public ParallelWriteParameters Parallel { get; init; } = new();
    
    /// <summary>
    /// FileSystem to write to
    /// </summary>
    public IFileSystem? FileSystem { get; init; }

    /// <summary>
    /// Logic to use to ensure a mod overridden forms list is accurate
    /// </summary>
    public OverriddenFormsOption OverriddenFormsOption { get; init; } = OverriddenFormsOption.NoCheck;

    /// <summary>
    /// Aligns a mod's ModKey to a path's implied ModKey.
    /// Will adjust its logic based on the MasterFlagSync option:
    ///  - ThrowIfMisaligned:  If the path and mod do not match, throw.
    ///  - CorrectToPath:  If the path and mod do not match, use path's key.
    /// </summary>
    /// <param name="mod">Mod to check and adjust</param>
    /// <param name="path">Path to compare to</param>
    /// <returns>ModKey to use</returns>
    /// <exception cref="ArgumentException">If misaligned and set to ThrowIfMisaligned</exception>
    public ModKey RunMasterMatch(IModGetter mod, FilePath path)
    {
        if (ModKey == ModKeyOption.NoCheck) return mod.ModKey;
        if (!Plugins.ModKey.TryFromNameAndExtension(path.Name, out var pathModKey))
        {
            throw new ArgumentException($"Could not convert path to a ModKey to compare against: {Path.GetFileName(path)}");
        }
        switch (ModKey)
        {
            case ModKeyOption.ThrowIfMisaligned:
                if (mod.ModKey != pathModKey)
                {
                    throw new ArgumentException($"ModKeys were misaligned: {mod.ModKey} != {pathModKey}.  " +
                                                $"Export to a file that matches the mod object's ModKey, or " +
                                                $"modify your {nameof(BinaryWriteParameters)}.{nameof(ModKey)} parameters " +
                                                $"to override this behavior.");
                }
                return mod.ModKey;
            case ModKeyOption.CorrectToPath:
                return pathModKey;
            default:
                throw new NotImplementedException();
        }
    }
}