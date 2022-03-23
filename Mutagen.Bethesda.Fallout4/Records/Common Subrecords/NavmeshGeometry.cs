using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class NavmeshGeometry
    {
        partial void CustomCtor()
        {
            Parent = new CellNavmeshParent();
        }
    }
    
    namespace Internals
    {
        public partial class NavmeshGeometryBinaryCreateTranslation
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

            public static partial void FillBinaryNavmeshGridCustom(MutagenFrame frame, INavmeshGeometry item)
            {
                item.NavmeshGrid = frame.ReadRemainingBytes();
            }
        }

        public partial class NavmeshGeometryBinaryWriteTranslation
        {
            public static partial void WriteBinaryParentCustom(MutagenWriter writer, INavmeshGeometryGetter item)
            {
                item.Parent.WriteToBinary(writer);
            }

            public static partial void WriteBinaryNavmeshGridCustom(MutagenWriter writer, INavmeshGeometryGetter item)
            {
                writer.Write(item.NavmeshGrid);
            }
        }

        public partial class NavmeshGeometryBinaryOverlay
        {
            public ReadOnlyMemorySlice<byte> NavmeshGrid { get; private set; }

            public IReadOnlyList<INavmeshTriangleGetter> Triangles =>
                new TrianglesOverlay(
                    _data,
                    _package,
                    trianglesStartPos: VerticesEndingPos);

            public IANavmeshParentGetter GetParentCustom(int location)
            {
                return NavmeshGeometryBinaryCreateTranslation.GetBinaryParent(new OverlayStream(_data.Slice(8), _package));
            }

            partial void CustomTrianglesEndPos()
            {
                var count = BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(VerticesEndingPos));
                TrianglesEndingPos = VerticesEndingPos + checked((int)((count * 0x15) + 4));
            }

            partial void CustomFactoryEnd(
                OverlayStream stream,
                int finalPos,
                int offset)
            {
                NavmeshGrid = _data.Slice(Unknown2EndingPos);
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
    }
}
