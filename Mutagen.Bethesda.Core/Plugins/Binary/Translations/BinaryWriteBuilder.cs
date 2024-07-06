using System.IO.Abstractions;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

/// <summary>
/// Internal helper object to avoid relisting the same variables
/// </summary>
internal record BinaryWriteBuilderParams<TModGetter>
    where TModGetter : class, IModGetter
{
    internal IBinaryWriteBuilderWriter<TModGetter> _writer { get; init; } = null!;
    internal BinaryWriteParameters _param { get; init; } = BinaryWriteParameters.Default;
    internal FilePath? _path { get; init; }
    internal Stream? _stream { get; init; }
    internal Func<TModGetter, BinaryWriteParameters, BinaryWriteParameters>? _loadOrderSetter { get; init; }
}

/// <summary>
/// Internal helper object to pass instructions for how to write a mod once the builder is ready
/// </summary>
internal interface IBinaryWriteBuilderWriter<TModGetter>
    where TModGetter : class, IModGetter
{
    Task WriteAsync(TModGetter mod, BinaryWriteBuilderParams<TModGetter> param);
    void Write(TModGetter mod, BinaryWriteBuilderParams<TModGetter> param);
}

public interface IBinaryModdedWriteBuilderLoadOrderChoice
{
    /// <summary>
    /// Writes the mod with no load order as reference.  Avoid if possible.<br />
    /// NOTE:  This will have the following negative consequences: <br />
    /// - For games with seperated master concepts (Starfield), this will cause potential corruption
    /// if any light or medium flagged mods are referenced. <br />
    /// - Masters will be unordered and may not match the load order the mod is eventually run with
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderTargetChoice WithNoLoadOrder();

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderTargetChoice WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<IModFlagsGetter>> loadOrder);

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderTargetChoice WithLoadOrder(
        ILoadOrderGetter<IModFlagsGetter> loadOrder);

    /// <summary>
    /// Writes the mod with the default load order and data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderTargetChoice WithDefaultLoadOrder();

    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="dataFolder">Data folder path to use when reading load order</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderTargetChoice WithLoadOrder(
        DirectoryPath dataFolder);

    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="dataFolder">Data folder path to use when reading load order</param>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderTargetChoice WithLoadOrder(
        DirectoryPath dataFolder,
        IEnumerable<ModKey> loadOrder);
}

public record BinaryModdedWriteBuilderLoadOrderChoice<TModGetter> : IBinaryModdedWriteBuilderLoadOrderChoice
    where TModGetter : class, IModGetter
{
    internal TModGetter _mod { get; init; } = null!;
    internal BinaryWriteBuilderParams<TModGetter> _params;

    internal BinaryModdedWriteBuilderLoadOrderChoice(
        TModGetter mod,
        IBinaryWriteBuilderWriter<TModGetter> writer)
    {
        _mod = mod;
        _params = new BinaryWriteBuilderParams<TModGetter>()
        {
            _writer = writer,
        };
    }

    /// <summary>
    /// Writes the mod with no load order as reference.  Avoid if possible.<br />
    /// NOTE:  This will have the following negative consequences: <br />
    /// - For games with seperated master concepts (Starfield), this will cause potential corruption
    /// if any light or medium flagged mods are referenced. <br />
    /// - Masters will be unordered and may not match the load order the mod is eventually run with
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderTargetChoice<TModGetter> WithNoLoadOrder()
    {
        return new BinaryModdedWriteBuilderTargetChoice<TModGetter>(_mod, _params);
    }
    IBinaryModdedWriteBuilderTargetChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithNoLoadOrder() => WithNoLoadOrder(); 
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<IModFlagsGetter>> loadOrder)
    {
        return new BinaryModdedWriteBuilderTargetChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p) =>
            {
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        loadOrder.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                };
            }
        });
    }
    IBinaryModdedWriteBuilderTargetChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(ILoadOrderGetter<IModListingGetter<IModFlagsGetter>> loadOrder) => WithLoadOrder(loadOrder);

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModFlagsGetter> loadOrder)
    {
        return new BinaryModdedWriteBuilderTargetChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p) =>
            {
                return p with
                {
                    LoadOrder = loadOrder,
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                };
            }
        });
    }
    IBinaryModdedWriteBuilderTargetChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(ILoadOrderGetter<IModFlagsGetter> loadOrder) => WithLoadOrder(loadOrder);

    /// <summary>
    /// Writes the mod with the default load order and data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderTargetChoice<TModGetter> WithDefaultLoadOrder()
    {
        return new BinaryModdedWriteBuilderTargetChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p) =>
            {
                var dataFolder = GameLocator.Instance.GetDataDirectory(m.GameRelease);
                var lo = LoadOrder.Import<TModGetter>(dataFolder, m.GameRelease, _params._param.FileSystem);   
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        lo.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
    IBinaryModdedWriteBuilderTargetChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithDefaultLoadOrder() => WithDefaultLoadOrder();
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="dataFolder">Data folder path to use when reading load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        DirectoryPath dataFolder)
    {
        return new BinaryModdedWriteBuilderTargetChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p) =>
            {
                var lo = LoadOrder.Import<TModGetter>(dataFolder, m.GameRelease, _params._param.FileSystem);   
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        lo.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
    IBinaryModdedWriteBuilderTargetChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(DirectoryPath dataFolder) => WithLoadOrder(dataFolder);
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="dataFolder">Data folder path to use when reading load order</param>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        DirectoryPath dataFolder,
        IEnumerable<ModKey> loadOrder)
    {
        return new BinaryModdedWriteBuilderTargetChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p) =>
            {
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder, loadOrder,
                    m.GameRelease, _params._param.FileSystem);   
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        lo.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
    IBinaryModdedWriteBuilderTargetChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(DirectoryPath dataFolder, IEnumerable<ModKey> loadOrder) => WithLoadOrder(dataFolder, loadOrder);
}

