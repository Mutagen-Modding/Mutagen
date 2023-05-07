namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimBehaviorAssetType : SkyrimModelAssetType
{
    public static readonly SkyrimBehaviorAssetType Instance = new();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{ ".hkx" };
}