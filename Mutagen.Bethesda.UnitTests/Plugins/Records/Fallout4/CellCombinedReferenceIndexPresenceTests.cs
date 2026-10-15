using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class CellCombinedReferenceIndexPresenceTests
{
    private const int EmptyCombinedReferenceIndexLength = 8;
    private const ushort FormVersion = 131;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildCellBytes(bool withCombinedReferenceIndex)
    {
        var content = new List<byte>();
        content.AddRange(Subrecord("DATA", [0x02, 0x00]));
        if (withCombinedReferenceIndex)
        {
            content.AddRange(Subrecord("XCRI", new byte[EmptyCombinedReferenceIndexLength]));
        }
        var contentBytes = content.ToArray();

        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("CELL").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)contentBytes.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), FormVersion);

        return header.Concat(contentBytes).ToArray();
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static ParsingMeta Meta() => new(
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static ICellGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return Cell.CreateFromBinary(new MutagenFrame(reader));
    }

    private static ICellGetter ParseOverlay(byte[] bytes)
    {
        return CellBinaryOverlay.CellFactory(bytes, new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(ICellGetter item)
    {
        var stream = new MemoryStream();
        using (var writer = new MutagenWriter(
                   stream,
                   new WritingBundle(GameConstants.Fallout4)
                   {
                       MasterReferences = Masters,
                       SeparatedMasterPackage = SeparatedMasterPackage.NotSeparate(Masters),
                   }))
        {
            CellBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static IReadOnlyList<int> CombinedReferenceIndexLengths(byte[] record) =>
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                GameConstants.Fallout4,
                RecordTypes.XCRI)
            .Select(x => x.ContentLength)
            .ToList();

    private static void ShouldHoldAnEmptyCombinedReferenceIndex(ICellGetter cell)
    {
        cell.CombinedMeshes.ShouldNotBeNull().ShouldBeEmpty();
        cell.CombinedMeshReferences.ShouldNotBeNull().ShouldBeEmpty();
    }

    private static void ShouldHoldNoCombinedReferenceIndex(ICellGetter cell)
    {
        cell.CombinedMeshes.ShouldBeNull();
        cell.CombinedMeshReferences.ShouldBeNull();
    }

    [Fact]
    public void DeepParse_EmptyCombinedReferenceIndex_IsWrittenBack_AndTheOverlayRereadsIt()
    {
        var written = Write(ParseDeep(BuildCellBytes(withCombinedReferenceIndex: true)));
        CombinedReferenceIndexLengths(written).ShouldBe([EmptyCombinedReferenceIndexLength]);
        ShouldHoldAnEmptyCombinedReferenceIndex(ParseOverlay(written));
    }

    [Fact]
    public void Overlay_EmptyCombinedReferenceIndex_IsWrittenBack_AndTheDeepParseRereadsIt()
    {
        var written = Write(ParseOverlay(BuildCellBytes(withCombinedReferenceIndex: true)));
        CombinedReferenceIndexLengths(written).ShouldBe([EmptyCombinedReferenceIndexLength]);
        ShouldHoldAnEmptyCombinedReferenceIndex(ParseDeep(written));
    }

    [Fact]
    public void DeepParse_NoCombinedReferenceIndex_WritesNone_AndTheOverlayRereadsNone()
    {
        var written = Write(ParseDeep(BuildCellBytes(withCombinedReferenceIndex: false)));
        CombinedReferenceIndexLengths(written).ShouldBeEmpty();
        ShouldHoldNoCombinedReferenceIndex(ParseOverlay(written));
    }

    [Fact]
    public void Overlay_NoCombinedReferenceIndex_WritesNone_AndTheDeepParseRereadsNone()
    {
        var written = Write(ParseOverlay(BuildCellBytes(withCombinedReferenceIndex: false)));
        CombinedReferenceIndexLengths(written).ShouldBeEmpty();
        ShouldHoldNoCombinedReferenceIndex(ParseDeep(written));
    }
}