public record BinaryWriteBuilderLoadOrderChoice<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;

    internal BinaryWriteBuilderLoadOrderChoice(
        IBinaryWriteBuilderWriter<TModGetter> writer)
    {
        _params = new BinaryWriteBuilderParams<TModGetter>()
        {
            _writer = writer,
        };
    }

    /// <summary>
    /// Writes the mod with no load order as reference.  Avoid if possible.<br />
    /// NOTE:  This will have the following negative consequences: <br />
    /// - For games with seperated master concepts (Starfield), this will cause potential corruption
    /// if any light or medium flagged mods are referenced. <br />
    /// - Masters will be unordered and may not match the load order the mod is eventually run with
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderTargetChoice<TModGetter> WithNoLoadOrder()
    {
        return new BinaryWriteBuilderTargetChoice<TModGetter>(_params);
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<IModFlagsGetter>> loadOrder)
    {
        return new BinaryWriteBuilderTargetChoice<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p) =>
            {
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        loadOrder.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModFlagsGetter> loadOrder)
    {
        return new BinaryWriteBuilderTargetChoice<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p) =>
            {
                return p with
                {
                    LoadOrder = loadOrder,
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                };
            }
        });
    }

    /// <summary>
    /// Writes the mod with the default load order and data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderTargetChoice<TModGetter> WithDefaultLoadOrder()
    {
        return new BinaryWriteBuilderTargetChoice<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p) =>
            {
                var dataFolder = GameLocator.Instance.GetDataDirectory(m.GameRelease);
                var lo = LoadOrder.Import<TModGetter>(dataFolder, m.GameRelease, _params._param.FileSystem);   
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        lo.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="dataFolder">Data folder path to use when reading load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        DirectoryPath dataFolder)
    {
        return new BinaryWriteBuilderTargetChoice<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p) =>
            {
                var lo = LoadOrder.Import<TModGetter>(dataFolder, m.GameRelease, _params._param.FileSystem);   
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        lo.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="dataFolder">Data folder path to use when reading load order</param>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderTargetChoice<TModGetter> WithLoadOrder(
        DirectoryPath dataFolder,
        IEnumerable<ModKey> loadOrder)
    {
        return new BinaryWriteBuilderTargetChoice<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p) =>
            {
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder, loadOrder,
                    m.GameRelease, _params._param.FileSystem);   
                return p with
                {
                    LoadOrder = new LoadOrder<IModFlagsGetter>(
                        lo.PriorityOrder.ResolveAllModsExist(),
                        disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
}

public interface IBinaryModdedWriteBuilderTargetChoice
{
    /// <summary>
    /// Instructs the builder to target a path to write the mod
    /// </summary>
    /// <param name="path">Path to the intended mod file to eventually write to</param>
    /// <param name="fileSystem">Filesystem to write mod file to</param>
    /// <returns>Builder object to continue customization</returns>
    public IFileBinaryModdedWriteBuilder ToPath(FilePath path, IFileSystem? fileSystem = null);
}

public record BinaryModdedWriteBuilderTargetChoice<TModGetter> : IBinaryModdedWriteBuilderTargetChoice
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;
    internal TModGetter _mod { get; init; } = null!;

    internal BinaryModdedWriteBuilderTargetChoice(
        TModGetter mod,
        BinaryWriteBuilderParams<TModGetter> @params)
    {
        _mod = mod;
        _params = @params;
    }

    /// <summary>
    /// Instructs the builder to target a path to write the mod
    /// </summary>
    /// <param name="path">Path to the intended mod file to eventually write to</param>
    /// <param name="fileSystem">Filesystem to write mod file to</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> ToPath(FilePath path, IFileSystem? fileSystem = null)
    {
        return new FileBinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _path = path,
            _param = _params._param with
            {
                FileSystem = fileSystem
            }
        });
    }

    IFileBinaryModdedWriteBuilder IBinaryModdedWriteBuilderTargetChoice.ToPath(FilePath path, IFileSystem? fileSystem = null) =>
        ToPath(path, fileSystem);
}

