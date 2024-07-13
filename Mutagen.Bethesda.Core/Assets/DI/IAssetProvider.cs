using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Assets;
namespace Mutagen.Bethesda.Assets.DI;

public interface IAssetProvider
{
    /// <summary>
    /// Checks if the asset path exists in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <returns>True if the asset exists</returns>
    bool Exists(AssetPath assetPath);
    bool Exists(IAssetLinkGetter assetLink) => Exists(assetLink.AssetPath);

    /// <summary>
    /// Tries to get a stream for the asset path in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <param name="stream">Resulting stream, only valid if the method returns true</param>
    /// <returns>True if the asset path exists in the context of the asset provider</returns>
    bool TryGetStream(AssetPath assetPath, [MaybeNullWhen(false)] out Stream stream);
    bool TryGetStream(IAssetLinkGetter assetPath, [MaybeNullWhen(false)] out Stream stream) => TryGetStream(assetPath.AssetPath, out stream);

    /// <summary>
    /// Gets a stream for the asset path in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <returns>Resulting stream</returns>
    /// <exception cref="FileNotFoundException">Thrown if the asset path does not exist in the context of the asset provider</exception>
    Stream GetStream(AssetPath assetPath) => TryGetStream(assetPath, out var stream) ? stream : throw new FileNotFoundException($"Asset not found: {assetPath}");
    Stream GetStream(IAssetLinkGetter assetLink) => GetStream(assetLink.AssetPath);

    /// <summary>
    /// Tries to get the size of the asset path in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <param name="size">Resulting size in bytes, only valid if the method returns true</param>
    /// <returns>True if the asset path exists in the context of the asset provider</returns>
    bool TryGetSize(AssetPath assetPath, out uint size);
    bool TryGetSize(IAssetLinkGetter assetPath, out uint size) => TryGetSize(assetPath.AssetPath, out size);

    /// <summary>
    /// Gets the size of the asset path in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <returns>Resulting size in bytes</returns>
    /// <exception cref="FileNotFoundException">Thrown if the asset path does not exist in the context of the asset provider</exception>
    uint GetSize(AssetPath assetPath) => TryGetSize(assetPath, out var size) ? size : throw new FileNotFoundException($"Asset not found: {assetPath}");
    uint GetSize(IAssetLinkGetter assetLink) => GetSize(assetLink.AssetPath);
}
