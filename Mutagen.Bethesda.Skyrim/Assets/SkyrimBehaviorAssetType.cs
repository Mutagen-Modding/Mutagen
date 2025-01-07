using Mutagen.Bethesda.Assets;
namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimBehaviorAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new SkyrimBehaviorAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{ ".hkx" };
}