public record BinaryWriteBuilderTargetChoice<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;
    internal BinaryWriteBuilderTargetChoice(
        BinaryWriteBuilderParams<TModGetter> @params)
    {
        _params = @params;
    }

    /// <summary>
    /// Instructs the builder to target a path to write the mod
    /// </summary>
    /// <param name="path">Path to the intended mod file to eventually write to</param>
    /// <param name="fileSystem">Filesystem to write mod file to</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> ToPath(FilePath path, IFileSystem? fileSystem = null)
    {
        return new FileBinaryWriteBuilder<TModGetter>(_params with
        {
            _path = path,
            _param = _params._param with
            {
                FileSystem = fileSystem
            }
        });
    }
}

public interface IFileBinaryModdedWriteBuilder
{
    /// <summary>
    /// Adjusts the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithModKeySync(ModKeyOption option);

    /// <summary>
    /// Disables the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoModKeySync();

    /// <summary>
    /// Adjusts the filesystem to write to
    /// </summary>
    /// <param name="fileSystem">Filesystem to write to</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithFileSystem(IFileSystem fileSystem);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithMastersListContent(MastersListContentOption option);

    /// <summary>
    /// Disables logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoMastersListContentCheck();

    /// <summary>
    /// Specify logic to use to keep a mod's record count in sync
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithRecordCount(RecordCountOption option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithMastersListOrdering(
        MastersListOrderingOption option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithMastersListOrdering(
        IEnumerable<ModKey> loadOrder);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithMastersListOrdering(
        ILoadOrderGetter loadOrder);

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoNextFormIDProcessing();

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <param name="useLowerRange">Force the lower FormID range usage on or off</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithForcedLowerFormIdRangeUsage(bool useLowerRange);

    /// <summary>
    /// Turns off logic to check for FormID uniqueness.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoFormIDUniquenessCheck();

    /// <summary>
    /// Turns off logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoFormIDCompactnessCheck();

    /// <summary>
    /// Adjusts logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <param name="option">Logic to use for checking FormID compactness</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithFormIDCompactnessCheck(FormIDCompactionOption option);

    /// <summary>
    /// StringsWriter override, for mods that are able to localize.
    /// </summary>
    /// <param name="stringsWriter">StringsWriter to use when localizing</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithStringsWriter(StringsWriter stringsWriter);

    /// <summary>
    /// If writing a localizable mod that has localization off, which language to output as the embedded strings
    /// </summary>
    /// <param name="language">Language to output as the embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithTargetLanguage(Language language);

    /// <summary>
    /// Disables logic to zero out all null FormIDs
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoNullFormIDStandardization();

    /// <summary>
    /// Encoding overrides to use for embedded strings
    /// </summary>
    /// <param name="encodingBundle">Encoding overrides to use for embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithEmbeddedEncodings(EncodingBundle encodingBundle);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="placeholder">ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder);

    /// <summary>
    /// When lower formID ranges are used in a non-allowed way, set the system to throw an exception <br />
    /// Typically this occurs when the lower ranges are used without any masters present.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder ThrowIfLowerRangeDisallowed();

    /// <summary>
    /// Adjusts system to not check lower formID ranges are used in a non-allowed way
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder NoCheckIfLowerRangeDisallowed();

    /// <summary>
    /// Sets rules to be used for determining how to parallelize writing
    /// </summary>
    /// <param name="parameters">Parameters to use for parallel writing</param>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder WithParallelWriteParameters(ParallelWriteParameters parameters);

    /// <summary>
    /// Sets writing to be done on current thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IFileBinaryModdedWriteBuilder SingleThread();

    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    void Write();

    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    Task WriteAsync();
}

public record FileBinaryModdedWriteBuilder<TModGetter> : IFileBinaryModdedWriteBuilder
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;
    internal TModGetter _mod { get; init; } = null!;

    internal FileBinaryModdedWriteBuilder(
        TModGetter mod,
        BinaryWriteBuilderParams<TModGetter> @params)
    {
        _mod = mod;
        _params = @params;
    }
    
    /// <summary>
    /// Adjusts the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithModKeySync(ModKeyOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    ModKey = option
                }
            }
        };
    }

    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithModKeySync(ModKeyOption option) => WithModKeySync(option);
    
    /// <summary>
    /// Disables the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoModKeySync()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    ModKey = ModKeyOption.NoCheck
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoModKeySync() => NoModKeySync();

    /// <summary>
    /// Adjusts the filesystem to write to
    /// </summary>
    /// <param name="fileSystem">Filesystem to write to</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithFileSystem(IFileSystem fileSystem)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    FileSystem = fileSystem
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithFileSystem(IFileSystem fileSystem) => WithFileSystem(fileSystem);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithMastersListContent(MastersListContentOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListContent = option
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithMastersListContent(MastersListContentOption option) => WithMastersListContent(option);

    /// <summary>
    /// Disables logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoMastersListContentCheck()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListContent = MastersListContentOption.NoCheck
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoMastersListContentCheck() => NoMastersListContentCheck();

    /// <summary>
    /// Specify logic to use to keep a mod's record count in sync
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithRecordCount(RecordCountOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    RecordCount = option
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithRecordCount(RecordCountOption option) => WithRecordCount(option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="option">Option to use</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
        MastersListOrderingOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingEnumOption()
                    {
                        Option = option
                    }
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithMastersListOrdering(MastersListOrderingOption option) => WithMastersListOrdering(option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
        IEnumerable<ModKey> loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithMastersListOrdering(IEnumerable<ModKey> loadOrder) => WithMastersListOrdering(loadOrder);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
        ILoadOrderGetter loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithMastersListOrdering(ILoadOrderGetter loadOrder) => WithMastersListOrdering(loadOrder);

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoNextFormIDProcessing()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    NextFormID = NextFormIDOption.NoCheck
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoNextFormIDProcessing() => NoNextFormIDProcessing();

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <param name="useLowerRange">Force the lower FormID range usage on or off</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithForcedLowerFormIdRangeUsage(bool useLowerRange)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MinimumFormID = AMinimumFormIdOption.Force(useLowerRange)
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithForcedLowerFormIdRangeUsage(bool useLowerRange) => WithForcedLowerFormIdRangeUsage(useLowerRange);

    /// <summary>
    /// Turns off logic to check for FormID uniqueness.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoFormIDUniquenessCheck()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    FormIDUniqueness = FormIDUniquenessOption.NoCheck
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoFormIDUniquenessCheck() => NoFormIDUniquenessCheck();

    /// <summary>
    /// Turns off logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoFormIDCompactnessCheck()
    {
        return WithFormIDCompactnessCheck(FormIDCompactionOption.NoCheck);
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoFormIDCompactnessCheck() => NoFormIDCompactnessCheck();

    /// <summary>
    /// Adjusts logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <param name="option">Logic to use for checking FormID compactness</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithFormIDCompactnessCheck(FormIDCompactionOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    FormIDCompaction = option
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithFormIDCompactnessCheck(FormIDCompactionOption option) => WithFormIDCompactnessCheck(option);

    /// <summary>
    /// StringsWriter override, for mods that are able to localize.
    /// </summary>
    /// <param name="stringsWriter">StringsWriter to use when localizing</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithStringsWriter(StringsWriter stringsWriter)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    StringsWriter = stringsWriter
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithStringsWriter(StringsWriter stringsWriter) => WithStringsWriter(stringsWriter);

    /// <summary>
    /// If writing a localizable mod that has localization off, which language to output as the embedded strings
    /// </summary>
    /// <param name="language">Language to output as the embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithTargetLanguage(Language language)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    TargetLanguageOverride = language
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithTargetLanguage(Language language) => WithTargetLanguage(language);

    /// <summary>
    /// Disables logic to zero out all null FormIDs
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoNullFormIDStandardization()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    CleanNulls = false
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoNullFormIDStandardization() => NoNullFormIDStandardization();

    /// <summary>
    /// Encoding overrides to use for embedded strings
    /// </summary>
    /// <param name="encodingBundle">Encoding overrides to use for embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithEmbeddedEncodings(EncodingBundle encodingBundle)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    Encodings = encodingBundle
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithEmbeddedEncodings(EncodingBundle encodingBundle) => WithEmbeddedEncodings(encodingBundle);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="placeholder">ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(placeholder)
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder) => WithPlaceholderMasterIfLowerRangeDisallowed(placeholder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder) => WithPlaceholderMasterIfLowerRangeDisallowed(loadOrder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder) => WithPlaceholderMasterIfLowerRangeDisallowed(loadOrder);

    /// <summary>
    /// When lower formID ranges are used in a non-allowed way, set the system to throw an exception <br />
    /// Typically this occurs when the lower ranges are used without any masters present.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> ThrowIfLowerRangeDisallowed()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = new ThrowIfLowerRangeDisallowed()
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.ThrowIfLowerRangeDisallowed() => ThrowIfLowerRangeDisallowed();
    
    /// <summary>
    /// Adjusts system to not check lower formID ranges are used in a non-allowed way
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> NoCheckIfLowerRangeDisallowed()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = new NoCheckIfLowerRangeDisallowed()
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.NoCheckIfLowerRangeDisallowed() => NoCheckIfLowerRangeDisallowed();

    /// <summary>
    /// Sets rules to be used for determining how to parallelize writing
    /// </summary>
    /// <param name="parameters">Parameters to use for parallel writing</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> WithParallelWriteParameters(ParallelWriteParameters parameters)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    Parallel = parameters
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.WithParallelWriteParameters(ParallelWriteParameters parameters) => WithParallelWriteParameters(parameters);
    
    /// <summary>
    /// Sets writing to be done on current thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryModdedWriteBuilder<TModGetter> SingleThread()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    Parallel = new ParallelWriteParameters()
                    {
                        MaxDegreeOfParallelism = 1
                    }
                }
            }
        };
    }
    IFileBinaryModdedWriteBuilder IFileBinaryModdedWriteBuilder.SingleThread() => SingleThread();
    
    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    public async Task WriteAsync()
    {
        if (_params._loadOrderSetter != null)
        {
            _params = _params with
            {
                _param = _params._loadOrderSetter(_mod, _params._param)
            };
        }
        await _params._writer.WriteAsync(_mod, _params);
    }
    
    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    public void Write()
    {
        if (_params._loadOrderSetter != null)
        {
            _params = _params with
            {
                _param = _params._loadOrderSetter(_mod, _params._param)
            };
        }
        _params._writer.Write(_mod, _params);
    }
}

