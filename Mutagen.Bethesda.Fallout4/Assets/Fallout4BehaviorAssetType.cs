using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4BehaviorAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4BehaviorAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => [".hkx"];
}
