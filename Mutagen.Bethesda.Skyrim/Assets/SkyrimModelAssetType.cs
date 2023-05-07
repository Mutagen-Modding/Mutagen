using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class SkyrimModelAssetType : IAssetType
{
#if NET7_0_OR_GREATER
    public static IAssetType Instance { get; } = new SkyrimModelAssetType();
#else
    public static readonly SkyrimModelAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".nif"};
}