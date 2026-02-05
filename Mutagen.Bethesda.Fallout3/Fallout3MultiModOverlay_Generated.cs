using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Noggog.StructuredStrings;
using Loqui.Internal;

namespace Mutagen.Bethesda.Fallout3;

/// <summary>
/// Multi-mod overlay that presents multiple Fallout3 mods as a single unified mod.
/// Typically used for reading split mods that were written due to exceeding master limits,
/// but can be used with any collection of mods.
/// </summary>
internal class Fallout3MultiModOverlay : IFallout3ModDisposableGetter
{
    private readonly IReadOnlyList<IFallout3ModGetter> _sourceMods;
    private readonly IReadOnlyList<IModDisposeGetter>? _disposeSourceMods;
    private readonly ModKey _modKey;
    private readonly IReadOnlyList<IMasterReferenceGetter> _masters;

    private MergedGroup<IGameSettingGetter>? _gameSettings;
    private MergedGroup<ITextureSetGetter>? _textureSets;
    private MergedGroup<IMenuIconGetter>? _menuIcons;
    private MergedGroup<IGlobalGetter>? _globals;
    private MergedGroup<IClassGetter>? _classes;
    private MergedGroup<IFactionGetter>? _factions;
    private MergedGroup<IHeadPartGetter>? _headParts;
    private MergedGroup<IHairGetter>? _hairs;
    private MergedGroup<IEyesGetter>? _eyes;
    private MergedGroup<IRaceGetter>? _races;
    private MergedGroup<ISoundGetter>? _sounds;
    private MergedGroup<IAcousticSpaceGetter>? _acousticSpaces;
    private MergedGroup<IMagicEffectGetter>? _magicEffects;

    /// <summary>
    /// Creates a new Fallout3MultiModOverlay from multiple source mod files.
    /// </summary>
    public Fallout3MultiModOverlay(
        ModKey modKey,
        IEnumerable<IFallout3ModGetter> sourceMods,
        IReadOnlyList<IMasterReferenceGetter> mergedMasters)
    {
        _modKey = modKey;
        var sourceList = sourceMods.ToList();
        _sourceMods = sourceList;
        _masters = mergedMasters;

        // Track disposable mods for cleanup
        var disposables = sourceList.OfType<IModDisposeGetter>().ToList();
        _disposeSourceMods = disposables.Count > 0 ? disposables : null;

        if (_sourceMods.Count == 0)
        {
            throw new ArgumentException("Must provide at least one source mod", nameof(sourceMods));
        }
    }

    public ModKey ModKey => _modKey;
    public IFallout3ModHeaderGetter ModHeader => _sourceMods[0].ModHeader;
    public IReadOnlyList<IMasterReferenceGetter> MasterReferences => _masters;
    public Fallout3Release Fallout3Release => _sourceMods[0].Fallout3Release;
    GameRelease IModGetter.GameRelease => Fallout3Release.ToGameRelease();

    public object CommonInstance() => Fallout3ModCommon.Instance;
    public object? CommonSetterInstance() => Fallout3ModSetterCommon.Instance;
    public object CommonSetterTranslationInstance() => Fallout3ModSetterTranslationCommon.Instance;


    public IFallout3GroupGetter<IGameSettingGetter> GameSettings =>
        _gameSettings ??= new MergedGroup<IGameSettingGetter>(
            _sourceMods.Select(m => m.GameSettings));
    public IFallout3GroupGetter<ITextureSetGetter> TextureSets =>
        _textureSets ??= new MergedGroup<ITextureSetGetter>(
            _sourceMods.Select(m => m.TextureSets));
    public IFallout3GroupGetter<IMenuIconGetter> MenuIcons =>
        _menuIcons ??= new MergedGroup<IMenuIconGetter>(
            _sourceMods.Select(m => m.MenuIcons));
    public IFallout3GroupGetter<IGlobalGetter> Globals =>
        _globals ??= new MergedGroup<IGlobalGetter>(
            _sourceMods.Select(m => m.Globals));
    public IFallout3GroupGetter<IClassGetter> Classes =>
        _classes ??= new MergedGroup<IClassGetter>(
            _sourceMods.Select(m => m.Classes));
    public IFallout3GroupGetter<IFactionGetter> Factions =>
        _factions ??= new MergedGroup<IFactionGetter>(
            _sourceMods.Select(m => m.Factions));
    public IFallout3GroupGetter<IHeadPartGetter> HeadParts =>
        _headParts ??= new MergedGroup<IHeadPartGetter>(
            _sourceMods.Select(m => m.HeadParts));
    public IFallout3GroupGetter<IHairGetter> Hairs =>
        _hairs ??= new MergedGroup<IHairGetter>(
            _sourceMods.Select(m => m.Hairs));
    public IFallout3GroupGetter<IEyesGetter> Eyes =>
        _eyes ??= new MergedGroup<IEyesGetter>(
            _sourceMods.Select(m => m.Eyes));
    public IFallout3GroupGetter<IRaceGetter> Races =>
        _races ??= new MergedGroup<IRaceGetter>(
            _sourceMods.Select(m => m.Races));
    public IFallout3GroupGetter<ISoundGetter> Sounds =>
        _sounds ??= new MergedGroup<ISoundGetter>(
            _sourceMods.Select(m => m.Sounds));
    public IFallout3GroupGetter<IAcousticSpaceGetter> AcousticSpaces =>
        _acousticSpaces ??= new MergedGroup<IAcousticSpaceGetter>(
            _sourceMods.Select(m => m.AcousticSpaces));
    public IFallout3GroupGetter<IMagicEffectGetter> MagicEffects =>
        _magicEffects ??= new MergedGroup<IMagicEffectGetter>(
            _sourceMods.Select(m => m.MagicEffects));

    BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter> IFallout3ModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter>(this, Fallout3Mod.Fallout3WriteBuilderInstantiator.Instance);

    IBinaryModdedWriteBuilderTargetChoice IModGetter.BeginWrite =>
        new BinaryModdedWriteBuilderTargetChoice<IFallout3ModGetter>(this, Fallout3Mod.Fallout3WriteBuilderInstantiator.Instance);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var link in mod.EnumerateAssetLinks(queryCategories, linkCache, assetType))
            {
                yield return link;
            }
        }
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var link in mod.EnumerateFormLinks())
            {
                yield return link;
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var record in mod.EnumerateMajorRecords())
            {
                yield return record;
            }
        }
    }

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var record in mod.EnumerateMajorRecords<T>(throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var record in mod.EnumerateMajorRecords(type, throwIfUnknown))
            {
                yield return record;
            }
        }
    }

    IGroupGetter<TMajor>? IModGetter.TryGetTopLevelGroup<TMajor>()
    {
        return (IGroupGetter<TMajor>?)((Fallout3ModCommon)((IFallout3ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: typeof(TMajor));
    }

    public IGroupGetter? TryGetTopLevelGroup(Type type)
    {
        return (IGroupGetter?)((Fallout3ModCommon)((IFallout3ModGetter)this).CommonInstance()!).GetGroup(
            obj: this,
            type: type);
    }

    IEnumerable<IModContext<IFallout3Mod, IFallout3ModGetter, TSetter, TGetter>> IMajorRecordContextEnumerable<IFallout3Mod, IFallout3ModGetter>.EnumerateMajorRecordContexts<TSetter, TGetter>(ILinkCache linkCache, bool throwIfUnknown)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordContexts<TSetter, TGetter>(linkCache, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    IEnumerable<IModContext<IFallout3Mod, IFallout3ModGetter, IMajorRecord, IMajorRecordGetter>> IMajorRecordContextEnumerable<IFallout3Mod, IFallout3ModGetter>.EnumerateMajorRecordContexts(ILinkCache linkCache, Type type, bool throwIfUnknown)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordContexts(linkCache, type, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts()
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordSimpleContexts())
            {
                yield return context;
            }
        }
    }

    public IEnumerable<IModContext<TGetter>> EnumerateMajorRecordSimpleContexts<TGetter>(bool throwIfUnknown = true) where TGetter : class, IMajorRecordQueryableGetter
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordSimpleContexts<TGetter>(throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> EnumerateMajorRecordSimpleContexts(Type type, bool throwIfUnknown = true)
    {
        foreach (var mod in _sourceMods)
        {
            foreach (var context in mod.EnumerateMajorRecordSimpleContexts(type, throwIfUnknown))
            {
                yield return context;
            }
        }
    }

    public void WriteToBinary(FilePath path, BinaryWriteParameters? param = null) => this.WriteToBinary(path, importMask: null, param: param);

    public void WriteToBinary(Stream stream, BinaryWriteParameters? param = null) => this.WriteToBinary(stream, importMask: null, param: param);

    public uint GetDefaultInitialNextFormID(bool? isSmallMasterOverride = null)
    {
        return _sourceMods[0].GetDefaultInitialNextFormID(isSmallMasterOverride);
    }

    public uint GetRecordCount()
    {
        return (uint)_sourceMods.Sum(m => m.GetRecordCount());
    }

    IMod IModGetter.DeepCopy()
    {
        return ((Fallout3ModSetterTranslationCommon)((IFallout3ModGetter)this).CommonSetterTranslationInstance()!).DeepCopy(
            item: this,
            errorMask: null,
            copyMask: null);
    }

    #pragma warning disable CS8603 // Possible null reference return
    IReadOnlyList<IFormLinkGetter<IMajorRecordGetter>> IModGetter.OverriddenForms
    {
        get => _sourceMods.SelectMany(m => m.OverriddenForms).ToList();
    }
    #pragma warning restore CS8603

    public uint NextFormID => _sourceMods.Max(m => m.NextFormID);

    public ILoquiRegistration Registration => Fallout3Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        throw new NotSupportedException("Multi-mod overlay does not support printing.");
    }

    public IMask<bool> GetEqualsMask(object rhs, EqualsMaskHelper.Include include)
    {
        throw new NotSupportedException("Multi-mod overlay does not support equality masking.");
    }

    // IModFlagsGetter members
    public bool CanUseLocalization => _sourceMods[0].CanUseLocalization;
    public bool UsingLocalization => _sourceMods[0].UsingLocalization;
    public bool CanBeSmallMaster => _sourceMods[0].CanBeSmallMaster;
    public bool IsSmallMaster => _sourceMods[0].IsSmallMaster;
    public bool CanBeMediumMaster => _sourceMods[0].CanBeMediumMaster;
    public bool IsMediumMaster => _sourceMods[0].IsMediumMaster;
    public bool IsMaster => _sourceMods[0].IsMaster;
    public bool ListsOverriddenForms => _sourceMods[0].ListsOverriddenForms;

    // IModMasterStyledGetter
    public MasterStyle MasterStyle => _sourceMods[0].MasterStyle;

    // IDisposable
    public void Dispose()
    {
        if (_disposeSourceMods == null) return;

        foreach (var mod in _disposeSourceMods)
        {
            mod.Dispose();
        }
    }
}
/// <summary>
/// Merged group that combines multiple groups into a single unified view.
/// Validates no duplicate FormKeys exist and caches results.
/// </summary>
internal class MergedGroup<TGetter> : IFallout3GroupGetter<TGetter>, IReadOnlyCache<TGetter, FormKey>
    where TGetter : class, IFallout3MajorRecordGetter, IBinaryItem
{
    private readonly IEnumerable<IGroupGetter<TGetter>> _sourceGroups;
    private Dictionary<FormKey, TGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedGroup(IEnumerable<IGroupGetter<TGetter>> sourceGroups)
    {
        _sourceGroups = sourceGroups;
    }

    private Dictionary<FormKey, TGetter> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                var cache = new Dictionary<FormKey, TGetter>();
                foreach (var group in _sourceGroups)
                {
                    foreach (var record in group)
                    {
                        if (!cache.TryAdd(record.FormKey, record))
                        {
                            throw new SplitModException(
                                $"Duplicate FormKey {record.FormKey} found in split mods. " +
                                "This indicates corruption or an error in the splitting logic.");
                        }
                    }
                }

                _cache = cache;
                return _cache;
            }
        }
    }

    public IEnumerator<TGetter> GetEnumerator() => Cache.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;

    public bool TryGetValue(FormKey key, [MaybeNullWhen(false)] out TGetter value)
    {
        return Cache.TryGetValue(key, out value);
    }

    public TGetter this[FormKey key] => Cache[key];
    IMajorRecordGetter IGroupGetter.this[FormKey key] => this[key];

    public bool ContainsKey(FormKey key) => Cache.ContainsKey(key);
    public IEnumerable<FormKey> FormKeys => Cache.Keys;
    public Type Type => typeof(TGetter);
    public IEnumerable<TGetter> Records => Cache.Values;

    IMod IGroupGetter.SourceMod => throw new NotSupportedException("Merged group has multiple source mods, not a single source.");
    IEnumerable<TGetter> IGroupCommonGetter<TGetter>.Records => Cache.Values;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Cache.Values;
    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => Cache.Values.Cast<IMajorRecordGetter>();
    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => new MajorRecordCacheWrapper(this);

    public ILoquiRegistration ContainedRecordRegistration => _sourceGroups.First().ContainedRecordRegistration;
    public Type ContainedRecordType => typeof(TGetter);

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var record in Cache.Values)
        {
            if (record is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged groups cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged groups do not support binary writing.");

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var record in Cache.Values)
        {
            if (record is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks())
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        return Cache.Values;
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        return Cache.Values.WhereCastable<TGetter, T>();
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        return Cache.Values.Where(r => type.IsAssignableFrom(r.GetType()));
    }

    // IFallout3GroupGetter members
    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout3GroupCommon<TGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => null;
    public object CommonSetterTranslationInstance() => Fallout3GroupSetterTranslationCommon.Instance;

    GroupTypeEnum IFallout3GroupGetter<TGetter>.Type => GroupTypeEnum.Type;
    int IFallout3GroupGetter<TGetter>.LastModified => 0;
    public int Unknown => 0;

    IReadOnlyCache<TGetter, FormKey> IFallout3GroupGetter<TGetter>.RecordCache => this;
    IReadOnlyCache<TGetter, FormKey> IGroupGetter<TGetter>.RecordCache => this;

    // IReadOnlyCache explicit implementations
    IEnumerable<FormKey> IReadOnlyCache<TGetter, FormKey>.Keys => Cache.Keys;
    IEnumerable<TGetter> IReadOnlyCache<TGetter, FormKey>.Items => Cache.Values;

    TGetter? IReadOnlyCache<TGetter, FormKey>.TryGetValue(FormKey key)
    {
        return TryGetValue(key, out var value) ? value : null;
    }

    IEnumerator<IKeyValue<FormKey, TGetter>> IEnumerable<IKeyValue<FormKey, TGetter>>.GetEnumerator()
    {
        return Cache.Select(kvp => (IKeyValue<FormKey, TGetter>)new KeyValue<FormKey, TGetter>(kvp.Key, kvp.Value)).GetEnumerator();
    }

    // Wrapper to cast TGetter to IMajorRecordGetter for IGroupGetter.RecordCache
    private class MajorRecordCacheWrapper : IReadOnlyCache<IMajorRecordGetter, FormKey>
    {
        private readonly MergedGroup<TGetter> _source;

        public MajorRecordCacheWrapper(MergedGroup<TGetter> source)
        {
            _source = source;
        }

        public IMajorRecordGetter this[FormKey key] => _source[key];
        public IEnumerable<FormKey> Keys => _source.Cache.Keys;
        public IEnumerable<IMajorRecordGetter> Items => _source.Cache.Values.Cast<IMajorRecordGetter>();
        public int Count => _source.Count;
        public bool ContainsKey(FormKey key) => _source.ContainsKey(key);
        public IMajorRecordGetter? TryGetValue(FormKey key) => _source.TryGetValue(key, out var value) ? value : null;

        public IEnumerator<IKeyValue<FormKey, IMajorRecordGetter>> GetEnumerator()
        {
            return _source.Cache.Select(kvp => (IKeyValue<FormKey, IMajorRecordGetter>)new KeyValue<FormKey, IMajorRecordGetter>(kvp.Key, kvp.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // ILoquiObject
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        Fallout3GroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }
}

/// <summary>
/// Merged list group that combines multiple list groups (like Cells) into a single unified view.
/// CellBlocks from different mods are merged by BlockNumber.
/// </summary>
internal class MergedListGroup : IFallout3ListGroupGetter<ICellBlockGetter>
{
    private readonly IEnumerable<IFallout3ListGroupGetter<ICellBlockGetter>> _sourceGroups;
    private List<ICellBlockGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedListGroup(IEnumerable<IFallout3ListGroupGetter<ICellBlockGetter>> sourceGroups)
    {
        _sourceGroups = sourceGroups;
    }

    private List<ICellBlockGetter> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                // Merge CellBlocks by BlockNumber
                var blocksByNumber = new Dictionary<int, List<ICellBlockGetter>>();

                foreach (var group in _sourceGroups)
                {
                    foreach (var block in group.Records)
                    {
                        if (!blocksByNumber.ContainsKey(block.BlockNumber))
                        {
                            blocksByNumber[block.BlockNumber] = new List<ICellBlockGetter>();
                        }
                        blocksByNumber[block.BlockNumber].Add(block);
                    }
                }

                // Create merged blocks
                var result = new List<ICellBlockGetter>();
                foreach (var blockNumber in blocksByNumber.Keys.OrderBy(k => k))
                {
                    var blocksForNumber = blocksByNumber[blockNumber];
                    if (blocksForNumber.Count == 1)
                    {
                        result.Add(blocksForNumber[0]);
                    }
                    else
                    {
                        // Multiple blocks with same number - merge them
                        result.Add(new MergedCellBlock(blockNumber, blocksForNumber));
                    }
                }

                _cache = result;
                return _cache;
            }
        }
    }

    public IEnumerator<ICellBlockGetter> GetEnumerator() => Cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;
    public ICellBlockGetter this[int index] => Cache[index];
    public IReadOnlyList<ICellBlockGetter> Records => Cache;

    ILoquiRegistration ILoquiObject.Registration => Fallout3Mod_Registration.Instance;
    public static ILoquiRegistration StaticRegistration => Fallout3Mod_Registration.Instance;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        Fallout3ListGroupMixIn.Print(
            item: this,
            sb: sb,
            name: name);
    }

    // IFallout3ListGroupGetter properties
    public GroupTypeEnum Type => _sourceGroups.FirstOrDefault()?.Type ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceGroups.Max(g => g.LastModified);
    public int Unknown => 0;

    public object CommonInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout3ListGroupCommon<ICellBlockGetter>.Instance, typeof(ICellBlockGetter), type);
    public object? CommonSetterInstance(Type type) => GenericCommonInstanceGetter.Get(Fallout3ListGroupSetterCommon<ICellBlock>.Instance, typeof(ICellBlockGetter), type);
    public object CommonSetterTranslationInstance() => Fallout3ListGroupSetterTranslationCommon.Instance;

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var block in Cache)
        {
            if (block is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged groups cannot be written to binary. Write the source mods individually.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged groups do not support binary writing.");

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var block in Cache)
        {
            if (block is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks())
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords())
                {
                    yield return record;
                }
            }
        }
    }

    IEnumerable<T> IMajorRecordGetterEnumerable.EnumerateMajorRecords<T>(bool throwIfUnknown)
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords<T>(throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var block in Cache)
        {
            if (block is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }
}

