using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using System.Collections;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Starfield;

public partial class NavmeshGeometry
{
    partial void CustomCtor()
    {
        Parent = new CellNavmeshParent();
    }
}
    
partial class NavmeshGeometryBinaryCreateTranslation
{
    public static partial void FillBinaryParentCustom(MutagenFrame frame, INavmeshGeometry item)
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
}

partial class NavmeshGeometryBinaryWriteTranslation
{
    public static partial void WriteBinaryParentCustom(MutagenWriter writer, INavmeshGeometryGetter item)
    {
        item.Parent.WriteToBinary(writer);
    }
}

partial class NavmeshGeometryBinaryOverlay
{
    public IReadOnlyList<INavmeshTriangleGetter> Triangles =>
        new TrianglesOverlay(
            _structData,
            _package,
            trianglesStartPos: VerticesEndingPos);

    public partial IANavmeshParentGetter GetParentCustom(int location)
    {
        return NavmeshGeometryBinaryCreateTranslation.GetBinaryParent(new OverlayStream(_structData.Slice(8), _package));
    }

    partial void CustomTrianglesEndPos()
    {
        var count = BinaryPrimitives.ReadUInt32LittleEndian(_structData.Slice(VerticesEndingPos));
        TrianglesEndingPos = VerticesEndingPos + checked((int)((count * 0x15) + 4));
    }

    class TrianglesOverlay : IReadOnlyList<INavmeshTriangleGetter>
    {
        private ReadOnlyMemorySlice<byte> _data;
        private BinaryOverlayFactoryPackage _package;
        private int _trianglesStartPos;
        private int _trianglesCount;

        public TrianglesOverlay(
            ReadOnlyMemorySlice<byte> data,
            BinaryOverlayFactoryPackage package,
            int trianglesStartPos)
        {
            this._data = data;
            this._package = package;
            this._trianglesStartPos = trianglesStartPos;
            this._trianglesCount = BinaryPrimitives.ReadInt32LittleEndian(this._data.Slice(this._trianglesStartPos));
            this._trianglesStartPos += 4;
        }

        public INavmeshTriangleGetter this[int index]
        {
            get
            {
                var triangleLoc = _trianglesStartPos + (index * 0x15);
                var triangle = NavmeshTriangleBinaryOverlay.NavmeshTriangleFactory(
                    _data.Slice(triangleLoc),
                    _package);
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

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}