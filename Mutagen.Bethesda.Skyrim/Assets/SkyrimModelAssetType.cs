using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimModelAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimModelAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => [
        ".nif", // Standard Skyrim model format
        ".bto", // Skyrim LOD objects format - same file format as .nif
        ".btr"  // Skyrim LOD terrain format - same file format as .nif
    ];
}
