using AutoFixture.Xunit2;
using FluentAssertions;
using Loqui;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public class EnumCompletenessTests
{
    #region GameRelease
    [Fact]
    public void ToCategoryCoverage()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            release.ToCategory();
        }
    }

    [Fact]
    public void GameConstants()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            Mutagen.Bethesda.Plugins.Meta.GameConstants.Get(release);
        }
    }

    [Fact]
    public void RelatedReleases()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            var cat = release.ToCategory();
            cat.GetRelatedReleases().Should().Contain(release);
        }
    }

    [Fact]
    public void Implicits()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            Mutagen.Bethesda.Plugins.Implicits.Get(release).Should().NotBeNull();
        }
    }

    [Theory]
    [MutagenAutoData]
    public void PluginListingsProvider(
        [Frozen] IGameReleaseContext gameReleaseContext,
        PluginListingsProvider sut)
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            gameReleaseContext.Release.Returns(release);
            sut.Get().ToArray();
        }
    }

    [Theory]
    [MutagenAutoData]
    public void ArchiveReaderProvider(
        [Frozen] IGameReleaseContext gameReleaseContext,
        FilePath path,
        ArchiveReaderProvider sut)
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            gameReleaseContext.Release.Returns(release);
            Assert.Throws<FileNotFoundException>(() =>
                sut.Create(path).Should().NotBeNull());
        }
    }

    [Fact]
    public void MutagenEncodingProvider()
    {
        foreach (var release in Enums<GameRelease>.Values
                     .Where(x => x != GameRelease.Oblivion))
        {
            foreach (var lang in Enums<Language>.Values)
            {
                MutagenEncoding.GetEncoding(release, lang).Should().NotBeNull();
            }
        }
    }
    #endregion

    #region GameCategory
    [Fact]
    public void HasFormVersion()
    {
        foreach (var cat in Enums<GameCategory>.Values)
        {
            cat.HasFormVersion();
        }
    }
    
    public void IncludesMasterReferenceDataSubrecords()
    {
        foreach (var cat in Enums<GameCategory>.Values)
        {
            cat.IncludesMasterReferenceDataSubrecords();
        }
    }
    #endregion

    #region PluginListingsPathProvider

    [Theory, DefaultAutoData]
    public void PluginListingsPathProviderTest(PluginListingsPathProvider prov)
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            try
            {
                prov.GetGameFolder(release).Should().NotBeEmpty();
            }
            catch (ArgumentException)
            {
                // Acceptable
            }
        }
    }

    #endregion
}