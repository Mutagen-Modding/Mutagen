using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Concurrency;
using AutoFixture.Xunit2;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.UnitTests.AutoData;
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
            var lightMasterPath = Path.Combine(dataDir.Path, Utility.LightMasterModKey.FileName);
            var lightMaster2Path = Path.Combine(dataDir.Path, Utility.LightMasterModKey2.FileName);
            var master2Path = Path.Combine(dataDir.Path, Utility.MasterModKey2.FileName);
            
            fs.File.WriteAllText(lightMasterPath, string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.Skyrim.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.PluginModKey}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
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
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.PluginModKey,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            fs.File.WriteAllText(lightMaster2Path, string.Empty);
            watcher.MarkCreated(lightMaster2Path);
            fs.File.Delete(lightMasterPath);
            watcher.MarkDeleted(lightMasterPath);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey2,
                Utility.MasterModKey,
                Utility.PluginModKey,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey2,
                Utility.MasterModKey,
                Utility.MasterModKey2,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            fs.File.Delete(lightMaster2Path);
            watcher.MarkDeleted(lightMaster2Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.PluginModKey,
            });

            // Does not respect just data folder modification
            // Since ModListing doesn't specify whether data folder is present
            // Data folder is just used for Timestamp alignment for Oblivion
            fs.File.Delete(master2Path);
            watcher.MarkDeleted(master2Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.PluginModKey,
            });
        }

        [Theory, MutagenAutoData(false)]
        public void LiveLoadOrder_EnsureReaddRetainsOrder(
            [Frozen]IScheduler scheduler,
            [Frozen]MockFileSystemWatcher watcher,
            [Frozen]MockFileSystem fs,
            [Frozen]ILiveLoadOrderTimings timings, 
            [Frozen]IPluginListingsPathProvider pluginPath,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]ICreationClubListingsPathProvider cccPath)
        {
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.Skyrim.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.MasterModKey3}",
                    $"*{Utility.PluginModKey}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
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
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.MasterModKey3,
                Utility.PluginModKey,
            });

            // Remove
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey3}",
                    $"*{Utility.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.MasterModKey3,
                Utility.PluginModKey,
            });

            // Then readd
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.MasterModKey3}",
                    $"*{Utility.PluginModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.MasterModKey3,
                Utility.PluginModKey,
            });
        }

        // Vortex puts CC mods on the plugins file, as unactivated.  Seemingly to drive load order?
        // This ensures that is respected
        [Theory, MutagenAutoData(false)]
        public void LiveLoadOrder_PluginsCCListingReorders(
            [Frozen]IScheduler scheduler,
            [Frozen]MockFileSystemWatcher watcher,
            [Frozen]MockFileSystem fs,
            [Frozen]ILiveLoadOrderTimings timings, 
            [Frozen]IPluginListingsPathProvider pluginPath,
            [Frozen]IDataDirectoryProvider dataDir,
            [Frozen]ICreationClubListingsPathProvider cccPath)
        {
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Skyrim.Constants.Skyrim.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Skyrim.Constants.Update.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Skyrim.Constants.Dawnguard.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.LightMasterModKey2.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
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
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Update,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey,
                Utility.LightMasterModKey2,
                Utility.MasterModKey,
                Utility.PluginModKey,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                    $"{Utility.LightMasterModKey}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Update,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey2,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.PluginModKey,
            });

            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                    $"{Utility.LightMasterModKey}",
                    $"{Utility.LightMasterModKey2}",
                });
            watcher.MarkChanged(pluginPath.Path);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Update,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey,
                Utility.LightMasterModKey2,
                Utility.MasterModKey,
                Utility.PluginModKey,
            });
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
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.Skyrim.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataDir.Path, Utility.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllLines(pluginPath.Path,
                new string[]
                {
                    $"*{Utility.PluginModKey}",
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey2}",
                });
            fs.File.WriteAllLines(cccPath.Path,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
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
                Utility.Skyrim,
                Utility.LightMasterModKey,
                Utility.PluginModKey,
                Utility.MasterModKey,
                Utility.PluginModKey2,
            });
        }
    }
}
