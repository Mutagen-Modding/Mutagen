using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Analysis;
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
            FormKey.Factory("0204C7:SkyrimTypical.esp"),
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
            0x426,
            0x43E,
            0x456,
            0x4C5,
            0x4DD,
            0x523,
            0x585);
        locs.GrupLocations.Values.Should().Equal(
            new GroupLocationMarker(new RangeInt64(80, 364), new("AMMO"), 0),
            new GroupLocationMarker(new RangeInt64(365, 794), new("CELL"), 0),
            new GroupLocationMarker(new RangeInt64(389, 794), new(0), 2),
            new GroupLocationMarker(new RangeInt64(413, 794), new(0), 3),
            new GroupLocationMarker(new RangeInt64(585, 794), new(0x39850), 6),
            new GroupLocationMarker(new RangeInt64(609, 706), new(0x39850), 8),
            new GroupLocationMarker(new RangeInt64(707, 794), new(0x39850), 9),
            new GroupLocationMarker(new RangeInt64(795, 1314), new("WRLD"), 0),
            new GroupLocationMarker(new RangeInt64(1062, 1314), new(0x204C7), 1),
            new GroupLocationMarker(new RangeInt64(1086, 1314), new(0x1FFFF), 4),
            new GroupLocationMarker(new RangeInt64(1110, 1314), new(0x5FFFC), 5),
            new GroupLocationMarker(new RangeInt64(1221, 1314), new(0x204F2), 6),
            new GroupLocationMarker(new RangeInt64(1245, 1314), new(0x204F2), 9),
            new GroupLocationMarker(new RangeInt64(1315, 1567), new("DIAL"), 0),
            new GroupLocationMarker(new RangeInt64(1413, 1567), new(0xE42), 7));
        locs.ListedRecords.Keys.Should().Equal(
            0x68,
            0x1B5,
            0x279,
            0x2DB,
            0x333,
            0x46E,
            0x4F5,
            0x53B,
            0x59D);
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
                FormKey.Factory("0204C7:SkyrimTypical.esp"), 
                new RangeInt64(819, 1061),
                new RecordType("WRLD")),
            new RecordLocationMarker(
                FormKey.Factory("0204F2:SkyrimTypical.esp"), 
                new RangeInt64(1134, 1220),
                new RecordType("CELL")),
            new RecordLocationMarker(
                FormKey.Factory("020532:SkyrimTypical.esp"), 
                new RangeInt64(1269, 1314),
                new RecordType("LAND")),
            new RecordLocationMarker(
                FormKey.Factory("000E42:SkyrimTypical.esp"), 
                new RangeInt64(1339, 1412),
                new RecordType("DIAL")),
            new RecordLocationMarker(
                FormKey.Factory("000E45:SkyrimTypical.esp"), 
                new RangeInt64(1437, 1567),
                new RecordType("INFO")));
    }
}