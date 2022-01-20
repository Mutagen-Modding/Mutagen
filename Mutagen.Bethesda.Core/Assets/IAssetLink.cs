namespace Mutagen.Bethesda.Assets; 

public interface IAssetPath {
    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    string RawPath { get; }
    
    /// <summary>
    /// Raw path relative to the game's data directory
    /// </summary>
    string DataRelativePath { get; }
    
    /// <summary>
    /// Extension of the asset
    /// </summary>
    string Extension { get; }
}

public interface IAssetLinkGetter : IAssetPath {
    IAssetType AssetType { get; }
}

public interface IAssetLinkGetter<TAssetType> where TAssetType : IAssetType { }

public interface IAssetLink : IAssetLinkGetter {
    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    new IAssetType AssetType { get; set; }
    /// <summary>
    /// Raw path relative to the game's data directory
    /// </summary>
    new string RawPath { get; set; }
    /// <summary>
    /// Extension of the asset
    /// </summary>
    
    void SetTo(string? path);

    void SetToNull();
}

public interface IAssetLink<TAssetType> where TAssetType : IAssetType { }