using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutoFixture;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubLiveLoadOrderProviderTests : IClassFixture<Fixture>
    {
        private readonly Fixture _fixture;

        public CreationClubLiveLoadOrderProviderTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Neither()
        {
            var scheduler = new TestScheduler();
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return Observable.Empty<IChangeSet<IModListingGetter>>();
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            folderWatcher.Get().Returns(Observable.Empty<IChangeSet<ModKey, ModKey>>());
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveLoadOrderProvider(
                        fileReader,
                        folderWatcher)
                    .Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(0);
            stateTest.Messages.Count.Should().Be(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Fact]
        public void OnlyFile()
        {
            var scheduler = new TestScheduler();
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    }));
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            folderWatcher.Get().Returns(Observable.Empty<IChangeSet<ModKey, ModKey>>());
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveLoadOrderProvider(
                        fileReader,
                        folderWatcher)
                    .Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(0);
            stateTest.Messages.Count.Should().Be(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Fact]
        public void OnlyFolder()
        {
            var scheduler = new TestScheduler();
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return Observable.Empty<IChangeSet<IModListingGetter>>();
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            folderWatcher.Get().Returns(x =>
            {
                return Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    }));
            });
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveLoadOrderProvider(
                        fileReader,
                        folderWatcher)
                    .Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(0);
            stateTest.Messages.Count.Should().Be(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Fact]
        public void Both()
        {
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    }));
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            folderWatcher.Get().Returns(x =>
            {
                return Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    }));
            });
            var list = new CreationClubLiveLoadOrderProvider(
                    fileReader,
                    folderWatcher)
                .Get(out var state)
                .AsObservableList();
            ErrorResponse err = ErrorResponse.Failure;
            using var sub = state.Subscribe(x => err = x);
            list.Count.Should().Be(1);
            list.Items.First().Should().BeEquivalentTo(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void FileAdded()
        {
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            var fileSubj = new Subject<IChangeSet<IModListingGetter>>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return fileSubj;
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            folderWatcher.Get().Returns(x =>
            {
                return Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    }));
            });
            var list = new CreationClubLiveLoadOrderProvider(
                    fileReader,
                    folderWatcher)
                .Get(out var state)
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
            list.Items.First().Should().BeEquivalentTo(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void FolderAdded()
        {
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    }));
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            var folderSubj = new Subject<IChangeSet<ModKey, ModKey>>();
            folderWatcher.Get().Returns(x =>
            {
                return folderSubj;
            });
            var list = new CreationClubLiveLoadOrderProvider(
                    fileReader,
                    folderWatcher)
                .Get(out var state)
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
            list.Items.First().Should().BeEquivalentTo(
                new ModListing("ModA.esp", true));
            err.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void FileRemoved()
        {
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            var fileSubj = new Subject<IChangeSet<IModListingGetter>>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return fileSubj;
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            folderWatcher.Get().Returns(x =>
            {
                return Observable.Return(new ChangeSet<ModKey, ModKey>(
                    new Change<ModKey, ModKey>[]
                    {
                        new Change<ModKey, ModKey>(ChangeReason.Add, "ModA.esp", "ModA.esp")
                    }));
            });
            var list = new CreationClubLiveLoadOrderProvider(
                    fileReader,
                    folderWatcher)
                .Get(out var state)
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
            list.Items.First().Should().BeEquivalentTo(
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

        [Fact]
        public void FolderRemoved()
        {
            var fileReader = Substitute.For<ICreationClubLiveListingsFileReader>();
            fileReader.Get(out var _).Returns(x =>
            {
                x[0] = Observable.Return(ErrorResponse.Success);
                return Observable.Return(new ChangeSet<IModListingGetter>(
                    new Change<IModListingGetter>[]
                    {
                        new Change<IModListingGetter>(
                            ListChangeReason.Add,
                            new ModListing("ModA.esp", true))
                    }));
            });
            var folderWatcher = Substitute.For<ICreationClubLiveLoadOrderFolderWatcher>();
            var folderSubj = new Subject<IChangeSet<ModKey, ModKey>>();
            folderWatcher.Get().Returns(x =>
            {
                return folderSubj;
            });
            var list = new CreationClubLiveLoadOrderProvider(
                    fileReader,
                    folderWatcher)
                .Get(out var state)
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
            list.Items.First().Should().BeEquivalentTo(
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