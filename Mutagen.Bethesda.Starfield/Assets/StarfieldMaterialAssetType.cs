using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldMaterialAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new StarfieldMaterialAssetType();
    public string BaseFolder => "Materials";
    public IEnumerable<string> FileExtensions => new []{".mat"};
}