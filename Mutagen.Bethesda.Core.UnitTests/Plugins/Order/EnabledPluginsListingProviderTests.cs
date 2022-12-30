using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class EnabledPluginsListingProviderTests
{
    [Theory, MutagenAutoData]
    public void Typical(
        [Frozen]IPluginListingsPathContext pathContext,
        [Frozen]IPluginRawListingsReader reader,
        IEnumerable<IModListingGetter> listings,
        EnabledPluginListingsProvider sut)
    {
        reader.Read(pathContext.Path).Returns(listings);
        sut.Get()
            .Should().Equal(listings);
    }
}