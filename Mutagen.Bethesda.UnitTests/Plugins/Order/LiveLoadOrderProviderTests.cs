using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using AutoFixture.Xunit2;
using DynamicData;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LiveLoadOrderProviderTests
    {
        [Theory, MutagenAutoData]
        public void NoSub(
            [Frozen]ILoadOrderListingsProvider loadOrderListingsProvider, 
            [Frozen]IPluginLiveLoadOrderProvider pluginLive,
            [Frozen]ICreationClubLiveLoadOrderProvider cccLive,
            LiveLoadOrderProvider sut)
        {
        }

        [Theory, MutagenAutoData(false)]
        public void GetsListingsInitially(
            [Frozen]ILoadOrderListingsProvider loadOrderListingsProvider, 
            IScheduler scheduler,
            LiveLoadOrderProvider sut)
        {
            var listings = new ModListing[]
            {
                new ModListing(Utility.MasterModKey, true),
                new ModListing(Utility.MasterModKey2, false),
            };
            A.CallTo(() => loadOrderListingsProvider.Get()).Returns(listings);
            var list = sut.Get(out var state, scheduler)
                .AsObservableList();
            list.Items.Should().Equal(listings);
            A.CallTo(() => loadOrderListingsProvider.Get()).MustHaveHappenedOnceExactly();
            var obsScheduler = new TestScheduler();
            var err = obsScheduler.Start(() => state);
            err.ShouldHaveNoErrors();
            err.ShouldNotBeCompleted();
            err.Messages.Select(x => x.Value.Value.Succeeded).Should().AllBeEquivalentTo(true);
        }

        [Theory, MutagenAutoData(false)]
        public void Throttles(
            [Frozen]TestScheduler scheduler,
            [Frozen]IPluginLiveLoadOrderProvider pluginLive,
            [Frozen]ILiveLoadOrderTimings throttle,
            LiveLoadOrderProvider sut)
        {
            A.CallTo(() => throttle.Throttle).Returns(TimeSpan.FromTicks(5));
            var pluginSubj = new Subject<Unit>();
            A.CallTo(() => pluginLive.Changed).Returns(pluginSubj);
            sut.Get(out _, scheduler)
                .AsObservableList();
            scheduler.AdvanceBy(1);
            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(1);
            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(throttle.Throttle.Ticks);
            A.CallTo(() => pluginLive.Changed).MustHaveHappenedOnceExactly();
        }

        [Theory, MutagenAutoData(false)]
        public void EitherChangedRequeries(
            [Frozen]IScheduler scheduler,
            [Frozen]ILoadOrderListingsProvider loadOrderListingsProvider, 
            [Frozen]IPluginLiveLoadOrderProvider pluginLive,
            [Frozen]ICreationClubLiveLoadOrderProvider cccLive,
            LiveLoadOrderProvider sut)
        {
            var pluginSubj = new Subject<Unit>();
            A.CallTo(() => pluginLive.Changed).Returns(pluginSubj);
            var cccSubj = new Subject<Unit>();
            A.CallTo(() => cccLive.Changed).Returns(cccSubj);
            sut.Get(out _, scheduler)
                .AsObservableList();
            pluginSubj.OnNext(Unit.Default);
            cccSubj.OnNext(Unit.Default);
            A.CallTo(() => loadOrderListingsProvider.Get())
                .MustHaveHappened(3, Times.Exactly);
        }

        [Theory, MutagenAutoData(false)]
        public void ErrorHandlingDuringSpam(
            [Frozen]TestScheduler scheduler,
            [Frozen]ILoadOrderListingsProvider loadOrderListingsProvider, 
            [Frozen]IPluginLiveLoadOrderProvider pluginLive,
            LiveLoadOrderProvider sut)
        {
            var pluginSubj = new Subject<Unit>();
            A.CallTo(() => pluginLive.Changed).Returns(pluginSubj);
            A.CallTo(() => loadOrderListingsProvider.Get())
                .Returns(Enumerable.Empty<IModListingGetter>()).Once()
                .Then.Throws<NotImplementedException>().Once()
                .Then.Returns(Enumerable.Empty<IModListingGetter>()).Once();
            
            sut.Get(out var state, scheduler)
                .AsObservableList();
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            err.Succeeded.Should().BeTrue();

            pluginSubj.OnNext(Unit.Default);
            err.Succeeded.Should().BeFalse();
            
            pluginSubj.OnNext(Unit.Default);
            err.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void RetryOnStaleInput(
            [Frozen]TestScheduler scheduler,
            [Frozen]ILiveLoadOrderTimings throttle,
            [Frozen]ILoadOrderListingsProvider loadOrderListingsProvider, 
            [Frozen]IPluginLiveLoadOrderProvider pluginLive,
            [Frozen]ICreationClubLiveLoadOrderProvider cccLive,
            LiveLoadOrderProvider sut)
        {
            A.CallTo(() => throttle.Throttle).Returns(TimeSpan.Zero);
            A.CallTo(() => throttle.RetryInterval).Returns(TimeSpan.FromMilliseconds(250));
            A.CallTo(() => throttle.RetryIntervalMax).Returns(TimeSpan.FromSeconds(5));
            var pluginSubj = new Subject<Unit>();
            A.CallTo(() => pluginLive.Changed).Returns(pluginSubj);
            var cccSubj = new Subject<Unit>();
            A.CallTo(() => cccLive.Changed).Returns(cccSubj);
            A.CallTo(() => loadOrderListingsProvider.Get())
                .Throws<NotImplementedException>().Twice()
                .Then.Returns(Enumerable.Empty<IModListingGetter>()).Once();
            
            sut.Get(out var state, scheduler)
                .AsObservableList();
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            err.Succeeded.Should().BeFalse();
            A.CallTo(() => loadOrderListingsProvider.Get()).MustHaveHappened(1, Times.Exactly);

            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(250).Ticks);
            err.Succeeded.Should().BeFalse();
            A.CallTo(() => loadOrderListingsProvider.Get()).MustHaveHappened(2, Times.Exactly);
            
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(500).Ticks);
            err.Succeeded.Should().BeTrue();
            A.CallTo(() => loadOrderListingsProvider.Get()).MustHaveHappened(3, Times.Exactly);
        }
    }
}