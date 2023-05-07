using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimTextureAssetType : IAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new SkyrimTextureAssetType();
#else
    public static readonly SkyrimTextureAssetType Instance = new();
#endif
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}