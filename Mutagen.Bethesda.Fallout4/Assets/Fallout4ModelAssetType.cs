using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Fallout4.Assets;

public class Fallout4ModelAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new Fallout4ModelAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => [".nif", ".bto", ".btr"];
}
