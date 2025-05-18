using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout3.Assets;

public class Fallout3TextureAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout3TextureAssetType();
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}