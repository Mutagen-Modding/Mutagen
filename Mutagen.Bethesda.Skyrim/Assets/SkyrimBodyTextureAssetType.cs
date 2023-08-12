using Mutagen.Bethesda.Assets;
namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimBodyTextureAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimBodyTextureAssetType();
#else
    public static readonly SkyrimBodyTextureAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{ ".egt" };
}
