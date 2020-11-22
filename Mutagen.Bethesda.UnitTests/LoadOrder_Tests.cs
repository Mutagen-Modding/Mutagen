using DynamicData;
using FluentAssertions;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class LoadOrder_Tests
    {
        [Fact]
        public void AlignToTimestamps_Typical()
        {
            var lo = new List<(ModKey ModKey, DateTime Write)>
            {
                (Utility.PluginModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
                (Utility.PluginModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
                (Utility.PluginModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
                (Utility.PluginModKey2, new DateTime(2020, 8, 8, 10, 9, 0)),
            };
            var results = LoadOrder.AlignToTimestamps(lo)
                .ToList();
            Assert.Equal(Utility.PluginModKey, results[0]);
            Assert.Equal(Utility.PluginModKey2, results[1]);
            Assert.Equal(Utility.PluginModKey3, results[2]);
            Assert.Equal(Utility.PluginModKey4, results[3]);
        }

        [Fact]
        public void AlignToTimestamps_SameTimestamps()
        {
            var lo = new List<(ModKey ModKey, DateTime Write)>
            {
                (Utility.PluginModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
                (Utility.PluginModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
                (Utility.PluginModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
                (Utility.PluginModKey2, new DateTime(2020, 8, 8, 10, 11, 0)),
            };
            var results = LoadOrder.AlignToTimestamps(lo)
                .ToList();
            Assert.Equal(Utility.PluginModKey, results[0]);
            Assert.Equal(Utility.PluginModKey3, results[1]);
            Assert.Equal(Utility.PluginModKey4, results[2]);
            Assert.Equal(Utility.PluginModKey2, results[3]);
        }

        [Fact]
        public void GetListings()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(GetListings)));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.FileName,
                    Utility.LightMasterModKey2.FileName,
                });
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"*{Utility.MasterModKey.FileName}",
                    $"{Utility.MasterModKey2.FileName}",
                    $"*{Utility.LightMasterModKey3.FileName}",
                    $"{Utility.LightMasterModKey4.FileName}",
                    $"*{Utility.PluginModKey.FileName}",
                    $"{Utility.PluginModKey2.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey3.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey4.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey2.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(7);
            results.Should().BeEquivalentTo(new LoadOrderListing[]
            {
                new LoadOrderListing(Utility.LightMasterModKey, enabled: true),
                new LoadOrderListing(Utility.MasterModKey, enabled: true),
                new LoadOrderListing(Utility.MasterModKey2, enabled: false),
                new LoadOrderListing(Utility.LightMasterModKey3, enabled: true),
                new LoadOrderListing(Utility.LightMasterModKey4, enabled: false),
                new LoadOrderListing(Utility.PluginModKey, enabled: true),
                new LoadOrderListing(Utility.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void GetListings_CreationClubMissing()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(GetListings_CreationClubMissing)));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"*{Utility.MasterModKey.FileName}",
                    $"{Utility.MasterModKey2.FileName}",
                    $"*{Utility.LightMasterModKey3.FileName}",
                    $"{Utility.LightMasterModKey4.FileName}",
                    $"*{Utility.PluginModKey.FileName}",
                    $"{Utility.PluginModKey2.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey3.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey4.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey2.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(6);
            results.Should().BeEquivalentTo(new LoadOrderListing[]
            {
                new LoadOrderListing(Utility.MasterModKey, enabled: true),
                new LoadOrderListing(Utility.MasterModKey2, enabled: false),
                new LoadOrderListing(Utility.LightMasterModKey3, enabled: true),
                new LoadOrderListing(Utility.LightMasterModKey4, enabled: false),
                new LoadOrderListing(Utility.PluginModKey, enabled: true),
                new LoadOrderListing(Utility.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void GetListings_NoCreationClub()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(GetListings_NoCreationClub)));
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"{Utility.MasterModKey.FileName}",
                    $"{Utility.PluginModKey.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.Oblivion,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: null)
                .ToList();
            results.Should().HaveCount(2);
            results.Should().BeEquivalentTo(new LoadOrderListing[]
            {
                new LoadOrderListing(Utility.MasterModKey, enabled: true),
                new LoadOrderListing(Utility.PluginModKey, enabled: true),
            });
        }

        /// <summary>
        /// Vortex lists creation club entries at the start of the plugins.txt, but leaves them marked
        /// as not active?
        /// </summary>
        [Fact]
        public void GetListings_VortexCreationClub()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(GetListings)));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.FileName,
                    Utility.LightMasterModKey2.FileName,
                });
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    Utility.LightMasterModKey2.FileName,
                    Utility.LightMasterModKey.FileName,
                    $"*{Utility.MasterModKey.FileName}",
                    $"{Utility.MasterModKey2.FileName}",
                    $"*{Utility.LightMasterModKey3.FileName}",
                    $"{Utility.LightMasterModKey4.FileName}",
                    $"*{Utility.PluginModKey.FileName}",
                    $"{Utility.PluginModKey2.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey3.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey4.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey2.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(8);
            results.Should().BeEquivalentTo(new LoadOrderListing[]
            {
                new LoadOrderListing(Utility.LightMasterModKey2, enabled: true),
                new LoadOrderListing(Utility.LightMasterModKey, enabled: true),
                new LoadOrderListing(Utility.MasterModKey, enabled: true),
                new LoadOrderListing(Utility.MasterModKey2, enabled: false),
                new LoadOrderListing(Utility.LightMasterModKey3, enabled: true),
                new LoadOrderListing(Utility.LightMasterModKey4, enabled: false),
                new LoadOrderListing(Utility.PluginModKey, enabled: true),
                new LoadOrderListing(Utility.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public async Task LiveLoadOrder()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(LiveLoadOrder)));
            var pluginPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataFolderPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(dataFolderPath);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Update.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Utility.PluginModKey.ToString(),
                });
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimLE, pluginPath, dataFolderPath, out var state, cccLoadOrderFilePath: cccPath);
            state.Subscribe(x =>
            {
                if (x.Failed) throw x.Exception ?? new Exception();
            });
            var list = live.AsObservableList();
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Update,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey,
                Utility.PluginModKey,
            });

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Utility.PluginModKey.ToString(),
                });
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey2.FileName), string.Empty);
            File.Delete(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName));
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey2,
                Utility.PluginModKey,
            });

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Skyrim.Constants.Dragonborn.ToString(),
                });
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Dawnguard,
                Skyrim.Constants.Dragonborn,
                Utility.LightMasterModKey2,
            });

            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Skyrim.Constants.Dragonborn.ToString(),
                    Utility.PluginModKey.ToString(),
                });
            File.Delete(Path.Combine(dataFolderPath, Utility.LightMasterModKey2.FileName));
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Dawnguard,
                Skyrim.Constants.Dragonborn,
                Utility.PluginModKey,
            });

            // Does not respect just data folder modification
            // Since LoadOrderListing doesn't specify whether data folder is present
            // Data folder is just used for Timestamp alignment for Oblivion
            File.Delete(Path.Combine(dataFolderPath, Skyrim.Constants.Dawnguard.FileName));
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Dawnguard,
                Skyrim.Constants.Dragonborn,
                Utility.PluginModKey,
            });
        }

        [Fact]
        public void OrderListings()
        {
            ModKey baseEsm = new ModKey("Base", ModType.Master);
            ModKey baseEsm2 = new ModKey("Base2", ModType.Master);
            ModKey ccEsm = new ModKey("CC", ModType.Master);
            ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
            ModKey ccEsl = new ModKey("CC", ModType.LightMaster);
            ModKey ccEsl2 = new ModKey("CC2", ModType.LightMaster);
            ModKey esm = new ModKey("Normal", ModType.Master);
            ModKey esm2 = new ModKey("Normal2", ModType.Master);
            ModKey esl = new ModKey("Normal", ModType.LightMaster);
            ModKey esl2 = new ModKey("Normal2", ModType.LightMaster);
            ModKey esp = new ModKey("Normal", ModType.Plugin);
            ModKey esp2 = new ModKey("Normal2", ModType.Plugin);

            LoadOrder.OrderListings(
                implicitListings: new ModKey[]
                {
                    baseEsm,
                    baseEsm2,
                },
                creationClubListings: new ModKey[]
                {
                    ccEsl,
                    ccEsl2,
                    ccEsm,
                    ccEsm2,
                },
                pluginsListings: new ModKey[]
                {
                    esp,
                    esp2,
                    esl,
                    esl2,
                    esm,
                    esm2,
                },
                selector: m => m)
                .Should().BeEquivalentTo(new ModKey[]
                {
                    baseEsm,
                    baseEsm2,
                    ccEsm,
                    ccEsm2,
                    ccEsl,
                    ccEsl2,
                    esm,
                    esm2,
                    esl,
                    esl2,
                    esp,
                    esp2,
                });
        }

        [Fact]
        public void OrderListings_EnsurePluginListedCCsDriveOrder()
        {
            ModKey ccEsm = new ModKey("CC", ModType.Master);
            ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
            ModKey ccEsm3 = new ModKey("CC3", ModType.Master);
            ModKey esm = new ModKey("Normal", ModType.Master);
            ModKey esm2 = new ModKey("Normal2", ModType.Master);

            LoadOrder.OrderListings(
                implicitListings: Array.Empty<ModKey>(),
                creationClubListings: new ModKey[]
                {
                    ccEsm,
                    ccEsm2,
                    ccEsm3,
                },
                pluginsListings: new ModKey[]
                {
                    ccEsm2,
                    esm,
                    ccEsm,
                    esm2,
                },
                selector: m => m)
                .Should().BeEquivalentTo(new ModKey[]
                {
                    // First, because wasn't listed on plugins
                    ccEsm3,
                    // 2nd because it was first on the plugins listings
                    ccEsm2,
                    // Was listed last on the plugins listing
                    ccEsm,
                    esm,
                    esm2,
                });
        }

        [Fact]
        public async Task LiveLoadOrder_EnsureReaddRetainsOrder()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(LiveLoadOrder_EnsureReaddRetainsOrder)));
            var pluginPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataFolderPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(dataFolderPath);
            File.WriteAllText(Path.Combine(dataFolderPath, Utility.LightMasterModKey.FileName), string.Empty);
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Update.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Utility.PluginModKey.ToString(),
                });
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.ToString(),
                    Utility.LightMasterModKey2.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimLE, pluginPath, dataFolderPath, out var state, cccLoadOrderFilePath: cccPath);
            var list = live.AsObservableList();
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Update,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey,
                Utility.PluginModKey,
            });

            // Remove
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Utility.PluginModKey.ToString(),
                });
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey,
                Utility.PluginModKey,
            });

            // Then readd
            File.WriteAllLines(pluginPath,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Update.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Utility.PluginModKey.ToString(),
                });
            await Task.Delay(1000);
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
            {
                Skyrim.Constants.Skyrim,
                Skyrim.Constants.Update,
                Skyrim.Constants.Dawnguard,
                Utility.LightMasterModKey,
                Utility.PluginModKey,
            });
        }

        // Vortex puts CC mods on the plugins file, as unactivated.  Seemingly to drive load order?
        // This ensures that is respected
        [Fact]
        public async Task LiveLoadOrder_PluginsCCListingReorders()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LoadOrder_Tests), nameof(LiveLoadOrder_PluginsCCListingReorders)));
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
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
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
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
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
            list.Items.Select(x => x.ModKey).Should().BeEquivalentTo(new ModKey[]
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
    }
}
