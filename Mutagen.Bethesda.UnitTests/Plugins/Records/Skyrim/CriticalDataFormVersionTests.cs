using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim;
using Xunit;
using SkyrimConstants = Mutagen.Bethesda.Skyrim.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class CriticalDataFormVersionTests
{
    private const ushort LegendaryEditionFormVersion = 43;
    private const uint EffectId = 0x00012345;

    private static readonly MasterReferenceCollection Masters = new(SkyrimConstants.Skyrim);

    private static byte[] WeaponWithCriticalDataWithoutUnused3()
    {
        var criticalData = new byte[16];
        BinaryPrimitives.WriteUInt32LittleEndian(criticalData.AsSpan(12), EffectId);
        var subrecordHeader = new byte[GameConstants.SkyrimLE.SubConstants.HeaderLength];
        Encoding.ASCII.GetBytes("CRDT").CopyTo(subrecordHeader, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(subrecordHeader.AsSpan(4), (ushort)criticalData.Length);
        var content = subrecordHeader.Concat(criticalData).ToArray();

        var header = new byte[GameConstants.SkyrimLE.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("WEAP").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), (uint)content.Length);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), LegendaryEditionFormVersion);
        return header.Concat(content).ToArray();
    }

    [Fact]
    public void Overlay_CriticalDataBeforeFormVersion44_HasNoUnused3()
    {
        var critical = WeaponBinaryOverlay.WeaponFactory(
                WeaponWithCriticalDataWithoutUnused3(),
                new BinaryOverlayFactoryPackage(new ParsingMeta(
                    GameConstants.SkyrimLE,
                    SkyrimConstants.Skyrim,
                    SeparatedMasterPackage.NotSeparate(Masters))))
            .Critical.ShouldNotBeNull();

        critical.Unused3.ShouldBe(0);
        critical.Effect.FormKey.ID.ShouldBe(EffectId);
    }
}
