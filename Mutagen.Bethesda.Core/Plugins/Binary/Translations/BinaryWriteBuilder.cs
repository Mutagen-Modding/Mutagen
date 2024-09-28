using System.IO.Abstractions;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;
// ReSharper disable InconsistentNaming
// ReSharper disable WithExpressionModifiesAllMembers

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

/// <summary>
/// Internal helper object to avoid relisting the same variables
/// </summary>
internal record BinaryWriteBuilderParams<TModGetter>
    where TModGetter : class, IModGetter
{
    internal required GameRelease _gameRelease { get; init; }
    internal IBinaryWriteBuilderWriter<TModGetter> _writer { get; init; } = null!;
    internal BinaryWriteParameters _param { get; init; } = BinaryWriteParameters.Default;
    internal FilePath? _path { get; init; }
    internal Stream? _stream { get; init; }
    internal Func<TModGetter, BinaryWriteBuilderParams<TModGetter>, BinaryWriteParameters>? _masterSyncAction { get; init; }
    internal Func<TModGetter, BinaryWriteBuilderParams<TModGetter>, IReadOnlyCollection<ModKey>, BinaryWriteParameters>? _loadOrderSetter { get; init; }
    internal Func<TModGetter, BinaryWriteParameters, DirectoryPath>? _dataFolderGetter { get; init; }
    internal IModMasterStyledGetter[] KnownMasters { get; init; } = Array.Empty<IModMasterStyledGetter>();
    internal ILoadOrderGetter<IModListingGetter<TModGetter>>? _knownModLoadOrder { get; init; }
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

public interface IBinaryModdedWriteBuilderTargetChoice
{
    /// <summary>
    /// Instructs the builder to target a path to write the mod
    /// </summary>
    /// <param name="path">Path to the intended mod file to eventually write to</param>
    /// <param name="fileSystem">Filesystem to write mod file to</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderLoadOrderChoice ToPath(FilePath path, IFileSystem? fileSystem = null);
}

public record BinaryModdedWriteBuilderTargetChoice<TModGetter> : IBinaryModdedWriteBuilderTargetChoice
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;
    internal TModGetter _mod { get; init; } = null!;

    internal BinaryModdedWriteBuilderTargetChoice(
        TModGetter mod,
        IBinaryWriteBuilderWriter<TModGetter> writer)
    {
        _mod = mod;
        _params = new BinaryWriteBuilderParams<TModGetter>()
        {
            _gameRelease = mod.GameRelease,
            _writer = writer,
        };
    }

    /// <summary>
    /// Instructs the builder to target a path to write the mod
    /// </summary>
    /// <param name="path">Path to the intended mod file to eventually write to</param>
    /// <param name="fileSystem">Filesystem to write mod file to</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderLoadOrderChoice<TModGetter> ToPath(FilePath path, IFileSystem? fileSystem = null)
    {
        return new BinaryModdedWriteBuilderLoadOrderChoice<TModGetter>(_mod, _params with
        {
            _path = path,
            _param = _params._param with
            {
                FileSystem = fileSystem
            }
        });
    }

    IBinaryModdedWriteBuilderLoadOrderChoice IBinaryModdedWriteBuilderTargetChoice.ToPath(FilePath path, IFileSystem? fileSystem = null) =>
        ToPath(path, fileSystem);
}

public record BinaryWriteBuilderTargetChoice<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;
    internal BinaryWriteBuilderTargetChoice(
        GameRelease release,
        IBinaryWriteBuilderWriter<TModGetter> writer)
    {
        _params = new BinaryWriteBuilderParams<TModGetter>()
        {
            _gameRelease = release,
            _writer = writer,
        };
    }

    /// <summary>
    /// Instructs the builder to target a path to write the mod
    /// </summary>
    /// <param name="path">Path to the intended mod file to eventually write to</param>
    /// <param name="fileSystem">Filesystem to write mod file to</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderLoadOrderChoice<TModGetter> ToPath(FilePath path, IFileSystem? fileSystem = null)
    {
        return new BinaryWriteBuilderLoadOrderChoice<TModGetter>(_params with
        {
            _path = path,
            _param = _params._param with
            {
                FileSystem = fileSystem
            }
        });
    }
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
    public IBinaryModdedWriteBuilder WithNoLoadOrder();

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilder WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<IModMasterStyledGetter>> loadOrder);

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilder WithLoadOrder(
        ILoadOrderGetter<IModMasterStyledGetter> loadOrder);

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilder WithLoadOrder(
        IEnumerable<IModMasterStyledGetter> loadOrder);

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilder WithLoadOrder(
        params IModMasterStyledGetter[] loadOrder);

    /// <summary>
    /// Writes the mod with the default load order and data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilder WithDefaultLoadOrder();

    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderDataFolderChoice WithLoadOrder(
        IEnumerable<ModKey> loadOrder);

    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderDataFolderChoice WithLoadOrder(
        params ModKey[] loadOrder);

    /// <summary>
    /// Writes the mod with the load order found in mod header, and with given data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public IBinaryModdedWriteBuilderDataFolderChoice WithLoadOrderFromHeaderMasters();
}

