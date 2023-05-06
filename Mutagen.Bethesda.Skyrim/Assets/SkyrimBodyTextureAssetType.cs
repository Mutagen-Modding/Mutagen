namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimBodyTextureAssetType : SkyrimModelAssetType
{
    public static readonly SkyrimBodyTextureAssetType Instance = new();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{ ".egt" };
}
