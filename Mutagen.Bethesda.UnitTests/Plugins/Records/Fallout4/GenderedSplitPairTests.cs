using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Shouldly;
using Xunit;
using static Mutagen.Bethesda.UnitTests.Plugins.Records.RawRecordBytes;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class GenderedSplitPairTests
{
    private static readonly GameConstants Constants = GameConstants.Fallout4;

    [Fact]
    public void ArmorAddon_InterleavedPairs_ReadsAllFourModels()
    {
        var bytes = Record(Constants, "ARMA",
            ("EDID", Str("TestArma")),
            ("DNAM", new byte[12]),
            ("MOD2", Str("male.nif")),
            ("MO2T", new byte[] { 1, 2, 3, 4 }),
            ("MOD4", Str("male1st.nif")),
            ("MO4T", new byte[] { 5, 6, 7, 8 }),
            ("MOD3", Str("female.nif")),
            ("MOD5", Str("female1st.nif")));
        var meta = Meta(Constants);
        var direct = ArmorAddon.CreateFromBinary(new MutagenFrame(new MutagenMemoryReadStream(bytes, meta)));
        var overlay = ArmorAddonBinaryOverlay.ArmorAddonFactory(new OverlayStream(bytes, meta), new BinaryOverlayFactoryPackage(meta));

        foreach (var arma in new IArmorAddonGetter[] { direct, overlay })
        {
            arma.WorldModel.ShouldNotBeNull();
            arma.WorldModel.Male.ShouldNotBeNull();
            arma.WorldModel.Male.File.ShouldBe("male.nif");
            arma.WorldModel.Female.ShouldNotBeNull();
            arma.WorldModel.Female.File.ShouldBe("female.nif");
            arma.FirstPersonModel.ShouldNotBeNull();
            arma.FirstPersonModel.Male.ShouldNotBeNull();
            arma.FirstPersonModel.Male.File.ShouldBe("male1st.nif");
            arma.FirstPersonModel.Female.ShouldNotBeNull();
            arma.FirstPersonModel.Female.File.ShouldBe("female1st.nif");
        }
    }
}
