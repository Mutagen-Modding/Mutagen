using System.IO.Abstractions.TestingHelpers;
using System.Reactive;
using AutoFixture.Xunit2;
using DynamicData;
using Shouldly;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Noggog.Testing.FileSystem;
using Noggog.Testing.IO;
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
        sut.DataDirectory.Path.Returns(new DirectoryPath($"{PathingUtil.DrivePrefix}DoesNotExist"));
        var obs = scheduler.Start(() =>
        {
            return sut.Get();
        });
        obs.Messages.ShouldHaveCount(1);
        obs.Messages[0].Value.Kind.ShouldBe(NotificationKind.OnCompleted);
    }

    [Theory, MutagenAutoData]
    public void Empty(
        [Frozen]TestScheduler scheduler,
        MockFileSystem fs,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        fs.Directory.CreateDirectory($"{PathingUtil.DrivePrefix}Missing");
        sut.DataDirectory.Path.Returns(new DirectoryPath($"{PathingUtil.DrivePrefix}Missing"));
        var obs = scheduler.Start(() =>
        {
            return sut.Get();
        });
        obs.Messages.ShouldHaveCount(0);
    }

    [Theory, MutagenAutoData]
    public void HasMod(
        ModKey existingModKey,
        CreationClubLiveLoadOrderFolderWatcher sut)
    {
        var list = sut
            .Get()
            .AsObservableCache();
        list.Count.ShouldBe(1);
        list.Items.First().ShouldBe(existingModKey);
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
        list.Count.ShouldBe(1);
        list.Items.First().ShouldBe(existingModKey);
            
        var modKeyB = ModKey.FromNameAndExtension("NewMod.esm");
        var modKeyBPath = Path.Combine(dataDir.Path, modKeyB.FileName);
        fs.File.WriteAllText(modKeyBPath, string.Empty);
        mockChange.MarkCreated(modKeyBPath);
        list.Count.ShouldBe(2);
        list.Items.First().ShouldBe(existingModKey);
        list.Items.Last().ShouldBe(modKeyB);
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
        list.Count.ShouldBe(2);
        list.Items.First().ShouldBe(existingModKey);
        list.Items.Last().ShouldBe(modKeyB);
        fs.File.Delete(modKeyBPath);
        mockChange.MarkDeleted(modKeyBPath);
        list.Count.ShouldBe(1);
        list.Items.First().ShouldBe(existingModKey);
    }
}