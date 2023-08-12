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
#if NET7_0_OR_GREATER
        public static IAssetType Instance { get; } = new TestAssetType();
#else
    public static readonly TestAssetType Instance = new();
#endif
        public string BaseFolder => "TestFolder";
        public IEnumerable<string> FileExtensions => new []{ ".test" };
    }

    [Theory, BasicAutoData]
    public void Typical(
        ISpecimenContext context,
        AssetLinkBuilder sut)
    {
        var assetLink = (IAssetLinkGetter)sut.Create(typeof(AssetLink<TestAssetType>), context);
        assetLink.RawPath.Should().Contain(Path.Combine("Data", TestAssetType.Instance.BaseFolder));
        Path.GetExtension(assetLink.RawPath).Should().Be(TestAssetType.Instance.FileExtensions.First());
    }

    [Theory, MutagenAutoData]
    public void DifferentPaths(
        AssetLink<TestAssetType> link,
        AssetLink<TestAssetType> link2)
    {
        link.RawPath.Should().NotBe(link2.RawPath);
    }

    [Theory, MutagenAutoData]
    public void Existing(
        IFileSystem fileSystem,
        AssetLink<TestAssetType> existingLink)
    {
        fileSystem.File.Exists(existingLink.RawPath).Should().BeTrue();
    }
}