public record BinaryModdedWriteBuilderLoadOrderChoice<TModGetter> : IBinaryModdedWriteBuilderLoadOrderChoice
    where TModGetter : class, IModGetter
{
    internal TModGetter _mod { get; init; } = null!;    
    internal BinaryWriteBuilderParams<TModGetter> _params;

    internal BinaryModdedWriteBuilderLoadOrderChoice(
        TModGetter mod,
        BinaryWriteBuilderParams<TModGetter> @params)
    {
        _mod = mod;
        _params = @params;
    }

    /// <summary>
    /// Writes the mod with no load order as reference.  Avoid if possible.<br />
    /// NOTE:  This will have the following negative consequences: <br />
    /// - For games with seperated master concepts (Starfield), this will cause potential corruption
    /// if any light or medium flagged mods are referenced. <br />
    /// - Masters will be unordered and may not match the load order the mod is eventually run with
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithNoLoadOrder()
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params);
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderLoadOrderChoice.WithNoLoadOrder() => WithNoLoadOrder(); 
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<IModMasterStyledGetter>> loadOrder)
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(ILoadOrderGetter<IModListingGetter<IModMasterStyledGetter>> loadOrder) => WithLoadOrder(loadOrder);

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModMasterStyledGetter> loadOrder)
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey)),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(ILoadOrderGetter<IModMasterStyledGetter> loadOrder) => WithLoadOrder(loadOrder);
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        params IModMasterStyledGetter[] loadOrder)
    {
        return WithLoadOrder(new LoadOrder<IModMasterStyledGetter>(loadOrder, disposeItems: false));
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(params IModMasterStyledGetter[] loadOrder) => WithLoadOrder(loadOrder);
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        IEnumerable<IModMasterStyledGetter> loadOrder)
    {
        return WithLoadOrder(new LoadOrder<IModMasterStyledGetter>(loadOrder, disposeItems: false));
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(IEnumerable<IModMasterStyledGetter> loadOrder) => WithLoadOrder(loadOrder);

    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<TModGetter>> loadOrder)
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _knownModLoadOrder = loadOrder,
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }

    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<TModGetter> loadOrder)
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _knownModLoadOrder = loadOrder.Transform(x => new ModListing<TModGetter>(x)),
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey)),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        params TModGetter[] loadOrder)
    {
        return WithLoadOrder(new LoadOrder<TModGetter>(loadOrder, disposeItems: false));
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithLoadOrder(
        IEnumerable<TModGetter> loadOrder)
    {
        return WithLoadOrder(new LoadOrder<TModGetter>(loadOrder, disposeItems: false));
    }

    /// <summary>
    /// Writes the mod with the default load order and data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithDefaultLoadOrder()
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _dataFolderGetter = (m, p) => GameLocatorLookupCache.Instance.GetDataDirectory(m.GameRelease),
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                var dataFolder = p._dataFolderGetter?.Invoke(m, p._param) ?? throw new ArgumentNullException("Data folder source was not set");
                var lo = LoadOrder.Import<IModMasterStyledGetter>(
                    dataFolder,
                    m.GameRelease,
                    factory: (modPath) => KeyedMasterStyle.FromPath(modPath, p._gameRelease, p._param.FileSystem),
                    p._param.FileSystem);
                
                ILoadOrderGetter<IModMasterStyledGetter>? modFlagsLo = null;
                if (GameConstants.Get(m.GameRelease).SeparateMasterLoadOrders)
                {
                    modFlagsLo = lo
                        .ResolveExistingMods(disposeItems: false);
                }
                return p._param with
                {
                    MasterFlagsLookup = modFlagsLo?.Where(x => !alreadyKnownMasters.Contains(x.ModKey)),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo)
                };
            }
        });
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderLoadOrderChoice.WithDefaultLoadOrder() => WithDefaultLoadOrder();
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderDataFolderChoice<TModGetter> WithLoadOrder(
        IEnumerable<ModKey> loadOrder)
    {
        return new BinaryModdedWriteBuilderDataFolderChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                ModKey[] loArray = loadOrder.ToArray();
                var dataFolder = p._dataFolderGetter?.Invoke(m, p._param);
                if (dataFolder == null || !GameConstants.Get(m.GameRelease).SeparateMasterLoadOrders)
                {
                    return p._param with
                    {
                        MastersListOrdering = new MastersListOrderingByLoadOrder(loArray),
                        LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loArray)
                    };
                }
                else
                {
                    var lo = LoadOrder.Import<IModMasterStyledGetter>(
                        dataFolder.Value, 
                        loArray,
                        p._gameRelease,
                        factory: (modPath) => KeyedMasterStyle.FromPath(modPath, p._gameRelease, p._param.FileSystem),
                        p._param.FileSystem);
                    return p._param with
                    {
                        MasterFlagsLookup = lo
                            .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                            .ResolveExistingMods(),
                        MastersListOrdering = new MastersListOrderingByLoadOrder(lo),
                        LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo)
                    };
                }
            }
        });
    }
    IBinaryModdedWriteBuilderDataFolderChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(IEnumerable<ModKey> loadOrder) => WithLoadOrder(loadOrder);
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilderDataFolderChoice<TModGetter> WithLoadOrder(
        params ModKey[] loadOrder)
    {
        return WithLoadOrder((IEnumerable<ModKey>)loadOrder);
    }
    IBinaryModdedWriteBuilderDataFolderChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrder(ModKey[] loadOrder) => WithLoadOrder(loadOrder);

    public BinaryModdedWriteBuilderDataFolderChoice<TModGetter> WithLoadOrderFromHeaderMasters()
    {
        return new BinaryModdedWriteBuilderDataFolderChoice<TModGetter>(_mod, _params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                var dataFolder = p._dataFolderGetter?.Invoke(m, p._param);
                if (dataFolder == null || !GameConstants.Get(m.GameRelease).SeparateMasterLoadOrders)
                {
                    var lo = _mod.MasterReferences.Select(x => x.Master).ToArray();
                    
                    return p._param with
                    {
                        MastersListOrdering = new MastersListOrderingByLoadOrder(lo),
                        LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo)
                    };
                }
                else
                {
                    var lo = LoadOrder.Import<TModGetter>(
                        dataFolder: dataFolder.Value, 
                        loadOrder: _mod.MasterReferences.Select(x => x.Master),
                        m.GameRelease,
                        p._param.FileSystem);

                    return p._param with
                    {
                        MasterFlagsLookup = lo
                            .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                            .ResolveExistingMods(),
                        MastersListOrdering = new MastersListOrderingByLoadOrder(lo),
                        LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo)
                    };
                }
            }
        });
    }
    IBinaryModdedWriteBuilderDataFolderChoice IBinaryModdedWriteBuilderLoadOrderChoice.WithLoadOrderFromHeaderMasters() => WithLoadOrderFromHeaderMasters();
}

