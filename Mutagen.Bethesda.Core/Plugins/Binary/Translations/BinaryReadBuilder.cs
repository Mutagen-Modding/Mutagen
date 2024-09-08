using System.IO.Abstractions;
using Loqui.Internal;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

/// <summary>
/// Internal helper object to avoid relisting the same variables
/// </summary>
internal record BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : IModDisposeGetter
{
    internal GameRelease GameRelease { get; init; }
    internal ModKey ModKey { get; init; }
    internal FilePath? _path { get; init; }
    internal Stream? _stream { get; init; }
    internal Func<Stream>? _streamFactory { get; init; }
    internal bool _needsRecordTypeInfoCacheReader { get; init; }
    internal ErrorMaskBuilder? ErrorMaskBuilder { get; init; }
    internal TGroupMask? GroupMask { get; init; }
    internal BinaryReadParameters Params { get; init; } = BinaryReadParameters.Default;
    internal IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> _instantiator { get; init; } = null!;
    internal Func<BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>, IEnumerable<IModMasterStyled>>? _loadOrderSetter { get; init; }
    internal Func<BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>, DirectoryPath>? _dataFolderGetter { get; init; }
}

/// <summary>
/// Internal helper object to pass instructions for how to construct a mod once the builder is ready
/// </summary>
internal interface IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : IModDisposeGetter
{
    TMod Mutable(BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> builder);
    TModGetter Readonly(BinaryReadBuilder<TMod, TModGetter, TGroupMask> builder);
}

/// <summary>
/// Start of the building process that decides whether to read from a path or a stream
/// </summary>
public class BinaryReadBuilderSourceChoice<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : IModDisposeGetter
{
    private readonly GameRelease _release;
    private readonly IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> _instantiator;
    private readonly bool _needsRecordTypeInfoCacheReader;

    internal BinaryReadBuilderSourceChoice(
        GameRelease release,
        IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> instantiator,
        bool needsRecordTypeInfoCacheReader)
    {
        _release = release;
        _instantiator = instantiator;
        _needsRecordTypeInfoCacheReader = needsRecordTypeInfoCacheReader;
    }
    
    /// <summary>
    /// Instructs the builder to look at a path to construct the mod
    /// </summary>
    /// <param name="path">Path to the mod file.  If the file name is not a ModKey format,
    /// you must construct the ModPath yourself with an explicitly defined ModKey to use
    /// </param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> FromPath(
        ModPath path)
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(
            new BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>()
            {
                _instantiator = _instantiator,
                GameRelease = _release,
                ModKey = path.ModKey,
                _path = path.Path,
                _needsRecordTypeInfoCacheReader = _needsRecordTypeInfoCacheReader
            });
    }
    
    /// <summary>
    /// Instructs the builder to look at a stream to construct the mod
    /// </summary>
    /// <param name="stream">Stream to read the mod data from</param>
    /// <param name="modKey">ModKey to use when reading the mod data</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> FromStream(
        Stream stream,
        ModKey modKey)
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(
            new BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>()
            {
                _instantiator = _instantiator,
                GameRelease = _release,
                ModKey = modKey,
                _stream = stream,
                _needsRecordTypeInfoCacheReader = _needsRecordTypeInfoCacheReader
            });
    }
}

/// <summary>
/// Start of the building process that decides whether to read from a path or a stream factory.
/// </summary>
public class BinaryReadBuilderSourceStreamFactoryChoice<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : IModDisposeGetter
{
    private readonly GameRelease _release;
    private readonly IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> _instantiator;
    private readonly bool _needsRecordTypeInfoCacheReader;

    internal BinaryReadBuilderSourceStreamFactoryChoice(
        GameRelease release,
        IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> instantiator,
        bool needsRecordTypeInfoCacheReader)
    {
        _release = release;
        _instantiator = instantiator;
        _needsRecordTypeInfoCacheReader = needsRecordTypeInfoCacheReader;
    }
    
