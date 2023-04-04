using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class AssetLinkTests
{
    [Fact]
    public void TrySetPath()
    {
        var link = new AssetLink<TestAssetType>();
        link.TrySetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"))
            .Should().BeTrue();
        link.RawPath.Should().Be(Path.Combine("SomeSubFolder", "SomeModel.nif"));
        link.DataRelativePath.Should().Be(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
    }
    
    [Fact]
    public void TrySetPathToNull()
    {
        var link = new AssetLink<TestAssetType>();
        link.TrySetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"))
            .Should().BeTrue();
        link.TrySetPath(null);
        link.RawPath.Should().Be(string.Empty);
        link.DataRelativePath.Should().Be("Meshes");
    }
    
    [Fact]
    public void TrySetPathFailed()
    {
        var link = new AssetLink<TestAssetType>();
        link.TrySetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"))
            .Should().BeTrue();
        link.TrySetPath(Path.Combine("Other", "SomeSubFolder", "SomeModel.nif"))
            .Should().BeFalse();
        link.RawPath.Should().Be(Path.Combine("SomeSubFolder", "SomeModel.nif"));
        link.DataRelativePath.Should().Be(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
    }
    
    [Fact]
    public void SetPath()
    {
        var link = new AssetLink<TestAssetType>();
        link.SetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        link.RawPath.Should().Be(Path.Combine("SomeSubFolder", "SomeModel.nif"));
        link.DataRelativePath.Should().Be(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
    }
    
    [Fact]
    public void SetPathToNull()
    {
        var link = new AssetLink<TestAssetType>();
        link.SetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        link.TrySetPath(null);
        link.RawPath.Should().Be(string.Empty);
        link.DataRelativePath.Should().Be("Meshes");
    }
    
    [Fact]
    public void SetPathFailed()
    {
        var link = new AssetLink<TestAssetType>();
        link.SetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        Assert.Throws<AssetPathMisalignedException>(() =>
        {
            link.SetPath(Path.Combine("Other", "SomeSubFolder", "SomeModel.nif"));
        });
        link.RawPath.Should().Be(Path.Combine("SomeSubFolder", "SomeModel.nif"));
        link.DataRelativePath.Should().Be(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
    }
}