public record BinaryWriteBuilderLoadOrderChoice<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;

    internal BinaryWriteBuilderLoadOrderChoice(BinaryWriteBuilderParams<TModGetter> @params)
    {
        _params = @params;
    }

    /// <summary>
    /// Writes the mod with no load order as reference.  Avoid if possible.<br />
    /// NOTE:  This will have the following negative consequences: <br />
    /// - For games with seperated master concepts (Starfield), this will cause potential corruption
    /// if any light or medium flagged mods are referenced. <br />
    /// - Masters will be unordered and may not match the load order the mod is eventually run with
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithNoLoadOrder()
    {
        return new BinaryWriteBuilder<TModGetter>(_params);
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<IModMasterStyledGetter>> loadOrder)
    {
        return new BinaryWriteBuilder<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModMasterStyledGetter> loadOrder)
    {
        return new BinaryWriteBuilder<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey)),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        params IModMasterStyledGetter[] loadOrder)
    {
        return WithLoadOrder(new LoadOrder<IModMasterStyledGetter>(loadOrder, disposeItems: false));
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        IEnumerable<IModMasterStyledGetter> loadOrder)
    {
        return WithLoadOrder(new LoadOrder<IModMasterStyledGetter>(loadOrder, disposeItems: false));
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<IModListingGetter<TModGetter>> loadOrder)
    {
        return new BinaryWriteBuilder<TModGetter>(_params with
        {
            _knownModLoadOrder = loadOrder,
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        ILoadOrderGetter<TModGetter> loadOrder)
    {
        return new BinaryWriteBuilder<TModGetter>(_params with
        {
            _knownModLoadOrder = loadOrder.Transform(x => new ModListing<TModGetter>(x)),
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                return p._param with
                {
                    MasterFlagsLookup = loadOrder
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey)),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(loadOrder)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        params TModGetter[] loadOrder)
    {
        return WithLoadOrder(new LoadOrder<TModGetter>(loadOrder, disposeItems: false));
    }
    
    /// <summary>
    /// Writes the mod with given load order as reference
    /// </summary>
    /// <param name="loadOrder">Load order to reference</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithLoadOrder(
        IEnumerable<TModGetter> loadOrder)
    {
        return WithLoadOrder(new LoadOrder<TModGetter>(loadOrder, disposeItems: false));
    }

    /// <summary>
    /// Writes the mod with the default load order and data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithDefaultLoadOrder()
    {
        return new BinaryWriteBuilder<TModGetter>(_params with
        {
            _dataFolderGetter = static (m, p) => GameLocatorLookupCache.Instance.GetDataDirectory(m.GameRelease),
            _loadOrderSetter = static (m, p, alreadyKnownMasters) =>
            {
                var dataFolder = p._dataFolderGetter?.Invoke(m, p._param) ?? throw new ArgumentNullException("Data folder source was not set");
                var lo = LoadOrder.Import<IModMasterStyledGetter>(
                    dataFolder,
                    m.GameRelease,
                    factory: (modPath) => KeyedMasterStyle.FromPath(modPath, p._gameRelease, p._param.FileSystem),
                    p._param.FileSystem);   
                
                return p._param with
                {
                    MasterFlagsLookup = lo
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderDataFolderChoice<TModGetter> WithLoadOrder(
        IEnumerable<ModKey> loadOrder)
    {
        return new BinaryWriteBuilderDataFolderChoice<TModGetter>(_params with
        {
            _loadOrderSetter = (m, p, alreadyKnownMasters) =>
            {
                var dataFolder = p._dataFolderGetter?.Invoke(m, p._param) ?? throw new ArgumentNullException("Data folder source was not set");
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder, loadOrder,
                    m.GameRelease, p._param.FileSystem);
                return p._param with
                {
                    MasterFlagsLookup = lo
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo)
                };
            }
        });
    }
    
    /// <summary>
    /// Writes the mod with the default load order and given data folder as reference.
    /// </summary>
    /// <param name="loadOrder">Load order</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilderDataFolderChoice<TModGetter> WithLoadOrder(
        params ModKey[] loadOrder)
    {
        return WithLoadOrder((IEnumerable<ModKey>)loadOrder);
    }

    public BinaryWriteBuilderDataFolderChoice<TModGetter> WithLoadOrderFromHeaderMasters()
    {
        return new BinaryWriteBuilderDataFolderChoice<TModGetter>(_params with
        {
            _loadOrderSetter = static (m, p, alreadyKnownMasters) =>
            {
                var dataFolder = p._dataFolderGetter?.Invoke(m, p._param) ?? throw new ArgumentNullException("Data folder source was not set");
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder, m.MasterReferences.Select(x => x.Master),
                    m.GameRelease, p._param.FileSystem);   
                
                return p._param with
                {
                    MasterFlagsLookup = lo
                        .Where(x => !alreadyKnownMasters.Contains(x.ModKey))
                        .ResolveExistingMods(disposeItems: false),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(lo),
                    LowerRangeDisallowedHandler = ALowerRangeDisallowedHandlerOption.AddPlaceholder(lo)
                };
            }
        });
    }
}

public record BinaryWriteBuilderDataFolderChoice<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _param;

    internal BinaryWriteBuilderDataFolderChoice(BinaryWriteBuilderParams<TModGetter> @params)
    {
        _param = @params;
    }

    public BinaryWriteBuilder<TModGetter> WithDefaultDataFolder()
    {
        return new BinaryWriteBuilder<TModGetter>(_param with
        {
            _dataFolderGetter = (m, p) => GameLocatorLookupCache.Instance.GetDataDirectory(m.GameRelease)
        });
    }
    
    public BinaryWriteBuilder<TModGetter> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryWriteBuilder<TModGetter>(_param);
        }
        return new BinaryWriteBuilder<TModGetter>(_param with
        {
            _dataFolderGetter = (m, p) => dataFolder.Value
        });
    }
    
    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithKnownMasters(params IModMasterStyledGetter[] knownMasters)
    {
        var match = _param.KnownMasters.FirstOrDefault(existingKnownMaster =>
            knownMasters.Any(x => x.ModKey == existingKnownMaster.ModKey));
        if (match != null)
        {
            throw new ArgumentException($"ModKey was already added as a known master: {match.ModKey}");
        }

        return new BinaryWriteBuilder<TModGetter>(_param with
        {
            KnownMasters = _param.KnownMasters.And(knownMasters).ToArray()
        });
    }

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithKnownMasters(params KeyedMasterStyle[] knownMasters)
    {
        return WithKnownMasters(knownMasters.Cast<IModMasterStyledGetter>().ToArray());
    }
}

