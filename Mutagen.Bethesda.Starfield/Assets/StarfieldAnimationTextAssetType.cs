using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldAnimationTextAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new StarfieldAnimationTextAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".agx"};
}