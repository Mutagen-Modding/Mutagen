using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Assets;

public static class AssetLinkCacheConstructionMixIn
{
    /// <summary>
    /// Creates an AssetLinkCache, which should only be used during sections of code where underlying records
    /// do not change.
    /// </summary>
    /// <param name="linkCache">LinkCache to resolve against for Resolved assets</param>
    /// <returns>An AssetLinkCache relative to the given ILinkCache</returns>
    public static IAssetLinkCache CreateImmutableAssetLinkCache(this ILinkCache linkCache)
    {
        return new AssetLinkCache(linkCache);
    }
}