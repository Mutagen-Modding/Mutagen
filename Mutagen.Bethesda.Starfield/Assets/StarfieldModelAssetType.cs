using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldModelAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new StarfieldModelAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".nif"};
}