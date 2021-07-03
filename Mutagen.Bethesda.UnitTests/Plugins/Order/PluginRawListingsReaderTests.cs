using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class PluginRawListingsReaderTests : TypicalTest
    {
        [Fact]
        public void PathDoesNotExist()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                new PluginRawListingsReader(
                        new MockFileSystem(),
                        Fixture.Create<IPluginListingsParser>())
                    .Read("C:/NonExistantPath")
                    .ToArray();
            });
        }

        [Fact]
        public void Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing("ModA.esp", true),
                new ModListing("ModB.esp", false),
            };
            var path = new FilePath("C:/Plugin.txt");
            var someStream = Substitute.For<Stream>();
            var fs = Substitute.For<IFileSystem>();
            fs.File.Exists(path).Returns(true);
            fs.FileStream
                .Create(path.Path, FileMode.Open, FileAccess.Read, FileShare.Read)
                .Returns(someStream);
            var parser = Substitute.For<IPluginListingsParser>();
            parser.Parse(someStream).Returns(listings);
            new PluginRawListingsReader(
                    fs,
                    parser)
                .Read(path)
                .Should().BeEquivalentTo(listings);
        }
    }
}