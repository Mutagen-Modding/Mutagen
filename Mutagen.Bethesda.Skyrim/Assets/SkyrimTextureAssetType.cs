using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimTextureAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimTextureAssetType();
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}