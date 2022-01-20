using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Assets; 

/// <summary>
/// File types that can be associated with Asset type
/// </summary>
public interface IAssetType {
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
        throw new NotImplementedException();
    }
}