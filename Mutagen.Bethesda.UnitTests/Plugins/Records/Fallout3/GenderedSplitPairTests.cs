using Mutagen.Bethesda.Fallout3;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Shouldly;
using Xunit;
using static Mutagen.Bethesda.UnitTests.Plugins.Records.RawRecordBytes;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout3;

public class GenderedSplitPairTests
{
    private static readonly GameConstants Constants = GameConstants.Fallout3;

    [Fact]
    public void Race_RepeatedBodyDataBlock_ReadsBothHalves()
    {
        var bytes = Record(Constants, "RACE",
            ("EDID", Str("TestRace")),
            ("NAM1", Array.Empty<byte>()),
            ("MNAM", Array.Empty<byte>()),
            ("MODL", Str("male.nif")),
            ("HNAM", U32(0x901)),
            ("NAM1", Array.Empty<byte>()),
            ("FNAM", Array.Empty<byte>()),
            ("MODL", Str("female.nif")));
        var meta = Meta(Constants);
        var direct = Race.CreateFromBinary(new MutagenFrame(new MutagenMemoryReadStream(bytes, meta)));
        var overlay = RaceBinaryOverlay.RaceFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));

        foreach (var race in new IRaceGetter[] { direct, overlay })
        {
            race.BodyData.ShouldNotBeNull();
            race.BodyData.Male.ShouldNotBeNull();
            race.BodyData.Male.Model.ShouldNotBeNull();
            race.BodyData.Male.Model.File.ShouldBe("male.nif");
            race.BodyData.Female.ShouldNotBeNull();
            race.BodyData.Female.Model.ShouldNotBeNull();
            race.BodyData.Female.Model.File.ShouldBe("female.nif");
        }
    }
}
