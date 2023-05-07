using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class TestAssetType : IAssetType
{
#if NET7_0
    public static IAssetType Instance { get; } = new TestAssetType();
#else
    public static readonly TestAssetType Instance = new();
#endif
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".nif"};
}