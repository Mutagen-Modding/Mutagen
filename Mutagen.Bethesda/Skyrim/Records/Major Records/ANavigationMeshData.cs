using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
            public uint NavmeshGridDivisor => throw new NotImplementedException();

            public float MaxDistanceX => throw new NotImplementedException();

            public float MaxDistanceY => throw new NotImplementedException();

            public P3Float Min => throw new NotImplementedException();

            public P3Float Max => throw new NotImplementedException();

            public ReadOnlyMemorySlice<byte> NavmeshGrid => throw new NotImplementedException();

            partial void CoverTrianglesLogicCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                throw new NotImplementedException();
            }
        }
    }
}
