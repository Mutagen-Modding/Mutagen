using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using DynamicData;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LiveLoadOrderProviderTests
    {
        [Fact]
        public void NoSub()
        {
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            var loadOrderListingsProvider = Substitute.For<ILoadOrderListingsProvider>();
            new LiveLoadOrderProvider(
                pluginLive,
                cccLive,
                loadOrderListingsProvider);
            var changed = pluginLive.DidNotReceive().Changed;
            pluginLive.DidNotReceive().Get(out _);
            changed = cccLive.DidNotReceive().Changed;
            cccLive.DidNotReceive().Get(out _);
            loadOrderListingsProvider.DidNotReceive().Get();
        }

        [Fact]
        public void GetsListingsInitially()
        {
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            var listings = new ModListing[]
            {
                new ModListing(Utility.MasterModKey, true),
                new ModListing(Utility.MasterModKey2, false),
            };
            var loadOrderListingsProvider = Substitute.For<ILoadOrderListingsProvider>();
            var scheduler = new TestScheduler();
            loadOrderListingsProvider.Get().Returns(listings);
            var list = new LiveLoadOrderProvider(
                    pluginLive,
                    cccLive,
                    loadOrderListingsProvider)
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().Equal(listings);
            loadOrderListingsProvider.Received(1).Get();
            var err = scheduler.Start(() => state);
            err.ShouldHaveNoErrors();
            err.ShouldNotBeCompleted();
            err.Messages.Select(x => x.Value.Value.Succeeded).Should().AllBeEquivalentTo(true);
        }

        [Fact]
        public void Throttles()
        {
            var pluginSubj = new Subject<Unit>();
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            pluginLive.Changed.Returns(pluginSubj);
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            var loadOrderListingsProvider = Substitute.For<ILoadOrderListingsProvider>();
            var scheduler = new TestScheduler();
            var list = new LiveLoadOrderProvider(
                    pluginLive,
                    cccLive,
                    loadOrderListingsProvider)
                .Get(out var state, scheduler)
                .AsObservableList();
            scheduler.AdvanceBy(1);
            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(1);
            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            loadOrderListingsProvider.Received(1).Get();
        }

        [Fact]
        public void EitherChangedRequeries()
        {
            var pluginSubj = new Subject<Unit>();
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            pluginLive.Changed.Returns(pluginSubj);
            var cccSubj = new Subject<Unit>();
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            cccLive.Changed.Returns(cccSubj);
            var loadOrderListingsProvider = Substitute.For<ILoadOrderListingsProvider>();
            var scheduler = new TestScheduler();
            var list = new LiveLoadOrderProvider(
                    pluginLive,
                    cccLive,
                    loadOrderListingsProvider)
                .Get(out var state, scheduler)
                .AsObservableList();
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            cccSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            loadOrderListingsProvider.Received(3).Get();
        }

        [Fact]
        public void ErrorHandlingDuringSpam()
        {
            var pluginSubj = new Subject<Unit>();
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            pluginLive.Changed.Returns(pluginSubj);
            var loadOrderListingsProvider = A.Fake<ILoadOrderListingsProvider>();
            A.CallTo(() => loadOrderListingsProvider.Get())
                .Returns(Enumerable.Empty<IModListingGetter>()).Once()
                .Then.Throws<NotImplementedException>().Once()
                .Then.Returns(Enumerable.Empty<IModListingGetter>()).Once();
            
            var scheduler = new TestScheduler();
            var list = new LiveLoadOrderProvider(
                    pluginLive,
                    cccLive,
                    loadOrderListingsProvider)
                .Get(out var state, scheduler)
                .AsObservableList();
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            err.Succeeded.Should().BeTrue();

            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            err.Succeeded.Should().BeFalse();
            
            pluginSubj.OnNext(Unit.Default);
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
            err.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void RetryOnStaleInput()
        {
            var pluginSubj = new Subject<Unit>();
            var pluginLive = Substitute.For<IPluginLiveLoadOrderProvider>();
            pluginLive.Changed.Returns(pluginSubj);
            var cccSubj = new Subject<Unit>();
            var cccLive = Substitute.For<ICreationClubLiveLoadOrderProvider>();
            cccLive.Changed.Returns(cccSubj);
            var loadOrderListingsProvider = A.Fake<ILoadOrderListingsProvider>();
            A.CallTo(() => loadOrderListingsProvider.Get())
                .Throws<NotImplementedException>().Twice()
                .Then.Returns(Enumerable.Empty<IModListingGetter>()).Once();
            
            var scheduler = new TestScheduler();
            var list = new LiveLoadOrderProvider(
                    pluginLive,
                    cccLive,
                    loadOrderListingsProvider)
                .Get(out var state, scheduler)
                .AsObservableList();
            scheduler.AdvanceBy(LiveLoadOrderProvider.ThrottleSpan.Ticks);
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