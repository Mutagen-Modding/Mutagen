using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Noggog;
using Noggog.Streams;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Text;

namespace Mutagen.Bethesda.Archives.Ba2;

class Ba2Reader : IArchiveReader
{
    private readonly ulong _nameTableOffset;
    internal Func<Stream> _streamFactory;
    public bool UseATIFourCC { get; set; } = false;

    public bool HasNameTable => _nameTableOffset > 0;
    
    public uint Version { get; }

    public Ba2Reader(FilePath filename, IFileSystem? fileSystem = null)
        : this(() => fileSystem.GetOrDefault().File.Open(filename.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
    {
    }

    public Ba2Reader(Func<Stream> streamGetter)
    {
        _streamFactory = streamGetter;
        using var reader = new BinaryReader(_streamFactory(), Encoding.UTF8);

        var headerMagic = Encoding.ASCII.GetString(reader.ReadBytes(4));

        if (headerMagic != "BTDX")
            throw new InvalidDataException($"Unknown header type: {headerMagic}");

        Version = reader.ReadUInt32();

        string fourcc = Encoding.ASCII.GetString(reader.ReadBytes(4));

        if (!Enum.TryParse(fourcc, out Ba2EntryType entryType))
        {
            throw new InvalidDataException($"Can't parse entry types of {fourcc}");
        }

        var numFiles = reader.ReadUInt32();
        _nameTableOffset = reader.ReadUInt64();

        if (Version > 1)
        {
            var unknown = reader.ReadUInt32();
        }

        var files = new List<IArchiveFile>();
        for (var idx = 0; idx < numFiles; idx += 1)
        {
            switch (entryType)
            {
                case Ba2EntryType.GNRL:
                    files.Add(new BA2FileEntry(this, Version, idx, reader));
                    break;
                case Ba2EntryType.DX10:
                    files.Add(new BA2DX10Entry(this, Version, idx, reader));
                    break;
                case Ba2EntryType.GNMF:
                    break;

            }
        }

        if (HasNameTable)
        {
            reader.BaseStream.Seek((long)_nameTableOffset, SeekOrigin.Begin);
            foreach (var file in files)
            {
                switch (file)
                {
                    case BA2FileEntry ba2:
                        ba2.Path = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt16()));
                        break;
                    case BA2DX10Entry dx:
                        dx.Path = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt16()));
                        break;
                    default:
                        throw new NotImplementedException();
                }

            }
        }
        Files = files;
    }

    public IEnumerable<IArchiveFile> Files { get; private set; }

    public bool TryGetFolder(string path, [MaybeNullWhen(false)] out IArchiveFolder folder)
    {
        folder = new Ba2FolderWrapper(
            path,
            Files.Where(f => f.Path.ToString().StartsWith(path, StringComparison.OrdinalIgnoreCase)));
        return folder.Files.Count > 0;
    }
}

class BA2DX10Entry : IArchiveFile
{
    internal uint _nameHash;
    internal string _extension;
    internal uint _dirHash;
    internal byte _unk8;
    internal byte _numChunks;
    internal ushort _chunkHdrLen;
    internal ushort _height;
    internal ushort _width;
    internal byte _numMips;
    internal byte _format;
    internal ushort _unk16;
    internal List<BA2TextureChunk> _chunks;
    private Ba2Reader _bsa;
    internal int _index;

    public BA2DX10Entry(Ba2Reader ba2Reader, uint version, int idx, BinaryReader reader)
    {
        _bsa = ba2Reader;
        _nameHash = reader.ReadUInt32();
        Path = _nameHash.ToString("X");
        if (version > 1)
        {
            var unknown = reader.ReadUInt32();
        }
        _extension = Encoding.UTF8.GetString(reader.ReadBytes(4));
        _dirHash = reader.ReadUInt32();
        _unk8 = reader.ReadByte();
        _numChunks = reader.ReadByte();
        _chunkHdrLen = reader.ReadUInt16();
        _height = reader.ReadUInt16();
        _width = reader.ReadUInt16();
        _numMips = reader.ReadByte();
        _format = reader.ReadByte();
        _unk16 = reader.ReadUInt16();
        _index = idx;
        if (version > 1)
        {
            var unknown = reader.ReadUInt32();
        }

        _chunks = Enumerable.Range(0, _numChunks)
            .Select(_ => new BA2TextureChunk(reader, version))
            .ToList();
    }

