using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldTextureAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new StarfieldTextureAssetType();
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}