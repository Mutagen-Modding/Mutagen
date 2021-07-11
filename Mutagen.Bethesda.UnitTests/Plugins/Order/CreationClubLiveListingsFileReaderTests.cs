using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive;
using AutoFixture;
using AutoFixture.Xunit2;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using Noggog.Testing.AutoFixture;
using Noggog.Testing.FileSystem;
using NSubstitute;
using Xunit;
using MockFileSystemWatcherFactory = System.IO.Abstractions.TestingHelpers.MockFileSystemWatcherFactory;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubLiveListingsFileReaderTests : TypicalTest
    {
        [Fact]
        public void NotUsed()
        {
            var scheduler = new TestScheduler();
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveListingsFileReader(
                        Fixture.Create<IFileSystem>(),
                        Fixture.Create<ICreationClubRawListingsReader>(),
                        new CreationClubListingsPathInjection(null))
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

        [Theory, MutagenAutoData]
        public void FileMissing(
            [Frozen]MockFileSystem fs)
        {
            var scheduler = new TestScheduler();
            ITestableObserver<ErrorResponse> stateTest = null!;
            var obs = scheduler.Start(() =>
            {
                var ret = new CreationClubLiveListingsFileReader(
                        fs,
                        Fixture.Create<ICreationClubRawListingsReader>(),
                        new CreationClubListingsPathInjection("C:/SomeMissingPath"))
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
            fs.File.OpenRead(path).Returns(Fixture.Create<Stream>());
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubListingsPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().Be(listingA);
            var stateTest = scheduler.Start(() => state);
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void FileIsCreated(
            [Frozen]FilePath path,
            [Frozen]MockFileSystemWatcher fileChanges,
            [Frozen]MockFileSystem fs)
        {
            var listingA = new ModListing("ModA.esp", true);
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubListingsPathInjection(path))
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
            list.Items.First().Should().Be(listingA);
            stateTest = scheduler.Start(() => state);
            stateTest.Messages[^1].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[^1].Value.Value.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void FileIsDeleted(
            [Frozen]FilePath path,
            [Frozen]MockFileSystemWatcher fileChanges,
            [Frozen]MockFileSystem fs)
        {
            fs.File.WriteAllText(path, string.Empty);
            var listingA = new ModListing("ModA.esp", true);
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubListingsPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().Be(listingA);
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

        [Theory, MutagenAutoData]
        public void FileIsUpdated(
            [Frozen]FilePath path,
            [Frozen]MockFileSystemWatcher fileChanges,
            [Frozen]MockFileSystem fs)
        {
            fs.File.WriteAllText(path, string.Empty);
            var listingA = new ModListing("ModA.esp", true);
            var listingB = new ModListing("ModB.esp", true);
            var reader = Substitute.For<ICreationClubRawListingsReader>();
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
            var list = new CreationClubLiveListingsFileReader(
                    fs,
                    reader,
                    new CreationClubListingsPathInjection(path))
                .Get(out var state)
                .AsObservableList();
            list.Items.Should().HaveCount(1);
            list.Items.First().Should().Be(listingA);
            var scheduler = new TestScheduler();
            var stateTest = scheduler.Start(() => state);
            stateTest.Messages.Should().HaveCount(1);
            stateTest.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            
            reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable().And(listingB));
            fileChanges.MarkChanged(path);
            list.Items.Should().HaveCount(2);
            list.Items.Should().Equal(
                listingA,
                listingB);
            stateTest = scheduler.Start(() => state);
            stateTest.Messages[^1].Value.Kind.Should().Be(NotificationKind.OnNext);
            stateTest.Messages[^1].Value.Value.Succeeded.Should().BeTrue();
        }
    }
}