using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LiveLoadOrderIntegrationTests
    {
        [Fact]
        public async Task LiveLoadOrder()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(LiveLoadOrderIntegrationTests));
            var pluginPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataFolderPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(dataFolderPath);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.Skyrim.FileName), string.Empty);
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.PluginModKey}",
                });
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
                });
            await Task.Delay(1000);
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimSE, pluginPath, dataFolderPath, out var state, cccLoadOrderFilePath: cccPath);
            state.Subscribe(x =>
            {
                if (x.Failed) throw x.Exception ?? new Exception();
            });
            var list = live.AsObservableList();
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.PluginModKey,
            });

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                });
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey2.FileName), string.Empty);
            File.Delete(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName));
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey2,
                Utility.MasterModKey,
                Utility.PluginModKey,
            });

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                });
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey2,
                Utility.MasterModKey,
                Utility.MasterModKey2,
            });

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.PluginModKey}",
                });
            File.Delete(Path.Combine(dataFolderPath, Utility.LightMasterModKey2.FileName));
            await Task.Delay(1000);
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
            File.Delete(Path.Combine(dataFolderPath, Utility.MasterModKey2.FileName));
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.MasterModKey,
                Utility.MasterModKey2,
                Utility.PluginModKey,
            });
        }

        [Fact]
        public async Task LiveLoadOrder_EnsureReaddRetainsOrder()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(LiveLoadOrderIntegrationTests));
            var pluginPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataFolderPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(dataFolderPath);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.Skyrim.FileName), string.Empty);
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.MasterModKey3}",
                    $"*{Utility.PluginModKey}",
                });
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
                });
            await Task.Delay(1000);
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimSE, pluginPath, dataFolderPath, out var state, cccLoadOrderFilePath: cccPath);
            var list = live.AsObservableList();
            await Task.Delay(1000);
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
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey3}",
                    $"*{Utility.PluginModKey}",
                });
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().Equal(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Utility.LightMasterModKey,
                Utility.MasterModKey,
                Utility.MasterModKey3,
                Utility.PluginModKey,
            });

            // Then readd
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.MasterModKey2}",
                    $"*{Utility.MasterModKey3}",
                    $"*{Utility.PluginModKey}",
                });
            await Task.Delay(1000);
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
        [Fact]
        public async Task LiveLoadOrder_PluginsCCListingReorders()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(LiveLoadOrderIntegrationTests));
            var pluginPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataFolderPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(dataFolderPath);
            File.WriteAllText(Path.Combine(dataFolderPath, Skyrim.Constants.Skyrim.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Skyrim.Constants.Update.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Skyrim.Constants.Dawnguard.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey2.FileName), string.Empty);
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                });
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimSE, pluginPath, dataFolderPath, out var state, cccLoadOrderFilePath: cccPath);
            var list = live.AsObservableList();
            await Task.Delay(1000);
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

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                    $"{Utility.LightMasterModKey}",
                });
            await Task.Delay(1000);
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

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey}",
                    $"{Utility.LightMasterModKey}",
                    $"{Utility.LightMasterModKey2}",
                });
            await Task.Delay(1000);
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

        [Fact]
        public async Task LiveLoadOrder_DontReorderPluginsFile()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(LiveLoadOrderIntegrationTests));
            var pluginPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataFolderPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(dataFolderPath);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.Skyrim.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    $"*{Utility.PluginModKey}",
                    $"*{Utility.MasterModKey}",
                    $"*{Utility.PluginModKey2}",
                });
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
                });
            await Task.Delay(1000);
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimSE, pluginPath, dataFolderPath, out var state, cccLoadOrderFilePath: cccPath);
            var list = live.AsObservableList();
            await Task.Delay(1000);
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
