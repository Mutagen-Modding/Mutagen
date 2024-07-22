using System.Runtime.InteropServices;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Plugins.Assets; 

public class AssetLinkPathingTests 
{
    static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    static string absPrefix = isWindows ? "C:\\" : "/";
    static readonly string DataPath = Path.Combine("Meshes" ,"Clutter", "MyMesh.nif");

    [Fact]
    public void AbsolutePath()
    {
        var path = @$"{absPrefix}{Path.Combine("Skyrim", "Data", "Meshes", "Clutter", "MyMesh.nif")}";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void DataRelativePath()
    {
        var path = Path.Combine("Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void PrefixedDataRelativePath()
    {
        var path = $"{Path.DirectorySeparatorChar}{Path.Combine("Data", "Meshes", "Clutter", "MyMesh.nif")}";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void FirstLevelChildRelativePath()
    {
        var path = $"{Path.DirectorySeparatorChar}{Path.Combine("Data",  "Clutter", "MyMesh.nif")}";
        Assert.Throws<AssetPathMisalignedException>(() =>
        {
            new AssetLink<TestAssetType>(path);
        });
    }

    [Fact]
    public void MissingBaseFolder()
    {
        var path = Path.Combine("Clutter", "MyMesh.nif");
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }

    [Fact]
    public void RelativePath()
    {
        var path = $".{Path.DirectorySeparatorChar}{Path.Combine("Data", "Meshes", "Clutter", "MyMesh.nif")}";
        var link = new AssetLink<TestAssetType>(path);
        link.DataRelativePath.Should().Be(DataPath);
        link.GivenPath.Should().Be(path);
    }
}
