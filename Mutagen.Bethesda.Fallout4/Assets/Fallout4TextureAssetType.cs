using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4TextureAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4TextureAssetType();
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => [".dds", ".png"];
}
