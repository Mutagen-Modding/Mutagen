using Alphaleonis.Win32.Filesystem;
using DynamicData;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                (Utility.ModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
                (Utility.ModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
                (Utility.ModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
                (Utility.ModKey2, new DateTime(2020, 8, 8, 10, 9, 0)),
            };
            var results = LoadOrder.AlignToTimestamps(lo)
                .ToList();
            Assert.Equal(Utility.ModKey, results[0]);
            Assert.Equal(Utility.ModKey2, results[1]);
            Assert.Equal(Utility.ModKey3, results[2]);
            Assert.Equal(Utility.ModKey4, results[3]);
        }

        [Fact]
        public void AlignToTimestamps_SameTimestamps()
        {
            var lo = new List<(ModKey ModKey, DateTime Write)>
            {
                (Utility.ModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
                (Utility.ModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
                (Utility.ModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
                (Utility.ModKey2, new DateTime(2020, 8, 8, 10, 11, 0)),
            };
            var results = LoadOrder.AlignToTimestamps(lo)
                .ToList();
            Assert.Equal(Utility.ModKey, results[0]);
            Assert.Equal(Utility.ModKey3, results[1]);
            Assert.Equal(Utility.ModKey4, results[2]);
            Assert.Equal(Utility.ModKey2, results[3]);
        }

        [Fact]
        public void AddImplicitMods()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, "AddImplicitMods"));
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Skyrim.FileName), "TEST");
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Dawnguard.FileName), "TEST");
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Dragonborn.FileName), "TEST");
            File.WriteAllText(Path.Combine(tmpFolder.Dir.Path, Utility.Update.FileName), "TEST");
            var loadOrder = new List<LoadOrderListing>()
            {
                new LoadOrderListing(Utility.ModKey, true),
                new LoadOrderListing(Utility.Dawnguard, true),
                new LoadOrderListing(Utility.ModKey2, true),
            };
            LoadOrder.AddImplicitMods(GameRelease.SkyrimSE, tmpFolder.Dir, loadOrder);
            Assert.Equal(6, loadOrder.Count);
            Assert.Equal(new LoadOrderListing(Utility.Skyrim, true), loadOrder[0]);
            Assert.Equal(new LoadOrderListing(Utility.Update, true), loadOrder[1]);
            Assert.Equal(new LoadOrderListing(Utility.Dragonborn, true), loadOrder[2]);
            Assert.Equal(new LoadOrderListing(Utility.ModKey, true), loadOrder[3]);
            Assert.Equal(new LoadOrderListing(Utility.Dawnguard, true), loadOrder[4]);
            Assert.Equal(new LoadOrderListing(Utility.ModKey2, true), loadOrder[5]);
        }

        [Fact]
        public void EnabledMarkerProcessing()
        {
            var item = LoadOrder.FromString(Utility.ModKey.FileName, enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.ModKey, item.ModKey);
            item = LoadOrder.FromString($"*{Utility.ModKey.FileName}", enabledMarkerProcessing: true);
            Assert.True(item.Enabled);
            Assert.Equal(Utility.ModKey, item.ModKey);
        }

        [Fact]
        public void WriteExclude()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(WriteExclude)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.Oblivion,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(Utility.ModKey, false),
                    new LoadOrderListing(Utility.ModKey2, true),
                    new LoadOrderListing(Utility.ModKey3, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Single(lines);
            Assert.Equal(Utility.ModKey2.FileName, lines[0]);
        }

        [Fact]
        public void WriteMarkers()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(WriteMarkers)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.SkyrimSE,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(Utility.ModKey, false),
                    new LoadOrderListing(Utility.ModKey2, true),
                    new LoadOrderListing(Utility.ModKey3, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal($"{Utility.ModKey.FileName}", lines[0]);
            Assert.Equal($"*{Utility.ModKey2.FileName}", lines[1]);
            Assert.Equal($"{Utility.ModKey3.FileName}", lines[2]);
        }

        [Fact]
        public void WriteImplicit()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(WriteImplicit)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.SkyrimSE,
                new LoadOrderListing[]
                {
                    new LoadOrderListing(Utility.Skyrim, true),
                    new LoadOrderListing(Utility.ModKey2, true),
                    new LoadOrderListing(Utility.Dawnguard, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(2, lines.Count);
            Assert.Equal($"*{Utility.ModKey2.FileName}", lines[0]);
            Assert.Equal($"{Utility.Dawnguard.FileName}", lines[1]);
        }

        [Fact]
        public async Task LiveLoadOrder()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(LiveLoadOrder)));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Update.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                });
            var live = LoadOrder.GetLiveLoadOrder(GameRelease.SkyrimLE, path, default, out var state);
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
    }
}
