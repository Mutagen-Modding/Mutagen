using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive;
using AutoFixture.Xunit2;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using Noggog.Testing.FileSystem;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubLiveLoadOrderFolderWatcherTests
{
    [Theory, MutagenAutoData]
    public void FolderDoesNotExist(
        [Frozen]TestScheduler scheduler,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        sut.DataDirectory.Path.Returns(new DirectoryPath("C:/DoesNotExist"));
        var obs = scheduler.Start(() =>
        {
            return sut.Get();
        });
        obs.Messages.Should().HaveCount(1);
        obs.Messages[0].Value.Kind.Should().Be(NotificationKind.OnCompleted);
    }

    [Theory, MutagenAutoData]
    public void Empty(
        [Frozen]TestScheduler scheduler,
        MockFileSystem fs,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        fs.Directory.CreateDirectory("C:/Missing");
        sut.DataDirectory.Path.Returns(new DirectoryPath("C:/Missing"));
        var obs = scheduler.Start(() =>
        {
            return sut.Get();
        });
        obs.Messages.Should().HaveCount(0);
    }

    [Theory, MutagenAutoData]
    public void HasMod(
        ModKey existingModKey,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        var list = sut
            .Get()
            .AsObservableCache();
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(existingModKey);
    }

    [Theory, MutagenAutoData]
    public void ModAdded(
        ModKey existingModKey,
        IDataDirectoryProvider dataDir,
        MockFileSystemWatcher mockChange,
        MockFileSystem fs,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        fs.File.WriteAllText(Path.Combine(dataDir.Path, existingModKey.FileName), string.Empty);
        var list = sut
            .Get()
            .AsObservableCache();
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(existingModKey);
            
        var modKeyB = ModKey.FromNameAndExtension("NewMod.esm");
        var modKeyBPath = Path.Combine(dataDir.Path, modKeyB.FileName);
        fs.File.WriteAllText(modKeyBPath, string.Empty);
        mockChange.MarkCreated(modKeyBPath);
        list.Count.Should().Be(2);
        list.Items.First().Should().Be(existingModKey);
        list.Items.Last().Should().Be(modKeyB);
    }

    [Theory, MutagenAutoData]
    public void ModRemoved(
        ModKey existingModKey,
        MockFileSystemWatcher mockChange,
        MockFileSystem fs,
        IDataDirectoryProvider dataDir,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        var modKeyB = ModKey.FromNameAndExtension("ModB.esm");
        var modKeyBPath = Path.Combine(dataDir.Path, modKeyB.FileName);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, modKeyB.FileName), string.Empty);
        var list = sut
            .Get()
            .AsObservableCache();
        list.Count.Should().Be(2);
        list.Items.First().Should().Be(existingModKey);
        list.Items.Last().Should().Be(modKeyB);
        fs.File.Delete(modKeyBPath);
        mockChange.MarkDeleted(modKeyBPath);
        list.Count.Should().Be(1);
        list.Items.First().Should().Be(existingModKey);
    }
}