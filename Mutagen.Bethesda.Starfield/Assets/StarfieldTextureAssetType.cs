using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldTextureAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new StarfieldTextureAssetType();
#else
    public static readonly StarfieldTextureAssetType Instance = new();
#endif
    public string BaseFolder => "Textures";
    public IEnumerable<string> FileExtensions => new []{ ".dds", ".png" };
}