    public string Path { get; internal set; }

    public uint Size => (uint)_chunks.Sum(f => f._fullSz) + HeaderSize + sizeof(uint);

    public uint HeaderSize => DDS.HeaderSizeForFormat((DXGI_FORMAT)_format);

    public Stream AsStream()
    {
        // ToDo
        // Optimize to be more streamy, rather than frontload into memory
        var ret = new byte[Size];
        var memStream = new MemoryStream(ret);
        CopyDataTo(memStream);
        memStream.Position = 0;
        return memStream;
    }

    public void CopyDataTo(Stream output)
    {
        var bw = new BinaryWriter(output);

        WriteHeader(bw);

        using var fs = _bsa._streamFactory();
        using var br = new BinaryReader(fs);
        foreach (var chunk in _chunks)
        {
            var full = new byte[chunk._fullSz];
            var isCompressed = chunk._packSz != 0;

            br.BaseStream.Seek((long)chunk._offset, SeekOrigin.Begin);

            if (!isCompressed)
            {
                br.BaseStream.Read(full, 0, full.Length);
            }
            else
            {
                byte[] compressed = new byte[chunk._packSz];
                br.BaseStream.Read(compressed, 0, compressed.Length);
                var inflater = new Inflater();
                inflater.SetInput(compressed);
                inflater.Inflate(full);
            }

            bw.BaseStream.Write(full, 0, full.Length);
        }
    }

    public byte[] GetBytes()
    {
        var ret = new byte[Size];
        var memStream = new MemoryStream(ret);
        CopyDataTo(memStream);
        return ret;
    }

    public ReadOnlySpan<byte> GetSpan()
    {
        return GetBytes();
    }

    public ReadOnlyMemorySlice<byte> GetMemorySlice()
    {
        return GetBytes();
    }

    private void WriteHeader(BinaryWriter bw)
    {
        var ddsHeader = new DDS_HEADER();

        ddsHeader.dwSize = ddsHeader.GetSize();
        ddsHeader.dwHeaderFlags = DDS.DDS_HEADER_FLAGS_TEXTURE | DDS.DDS_HEADER_FLAGS_LINEARSIZE | DDS.DDS_HEADER_FLAGS_MIPMAP;
        ddsHeader.dwHeight = _height;
        ddsHeader.dwWidth = _width;
        ddsHeader.dwMipMapCount = _numMips;
        ddsHeader.PixelFormat.dwSize = ddsHeader.PixelFormat.GetSize();
        ddsHeader.dwDepth = 1;
        ddsHeader.dwSurfaceFlags = DDS.DDS_SURFACE_FLAGS_TEXTURE | DDS.DDS_SURFACE_FLAGS_MIPMAP;

        switch ((DXGI_FORMAT)_format)
        {
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '1');
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height / 2); // 4bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '3');
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height); // 8bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', 'T', '5');
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height); // 8bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                if (_bsa.UseATIFourCC)
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('A', 'T', 'I', '2'); // this is more correct but the only thing I have found that supports it is the nvidia photoshop plugin
                else
                    ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('B', 'C', '5', 'U');
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height); // 8bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', '1', '0');
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height / 2); // 4bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_FOURCC;
                ddsHeader.PixelFormat.dwFourCC = DDS.MAKEFOURCC('D', 'X', '1', '0');
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height); // 8bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_RGBA;
                ddsHeader.PixelFormat.dwRGBBitCount = 32;
                ddsHeader.PixelFormat.dwRBitMask = 0x000000FF;
                ddsHeader.PixelFormat.dwGBitMask = 0x0000FF00;
                ddsHeader.PixelFormat.dwBBitMask = 0x00FF0000;
                ddsHeader.PixelFormat.dwABitMask = 0xFF000000;
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height * 4); // 32bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_RGBA;
                ddsHeader.PixelFormat.dwRGBBitCount = 32;
                ddsHeader.PixelFormat.dwRBitMask = 0x00FF0000;
                ddsHeader.PixelFormat.dwGBitMask = 0x0000FF00;
                ddsHeader.PixelFormat.dwBBitMask = 0x000000FF;
                ddsHeader.PixelFormat.dwABitMask = 0xFF000000;
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height * 4); // 32bpp
                break;
            case DXGI_FORMAT.DXGI_FORMAT_R8_UNORM:
                ddsHeader.PixelFormat.dwFlags = DDS.DDS_RGB;
                ddsHeader.PixelFormat.dwRGBBitCount = 8;
                ddsHeader.PixelFormat.dwRBitMask = 0xFF;
                ddsHeader.dwPitchOrLinearSize = (uint)(_width * _height); // 8bpp
                break;
            default:
                throw new Exception("Unsupported DDS header format. File: " + Path);
        }

        bw.Write((uint)DDS.DDS_MAGIC);
        ddsHeader.Write(bw);

        switch ((DXGI_FORMAT)_format)
        {
            case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
            case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
            case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                var dxt10 = new DDS_HEADER_DXT10()
                {
                    dxgiFormat = _format,
                    resourceDimension = (uint)DXT10_RESOURCE_DIMENSION.DIMENSION_TEXTURE2D,
                    miscFlag = 0,
                    arraySize = 1,
                    miscFlags2 = DDS.DDS_ALPHA_MODE_UNKNOWN
                };
                dxt10.Write(bw);
                break;
        }
    }
}

