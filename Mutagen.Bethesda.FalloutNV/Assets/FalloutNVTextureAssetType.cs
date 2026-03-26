using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.FalloutNV.Assets;

public class FalloutNVTextureAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new FalloutNVTextureAssetType();
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}