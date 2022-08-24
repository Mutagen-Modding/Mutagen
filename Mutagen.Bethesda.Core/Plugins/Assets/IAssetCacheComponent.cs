namespace Mutagen.Bethesda.Plugins.Assets;

/// <summary>
/// Marker interface for classes that are involved in caching and resolving assets for an AssetLinkCache
/// </summary>
public interface IAssetCacheComponent
{
    void Prep(IAssetLinkCache assetLinkCache);
}