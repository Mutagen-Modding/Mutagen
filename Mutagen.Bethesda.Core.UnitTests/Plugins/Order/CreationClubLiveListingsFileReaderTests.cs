using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive;
using AutoFixture.Xunit2;
using DynamicData;
using Shouldly;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Noggog.Testing.FileSystem;
using Noggog.Testing.IO;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubLiveListingsFileReaderTests
{
    [Theory, MutagenAutoData(Release: GameRelease.Oblivion)]
    public void NotUsed(
        TestScheduler scheduler,
        CreationClubLiveListingsFileReader sut)
    {
        ITestableObserver<ErrorResponse> stateTest = null!;
        var obs = scheduler.Start(() =>
        {
            var ret = sut.Get(out var state);
            stateTest = scheduler.Start(() => state);
            return ret;
        });
        obs.Messages.Count.ShouldBe(1);
        obs.Messages.Where(x => x.Value.Kind == NotificationKind.OnCompleted)
            .ShouldHaveCount(1);
        stateTest.Messages.ShouldHaveCount(2);
        stateTest.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[0].Value.Value.Succeeded.ShouldBeTrue();
        stateTest.Messages[1].Value.Kind.ShouldBe(NotificationKind.OnCompleted);
    }

    [Theory, MutagenAutoData]
    public void FileMissing(
        FilePath missingPath,
        [Frozen]TestScheduler scheduler,
        [Frozen]ICreationClubListingsPathProvider cccPath,
        CreationClubLiveListingsFileReader sut)
    {
        cccPath.Path.Returns(missingPath);
        ITestableObserver<ErrorResponse> stateTest = null!;
        scheduler.Start(() =>
        {
            var ret = sut
                .Get(out var state);
            stateTest = scheduler.Start(() => state);
            return ret;
        });
        stateTest.Messages.ShouldHaveCount(1);
        stateTest.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[0].Value.Value.Succeeded.ShouldBeFalse();
    }

    [Theory, MutagenAutoData]
    public void FileExists(FileSystemStream stream)
    {
        var scheduler = new TestScheduler();
        var path = $"{PathingUtil.DrivePrefix}SomePath";
        var listingA = new LoadOrderListing("ModA.esp", true);
        var fs = Substitute.For<IFileSystem>();
        fs.File.OpenRead(path).Returns(stream);
        var reader = Substitute.For<ICreationClubRawListingsReader>();
        reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
        var list = new CreationClubLiveListingsFileReader(
                fs,
                reader,
                new CreationClubListingsPathInjection(path))
            .Get(out var state)
            .AsObservableList();
        list.Items.ShouldHaveCount(1);
        list.Items.First().ShouldBe(listingA);
        var stateTest = scheduler.Start(() => state);
        stateTest.Messages.ShouldHaveCount(1);
        stateTest.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[0].Value.Value.Succeeded.ShouldBeTrue();
    }

    [Theory, MutagenAutoData]
    public void FileIsCreated(
        [Frozen]FilePath path,
        [Frozen]MockFileSystemWatcher fileChanges,
        [Frozen]MockFileSystem fs)
    {
        var listingA = new LoadOrderListing("ModA.esp", true);
        var reader = Substitute.For<ICreationClubRawListingsReader>();
        reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
        var list = new CreationClubLiveListingsFileReader(
                fs,
                reader,
                new CreationClubListingsPathInjection(path))
            .Get(out var state)
            .AsObservableList();
        list.Items.ShouldHaveCount(0);
        var scheduler = new TestScheduler();
        var stateTest = scheduler.Start(() => state);
        stateTest.Messages.ShouldHaveCount(1);
        stateTest.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[0].Value.Value.Succeeded.ShouldBeFalse();
        fs.File.WriteAllText(path, string.Empty);
        fileChanges.MarkCreated(path);
        list.Items.ShouldHaveCount(1);
        list.Items.First().ShouldBe(listingA);
        stateTest = scheduler.Start(() => state);
        stateTest.Messages[^1].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[^1].Value.Value.Succeeded.ShouldBeTrue();
    }

    [Theory, MutagenAutoData]
    public void FileIsDeleted(
        [Frozen]FilePath path,
        [Frozen]MockFileSystemWatcher fileChanges,
        [Frozen]MockFileSystem fs)
    {
        fs.File.WriteAllText(path, string.Empty);
        var listingA = new LoadOrderListing("ModA.esp", true);
        var reader = Substitute.For<ICreationClubRawListingsReader>();
        reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
        var list = new CreationClubLiveListingsFileReader(
                fs,
                reader,
                new CreationClubListingsPathInjection(path))
            .Get(out var state)
            .AsObservableList();
        list.Items.ShouldHaveCount(1);
        list.Items.First().ShouldBe(listingA);
        var scheduler = new TestScheduler();
        var stateTest = scheduler.Start(() => state);
        stateTest.Messages.ShouldHaveCount(1);
        stateTest.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[0].Value.Value.Succeeded.ShouldBeTrue();
        fs.File.Delete(path);
        fileChanges.MarkDeleted(path);
        list.Items.ShouldHaveCount(0);
        stateTest = scheduler.Start(() => state);
        stateTest.Messages[^1].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[^1].Value.Value.Succeeded.ShouldBeFalse();
    }

    [Theory, MutagenAutoData]
    public void FileIsUpdated(
        [Frozen]FilePath path,
        [Frozen]MockFileSystemWatcher fileChanges,
        [Frozen]MockFileSystem fs)
    {
        fs.File.WriteAllText(path, string.Empty);
        var listingA = new LoadOrderListing("ModA.esp", true);
        var listingB = new LoadOrderListing("ModB.esp", true);
        var reader = Substitute.For<ICreationClubRawListingsReader>();
        reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable());
        var list = new CreationClubLiveListingsFileReader(
                fs,
                reader,
                new CreationClubListingsPathInjection(path))
            .Get(out var state)
            .AsObservableList();
        list.Items.ShouldHaveCount(1);
        list.Items.First().ShouldBe(listingA);
        var scheduler = new TestScheduler();
        var stateTest = scheduler.Start(() => state);
        stateTest.Messages.ShouldHaveCount(1);
        stateTest.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[0].Value.Value.Succeeded.ShouldBeTrue();
            
        reader.Read(Arg.Any<Stream>()).Returns(listingA.AsEnumerable().And(listingB));
        fileChanges.MarkChanged(path);
        list.Items.ShouldHaveCount(2);
        list.Items.ShouldEqualEnumerable(
            listingA,
            listingB);
        stateTest = scheduler.Start(() => state);
        stateTest.Messages[^1].Value.Kind.ShouldBe(NotificationKind.OnNext);
        stateTest.Messages[^1].Value.Value.Succeeded.ShouldBeTrue();
    }
}