/// <summary>
/// Merged cell block that combines multiple cell blocks with the same BlockNumber.
/// </summary>
internal class MergedCellBlock : ICellBlockGetter
{
    private readonly int _blockNumber;
    private readonly List<ICellBlockGetter> _sourceBlocks;
    private List<ICellSubBlockGetter>? _mergedSubBlocks;
    private readonly object _mergeLock = new object();

    public MergedCellBlock(int blockNumber, List<ICellBlockGetter> sourceBlocks)
    {
        _blockNumber = blockNumber;
        _sourceBlocks = sourceBlocks;
    }

    public int BlockNumber => _blockNumber;
    public GroupTypeEnum GroupType => _sourceBlocks.FirstOrDefault()?.GroupType ?? GroupTypeEnum.InteriorCellBlock;
    public int LastModified => _sourceBlocks.Max(b => b.LastModified);
    public int Unknown => 0;

    public IReadOnlyList<ICellSubBlockGetter> SubBlocks
    {
        get
        {
            if (_mergedSubBlocks != null) return _mergedSubBlocks;

            lock (_mergeLock)
            {
                if (_mergedSubBlocks != null) return _mergedSubBlocks;

                // Merge SubBlocks from all source blocks
                var allSubBlocks = new List<ICellSubBlockGetter>();
                foreach (var block in _sourceBlocks)
                {
                    allSubBlocks.AddRange(block.SubBlocks);
                }

                _mergedSubBlocks = allSubBlocks;
                return _mergedSubBlocks;
            }
        }
    }

    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged Cell Block {BlockNumber} ({SubBlocks.Count} sub-blocks from {_sourceBlocks.Count} source blocks)");
    }

    public object CommonInstance() => CellBlockCommon.Instance;
    public object? CommonSetterInstance() => null;
    public object CommonSetterTranslationInstance() => CellBlockSetterTranslationCommon.Instance;

    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        throw new NotSupportedException("Merged cell blocks cannot be written to binary.");
    }

    object IBinaryItem.BinaryWriteTranslator => throw new NotSupportedException("Merged cell blocks do not support binary writing.");

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories = AssetLinkQuery.Listed, IAssetLinkCache? linkCache = null, Type? assetType = null)
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IAssetLinkContainerGetter assetContainer)
            {
                foreach (var link in assetContainer.EnumerateAssetLinks(queryCategories, linkCache, assetType))
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks()
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IFormLinkContainerGetter formLinkContainer)
            {
                foreach (var link in formLinkContainer.EnumerateFormLinks())
                {
                    yield return link;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords()
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords())
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true) where T : class, IMajorRecordQueryableGetter
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords<T>(throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        foreach (var subBlock in SubBlocks)
        {
            if (subBlock is IMajorRecordGetterEnumerable enumerable)
            {
                foreach (var record in enumerable.EnumerateMajorRecords(type, throwIfUnknown))
                {
                    yield return record;
                }
            }
        }
    }
}

