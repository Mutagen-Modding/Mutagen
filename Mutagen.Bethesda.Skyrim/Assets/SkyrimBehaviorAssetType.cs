using Mutagen.Bethesda.Assets;
namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimBehaviorAssetType : SkyrimModelAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new SkyrimBehaviorAssetType();
#else
    public static readonly SkyrimBehaviorAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{ ".hkx" };
}