using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Plugins.Assets;

public interface IAssetLinkGetter
{
    IAssetType AssetTypeInstance { get; }

    /// <summary>
    /// Original given path to the Asset Link
    /// </summary>
    string GivenPath { get; }

    /// <summary>
    /// Raw path relative to the game's data directory, as it is commonly known in the Bethesda modding community.
    /// <example>
    /// <code>
    /// Example:
    ///     C:\Skyrim\Data\Meshes\Clutter\MyMesh.nif => Meshes\Clutter\MyMesh.nif
    ///     \Data\Meshes\Clutter\MyMesh.nif          => Meshes\Clutter\MyMesh.nif
    ///     Data\Meshes\Clutter\MyMesh.nif           => Meshes\Clutter\MyMesh.nif
    ///     Clutter\MyMesh.nif                       => Meshes\Clutter\MyMesh.nif
    /// </code>
    /// </example>
    /// </summary>
    DataRelativePath DataRelativePath { get; }

    /// <summary>
    /// Extension of the asset
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// Type of the asset
    /// </summary>
    IAssetType Type { get; }

    /// <summary>
    /// True if Asset Link points to a null path
    /// </summary>
    public bool IsNull { get; }
}

public interface IAssetLinkGetter<out TAssetType> : IAssetLinkGetter
    where TAssetType : IAssetType
{
}

public interface IAssetLink<out TAssetType> : IAssetLink<IAssetLink<TAssetType>, TAssetType>
    where TAssetType : IAssetType
{
    new TAssetType AssetTypeInstance { get; }

    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    new string GivenPath { get; set; }
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
    new string GivenPath { get; set; }
    
    void SetToNull();
}

public interface IAssetLink : IAssetLinkGetter
{
    /// <summary>
    /// Attempts to set the path of the AssetLink.  <br />
    /// Will back out and return false if path does not align with expected folder structure.
    /// </summary>
    /// <returns>True if path matched expected patterns.  False if it did not align with expected folder structure</returns>
    bool TrySetPath(DataRelativePath? path);
    
    /// <summary>
    /// Attempts to set the path of the AssetLink.  <br />
    /// Will back out and return false if path does not align with expected folder structure.
    /// </summary>
    /// <returns>True if path matched expected patterns.  False if it did not align with expected folder structure</returns>
    bool TrySetPath(string? path);

    /// <summary>
    /// Raw path pointing to the asset
    /// </summary>
    new string GivenPath { get; set; }
}