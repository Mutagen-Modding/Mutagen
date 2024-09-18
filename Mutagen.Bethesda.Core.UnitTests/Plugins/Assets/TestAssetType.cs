using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class TestAssetType : IAssetType
{
    public static IAssetType Instance { get; } = new TestAssetType();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{".nif"};
}