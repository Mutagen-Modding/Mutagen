using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutoFixture.Xunit2;
using DynamicData;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubLiveLoadOrderProviderTests : TypicalTest
    {
        [Theory, MutagenAutoData]
        public void Neither(
            [Frozen]TestScheduler scheduler,
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            A.CallTo(() => fileReader.Get(out mockState)).Returns(
                Observable.Empty<IChangeSet<IModListingGetter>>());
            A.CallTo(() => folderWatcher.Get())
                .Returns(Observable.Empty<IChangeSet<ModKey, ModKey>>());
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = sut.Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(0);
            stateTest.Messages.Count.Should().Be(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Theory, MutagenAutoData]
        public void OnlyFile(
            [Frozen]TestScheduler scheduler,
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            A.CallTo(() => fileReader.Get(out mockState)).Returns(
                Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    })));
            A.CallTo(() => folderWatcher.Get())
                .Returns(Observable.Empty<IChangeSet<ModKey, ModKey>>());
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = sut.Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(0);
            stateTest.Messages.Count.Should().Be(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Theory, MutagenAutoData]
        public void OnlyFolder(
            [Frozen]TestScheduler scheduler,
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            A.CallTo(() => fileReader.Get(out mockState)).Returns(
                Observable.Empty<IChangeSet<IModListingGetter>>());
            A.CallTo(() => folderWatcher.Get())
                .Returns(Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    })));
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = sut.Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(0);
            stateTest.Messages.Count.Should().Be(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Theory, MutagenAutoData]
        public void Both(
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            A.CallTo(() => fileReader.Get(out mockState)).Returns(
                Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    })));
            A.CallTo(() => folderWatcher.Get())
                .Returns(Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    })));
            var list = sut
                .Get(out var state)
                .AsObservableList();
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void FileAdded(
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            var fileSubj = new Subject<IChangeSet<IModListingGetter>>();
            
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            A.CallTo(() => fileReader.Get(out mockState)).Returns(fileSubj);
            
            A.CallTo(() => folderWatcher.Get())
                .Returns(Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    })));
            
            var list = sut.Get(out var state)
                .AsObservableList();
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            list.Count.Should().Be(0);
            fileSubj.OnNext(new ChangeSet<IModListingGetter>(
                new Change<IModListingGetter>[]
                {
                    new Change<IModListingGetter>(
                        ListChangeReason.Add,
                        new ModListing("ModA.esp", true))
                }));
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void FolderAdded(
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            A.CallTo(() => fileReader.Get(out mockState)).Returns(
                Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    })));
            
            var folderSubj = new Subject<IChangeSet<ModKey, ModKey>>();
            A.CallTo(() => folderWatcher.Get()).Returns(folderSubj);
            var list = sut.Get(out var state)
                .AsObservableList();
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
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
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void FileRemoved(
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            var fileSubj = new Subject<IChangeSet<IModListingGetter>>();
            A.CallTo(() => fileReader.Get(out mockState)).Returns(fileSubj);

            A.CallTo(() => folderWatcher.Get()).Returns(
                Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    })));
            var list = sut.Get(out var state)
                .AsObservableList();
            fileSubj.OnNext(new ChangeSet<IModListingGetter>(
                new Change<IModListingGetter>[]
                {
                    new Change<IModListingGetter>(
                        ListChangeReason.Add,
                        new ModListing("ModA.esp", true))
                }));
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
            fileSubj.OnNext(new ChangeSet<IModListingGetter>(
                new Change<IModListingGetter>[]
                {
                    new Change<IModListingGetter>(
                        ListChangeReason.Remove,
                        new ModListing("ModA.esp", true),
                        0)
                }));
            list.Count.Should().Be(0);
            err.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void FolderRemoved(
            [Frozen]ICreationClubLiveListingsFileReader fileReader,
            [Frozen]ICreationClubLiveLoadOrderFolderWatcher folderWatcher,
            CreationClubLiveLoadOrderProvider sut)
        {
            IObservable<ErrorResponse> mockState = Observable.Return(ErrorResponse.Success);
            
            A.CallTo(() => fileReader.Get(out mockState)).Returns(
                Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    })));
            var folderSubj = new Subject<IChangeSet<ModKey, ModKey>>();
            A.CallTo(() => folderWatcher.Get()).Returns(folderSubj);
            var list = sut.Get(out var state)
                .AsObservableList();
            folderSubj.OnNext(new ChangeSet<ModKey, ModKey>(
                new Change<ModKey, ModKey>[]
                {
                    new Change<ModKey, ModKey>(
                        ChangeReason.Add,
                        ModKey.FromNameAndExtension("ModA.esp"),
                        ModKey.FromNameAndExtension("ModA.esp"))
                }));
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
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
            err.Succeeded.Should().BeTrue();
        }
    }
}