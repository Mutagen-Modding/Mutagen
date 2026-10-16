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

public class CountedListShortCountTests
{
    private const int EntriesListed = 2;
    private const uint CountedTooFew = 1;
    private const int ContainerEntryLength = 8;
    private const ushort FormVersion = 131;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildRefrBytesWithConnections(uint? count)
    {
        var content = new List<byte>();
        content.AddRange(Subrecord("NAME", BitConverter.GetBytes(0x0000001Fu)));
        if (count != null)
        {
            content.AddRange(Subrecord("XWPG", BitConverter.GetBytes(count.Value)));
        }
        for (uint i = 1; i <= EntriesListed; i++)
        {
            var connection = new byte[12];
            BinaryPrimitives.WriteUInt32LittleEndian(connection, 0x800 + i);
            content.AddRange(Subrecord("XWPN", connection));
        }
        content.AddRange(Subrecord("DATA", new byte[24]));
        return Record("REFR", content);
    }

    private static byte[] BuildRefrBytesCountingFewerConnectionsThanFollow() => BuildRefrBytesWithConnections(CountedTooFew);

    private static byte[] BuildContainerBytesCountingFewerItemsThanFollow()
    {
        var content = new List<byte>();
        content.AddRange(Subrecord("COCT", BitConverter.GetBytes(CountedTooFew)));
        for (var i = 0; i < EntriesListed; i++)
        {
            content.AddRange(Subrecord("CNTO", new byte[ContainerEntryLength]));
        }
        return Record("CONT", content);
    }

    private static byte[] Record(string type, List<byte> content)
    {
        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Count));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), FormVersion);
        return header.Concat(content).ToArray();
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

    private static uint WrittenCount(byte[] record) =>
        BinaryPrimitives.ReadUInt32LittleEndian(
            RecordSpanExtensions.FindAllOfSubrecord(
                    new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                    GameConstants.Fallout4,
                    RecordTypes.XWPG)
                .Single().Content);

    [Fact]
    public void DeepParse_ACountLowerThanTheItemsThatFollow_ReadsEveryItem()
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(BuildContainerBytesCountingFewerItemsThanFollow()), Meta());
        Container.CreateFromBinary(new MutagenFrame(reader)).Items.ShouldNotBeNull().Count.ShouldBe(EntriesListed);
    }

    [Fact]
    public void Overlay_ACountLowerThanTheItemsThatFollow_ReadsEveryItem()
    {
        ContainerBinaryOverlay.ContainerFactory(
                BuildContainerBytesCountingFewerItemsThanFollow(), new BinaryOverlayFactoryPackage(Meta()))
            .Items.ShouldNotBeNull().Count.ShouldBe(EntriesListed);
    }

    [Fact]
    public void DeepParse_ACountLowerThanTheConnectionsThatFollow_ReadsEveryConnection()
    {
        ParseDeep(BuildRefrBytesCountingFewerConnectionsThanFollow()).PowerGridConnections.Count.ShouldBe(EntriesListed);
    }

    [Fact]
    public void DeepParse_ConnectionsWithNoCount_ReadsEveryConnection()
    {
        ParseDeep(BuildRefrBytesWithConnections(count: null)).PowerGridConnections.Count.ShouldBe(EntriesListed);
    }

    [Fact]
    public void Overlay_ACountLowerThanTheConnectionsThatFollow_ReadsEveryConnection()
    {
        ParseOverlay(BuildRefrBytesCountingFewerConnectionsThanFollow()).PowerGridConnections.Count.ShouldBe(EntriesListed);
    }

    [Fact]
    public void DeepParse_WritesTheCountOfTheConnections_AndTheOverlayRereadsThem()
    {
        var written = Write(ParseDeep(BuildRefrBytesCountingFewerConnectionsThanFollow()));
        WrittenCount(written).ShouldBe((uint)EntriesListed);
        ParseOverlay(written).PowerGridConnections.Count.ShouldBe(EntriesListed);
    }

    [Fact]
    public void Overlay_WritesTheCountOfTheConnections_AndTheDeepParseRereadsThem()
    {
        var written = Write(ParseOverlay(BuildRefrBytesCountingFewerConnectionsThanFollow()));
        WrittenCount(written).ShouldBe((uint)EntriesListed);
        ParseDeep(written).PowerGridConnections.Count.ShouldBe(EntriesListed);
    }
}
