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
        return Mswp(formVersion, content);
    }

    private static byte[] BuildTwoSubstitutionMswpBytes()
    {
        var content = new List<byte>();
        content.AddRange(Subrecord("FNAM", NullTerminated("materials")));
        content.AddRange(Subrecord("BNAM", NullTerminated("FirstOriginal")));
        content.AddRange(Subrecord("SNAM", NullTerminated("FirstReplacement")));
        content.AddRange(Subrecord("FNAM", NullTerminated("")));
        content.AddRange(Subrecord("CNAM", BitConverter.GetBytes(1f)));
        content.AddRange(Subrecord("BNAM", NullTerminated("SecondOriginal")));
        content.AddRange(Subrecord("SNAM", NullTerminated("SecondReplacement")));
        content.AddRange(Subrecord("CNAM", BitConverter.GetBytes(2f)));
        return Mswp(NewFormVersion, content);
    }

    private static byte[] Mswp(ushort formVersion, List<byte> content)
    {
        var header = new byte[GameConstants.Fallout4.MajorConstants.HeaderLength];
        Encoding.ASCII.GetBytes("MSWP").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)content.Count));
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), 0x00000001);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), formVersion);
        return header.Concat(content).ToArray();
    }

    private static void ShouldHoldBothSubstitutions(IMaterialSwapGetter item)
    {
        item.Substitutions.Count.ShouldBe(2);
        item.Substitutions[0].ObsoleteTreeFolder.ShouldBe(string.Empty);
        item.Substitutions[0].ColorRemappingIndex.ShouldBe(1f);
        item.Substitutions[1].OriginalMaterial.ShouldBe("SecondOriginal");
        item.Substitutions[1].ObsoleteTreeFolder.ShouldBeNull();
        item.Substitutions[1].ColorRemappingIndex.ShouldBe(2f);
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
    public void Overlay_NewFormVersionNoTopFnam_WritesOnlyTheSubstitutionsFnam()
    {
        var written = Write(ParseOverlay(BuildMswpBytes(NewFormVersion, topFnam: null)));
        CountFnam(written).ShouldBe(1);
        ParseOverlay(written).TreeFolder.ShouldBeNull();
    }

    [Fact]
    public void Overlay_NewFormVersionTopFnam_WritesBothFnams()
    {
        var written = Write(ParseOverlay(BuildMswpBytes(NewFormVersion, topFnam: "materials")));
        CountFnam(written).ShouldBe(2);
        ParseOverlay(written).TreeFolder.ShouldBe("materials");
    }

    [Fact]
    public void DeepParse_NewFormVersionTwoSubstitutions_KeepsEachFnamAndCnamAcrossAWrite()
    {
        var item = ParseDeep(BuildTwoSubstitutionMswpBytes());
        ShouldHoldBothSubstitutions(item);
        ShouldHoldBothSubstitutions(ParseOverlay(Write(item)));
    }

    [Fact]
    public void Overlay_NewFormVersionTwoSubstitutions_KeepsEachFnamAndCnamAcrossAWrite()
    {
        var item = ParseOverlay(BuildTwoSubstitutionMswpBytes());
        ShouldHoldBothSubstitutions(item);
        ShouldHoldBothSubstitutions(ParseDeep(Write(item)));
    }

    [Fact]
    public void DeepParse_LegacyFormVersionSubstitutionFnam_IsTheTreeFolderAndWrittenOnce()
    {
        var item = ParseDeep(BuildMswpBytes(LegacyFormVersion, topFnam: null));
        item.Substitutions.ShouldHaveSingleItem().ObsoleteTreeFolder.ShouldBeNull();
        item.TreeFolder.ShouldBe(string.Empty);
        CountFnam(Write(item)).ShouldBe(1);
    }

    [Fact]
    public void Overlay_LegacyFormVersionSubstitutionFnam_IsTheTreeFolderAndWrittenOnce()
    {
        var item = ParseOverlay(BuildMswpBytes(LegacyFormVersion, topFnam: null));
        item.Substitutions.ShouldHaveSingleItem().ObsoleteTreeFolder.ShouldBeNull();
        item.TreeFolder.ShouldBe(string.Empty);
        CountFnam(Write(item)).ShouldBe(1);
    }
}
