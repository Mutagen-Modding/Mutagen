using System.Collections.Generic;
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
using Xunit;
using MockFileSystemWatcherFactory = System.IO.Abstractions.TestingHelpers.MockFileSystemWatcherFactory;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubLiveLoadOrderFolderWatcherTests : TypicalTest
    {
        [Theory, MutagenAutoData]
        public void FolderDoesNotExist(
            [Frozen]MockFileSystem fs)
        {
            var scheduler = new TestScheduler();
            var obs = scheduler.Start(() =>
            {
                return new CreationClubLiveLoadOrderFolderWatcher(
                        fs,
                        new DataDirectoryInjection("C:/DoesNotExist"))
                    .Get();
            });
            obs.Messages.Should().HaveCount(1);
            obs.Messages[0].Value.Kind.Should().Be(NotificationKind.OnCompleted);
        }

        [Theory, MutagenAutoData]
        public void Empty(
            [Frozen]DirectoryPath dir,
            [Frozen]MockFileSystem fs)
        {
            var scheduler = new TestScheduler();
            var obs = scheduler.Start(() =>
            {
                return new CreationClubLiveLoadOrderFolderWatcher(
                        fs,
                        new DataDirectoryInjection(dir))
                    .Get();
            });
            obs.Messages.Should().HaveCount(0);
        }

        [Theory, MutagenAutoData]
        public void HasMod(
            [Frozen]ModKey modKey,
            CreationClubLiveLoadOrderFolderWatcher sut)
        {
            var list = sut
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(modKey);
        }

        [Theory, MutagenAutoData]
        public void ModAdded(
            [Frozen]ModKey modKey,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]MockFileSystemWatcher mockChange,
            [Frozen]MockFileSystem fs,
            CreationClubLiveLoadOrderFolderWatcher sut)
        {
            var modKeyB = ModKey.FromNameAndExtension("NewMod.esm");
            var modKeyBPath = Path.Combine(dataDir.Path, modKeyB.FileName);
            var list = sut
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(modKey);
            fs.File.WriteAllText(modKeyBPath, string.Empty);
            mockChange.MarkCreated(modKeyBPath);
            list.Count.Should().Be(2);
            list.Items.First().Should().Be(modKey);
            list.Items.Last().Should().Be(modKeyB);
        }

        [Theory, MutagenAutoData]
        public void ModRemovedz(
            [Frozen]MockFileSystemWatcher zz,
            [Frozen]MockFileSystem fsz,
            [Frozen]ModKey modKey)
        {
            var modKeyA = ModKey.FromNameAndExtension("ModA.esp");
            var modKeyB = ModKey.FromNameAndExtension("ModB.esm");
            var dataDir = $"C:/SomeDataDir";
            var modKeyBPath = Path.Combine(dataDir, modKeyB.FileName);
            var mockChange = new MockFileSystemWatcher();
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Path.Combine(dataDir, modKeyA.FileName), string.Empty },
                { Path.Combine(dataDir, modKeyB.FileName), string.Empty },
            })
            {
                FileSystemWatcher = new Noggog.Testing.FileSystem.MockFileSystemWatcherFactory(mockChange)
            };
            var list = new CreationClubLiveLoadOrderFolderWatcher(
                    fs,
                    new DataDirectoryInjection(dataDir))
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(2);
            list.Items.First().Should().Be(modKeyA);
            list.Items.Last().Should().Be(modKeyB);
            fs.File.Delete(modKeyBPath);
            mockChange.MarkDeleted(modKeyBPath);
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(modKeyA);
        }

        [Theory, MutagenAutoData]
        public void ModRemoved(
            [Frozen]MockFileSystemWatcher mockChange,
            [Frozen]MockFileSystem fs,
            [Frozen]ModKey modKey,
            [Frozen]IDataDirectoryProvider dataDir,
            CreationClubLiveLoadOrderFolderWatcher sut)
        {
            var modKeyB = ModKey.FromNameAndExtension("ModB.esm");
            var modKeyBPath = Path.Combine(dataDir.Path, modKeyB.FileName);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, modKeyB.FileName), string.Empty);
            var list = sut
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(2);
            list.Items.First().Should().Be(modKey);
            list.Items.Last().Should().Be(modKeyB);
            fs.File.Delete(modKeyBPath);
            mockChange.MarkDeleted(modKeyBPath);
            list.Count.Should().Be(1);
            list.Items.First().Should().Be(modKey);
        }
    }
}