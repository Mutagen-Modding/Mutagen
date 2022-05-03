using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

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
    public void PassesFileSystemStreamToParser(
        [Frozen] IFileSystem fs,
        FilePath path,
        Stream someStream,
        PluginRawListingsReader sut)
    {
        fs.File.Exists(path).Returns(true);
        fs.FileStream
            .Create(path.Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            .Returns(someStream);
        sut.Read(path);
        sut.Parser.Received(1).Parse(someStream);
    }

    [Theory, MutagenAutoData]
    public void ParserResultsGetReturned(
        FilePath existingPath,
        PluginRawListingsReader sut)
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing("ModA.esp", true),
            new LoadOrderListing("ModB.esp", false),
        };
        sut.Parser.Parse(default!).ReturnsForAnyArgs(listings);
        sut.Read(existingPath)
            .Should().BeEquivalentTo(listings);
    }
}