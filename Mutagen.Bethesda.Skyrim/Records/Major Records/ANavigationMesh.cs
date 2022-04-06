using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

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

partial class ANavigationMeshBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryLengthLogicCustom(MutagenFrame frame, IANavigationMeshInternal item)
    {
        frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
        var xxxxSize = frame.ReadInt32();
        HeaderTranslation.ReadNextSubrecordType(frame, out var len);
        frame = frame.SpawnWithLength(xxxxSize, checkFraming: false);
        GetSetData(frame, item);
        return null;
    }

    public static partial ParseResult FillBinaryDataLogicCustom(MutagenFrame frame, IANavigationMeshInternal item)
    {
        HeaderTranslation.ReadNextSubrecordType(frame, out var len);
        frame = frame.SpawnWithLength(len);
        GetSetData(frame, item);
        return null;
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
                ret.Parent.FormKey = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(0, 4), frame.MetaData.MasterReferences!);
                ret.Coordinates = new P2Int16(
                    BinaryPrimitives.ReadInt16LittleEndian(parentBytes.Slice(4)),
                    BinaryPrimitives.ReadInt16LittleEndian(parentBytes.Slice(6)));
                worldspace.Data = ret;
            }
                break;
            case ICellNavigationMesh cell:
            {
                var ret = CellNavigationMeshData.CreateFromBinary(frame);
                ret.UnusedWorldspaceParent.FormKey = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(0, 4), frame.MetaData.MasterReferences!);
                ret.Parent.FormKey = FormKeyBinaryTranslation.Instance.Parse(parentBytes.Slice(4, 4), frame.MetaData.MasterReferences!);
                cell.Data = ret;
            }
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class ANavigationMeshBinaryWriteTranslation
{
    public static partial void WriteBinaryLengthLogicCustom(MutagenWriter writer, IANavigationMeshGetter item)
    {
        // Handled in data logic
    }

    public static partial void WriteBinaryDataLogicCustom(MutagenWriter writer, IANavigationMeshGetter item)
    {
        if (item.Data is not { } data) return;
        using (var header = HeaderExport.Subrecord(
                   writer,
                   RecordTypes.NVNM,
                   overflowRecord: RecordTypes.XXXX,
                   out var writerToUse))
        {
            data.WriteToBinary(writerToUse);
        }
    }
}

partial class ANavigationMeshBinaryOverlay
{
    #region Interfaces
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IANavigationMeshDataGetter IANavigationMeshGetter.Data => throw new NotImplementedException();
    #endregion

    protected ReadOnlyMemorySlice<byte>? _dataSpan;

    public partial ParseResult LengthLogicCustomParse(OverlayStream stream, int offset)
    {
        var xxxxHeader = stream.ReadSubrecordFrame();
        if (xxxxHeader.Content.Length != 4)
        {
            throw new ArgumentException("Unexpected length");
        }
        var len = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(xxxxHeader.Content));
        stream.ReadSubrecord();
        _dataSpan = _data.Slice(stream.Position - offset, len);
        stream.Position += checked((int)len);
        return null;
    }

    public partial ParseResult DataLogicCustomParse(OverlayStream stream, int offset)
    {
        var subHeader = stream.ReadSubrecord();
        var contentLength = subHeader.ContentLength;
        _dataSpan = _data.Slice(stream.Position - offset, contentLength);
        stream.Position += contentLength;
        return null;
    }
}