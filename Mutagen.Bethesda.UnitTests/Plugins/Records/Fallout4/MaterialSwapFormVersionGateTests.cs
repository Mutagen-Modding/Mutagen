using System.Buffers.Binary;
using System.Text;
using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

/// <summary>
/// Regression coverage for Mutagen-Modding/Mutagen#687: MaterialSwap's FNAM form-version
/// gate needs MetaData.FormVersion available while its custom struct-fill logic runs, for
/// both the deep parser and the overlay factory.
/// </summary>
public class MaterialSwapFormVersionGateTests
{
    /// <summary>
    /// Builds a minimal, hand-crafted MSWP record: a top-level FNAM (the new-format tree
    /// folder), then a single substitution (BNAM/SNAM) followed by its own obsolete FNAM
    /// field. Pre-112, all FNAM occurrences in a record are expected to carry the same
    /// string; 112+, the top FNAM is the tree folder and a substitution's own FNAM is
    /// vestigial and may legitimately differ.
    /// </summary>
    private static byte[] BuildMswpBytes(ushort formVersion, string topFnam, string substitutionFnam)
    {
        var content = new List<byte>();
        content.AddRange(Subrecord("FNAM", NullTerminated(topFnam)));
        content.AddRange(Subrecord("BNAM", NullTerminated("OriginalMat")));
        content.AddRange(Subrecord("SNAM", NullTerminated("ReplacementMat")));
        content.AddRange(Subrecord("FNAM", NullTerminated(substitutionFnam)));
        var contentBytes = content.ToArray();

        var header = new byte[24];
        Encoding.ASCII.GetBytes("MSWP").CopyTo(header, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(4), checked((uint)contentBytes.Length));
        // Flags (offset 8) and VersionControl (offset 16) left zeroed.
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(12), 0x00000001); // FormID
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(20), formVersion);
        // Version2 (offset 22) left zeroed.

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
        global::Mutagen.Bethesda.Fallout4.Constants.Fallout4,
        SeparatedMasterPackage.NotSeparate(new MasterReferenceCollection(global::Mutagen.Bethesda.Fallout4.Constants.Fallout4)));

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

    [Fact]
    public void DeepParse_V131DifferingFnam_ParsesTreeFolder()
    {
        var bytes = BuildMswpBytes(131, topFnam: "materials", substitutionFnam: "");
        var item = ParseDeep(bytes);
        item.TreeFolder.ShouldBe("materials");
    }

    [Fact]
    public void DeepParse_SubNewFormVersionDifferingFnam_StillThrows()
    {
        var bytes = BuildMswpBytes(90, topFnam: "materials", substitutionFnam: "");
        Should.Throw<MalformedDataException>(() => ParseDeep(bytes));
    }

    [Fact]
    public void Overlay_V131DifferingFnam_ParsesTreeFolder()
    {
        var bytes = BuildMswpBytes(131, topFnam: "materials", substitutionFnam: "");
        var item = ParseOverlay(bytes);
        item.TreeFolder.ShouldBe("materials");
    }

    [Fact]
    public void Overlay_SubNewFormVersionDifferingFnam_StillThrows()
    {
        var bytes = BuildMswpBytes(90, topFnam: "materials", substitutionFnam: "");
        var item = ParseOverlay(bytes);
        Should.Throw<MalformedDataException>(() => item.TreeFolder);
    }
}
