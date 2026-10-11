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

public class PlacedObjectAllZeroRoomDataTests
{
    private const int RoomDataLength = 4;
    private const ushort FormVersion = 131;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildRefrBytes(bool withRoomData)
    {
        var content = new List<byte>();
        content.AddRange(Subrecord("NAME", BitConverter.GetBytes(0x0000001Fu)));
        if (withRoomData)
        {
            content.AddRange(Subrecord("XRMR", new byte[RoomDataLength]));
        }
        content.AddRange(Subrecord("DATA", new byte[24]));
        var contentBytes = content.ToArray();

        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("REFR").CopyTo(header, 0);
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

    private static IPlacedObjectGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return PlacedObject.CreateFromBinary(new MutagenFrame(reader));
    }

    private static IPlacedObjectGetter ParseOverlay(byte[] bytes)
    {
        return PlacedObjectBinaryOverlay.PlacedObjectFactory(bytes, new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(IPlacedObjectGetter item)
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
            PlacedObjectBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static IReadOnlyList<int> RoomDataLengths(byte[] record) =>
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                GameConstants.Fallout4,
                RecordTypes.XRMR)
            .Select(x => x.ContentLength)
            .ToList();

    [Fact]
    public void DeepParse_AllZeroRoomData_IsWrittenBack_AndTheOverlayRereadsIt()
    {
        var written = Write(ParseDeep(BuildRefrBytes(withRoomData: true)));
        RoomDataLengths(written).ShouldBe([RoomDataLength]);
        ParseOverlay(written).Unknown.ShouldBe((short)0);
    }

    [Fact]
    public void Overlay_AllZeroRoomData_IsWrittenBack_AndTheDeepParseRereadsIt()
    {
        var written = Write(ParseOverlay(BuildRefrBytes(withRoomData: true)));
        RoomDataLengths(written).ShouldBe([RoomDataLength]);
        ParseDeep(written).Unknown.ShouldBe((short)0);
    }

    [Fact]
    public void DeepParse_NoRoomData_WritesNone_AndTheOverlayRereadsNone()
    {
        var written = Write(ParseDeep(BuildRefrBytes(withRoomData: false)));
        RoomDataLengths(written).ShouldBeEmpty();
        ParseOverlay(written).Unknown.ShouldBeNull();
    }

    [Fact]
    public void Overlay_NoRoomData_WritesNone_AndTheDeepParseRereadsNone()
    {
        var written = Write(ParseOverlay(BuildRefrBytes(withRoomData: false)));
        RoomDataLengths(written).ShouldBeEmpty();
        ParseDeep(written).Unknown.ShouldBeNull();
    }
}