    /// <summary>
    /// Instructs the builder to look at a path to construct the mod
    /// </summary>
    /// <param name="path">Path to the mod file.  If the file name is not a ModKey format,
    /// you must construct the ModPath yourself with an explicitly defined ModKey to use
    /// </param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> FromPath(ModPath path)
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(
            new BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>()
            {
                _instantiator = _instantiator,
                GameRelease = _release,
                ModKey = path.ModKey,
                _path = path.Path,
                _needsRecordTypeInfoCacheReader = _needsRecordTypeInfoCacheReader
            });
    }
    
    /// <summary>
    /// Instructs the builder to look at a stream factory to construct the mod
    /// </summary>
    /// <param name="streamFactory">
    /// Stream factory to retrieve streams to read the mod data from.
    /// Must return a new stream each time it is called.
    /// </param>
    /// <param name="modKey">ModKey to use when reading the mod data</param>
    /// <exception cref="ArgumentException">Thrown if the streamFactory returns the same stream twice</exception>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> FromStreamFactory(
        Func<Stream> streamFactory,
        ModKey modKey)
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(
            new BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>()
            {
                _instantiator = _instantiator,
                GameRelease = _release,
                ModKey = modKey,
                _streamFactory = streamFactory,
                _needsRecordTypeInfoCacheReader = _needsRecordTypeInfoCacheReader
            });
    }
}

/// <summary>
/// Start of the building process that decides whether to read from a path or a stream.
/// Follows up with a forced choice of load order for games with separated load orders
/// </summary>
public class BinaryReadBuilderSeparatedSourceChoice<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : class, IModDisposeGetter
{
    private readonly GameRelease _release;
    private readonly IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> _instantiator;
    private readonly bool _needsRecordTypeInfoCacheReader;

    internal BinaryReadBuilderSeparatedSourceChoice(
        GameRelease release,
        IBinaryReadBuilderInstantiator<TMod, TModGetter, TGroupMask> instantiator,
        bool needsRecordTypeInfoCacheReader)
    {
        _release = release;
        _instantiator = instantiator;
        _needsRecordTypeInfoCacheReader = needsRecordTypeInfoCacheReader;
    }
    
    /// <summary>
    /// Instructs the builder to look at a path to construct the mod
    /// </summary>
    /// <param name="path">Path to the mod file.  If the file name is not a ModKey format,
    /// you must construct the ModPath yourself with an explicitly defined ModKey to use
    /// </param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderSeparatedChoice<TMod, TModGetter, TGroupMask> FromPath(
        ModPath path)
    {
        return new BinaryReadBuilderSeparatedChoice<TMod, TModGetter, TGroupMask>(
            new BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>()
            {
                _instantiator = _instantiator,
                GameRelease = _release,
                ModKey = path.ModKey,
                _path = path.Path,
                _needsRecordTypeInfoCacheReader = _needsRecordTypeInfoCacheReader
            });
    }
    
    /// <summary>
    /// Instructs the builder to look at a stream to construct the mod
    /// </summary>
    /// <param name="stream">Stream to read the mod data from</param>
    /// <param name="modKey">ModKey to use when reading the mod data</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderSeparatedChoice<TMod, TModGetter, TGroupMask> FromStream(
        Stream stream,
        ModKey modKey)
    {
        return new BinaryReadBuilderSeparatedChoice<TMod, TModGetter, TGroupMask>(
            new BinaryReadBuilderParams<TMod, TModGetter, TGroupMask>()
            {
                _instantiator = _instantiator,
                GameRelease = _release,
                ModKey = modKey,
                _stream = stream,
                _needsRecordTypeInfoCacheReader = _needsRecordTypeInfoCacheReader
            });
    }
}

/// <summary>
/// Choice of whether to provide a load order
/// </summary>
public class BinaryReadBuilderSeparatedChoice<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : class, IModDisposeGetter
{
    private readonly BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> _param;

    internal BinaryReadBuilderSeparatedChoice(
        BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> param)
    {
        _param = param;
    }
    
