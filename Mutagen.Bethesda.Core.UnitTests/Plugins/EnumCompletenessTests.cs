using AutoFixture.Xunit2;
using Shouldly;
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
            cat.GetRelatedReleases().ShouldContain(release);
        }
    }

    [Fact]
    public void Implicits()
    {
        foreach (var release in Enums<GameRelease>.Values)
        {
            Mutagen.Bethesda.Plugins.Implicits.Get(release).ShouldNotBeNull();
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
    public void CreationClubEnabledProvider(
        [Frozen] IGameCategoryContext gameCategoryContext,
        CreationClubEnabledProvider sut)
    {
        foreach (var category in Enums<GameCategory>.Values)
        {
            gameCategoryContext.Category.Returns(category);
            bool? b = sut.Used;
            b.ShouldNotBeNull();
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
                sut.Create(path).ShouldNotBeNull());
        }
    }

    [Fact]
    public void MutagenEncodingProvider()
    {
        foreach (var release in Enums<GameRelease>.Values
                     .Where(x => x != GameRelease.Oblivion)
                     .Where(x => x != GameRelease.Fallout3)
                     .Where(x => x != GameRelease.FalloutNV))
        {
            foreach (var lang in Enums<Language>.Values)
            {
                MutagenEncoding.GetEncoding(release, lang).ShouldNotBeNull();
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
            var constants = Mutagen.Bethesda.Plugins.Meta.GameConstants.Get(release);
            if (constants.PluginsFileInGameFolder) continue;
            try
            {
                prov.GetGameFolder(release).ShouldNotBeEmpty();
            }
            catch (ArgumentException)
            {
                // Acceptable
            }
        }
    }

    #endregion
}