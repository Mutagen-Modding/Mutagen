using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Noggog.Streams.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal class OverlayStream : IMutagenReadStream
{
    private BinaryMemoryReadStructStream _readStream;
    public ParsingBundle MetaData { get; }

    public ReadOnlyMemorySlice<byte> Data { get; private set; }
    public int Position
    {
        get => _readStream.PositionInt;
        set => _readStream.PositionInt = value;
    }
    long IBinaryReadStream.Position
    {
        get => _readStream.Position;
        set => _readStream.Position = checked((int)value);
    }

    public int Length => _readStream.Length;

    public bool IsLittleEndian => _readStream.IsLittleEndian;

    public OverlayStream(ReadOnlyMemorySlice<byte> data, ParsingBundle constants)
    {
        Data = data;
        _readStream = new BinaryMemoryReadStructStream(data);
        MetaData = constants;
    }

    public ReadOnlyMemorySlice<byte> Read(int amount)
    {
        Position += amount;
        return Data.Slice(Position - amount, amount);
    }

    #region IMutagenReadStream
    public long OffsetReference => 0;

    IMutagenReadStream IMutagenReadStream.ReadAndReframe(int length)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region IBinaryReadStream
    long IBinaryReadStream.Length => _readStream.Length;

    public long Remaining => _readStream.Remaining;

    public bool Complete => _readStream.Complete;

    public ReadOnlySpan<byte> RemainingSpan => _readStream.RemainingSpan;

    public ReadOnlyMemorySlice<byte> RemainingMemory => _readStream.RemainingMemory;

    public bool IsPersistantBacking => true;

    public void Dispose()
    {
        _readStream.Dispose();
    }

    public int Get(byte[] buffer, int targetOffset, int amount)
    {
        return _readStream.Get(buffer, targetOffset, amount);
    }

    public int Get(byte[] buffer, int targetOffset)
    {
        return _readStream.Get(buffer, targetOffset);
    }

    public bool GetBoolean()
    {
        return _readStream.GetBoolean();
    }

    public bool GetBoolean(int offset)
    {
        return _readStream.GetBoolean(offset);
    }

    public byte[] GetBytes(int amount)
    {
        return _readStream.GetBytes(amount);
    }

    public double GetDouble()
    {
        return _readStream.GetDouble();
    }

    public double GetDouble(int offset)
    {
        return _readStream.GetDouble(offset);
    }

    public float GetFloat()
    {
        return _readStream.GetFloat();
    }

    public float GetFloat(int offset)
    {
        return _readStream.GetFloat(offset);
    }

    public short GetInt16()
    {
        return _readStream.GetInt16();
    }

    public short GetInt16(int offset)
    {
        return _readStream.GetInt16(offset);
    }

    public int GetInt32()
    {
        return _readStream.GetInt32();
    }

    public int GetInt32(int offset)
    {
        return _readStream.GetInt32(offset);
    }

    public long GetInt64()
    {
        return _readStream.GetInt64();
    }

    public long GetInt64(int offset)
    {
        return _readStream.GetInt64(offset);
    }

    public sbyte GetInt8()
    {
        return _readStream.GetInt8();
    }

    public sbyte GetInt8(int offset)
    {
        return _readStream.GetInt8(offset);
    }

    public ReadOnlyMemorySlice<byte> GetMemory(int amount, bool readSafe = true)
    {
        return _readStream.GetMemory(amount, readSafe);
    }

    public ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset, bool readSafe = true)
    {
        return _readStream.GetMemory(amount, offset, readSafe);
    }

    public ReadOnlySpan<byte> GetSpan(int amount, bool readSafe = true)
    {
        return _readStream.GetSpan(amount, readSafe);
    }

    public ReadOnlySpan<byte> GetSpan(int amount, int offset, bool readSafe = true)
    {
        return _readStream.GetSpan(amount, offset, readSafe);
    }

    public string GetStringUTF8(int amount)
    {
        return _readStream.GetStringUTF8(amount);
    }

    public string GetStringUTF8(int amount, int offset)
    {
        return _readStream.GetStringUTF8(amount, offset);
    }

    public ushort GetUInt16()
    {
        return _readStream.GetUInt16();
    }

    public ushort GetUInt16(int offset)
    {
        return _readStream.GetUInt16(offset);
    }

    public uint GetUInt32()
    {
        return _readStream.GetUInt32();
    }

    public uint GetUInt32(int offset)
    {
        return _readStream.GetUInt32(offset);
    }

    public ulong GetUInt64()
    {
        return _readStream.GetUInt64();
    }

    public ulong GetUInt64(int offset)
    {
        return _readStream.GetUInt64(offset);
    }

    public byte GetUInt8()
    {
        return _readStream.GetUInt8();
    }

    public byte GetUInt8(int offset)
    {
        return _readStream.GetUInt8(offset);
    }

    public int Read(byte[] buffer, int offset, int amount)
    {
        return _readStream.Read(buffer, offset, amount);
    }

    public int Read(byte[] buffer)
    {
        return _readStream.Read(buffer);
    }

    public bool ReadBoolean()
    {
        return _readStream.ReadBoolean();
    }

    public byte[] ReadBytes(int amount)
    {
        return _readStream.ReadBytes(amount);
    }

    public double ReadDouble()
    {
        return _readStream.ReadDouble();
    }

    public float ReadFloat()
    {
        return _readStream.ReadFloat();
    }

    public short ReadInt16()
    {
        return _readStream.ReadInt16();
    }

    public int ReadInt32()
    {
        return _readStream.ReadInt32();
    }

    public long ReadInt64()
    {
        return _readStream.ReadInt64();
    }

    public sbyte ReadInt8()
    {
        return _readStream.ReadInt8();
    }

    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, bool readSafe = true)
    {
        return _readStream.ReadMemory(amount, readSafe);
    }

    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset, bool readSafe = true)
    {
        return _readStream.ReadMemory(amount, offset, readSafe);
    }

    public ReadOnlySpan<byte> ReadSpan(int amount, bool readSafe = true)
    {
        return _readStream.ReadSpan(amount, readSafe);
    }

    public ReadOnlySpan<byte> ReadSpan(int amount, int offset, bool readSafe = true)
    {
        return _readStream.ReadSpan(amount, offset, readSafe);
    }

    public string ReadStringUTF8(int amount)
    {
        return _readStream.ReadStringUTF8(amount);
    }

    public ushort ReadUInt16()
    {
        return _readStream.ReadUInt16();
    }

    public uint ReadUInt32()
    {
        return _readStream.ReadUInt32();
    }

    public ulong ReadUInt64()
    {
        return _readStream.ReadUInt64();
    }

    public byte ReadUInt8()
    {
        return _readStream.ReadUInt8();
    }

    public void CopyTo(Stream stream, int amount)
    {
        _readStream.CopyTo(stream, amount);
    }

    public void CopyTo(Stream stream)
    {
        _readStream.CopyTo(stream);
    }
    #endregion
}