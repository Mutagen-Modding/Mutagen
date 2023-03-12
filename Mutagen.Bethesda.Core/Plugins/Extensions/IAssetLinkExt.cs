using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda;

/// <summary>
/// A static class that contains extension functions for AssetLinks
/// </summary>
public static class IAssetLinkExt
{
    /// <summary>
    /// Creates a new AssetLink with the same type
    /// </summary>
    public static IAssetLink<TAssetType> AsSetter<TAssetType>(this IAssetLinkGetter<TAssetType> link)
        where TAssetType : class, IAssetType
    {
        return new AssetLink<TAssetType>(link.RawPath);
    }
}