﻿using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class FindImplicitlyIncludedModsTests
{
    private static readonly ModKey ModA = "ModA.esp";
    private static readonly ModKey ModB = "ModB.esp";
    private static readonly ModKey ModC = "ModC.esp";
    private static readonly ModKey ModD = "ModD.esp";

    record Listing(ModKey Mod, params ModKey[] Masters);
        
    private void SetReaderFactory(
        MockFileSystem mockFileSystem,
        DirectoryPath directoryPath,
        FindImplicitlyIncludedMods includedMods,
        params Listing[] listings)
    {
        foreach (var listing in listings)
        {
            mockFileSystem.File.WriteAllText(Path.Combine(directoryPath.Path, listing.Mod.FileName), string.Empty);
            includedMods.ReaderFactory
                .FromPath(Path.Combine(directoryPath.Path, listing.Mod.FileName.String))
                .Returns(_ =>
                {
                    var reader = Substitute.For<IReadOnlyMasterReferenceCollection>();
                    reader.Masters.Returns(_ =>
                    {
                        return new List<IMasterReferenceGetter>(listing.Masters.Select(m =>
                        {
                            return new MasterReference()
                            {
                                Master = m
                            };
                        }));
                    });
                    return reader;
                });
        }
    }
        
    [Theory, MutagenAutoData]
    public void NothingToDo(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA, ModC),
            new Listing(ModC));
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModC, true),
            new LoadOrderListing(ModB, true),
        };
        var found = sut.Find(list)
            .ToList();
        found.ShouldBeEmpty();
    }
        
    [Theory, MutagenAutoData]
    public void EnableOne(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA, ModC),
            new Listing(ModC));
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModC, false),
            new LoadOrderListing(ModB, true),
        };
        var found = sut.Find(list)
            .ToList();
        found.ShouldHaveCount(1);
        found[0].ShouldBe(ModC);
    }
        
    [Theory, MutagenAutoData]
    public void SkipUnreferenced(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA));
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModC, false),
            new LoadOrderListing(ModB, true),
        };
        var found = sut.Find(list)
            .ToList();
        found.ShouldBeEmpty();
    }
        
    [Theory, MutagenAutoData]
    public void RecursiveEnable(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA, ModC),
            new Listing(ModC, ModD),
            new Listing(ModD));
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModD, false),
            new LoadOrderListing(ModC, false),
            new LoadOrderListing(ModB, true),
        };
        var found = sut.Find(list)
            .ToList();
        found.ShouldHaveCount(2);
        found.ShouldEqual(
            ModC,
            ModD);
    }
        
    [Theory, MutagenAutoData]
    public void RecursiveEnableBadLo(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA, ModC),
            new Listing(ModC, ModD),
            new Listing(ModD));
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModC, false),
            new LoadOrderListing(ModB, true),
            new LoadOrderListing(ModD, false),
        };
        var found = sut.Find(list)
            .ToArray();
        found.ShouldHaveCount(2);
        found.ShouldEqual(
            ModC,
            ModD);
    }

    [Theory, MutagenAutoData]
    public void UnlistedReference(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA, ModC),
            new Listing(ModC));
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModB, true),
        };
        var found = sut.Find(list)
            .ToList();
        found.ShouldBeEmpty();
    }

    [Theory, MutagenAutoData]
    public void MissingModThrows(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA),
            new Listing(ModB, ModA, ModC),
            new Listing(ModC, ModD),
            new Listing(ModD));
            
        mockFileSystem.File.Delete(Path.Combine(dataDirectoryProvider.Path, ModB.FileName));
            
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModC, false),
            new LoadOrderListing(ModB, true),
            new LoadOrderListing(ModD, false),
        };
            
        Assert.Throws<MissingModException>(() =>
        {
            sut.Find(list, skipMissingMods: false)
                .ToArray();
        });
    }

    [Theory, MutagenAutoData]
    public void MissingModSkipsIfAsked(
        MockFileSystem mockFileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        FindImplicitlyIncludedMods sut)
    {
        SetReaderFactory(
            mockFileSystem,
            dataDirectoryProvider.Path,
            sut,
            new Listing(ModA, ModD),
            new Listing(ModB, ModC),
            new Listing(ModC),
            new Listing(ModD));
            
        mockFileSystem.File.Delete(Path.Combine(dataDirectoryProvider.Path, ModB.FileName));
            
        var list = new List<ILoadOrderListingGetter>()
        {
            new LoadOrderListing(ModA, true),
            new LoadOrderListing(ModC, false),
            new LoadOrderListing(ModB, true),
            new LoadOrderListing(ModD, false),
        };
            
        var found = sut.Find(list, skipMissingMods: true)
            .ToArray();
        found.ShouldEqual(ModD);
    }
}