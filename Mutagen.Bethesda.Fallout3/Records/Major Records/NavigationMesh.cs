using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout3;

public partial class NavigationMesh
{
    [Flags]
    public enum MajorFlag : uint
    {
        AutoGen = 0x0400_0000,
    }
}

partial class NavmeshGridBinaryCreateTranslation
{
    public static partial void FillBinaryCellsCustom(MutagenFrame frame, INavmeshGrid item)
    {
        while (frame.Remaining >= 2)
        {
            var count = frame.ReadUInt16();
            var cell = new NavmeshGridCell();
            for (int i = 0; i < count; i++)
            {
                cell.Triangles.Add(frame.ReadUInt16());
            }
            item.Cells.Add(cell);
        }
    }
}

partial class NavmeshGridBinaryWriteTranslation
{
    public static partial void WriteBinaryCellsCustom(MutagenWriter writer, INavmeshGridGetter item)
    {
        foreach (var cell in item.Cells)
        {
            writer.Write(checked((ushort)cell.Triangles.Count));
            foreach (var triangle in cell.Triangles)
            {
                writer.Write(triangle);
            }
        }
    }
}

partial class NavmeshGridBinaryOverlay
{
    private IReadOnlyList<INavmeshGridCellGetter>? _cells;

    public IReadOnlyList<INavmeshGridCellGetter> Cells => _cells ??= ConstructCells();

    private IReadOnlyList<INavmeshGridCellGetter> ConstructCells()
    {
        var mem = _structData.Slice(0x24);
        var span = mem.Span;
        var locs = new List<int>();
        int pos = 0;
        while (pos + 2 <= span.Length)
        {
            locs.Add(pos);
            pos += 2 + BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(pos)) * 2;
        }
        return BinaryOverlayList.FactoryByArray<INavmeshGridCellGetter>(
            mem,
            _package,
            static (s, p) => NavmeshGridCellBinaryOverlay.NavmeshGridCellFactory(s, p),
            locs);
    }
}
