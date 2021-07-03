using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class EnabledPluginsListingProviderTests : TypicalTest
    {
        [Fact]
        public void Typical()
        {
            var path = Fixture.Create<IPluginPathContext>();
            var reader = Substitute.For<IPluginRawListingsReader>();
            var listings = Fixture.CreateMany<IModListingGetter>();
            reader.Read(path.Path).Returns(listings);
            new EnabledPluginListingsProvider(
                    reader,
                    path)
                .Get()
                .Should().Equal(listings);
        }
    }
}