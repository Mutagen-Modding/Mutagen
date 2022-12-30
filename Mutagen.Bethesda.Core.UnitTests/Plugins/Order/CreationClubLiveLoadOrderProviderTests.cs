using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubLiveLoadOrderProviderTests
{
    [Theory, MutagenAutoData]
    public void Neither(TestScheduler scheduler, CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return Observable.Empty<IChangeSet<ILoadOrderListingGetter>>();
        });
        sut.FolderWatcher.Get().Returns(
            Observable.Empty<IChangeSet<ModKey, ModKey>>());
        var obs = scheduler.Start(() =>
        {
            var ret = sut.Get(out var state);
            state.Subscribe();
            stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
            return ret;
        });
        obs.Messages.Count.Should().Be(0);
    }

    [Theory, MutagenAutoData]
    public void OnlyFile(TestScheduler scheduler, CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return Observable.Return(new ChangeSet<ILoadOrderListingGetter>(
                new Change<ILoadOrderListingGetter>[]
                {
                    new Change<ILoadOrderListingGetter>(
                        ListChangeReason.Add,
                        new LoadOrderListing("ModA.esp", true))
                }));
        });
        sut.FolderWatcher.Get().Returns(
            Observable.Empty<IChangeSet<ModKey, ModKey>>());
        var obs = scheduler.Start(() =>
        {
            var ret = sut.Get(out var state);
            state.Subscribe();
            stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
            return ret;
        });
        obs.Messages.Count.Should().Be(0);
    }

    [Theory, MutagenAutoData]
    public void OnlyFolder(TestScheduler scheduler, CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return Observable.Empty<IChangeSet<ILoadOrderListingGetter>>();
        });
        sut.FolderWatcher.Get().Returns(
            Observable.Return(new ChangeSet<ModKey, ModKey>(
                new Change<ModKey, ModKey>[]
                {
                    new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                })));
        var obs = scheduler.Start(() =>
        {
            var ret = sut.Get(out var state);
            state.Subscribe();
            stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
            return ret;
        });
        obs.Messages.Count.Should().Be(0);
    }

    [Theory, MutagenAutoData]
    public void Both(CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return Observable.Return(new ChangeSet<ILoadOrderListingGetter>(
                new Change<ILoadOrderListingGetter>[]
                {
                    new Change<ILoadOrderListingGetter>(
                        ListChangeReason.Add,
                        new LoadOrderListing("ModA.esp", true))
                }));
        });
        sut.FolderWatcher.Get().Returns(
            Observable.Return(new ChangeSet<ModKey, ModKey>(
                new Change<ModKey, ModKey>[]
                {
                    new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                })));
        var list = sut
            .Get(out var state)
            .AsObservableList();
        state.Subscribe();
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(
            new LoadOrderListing("ModA.esp", true));
        stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
    }

    [Theory, MutagenAutoData]
    public void FileAdded(CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        var fileSubj = new Subject<IChangeSet<ILoadOrderListingGetter>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return fileSubj;
        });
            
        sut.FolderWatcher.Get().Returns(
            Observable.Return(new ChangeSet<ModKey, ModKey>(
                new Change<ModKey, ModKey>[]
                {
                    new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                })));
            
        var list = sut.Get(out var state)
            .AsObservableList();
        state.Subscribe();
        list.Count.Should().Be(0);
        fileSubj.OnNext(new ChangeSet<ILoadOrderListingGetter>(
            new Change<ILoadOrderListingGetter>[]
            {
                new Change<ILoadOrderListingGetter>(
                    ListChangeReason.Add,
                    new LoadOrderListing("ModA.esp", true))
            }));
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(
            new LoadOrderListing("ModA.esp", true));
        stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
    }

    [Theory, MutagenAutoData]
    public void FolderAdded(CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return
                Observable.Return(new ChangeSet<ILoadOrderListingGetter>(
                    new Change<ILoadOrderListingGetter>[]
                    {
                        new Change<ILoadOrderListingGetter>(
                            ListChangeReason.Add,
                            new LoadOrderListing("ModA.esp", true))
                    }));
        });
            
        var folderSubj = new Subject<IChangeSet<ModKey, ModKey>>();
        sut.FolderWatcher.Get().Returns(folderSubj);
            
        var list = sut.Get(out var state)
            .AsObservableList();
        state.Subscribe();
        list.Count.Should().Be(0);
        folderSubj.OnNext(new ChangeSet<ModKey, ModKey>(
            new Change<ModKey, ModKey>[]
            {
                new Change<ModKey, ModKey>(
                    ChangeReason.Add,
                    ModKey.FromNameAndExtension("ModA.esp"),
                    ModKey.FromNameAndExtension("ModA.esp"))
            }));
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(
            new LoadOrderListing("ModA.esp", true));
        stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
    }

    [Theory, MutagenAutoData]
    public void FileRemoved(CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        var fileSubj = new Subject<IChangeSet<ILoadOrderListingGetter>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return fileSubj;
        });

        sut.FolderWatcher.Get().Returns(
            Observable.Return(new ChangeSet<ModKey, ModKey>(
                new Change<ModKey, ModKey>[]
                {
                    new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                })));
        var list = sut.Get(out var state)
            .AsObservableList();
        state.Subscribe();
        fileSubj.OnNext(new ChangeSet<ILoadOrderListingGetter>(
            new Change<ILoadOrderListingGetter>[]
            {
                new Change<ILoadOrderListingGetter>(
                    ListChangeReason.Add,
                    new LoadOrderListing("ModA.esp", true))
            }));
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(
            new LoadOrderListing("ModA.esp", true));
        fileSubj.OnNext(new ChangeSet<ILoadOrderListingGetter>(
            new Change<ILoadOrderListingGetter>[]
            {
                new Change<ILoadOrderListingGetter>(
                    ListChangeReason.Remove,
                    new LoadOrderListing("ModA.esp", true),
                    0)
            }));
        list.Count.Should().Be(0);
        stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
    }

    [Theory, MutagenAutoData]
    public void FolderRemoved(CreationClubLiveLoadOrderProvider sut)
    {
        var stateObs = Substitute.For<IObservable<ErrorResponse>>();
        sut.FileReader.Get(out _).Returns(x =>
        {
            x[0] = stateObs;
            return
                Observable.Return(new ChangeSet<ILoadOrderListingGetter>(
                    new Change<ILoadOrderListingGetter>[]
                    {
                        new Change<ILoadOrderListingGetter>(
                            ListChangeReason.Add,
                            new LoadOrderListing("ModA.esp", true))
                    }));
        });
        var folderSubj = new Subject<IChangeSet<ModKey, ModKey>>();
        sut.FolderWatcher.Get().Returns(folderSubj);
        var list = sut.Get(out var state)
            .AsObservableList();
        state.Subscribe();
        folderSubj.OnNext(new ChangeSet<ModKey, ModKey>(
            new Change<ModKey, ModKey>[]
            {
                new Change<ModKey, ModKey>(
                    ChangeReason.Add,
                    ModKey.FromNameAndExtension("ModA.esp"),
                    ModKey.FromNameAndExtension("ModA.esp"))
            }));
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(
            new LoadOrderListing("ModA.esp", true));
        folderSubj.OnNext(new ChangeSet<ModKey, ModKey>(
            new Change<ModKey, ModKey>[]
            {
                new Change<ModKey, ModKey>(
                    ChangeReason.Remove,
                    ModKey.FromNameAndExtension("ModA.esp"),
                    ModKey.FromNameAndExtension("ModA.esp"),
                    0)
            }));
        list.Count.Should().Be(0);
        stateObs.ReceivedWithAnyArgs(1).Subscribe(default!);
    }
}