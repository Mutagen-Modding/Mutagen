using System.IO.Abstractions;
using AutoFixture.Kernel;
using FluentAssertions;
using Mutagen.Bethesda.Assets;
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
        assetLink.GivenPath.Should().Contain(Path.Combine("Data", TestAssetType.Instance.BaseFolder));
        Path.GetExtension(assetLink.GivenPath).Should().Be(TestAssetType.Instance.FileExtensions.First());
    }

    [Theory, MutagenAutoData]
    public void DifferentPaths(
        AssetLink<TestAssetType> link,
        AssetLink<TestAssetType> link2)
    {
        link.GivenPath.Should().NotBe(link2.GivenPath);
    }

    [Theory, MutagenAutoData]
    public void Existing(
        IFileSystem fileSystem,
        AssetLink<TestAssetType> existingLink)
    {
        fileSystem.File.Exists(existingLink.GivenPath).Should().BeTrue();
    }
}