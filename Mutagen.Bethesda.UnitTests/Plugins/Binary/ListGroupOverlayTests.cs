using System.Buffers.Binary;
using System.Text;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class ListGroupOverlayTests
{
    private static readonly int RecordHeaderLength = GameConstants.Fallout4.MajorConstants.HeaderLength;
    private static readonly int GroupHeaderLength = GameConstants.Fallout4.GroupConstants.HeaderLength;
    private const ushort FormVersion = 131;
    private static readonly ModKey ModKey = ModKey.FromFileName("ListGroupOverlay.esp");

    private static byte[] RecordHeader(string type, uint length, uint formId)
    {
        var header = new byte[RecordHeaderLength];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), length);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), formId);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), FormVersion);
        return header;
    }

    private static byte[] GroupHeader(uint length, string label, int groupType)
    {
        var header = new byte[GroupHeaderLength];
        Encoding.ASCII.GetBytes("GRUP").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), length);
        Encoding.ASCII.GetBytes(label).CopyTo(header, 8);
        BinaryPrimitives.WriteInt32LittleEndian(header.AsSpan(12), groupType);
        return header;
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static byte[] Record(string type, uint formId, params byte[][] subrecords)
    {
        var content = subrecords.SelectMany(x => x).ToArray();
        return RecordHeader(type, checked((uint)content.Length), formId).Concat(content).ToArray();
    }

    private static byte[] TopLevelGroup(string label, byte[] content)
    {
        return GroupHeader(checked((uint)(GroupHeaderLength + content.Length)), label, 0).Concat(content).ToArray();
    }

    private static byte[] PluginWithCellGroupHolding(byte[] entry) =>
    [
        .. Record("TES4", 0, Subrecord("HEDR", new byte[12])),
        .. TopLevelGroup("CELL", entry),
    ];

    private static IFallout4ModDisposableGetter Overlay(byte[] plugin) =>
        Fallout4Mod.CreateFromBinaryOverlay(new MemoryStream(plugin), Fallout4Release.Fallout4, ModKey);

    private static byte[] CellBlockGroupOfLength(uint length) =>
        GroupHeader(length, "\0\0\0\0", (int)GroupTypeEnum.InteriorCellBlock);

    [Fact]
    public void Overlay_ARecordWhereACellBlockBelongs_ThrowsNamingTheRecordType()
    {
        using var mod = Overlay(PluginWithCellGroupHolding(
            Record("MISC", 0x802, Subrecord("EDID", Encoding.ASCII.GetBytes("Stray\0")))));

        Should.Throw<RecordException>(() => mod.Cells.Count).Message.ShouldContain("MISC");
    }

    [Fact]
    public void Overlay_ACellBlockGroupShorterThanItsHeader_ThrowsAsMalformed()
    {
        using var mod = Overlay(PluginWithCellGroupHolding(CellBlockGroupOfLength(0)));

        Should.Throw<MalformedDataException>(() => mod.Cells.Count);
    }

    [Fact]
    public void Overlay_AnEmptyCellBlockGroup_ReadsAsOneBlock()
    {
        using var mod = Overlay(PluginWithCellGroupHolding(CellBlockGroupOfLength(checked((uint)GroupHeaderLength))));

        mod.Cells.Count.ShouldBe(1);
    }
}
