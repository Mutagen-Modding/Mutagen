﻿using Mutagen.Bethesda.Assets;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Assets;

/// <summary>
/// An interface for classes that contain AssetLinks and can enumerate them.
/// </summary>
public interface IAssetLinkContainer : IAssetLinkContainerGetter
{
    /// <summary>
    /// Swaps out all links to point to new assets
    /// </summary>
    /// <param name="mapping">Mapping to carry out</param>
    /// <param name="query">Types of asset links to apply the remapping to</param>
    /// <param name="linkCache">Asset Link Cache, which is required for Resolved asset links</param>
    void RemapAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping, AssetLinkQuery query, IAssetLinkCache? linkCache);
    
    /// <summary>
    /// Swaps out all listed links to point to new assets
    /// </summary>
    /// <param name="mapping">Mapping to carry out</param>
    void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping);

    /// <summary>
    /// Enumerates only AssetLinks that are explicitly listed in the record and can be modified directly.
    /// </summary>
    new IEnumerable<IAssetLink> EnumerateListedAssetLinks();
}

public static class AssetLinkContainerExt
{
    public static void RemapInferredAssetLinks(
        this IAssetLinkContainer assetLinkContainerGetter,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping)
    {
        assetLinkContainerGetter.RemapAssetLinks(mapping, AssetLinkQuery.Inferred, null);
    }
    
    public static void RemapResolvedAssetLinks(
        this IAssetLinkContainer assetLinkContainerGetter,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        IAssetLinkCache linkCache)
    {
        assetLinkContainerGetter.RemapAssetLinks(mapping, AssetLinkQuery.Resolved, linkCache);
    }
    
    public static void RemapAllAssetLinks(
        this IAssetLinkContainer assetLinkContainerGetter,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        IAssetLinkCache linkCache)
    {
        assetLinkContainerGetter.RemapAssetLinks(mapping, AssetLinkQuery.Listed | AssetLinkQuery.Inferred | AssetLinkQuery.Resolved, linkCache);
    }
}

/// <summary>
/// An interface for classes that contain AssetLinks and can enumerate them.
/// </summary>
public interface IAssetLinkContainerGetter
{
    IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(
        AssetLinkQuery queryCategories, 
        IAssetLinkCache? linkCache = null, 
        Type? assetType = null);
}

public static class AssetLinkContainerGetterExt
{
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        AssetLinkQuery queryCategories,
        IAssetLinkCache? linkCache = null)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(queryCategories, linkCache, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateListedAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Listed, linkCache: null, assetType);
    }
    
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateListedAssetLinks<TAsset>(this IAssetLinkContainerGetter assetLinkContainerGetter)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Listed, linkCache: null, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateInferredAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Inferred, linkCache: null, assetType);
    }
    
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateInferredAssetLinks<TAsset>(this IAssetLinkContainerGetter assetLinkContainerGetter)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Inferred, linkCache: null, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateResolvedAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache linkCache,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Resolved, linkCache: linkCache, assetType);
    }
    
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateResolvedAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache linkCache)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Resolved, linkCache: linkCache, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateAllAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache linkCache,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(
            AssetLinkQuery.Resolved | AssetLinkQuery.Inferred | AssetLinkQuery.Listed, 
            linkCache: linkCache,
            assetType);
    }
    
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateAllAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache linkCache)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(
                AssetLinkQuery.Resolved | AssetLinkQuery.Inferred | AssetLinkQuery.Listed, 
                linkCache: linkCache, 
                typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
}