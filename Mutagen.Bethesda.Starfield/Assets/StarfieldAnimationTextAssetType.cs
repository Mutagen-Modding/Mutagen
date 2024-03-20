using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldAnimationTextAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new StarfieldAnimationTextAssetType();
#else
    public static readonly StarfieldAnimationTextAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".agx"};
}