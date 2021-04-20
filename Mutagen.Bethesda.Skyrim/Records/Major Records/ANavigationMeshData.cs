using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class ANavigationMeshDataBinaryCreateTranslation
        {
            static partial void FillBinaryCoverTrianglesLogicCustom(MutagenFrame frame, IANavigationMeshData item)
            {
                var count = frame.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    var index = frame.ReadInt16();
                    item.Triangles[index].IsCover = true;
                }
            }

            static partial void FillBinaryParentLogicCustom(MutagenFrame frame, IANavigationMeshData item)
            {
                // Handled in Navigation Mesh parent
                frame.Position += 8;
            }

            static partial void FillBinaryNavmeshGridCustom(MutagenFrame frame, IANavigationMeshData item)
            {
                item.NavmeshGrid = frame.ReadRemainingBytes();
            }
        }

        public partial class ANavigationMeshDataBinaryWriteTranslation
        {
            static partial void WriteBinaryCoverTrianglesLogicCustom(MutagenWriter writer, IANavigationMeshDataGetter item)
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

            static partial void WriteBinaryParentLogicCustom(MutagenWriter writer, IANavigationMeshDataGetter item)
            {
                switch (item)
                {
                    case ICellNavigationMeshDataGetter cell:
                        FormKeyBinaryTranslation.Instance.Write(writer, cell.UnusedWorldspaceParent.FormKey);
                        FormKeyBinaryTranslation.Instance.Write(writer, cell.Parent.FormKey);
                        break;
                    case IWorldspaceNavigationMeshDataGetter worldspace:
                        FormKeyBinaryTranslation.Instance.Write(writer, worldspace.Parent.FormKey);
                        var coords = worldspace.Coordinates;
                        writer.Write(coords.X);
                        writer.Write(coords.Y);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            static partial void WriteBinaryNavmeshGridCustom(MutagenWriter writer, IANavigationMeshDataGetter item)
            {
                writer.Write(item.NavmeshGrid);
            }
        }

        public partial class ANavigationMeshDataBinaryOverlay
        {
            public uint NavmeshGridDivisor => BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(CoverTrianglesLogicEndingPos));

            public float MaxDistanceX => _data.Slice(CoverTrianglesLogicEndingPos + 4).Float();

            public float MaxDistanceY => _data.Slice(CoverTrianglesLogicEndingPos + 8).Float();

            public P3Float Min => new P3Float(
                _data.Slice(CoverTrianglesLogicEndingPos + 12).Float(),
                _data.Slice(CoverTrianglesLogicEndingPos + 16).Float(),
                _data.Slice(CoverTrianglesLogicEndingPos + 20).Float());

            public P3Float Max => new P3Float(
                _data.Slice(CoverTrianglesLogicEndingPos + 24).Float(),
                _data.Slice(CoverTrianglesLogicEndingPos + 28).Float(),
                _data.Slice(CoverTrianglesLogicEndingPos + 32).Float());

            public ReadOnlyMemorySlice<byte> NavmeshGrid { get; private set; }

            public IReadOnlyList<INavmeshTriangleGetter> Triangles => 
                new TrianglesOverlay(
                    _data, 
                    _package,
                    trianglesStartPos: VerticesEndingPos, 
                    coverIndicesStartPos: DoorTrianglesEndingPos);

            protected void CustomLogic()
            {
                VerticesEndingPos = checked((int)((BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(16)) * 12) + 20));
                TrianglesEndingPos = VerticesEndingPos + checked((int)((BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(VerticesEndingPos)) * 16) + 4));
                EdgeLinksEndingPos = TrianglesEndingPos + checked((int)((BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(TrianglesEndingPos)) * 10) + 4));
                DoorTrianglesEndingPos = EdgeLinksEndingPos + checked((int)((BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(EdgeLinksEndingPos)) * 10) + 4));
                CoverTrianglesLogicEndingPos = DoorTrianglesEndingPos + checked((int)((BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(DoorTrianglesEndingPos)) * 2) + 4));
                NavmeshGrid = _data.Slice(CoverTrianglesLogicEndingPos + 0x24);
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
                    this._data = data;
                    this._package = package;
                    this._trianglesStartPos = trianglesStartPos;
                    this._trianglesCount = BinaryPrimitives.ReadInt32LittleEndian(this._data.Slice(this._trianglesStartPos));
                    this._trianglesStartPos += 4;
                    this._isCover = new bool[this._trianglesCount];
                    var coverCount = BinaryPrimitives.ReadInt32LittleEndian(this._data.Slice(coverIndicesStartPos));
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
                        var triangle = NavmeshTriangleBinaryOverlay.NavmeshTriangleFactory(
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

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            }
        }
    }
}
