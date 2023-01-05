using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class TestAssetType : IAssetType
{
    public static readonly TestAssetType Instance = new();
    public string BaseFolder => "Meshes";
    public IEnumerable<string> FileExtensions => new []{"nif"};
}