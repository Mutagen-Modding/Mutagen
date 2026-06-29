using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout3;

public partial class NavigationMeshInfo
{
    [Flags]
    public enum Flag
    {
        InitiallyDisabled = 0x10,
        IsIsland = 0x20,
    }
}

partial class NavigationMeshInfoBinaryCreateTranslation
{
    public static partial void FillBinaryIslandCustom(MutagenFrame frame, INavigationMeshInfo item)
    {
        if ((item.Flags & NavigationMeshInfo.Flag.IsIsland) == 0) return;
        var island = new IslandData()
        {
            Min = new P3Float(frame.ReadFloat(), frame.ReadFloat(), frame.ReadFloat()),
            Max = new P3Float(frame.ReadFloat(), frame.ReadFloat(), frame.ReadFloat()),
        };
        var vertexCount = frame.ReadUInt16();
        var triangleCount = frame.ReadUInt16();
        for (int i = 0; i < vertexCount; i++)
        {
            island.Vertices.Add(new P3Float(frame.ReadFloat(), frame.ReadFloat(), frame.ReadFloat()));
        }
        for (int i = 0; i < triangleCount; i++)
        {
            island.Triangles.Add(new P3Int16(frame.ReadInt16(), frame.ReadInt16(), frame.ReadInt16()));
        }
        item.Island = island;
    }

    public static partial void FillBinaryPreferredPercentCustom(MutagenFrame frame, INavigationMeshInfo item)
    {
        item.PreferredPercent = frame.ReadFloat();
    }
}

partial class NavigationMeshInfoBinaryWriteTranslation
{
    public static partial void WriteBinaryIslandCustom(MutagenWriter writer, INavigationMeshInfoGetter item)
    {
        if (item.Island is not { } island) return;
        writer.Write(island.Min.X);
        writer.Write(island.Min.Y);
        writer.Write(island.Min.Z);
        writer.Write(island.Max.X);
        writer.Write(island.Max.Y);
        writer.Write(island.Max.Z);
        writer.Write(checked((ushort)island.Vertices.Count));
        writer.Write(checked((ushort)island.Triangles.Count));
        foreach (var v in island.Vertices)
        {
            writer.Write(v.X);
            writer.Write(v.Y);
            writer.Write(v.Z);
        }
        foreach (var t in island.Triangles)
        {
            writer.Write(t.X);
            writer.Write(t.Y);
            writer.Write(t.Z);
        }
    }

    public static partial void WriteBinaryPreferredPercentCustom(MutagenWriter writer, INavigationMeshInfoGetter item)
    {
        writer.Write(item.PreferredPercent);
    }
}

partial class NavigationMeshInfoBinaryOverlay
{
    private IIslandDataGetter? _island;

    partial void CustomIslandEndPos()
    {
        const int islandStart = 0x1C;
        var flags = BinaryPrimitives.ReadUInt32LittleEndian(_structData.Span);
        if ((flags & (uint)NavigationMeshInfo.Flag.IsIsland) == 0)
        {
            _island = null;
            IslandEndingPos = islandStart;
            return;
        }

        var span = _structData.Span.Slice(islandStart);
        var island = new IslandData()
        {
            Min = new P3Float(
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(0, 4)),
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(4, 4)),
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(8, 4))),
            Max = new P3Float(
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(12, 4)),
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(16, 4)),
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(20, 4))),
        };
        var vertexCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(24, 2));
        var triangleCount = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(26, 2));
        var offset = 28;
        for (int i = 0; i < vertexCount; i++)
        {
            island.Vertices.Add(new P3Float(
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(offset, 4)),
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(offset + 4, 4)),
                BinaryPrimitives.ReadSingleLittleEndian(span.Slice(offset + 8, 4))));
            offset += 12;
        }
        for (int i = 0; i < triangleCount; i++)
        {
            island.Triangles.Add(new P3Int16(
                BinaryPrimitives.ReadInt16LittleEndian(span.Slice(offset, 2)),
                BinaryPrimitives.ReadInt16LittleEndian(span.Slice(offset + 2, 2)),
                BinaryPrimitives.ReadInt16LittleEndian(span.Slice(offset + 4, 2))));
            offset += 6;
        }
        _island = island;
        IslandEndingPos = islandStart + offset;
    }

    public partial IIslandDataGetter? GetIslandCustom(int location) => _island;

    public partial float GetPreferredPercentCustom(int location) =>
        BinaryPrimitives.ReadSingleLittleEndian(_structData.Span.Slice(location, 4));
}
