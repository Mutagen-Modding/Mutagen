using System;
namespace Mutagen.Bethesda.Assets;

public interface IAssetPath
{
    protected static readonly StringComparison PathComparison = StringComparison.OrdinalIgnoreCase;
    protected static readonly StringComparer PathComparer = StringComparer.FromComparison(PathComparison);
    protected static readonly string NullPath = string.Empty;
    
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

public interface IAssetLinkGetter<out TAssetType> : IAssetPath
    where TAssetType : IAssetType
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
    /// Raw path pointing to the asset
    /// </summary>
    new string RawPath { get; set; }
}

public interface IAssetLink : IAssetLink<IAssetType>
{
}