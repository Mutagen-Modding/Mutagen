using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout3;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;
using Fallout3Constants = Mutagen.Bethesda.Fallout3.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout3;

public class WaterOpacityDefaultTests
{
    private static readonly MasterReferenceCollection Masters = new(Fallout3Constants.Fallout3);

    private static byte[] BuildWatrBytesWithoutAnam()
    {
        var header = new byte[GameConstants.Fallout3.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("WATR").CopyTo(header, 0);
        return header;
    }

    [Fact]
    public void Overlay_WithoutAnam_OpacityIsTheDefault()
    {
        WaterBinaryOverlay.WaterFactory(
                BuildWatrBytesWithoutAnam(),
                new BinaryOverlayFactoryPackage(new ParsingMeta(
                    GameConstants.Fallout3,
                    Fallout3Constants.Fallout3,
                    SeparatedMasterPackage.NotSeparate(Masters))))
            .Opacity.ShouldBe(Water.OpacityDefault);
    }
}
