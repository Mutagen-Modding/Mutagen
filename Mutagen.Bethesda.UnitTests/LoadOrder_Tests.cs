using DynamicData;
using FluentAssertions;
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
                new LoadOrderListing(Utility.MasterModKey, enabled: true),
                new LoadOrderListing(Utility.MasterModKey2, enabled: false),
                new LoadOrderListing(Utility.LightMasterModKey, enabled: true),
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
        }
    }
}
