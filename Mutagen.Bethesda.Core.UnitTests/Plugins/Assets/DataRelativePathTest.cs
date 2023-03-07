using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Plugins.Assets; 

public class DataRelativePathTest {
    const string DataPath = "Meshes\\Clutter\\MyMesh.nif";

    [Fact]
    public void AbsolutePath()
    {
        var link = new AssetLink<TestAssetType>("C:\\Skyrim\\Data\\Meshes\\Clutter\\MyMesh.nif");
        link.DataRelativePath.Should().Be(DataPath);
    }

    [Fact]
    public void DataRelativePath()
    {
        var link = new AssetLink<TestAssetType>("\\Data\\Meshes\\Clutter\\MyMesh.nif");
        link.DataRelativePath.Should().Be(DataPath);
    }

    [Fact]
    public void FirstLevelChildRelativePath()
    {
        var link = new AssetLink<TestAssetType>("Clutter\\MyMesh.nif");
        link.DataRelativePath.Should().Be(DataPath);
    }
}
