using FluentAssertions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class DataRelativeAssetPathTests 
{
    static readonly string DataPath = Path.Combine("Meshes" ,"Clutter", "MyMesh.nif");

    [Fact]
    public void AbsolutePath()
    {
        var path = "C:\\Skyrim\\Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new DataRelativeAssetPath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void DataRelativePath()
    {
        var path = "Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new DataRelativeAssetPath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void PrefixedDataRelativePath()
    {
        var path = "\\Data\\Meshes\\Clutter\\MyMesh.nif";
        var link = new DataRelativeAssetPath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void FirstLevelChildRelativePath()
    {
        var path = "Clutter\\MyMesh.nif";
        var link = new DataRelativeAssetPath(path);
        link.Path.Should().Be(path);
    }

    [Fact]
    public void NoDataFolderInAbsolutePath()
    {
        Assert.Throws<AssetPathMisalignedException>(() =>
        {
            new DataRelativeAssetPath(
                "C:\\Skyrim\\NoDataPath\\MyMesh.nif");
        });
    }
}
