using Mutagen.Bethesda.Assets;
namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimBodyTextureAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimBodyTextureAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{ ".egt" };
}
