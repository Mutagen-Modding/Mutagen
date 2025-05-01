using Shouldly;
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
        IAssetLink link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.TrySetPath(path)
            .ShouldBeTrue();
        link.GivenPath.ShouldBe(path);
        link.DataRelativePath.ShouldBe(path);
    }
    
    [Fact]
    public void TrySetPathToNull()
    {
        IAssetLink link = new AssetLink<TestAssetType>();
        link.TrySetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"))
            .ShouldBeTrue();
        link.TrySetPath(null);
        link.GivenPath.ShouldBe(string.Empty);
        link.DataRelativePath.ShouldBe(string.Empty);
    }
    
    [Fact]
    public void TrySetPathFailed()
    {
        IAssetLink link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.TrySetPath(path)
            .ShouldBeTrue();
        link.TrySetPath(Path.Combine("Other", "SomeSubFolder", "SomeModel.nif"))
            .ShouldBeFalse();
        link.GivenPath.ShouldBe(path);
        link.DataRelativePath.ShouldBe(path);
    }
    
    [Fact]
    public void SetPathDataRelative()
    {
        IAssetLink link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.SetPath(path);
        link.GivenPath.ShouldBe(path);
        link.DataRelativePath.ShouldBe(path);
    }
    
    [Fact]
    public void SetPathAbsolute()
    {
        IAssetLink link = new AssetLink<TestAssetType>();
        var path = Path.Combine("SomeFolder", "Data", "Meshes", "SomeSubFolder", "SomeModel.nif");
        link.SetPath(path);
        link.GivenPath.ShouldBe(path);
        link.DataRelativePath.ShouldBe(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
    }
    
    [Fact]
    public void SetPathToNull()
    {
        IAssetLink link = new AssetLink<TestAssetType>();
        link.SetPath(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        link.TrySetPath(null);
        link.GivenPath.ShouldBe(string.Empty);
        link.DataRelativePath.ShouldBe(string.Empty);
    }
    
    [Fact]
    public void SetPathFailed()
    {
        IAssetLink link = new AssetLink<TestAssetType>();
        var path = Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif");
        link.SetPath(path);
        Assert.Throws<AssetPathMisalignedException>(() =>
        {
            link.SetPath(Path.Combine("Other", "SomeSubFolder", "SomeModel.nif"));
        });
        link.GivenPath.ShouldBe(path);
        link.DataRelativePath.ShouldBe(path);
    }
    
    [Fact]
    public void TestEqualsDifferentRawPath()
    {
        IAssetLinkGetter link = new AssetLinkGetter<TestAssetType>(Path.Combine("SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestEqualsAbsolutePath()
    {
        var filePath = new FilePath(Path.Combine("SomeFolder", "Data", "Meshes", "SomeSubFolder", "SomeModel.nif"));
        IAssetLinkGetter link = new AssetLinkGetter<TestAssetType>(filePath.Path);
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsGetterGetter()
    {
        IAssetLinkGetter link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsGetterSetter()
    {
        IAssetLinkGetter link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsSetterGetter()
    {
        IAssetLinkGetter link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestObjectEqualsSetterSetter()
    {
        IAssetLinkGetter link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        Equals(link, otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestEqualsGetterGetter()
    {
        IAssetLinkGetter link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestEqualsGetterSetter()
    {
        IAssetLinkGetter link = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestEqualsSetterGetter()
    {
        IAssetLinkGetter link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLinkGetter<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).ShouldBeTrue();
    }
    
    [Fact]
    public void TestEqualsSetterSetter()
    {
        IAssetLinkGetter link = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        var otherLink = new AssetLink<TestAssetType>(Path.Combine("Meshes", "SomeSubFolder", "SomeModel.nif"));
        
        link.Equals(otherLink).ShouldBeTrue();
    }
}