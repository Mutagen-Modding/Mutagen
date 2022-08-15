using Mutagen.Bethesda.Plugins.Cache;
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
        AssetLinkQuery queryCategories = AssetLinkQuery.Listed, 
        IAssetLinkCache? linkCache = null, 
        Type? assetType = null);
}

public static class AssetLinkContainerGetterExt
{
    public static IEnumerable<TAsset> EnumerateAssetLinks<TAsset>(
        this IAssetLinkContainerGetter assetLinkContainerGetter,
        AssetLinkQuery queryCategories = AssetLinkQuery.Listed,
        IAssetLinkCache? linkCache = null)
        where TAsset : IAssetLinkGetter
    {
        return assetLinkContainerGetter.EnumerateAssetLinks(queryCategories, linkCache, typeof(TAsset))
            .WhereCastable<IAssetLinkGetter, TAsset>();
    }
}