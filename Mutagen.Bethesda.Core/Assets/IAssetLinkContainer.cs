using Noggog;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// An interface for classes that contain AssetLinks and can enumerate them.
/// </summary>
public interface IAssetLinkContainer : IAssetLinkContainerGetter
{
    /// <summary>
    /// Swaps out all links to point to new assets
    /// </summary>
    void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping);

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
    public static IEnumerable<TAsset> EnumerateAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        AssetLinkQuery queryCategories,
        IAssetLinkCache? linkCache = null)
        where TAsset : IAssetLinkGetter
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(queryCategories, linkCache, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, TAsset>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateListedAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Listed, linkCache: null, assetType);
    }
    
    public static IEnumerable<TAsset> EnumerateListedAssetLinks<TAsset>(this IAssetLinkContainerGetter assetLinkContainerGetter)
        where TAsset : IAssetLinkGetter
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Listed, linkCache: null, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, TAsset>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateInferredAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Inferred, linkCache: null, assetType);
    }
    
    public static IEnumerable<TAsset> EnumerateInferredAssetLinks<TAsset>(this IAssetLinkContainerGetter assetLinkContainerGetter)
        where TAsset : IAssetLinkGetter
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Inferred, linkCache: null, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, TAsset>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateResolvedAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Resolved, linkCache: linkCache, assetType);
    }
    
    public static IEnumerable<TAsset> EnumerateResolvedAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache)
        where TAsset : IAssetLinkGetter
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(AssetLinkQuery.Resolved, linkCache: linkCache, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, TAsset>();
    }
    
    public static IEnumerable<IAssetLinkGetter> EnumerateAllAssetLinks(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache,
        Type? assetType = null)
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(
            AssetLinkQuery.Resolved | AssetLinkQuery.Inferred | AssetLinkQuery.Resolved, 
            linkCache: linkCache,
            assetType);
    }
    
    public static IEnumerable<TAsset> EnumerateAllAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        IAssetLinkCache? linkCache)
        where TAsset : IAssetLinkGetter
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(
                AssetLinkQuery.Resolved | AssetLinkQuery.Inferred | AssetLinkQuery.Resolved, 
                linkCache: linkCache, 
                typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, TAsset>();
    }
}