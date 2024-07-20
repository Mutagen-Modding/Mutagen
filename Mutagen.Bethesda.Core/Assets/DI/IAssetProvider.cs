using System.Diagnostics.CodeAnalysis;
namespace Mutagen.Bethesda.Assets.DI;

public interface IAssetProvider
{
    /// <summary>
    /// Checks if the asset path exists in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <returns>True if the asset exists</returns>
    bool Exists(DataRelativePath assetPath);

    /// <summary>
    /// Tries to get a stream for the asset path in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <param name="stream">Resulting stream, only valid if the method returns true</param>
    /// <returns>True if the asset path exists in the context of the asset provider</returns>
    bool TryGetStream(DataRelativePath assetPath, [MaybeNullWhen(false)] out Stream stream);

    /// <summary>
    /// Tries to get the size of the asset path in the context of the asset provider
    /// </summary>
    /// <param name="assetPath">Asset path</param>
    /// <param name="size">Resulting size in bytes, only valid if the method returns true</param>
    /// <returns>True if the asset path exists in the context of the asset provider</returns>
    bool TryGetSize(DataRelativePath assetPath, out uint size);
}
