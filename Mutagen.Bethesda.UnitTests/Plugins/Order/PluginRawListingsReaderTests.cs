using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class PluginRawListingsReaderTests
    {
        [Theory, MutagenAutoData]
        public void PathDoesNotExist(
            FilePath missingPath,
            PluginRawListingsReader sut)
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                sut.Read(missingPath).ToArray();
            });
        }

        [Theory, MutagenAutoData(UseMockFileSystem: false)]
        public void Typical(
            [Frozen] IFileSystem fs,
            FilePath path,
            Stream someStream)
        {
            var listings = new ModListing[]
            {
                new ModListing("ModA.esp", true),
                new ModListing("ModB.esp", false),
            };
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