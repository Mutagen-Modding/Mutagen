using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class ModListings_Tests
    {
        [Fact]
        public void EnabledMarkerProcessing()
        {
            var item = ModListing.FromString(Utility.PluginModKey.FileName, enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = ModListing.FromString($"*{Utility.PluginModKey.FileName}", enabledMarkerProcessing: true);
            Assert.True(item.Enabled);
            Assert.False(item.Ghosted);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        [Fact]
        public void GhostProcessing()
        {
            var item = ModListing.FromString(Utility.PluginModKey.FileName, enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = ModListing.FromString($"{Utility.PluginModKey.FileName}.ghost", enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.True(item.Ghosted);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        [Fact]
        public void EnabledGhostProcessing()
        {
            var item = ModListing.FromString(Utility.PluginModKey.FileName, enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = ModListing.FromString($"*{Utility.PluginModKey.FileName}.ghost", enabledMarkerProcessing: true);
            Assert.False(item.Enabled);
            Assert.True(item.Ghosted);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        [Fact]
        public async Task LiveLoadOrder()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(ModListings_Tests));
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
            using var tmpFolder = Utility.GetTempFolder(nameof(ModListings_Tests));
            using var file = File.Create(Path.Combine(tmpFolder.Dir.Path, "Skyrim.esm"));
            var missingPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.GetListings(
                pluginsFilePath: missingPath,
                creationClubFilePath: null,
                game: GameRelease.SkyrimSE,
                dataPath: tmpFolder.Dir.Path)
                .Should().Equal(new ModListing("Skyrim.esm", true));
        }

        [Fact]
        public void FromPathMissing()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(ModListings_Tests));
            var missingPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            Action a = () =>
                PluginListings.ListingsFromPath(
                    pluginTextPath: missingPath,
                    game: GameRelease.Oblivion,
                    dataPath: default);
            a.Should().Throw<FileNotFoundException>();
        }

        // Just testing new C# records act as expected
        [Fact]
        public void LoadOrderListingTests()
        {
            var listing1 = new ModListing(Utility.PluginModKey, enabled: true);
            var listing1Eq = new ModListing
            {
                ModKey = Utility.PluginModKey,
                Enabled = true,
            };
            var listing1Disabled = new ModListing
            {
                ModKey = Utility.PluginModKey,
                Enabled = false,
            };
            var listing2 = new ModListing(Utility.PluginModKey2, enabled: true);
            var listing2Eq = new ModListing()
            {
                ModKey = Utility.PluginModKey2,
                Enabled = true
            };
            var listing2Disabled = new ModListing()
            {
                ModKey = Utility.PluginModKey2,
                Enabled = false
            };

            listing1.Should().BeEquivalentTo(listing1Eq);
            listing1.Should().NotBeEquivalentTo(listing1Disabled);
            listing1.Should().NotBeEquivalentTo(listing2);
            listing2.Should().BeEquivalentTo(listing2Eq);
            listing2.Should().NotBeEquivalentTo(listing2Disabled);
            listing2.Should().NotBeEquivalentTo(listing1);
        }
    }
}
