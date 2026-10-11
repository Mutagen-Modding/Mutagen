using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class GlobalUnknownTypeCharTests
{
    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildGlobBytes(byte? typeChar)
    {
        var content = (typeChar is { } c ? Subrecord("FNAM", [c]) : [])
            .Concat(Subrecord("FLTV", BitConverter.GetBytes(1f)))
            .ToArray();

        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("GLOB").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), 0x00000001);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), 131);

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

    private static IGlobalGetter ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return Global.CreateFromBinary(new MutagenFrame(reader), default);
    }

    private static IGlobalGetter ParseOverlay(byte[] bytes)
    {
        var package = new BinaryOverlayFactoryPackage(Meta());
        return GlobalBinaryOverlay.GlobalFactory(new OverlayStream(bytes, package), package, default);
    }

    private static byte[] Write(IGlobalGetter item)
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
            item.WriteToBinary(writer);
        }
        return stream.ToArray();
    }

    private static IEnumerable<byte[]> SubrecordsOf(byte[] record, RecordType type) =>
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
                GameConstants.Fallout4,
                type)
            .Select(subrecord => subrecord.Content.ToArray());

    private static void ShouldBeWrittenWithItsTypeCharAndValue(byte[] written, byte typeChar)
    {
        SubrecordsOf(written, RecordTypes.FNAM).ShouldHaveSingleItem().ShouldBe(new[] { typeChar });
        SubrecordsOf(written, RecordTypes.FLTV).ShouldHaveSingleItem().ShouldBe(BitConverter.GetBytes(1f));
    }

    private static void ShouldBeAnUnknownGlobalHoldingItsValue(IGlobalGetter global, byte typeChar)
    {
        global.ShouldBeAssignableTo<IGlobalUnknownGetter>()!.Data.ShouldBe(1f);
        global.TypeChar.ShouldBe((char)typeChar);
    }

    [Theory]
    [InlineData(0)]
    [InlineData((byte)'x')]
    public void DeepParse_UnrecognizedTypeChar_IsAnUnknownGlobalHoldingItsValue(byte typeChar)
    {
        ShouldBeAnUnknownGlobalHoldingItsValue(ParseDeep(BuildGlobBytes(typeChar)), typeChar);
    }

    [Theory]
    [InlineData(0)]
    [InlineData((byte)'x')]
    public void Overlay_UnrecognizedTypeChar_IsAnUnknownGlobalHoldingItsValue(byte typeChar)
    {
        ShouldBeAnUnknownGlobalHoldingItsValue(ParseOverlay(BuildGlobBytes(typeChar)), typeChar);
    }

    [Theory]
    [InlineData(0)]
    [InlineData((byte)'x')]
    public void DeepParse_UnrecognizedTypeChar_WritesItBack(byte typeChar)
    {
        var written = Write(ParseDeep(BuildGlobBytes(typeChar)));
        ShouldBeWrittenWithItsTypeCharAndValue(written, typeChar);
        ParseOverlay(written).TypeChar.ShouldBe((char)typeChar);
    }

    [Theory]
    [InlineData(0)]
    [InlineData((byte)'x')]
    public void Overlay_UnrecognizedTypeChar_WritesItBack(byte typeChar)
    {
        var written = Write(ParseOverlay(BuildGlobBytes(typeChar)));
        ShouldBeWrittenWithItsTypeCharAndValue(written, typeChar);
        ParseDeep(written).TypeChar.ShouldBe((char)typeChar);
    }

    [Fact]
    public void DeepParse_NoTypeChar_StaysAFloatWrittenWithoutOne()
    {
        var global = ParseDeep(BuildGlobBytes(null));
        global.ShouldBeAssignableTo<IGlobalFloatGetter>()!.Data.ShouldBe(1f);
        SubrecordsOf(Write(global), RecordTypes.FNAM).ShouldBeEmpty();
    }

    [Fact]
    public void Overlay_NoTypeChar_StaysAFloatWrittenWithoutOne()
    {
        var global = ParseOverlay(BuildGlobBytes(null));
        global.ShouldBeAssignableTo<IGlobalFloatGetter>()!.Data.ShouldBe(1f);
        SubrecordsOf(Write(global), RecordTypes.FNAM).ShouldBeEmpty();
    }
}
