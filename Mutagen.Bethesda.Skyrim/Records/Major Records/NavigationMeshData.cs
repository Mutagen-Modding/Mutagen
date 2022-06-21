using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;
using System.Collections;

namespace Mutagen.Bethesda.Skyrim;

partial class NavigationMeshDataBinaryCreateTranslation
{
    public static partial void FillBinaryCoverTrianglesLogicCustom(MutagenFrame frame, INavigationMeshData item)
    {
        var count = frame.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            var index = frame.ReadInt16();
            item.Triangles[index].IsCover = true;
        }
    }

    public static partial void FillBinaryParentCustom(MutagenFrame frame, INavigationMeshData item)
    {
        item.Parent = GetBinaryParent(frame);
    }

    public static ANavmeshParent GetBinaryParent<TReader>(TReader frame)
        where TReader : IMutagenReadStream
    {
        var formKey = FormKeyBinaryTranslation.Instance.Parse(frame);
        if (formKey.IsNull)
        {
            var cell = new CellNavmeshParent();
            cell.UnusedWorldspaceParent.SetTo(formKey);
            cell.Parent.SetTo(FormKeyBinaryTranslation.Instance.Parse(frame));
            return cell;
        }
        else
        {
            var wrld = new WorldspaceNavmeshParent();
            wrld.Parent.SetTo(formKey);
            wrld.Coordinates = P2Int16BinaryTranslation<TReader, MutagenWriter>.Instance.Parse(reader: frame);
            return wrld;
        }
    }

    public static partial void FillBinaryNavmeshGridCustom(MutagenFrame frame, INavigationMeshData item)
    {
        item.NavmeshGrid = frame.ReadRemainingBytes();
    }
}

partial class NavigationMeshDataBinaryWriteTranslation
{
    public static partial void WriteBinaryCoverTrianglesLogicCustom(MutagenWriter writer, INavigationMeshDataGetter item)
    {
        var triangles = item.Triangles;
        var indices = new List<short>();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i].IsCover)
            {
                indices.Add(checked((short)i));
            }
        }
        writer.Write(indices.Count);
        foreach (var index in indices)
        {
            writer.Write(index);
        }
    }

    public static partial void WriteBinaryParentCustom(MutagenWriter writer, INavigationMeshDataGetter item)
    {
        item.Parent.WriteToBinary(writer);
    }

    public static partial void WriteBinaryNavmeshGridCustom(MutagenWriter writer, INavigationMeshDataGetter item)
    {
        writer.Write(item.NavmeshGrid);
    }
}

partial class NavigationMeshDataBinaryOverlay
{
    public uint NavmeshGridDivisor => BinaryPrimitives.ReadUInt32LittleEndian(_structData.Slice(CoverTrianglesLogicEndingPos));

    public float MaxDistanceX => _structData.Slice(CoverTrianglesLogicEndingPos + 4).Float();

    public float MaxDistanceY => _structData.Slice(CoverTrianglesLogicEndingPos + 8).Float();

    public P3Float Min => new P3Float(
        _structData.Slice(CoverTrianglesLogicEndingPos + 12).Float(),
        _structData.Slice(CoverTrianglesLogicEndingPos + 16).Float(),
        _structData.Slice(CoverTrianglesLogicEndingPos + 20).Float());

    public P3Float Max => new P3Float(
        _structData.Slice(CoverTrianglesLogicEndingPos + 24).Float(),
        _structData.Slice(CoverTrianglesLogicEndingPos + 28).Float(),
        _structData.Slice(CoverTrianglesLogicEndingPos + 32).Float());

    public ReadOnlyMemorySlice<byte> NavmeshGrid { get; private set; }

    public IReadOnlyList<INavmeshTriangleGetter> Triangles =>
        new TrianglesOverlay(
            _structData,
            _package,
            trianglesStartPos: VerticesEndingPos,
            coverIndicesStartPos: DoorTrianglesEndingPos);

    public partial IANavmeshParentGetter GetParentCustom(int location)
    {
        return NavigationMeshDataBinaryCreateTranslation.GetBinaryParent(new OverlayStream(_structData.Slice(8), _package));
    }

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        CoverTrianglesLogicEndingPos = DoorTrianglesEndingPos + checked((int)((BinaryPrimitives.ReadUInt32LittleEndian(_structData.Slice(DoorTrianglesEndingPos)) * 2) + 4));
        NavmeshGrid = _structData.Slice(CoverTrianglesLogicEndingPos + 0x24);
    }

    partial void CustomTrianglesEndPos()
    {
        var count = BinaryPrimitives.ReadUInt32LittleEndian(_structData.Slice(VerticesEndingPos));
        TrianglesEndingPos = VerticesEndingPos + checked((int)((count * 0x10) + 4));
    }

    class TrianglesOverlay : IReadOnlyList<INavmeshTriangleGetter>
    {
        private ReadOnlyMemorySlice<byte> _data;
        private BinaryOverlayFactoryPackage _package;
        private int _trianglesStartPos;
        private int _trianglesCount;
        private bool[] _isCover;

        public TrianglesOverlay(
            ReadOnlyMemorySlice<byte> data,
            BinaryOverlayFactoryPackage package,
            int trianglesStartPos,
            int coverIndicesStartPos)
        {
            _data = data;
            _package = package;
            _trianglesStartPos = trianglesStartPos;
            _trianglesCount = BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(_trianglesStartPos));
            _trianglesStartPos += 4;
            _isCover = new bool[_trianglesCount];
            var coverCount = BinaryPrimitives.ReadInt32LittleEndian(_data.Slice(coverIndicesStartPos));
            coverIndicesStartPos += 4;
            var coverIndices = _data.Span.Slice(coverIndicesStartPos, coverCount * 2).AsInt16Span();
            foreach (var index in coverIndices)
            {
                _isCover[index] = true;
            }
        }

        public INavmeshTriangleGetter this[int index]
        {
            get
            {
                var triangleLoc = _trianglesStartPos + (index * 16);
                var triangle = (NavmeshTriangleBinaryOverlay)NavmeshTriangleBinaryOverlay.NavmeshTriangleFactory(
                    _data.Slice(triangleLoc),
                    _package);
                triangle.IsCover = _isCover[index];
                return triangle;
            }
        }

        public int Count => _trianglesCount;

        public IEnumerator<INavmeshTriangleGetter> GetEnumerator()
        {
            for (int i = 0; i < _trianglesCount; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}