    /// <summary>
    /// Provides a load order of mod objects to look to. <br />
    /// This is used to construct the separated load order needed to interpret FormIDs. <br />
    /// It is expected to contain all of the mods that this mod has as masters. 
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithDefaultLoadOrder()
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = static (param) => GameLocator.Instance.GetDataDirectory(param.GameRelease),
            _loadOrderSetter = static (param) =>
            {
                var dataFolder = param._dataFolderGetter?.Invoke(param) ?? throw new ArgumentNullException("Data folder source was not set");
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder, 
                    param.GameRelease, 
                    param.Params.FileSystem);   
                return lo.ListedOrder.ResolveExistingMods();
            }
        });
    }

    /// <summary>
    /// Provides a load order of mod objects to look to. <br />
    /// This is used to construct the separated load order needed to interpret FormIDs. <br />
    /// It is expected to contain all of the mods that this mod has as masters. 
    /// </summary>
    /// <param name="loadOrder">Load order to refer to when parsing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask> WithLoadOrder(IEnumerable<ModKey>? loadOrder)
    {
        return WithLoadOrder(loadOrder?.ToArray() ?? Array.Empty<ModKey>());
    }

    /// <summary>
    /// Provides a load order of mod objects to look to. <br />
    /// This is used to construct the separated load order needed to interpret FormIDs. <br />
    /// It is expected to contain all of the mods that this mod has as masters. 
    /// </summary>
    /// <param name="loadOrder">Load order to refer to when parsing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask> WithLoadOrder(params ModKey[] loadOrder)
    {
        return new BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask>(_param with
        {
            _loadOrderSetter = (param) =>
            {
                if (loadOrder.Length == 0)
                {
                    return Array.Empty<IModFlagsGetter>();
                }
                
                var dataFolder = param._dataFolderGetter?.Invoke(param);
                if (dataFolder == null)
                {
                    return Array.Empty<IModFlagsGetter>();
                }
                
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder.Value, loadOrder,
                    param.GameRelease, param.Params.FileSystem);
                return lo.ListedOrder.ResolveExistingMods();
            }
        });
    }

    /// <summary>
    /// Provides a load order of mod objects to look to. <br />
    /// This is used to construct the separated load order needed to interpret FormIDs. <br />
    /// It is expected to contain all of the mods that this mod has as masters. 
    /// </summary>
    /// <param name="loadOrder">Load order to refer to when parsing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask> WithLoadOrder(IEnumerable<IModFlagsGetter>? loadOrder)
    {
        return WithLoadOrder(loadOrder?.ToArray() ?? Array.Empty<IModFlagsGetter>());
    }

    /// <summary>
    /// Provides a load order of mod objects to look to. <br />
    /// This is used to construct the separated load order needed to interpret FormIDs. <br />
    /// It is expected to contain all of the mods that this mod has as masters. 
    /// </summary>
    /// <param name="loadOrder">Load order to refer to when parsing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask> WithLoadOrder(params IModFlagsGetter[] loadOrder)
    {
        return new BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask>(_param with
        {
            _loadOrderSetter = (param) =>
            {
                if (loadOrder.Length == 0)
                {
                    return Array.Empty<IModFlagsGetter>();
                }
                
                var dataFolder = param._dataFolderGetter?.Invoke(param);
                if (dataFolder == null)
                {
                    return Array.Empty<IModFlagsGetter>();
                }
                
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder.Value, loadOrder.Select(x => x.ModKey),
                    param.GameRelease, param.Params.FileSystem);
                return lo.ListedOrder.ResolveExistingMods();
            }
        });
    }

    /// <summary>
    /// Provides a load order of mod objects to look to. <br />
    /// This is used to construct the separated load order needed to interpret FormIDs. <br />
    /// It is expected to contain all of the mods that this mod has as masters. 
    /// </summary>
    /// <param name="loadOrder">Load order to refer to when parsing</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithLoadOrder(ILoadOrderGetter<IModFlagsGetter>? loadOrder)
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(
            _param with
            {
                Params = _param.Params with
                {
                    LoadOrder = loadOrder
                }
            });
    }

    /// <summary>
    /// Opt to not provide a load order. <br />
    /// WARNING:  This can lead to corrupted content if the mod contains references to light or half masters.
    /// It is useful in certain controlled circumstances.  Use at your own risk
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithNoLoadOrder()
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param);
    }

    /// <summary>
    /// Writes the mod with the load order found in mod header, and with given data folder as reference.
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask> WithLoadOrderFromHeaderMasters()
    {
        return new BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask>(_param with
        {
            _loadOrderSetter = static (param) =>
            {
                var dataFolder = param._dataFolderGetter?.Invoke(param);
                if (dataFolder == null)
                {
                    return Array.Empty<IModFlagsGetter>();
                }
                
                ModHeaderFrame modHeader;
                if (param._path != null)
                {
                    modHeader = ModHeaderFrame.FromPath(
                        new ModPath(param.ModKey, param._path.Value), param.GameRelease,
                        readSafe: true,
                        param.Params.FileSystem);
                }
                else if (param._stream != null)
                {
                    var pos = param._stream.Position;
                    modHeader = ModHeaderFrame.FromStream(param._stream, param.ModKey, param.GameRelease);
                    param._stream.Position = pos;
                }
                else if (param._streamFactory != null)
                {
                    using var stream = param._streamFactory();
                    modHeader = ModHeaderFrame.FromStream(stream, param.ModKey, param.GameRelease);
                }
                else
                {
                    throw new ArgumentException("Parameters didn't define any filepath or streams");
                }

                var masters = MasterReferenceCollection.FromModHeader(
                    param.ModKey,
                    modHeader);
                var lo = LoadOrder.Import<TModGetter>(
                    dataFolder.Value, masters.Masters.Select(x => x.Master),
                    param.GameRelease, param.Params.FileSystem);
                return lo.ListedOrder.ResolveExistingMods();
            }
        });
    }
}

