using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Testing;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Analysis;

public class RecordTypeLocatorTests
{
    [Fact]
    public void TypicalSkyrim()
    {
        var stream = TestDataPathing.GetReadFrame("Plugins/Analysis/SkyrimTypical.esp", GameRelease.SkyrimSE);
        var locs = RecordLocator.GetLocations(stream);
        locs.FormKeys.Should().Equal(
            FormKey.Factory("034182:SkyrimTypical.esp"),
            FormKey.Factory("039850:SkyrimTypical.esp"),
            FormKey.Factory("0C6974:SkyrimTypical.esp"),
            FormKey.Factory("039861:SkyrimTypical.esp"),
            FormKey.Factory("0580A2:SkyrimTypical.esp"),
            FormKey.Factory("0204C7:SkyrimTypical.esp"),
            FormKey.Factory("0243E0:SkyrimTypical.esp"),
            FormKey.Factory("0C27CA:SkyrimTypical.esp"),
            FormKey.Factory("0204F2:SkyrimTypical.esp"),
            FormKey.Factory("020532:SkyrimTypical.esp"),
            FormKey.Factory("000E42:SkyrimTypical.esp"),
            FormKey.Factory("000E45:SkyrimTypical.esp"));
        locs.GrupLocations.Keys.Should().Equal(
            0x50,
            0x16D,
            0x185,
            0x19D,
            0x249,
            0x261,
            0x2C3,
            0x31B,
            0x3CA,
            0x4D5,
            0x533,
            0x54B,
            0x5A3,
            0x5BB,
            0x62A,
            0x642,
            0x688,
            0x6EA);
        locs.GrupLocations.Values.Should().Equal(
            new GroupLocationMarker(new RangeInt64(80, 364), new("AMMO"), 0),
            new GroupLocationMarker(new RangeInt64(365, 969), new("CELL"), 0),
            new GroupLocationMarker(new RangeInt64(389, 969), new(0), 2),
            new GroupLocationMarker(new RangeInt64(413, 794), new(0), 3),
            new GroupLocationMarker(new RangeInt64(585, 794), new(0x39850), 6),
            new GroupLocationMarker(new RangeInt64(609, 706), new(0x39850), 8),
            new GroupLocationMarker(new RangeInt64(707, 794), new(0x39850), 9),
            new GroupLocationMarker(new RangeInt64(795, 969), new(0x1), 3),
            new GroupLocationMarker(new RangeInt64(970, 1671), new("WRLD"), 0),
            new GroupLocationMarker(new RangeInt64(1237, 1671), new(0x204C7), 1),
            new GroupLocationMarker(new RangeInt64(1331, 1442), new(0x243E0), 6),
            new GroupLocationMarker(new RangeInt64(1355, 1442), new(0x243E0), 8),
            new GroupLocationMarker(new RangeInt64(1443, 1671), new(0x1FFFF), 4),
            new GroupLocationMarker(new RangeInt64(1467, 1671), new(0x5FFFC), 5),
            new GroupLocationMarker(new RangeInt64(1578, 1671), new(0x204F2), 6),
            new GroupLocationMarker(new RangeInt64(1602, 1671), new(0x204F2), 9),
            new GroupLocationMarker(new RangeInt64(1672, 1924), new("DIAL"), 0),
            new GroupLocationMarker(new RangeInt64(1770, 1924), new(0xE42), 7));
        locs.ListedRecords.Keys.Should().Equal(
            0x68,
            0x1B5,
            0x279,
            0x2DB,
            0x333,
            0x3E2,
            0x4ED,
            0x563,
            0x5D3,
            0x65A,
            0x6A0,
            0x702);
        locs.ListedRecords.Values.Should().Equal(
            new RecordLocationMarker(
                FormKey.Factory("034182:SkyrimTypical.esp"), 
                new RangeInt64(104, 364),
                new RecordType("AMMO")),
            new RecordLocationMarker(
                FormKey.Factory("039850:SkyrimTypical.esp"), 
                new RangeInt64(437, 584),
                new RecordType("CELL")),
            new RecordLocationMarker(
                FormKey.Factory("0C6974:SkyrimTypical.esp"), 
                new RangeInt64(633, 706),
                new RecordType("REFR")),
            new RecordLocationMarker(
                FormKey.Factory("039861:SkyrimTypical.esp"), 
                new RangeInt64(731, 794),
                new RecordType("REFR")),
            new RecordLocationMarker(
                FormKey.Factory("0580A2:SkyrimTypical.esp"), 
                new RangeInt64(819, 969),
                new RecordType("CELL")),
            new RecordLocationMarker(
                FormKey.Factory("0204C7:SkyrimTypical.esp"), 
                new RangeInt64(994, 1236),
                new RecordType("WRLD")),
            new RecordLocationMarker(
                FormKey.Factory("0243E0:SkyrimTypical.esp"), 
                new RangeInt64(1261, 1330),
                new RecordType("CELL")),
            new RecordLocationMarker(
                FormKey.Factory("0C27CA:SkyrimTypical.esp"), 
                new RangeInt64(1379, 1442),
                new RecordType("REFR")),
            new RecordLocationMarker(
                FormKey.Factory("0204F2:SkyrimTypical.esp"), 
                new RangeInt64(1491, 1577),
                new RecordType("CELL")),
            new RecordLocationMarker(
                FormKey.Factory("020532:SkyrimTypical.esp"), 
                new RangeInt64(1626, 1671),
                new RecordType("LAND")),
            new RecordLocationMarker(
                FormKey.Factory("000E42:SkyrimTypical.esp"), 
                new RangeInt64(1696, 1769),
                new RecordType("DIAL")),
            new RecordLocationMarker(
                FormKey.Factory("000E45:SkyrimTypical.esp"), 
                new RangeInt64(1794, 1924),
                new RecordType("INFO")));
    }

