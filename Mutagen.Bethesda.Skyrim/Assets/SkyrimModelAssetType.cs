using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimModelAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimModelAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".nif"};
}