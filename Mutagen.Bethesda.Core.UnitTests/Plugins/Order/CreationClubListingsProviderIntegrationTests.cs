﻿using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Noggog.Testing.IO;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubListingsProviderIntegrationTests
{
    private static readonly string DataDir = $"{PathingUtil.DrivePrefix}DataDirectory";
        
    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void CccNotUsed(
        CreationClubListingsProvider sut)
    {
        sut.Get().ShouldBeEmpty();
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
            .Get().ShouldEqual(
                new ModListing("ModA.esp", true, true),
                new ModListing("ModB.esp", true, true));
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
            .Get().ShouldEqual(
                new ModListing("ModA.esp", true, true));   
    }
}