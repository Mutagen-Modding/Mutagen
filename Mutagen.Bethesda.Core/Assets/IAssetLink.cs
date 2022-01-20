namespace Mutagen.Bethesda.Assets;

public interface IAssetPath
{
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

public interface IAssetLinkGetter<out TAssetType> where TAssetType : IAssetType
{
    /// <summary>
    /// Type of asset`
    /// </summary>
    TAssetType AssetType { get; }
}

public interface IAssetLinkGetter : IAssetLinkGetter<IAssetType>
{
}

public interface IAssetLink<out TAssetType> : IAssetLinkGetter<TAssetType>
    where TAssetType : IAssetType
{
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

public interface IAssetLink : IAssetLink<IAssetType>
{
}