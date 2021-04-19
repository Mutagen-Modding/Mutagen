using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Archives.Bsa
{
    class BsaReader : IArchiveReader
    {
        public const int HeaderLength = 0x24;

        private readonly uint _folderRecordOffset;
        private readonly Lazy<BsaFolderRecord[]> _folders;
        private readonly Lazy<Dictionary<string, BsaFolderRecord>> _foldersByName;
        private readonly string _magic = string.Empty;
        private readonly Func<Stream> _streamGetter;

        public uint FolderCount { get; }
        public uint FileCount { get; }
        public uint TotalFileNameLength { get; }
        public uint TotalFolderNameLength { get; }

        public BsaVersionType HeaderType { get; private set; }

        public BsaArchiveFlags ArchiveFlags { get; private set; }

        public BsaFileFlags FileFlags { get; private set; }

        public IEnumerable<IArchiveFile> Files => _folders.Value.SelectMany(f => f.Files);

        public IEnumerable<IArchiveFolder> Folders => _folders.Value;

        public bool HasFolderNames => ArchiveFlags.HasFlag(BsaArchiveFlags.HasFolderNames);

        public bool HasFileNames => ArchiveFlags.HasFlag(BsaArchiveFlags.HasFileNames);

        public bool CompressedByDefault => ArchiveFlags.HasFlag(BsaArchiveFlags.Compressed);

        public bool Bit9Set => ArchiveFlags.HasFlag(BsaArchiveFlags.HasFileNameBlobs);

        public bool HasNameBlobs
        {
            get
            {
                if (HeaderType == BsaVersionType.FO3 || HeaderType == BsaVersionType.SSE) return Bit9Set;
                return false;
            }
        }

        public BsaReader(string filename)
            : this(() => File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        public BsaReader(Func<Stream> streamGetter)
        {
            _streamGetter = streamGetter;
            using var rdr = GetStream();

            var fourcc = Encoding.ASCII.GetString(rdr.ReadBytes(4));

            if (fourcc != "BSA\0")
                throw new InvalidDataException("Archive is not a BSA");

            _magic = fourcc;
            HeaderType = (BsaVersionType)rdr.ReadUInt32();
            _folderRecordOffset = rdr.ReadUInt32();
            ArchiveFlags = (BsaArchiveFlags)rdr.ReadUInt32();
            FolderCount = rdr.ReadUInt32();
            FileCount = rdr.ReadUInt32();
            TotalFolderNameLength = rdr.ReadUInt32();
            TotalFileNameLength = rdr.ReadUInt32();
            FileFlags = (BsaFileFlags)rdr.ReadUInt32();

            _folders = new Lazy<BsaFolderRecord[]>(
                isThreadSafe: true,
                valueFactory: () => LoadFolderRecords());
            _foldersByName = new Lazy<Dictionary<string, BsaFolderRecord>>(
                isThreadSafe: true,
                valueFactory: GetFolderDictionary);
        }

        internal BinaryReader GetStream()
        {
            return new BinaryReader(_streamGetter());
        }

        private BsaFolderRecord[] LoadFolderRecords()
        {
            using var rdr = GetStream();
            rdr.BaseStream.Position = _folderRecordOffset;
            var folderHeaderLength = BsaFolderRecord.HeaderLength(HeaderType);
            ReadOnlyMemorySlice<byte> folderHeaderData = rdr.ReadBytes(checked((int)(folderHeaderLength * FolderCount)));

            var ret = new BsaFolderRecord[FolderCount];
            for (var idx = 0; idx < FolderCount; idx += 1)
                ret[idx] = new BsaFolderRecord(this, folderHeaderData.Slice(idx * folderHeaderLength, folderHeaderLength), idx);

            // Slice off appropriate file header data per folder
            int fileCountTally = 0;
            foreach (var folder in ret)
            {
                folder.ProcessFileRecordHeadersBlock(rdr, fileCountTally);
                fileCountTally = checked((int)(fileCountTally + folder.FileCount));
            }

            if (HasFileNames)
            {
                var filenameBlock = new BsaFileNameBlock(this, rdr.BaseStream.Position);
                foreach (var folder in ret)
                {
                    folder.FileNameBlock = filenameBlock;
                }
            }

            return ret;
        }

        private Dictionary<string, BsaFolderRecord> GetFolderDictionary()
        {
            if (!HasFolderNames)
            {
                throw new ArgumentException("Cannot get folders by name if the BSA does not have folder names.");
            }
            var ret = new Dictionary<string, BsaFolderRecord>();
            foreach (var folder in _folders.Value)
            {
                ret.Add(folder.Path!, folder);
            }
            return ret;
        }

        public bool TryGetFolder(string path, [MaybeNullWhen(false)] out IArchiveFolder folder)
        {
            if (!HasFolderNames
                || !_foldersByName.Value.TryGetValue(path, out var folderRec))
            {
                folder = default;
                return false;
            }
            folder = folderRec;
            return true;
        }
    }
}