/// <summary>
/// Choice of Data folder location
/// </summary>
public class BinaryReadBuilderDataFolderChoice<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : class, IModDisposeGetter
{
    private readonly BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> _param;

    internal BinaryReadBuilderDataFolderChoice(
        BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> param)
    {
        _param = param;
    }
    
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithDefaultDataFolder()
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = static (param) => GameLocator.Instance.GetDataDirectory(param.GameRelease)
        });
    }
    
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param);
        }
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = (param) => dataFolder.Value
        });
    }
    
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithNoDataFolder()
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param);
    }
}

public record BinaryReadBuilder<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : IModDisposeGetter
{
    internal BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> _param { get; set; }

    internal BinaryReadBuilder(
        BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> param)
    {
        _param = param;
    }

    /// <summary>
    /// Executes the instructions to read the mod into a readonly object. <br />
    /// <br /> 
    /// This is a lazy loading object, with does minimal work up front, and does any
    /// parsing work as fields are accessed.<br />
    /// There is no caching, so every access will reparse the data. <br />
    /// If you want a mutable version of the mod, call Mutable() first.
    /// </summary>
    /// <returns>A readonly mod object with minimal initial parsing done</returns>
    public TModGetter Construct()
    {
        _param = BinaryReadBuilderHelper.RunLoadOrderSetter(_param);
        return _param._instantiator.Readonly(this);
    }

    /// <summary>
    /// Specifies that a mutable version of the mod should be returned.<br />
    /// <br />
    /// Note that this loads in the entire mod up front, which takes longer and uses more memory. <br />
    /// Use this call only if you intend to mutate the mod after loading it, otherwise use the
    /// non-mutable alternative. 
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> Mutable()
    {
        return new BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask>(_param);
    }

    #region Common