public interface IBinaryModdedWriteBuilderDataFolderChoice
{
    IBinaryModdedWriteBuilder WithDefaultDataFolder();
    IBinaryModdedWriteBuilder WithDataFolder(DirectoryPath? dataFolder);
    IBinaryModdedWriteBuilder WithNoDataFolder();
    IBinaryModdedWriteBuilder WithKnownMasters(params IModMasterStyledGetter[] knownMasters);
    IBinaryModdedWriteBuilder WithKnownMasters(params KeyedMasterStyle[] knownMasters);
}

public record BinaryModdedWriteBuilderDataFolderChoice<TModGetter> : IBinaryModdedWriteBuilderDataFolderChoice
    where TModGetter : class, IModGetter
{
    private readonly TModGetter _mod;
    internal BinaryWriteBuilderParams<TModGetter> _param;

    internal BinaryModdedWriteBuilderDataFolderChoice(TModGetter mod, BinaryWriteBuilderParams<TModGetter> @params)
    {
        _mod = mod;
        _param = @params;
    }

    public BinaryModdedWriteBuilder<TModGetter> WithDefaultDataFolder()
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _param with
        {
            _dataFolderGetter = (m, p) => GameLocatorLookupCache.Instance.GetDataDirectory(m.GameRelease)
        });
    }

    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderDataFolderChoice.WithDefaultDataFolder() =>
        WithDefaultDataFolder();
    
    public BinaryModdedWriteBuilder<TModGetter> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryModdedWriteBuilder<TModGetter>(_mod, _param);
        }
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _param with
        {
            _dataFolderGetter = (m, p) => dataFolder.Value
        });
    }

    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderDataFolderChoice.WithDataFolder(DirectoryPath? dataFolder) =>
        WithDataFolder(dataFolder);
    
    public BinaryModdedWriteBuilder<TModGetter> WithNoDataFolder()
    {
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _param);
    }

    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderDataFolderChoice.WithNoDataFolder() =>
        WithNoDataFolder();
    
    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithKnownMasters(params IModMasterStyledGetter[] knownMasters)
    {
        var match = _param.KnownMasters.FirstOrDefault(existingKnownMaster =>
            knownMasters.Any(x => x.ModKey == existingKnownMaster.ModKey));
        if (match != null)
        {
            throw new ArgumentException($"ModKey was already added as a known master: {match.ModKey}");
        }

        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _param with
        {
            KnownMasters = _param.KnownMasters.And(knownMasters).ToArray()
        });
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderDataFolderChoice.WithKnownMasters(params IModMasterStyledGetter[] knownMasters) =>
        WithKnownMasters(knownMasters);

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithKnownMasters(params KeyedMasterStyle[] knownMasters)
    {
        return WithKnownMasters(knownMasters.Cast<IModMasterStyledGetter>().ToArray());
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilderDataFolderChoice.WithKnownMasters(params KeyedMasterStyle[] knownMasters) =>
        WithKnownMasters(knownMasters);
}

public interface IBinaryModdedWriteBuilder
{
    /// <summary>
    /// Adjusts the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithModKeySync(ModKeyOption option);

    /// <summary>
    /// Disables the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoModKeySync();

    /// <summary>
    /// Adjusts the filesystem to write to
    /// </summary>
    /// <param name="fileSystem">Filesystem to write to</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithFileSystem(IFileSystem? fileSystem);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithMastersListContent(MastersListContentOption option);

    /// <summary>
    /// Disables logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoMastersListContentCheck();

