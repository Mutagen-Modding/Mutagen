using Mutagen.Bethesda.Assets;
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
    void RemapAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping, AssetLinkQuery query);

    /// <summary>
    /// Enumerates only AssetLinks that are explicitly listed in the record and can be modified directly.
    /// </summary>
    new IEnumerable<IAssetLink> EnumerateListedAssetLinks();
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
        IAssetLinkCache? linkCache,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Resolved, linkCache: linkCache, assetType);
    }
    
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateResolvedAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Resolved, linkCache: linkCache, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateAllAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(
            AssetLinkQuery.Resolved | AssetLinkQuery.Inferred | AssetLinkQuery.Listed, 
            linkCache: linkCache,
            assetType);
    }
    
    public static IEnumerable<IAssetLinkGetter<TAsset>> EnumerateAllAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache)
        where TAsset : IAssetType
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(
                AssetLinkQuery.Resolved | AssetLinkQuery.Inferred | AssetLinkQuery.Listed, 
                linkCache: linkCache, 
                typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, IAssetLinkGetter<TAsset>>();
    }
}