using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class FindImplicitlyIncludedModsTests
    {
        private static readonly ModKey ModA = "ModA.esp";
        private static readonly ModKey ModB = "ModB.esp";
        private static readonly ModKey ModC = "ModC.esp";
        private static readonly ModKey ModD = "ModD.esp";

        record Listing(ModKey Mod, params ModKey[] Masters);
        
        private void SetReaderFactory(
            FindImplicitlyIncludedMods includedMods,
            params Listing[] listings)
        {
            foreach (var listing in listings)
            {
                includedMods.ReaderFactory
                    .FromPath(Path.Combine(includedMods.DirectoryProvider.Path, listing.Mod.FileName.String))
                    .Returns(_ =>
                    {
                        var reader = Substitute.For<IMasterReferenceReader>();
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
        public void NothingToDo(FindImplicitlyIncludedMods sut)
        {
            SetReaderFactory(sut,
                new Listing(ModA),
                new Listing(ModB, ModA, ModC));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, true),
                new ModListing(ModB, true),
            };
            var found = sut.Find(list)
                .ToList();
            found.Should().BeEmpty();
        }
        
        [Theory, MutagenAutoData]
        public void EnableOne(FindImplicitlyIncludedMods sut)
        {
            SetReaderFactory(sut,
                new Listing(ModA),
                new Listing(ModB, ModA, ModC));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
            };
            var found = sut.Find(list)
                .ToList();
            found.Should().HaveCount(1);
            found[0].Should().Be(ModC);
        }
        
        [Theory, MutagenAutoData]
        public void SkipUnreferenced(FindImplicitlyIncludedMods sut)
        {
            SetReaderFactory(sut,
                new Listing(ModA),
                new Listing(ModB, ModA));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
            };
            var found = sut.Find(list)
                .ToList();
            found.Should().BeEmpty();
        }
        
        [Theory, MutagenAutoData]
        public void RecursiveEnable(FindImplicitlyIncludedMods sut)
        {
            SetReaderFactory(sut,
                new Listing(ModA),
                new Listing(ModB, ModA, ModC),
                new Listing(ModC, ModD));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModD, false),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
            };
            var found = sut.Find(list)
                .ToList();
            found.Should().HaveCount(2);
            found.Should().Equal(
                ModC,
                ModD);
        }
        
        [Theory, MutagenAutoData]
        public void RecursiveEnableBadLo(FindImplicitlyIncludedMods sut)
        {
            SetReaderFactory(sut,
                new Listing(ModA),
                new Listing(ModB, ModA, ModC),
                new Listing(ModC, ModD));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
                new ModListing(ModD, false),
            };
            var found = sut.Find(list)
                .ToArray();
            found.Should().HaveCount(2);
            found.Should().Equal(
                ModC,
                ModD);
        }

        [Theory, MutagenAutoData]
        public void UnlistedReference(FindImplicitlyIncludedMods sut)
        {
            SetReaderFactory(sut,
                new Listing(ModA),
                new Listing(ModB, ModA, ModC));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModB, true),
            };
            var found = sut.Find(list)
                .ToList();
            found.Should().BeEmpty();
        }
    }
}