    /// <summary>
    /// Specify logic to use to keep a mod's record count in sync
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithRecordCount(RecordCountOption option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithMastersListOrdering(
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
    IBinaryModdedWriteBuilder WithMastersListOrdering(
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
    IBinaryModdedWriteBuilder WithMastersListOrdering(
        ILoadOrderGetter loadOrder);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="otherMasters">Other masters to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithMastersListOrdering(
        IReadOnlyMasterReferenceCollection otherMasters);

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoNextFormIDProcessing();

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <param name="useLowerRange">Force the lower FormID range usage on or off</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithForcedLowerFormIdRangeUsage(bool? useLowerRange);

    /// <summary>
    /// Turns off logic to check for FormID uniqueness.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoFormIDUniquenessCheck();

    /// <summary>
    /// Turns off logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoFormIDCompactnessCheck();

    /// <summary>
    /// Adjusts logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <param name="option">Logic to use for checking FormID compactness</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithFormIDCompactnessCheck(FormIDCompactionOption option);

    /// <summary>
    /// StringsWriter override, for mods that are able to localize.
    /// </summary>
    /// <param name="stringsWriter">StringsWriter to use when localizing</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithStringsWriter(StringsWriter? stringsWriter);

    /// <summary>
    /// If writing a localizable mod that has localization off, which language to output as the embedded strings
    /// </summary>
    /// <param name="language">Language to output as the embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithTargetLanguage(Language language);

    /// <summary>
    /// Disables logic to zero out all null FormIDs
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoNullFormIDStandardization();

    /// <summary>
    /// Encoding overrides to use for embedded strings
    /// </summary>
    /// <param name="encodingBundle">Encoding overrides to use for embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithEmbeddedEncodings(EncodingBundle? encodingBundle);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="placeholder">ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder);

    /// <summary>
    /// When lower formID ranges are used in a non-allowed way, set the system to throw an exception <br />
    /// Typically this occurs when the lower ranges are used without any masters present.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder ThrowIfLowerRangeDisallowed();

    /// <summary>
    /// Adjusts system to not check lower formID ranges are used in a non-allowed way
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder NoCheckIfLowerRangeDisallowed();

    /// <summary>
    /// Sets rules to be used for determining how to parallelize writing
    /// </summary>
    /// <param name="parameters">Parameters to use for parallel writing</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithParallelWriteParameters(ParallelWriteParameters parameters);

    /// <summary>
    /// Sets writing to be done on current thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder SingleThread();

    /// <summary>
    /// Specifies a list of masters to include if they are not included naturally
    /// </summary>
    /// <param name="modKeys">Extra ModKeys to include</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithExtraIncludedMasters(IEnumerable<ModKey> modKeys);

    /// <summary>
    /// Specifies a list of masters to include if they are not included naturally
    /// </summary>
    /// <param name="modKeys">Extra ModKeys to include</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithExtraIncludedMasters(params ModKey[] modKeys);

    /// <summary>
    /// Specifies a list of masters to set the mod to contain. <br />
    /// This overrides all normally contained masters, and may result in a corrupted mod if set incorrectly. <br />
    /// If set after <see cref="WithExtraIncludedMasters" /> or <see cref="WithCkRequiredMasters"/>, they will be forgotten.
    /// </summary>
    /// <param name="modKeys">ModKeys to have the mod contain</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithExplicitOverridingMasterList(IEnumerable<ModKey> modKeys);

    /// <summary>
    /// Specifies a list of masters to set the mod to contain. <br />
    /// This overrides all normally contained masters, and may result in a corrupted mod if set incorrectly. <br />
    /// If set after <see cref="WithExtraIncludedMasters" /> or <see cref="WithCkRequiredMasters"/>, they will be forgotten.
    /// </summary>
    /// <param name="modKeys">ModKeys to have the mod contain</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithExplicitOverridingMasterList(params ModKey[] modKeys);

    /// <summary>
    /// The Creation Kit complains when loading mods without all transitive masters listed. <br />
    /// This call makes sure to include all transitive masters, even if they are not needed by the mod's content
    /// to avoid issues when loading the plugin in the Creation Kit.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithAllParentMasters();

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithKnownMasters(params IModMasterStyledGetter[] knownMasters);

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    IBinaryModdedWriteBuilder WithKnownMasters(params KeyedMasterStyle[] knownMasters);

    internal IBinaryModdedWriteBuilder WithOverriddenFormsOption(OverriddenFormsOption option);

    public IBinaryModdedWriteBuilder WithDataFolder(DirectoryPath? dataFolder);

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

public record BinaryModdedWriteBuilder<TModGetter> : IBinaryModdedWriteBuilder
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;
    internal TModGetter _mod { get; init; } = null!;

    internal BinaryModdedWriteBuilder(
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
    public BinaryModdedWriteBuilder<TModGetter> WithModKeySync(ModKeyOption option)
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

    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithModKeySync(ModKeyOption option) => WithModKeySync(option);
    
    /// <summary>
    /// Disables the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoModKeySync()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoModKeySync() => NoModKeySync();

    /// <summary>
    /// Adjusts the filesystem to write to
    /// </summary>
    /// <param name="fileSystem">Filesystem to write to</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithFileSystem(IFileSystem? fileSystem)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithFileSystem(IFileSystem? fileSystem) => WithFileSystem(fileSystem);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithMastersListContent(MastersListContentOption option)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithMastersListContent(MastersListContentOption option) => WithMastersListContent(option);

    /// <summary>
    /// Disables logic to use to keep a mod's master list keys in sync<br/>
    /// This setting is just used to sync the contents of the list, not the order
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoMastersListContentCheck()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoMastersListContentCheck() => NoMastersListContentCheck();

    /// <summary>
    /// Specify logic to use to keep a mod's record count in sync
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithRecordCount(RecordCountOption option)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithRecordCount(RecordCountOption option) => WithRecordCount(option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="option">Option to use</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithMastersListOrdering(MastersListOrderingOption option) => WithMastersListOrdering(option);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithMastersListOrdering(IEnumerable<ModKey> loadOrder) => WithMastersListOrdering(loadOrder);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="loadOrder">Load order to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithMastersListOrdering(ILoadOrderGetter loadOrder) => WithMastersListOrdering(loadOrder);

    /// <summary>
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="otherMasters">Other masters to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithMastersListOrdering(
        IReadOnlyMasterReferenceCollection otherMasters)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingByLoadOrder(otherMasters
                        .Masters.Select(x => x.Master))
                }
            }
        };
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithMastersListOrdering(IReadOnlyMasterReferenceCollection otherMasters) => WithMastersListOrdering(otherMasters);

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoNextFormIDProcessing()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoNextFormIDProcessing() => NoNextFormIDProcessing();

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <param name="useLowerRange">Force the lower FormID range usage on or off</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithForcedLowerFormIdRangeUsage(bool? useLowerRange)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithForcedLowerFormIdRangeUsage(bool? useLowerRange) => WithForcedLowerFormIdRangeUsage(useLowerRange);

    /// <summary>
    /// Turns off logic to check for FormID uniqueness.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoFormIDUniquenessCheck()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoFormIDUniquenessCheck() => NoFormIDUniquenessCheck();

    /// <summary>
    /// Turns off logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoFormIDCompactnessCheck()
    {
        return WithFormIDCompactnessCheck(FormIDCompactionOption.NoCheck);
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoFormIDCompactnessCheck() => NoFormIDCompactnessCheck();

    /// <summary>
    /// Adjusts logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <param name="option">Logic to use for checking FormID compactness</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithFormIDCompactnessCheck(FormIDCompactionOption option)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithFormIDCompactnessCheck(FormIDCompactionOption option) => WithFormIDCompactnessCheck(option);

    /// <summary>
    /// StringsWriter override, for mods that are able to localize.
    /// </summary>
    /// <param name="stringsWriter">StringsWriter to use when localizing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithStringsWriter(StringsWriter? stringsWriter)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithStringsWriter(StringsWriter? stringsWriter) => WithStringsWriter(stringsWriter);

    /// <summary>
    /// If writing a localizable mod that has localization off, which language to output as the embedded strings
    /// </summary>
    /// <param name="language">Language to output as the embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithTargetLanguage(Language language)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithTargetLanguage(Language language) => WithTargetLanguage(language);

    /// <summary>
    /// Disables logic to zero out all null FormIDs
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoNullFormIDStandardization()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoNullFormIDStandardization() => NoNullFormIDStandardization();

    /// <summary>
    /// Encoding overrides to use for embedded strings
    /// </summary>
    /// <param name="encodingBundle">Encoding overrides to use for embedded strings</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithEmbeddedEncodings(EncodingBundle? encodingBundle)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithEmbeddedEncodings(EncodingBundle? encodingBundle) => WithEmbeddedEncodings(encodingBundle);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="placeholder">ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder) => WithPlaceholderMasterIfLowerRangeDisallowed(placeholder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder) => WithPlaceholderMasterIfLowerRangeDisallowed(loadOrder);

    /// <summary>
    /// Adjusts how to handle when lower formID ranges are used in a non-allowed way. <br />
    /// Typically this occurs when the lower ranges are used without any masters present. <br />
    /// If this occurs with this option, the given mod will be added as a placeholder to make the setup legal
    /// </summary>
    /// <param name="loadOrder">LoadOrder to look to for a ModKey to add as a master if lower range FormIDs are used without any other masters present</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder) => WithPlaceholderMasterIfLowerRangeDisallowed(loadOrder);

    /// <summary>
    /// When lower formID ranges are used in a non-allowed way, set the system to throw an exception <br />
    /// Typically this occurs when the lower ranges are used without any masters present.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> ThrowIfLowerRangeDisallowed()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.ThrowIfLowerRangeDisallowed() => ThrowIfLowerRangeDisallowed();
    
    /// <summary>
    /// Adjusts system to not check lower formID ranges are used in a non-allowed way
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> NoCheckIfLowerRangeDisallowed()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.NoCheckIfLowerRangeDisallowed() => NoCheckIfLowerRangeDisallowed();

    /// <summary>
    /// Sets rules to be used for determining how to parallelize writing
    /// </summary>
    /// <param name="parameters">Parameters to use for parallel writing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithParallelWriteParameters(ParallelWriteParameters parameters)
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithParallelWriteParameters(ParallelWriteParameters parameters) => WithParallelWriteParameters(parameters);
    
    /// <summary>
    /// Sets writing to be done on current thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> SingleThread()
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
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.SingleThread() => SingleThread();

    /// <summary>
    /// Specifies a list of masters to include if they are not included naturally <br/>
    /// If called several times, the extra ModKeys will accumulate.
    /// </summary>
    /// <param name="modKeys">Extra ModKeys to include</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithExtraIncludedMasters(IEnumerable<ModKey> modKeys)
    {
        return WithExtraIncludedMasters(modKeys.ToArray());
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithExtraIncludedMasters(IEnumerable<ModKey> modKeys) => WithExtraIncludedMasters(modKeys);

    /// <summary>
    /// Specifies a list of masters to include if they are not included naturally <br/>
    /// If called several times, the extra ModKeys will accumulate.
    /// </summary>
    /// <param name="modKeys">Extra ModKeys to include</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithExtraIncludedMasters(params ModKey[] modKeys)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersContentCustomOverride = (mods) =>
                    {
                        return (_params._param.MastersContentCustomOverride?.Invoke(mods) ?? mods)
                            .And(modKeys)
                            .Distinct()
                            .ToArray();
                    }
                }
            }
        };
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithExtraIncludedMasters(params ModKey[] modKeys) => WithExtraIncludedMasters(modKeys);

    /// <summary>
    /// Specifies a list of masters to set the mod to contain. <br />
    /// This overrides all normally contained masters, and may result in a corrupted mod if set incorrectly. <br />
    /// If set after <see cref="WithExtraIncludedMasters(System.Collections.Generic.IEnumerable{Mutagen.Bethesda.Plugins.ModKey})" /> or <see cref="WithAllParentMasters"/>, they will be forgotten.
    /// </summary>
    /// <param name="modKeys">ModKeys to have the mod contain</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithExplicitOverridingMasterList(IEnumerable<ModKey> modKeys)
    {
        return this with
        {
            _params = _params with
            {
                _masterSyncAction = null,
                _param = _params._param with
                {
                    MastersListContent = MastersListContentOption.NoCheck,
                    MastersContentCustomOverride = (mods) => modKeys.ToArray(),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(modKeys.ToArray())
                }
            }
        };
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithExplicitOverridingMasterList(IEnumerable<ModKey> modKeys) => WithExplicitOverridingMasterList(modKeys);

    /// <summary>
    /// Specifies a list of masters to set the mod to contain. <br />
    /// This overrides all normally contained masters, and may result in a corrupted mod if set incorrectly. <br />
    /// If set after <see cref="WithExtraIncludedMasters" /> or <see cref="WithAllParentMasters"/>, they will be forgotten.
    /// </summary>
    /// <param name="modKeys">ModKeys to have the mod contain</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithExplicitOverridingMasterList(params ModKey[] modKeys)
    {
        return WithExplicitOverridingMasterList((IEnumerable<ModKey>)modKeys);
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithExplicitOverridingMasterList(params ModKey[] modKeys) => WithExplicitOverridingMasterList(modKeys);

    /// <summary>
    /// The Creation Kit complains when loading mods without all transitive masters listed. <br />
    /// This call makes sure to include all transitive masters, even if they are not needed by the mod's content
    /// to avoid issues when loading the plugin in the Creation Kit.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithAllParentMasters()
    {
        return this with
        {
            _params = _params with
            {
                _masterSyncAction = static (mod, p) =>
                {
                    var dataFolder = p._dataFolderGetter?.Invoke(mod, p._param);

                    return p._param with
                    {
                        MastersContentCustomOverride = (mods) =>
                        {
                            return TransitiveMasterLocator.GetAllMasters(
                                p._gameRelease,
                                mod.ModKey,
                                mods,
                                p._knownModLoadOrder,
                                dataFolder,
                                p._param.FileSystem);
                        }
                    };
                },
            }
        };
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithAllParentMasters() => WithAllParentMasters();
    
    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithKnownMasters(params IModMasterStyledGetter[] knownMasters)
    {
        var match = _params.KnownMasters.FirstOrDefault(existingKnownMaster =>
            knownMasters.Any(x => x.ModKey == existingKnownMaster.ModKey));
        if (match != null)
        {
            throw new ArgumentException($"ModKey was already added as a known master: {match.ModKey}");
        }

        return this with
        {
            _params = _params with
            {
                KnownMasters = _params.KnownMasters.And(knownMasters).ToArray()
            }
        };
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithKnownMasters(params IModMasterStyledGetter[] knownMasters) => WithKnownMasters(knownMasters);

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryModdedWriteBuilder<TModGetter> WithKnownMasters(params KeyedMasterStyle[] knownMasters)
    {
        return WithKnownMasters(knownMasters.Cast<IModMasterStyledGetter>().ToArray());
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithKnownMasters(params KeyedMasterStyle[] knownMasters) => WithKnownMasters(knownMasters);
    
    internal BinaryModdedWriteBuilder<TModGetter> WithOverriddenFormsOption(OverriddenFormsOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    OverriddenFormsOption = option
                }
            }
        };
    }

    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithOverriddenFormsOption(OverriddenFormsOption option) => WithOverriddenFormsOption(option);
    
    public BinaryModdedWriteBuilder<TModGetter> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params);
        }
        return new BinaryModdedWriteBuilder<TModGetter>(_mod, _params with
        {
            _dataFolderGetter = (m, p) => dataFolder.Value
        });
    }
    IBinaryModdedWriteBuilder IBinaryModdedWriteBuilder.WithDataFolder(DirectoryPath? dataFolder) => WithDataFolder(dataFolder);
    
    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    public async Task WriteAsync()
    {
        await _params._writer.WriteAsync(
            _mod, 
            BinaryWriteBuilderHelper.RunPreWriteSetters<TModGetter>(_mod, _params));
    }
    
    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    public void Write()
    {
        _params._writer.Write(
            _mod, 
            BinaryWriteBuilderHelper.RunPreWriteSetters<TModGetter>(_mod, _params));
    }
}

