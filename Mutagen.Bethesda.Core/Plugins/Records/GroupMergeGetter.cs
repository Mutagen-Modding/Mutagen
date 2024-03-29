﻿using System.Collections;
using Loqui;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Records;

internal sealed class GroupMergeGetter<TGroup, TMajor> : IGroupGetter<TMajor>, IReadOnlyCache<TMajor, FormKey>
    where TGroup : class, IGroupGetter<TMajor>
    where TMajor : class, IMajorRecordGetter
{
    public IReadOnlyList<TGroup> SubGroups { get; }

    public GroupMergeGetter(IReadOnlyList<TGroup> subGroups)
    {
        SubGroups = subGroups;
        if (SubGroups.Count == 0)
        {
            throw new ArgumentException($"Cannot supply an empty list of subgroups to {nameof(GroupMergeGetter<TGroup, TMajor>)}");
        }
    }

    #region IGroupGetter

    public IMod SourceMod => SubGroups[0].SourceMod;

    public TMajor this[FormKey key] => Get(key);
    IMajorRecordGetter IGroupGetter.this[FormKey key] => Get(key);

    public int Count => SubGroups.Sum(x => x.Count);
    public IEnumerable<FormKey> FormKeys => SubGroups.SelectMany(x => x.FormKeys);
    
    public IEnumerable<TMajor> Records => SubGroups.SelectMany(x => x.Records);
    IEnumerable<IMajorRecordGetter> IGroupGetter.Records => Records;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Records;

    public IEnumerator<TMajor> GetEnumerator() => Records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IReadOnlyCache<TMajor, FormKey> RecordCache => this;
    IReadOnlyCache<IMajorRecordGetter, FormKey> IGroupGetter.RecordCache => this;

    public IEnumerable<IFormLinkGetter> EnumerateFormLinks() => SubGroups.SelectMany(x => x.EnumerateFormLinks());

    public IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(
        AssetLinkQuery queryCategories = AssetLinkQuery.Listed,
        IAssetLinkCache? linkCache = null,
        Type? assetType = null)
    {
        return SubGroups.SelectMany(x => x.EnumerateAssetLinks(queryCategories, linkCache, assetType));
    }

    private TMajor Get(FormKey key)
    {
        foreach (var subGroup in SubGroups)
        {
            if (subGroup.TryGetValue(key, out var val)) return val;
        }

        throw new KeyNotFoundException();
    }
    
    public bool ContainsKey(FormKey key)
    {
        foreach (var subGroup in SubGroups)
        {
            if (subGroup.ContainsKey(key)) return true;
        }

        return false;
    }

    public ILoquiRegistration ContainedRecordRegistration => SubGroups[0].ContainedRecordRegistration;
    public Type ContainedRecordType => typeof(TMajor);

    #endregion

    #region Cache

    public IEnumerable<FormKey> Keys => SubGroups.SelectMany(x => x.FormKeys);
    public IEnumerable<TMajor> Items => Records;

    IEnumerator<IKeyValue<FormKey, TMajor>> IEnumerable<IKeyValue<FormKey, TMajor>>.GetEnumerator() =>
        SubGroups.SelectMany(x => x.RecordCache).GetEnumerator();

    public TMajor? TryGetValue(FormKey key)
    {
        foreach (var subGroup in SubGroups)
        {
            if (subGroup.TryGetValue(key, out var rec)) return rec;
        }

        return null;
    }

    #endregion
}