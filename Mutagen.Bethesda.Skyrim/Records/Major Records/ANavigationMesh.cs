using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
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
        IANavigationMeshDataGetter? IANavigationMeshGetter.Data => throw new NotImplementedException();
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
        IANavigationMeshDataGetter? Data { get; }
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
                        {
                            var ret = WorldspaceNavigationMeshData.CreateFromBinary(frame);
                            ret.Parent = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(0, 4), frame.MetaData.MasterReferences!);
                            ret.Coordinates = new P2Int16(
                                BinaryPrimitives.ReadInt16LittleEndian(parentBytes.Slice(4)),
                                BinaryPrimitives.ReadInt16LittleEndian(parentBytes.Slice(6)));
                            worldspace.Data = ret;
                        }
                        break;
                    case ICellNavigationMesh cell:
                        {
                            var ret = CellNavigationMeshData.CreateFromBinary(frame);
                            ret.UnusedWorldspaceParent = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(0, 4), frame.MetaData.MasterReferences!);
                            ret.Parent = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(4, 4), frame.MetaData.MasterReferences!);
                            cell.Data = ret;
                        }
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
                if (!item.Data.TryGet(out var data)) return;
                using (var header = HeaderExport.Subrecord(
                    writer,
                    RecordTypes.NVNM,
                    largeLengthRecord: RecordTypes.XXXX))
                {
                    data.WriteToBinary(header.PrepWriter);
                }
            }
        }

        public partial class ANavigationMeshBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            IANavigationMeshDataGetter IANavigationMeshGetter.Data => throw new NotImplementedException();
            #endregion

            protected ReadOnlyMemorySlice<byte>? _dataSpan;

            partial void LengthLogicCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                var xxxxHeader = _package.MetaData.Constants.ReadSubrecordFrame(stream);
                if (xxxxHeader.Content.Length != 4)
                {
                    throw new ArgumentException("Unexpected length");
                }
                var len = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(xxxxHeader.Content));
                _package.MetaData.Constants.ReadSubrecord(stream);
                _dataSpan = _data.Slice(stream.Position - offset, len);
                stream.Position += checked((int)len);
            }

            partial void DataLogicCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                var subHeader = _package.MetaData.Constants.ReadSubrecord(stream);
                var contentLength = subHeader.ContentLength;
                _dataSpan = _data.Slice(stream.Position - offset, contentLength);
                stream.Position += contentLength;
            }
        }
    }
}