public record BinaryWriteBuilder<TModGetter>
    where TModGetter : class, IModGetter
{
    internal BinaryWriteBuilderParams<TModGetter> _params;

    internal BinaryWriteBuilder(
        BinaryWriteBuilderParams<TModGetter> @params)
    {
        _params = @params;
    }
    
    /// <summary>
    /// Adjusts the logic to use to keep a mod's ModKey in sync with its path
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithModKeySync(ModKeyOption option)
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
    public BinaryWriteBuilder<TModGetter> NoModKeySync()
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
    public BinaryWriteBuilder<TModGetter> WithFileSystem(IFileSystem fileSystem)
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
    public BinaryWriteBuilder<TModGetter> WithMastersListContent(MastersListContentOption option)
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
    public BinaryWriteBuilder<TModGetter> NoMastersListContentCheck()
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
    public BinaryWriteBuilder<TModGetter> WithRecordCount(RecordCountOption option)
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
    public BinaryWriteBuilder<TModGetter> WithMastersListOrdering(
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
    public BinaryWriteBuilder<TModGetter> WithMastersListOrdering(
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
    public BinaryWriteBuilder<TModGetter> WithMastersListOrdering(
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
    /// Specify what logic to use to keep a mod's master list ordering in sync<br/>
    /// This setting is just used to sync the order of the list, not the content<br />
    /// <br />
    /// NOTE: If you provided a load order then setting this is unnecessary and
    /// will worsen the master ordering
    /// </summary>
    /// <param name="otherMasters">Other masters to use for ordering masters</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithMastersListOrdering(
        IReadOnlyMasterReferenceCollection otherMasters)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersListOrdering = new MastersListOrderingByLoadOrder(otherMasters
                        .Masters.Select(x => x.Master))
                }
            }
        };
    }

    /// <summary>
    /// Turns off logic to adjust the Next FormID automatically.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> NoNextFormIDProcessing()
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
    public BinaryWriteBuilder<TModGetter> WithForcedLowerFormIdRangeUsage(bool? useLowerRange)
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
    public BinaryWriteBuilder<TModGetter> NoFormIDUniquenessCheck()
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
    public BinaryWriteBuilder<TModGetter> NoFormIDCompactnessCheck()
    {
        return WithFormIDCompactnessCheck(FormIDCompactionOption.NoCheck);
    }

    /// <summary>
    /// Adjusts logic to check that FormID are compacted according to the flags set in the mod's header.
    /// </summary>
    /// <param name="option">Logic to use for checking FormID compactness</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithFormIDCompactnessCheck(FormIDCompactionOption option)
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
    public BinaryWriteBuilder<TModGetter> WithStringsWriter(StringsWriter stringsWriter)
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
    public BinaryWriteBuilder<TModGetter> WithTargetLanguage(Language language)
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
    public BinaryWriteBuilder<TModGetter> NoNullFormIDStandardization()
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
    public BinaryWriteBuilder<TModGetter> WithEmbeddedEncodings(EncodingBundle? encodingBundle)
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
    public BinaryWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ModKey placeholder)
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
    public BinaryWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(ILoadOrderGetter loadOrder)
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
    public BinaryWriteBuilder<TModGetter> WithPlaceholderMasterIfLowerRangeDisallowed(IEnumerable<ModKey> loadOrder)
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
    public BinaryWriteBuilder<TModGetter> ThrowIfLowerRangeDisallowed()
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
    public BinaryWriteBuilder<TModGetter> NoCheckIfLowerRangeDisallowed()
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
    public BinaryWriteBuilder<TModGetter> WithParallelWriteParameters(ParallelWriteParameters parameters)
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
    public BinaryWriteBuilder<TModGetter> SingleThread()
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
    /// Specifies a list of masters to include if they are not included naturally <br/>
    /// If called several times, the extra ModKeys will accumulate.
    /// </summary>
    /// <param name="modKeys">Extra ModKeys to include</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithExtraIncludedMasters(IEnumerable<ModKey> modKeys)
    {
        return WithExtraIncludedMasters(modKeys.ToArray());
    }

    /// <summary>
    /// Specifies a list of masters to include if they are not included naturally <br/>
    /// If called several times, the extra ModKeys will accumulate.
    /// </summary>
    /// <param name="modKeys">Extra ModKeys to include</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithExtraIncludedMasters(params ModKey[] modKeys)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    MastersContentCustomOverride = (mods) =>
                    {
                        return (_params._param.MastersContentCustomOverride?.Invoke(mods) ?? mods)
                            .And(modKeys)
                            .Distinct()
                            .ToArray();
                    }
                }
            }
        };
    }

    /// <summary>
    /// Specifies a list of masters to set the mod to contain. <br />
    /// This overrides all normally contained masters, and may result in a corrupted mod if set incorrectly. <br />
    /// If set after <see cref="WithExtraIncludedMasters" /> or <see cref="P"/>, they will be forgotten.
    /// </summary>
    /// <param name="modKeys">ModKeys to have the mod contain</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithExplicitOverridingMasterList(IEnumerable<ModKey> modKeys)
    {
        return this with
        {
            _params = _params with
            {
                _masterSyncAction = null,
                _param = _params._param with
                {
                    MastersListContent = MastersListContentOption.NoCheck,
                    MastersContentCustomOverride = (mods) => modKeys.ToArray(),
                    MastersListOrdering = new MastersListOrderingByLoadOrder(modKeys.ToArray())
                }
            }
        };
    }

    /// <summary>
    /// Specifies a list of masters to set the mod to contain. <br />
    /// This overrides all normally contained masters, and may result in a corrupted mod if set incorrectly. <br />
    /// If set after <see cref="WithExtraIncludedMasters" /> or <see cref="WithCkRequiredMasters"/>, they will be forgotten.
    /// </summary>
    /// <param name="modKeys">ModKeys to have the mod contain</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithExplicitOverridingMasterList(params ModKey[] modKeys)
    {
        return WithExplicitOverridingMasterList((IEnumerable<ModKey>)modKeys);
    }
    
    /// <summary>
    /// The Creation Kit complains when loading mods without all transitive masters listed. <br />
    /// This call makes sure to include all transitive masters, even if they are not needed by the mod's content
    /// to avoid issues when loading the plugin in the Creation Kit.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithAllParentMasters()
    {
        return this with
        {
            _params = _params with
            {
                _masterSyncAction = static (mod, p) =>
                {
                    var dataFolder = p._dataFolderGetter?.Invoke(mod, p._param);

                    return p._param with
                    {
                        MastersContentCustomOverride = (mods) =>
                        {
                            return TransitiveMasterLocator.GetAllMasters(
                                p._gameRelease,
                                mod.ModKey,
                                mods,
                                p._knownModLoadOrder,
                                dataFolder,
                                p._param.FileSystem);
                        }
                    };
                },
            }
        };
    }

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithKnownMasters(params IModMasterStyledGetter[] knownMasters)
    {
        var match = _params.KnownMasters.FirstOrDefault(existingKnownMaster =>
            knownMasters.Any(x => x.ModKey == existingKnownMaster.ModKey));
        if (match != null)
        {
            throw new ArgumentException($"ModKey was already added as a known master: {match.ModKey}");
        }

        return this with
        {
            _params = _params with
            {
                KnownMasters = _params.KnownMasters.And(knownMasters).ToArray()
            }
        };
    }

    /// <summary>
    /// Separated master games (like Starfield) need to know what their masters styles are in order to parse correctly, 
    /// which is normally retrieved via looking at the mod files themselves from the Data Folder. <br />
    /// This is an alternative to hand provide the information so that they do not need to be present in the Data folder
    /// </summary>
    /// <param name="knownMasters">Master information to hand provide</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryWriteBuilder<TModGetter> WithKnownMasters(params KeyedMasterStyle[] knownMasters)
    {
        return WithKnownMasters(knownMasters.Cast<IModMasterStyledGetter>().ToArray());
    }
    
    internal BinaryWriteBuilder<TModGetter> WithOverriddenFormsOption(OverriddenFormsOption option)
    {
        return this with
        {
            _params = _params with
            {
                _param = _params._param with
                {
                    OverriddenFormsOption = option
                }
            }
        };
    }
    
    public BinaryWriteBuilder<TModGetter> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryWriteBuilder<TModGetter>(_params);
        }
        return new BinaryWriteBuilder<TModGetter>(_params with
        {
            _dataFolderGetter = (m, p) => dataFolder.Value
        });
    }
    
    /// <summary>
    /// Executes the instructions to write a mod.
    /// </summary>
    /// <param name="mod">Mod to write to</param>
    /// <returns>A task to await for writing completion</returns>
    public async Task WriteAsync(TModGetter mod)
    {
        await _params._writer.WriteAsync(
            mod, 
            BinaryWriteBuilderHelper.RunPreWriteSetters<TModGetter>(mod, _params));
    }
    
    /// <summary>
    /// Executes the instructions to write the mod.
    /// </summary>
    /// <returns>A task to await for writing completion</returns>
    public void Write(TModGetter mod)
    {
        _params._writer.Write(
            mod, 
            BinaryWriteBuilderHelper.RunPreWriteSetters<TModGetter>(mod, _params));
    }
}

