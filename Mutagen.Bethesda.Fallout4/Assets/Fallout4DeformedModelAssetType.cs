using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4DeformedModelAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4DeformedModelAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => [".tri"];
}