    [Fact]
    public void TypicalInterest()
    {
        var stream = TestDataPathing.GetReadFrame("Plugins/Analysis/SkyrimTypical.esp", GameRelease.SkyrimSE);
        var locs = RecordLocator.GetLocations(stream,
            new RecordInterest("AMMO"));
        locs.FormKeys.Should().Equal(
            FormKey.Factory("034182:SkyrimTypical.esp"));
        locs.GrupLocations.Keys.Should().Equal(
            0x50);
        locs.GrupLocations.Values.Should().Equal(
            new GroupLocationMarker(new RangeInt64(80, 364), new("AMMO"), 0));
        locs.ListedRecords.Keys.Should().Equal(
            0x68);
        locs.ListedRecords.Values.Should().Equal(
            new RecordLocationMarker(
                FormKey.Factory("034182:SkyrimTypical.esp"),
                new RangeInt64(104, 364),
                new RecordType("AMMO")));
    }

    [Fact]
    public void DeepInterest()
    {
        var stream = TestDataPathing.GetReadFrame("Plugins/Analysis/SkyrimTypical.esp", GameRelease.SkyrimSE);
        var locs = RecordLocator.GetLocations(stream,
            new RecordInterest("INFO"));
        locs.FormKeys.Should().Equal(
            FormKey.Factory("000E45:SkyrimTypical.esp"));
        locs.GrupLocations.Keys.Should().Equal(
            0x688,
            0x6EA);
        locs.GrupLocations.Values.Should().Equal(
            new GroupLocationMarker(new RangeInt64(1672, 1924), new("DIAL"), 0),
            new GroupLocationMarker(new RangeInt64(1770, 1924), new(0xE42), 7));
        locs.ListedRecords.Keys.Should().Equal(
            0x702);
        locs.ListedRecords.Values.Should().Equal(
            new RecordLocationMarker(
                FormKey.Factory("000E45:SkyrimTypical.esp"),
                new RangeInt64(1794, 1924),
                new RecordType("INFO")));
    }
}