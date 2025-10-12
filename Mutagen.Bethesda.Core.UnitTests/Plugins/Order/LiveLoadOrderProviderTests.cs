using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutoFixture.Xunit2;
using DynamicData;
using Shouldly;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class LiveLoadOrderProviderTests
{
    [Theory, MutagenAutoData]
    public void NoSub(LiveLoadOrderProvider sut)
    {
        sut.ListingsProvider.DidNotReceive().Get();
        var c = sut.PluginLive.DidNotReceive().Changed;
        sut.PluginLive.DidNotReceive().Get(out _);
        c = sut.CccLive.DidNotReceive().Changed;
        sut.CccLive.DidNotReceive().Get(out _);
    }

    [Theory, MutagenAutoData(ConfigureMembers: false)]
    public void GetsListingsInitially(
        IScheduler scheduler,
        LiveLoadOrderProvider sut)
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.MasterModKey, true),
            new LoadOrderListing(TestConstants.MasterModKey2, false),
        };
        sut.ListingsProvider.Get().Returns(listings);
        sut.PluginLive.Changed.Returns(Observable.Empty<Unit>());
        sut.CccLive.Changed.Returns(Observable.Empty<Unit>());
        var list = sut.Get(out var state, scheduler)
            .AsObservableList();
        list.Items.ShouldBe(listings);
        sut.ListingsProvider.Received(1).Get();
        var obsScheduler = new TestScheduler();
        var err = obsScheduler.Start(() => state);
        err.ShouldHaveNoErrors();
        err.ShouldNotBeCompleted();
        err.Messages.Select(x => x.Value.Value.Succeeded).ShouldAllBe(x => x == true);
    }

    [Theory, MutagenAutoData(ConfigureMembers: false)]
    public void Throttles(
        TestScheduler scheduler,
        LiveLoadOrderProvider sut)
    {
        sut.Timings.Throttle.Returns(TimeSpan.FromTicks(5));
        var pluginSubj = new Subject<Unit>();
        sut.PluginLive.Changed.Returns(pluginSubj);
        sut.Get(out _, scheduler)
            .AsObservableList();
        scheduler.AdvanceBy(1);
        pluginSubj.OnNext(Unit.Default);
        scheduler.AdvanceBy(1);
        pluginSubj.OnNext(Unit.Default);
        scheduler.AdvanceBy(sut.Timings.Throttle.Ticks);
        var c = sut.PluginLive.Received(1).Changed;
    }

    [Theory, MutagenAutoData(ConfigureMembers: false)]
    public void EitherChangedRequeries(
        IScheduler scheduler,
        LiveLoadOrderProvider sut)
    {
        var pluginSubj = new Subject<Unit>();
        sut.PluginLive.Changed.Returns(pluginSubj);
        var cccSubj = new Subject<Unit>();
        sut.CccLive.Changed.Returns(cccSubj);
        sut.Get(out _, scheduler)
            .AsObservableList();
        pluginSubj.OnNext(Unit.Default);
        cccSubj.OnNext(Unit.Default);
        sut.ListingsProvider.Received(3).Get();
    }

    [Theory, MutagenAutoData]
    public void ErrorHandlingDuringSpam(
        [Frozen] TestScheduler scheduler,
        LiveLoadOrderProvider sut)
    {
        var pluginSubj = new Subject<Unit>();
        sut.PluginLive.Changed.Returns(pluginSubj);
        sut.CccLive.Changed.Returns(Observable.Empty<Unit>());
        sut.ListingsProvider.Get().Returns(
            _ => [],
            _ => throw new NotImplementedException(),
            _ => []);
            
        sut.Get(out var state, scheduler)
            .AsObservableList();
        ErrorResponse err = ErrorResponse.Failure;
        using var sub = state.Subscribe(x => err = x);
        err.Succeeded.ShouldBeTrue();

        pluginSubj.OnNext(Unit.Default);
        err.Succeeded.ShouldBeFalse();
            
        pluginSubj.OnNext(Unit.Default);
        err.Succeeded.ShouldBeTrue();
    }

    [Theory, MutagenAutoData]
    public void RetryOnStaleInput(
        TestScheduler scheduler,
        LiveLoadOrderProvider sut)
    {
        sut.Timings.Throttle.Returns(TimeSpan.Zero);
        sut.Timings.RetryInterval.Returns(TimeSpan.FromMilliseconds(250));
        sut.Timings.RetryIntervalMax.Returns(TimeSpan.FromSeconds(5));
        var pluginSubj = new Subject<Unit>();
        sut.PluginLive.Changed.Returns(pluginSubj);
        var cccSubj = new Subject<Unit>();
        sut.CccLive.Changed.Returns(cccSubj);
        sut.ListingsProvider.Get().Returns(
            _ => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
            _ => []);
            
        sut.Get(out var state, scheduler)
            .AsObservableList();
        ErrorResponse err = ErrorResponse.Failure;
        using var sub = state.Subscribe(x => err = x);
        err.Succeeded.ShouldBeFalse();
        sut.ListingsProvider.Received(1).Get();

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(250).Ticks);
        err.Succeeded.ShouldBeFalse();
        sut.ListingsProvider.Received(2).Get();
            
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(500).Ticks);
        err.Succeeded.ShouldBeTrue();
        sut.ListingsProvider.Received(3).Get();
    }
}