using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive;
using AutoFixture;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubLiveListingsFileReaderTests : IClassFixture<Fixture>
    {
        private readonly Fixture _fixture;

        public CreationClubLiveListingsFileReaderTests(Fixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void NotUsed()
        {
            var scheduler = new TestScheduler();
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveListingsFileReader(
                        _fixture.Inject.Create<IFileSystem>(),
                        _fixture.Inject.Create<ICreationClubRawListingsReader>(),
                        new CreationClubPathInjection(null))
                    .Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            obs.Messages.Count.Should().Be(1);
            obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnCompleted)
                .Should().HaveCount(1);
            stateTest.Messages.Should().HaveCount(2);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            stateTest.Messages[1].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Fact]
        public void FileMissing()
        {
            var scheduler = new TestScheduler();
            var fs = new MockFileSystem()
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory()
            };
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveListingsFileReader(
                        fs,
                        _fixture.Inject.Create<ICreationClubRawListingsReader>(),
                        new CreationClubPathInjection("C:/SomeMissingPath"))
                    .Get(out var state);
                stateTest = scheduler.Start(() => state);
                return ret;
            });
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeFalse();
        }

        [Fact]
        public void FileExists()
        {
            var scheduler = new TestScheduler();
            var path = "C:/SomePath";
            var listingA = new ModListing("ModA.esp", true);
            var fs = Substitute.For<IFileSystem>();
            fs.File.OpenRead(path).Returns(_fixture.Inject.Create<Stream>());
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().BeEquivalentTo(listingA);
            var stateTest = scheduler.Start(() => state);
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void FileIsCreated()
        {
            var fileChanges = new MockFileSystemWatcher();
            var fs = new MockFileSystem()
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory(fileChanges)
            };
            var path = "C:/SomePath";
            var listingA = new ModListing("ModA.esp", true);
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(0);
            var scheduler = new TestScheduler();
            var stateTest = scheduler.Start(() => state);
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeFalse();
            fs.File.WriteAllText(path, string.Empty);
            fileChanges.MarkCreated(path);
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().BeEquivalentTo(listingA);
            stateTest = scheduler.Start(() => state);
            stateTest.Messages[^1].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[^1].Value.Value.Succeeded.Should().BeTrue();
        }

        [Fact]
        public void FileIsDeleted()
        {
            var fileChanges = new MockFileSystemWatcher();
            var fs = new MockFileSystem()
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory(fileChanges)
            };
            var path = "C:/SomePath";
            fs.File.WriteAllText(path, string.Empty);
            var listingA = new ModListing("ModA.esp", true);
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().BeEquivalentTo(listingA);
            var scheduler = new TestScheduler();
            var stateTest = scheduler.Start(() => state);
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            fs.File.Delete(path);
            fileChanges.MarkDeleted(path);
            list.Items.Should().HaveCount(0);
            stateTest = scheduler.Start(() => state);
            stateTest.Messages[^1].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[^1].Value.Value.Succeeded.Should().BeFalse();
        }

        [Fact]
        public void FileIsUpdated()
        {
            var fileChanges = new MockFileSystemWatcher();
            var fs = new MockFileSystem()
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory(fileChanges)
            };
            var path = "C:/SomePath";
            fs.File.WriteAllText(path, string.Empty);
            var listingA = new ModListing("ModA.esp", true);
            var listingB = new ModListing("ModB.esp", true);
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().BeEquivalentTo(listingA);
            var scheduler = new TestScheduler();
            var stateTest = scheduler.Start(() => state);
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable().And(listingB));
            fileChanges.MarkChanged(path);
            list.Items.Should().HaveCount(2);
            list.Items.Should().BeEquivalentTo(
                listingA,
                listingB);
            stateTest = scheduler.Start(() => state);
            stateTest.Messages[^1].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[^1].Value.Value.Succeeded.Should().BeTrue();
        }
    }
}