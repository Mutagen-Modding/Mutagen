using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Plugins.Assets; 

public class AssetLinkPathingTests 
{
    static readonly string DataPath = Path.Combine("Meshes" ,"Clutter", "MyMesh.nif");

    [Fact]
    public void AbsolutePath()
    {
        var path = "C:\\Skyrim\\Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void DataRelativePath()
    {
        var path = "Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void PrefixedDataRelativePath()
    {
        var path = "\\Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void FirstLevelChildRelativePath()
    {
        var path = Path.Combine("Clutter", "MyMesh.nif");
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }
}
