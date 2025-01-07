namespace Mutagen.Bethesda.Assets;

/// <summary>
/// File types that can be associated with Asset type
/// </summary>
public interface IAssetType
{
    static virtual IAssetType Instance => null!;

    /// <summary>
    /// Base folder(s) relative to the game's data directory
    /// </summary>
    string BaseFolder { get; }

    /// <summary>
    /// File extension this asset type is associated with
    /// </summary>
    IEnumerable<string> FileExtensions { get; }
}