public record FileBinaryWriteBuilder<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;

    internal FileBinaryWriteBuilder(
        BinaryWriteBuilderParams<TModGetter> @params)
    {
        _params = @params;
    }
    
    /// <summary>
    /// Adjusts the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithModKeySync(ModKeyOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    ModKey = option
                }
            }
        };
    }
    
    /// <summary>
    /// Disables the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoModKeySync()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    ModKey = ModKeyOption.NoCheck
                }
            }
        };
    }

    /// <summary>
    /// Adjusts the filesystem to write to
    /// </summary>
    /// <param name="fileSystem">Filesystem to write to</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithFileSystem(IFileSystem fileSystem)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    FileSystem = fileSystem
                }
            }
        };
    }

    /// <summary>
    /// Specify what logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithMastersListContent(MastersListContentOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListContent = option
                }
            }
        };
    }

    /// <summary>
    /// Disables logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoMastersListContentCheck()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListContent = MastersListContentOption.NoCheck
                }
            }
        };
    }

    /// <summary>
    /// Specify logic to use to keep a mod's record count in sync
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithRecordCount(RecordCountOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    RecordCount = option
                }
            }
        };
    }

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="option">Option to use</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithMastersListOrdering(
        MastersListOrderingOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingEnumOption()
                    {
                        Option = option
                    }
                }
            }
        };
    }

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithMastersListOrdering(
        IEnumerable<ModKey> loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                }
            }
        };
    }

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithMastersListOrdering(
        ILoadOrderGetter loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                }
            }
        };
    }

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoNextFormIDProcessing()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    NextFormID = NextFormIDOption.NoCheck
                }
            }
        };
    }

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <param name="useLowerRange">Force the lower FormID range usage on or off</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithForcedLowerFormIdRangeUsage(bool useLowerRange)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MinimumFormID = AMinimumFormIdOption.Force(useLowerRange)
                }
            }
        };
    }

    /// <summary>
    /// Turns off logic to check for FormID uniqueness.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoFormIDUniquenessCheck()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    FormIDUniqueness = FormIDUniquenessOption.NoCheck
                }
            }
        };
    }

    /// <summary>
    /// Turns off logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoFormIDCompactnessCheck()
    {
        return WithFormIDCompactnessCheck(FormIDCompactionOption.NoCheck);
    }

    /// <summary>
    /// Adjusts logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <param name="option">Logic to use for checking FormID compactness</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithFormIDCompactnessCheck(FormIDCompactionOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    FormIDCompaction = option
                }
            }
        };
    }

    /// <summary>
    /// StringsWriter override, for mods that are able to localize.
    /// </summary>
    /// <param name="stringsWriter">StringsWriter to use when localizing</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithStringsWriter(StringsWriter stringsWriter)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    StringsWriter = stringsWriter
                }
            }
        };
    }

    /// <summary>
    /// If writing a localizable mod that has localization off, which language to output as the embedded strings
    /// </summary>
    /// <param name="language">Language to output as the embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithTargetLanguage(Language language)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    TargetLanguageOverride = language
                }
            }
        };
    }

    /// <summary>
    /// Disables logic to zero out all null FormIDs
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoNullFormIDStandardization()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    CleanNulls = false
                }
            }
        };
    }

    /// <summary>
    /// Encoding overrides to use for embedded strings
    /// </summary>
    /// <param name="encodingBundle">Encoding overrides to use for embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithEmbeddedEncodings(EncodingBundle encodingBundle)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    Encodings = encodingBundle
                }
            }
        };
    }

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="placeholder">ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(placeholder)
                }
            }
        };
    }

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                }
            }
        };
    }

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                }
            }
        };
    }

    /// <summary>
    /// When lower formID ranges are used in a non-allowed way, set the system to throw an exception <br />
    /// Typically this occurs when the lower ranges are used without any masters present.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> ThrowIfLowerRangeDisallowed()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = new ThrowIfLowerRangeDisallowed()
                }
            }
        };
    }
    
    /// <summary>
    /// Adjusts system to not check lower formID ranges are used in a non-allowed way
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> NoCheckIfLowerRangeDisallowed()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    LowerRangeDisallowedHandler = new NoCheckIfLowerRangeDisallowed()
                }
            }
        };
    }

    /// <summary>
    /// Sets rules to be used for determining how to parallelize writing
    /// </summary>
    /// <param name="parameters">Parameters to use for parallel writing</param>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> WithParallelWriteParameters(ParallelWriteParameters parameters)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    Parallel = parameters
                }
            }
        };
    }
    
    /// <summary>
    /// Sets writing to be done on current thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public FileBinaryWriteBuilder<TModGetter> SingleThread()
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    Parallel = new ParallelWriteParameters()
                    {
                        MaxDegreeOfParallelism = 1
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Executes the instructions to write a mod.
    /// </summary>
    /// <param name="mod">Mod to write to</param>
    /// <returns>A task to await for writing completion</returns>
    public async Task WriteAsync(TModGetter mod)
    {
        if (_params._loadOrderSetter != null)
        {
            _params = _params with
            {
                _param = _params._loadOrderSetter(mod, _params._param)
            };
        }
        await _params._writer.WriteAsync(mod, _params);
    }
    
    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    public void Write(TModGetter mod)
    {
        if (_params._loadOrderSetter != null)
        {
            _params = _params with
            {
                _param = _params._loadOrderSetter(mod, _params._param)
            };
        }
        _params._writer.Write(mod, _params);
    }
}
