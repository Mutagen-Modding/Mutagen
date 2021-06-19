using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Core.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class FindImplicitlyIncludedModsTests : IClassFixture<Fixture>
    {
        private readonly Fixture _Fixture;

        public FindImplicitlyIncludedModsTests(Fixture fixture)
        {
            _Fixture = fixture;
        }

        private static readonly ModKey ModA = "ModA.esp";
        private static readonly  ModKey ModB = "ModB.esp";
        private static readonly  ModKey ModC = "ModC.esp";
        private static readonly  ModKey ModD = "ModD.esp";

        record Listing(ModKey Mod, params ModKey[] Masters);
        
        private IMasterReferenceReaderFactory GetReaderFactory(params Listing[] listings)
        {
            var masterReaderFactory = Substitute.For<IMasterReferenceReaderFactory>();
            foreach (var listing in listings)
            {
                masterReaderFactory.FromPath(listing.Mod.FileName.String, Arg.Any<GameRelease>())
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
            return masterReaderFactory;
        }
        
        [Fact]
        public void NothingToDo()
        {
            var adder = new FindImplicitlyIncludedMods(GetReaderFactory(
                new Listing(ModA),
                new Listing(ModB, ModA, ModC)));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, true),
                new ModListing(ModB, true),
            };
            var found = adder.Find(
                _Fixture.Inject.Create<GameRelease>(),
                _Fixture.Inject.Create<DirectoryPath>(),
                list)
                .ToList();
            found.Should().BeEmpty();
        }
        
        [Fact]
        public void EnableOne()
        {
            var adder = new FindImplicitlyIncludedMods(GetReaderFactory(
                new Listing(ModA),
                new Listing(ModB, ModA, ModC)));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
            };
            var found = adder.Find(
                    _Fixture.Inject.Create<GameRelease>(),
                    _Fixture.Inject.Create<DirectoryPath>(),
                    list)
                .ToList();
            found.Should().HaveCount(1);
            found[0].Should().Be(ModC);
        }
        
        [Fact]
        public void SkipUnreferenced()
        {
            var adder = new FindImplicitlyIncludedMods(GetReaderFactory(
                new Listing(ModA),
                new Listing(ModB, ModA)));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
            };
            var found = adder.Find(
                    _Fixture.Inject.Create<GameRelease>(),
                    _Fixture.Inject.Create<DirectoryPath>(),
                    list)
                .ToList();
            found.Should().BeEmpty();
        }
        
        [Fact]
        public void RecursiveEnable()
        {
            var adder = new FindImplicitlyIncludedMods(GetReaderFactory(
                new Listing(ModA),
                new Listing(ModB, ModA, ModC),
                new Listing(ModC, ModD)));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModD, false),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
            };
            var found = adder.Find(
                _Fixture.Inject.Create<GameRelease>(),
                _Fixture.Inject.Create<DirectoryPath>(),
                list)
                .ToList();
            found.Should().HaveCount(2);
            found.Should().BeEquivalentTo(
                ModC,
                ModD);
        }
        
        [Fact]
        public void RecursiveEnableBadLo()
        {
            var adder = new FindImplicitlyIncludedMods(GetReaderFactory(
                new Listing(ModA),
                new Listing(ModB, ModA, ModC),
                new Listing(ModC, ModD)));
            var list = new List<IModListingGetter>()
            {
                new ModListing(ModA, true),
                new ModListing(ModC, false),
                new ModListing(ModB, true),
                new ModListing(ModD, false),
            };
            var found = adder.Find(
                _Fixture.Inject.Create<GameRelease>(),
                _Fixture.Inject.Create<DirectoryPath>(),
                list)
                .ToArray();
            found.Should().HaveCount(2);
            found.Should().BeEquivalentTo(
                ModC,
                ModD);
        }
    }
}