    /// <summary>
    /// Normal string folder path locations will be ignored in favor of the path provided.
    /// </summary>
    /// <param name="dir">Directory to look for strings files within</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithStringsFolder(DirectoryPath dir)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        StringsFolderOverride = dir
                    }
                }
            }
        };
    }

    /// <summary>
    /// Normal string folder path locations will be ignored in favor of the path provided.
    /// </summary>
    /// <param name="param">Strings parameter object</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithStringsParameters(StringsReadParameters param)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = param
                }
            }
        };
    }

    /// <summary>
    /// Normal bsa folder path locations will be ignored in favor of the path provided.
    /// </summary>
    /// <param name="dir">Directory to look for strings files within</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithBsaFolder(DirectoryPath dir)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        BsaFolderOverride = dir
                    }
                }
            }
        };
    }

    /// <summary>
    /// How to order BSAs when searching for content.<br/>
    /// Null is the default, which will fall back on typical ini files for the bsa ordering.
    /// </summary>
    /// <param name="ordering">Filename order to consider BSAs with</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithBsaOrdering(IEnumerable<FileName> ordering)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        BsaOrdering = ordering
                    }
                }
            }
        };
    }

    /// <summary>
    /// Overrides the string encodings to be used
    /// </summary>
    /// <param name="encodingProvider">Encoding provider to use for strings</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithEncoding(IMutagenEncodingProvider encodingProvider)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        EncodingProvider = encodingProvider
                    }
                }
            }
        };
    }

    /// <summary>
    /// Controls a few things:<br/>
    /// 1)  What language TranslatedString members query when their `String` members are accessed
    /// 2)  What language a non-localized TranslatedString will be interpreted as when exported in a now localized context.
    /// </summary>
    /// <param name="targetLanguage">Language to target</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithTargetLanguage(Language targetLanguage)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        TargetLanguage = targetLanguage
                    }
                }
            }
        };
    }

    /// <summary>
    /// Overrides what encoding to be used for strings that have no translation concepts
    /// </summary>
    /// <param name="nonTranslatedEncoding">Object to use when encoding to be used for strings that have no translation concepts</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithNonTranslatedEncoding(IMutagenEncoding nonTranslatedEncoding)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        NonTranslatedEncodingOverride = nonTranslatedEncoding
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Overrides what encoding to be used for TranslatedStrings that are not localized
    /// </summary>
    /// <param name="nonLocalizedEncoding">Object to use when encoding to be used for strings that have no translation concepts</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithNonLocalizedEncoding(IMutagenEncoding nonLocalizedEncoding)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        NonLocalizedEncodingOverride = nonLocalizedEncoding
                    }
                }
            }
        };
    }

    /// <summary>
    /// Forces parsing to be done on a single thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> SingleThread()
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    Parallel = false
                }
            }
        };
    }

    /// <summary>
    /// Sets the import systems to run in parallel when possible
    /// </summary>
    /// <param name="parallel">Whether it should import in parallel</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> Parallel(bool parallel = true)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    Parallel = parallel
                }
            }
        };
    }

    /// <summary>
    /// Throws an exception if an unknown subrecord is encountered
    /// </summary>
    /// <param name="shouldThrow">Whether it should throw</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> ThrowIfUnknownSubrecord(bool shouldThrow = true)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    ThrowOnUnknownSubrecord = shouldThrow
                }
            }
        };
    }
    
    /// <summary>
    /// Sets the filesystem to use
    /// </summary>
    /// <param name="fileSystem">FileSystem to read from</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithFileSystem(IFileSystem? fileSystem)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    FileSystem = fileSystem
                }
            }
        };
    }

    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithDefaultDataFolder()
    {
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = static (param) => GameLocator.Instance.GetDataDirectory(param.GameRelease)
        });
    }
    
    public BinaryReadBuilder<TMod, TModGetter, TGroupMask> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param);
        }
        return new BinaryReadBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = (param) => dataFolder.Value
        });
    }
    
    #endregion
}