class BA2TextureChunk
{
    internal readonly ulong _offset;
    internal readonly uint _packSz;
    internal readonly uint _fullSz;
    internal readonly ushort _startMip;
    internal readonly ushort _endMip;
    internal readonly uint _align;

    public BA2TextureChunk(BinaryReader rdr, uint version)
    {
        _offset = rdr.ReadUInt64();
        _packSz = rdr.ReadUInt32();
        _fullSz = rdr.ReadUInt32();
        if (version <= 1)
        {
            // Unsure if these are actually the fields that are missing in v2, just a guess
            _startMip = rdr.ReadUInt16();
            _endMip = rdr.ReadUInt16();
            _align = rdr.ReadUInt32();
        }
    }
}

class BA2FileEntry : IArchiveFile
{
    internal readonly uint _nameHash;
    internal readonly string _extension;
    internal readonly uint _dirHash;
    internal readonly uint _flags;
    internal readonly ulong _offset;
    internal readonly uint _size;
    internal readonly uint _realSize;
    internal readonly uint _align;
    internal readonly Ba2Reader _bsa;
    internal readonly int _index;
    internal readonly uint _version;

    public bool Compressed => _size != 0;

    public BA2FileEntry(Ba2Reader ba2Reader, uint version, int index, BinaryReader reader)
    {
        _index = index;
        _bsa = ba2Reader;
        _version = version;
        _nameHash = reader.ReadUInt32();
        Path = _nameHash.ToString("X");
        if (version > 1)
        {
            var unknown = reader.ReadUInt32();
        }
        _extension = Encoding.UTF8.GetString(reader.ReadBytes(4));
        _dirHash = reader.ReadUInt32();
        _flags = reader.ReadUInt32();
        _offset = reader.ReadUInt64();
        _size = reader.ReadUInt32();
        _realSize = reader.ReadUInt32();
        if (version <= 1)
        {
            _align = reader.ReadUInt32();
        }
    }
        
    public string Path { get; internal set; }

    public uint Size => _realSize;

    public Stream AsStream()
    {
        var fs = _bsa._streamFactory();
        fs.Seek((long)_offset, SeekOrigin.Begin);
        uint len = Compressed ? _size : _realSize;

        if (!Compressed)
        {
            return new FramedStream(fs, len);
        }
        else
        {
            return new FramedStream(
                new InflaterInputStream(fs)
                {
                    IsStreamOwner = true
                }, 
                _realSize,
                doubleCheckLength: false);
        }
    }

    public byte[] GetBytes()
    {
        using var s = AsStream();
        byte[] ret = new byte[s.Remaining()];
        s.Read(ret);
        return ret;
    }

    public ReadOnlySpan<byte> GetSpan()
    {
        return GetBytes();
    }

    public ReadOnlyMemorySlice<byte> GetMemorySlice()
    {
        return GetBytes();
    }
}