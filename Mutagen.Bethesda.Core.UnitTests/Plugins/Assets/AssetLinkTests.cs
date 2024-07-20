using FluentAssertions;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Assets;

public class AssetLinkTests
{
    [Fact]
    public void TrySetPath()
    {
        var link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.TrySetPath(path)
            .Should().BeTrue();
        link.GivenPath.Should().Be(path);
        link.DataRelativePath.Should().Be(path);
    }
    
    [Fact]
    public void TrySetPathToNull()
    {
        var link = new AssetLink<TestAssetType>();
        link.TrySetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"))
            .Should().BeTrue();
        link.TrySetPath(null);
        link.GivenPath.Should().Be(string.Empty);
        link.DataRelativePath.Should().Be(string.Empty);
    }
    
    [Fact]
    public void TrySetPathFailed()
    {
        var link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.TrySetPath(path)
            .Should().BeTrue();
        link.TrySetPath(Path.Combine("Other", "SomeSubFolder", "SomeModel.nif"))
            .Should().BeFalse();
        link.GivenPath.Should().Be(path);
        link.DataRelativePath.Should().Be(path);
    }
    
    [Fact]
    public void SetPathDataRelative()
    {
        var link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.SetPath(path);
        link.GivenPath.Should().Be(path);
        link.DataRelativePath.Should().Be(path);
    }
    
    [Fact]
    public void SetPathAbsolute()
    {
        var link = new AssetLink<TestAssetType>();
        var path = Path.Combine("SomeFolder", "Data", "Meshes", "SomeSubFolder", "SomeModel.nif");
        link.SetPath(path);
        link.GivenPath.Should().Be(path);
        link.DataRelativePath.Should().Be(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
    }
    
    [Fact]
    public void SetPathToNull()
    {
        var link = new AssetLink<TestAssetType>();
        link.SetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        link.TrySetPath(null);
        link.GivenPath.Should().Be(string.Empty);
        link.DataRelativePath.Should().Be(string.Empty);
    }
    
    [Fact]
    public void SetPathFailed()
    {
        var link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.SetPath(path);
        Assert.Throws<AssetPathMisalignedException>(() =>
        {
            link.SetPath(Path.Combine("Other", "SomeSubFolder", "SomeModel.nif"));
        });
        link.GivenPath.Should().Be(path);
        link.DataRelativePath.Should().Be(path);
    }
    
    [Fact]
    public void TestEqualsDifferentRawPath()
    {
        var link = new AssetLinkGetter<TestAssetType>(Path.Combine("SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestEqualsAbsolutePath()
    {
        var filePath = new FilePath(Path.Combine("SomeFolder", "Data", "Meshes", "SomeSubFolder", "SomeModel.nif"));
        var link = new AssetLinkGetter<TestAssetType>(filePath.Path);
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsGetterGetter()
    {
        var link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsGetterSetter()
    {
        var link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsSetterGetter()
    {
        var link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsSetterSetter()
    {
        var link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestEqualsGetterGetter()
    {
        var link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestEqualsGetterSetter()
    {
        var link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestEqualsSetterGetter()
    {
        var link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).Should().BeTrue();
    }
    
    [Fact]
    public void TestEqualsSetterSetter()
    {
        var link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).Should().BeTrue();
    }
}