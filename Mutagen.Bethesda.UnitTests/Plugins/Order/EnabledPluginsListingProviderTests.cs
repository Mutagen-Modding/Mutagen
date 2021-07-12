using System.Collections.Generic;
using AutoFixture.Xunit2;
using FakeItEasy;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class EnabledPluginsListingProviderTests : TypicalTest
    {
        [Theory, MutagenAutoData(false)]
        public void Typical(
            [Frozen]IPluginListingsPathProvider pathProvider,
            [Frozen]IPluginRawListingsReader reader,
            IEnumerable<IModListingGetter> listings,
            EnabledPluginListingsProvider sut)
        {
            A.CallTo(() => reader.Read(pathProvider.Path)).Returns(listings);
            sut.Get()
                .Should().Equal(listings);
        }
    }
}