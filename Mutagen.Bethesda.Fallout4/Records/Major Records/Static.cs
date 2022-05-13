using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

public partial class Static
{
    [Flags]
    public enum MajorFlag
    {
        HeadingMarker = 0x0000_0004,
        NonOccluder = 0x0000_0010,
        Deleted = 0x0000_0020,
        HasTreeLod = 0x0000_0040,
        AddOnLodObject = 0x0000_0080,
        HiddenFromLocalMap = 0x0000_0200,
        HeadtrackMarker = 0x0000_0400,
        UsedAsPlatform = 0x0000_0800,
        PackInUseOnly = 0x0000_2000,
        HasDistantLod = 0x0000_8000,
        UsesHdLodTexture = 0x0002_0000,
        HasCurrents = 0x0008_0000,
        IsMarker = 0x0080_0000,
        Obstacle = 0x0200_0000,
        NavMeshGenerationFilter = 0x0400_0000,
        NavMeshGenerationBoundingBox = 0x0800_0000,
        /// <summary>
        /// Sky cell only
        /// </summary>
        ShowInWorldMap = 0x1000_0000,
        NavMeshGenerationGround = 0x4000_0000,
    }
}

partial class StaticBinaryOverlay
{
    public IReadOnlyList<IDistantLodGetter> DistantLods { get; private set; } = Array.Empty<IDistantLodGetter>();

    public partial ParseResult DistantLodParsingCustomParse(
        OverlayStream stream,
        int offset)
    {
        var amount = StaticBinaryCreateTranslation.ReadHeader(stream);
        if (amount == 0) return (int)Static_FieldIndex.DistantLods;
        var list = new List<IDistantLodGetter>();
        for (int i = 0; i < amount; i++)
        {
            list.Add(StaticBinaryCreateTranslation.Parse(stream));
        }

        DistantLods = list;
        return (int)Static_FieldIndex.DistantLods;
    }
}
    
partial class StaticBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryDistantLodParsingCustom(
        MutagenFrame frame,
        IStaticInternal item)
    {
        var amount = StaticBinaryCreateTranslation.ReadHeader(frame);
        if (amount == 0) return (int)Static_FieldIndex.DistantLods;
        item.DistantLods.Clear();
        for (int i = 0; i < amount; i++)
        {
            item.DistantLods.Add(StaticBinaryCreateTranslation.Parse(frame));
        }

        return (int)Static_FieldIndex.DistantLods;
    }

    public static int ReadHeader<T>(T frame)
        where T : IMutagenReadStream
    {
        var subRec = frame.ReadSubrecordHeader(RecordTypes.MNAM);
        var len = subRec.ContentLength;
        var amount = len / 260;
        var over = len % 260;
        if (over != 0)
        {
            throw new ArgumentException($"Unexpected length.  {subRec.ContentLength} % 260 != 0");
        }

        if (amount > 4)
        {
            throw new ArgumentException($"Cannot have more than 4 distant lods: {amount}");
        }

        return amount;
    }

    public static DistantLod Parse<T>(T frame)
        where T : IMutagenReadStream
    {
        var str = StringBinaryTranslation.Instance.Parse(
            reader: frame,
            stringBinaryType: StringBinaryType.NullTerminate,
            parseWhole: false);
        var bytes = frame.ReadBytes(260 - str.Length - 1);
        return new DistantLod()
        {
            Mesh = str,
            Data = bytes
        };
    }
}

partial class StaticBinaryWriteTranslation
{
    public static partial void WriteBinaryDistantLodParsingCustom(
        MutagenWriter writer,
        IStaticGetter item)
    {
        var lods = item.DistantLods;
        if (lods.Count == 0) return;
                
        using var header = HeaderExport.Subrecord(writer, RecordTypes.MNAM);
        if (lods.Count > 4)
        {
            throw new ArgumentException($"Cannot have more than 4 distant lods: {lods.Count}");
        }

        foreach (var lod in lods)
        {
            lod.WriteToBinary(writer);
        }
    }

    public static void Write(MutagenWriter writer, IDistantLodGetter lod)
    {
        var len = lod.Data.Length + lod.Mesh.Length + 1;
        if (len != 260)
        {
            throw new ArgumentException($"Distant Lod string and data size did not add up to expected length. {len} != 260");
        }
        lod.WriteToBinary(writer);
    }
}