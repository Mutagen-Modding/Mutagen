using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LoadOrderListingsProviderTests : TypicalTest
    {
        [Fact]
        public void Typical()
        {
            var implicits = Fixture.CreateMany<IModListingGetter>();
            var implicitProv = Substitute.For<IImplicitListingsProvider>();
            implicitProv.Get().Returns(implicits);
            var ccc = Fixture.CreateMany<IModListingGetter>();
            var cccProv = Substitute.For<ICreationClubListingsProvider>();
            cccProv.Get(throwIfMissing: false).Returns(ccc);
            var plugins = Fixture.CreateMany<IModListingGetter>();
            var pluginsProv = Substitute.For<IPluginListingsProvider>();
            pluginsProv.Get().Returns(plugins);
            var orderRet = Fixture.CreateMany<IModListingGetter>();
            var orderListings = Substitute.For<IOrderListings>();
            orderListings.Order(
                    Arg.Any<IEnumerable<IModListingGetter>>(),
                    Arg.Any<IEnumerable<IModListingGetter>>(),
                    Arg.Any<IEnumerable<IModListingGetter>>(),
                    Arg.Any<Func<IModListingGetter, ModKey>>())
                .Returns(orderRet);
            new LoadOrderListingsProvider(
                    orderListings,
                    implicitProv,
                    pluginsProv,
                    cccProv)
                .Get().Should().Equal(orderRet);
            cccProv.Received().Get(false);
            orderListings.Received()
                .Order(
                    Arg.Is<IEnumerable<IModListingGetter>>(x => x.SequenceEqual(implicits)),
                    Arg.Is<IEnumerable<IModListingGetter>>(x => x.SequenceEqual(plugins)),
                    Arg.Is<IEnumerable<IModListingGetter>>(x => x.SequenceEqual(ccc)),
                    Arg.Any<Func<IModListingGetter, ModKey>>());
        }
        
        [Fact]
        public void BlockImplictsFromPlugins()
        {
            var ccc = Fixture.CreateMany<IModListingGetter>();
            var cccProv = Substitute.For<ICreationClubListingsProvider>();
            cccProv.Get(throwIfMissing: false).Returns(ccc);
            var plugins = Fixture.CreateMany<IModListingGetter>();
            var pluginsProv = Substitute.For<IPluginListingsProvider>();
            pluginsProv.Get().Returns(plugins);
            var implicits = plugins.Take(1);
            var implicitProv = Substitute.For<IImplicitListingsProvider>();
            implicitProv.Get().Returns(implicits);
            var orderRet = Fixture.CreateMany<IModListingGetter>();
            var orderListings = Substitute.For<IOrderListings>();
            orderListings.Order(
                    Arg.Any<IEnumerable<IModListingGetter>>(),
                    Arg.Any<IEnumerable<IModListingGetter>>(),
                    Arg.Any<IEnumerable<IModListingGetter>>(),
                    Arg.Any<Func<IModListingGetter, ModKey>>())
                .Returns(orderRet);
            new LoadOrderListingsProvider(
                    orderListings,
                    implicitProv,
                    pluginsProv,
                    cccProv)
                .Get().Should().Equal(orderRet);
            cccProv.Received().Get(false);
            orderListings.Received()
                .Order(
                    Arg.Is<IEnumerable<IModListingGetter>>(x => x.SequenceEqual(implicits)),
                    Arg.Is<IEnumerable<IModListingGetter>>(x => x.SequenceEqual(plugins.Skip(1))),
                    Arg.Is<IEnumerable<IModListingGetter>>(x => x.SequenceEqual(ccc)),
                    Arg.Any<Func<IModListingGetter, ModKey>>());
        }
    }
}