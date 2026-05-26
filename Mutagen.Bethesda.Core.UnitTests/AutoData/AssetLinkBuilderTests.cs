using System.IO.Abstractions;
using AutoFixture.Kernel;
using Shouldly;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class AssetLinkBuilderTests
{
    public class TestAssetType : IAssetType
    {
        public static IAssetType Instance { get; } = new TestAssetType();
        public string BaseFolder => "TestFolder";
        public IEnumerable<string> FileExtensions => new []{ ".test" };
    }

    [Theory, BasicAutoData]
    public void Typical(
        ISpecimenContext context,
        AssetLinkBuilder sut)
    {
        var assetLink = (IAssetLinkGetter)sut.Create(typeof(AssetLink<TestAssetType>), context);
        assetLink.DataRelativePath.Path.ShouldContain(TestAssetType.Instance.BaseFolder);
        Path.GetExtension(assetLink.GivenPath).ShouldBe(TestAssetType.Instance.FileExtensions.First());
    }

    [Theory, MutagenAutoData]
    public void DifferentPaths(
        AssetLink<TestAssetType> link,
        AssetLink<TestAssetType> link2)
    {
        link.GivenPath.ShouldNotBe(link2.GivenPath);
    }

    [Theory, MutagenAutoData]
    public void Existing(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        AssetLink<TestAssetType> existingLink)
    {
        var fullPath = Path.Combine(dataDirectoryProvider.Path, existingLink.DataRelativePath.Path);
        fileSystem.File.Exists(fullPath).ShouldBeTrue();
    }
}