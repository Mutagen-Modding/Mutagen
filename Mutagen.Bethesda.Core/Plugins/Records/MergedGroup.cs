using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Analysis.DI;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins.Records;

/// <summary>
/// Merged group that combines multiple groups into a single unified view.
/// Validates no duplicate FormKeys exist and caches results.
/// </summary>
public class MergedGroup<TMod, TModGetter> : ILoquiObject, IGroupGetter<TModGetter>, IReadOnlyCache<TModGetter, FormKey>, IBinaryItem, IFormLinkContainerGetter, IAssetLinkContainerGetter, IMajorRecordGetterEnumerable
    where TMod : class, IMajorRecord, TModGetter
    where TModGetter : class, IMajorRecordGetter, IBinaryItem
{
    private readonly IEnumerable<IGroupGetter<TModGetter>> _sourceGroups;
    private readonly IModGetter _sourceMod;
    private Dictionary<FormKey, TModGetter>? _cache;
    private readonly object _cacheLock = new object();

    public MergedGroup(IEnumerable<IGroupGetter<TModGetter>> sourceGroups, IModGetter sourceMod)
    {
        _sourceGroups = sourceGroups;
        _sourceMod = sourceMod;
    }

    private Dictionary<FormKey, TModGetter> Cache
    {
        get
        {
            if (_cache != null) return _cache;

            lock (_cacheLock)
            {
                if (_cache != null) return _cache;

                var cache = new Dictionary<FormKey, TModGetter>();
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

    public IEnumerator<TModGetter> GetEnumerator() => Cache.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Cache.Count;

    public bool TryGetValue(FormKey key, [MaybeNullWhen(false)] out TModGetter value)
    {
        return Cache.TryGetValue(key, out value);
    }

    public TModGetter this[FormKey key] => Cache[key];
    IMajorRecordGetter IGroupGetter.this[FormKey key] => this[key];

    public bool ContainsKey(FormKey key) => Cache.ContainsKey(key);

    public IEnumerable<FormKey> FormKeys => Cache.Keys;

    public Type Type => typeof(TModGetter);

    public IEnumerable<TModGetter> Records => Cache.Values;

    // IGroupGetter members
    IMod IGroupGetter.SourceMod => (_sourceMod as IMod) ?? throw new InvalidOperationException("Source mod is read-only and does not implement IMod.");
    IEnumerable<TModGetter> IGroupCommonGetter<TModGetter>.Records => Cache.Values;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Cache.Values;
    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => Cache.Values.Cast<IMajorRecordGetter>();
    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => new MajorRecordCacheWrapper(this);

    // IGroupCommonGetter members
    public ILoquiRegistration ContainedRecordRegistration => _sourceGroups.First().ContainedRecordRegistration;
    public Type ContainedRecordType => typeof(TModGetter);

    // IAssetLinkContainerGetter
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

    // IBinaryItem - write all records in the merged cache
    void IBinaryItem.WriteToBinary(MutagenWriter writer, TypedWriteParams translationParams)
    {
        foreach (var record in Cache.Values)
        {
            record.WriteToBinary(writer, translationParams);
        }
    }

    object IBinaryItem.BinaryWriteTranslator => this;

    // IFormLinkContainerGetter
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

    // ILoquiObject - use null! since we don't have a real registration for merged groups
    ILoquiRegistration ILoquiObject.Registration => null!;

    public void Print(StructuredStringBuilder sb, string? name = null)
    {
        sb.AppendLine($"Merged Group ({Count} records):");
        foreach (var record in Cache.Values.Take(10))
        {
            sb.AppendLine($"  - {record.FormKey}: {record.EditorID}");
        }
        if (Count > 10)
        {
            sb.AppendLine($"  ... and {Count - 10} more");
        }
    }

    // IMajorRecordGetterEnumerable
    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords() => Cache.Values;

    public IEnumerable<T> EnumerateMajorRecords<T>(bool throwIfUnknown = true)
        where T : class, IMajorRecordQueryableGetter
    {
        return Cache.Values.OfType<T>();
    }

    public IEnumerable<IMajorRecordGetter> EnumerateMajorRecords(Type type, bool throwIfUnknown = true)
    {
        return Cache.Values.Where(r => type.IsAssignableFrom(r.GetType()));
    }

    IReadOnlyCache<TModGetter, FormKey> IGroupGetter<TModGetter>.RecordCache => this;

    // IReadOnlyCache explicit implementations
    IEnumerable<FormKey> IReadOnlyCache<TModGetter, FormKey>.Keys => Cache.Keys;
    IEnumerable<TModGetter> IReadOnlyCache<TModGetter, FormKey>.Items => Cache.Values;

    TModGetter? IReadOnlyCache<TModGetter, FormKey>.TryGetValue(FormKey key)
    {
        return TryGetValue(key, out var value) ? value : null;
    }

    IEnumerator<IKeyValue<FormKey, TModGetter>> IEnumerable<IKeyValue<FormKey, TModGetter>>.GetEnumerator()
    {
        return Cache.Select(kvp => (IKeyValue<FormKey, TModGetter>)new KeyValue<FormKey, TModGetter>(kvp.Key, kvp.Value)).GetEnumerator();
    }

    // Wrapper to cast TModGetter to IMajorRecordGetter for IGroupGetter.RecordCache
    private class MajorRecordCacheWrapper : IReadOnlyCache<IMajorRecordGetter, FormKey>
    {
        private readonly MergedGroup<TMod, TModGetter> _source;

        public MajorRecordCacheWrapper(MergedGroup<TMod, TModGetter> source)
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
}
