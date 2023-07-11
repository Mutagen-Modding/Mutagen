using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// File types that can be associated with Asset type
/// </summary>
public interface IAssetType
{
#if NET7_0_OR_GREATER
    static virtual IAssetType Instance => null!;
#endif

    /// <summary>
    /// Base folder(s) relative to the game's data directory
    /// </summary>
    string BaseFolder { get; }

    /// <summary>
    /// File extension this asset type is associated with
    /// </summary>
    IEnumerable<string> FileExtensions { get; }

    /// <summary>
    /// Parse asset type by game release and path
    /// </summary>
    /// <param name="gameRelease">Release of the game this asset comes from</param>
    /// <param name="path">Path of the asset</param>
    /// <returns>Instance of the parsed asset type or null if no asset type could be determined</returns>
    public static IAssetType? GetAssetType(GameRelease gameRelease, string path) {
#if NET7_0_OR_GREATER
        switch (gameRelease) {
            case GameRelease.Oblivion:
                break;
            case GameRelease.SkyrimLE:
            case GameRelease.SkyrimSE:
            case GameRelease.SkyrimSEGog:
            case GameRelease.SkyrimVR:
            case GameRelease.EnderalLE:
            case GameRelease.EnderalSE:
                
                break;
            case GameRelease.Fallout4:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameRelease), gameRelease, null);
        }
#else
#endif
        throw new NotImplementedException();
    }

    static IAssetType() {
        
    }
}