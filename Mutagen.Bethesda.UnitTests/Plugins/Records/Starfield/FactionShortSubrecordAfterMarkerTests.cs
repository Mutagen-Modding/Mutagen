using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Starfield;
using Xunit;
using StarfieldConstants = Mutagen.Bethesda.Starfield.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Starfield;

public class FactionShortSubrecordAfterMarkerTests
{
    private const string EditorId = "ShortFormationRadius";

    private static readonly MasterReferenceCollection Masters = new(StarfieldConstants.Starfield);

    private static byte[] FactionEndingInATwoByteFormationRadius()
    {
        var content = Subrecord("EDID", Encoding.ASCII.GetBytes(EditorId + "\0"))
            .Concat(Subrecord("CRGP", []))
            .Concat(Subrecord("GRPH", new byte[2]))
            .ToArray();
        var header = new byte[GameConstants.Starfield.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("FACT").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        return header.Concat(content).ToArray();
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[GameConstants.Starfield.SubConstants.HeaderLength];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static ParsingMeta Meta() => new(
        GameConstants.Starfield,
        StarfieldConstants.Starfield,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static void ShouldReadTheFactionWithoutItsFormationRadius(IFactionGetter faction)
    {
        faction.EditorID.ShouldBe(EditorId);
        faction.FormationRadius.ShouldBeNull();
    }

    [Fact]
    public void DeepParse_AFormationRadiusShorterThanAFloat_ReadsAsAbsent()
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(FactionEndingInATwoByteFormationRadius()), Meta());
        ShouldReadTheFactionWithoutItsFormationRadius(Faction.CreateFromBinary(new MutagenFrame(reader)));
    }

    [Fact]
    public void Overlay_AFormationRadiusShorterThanAFloat_ReadsAsAbsent()
    {
        ShouldReadTheFactionWithoutItsFormationRadius(
            FactionBinaryOverlay.FactionFactory(FactionEndingInATwoByteFormationRadius(), new BinaryOverlayFactoryPackage(Meta())));
    }
}
