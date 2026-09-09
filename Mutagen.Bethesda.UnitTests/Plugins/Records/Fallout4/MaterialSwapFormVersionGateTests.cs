using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;
using Fallout4Constants = Mutagen.Bethesda.Fallout4.Constants;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

// Regression coverage for https://github.com/Mutagen-Modding/Mutagen/issues/687:
// MaterialSwap's FNAM form-version gate needs FormVersion while its custom
// struct-fill runs, on both the deep parser and the overlay.
public class MaterialSwapFormVersionGateTests
{
    private const ushort NewFormVersion = MaterialSwapBinaryCreateTranslation.NewFormVersion;
    private const ushort LegacyFormVersion = 90;

    private static readonly MasterReferenceCollection Masters = new(Fallout4Constants.Fallout4);

    private static byte[] BuildMswpBytes(ushort formVersion, string? topFnam)
    {
        var content = new List<byte>();
        if (topFnam != null)
        {
            content.AddRange(Subrecord("FNAM", NullTerminated(topFnam)));
        }
        content.AddRange(Subrecord("BNAM", NullTerminated("OriginalMat")));
        content.AddRange(Subrecord("SNAM", NullTerminated("ReplacementMat")));
        content.AddRange(Subrecord("FNAM", NullTerminated("")));
        var contentBytes = content.ToArray();

        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("MSWP").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)contentBytes.Length));
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), 0x00000001);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), formVersion);

        return header.Concat(contentBytes).ToArray();
    }

    private static byte[] Subrecord(string type, byte[] content)
    {
        var header = new byte[6];
        Encoding.ASCII.GetBytes(type).CopyTo(header, 0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(4), checked((ushort)content.Length));
        return header.Concat(content).ToArray();
    }

    private static byte[] NullTerminated(string s) => Encoding.ASCII.GetBytes(s + "\0");

    private static ParsingMeta Meta() => new(
        GameConstants.Fallout4,
        Fallout4Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(Masters));

    private static MaterialSwap ParseDeep(byte[] bytes)
    {
        using var reader = new MutagenBinaryReadStream(new MemoryStream(bytes), Meta());
        return MaterialSwap.CreateFromBinary(new MutagenFrame(reader));
    }

    private static IMaterialSwapGetter ParseOverlay(byte[] bytes)
    {
        return MaterialSwapBinaryOverlay.MaterialSwapFactory(
            bytes,
            new BinaryOverlayFactoryPackage(Meta()));
    }

    private static byte[] Write(IMaterialSwapGetter item)
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
            MaterialSwapBinaryWriteTranslation.Instance.Write(writer, item, default);
        }
        return stream.ToArray();
    }

    private static int CountFnam(byte[] record)
    {
        return RecordSpanExtensions.FindAllOfSubrecord(
            new ReadOnlyMemorySlice<byte>(record).Slice(GameConstants.Fallout4.MajorConstants.HeaderLength),
            GameConstants.Fallout4,
            RecordTypes.FNAM).Count;
    }

    [Fact]
    public void DeepParse_NewFormVersionDifferingFnam_ParsesTreeFolder()
    {
        var item = ParseDeep(BuildMswpBytes(NewFormVersion, topFnam: "materials"));
        item.TreeFolder.ShouldBe("materials");
    }

    [Fact]
    public void DeepParse_LegacyFormVersionDifferingFnam_Throws()
    {
        Should.Throw<MalformedDataException>(() => ParseDeep(BuildMswpBytes(LegacyFormVersion, topFnam: "materials")));
    }

    [Fact]
    public void Overlay_NewFormVersionDifferingFnam_ParsesTreeFolder()
    {
        var item = ParseOverlay(BuildMswpBytes(NewFormVersion, topFnam: "materials"));
        item.TreeFolder.ShouldBe("materials");
    }

    [Fact]
    public void Overlay_LegacyFormVersionDifferingFnam_Throws()
    {
        var item = ParseOverlay(BuildMswpBytes(LegacyFormVersion, topFnam: "materials"));
        Should.Throw<MalformedDataException>(() => item.TreeFolder);
    }

    [Fact]
    public void DeepParse_NewFormVersionNoTopFnam_TreeFolderIsNull()
    {
        var item = ParseDeep(BuildMswpBytes(NewFormVersion, topFnam: null));
        item.TreeFolder.ShouldBeNull();
    }

    [Fact]
    public void Overlay_NewFormVersionNoTopFnam_TreeFolderIsNull()
    {
        var item = ParseOverlay(BuildMswpBytes(NewFormVersion, topFnam: null));
        item.TreeFolder.ShouldBeNull();
    }

    [Fact]
    public void Overlay_NewFormVersionNoTopFnam_WritesNoFnam()
    {
        var written = Write(ParseOverlay(BuildMswpBytes(NewFormVersion, topFnam: null)));
        CountFnam(written).ShouldBe(0);
        ParseOverlay(written).TreeFolder.ShouldBeNull();
    }

    [Fact]
    public void Overlay_NewFormVersionTopFnam_WritesOneFnam()
    {
        var written = Write(ParseOverlay(BuildMswpBytes(NewFormVersion, topFnam: "materials")));
        CountFnam(written).ShouldBe(1);
        ParseOverlay(written).TreeFolder.ShouldBe("materials");
    }
}
