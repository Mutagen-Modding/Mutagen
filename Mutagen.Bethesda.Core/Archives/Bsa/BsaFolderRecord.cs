using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Archives.Bsa;

class BsaFolderRecord : IArchiveFolder
{
    internal readonly BsaReader BSA;
    private readonly ReadOnlyMemorySlice<byte> _data;
    internal Lazy<BsaFileRecord[]> _files = null!;
    private int _prevFileCount;
    internal BsaFileNameBlock? FileNameBlock;
    public int Index { get; }
    public string? Path { get; private set; }

    public IReadOnlyCollection<IArchiveFile> Files => _files.Value;

    internal BsaFolderRecord(BsaReader bsa, ReadOnlyMemorySlice<byte> data, int index)
    {
        BSA = bsa;
        _data = data;
        Index = index;
    }

    private bool IsLongform => BSA.HeaderType == BsaVersionType.SSE;

    public ulong Hash => BinaryPrimitives.ReadUInt64LittleEndian(_data);

    public int FileCount => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(0x8)));

    public uint Unknown => IsLongform ?
        BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(0xC)) :
        0;

    public ulong Offset => IsLongform ?
        BinaryPrimitives.ReadUInt64LittleEndian(_data.Slice(0x10)) :
        BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(0xC));

    public static int HeaderLength(BsaVersionType version)
    {
        return version switch
        {
            BsaVersionType.SSE => 0x18,
            _ => 0x10,
        };
    }

    internal void ProcessFileRecordHeadersBlock(BinaryReader rdr, int fileCountTally)
    {
        _prevFileCount = fileCountTally;
        var totalFileLen = checked((int)(FileCount * BsaFileRecord.HeaderLength));

        ReadOnlyMemorySlice<byte> data;
        if (BSA.HasFolderNames)
        {
            var len = rdr.ReadByte();
            data = rdr.ReadBytes(len + totalFileLen);
            Path = data.Slice(0, len).ReadStringTerm(BSA.HeaderType);
            data = data.Slice(len);
        }
        else
        {
            data = rdr.ReadBytes(totalFileLen);
        }

        _files = new Lazy<BsaFileRecord[]>(
            isThreadSafe: true,
            valueFactory: () => ParseFileRecords(data));
    }

    private BsaFileRecord[] ParseFileRecords(ReadOnlyMemorySlice<byte> data)
    {
        var fileCount = FileCount;
        var ret = new BsaFileRecord[fileCount];
        for (var idx = 0; idx < fileCount; idx += 1)
        {
            var fileData = data.Slice(idx * BsaFileRecord.HeaderLength, BsaFileRecord.HeaderLength);
            ret[idx] = new BsaFileRecord(this, fileData, idx, idx + _prevFileCount, FileNameBlock);
        }
        return ret;
    }
}