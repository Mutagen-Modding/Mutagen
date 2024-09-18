using System.Runtime.InteropServices;
using FluentAssertions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class DataRelativePathTests 
{
    static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    static string absPrefix = isWindows ? "C:\\" : "/";
    static readonly string DataPath = Path.Combine("Meshes" ,"Clutter", "MyMesh.nif");

    [Fact]
    public void AbsolutePath()
    {
        var path = @$"{absPrefix}{Path.Combine("Skyrim", "Data", "Meshes", "Clutter", "MyMesh.nif")}";
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void DataRelativePath()
    {
        var path = Path.Combine("Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void DataRelativePathWithPrefix()
    {
        var path = Path.Combine("SomeFolder", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void PrefixedDataRelativePath()
    {
        var path = $"{Path.DirectorySeparatorChar}{Path.Combine("Data", "Meshes", "Clutter", "MyMesh.nif")}";
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void FirstLevelChildRelativePath()
    {
        var path = Path.Combine("Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(path);
    }

    [Fact]
    public void NoDataFolderInAbsolutePath()
    {
        Assert.Throws<AssetPathMisalignedException>(() =>
        {
            new DataRelativePath($"{absPrefix}{Path.Combine("Skyrim", "NoDataPath", "MyMesh.nif")}");
        });
    }

    [Fact]
    public void BaseFolderContainsData()
    {
        var path = Path.Combine("SomeDataFolder", "OtherFolder", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void BaseFolderContainsData2()
    {
        var path = Path.Combine("SomeData", "OtherFolder", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void BaseFolderContainsData3()
    {
        var path = Path.Combine("DataFolder", "OtherFolder", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void BaseFolderContainsData4()
    {
        var path = Path.Combine("OtherFolder", "SomeDataFolder", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void BaseFolderContainsData5()
    {
        var path = Path.Combine("OtherFolder", "SomeData", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void BaseFolderContainsData6()
    {
        var path = Path.Combine("OtherFolder", "DataFolder", "Data", "Meshes", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(DataPath);
    }

    [Fact]
    public void AnimTextData()
    {
        var path = Path.Combine("AnimTextData", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(path);
    }

    [Fact]
    public void AnimTextDataWithData()
    {
        var path = Path.Combine("Data", "AnimTextData", "Clutter", "MyMesh.nif");
        var link = new DataRelativePath(path);
        link.Path.Should().Be(Path.Combine("AnimTextData", "Clutter", "MyMesh.nif"));
    }
}
