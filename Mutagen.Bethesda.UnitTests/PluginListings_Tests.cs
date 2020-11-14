using DynamicData;
using FluentAssertions;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class PluginListings_Tests
    {
        [Fact]
        public void AddImplicitMods()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(AddImplicitMods)));
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Skyrim.FileName), "TEST");
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Dawnguard.FileName), "TEST");
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Dragonborn.FileName), "TEST");
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Update.FileName), "TEST");
            var loadOrder = new List<LoadOrderListing>()
            {
                new LoadOrderListing(Utility.PluginModKey, true),
                new LoadOrderListing(Utility.Dawnguard, true),
                new LoadOrderListing(Utility.PluginModKey2, true),
            };
            PluginListings.AddImplicitMods(GameRelease.SkyrimSE, tmpFolder.Dir, loadOrder);
            Assert.Equal(6, loadOrder.Count);
            Assert.Equal(new LoadOrderListing(Utility.Skyrim, true), loadOrder[0]);
            Assert.Equal(new LoadOrderListing(Utility.Update, true), loadOrder[1]);
            Assert.Equal(new LoadOrderListing(Utility.Dragonborn, true), loadOrder[2]);
            Assert.Equal(new LoadOrderListing(Utility.PluginModKey, true), loadOrder[3]);
            Assert.Equal(new LoadOrderListing(Utility.Dawnguard, true), loadOrder[4]);
            Assert.Equal(new LoadOrderListing(Utility.PluginModKey2, true), loadOrder[5]);
        }

        [Fact]
        public void EnabledMarkerProcessing()
        {
            var item = LoadOrderListing.FromString(Utility.PluginModKey.FileName, enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = LoadOrderListing.FromString($"*{Utility.PluginModKey.FileName}", enabledMarkerProcessing: true);
            Assert.True(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        [Fact]
        public void WriteExclude()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(WriteExclude)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            PluginListings.Write(
                path,
                GameRelease.Oblivion,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(Utility.PluginModKey, false),
                    new LoadOrderListing(Utility.PluginModKey2, true),
                    new LoadOrderListing(Utility.PluginModKey3, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Single(lines);
            Assert.Equal(Utility.PluginModKey2.FileName, lines[0]);
        }

        [Fact]
        public void WriteMarkers()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(WriteMarkers)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            PluginListings.Write(
                path,
                GameRelease.SkyrimSE,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(Utility.PluginModKey, false),
                    new LoadOrderListing(Utility.PluginModKey2, true),
                    new LoadOrderListing(Utility.PluginModKey3, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal($"{Utility.PluginModKey.FileName}", lines[0]);
            Assert.Equal($"*{Utility.PluginModKey2.FileName}", lines[1]);
            Assert.Equal($"{Utility.PluginModKey3.FileName}", lines[2]);
        }

        [Fact]
        public void WriteImplicit()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(WriteImplicit)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            PluginListings.Write(
                path,
                GameRelease.SkyrimSE,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(Utility.Skyrim, true),
                    new LoadOrderListing(Utility.PluginModKey2, true),
                    new LoadOrderListing(Utility.Dawnguard, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(2, lines.Count);
            Assert.Equal($"*{Utility.PluginModKey2.FileName}", lines[0]);
            Assert.Equal($"{Utility.Dawnguard.FileName}", lines[1]);
        }

        [Fact]
        public async Task LiveLoadOrder()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(LiveLoadOrder)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Update.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                });
            var live = PluginListings.GetLiveLoadOrder(GameRelease.SkyrimLE, path, default, out var state);
            var list = live.AsObservableList();
            Assert.Equal(3, list.Count);
            Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
            Assert.Equal(Skyrim.Constants.Update, list.Items.ElementAt(1).ModKey);
            Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(2).ModKey);
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                });
            await Task.Delay(200);
            Assert.Equal(2, list.Count);
            Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
            Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(1).ModKey);
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                    Skyrim.Constants.Dragonborn.ToString(),
                });
            await Task.Delay(200);
            Assert.Equal(3, list.Count);
            Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
            Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(1).ModKey);
            Assert.Equal(Skyrim.Constants.Dragonborn, list.Items.ElementAt(2).ModKey);
        }

        [Fact]
        public void FromPathMissingWithImplicit()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(FromPathMissingWithImplicit)));
            using var file = File.Create(Path.Combine(tmp.Dir.Path, "Skyrim.esm"));
            var missingPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            PluginListings.ListingsFromPath(
                pluginTextPath: missingPath,
                game: GameRelease.SkyrimSE,
                dataPath: tmp.Dir.Path)
                .Should().Equal(new LoadOrderListing("Skyrim.esm", true));
        }

        [Fact]
        public void FromPathMissing()
        {
            using var tmp = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(PluginListings_Tests), nameof(FromPathMissing)));
            var missingPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            Action a = () =>
                PluginListings.ListingsFromPath(
                    pluginTextPath: missingPath,
                    game: GameRelease.Oblivion,
                    dataPath: default);
            a.Should().Throw<FileNotFoundException>();
        }
    }
}
