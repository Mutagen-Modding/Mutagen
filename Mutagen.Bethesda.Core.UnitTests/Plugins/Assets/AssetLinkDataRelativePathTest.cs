using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Plugins.Assets; 

public class AssetLinkDataRelativePathTest 
{
    static readonly string DataPath = Path.Combine("Meshes" ,"Clutter", "MyMesh.nif");
    static readonly string RawPath = Path.Combine("Clutter", "MyMesh.nif");

    [Fact]
    public void AbsolutePath()
    {
        var path = "C:\\Skyrim\\Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.RawPath.Should().Be(RawPath);
    }

    [Fact]
    public void DataRelativePath()
    {
        var path = "Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.RawPath.Should().Be(RawPath);
    }

    [Fact]
    public void PrefixedDataRelativePath()
    {
        var path = "\\Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.RawPath.Should().Be(RawPath);
    }

    [Fact]
    public void FirstLevelChildRelativePath()
    {
        var link = new AssetLink<TestAssetType>(RawPath);
        link.DataRelativePath.Should().Be(RawPath);
        link.RawPath.Should().Be(RawPath);
    }
}
