using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Path = System.IO.Path;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class ModListingParserTests
    {
        private IModListingParser GetParser(bool enabledMarkers)
        {
            return new ModListingParser(
                new HasEnabledMarkersInjector(enabledMarkers));
        }

        private ModListing Get(IModListingParser parser, ParseType type, string str)
        {
            ModListing? listing;
            switch (type)
            {
                case ParseType.String:
                    return parser.FromString(str);
                case ParseType.FileName:
                    return parser.FromFileName(str);
                case ParseType.TryFilename:
                    if (parser.TryFromFileName(str, out listing))
                    {
                        return listing;
                    }
                    throw new Exception();
                case ParseType.TryString:
                    if (parser.TryFromString(str, out listing))
                    {
                        return listing;
                    }
                    throw new Exception();
                default:
                    throw new Exception();
            }
        }

        public enum ParseType
        {
            String,
            FileName,
            TryString,
            TryFilename
        }
        
        #region Enabled

        [Theory]
        [InlineData(ParseType.String)]
        [InlineData(ParseType.FileName)]
        [InlineData(ParseType.TryString)]
        [InlineData(ParseType.TryFilename)]
        public void EnabledMarkerProcessing(ParseType type)
        {
            var parser = GetParser(true);
            var item = Get(parser, type, Utility.PluginModKey.FileName);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = Get(parser, type, $"*{Utility.PluginModKey.FileName}");
            Assert.True(item.Enabled);
            Assert.False(item.Ghosted);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        [Theory]
        [InlineData(ParseType.String)]
        [InlineData(ParseType.FileName)]
        [InlineData(ParseType.TryString)]
        [InlineData(ParseType.TryFilename)]
        public void GhostProcessing(ParseType type)
        {
            var parser = GetParser(true);
            var item = Get(parser, type, Utility.PluginModKey.FileName);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = Get(parser, type, $"{Utility.PluginModKey.FileName}.ghost");
            Assert.False(item.Enabled);
            Assert.True(item.Ghosted);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        [Theory]
        [InlineData(ParseType.String)]
        [InlineData(ParseType.FileName)]
        [InlineData(ParseType.TryString)]
        [InlineData(ParseType.TryFilename)]
        public void EnabledGhostProcessing(ParseType type)
        {
            var parser = GetParser(true);
            var item = Get(parser, type, Utility.PluginModKey.FileName);
            Assert.False(item.Enabled);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
            item = Get(parser, type, $"*{Utility.PluginModKey.FileName}.ghost");
            Assert.False(item.Enabled);
            Assert.True(item.Ghosted);
            Assert.Equal(Utility.PluginModKey, item.ModKey);
        }

        #endregion

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

            listing1.Should().Be(listing1Eq);
            listing1.Should().NotBe(listing1Disabled);
            listing1.Should().NotBe(listing2);
            listing2.Should().Be(listing2Eq);
            listing2.Should().NotBe(listing2Disabled);
            listing2.Should().NotBe(listing1);
        }
    }
}
