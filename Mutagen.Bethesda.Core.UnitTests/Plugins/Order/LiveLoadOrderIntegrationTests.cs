using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Concurrency;
using AutoFixture.Xunit2;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Core.UnitTests.AutoData;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Noggog.Testing.FileSystem;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LiveLoadOrderIntegrationTests
    {
        [Theory, MutagenAutoData(false)]
        public void LiveLoadOrder(
            [Frozen]IScheduler scheduler,
            [Frozen]MockFileSystemWatcher watcher,
            [Frozen]MockFileSystem fs,
            [Frozen]ILiveLoadOrderTimings timings, 
            [Frozen]IPluginListingsPathProvider pluginPath,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]ICreationClubListingsPathProvider cccPath)
        {
            var implicitKey = Implicits.Get(GameRelease.SkyrimSE).Listings.First();
            var lightMasterPath = Path.Combine(dataDir.Path, TestConstants.LightMasterModKey.FileName);
            var lightMaster2Path = Path.Combine(dataDir.Path, TestConstants.LightMasterModKey2.FileName);
            var master2Path = Path.Combine(dataDir.Path, TestConstants.MasterModKey2.FileName);
            
            fs.File.WriteAllText(lightMasterPath, string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, TestConstants.Skyrim.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.MasterModKey2}",
                    $"*{TestConstants.PluginModKey}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    TestConstants.LightMasterModKey.ToString(),
                    TestConstants.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(
                GameRelease.SkyrimSE,
                pluginPath.Path,
                dataDir.Path, 
                out var state,
                scheduler: scheduler,
                cccLoadOrderFilePath: cccPath.Path,
                timings: timings,
                fileSystem: fs);
            state.Subscribe(x =>
            {
                if (x.Failed) throw x.Exception ?? new Exception();
            });
            var list = live.AsObservableList();
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.LightMasterModKey,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey2,
                TestConstants.PluginModKey,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            fs.File.WriteAllText(lightMaster2Path, string.Empty);
            watcher.MarkCreated(lightMaster2Path);
            fs.File.Delete(lightMasterPath);
            watcher.MarkDeleted(lightMasterPath);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.LightMasterModKey2,
                TestConstants.MasterModKey,
                TestConstants.PluginModKey,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.MasterModKey2}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.LightMasterModKey2,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey2,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.MasterModKey2}",
                    $"*{TestConstants.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            fs.File.Delete(lightMaster2Path);
            watcher.MarkDeleted(lightMaster2Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey2,
                TestConstants.PluginModKey,
            });

            // Does not respect just data folder modification
            // Since ModListing doesn't specify whether data folder is present
            // Data folder is just used for Timestamp alignment for Oblivion
            fs.File.Delete(master2Path);
            watcher.MarkDeleted(master2Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey2,
                TestConstants.PluginModKey,
            });
        }

        [Theory, MutagenAutoData(true)]
        public void LiveLoadOrder_EnsureReaddRetainsOrder(
            [Frozen]IScheduler scheduler,
            [Frozen]MockFileSystemWatcher watcher,
            [Frozen]MockFileSystem fs,
            [Frozen]ILiveLoadOrderTimings timings, 
            [Frozen]IPluginListingsPathProvider pluginPath,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]ICreationClubListingsPathProvider cccPath)
        {
            var implicitKey = Implicits.Get(GameRelease.SkyrimSE).Listings.First();
            fs.File.WriteAllText(Path.Combine(dataDir.Path, TestConstants.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, implicitKey.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.MasterModKey2}",
                    $"*{TestConstants.MasterModKey3}",
                    $"*{TestConstants.PluginModKey}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    TestConstants.LightMasterModKey.ToString(),
                    TestConstants.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(
                GameRelease.SkyrimSE,
                pluginPath.Path,
                dataDir.Path, 
                out var state, 
                cccLoadOrderFilePath: cccPath.Path,
                scheduler: scheduler,
                fileSystem: fs,
                timings: timings);
            var list = live.AsObservableList();
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.LightMasterModKey,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey2,
                TestConstants.MasterModKey3,
                TestConstants.PluginModKey,
            });

            // Remove
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.MasterModKey3}",
                    $"*{TestConstants.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.LightMasterModKey,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey3,
                TestConstants.PluginModKey,
            });

            // Then readd
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.MasterModKey2}",
                    $"*{TestConstants.MasterModKey3}",
                    $"*{TestConstants.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                implicitKey,
                TestConstants.LightMasterModKey,
                TestConstants.MasterModKey,
                TestConstants.MasterModKey2,
                TestConstants.MasterModKey3,
                TestConstants.PluginModKey,
            });
        }

        // Vortex puts CC mods on the plugins file, as unactivated.  Seemingly to drive load order?
        // This ensures that is respected
        [Theory, MutagenAutoData(false)]
        public void LiveLoadOrder_PluginsCCListingReorders(
            [Frozen]IScheduler scheduler,
            [Frozen]MockFileSystemWatcher watcher,
            [Frozen]MockFileSystem fs,
            [Frozen]IImplicitListingModKeyProvider implicitListingsProvider,
            [Frozen]ILiveLoadOrderTimings timings, 
            [Frozen]IPluginListingsPathProvider pluginPath,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]ICreationClubListingsPathProvider cccPath)
        {
            var implicitKeys = implicitListingsProvider.Listings.ToArray();
            foreach (var implicitListing in implicitKeys)
            {
                fs.File.WriteAllText(Path.Combine(dataDir.Path, implicitListing.FileName), string.Empty);
            }
            fs.File.WriteAllText(Path.Combine(dataDir.Path, TestConstants.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, TestConstants.LightMasterModKey2.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.PluginModKey}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    TestConstants.LightMasterModKey.ToString(),
                    TestConstants.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(
                GameRelease.SkyrimSE, 
                pluginPath.Path, 
                dataDir.Path, 
                out var state, 
                scheduler: scheduler,
                cccLoadOrderFilePath: cccPath.Path,
                fileSystem: fs,
                timings: timings);
            var list = live.AsObservableList();
            list.Items.Select(x => x.ModKey).Should().Equal(
                implicitKeys.Concat(
                    new ModKey[]
                    {
                        TestConstants.LightMasterModKey,
                        TestConstants.LightMasterModKey2,
                        TestConstants.MasterModKey,
                        TestConstants.PluginModKey,
                    }));

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.PluginModKey}",
                    $"{TestConstants.LightMasterModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(
                implicitKeys.Concat(
                    new ModKey[] {
                        TestConstants.LightMasterModKey2,
                        TestConstants.LightMasterModKey,
                        TestConstants.MasterModKey,
                        TestConstants.PluginModKey,
                    }));

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.PluginModKey}",
                    $"{TestConstants.LightMasterModKey}",
                    $"{TestConstants.LightMasterModKey2}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(
                implicitKeys.Concat(
                    new ModKey[]
                    {
                        TestConstants.LightMasterModKey,
                        TestConstants.LightMasterModKey2,
                        TestConstants.MasterModKey,
                        TestConstants.PluginModKey,
                    }));
        }

        [Theory, MutagenAutoData(false)]
        public void LiveLoadOrder_DontReorderPluginsFile(
            [Frozen]IScheduler scheduler,
            [Frozen]MockFileSystem fs,
            [Frozen]ILiveLoadOrderTimings timings, 
            [Frozen]IPluginListingsPathProvider pluginPath,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]ICreationClubListingsPathProvider cccPath)
        {
            fs.File.WriteAllText(Path.Combine(dataDir.Path, TestConstants.Skyrim.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, TestConstants.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{TestConstants.PluginModKey}",
                    $"*{TestConstants.MasterModKey}",
                    $"*{TestConstants.PluginModKey2}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    TestConstants.LightMasterModKey.ToString(),
                    TestConstants.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(
                GameRelease.SkyrimSE,
                pluginPath.Path,
                dataDir.Path, 
                out var state, 
                cccLoadOrderFilePath: cccPath.Path,
                scheduler: scheduler,
                fileSystem: fs,
                timings: timings);
            var list = live.AsObservableList();
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                TestConstants.Skyrim,
                TestConstants.LightMasterModKey,
                TestConstants.PluginModKey,
                TestConstants.MasterModKey,
                TestConstants.PluginModKey2,
            });
        }
    }
}
