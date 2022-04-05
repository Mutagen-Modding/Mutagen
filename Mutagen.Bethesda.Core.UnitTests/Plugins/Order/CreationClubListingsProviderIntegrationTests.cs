using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubListingsProviderIntegrationTests
{
    private const string DataDir = "C:/DataDirectory";
        
    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void CccNotUsed(
        CreationClubListingsProvider sut)
    {
        sut.Get().Should().BeEmpty();
    }

    [Theory, MutagenAutoData]
    public void CccMissing(
        FilePath missingFile,
        CreationClubListingsProvider sut)
    {
        sut.ListingsPathProvider.Path.Returns(missingFile);
        Assert.Throws<FileNotFoundException>(() =>
        {
            sut.Get();
        });
    }

    [Fact]
    public void GetListings()
    {
        var cccPath = Path.Combine(DataDir, "cccPlugin.txt");
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { cccPath, @"ModA.esp
ModB.esp"},
            { Path.Combine(DataDir, "ModA.esp"), string.Empty },
            { Path.Combine(DataDir, "ModB.esp"), string.Empty },
        });

        var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
        new CreationClubListingsProvider(
                fs,
                dataDirectoryInjection,
                new CreationClubListingsPathInjection(cccPath),
                new CreationClubRawListingsReader())
            .Get().Should().Equal(
                new ModListing("ModA.esp", true),
                new ModListing("ModB.esp", true));
    }

    [Fact]
    public void SkipMissingListings()
    {
        var cccPath = Path.Combine(DataDir, "cccPlugin.txt");
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { cccPath, @"ModA.esp
ModB.esp"},
            { Path.Combine(DataDir, "ModA.esp"), string.Empty }
        });

        var dataDirectoryInjection = new DataDirectoryInjection(DataDir);
        new CreationClubListingsProvider(
                fs,
                dataDirectoryInjection,
                new CreationClubListingsPathInjection(cccPath),
                new CreationClubRawListingsReader())
            .Get().Should().Equal(
                new ModListing("ModA.esp", true));   
    }
}