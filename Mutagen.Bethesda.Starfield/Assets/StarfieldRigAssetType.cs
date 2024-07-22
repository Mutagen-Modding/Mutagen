using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldRigAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new StarfieldRigAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".rig"};
}