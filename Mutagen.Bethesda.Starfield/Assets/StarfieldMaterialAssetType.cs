using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Starfield.Assets;

public class StarfieldMaterialAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new StarfieldMaterialAssetType();
#else
    public static readonly StarfieldModelAssetType Instance = new();
#endif
    public string BaseFolder => "Materials";
    public IEnumerable<string> FileExtensions => new []{".mat"};
}