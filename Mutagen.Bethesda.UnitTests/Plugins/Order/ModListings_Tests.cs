using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Path = System.IO.Path;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class ModListings_Tests
    {
        private const string BaseFolder = "C:/BaseFolder";
        
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
