using System.IO.Abstractions;
using AutoFixture.Xunit2;
using Shouldly;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
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
            return sut.Read(missingPath).ToArray();
        });
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
            .ShouldEqualEnumerable(listings);
    }
}