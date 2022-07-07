using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda;

public static class AssetLinkRemappingMixIn
{
    public static void Relink(this IAssetLink link, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        if (mapping.TryGetValue(link, out var replacement))
        {
            link.RawPath = replacement;
        }
    }

    private static TLinkType RelinkToNew<TLinkType, TAssetType>(this TLinkType link, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
        where TLinkType : IAssetLink<TLinkType, TAssetType>
        where TAssetType : IAssetType
    {
        if (mapping.TryGetValue(link, out var replacement))
        {
            var clone = link.ShallowClone();
            clone.RawPath = replacement;
            return clone;
        }
        return link;
    }
    
    public static void RemapListedAssetLinks<TLinkType, TAssetType>(this IList<TLinkType> linkList, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
        where TLinkType : IAssetLink<TLinkType, TAssetType>
        where TAssetType : IAssetType
    {
        for (int i = 0; i < linkList.Count; i++)
        {
            linkList[i] = linkList[i].RelinkToNew<TLinkType, TAssetType>(mapping);
        }
    }
    
    public static void RemapListedAssetLinks<TItem>(this IList<IAssetLinkContainer> linkList, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        foreach (var item in linkList)
        {
            item.RemapListedAssetLinks(mapping);
        }
    }
    
    public static void RemapListedAssetLinks<TItem>(this IGenderedItemGetter<TItem?> gendered, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
        where TItem : class, IAssetLinkContainer
    {
        gendered.Male?.RemapListedAssetLinks(mapping);
        gendered.Female?.RemapListedAssetLinks(mapping);
    }

    public static void RemapListedAssetLinks<TLinkType, TAssetType>(this IGenderedItem<TLinkType> gendered, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
        where TLinkType : IAssetLink<TLinkType, TAssetType>
        where TAssetType : IAssetType
    {
        gendered.Male = gendered.Male.RelinkToNew<TLinkType, TAssetType>(mapping);
        gendered.Female = gendered.Female.RelinkToNew<TLinkType, TAssetType>(mapping);
    }

    public static void RemapListedAssetLinks<TMajorGetter>(this IReadOnlyCache<TMajorGetter, FormKey> cache, IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
        where TMajorGetter : class, IMajorRecordGetter, IAssetLinkContainer
    {
        foreach (var item in cache.Items)
        {
            item.RemapListedAssetLinks(mapping);
        }
    }
}