public record BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> : BinaryReadBuilder<TMod, TModGetter, TGroupMask>
    where TMod : IMod
    where TModGetter : IModDisposeGetter
{
    internal BinaryReadMutableBuilder(
        BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> param)
        : base(param)
    {
    }
    
    #region Common

    /// <summary>
    /// Normal string folder path locations will be ignored in favor of the path provided.
    /// </summary>
    /// <param name="dir">Directory to look for strings files within</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithStringsFolder(DirectoryPath dir)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        StringsFolderOverride = dir
                    }
                }
            }
        };
    }

    /// <summary>
    /// Normal string folder path locations will be ignored in favor of the path provided.
    /// </summary>
    /// <param name="param">Strings parameter object</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithStringsParameters(StringsReadParameters param)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = param
                }
            }
        };
    }

    /// <summary>
    /// Normal bsa folder path locations will be ignored in favor of the path provided.
    /// </summary>
    /// <param name="dir">Directory to look for strings files within</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithBsaFolder(DirectoryPath dir)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        BsaFolderOverride = dir
                    }
                }
            }
        };
    }

    /// <summary>
    /// How to order BSAs when searching for content.<br/>
    /// Null is the default, which will fall back on typical ini files for the bsa ordering.
    /// </summary>
    /// <param name="ordering">Filename order to consider BSAs with</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithBsaOrdering(IEnumerable<FileName> ordering)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        BsaOrdering = ordering
                    }
                }
            }
        };
    }

    /// <summary>
    /// Overrides the string encodings to be used
    /// </summary>
    /// <param name="encodingProvider">Encoding provider to use for strings</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithEncoding(IMutagenEncodingProvider encodingProvider)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        EncodingProvider = encodingProvider
                    }
                }
            }
        };
    }

    /// <summary>
    /// Controls a few things:<br/>
    /// 1)  What language TranslatedString members query when their `String` members are accessed
    /// 2)  What language a non-localized TranslatedString will be interpreted as when exported in a now localized context.
    /// </summary>
    /// <param name="targetLanguage">Language to target</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithTargetLanguage(Language targetLanguage)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        TargetLanguage = targetLanguage
                    }
                }
            }
        };
    }

    /// <summary>
    /// Overrides what encoding to be used for strings that have no translation concepts
    /// </summary>
    /// <param name="nonTranslatedEncoding">Object to use when encoding to be used for strings that have no translation concepts</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithNonTranslatedEncoding(IMutagenEncoding nonTranslatedEncoding)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        NonTranslatedEncodingOverride = nonTranslatedEncoding
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Overrides what encoding to be used for TranslatedStrings that are not localized
    /// </summary>
    /// <param name="nonLocalizedEncoding">Object to use when encoding to be used for strings that have no translation concepts</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithNonLocalizedEncoding(IMutagenEncoding nonLocalizedEncoding)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                {
                    StringsParam = (_param.Params.StringsParam ?? new()) with
                    {
                        NonLocalizedEncodingOverride = nonLocalizedEncoding
                    }
                }
            }
        };
    }

    /// <summary>
    /// Forces parsing to be done on a single thread
    /// </summary>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> SingleThread()
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    Parallel = false
                }
            }
        };
    }

    /// <summary>
    /// Sets the import systems to run in parallel when possible
    /// </summary>
    /// <param name="parallel">Whether it should import in parallel</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> Parallel(bool parallel = true)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    Parallel = parallel
                }
            }
        };
    }

    /// <summary>
    /// Throws an exception if an unknown subrecord is encountered
    /// </summary>
    /// <param name="shouldThrow">Whether it should throw</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> ThrowIfUnknownSubrecord(bool shouldThrow = true)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    ThrowOnUnknownSubrecord = shouldThrow
                }
            }
        };
    }
    
    /// <summary>
    /// Sets the filesystem to use
    /// </summary>
    /// <param name="fileSystem">FileSystem to read from</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithFileSystem(IFileSystem fileSystem)
    {
        return this with
        {
            _param = _param with
            {
                Params = _param.Params with
                { 
                    FileSystem = fileSystem
                }
            }
        };
    }

    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithDefaultDataFolder()
    {
        return new BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = static (param) => GameLocator.Instance.GetDataDirectory(param.GameRelease)
        });
    }
    
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithDataFolder(DirectoryPath? dataFolder)
    {
        if (dataFolder == null)
        {
            return new BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask>(_param);
        }
        return new BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask>(_param with
        {
            _dataFolderGetter = (param) => dataFolder.Value
        });
    }
    
    #endregion
    
    /// <summary>
    /// Adds an error mask builder, which helps debug erroring fields when loading
    /// the entire mod
    /// </summary>
    /// <param name="errorMask">Error mask builder to report errors to</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithErrorMask(ErrorMaskBuilder? errorMask)
    {
        return this with
        {
            _param = _param with
            {
                ErrorMaskBuilder = errorMask
            }
        };
    }
    
    /// <summary>
    /// Provides masking to only import certain groups, and skip others.
    /// </summary>
    /// <param name="mask">Mask of which groups to import</param>
    /// <returns>Builder object to continue customization</returns>
    public BinaryReadMutableBuilder<TMod, TModGetter, TGroupMask> WithGroupMask(TGroupMask mask)
    {
        return this with
        {
            _param = _param with
            {
                GroupMask = mask
            }
        };
    }

    /// <summary>
    /// Executes the instructions to read the mod into a mutable object. <br />
    /// <br />
    /// Note that this loads in the entire mod up front, which takes longer and uses more memory. <br />
    /// Use this call only if you intend to mutate the mod after loading it, otherwise use the
    /// non-mutable alternative. 
    /// </summary>
    /// <returns>A mutable mod object with all the data loaded</returns>
    public new TMod Construct()
    {
        _param = BinaryReadBuilderHelper.RunLoadOrderSetter(_param);
        return _param._instantiator.Mutable(this);
    }
}

internal static class BinaryReadBuilderHelper
{
    public static BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> RunLoadOrderSetter<TMod, TModGetter, TGroupMask>(
        BinaryReadBuilderParams<TMod, TModGetter, TGroupMask> p)
        where TMod : IMod
        where TModGetter : IModDisposeGetter
    {
        IReadOnlyCollection<IModMasterStyled> loadOrder = Array.Empty<IModMasterStyled>(); 
        
        if (p._loadOrderSetter != null)
        {
            loadOrder = p._loadOrderSetter(p).ToArray();
        }

        if (loadOrder.Count == 0)
        {
            return p;
        }

        return p with
        {
            Params = p.Params with
            {
                LoadOrder = new LoadOrder<IModMasterStyled>(
                    loadOrder.Distinct(x => x.ModKey))
            }
        };
    }
}