using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimTextureAssetType : IAssetType
{
    public static readonly SkyrimTextureAssetType Instance = new();
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}