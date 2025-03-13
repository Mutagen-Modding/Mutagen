using Shouldly;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class ModListingParserTests
{
    private ILoadOrderListingParser GetParser(bool enabledMarkers)
    {
        return new LoadOrderListingParser(
            new HasEnabledMarkersInjection(enabledMarkers));
    }

    private LoadOrderListing Get(ILoadOrderListingParser parser, ParseType type, string str)
    {
        LoadOrderListing? listing;
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
        var item = Get(parser, type, TestConstants.PluginModKey.FileName);
        Assert.False(item.Enabled);
        Assert.Equal(TestConstants.PluginModKey, item.ModKey);
        item = Get(parser, type, $"*{TestConstants.PluginModKey.FileName}");
        Assert.True(item.Enabled);
        Assert.False(item.Ghosted);
        Assert.Equal(TestConstants.PluginModKey, item.ModKey);
    }

    [Theory]
    [InlineData(ParseType.String)]
    [InlineData(ParseType.FileName)]
    [InlineData(ParseType.TryString)]
    [InlineData(ParseType.TryFilename)]
    public void GhostProcessing(ParseType type)
    {
        var parser = GetParser(true);
        var item = Get(parser, type, TestConstants.PluginModKey.FileName);
        Assert.False(item.Enabled);
        Assert.Equal(TestConstants.PluginModKey, item.ModKey);
        item = Get(parser, type, $"{TestConstants.PluginModKey.FileName}.ghost");
        Assert.False(item.Enabled);
        Assert.True(item.Ghosted);
        Assert.Equal(TestConstants.PluginModKey, item.ModKey);
    }

    [Theory]
    [InlineData(ParseType.String)]
    [InlineData(ParseType.FileName)]
    [InlineData(ParseType.TryString)]
    [InlineData(ParseType.TryFilename)]
    public void EnabledGhostProcessing(ParseType type)
    {
        var parser = GetParser(true);
        var item = Get(parser, type, TestConstants.PluginModKey.FileName);
        Assert.False(item.Enabled);
        Assert.Equal(TestConstants.PluginModKey, item.ModKey);
        item = Get(parser, type, $"*{TestConstants.PluginModKey.FileName}.ghost");
        Assert.False(item.Enabled);
        Assert.True(item.Ghosted);
        Assert.Equal(TestConstants.PluginModKey, item.ModKey);
    }

    #endregion

    // Just testing new C# records act as expected
    [Fact]
    public void LoadOrderListingTests()
    {
        var listing1 = new LoadOrderListing(TestConstants.PluginModKey, enabled: true);
        var listing1Eq = new LoadOrderListing
        {
            ModKey = TestConstants.PluginModKey,
            Enabled = true,
        };
        var listing1Disabled = new LoadOrderListing
        {
            ModKey = TestConstants.PluginModKey,
            Enabled = false,
        };
        var listing2 = new LoadOrderListing(TestConstants.PluginModKey2, enabled: true);
        var listing2Eq = new LoadOrderListing()
        {
            ModKey = TestConstants.PluginModKey2,
            Enabled = true
        };
        var listing2Disabled = new LoadOrderListing()
        {
            ModKey = TestConstants.PluginModKey2,
            Enabled = false
        };

        listing1.ShouldBe(listing1Eq);
        listing1.ShouldNotBe(listing1Disabled);
        listing1.ShouldNotBe(listing2);
        listing2.ShouldBe(listing2Eq);
        listing2.ShouldNotBe(listing2Disabled);
        listing2.ShouldNotBe(listing1);
    }
}