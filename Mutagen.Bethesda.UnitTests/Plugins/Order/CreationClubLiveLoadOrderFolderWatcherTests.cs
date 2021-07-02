using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubLiveLoadOrderFolderWatcherTests : IClassFixture<Fixture>
    {
        private readonly Fixture _fixture;

        public CreationClubLiveLoadOrderFolderWatcherTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void FolderDoesNotExist()
        {
            var scheduler = new TestScheduler();
            var fs = new MockFileSystem()
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory()
            };
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

        [Fact]
        public void Empty()
        {
            var scheduler = new TestScheduler();
            var path = "C:/SomeDataDir";
            var fs = new MockFileSystem()
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory()
            };
            fs.Directory.CreateDirectory(path);
            var obs = scheduler.Start(() =>
            {
                return new CreationClubLiveLoadOrderFolderWatcher(
                        fs,
                        new DataDirectoryInjection(path))
                    .Get();
            });
            obs.Messages.Should().HaveCount(0);
        }

        [Fact]
        public void HasMod()
        {
            var modKeyA = ModKey.FromNameAndExtension("ModA.esp");
            var dataDir = $"C:/SomeDataDir";
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Path.Combine(dataDir, modKeyA.FileName), string.Empty }
            })
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory()
            };
            var list = new CreationClubLiveLoadOrderFolderWatcher(
                    fs,
                    new DataDirectoryInjection(dataDir))
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(1);
            list.Items.First().Should().BeEquivalentTo(modKeyA);
        }

        [Fact]
        public void ModAdded()
        {
            var modKeyA = ModKey.FromNameAndExtension("ModA.esp");
            var modKeyB = ModKey.FromNameAndExtension("ModB.esm");
            var dataDir = $"C:/SomeDataDir";
            var modKeyBPath = Path.Combine(dataDir, modKeyB.FileName);
            var mockChange = new MockFileSystemWatcher();
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Path.Combine(dataDir, modKeyA.FileName), string.Empty }
            })
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory(mockChange)
            };
            var list = new CreationClubLiveLoadOrderFolderWatcher(
                    fs,
                    new DataDirectoryInjection(dataDir))
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(1);
            list.Items.First().Should().BeEquivalentTo(modKeyA);
            fs.File.WriteAllText(modKeyBPath, string.Empty);
            mockChange.MarkCreated(modKeyBPath);
            list.Count.Should().Be(2);
            list.Items.First().Should().BeEquivalentTo(modKeyA);
            list.Items.Last().Should().BeEquivalentTo(modKeyB);
        }

        [Fact]
        public void ModRemoved()
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
                FileSystemWatcher = new MockFileSystemWatcherFactory(mockChange)
            };
            var list = new CreationClubLiveLoadOrderFolderWatcher(
                    fs,
                    new DataDirectoryInjection(dataDir))
                .Get()
                .AsObservableCache();
            list.Count.Should().Be(2);
            list.Items.First().Should().BeEquivalentTo(modKeyA);
            list.Items.Last().Should().BeEquivalentTo(modKeyB);
            fs.File.Delete(modKeyBPath);
            mockChange.MarkDeleted(modKeyBPath);
            list.Count.Should().Be(1);
            list.Items.First().Should().BeEquivalentTo(modKeyA);
        }
    }
}