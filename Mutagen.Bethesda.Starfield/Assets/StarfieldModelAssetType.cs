using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldModelAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new StarfieldModelAssetType();
#else
    public static readonly StarfieldModelAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".nif"};
}