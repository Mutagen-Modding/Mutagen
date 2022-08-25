using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Plugins.Assets;

public interface IAssetLinkGetter
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

    /// <summary>
    /// Extension of the asset
    /// </summary>
    IAssetType Type { get; }
}

public interface IAssetLinkGetter<out TAssetType> : IAssetLinkGetter
    where TAssetType : IAssetType
{
}

public interface IAssetLink<out TAssetType> : IAssetLink<IAssetLink<TAssetType>, TAssetType>
    where TAssetType : IAssetType
{
    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    new string RawPath { get; set; }
}

public interface IAssetLink<out TLinkType, out TAssetType> : 
    IAssetLink, 
    IAssetLinkGetter<TAssetType>
    where TAssetType : IAssetType
    where TLinkType : IAssetLink<TLinkType, TAssetType>
{
    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    new string RawPath { get; set; }
    
    void SetToNull();
}

public interface IAssetLink : IAssetLinkGetter
{
    /// <summary>
    /// Set the path to a path that is relative to the game's Data directory
    /// i.e.: @"Skyrim Special Edition\Data\Meshes\Clutter\Spoon.nif" needs to be @"Meshes\Clutter\Spoon.nif"
    /// </summary>
    bool TrySetPath(string? path);

    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    new string RawPath { get; set; }
}