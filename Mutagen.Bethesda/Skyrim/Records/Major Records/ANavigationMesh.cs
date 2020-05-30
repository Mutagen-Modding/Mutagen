using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ANavigationMesh
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IANavigationMeshData IANavigationMesh.Data => throw new NotImplementedException();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IANavigationMeshDataGetter IANavigationMeshGetter.Data => throw new NotImplementedException();
        #endregion

        [Flags]
        public enum MajorFlag : uint
        {
            AutoGen = 0x0400_0000,
            NavmeshGenCell = 0x8000_0000,
        }
    }

    public partial interface IANavigationMesh
    {
        new IANavigationMeshData Data { get; }
    }

    public partial interface IANavigationMeshGetter
    {
        IANavigationMeshDataGetter Data { get; }
    }

    namespace Internals
    {
        public partial class ANavigationMeshBinaryCreateTranslation
        {
            static partial void FillBinaryLengthLogicCustom(MutagenFrame frame, IANavigationMeshInternal item)
            {
                frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
                var xxxxSize = frame.ReadInt32();
                HeaderTranslation.ReadNextSubrecordType(frame, out var len);
                frame = frame.SpawnWithLength(xxxxSize, checkFraming: false);
                GetSetData(frame, item);
            }

            static partial void FillBinaryDataLogicCustom(MutagenFrame frame, IANavigationMeshInternal item)
            {
                HeaderTranslation.ReadNextSubrecordType(frame, out var len);
                frame = frame.SpawnWithLength(len);
                GetSetData(frame, item);
            }

            public static void GetSetData(MutagenFrame frame, IANavigationMeshInternal item)
            {
                var parentBytes = frame.GetSpan(
                    amount: 8,
                    offset: 8,
                    readSafe: true);
                switch (item)
                {
                    case IWorldspaceNavigationMesh worldspace:
                        throw new NotImplementedException();
                    case ICellNavigationMesh cell:
                        var ret = CellNavigationMeshData.CreateFromBinary(frame);
                        ret.UnusedWorldspaceParent = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(0, 4), frame.MetaData.MasterReferences!);
                        ret.Parent = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(4, 4), frame.MetaData.MasterReferences!);
                        cell.Data = ret;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public partial class ANavigationMeshBinaryWriteTranslation
        {
            static partial void WriteBinaryLengthLogicCustom(MutagenWriter writer, IANavigationMeshGetter item)
            {
                // Handled in data logic
            }

            static partial void WriteBinaryDataLogicCustom(MutagenWriter writer, IANavigationMeshGetter item)
            {
                using (var header = HeaderExport.ExportSubrecordHeader(
                    writer, 
                    ANavigationMesh_Registration.NVNM_HEADER,
                    largeLengthRecord: ANavigationMesh_Registration.XXXX_HEADER))
                {
                    item.Data.WriteToBinary(header.PrepWriter);
                }
            }
        }

        public partial class ANavigationMeshBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IANavigationMeshDataGetter IANavigationMeshGetter.Data => throw new NotImplementedException();
            #endregion

            partial void DataLogicCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                throw new NotImplementedException();
            }
        }
    }
}
