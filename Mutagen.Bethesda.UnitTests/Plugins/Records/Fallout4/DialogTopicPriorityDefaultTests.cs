using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class DialogTopicPriorityDefaultTests
{
    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildDialBytesWithoutPnam()
    {
        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("DIAL").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), 0x00000001);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), 131);
        return header;
    }

    private static ParsingMeta Meta() => new(
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(Masters));

    [Fact]
    public void Overlay_WithoutPnam_PriorityIsTheDefault()
    {
        DialogTopicBinaryOverlay.DialogTopicFactory(BuildDialBytesWithoutPnam(), new BinaryOverlayFactoryPackage(Meta()))
            .Priority.ShouldBe(DialogTopic.PriorityDefault);
    }
}