internal static class BinaryWriteBuilderHelper
{
    public static BinaryWriteBuilderParams<TModGetter> RunPreWriteSetters<TModGetter>(
        TModGetter mod,
        BinaryWriteBuilderParams<TModGetter> p)
        where TModGetter : class, IModGetter
    {
        var knownSet = new HashSet<ModKey>(p.KnownMasters.Select(x => x.ModKey));
        if (p._gameRelease != mod.GameRelease)
        {
            throw new ArgumentException($"GameRelease did not match provided mod: {p._gameRelease} != {mod.GameRelease}");
        }
        if (p._loadOrderSetter != null)
        {
            p = p with
            {
                _param = p._loadOrderSetter(mod, p, knownSet)
            };
        }

        
        if (p._param.MasterFlagsLookup != null)
        {
            Cache<IModMasterStyledGetter, ModKey>? masterFlagsLookup = new(x => x.ModKey);
            masterFlagsLookup.SetTo(p._param.MasterFlagsLookup.Items);
            p.KnownMasters.ForEach(x => masterFlagsLookup.Add(x));

            if (masterFlagsLookup.Count == 0)
            {
                masterFlagsLookup = null;
            }
            
            p = p with
            {
                _param = p._param with
                {
                    MasterFlagsLookup = masterFlagsLookup
                }
            };
        }
        else if (p.KnownMasters.Length > 0)
        {
            Cache<IModMasterStyledGetter, ModKey> masterFlagsLookup = new(x => x.ModKey);
            masterFlagsLookup.SetTo(p.KnownMasters);
            p = p with
            {
                _param = p._param with
                {
                    MasterFlagsLookup = masterFlagsLookup
                }
            };
        }
        
        if (p._masterSyncAction != null)
        {
            p = p with
            {
                _param = p._masterSyncAction(mod, p)
            };
        }

        return p;
    }
}