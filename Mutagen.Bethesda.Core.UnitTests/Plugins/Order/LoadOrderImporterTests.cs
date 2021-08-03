using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Mutagen.Bethesda.Core.UnitTests.AutoData;
using Mutagen.Bethesda.Core.UnitTests.Placeholders;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LoadOrderImporterTests
    {
        [Theory, MutagenAutoData]
        public void Typical(
            [Frozen]MockFileSystem fs,
            IEnumerable<ModPath> modPaths,
            [Frozen] IModImporter<TestMod> importer,
            LoadOrderImporter<TestMod> sut)
        {
            foreach (var mp in modPaths)
            {
                fs.File.WriteAllText(mp.Path, string.Empty);
            }

            foreach (var modPath in modPaths)
            {
                importer.Import(modPath)
                    .Returns(new TestMod(modPath.ModKey));
            }

            sut.LoadOrderListingsProvider.Get()
                .Returns(modPaths.Select(x => new ModListing(x.ModKey, true)));
            
            var lo = sut.Import();
            
            var expected = modPaths
                .Select(x => importer.Import(x))
                .ToArray();
            lo.Count.Should().Be(modPaths.Count());
            lo.Select(x => x.Value.Mod)
                .Should().Equal(expected);
        }

        [Fact]
        public void ImportThrows()
        {
            var fs = Substitute.For<IFileSystem>();
            fs.File.Exists(Arg.Any<string>()).Returns(true);
            var importer = Substitute.For<IModImporter<TestMod>>();
            importer.Import(Arg.Any<ModPath>()).Throws(new NotImplementedException());
            Action a = () =>
            {
                new LoadOrderImporter<TestMod>(
                        fs,
                        new DataDirectoryInjection("C:/DataFolder"),
                        new LoadOrderListingsInjection(TestConstants.Dawnguard),
                        importer)
                    .Import();
            };
            a.Should().Throw<AggregateException>()
                .WithInnerException<RecordException>();
        }

        [Fact]
        public void ExceptionDisposesExistingMods()
        {
            var dataFolder = "C:/DataFolder";
            var modPaths = new ModKey[]
                {
                    TestConstants.MasterModKey,
                    TestConstants.MasterModKey2,
                }
                .Select(x => ModPath.FromPath(Path.Combine(dataFolder, x.FileName)))
                .ToArray();
            var fs = Substitute.For<IFileSystem>();
            fs.File.Exists(Arg.Any<string>()).Returns(true);
            var importer = Substitute.For<IModImporter<IModDisposeGetter>>();
            var mod = Substitute.For<IModDisposeGetter>();
            importer.Import(modPaths.First()).Returns(mod);
            importer.Import(modPaths.Last()).Throws(new ArgumentException());
            Assert.Throws<AggregateException>(() =>
            {
                new LoadOrderImporter<IModDisposeGetter>(
                        fs,
                        new DataDirectoryInjection(dataFolder),
                        new LoadOrderListingsInjection(modPaths.Select(x => x.ModKey).ToArray()),
                        importer)
                    .Import();
            });
            mod.Received().Dispose();
        }

        [Fact]
        public void EntryDoesNotExist()
        {
            var dataFolder = "C:/DataFolder";
            var modPaths = new ModKey[]
                {
                    TestConstants.MasterModKey,
                    TestConstants.MasterModKey2,
                    TestConstants.MasterModKey3,
                }
                .Select(x => ModPath.FromPath(Path.Combine(dataFolder, x.FileName)))
                .ToArray();
            var fs = new MockFileSystem(
                modPaths
                    .Skip(1)
                    .ToDictionary(x => (string)x.Path, x => new MockFileData(string.Empty)));
            var importer = Substitute.For<IModImporter<TestMod>>();
            foreach (var modPath in modPaths)
            {
                importer.Import(modPath).Returns(new TestMod(modPath.ModKey));
            }
            var lo = new LoadOrderImporter<TestMod>(
                    fs,
                    new DataDirectoryInjection(dataFolder),
                    new LoadOrderListingsInjection(modPaths.Select(x => x.ModKey).ToArray()),
                    importer)
                .Import();
            lo.Count.Should().Be(3);
            lo.First().Value.Mod.Should().BeNull();
            lo.First().Value.ModKey.Should().Be(TestConstants.MasterModKey);
            lo.Select(x => x.Value.Mod)
                .Skip(1)
                .Should().Equal(
                    modPaths
                        .Skip(1)
                        .Select(x => importer.Import(x)));
        }
    }
}