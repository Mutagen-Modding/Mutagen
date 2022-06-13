using Noggog;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Binary.Streams;

/// <summary>
/// Struct representing a begin and end location within a stream.
/// It does not enforce reading within those bounds, but simply acts as a reference marker.
/// It also implements input stream interfaces, which will read from the source stream.
/// </summary>
public readonly struct MutagenFrame : IMutagenReadStream
{
    /// <summary>
    /// Associated reader
    /// </summary>
    public readonly IMutagenReadStream Reader;
        
    /// <summary>
    /// Starting point for the frame
    /// </summary>
    public readonly long InitialPosition;
        
    /// <summary>
    /// First position not included in the frame
    /// </summary>
    public readonly long FinalLocation;

    /// <summary>
    /// Whether the frame's contents have been read.
    /// Associated reader might have more content
    /// </summary>
    public bool Complete => Position >= FinalLocation;

    /// <inheritdoc/>
    public long Position
    {
        get => Reader.Position;
        set => Reader.Position = value;
    }
        
    /// <inheritdoc/>
    public long PositionWithOffset => Position + Reader.OffsetReference;
        
    /// <inheritdoc/>
    public long FinalWithOffset => FinalLocation + Reader.OffsetReference;
        
    /// <inheritdoc/>
    public long TotalLength => FinalLocation - InitialPosition;
        
    /// <inheritdoc/>
    public long Remaining => FinalLocation - Position;

    /// <inheritdoc/>
    public long Length => Reader.Length;

    /// <inheritdoc/>
    public long OffsetReference => Reader.OffsetReference;

    /// <inheritdoc/>
    public ReadOnlySpan<byte> RemainingSpan => Reader.RemainingSpan;
        
    /// <inheritdoc/>
    public ReadOnlyMemorySlice<byte> RemainingMemory => Reader.RemainingMemory;

    /// <inheritdoc/>
    public ParsingBundle MetaData => Reader.MetaData;

    public bool IsLittleEndian => Reader.IsLittleEndian;

    public bool IsPersistantBacking => Reader.IsPersistantBacking;

    public Stream BaseStream => Reader.BaseStream;

    /// <summary>
    /// Constructs new frame around current reader position until its completion
    /// </summary>
    [DebuggerStepThrough]
    public MutagenFrame(IMutagenReadStream reader)
    {
        Reader = reader;
        InitialPosition = reader.Position;
        FinalLocation = reader.Length;
    }

    [DebuggerStepThrough]
    private MutagenFrame(
        IMutagenReadStream reader,
        long finalPosition)
    {
        Reader = reader;
        InitialPosition = reader.Position;
        FinalLocation = finalPosition;
    }

    /// <inheritdoc/>
    public bool TryCheckUpcomingRead(long length)
    {
        return Position + length <= FinalLocation;
    }

    /// <inheritdoc/>
    public void CheckUpcomingRead(long length)
    {
        if (!TryCheckUpcomingRead(length, out var ex))
        {
            throw ex;
        }
    }

    /// <inheritdoc/>
    public bool TryCheckUpcomingRead(long length, [MaybeNullWhen(true)]out Exception ex)
    {
        if (!TryCheckUpcomingRead(length))
        {
            if (Complete)
            {
                ex = new ArgumentException($"Frame was complete, so did not have any remaining bytes to parse. At {PositionWithOffset}. Desired {length} more bytes. {Remaining} past the final position {FinalWithOffset}.");
                return false;
            }
            else
            {
                ex = new ArgumentException($"Frame did not have enough remaining bytes to parse. At {PositionWithOffset}. Desired {length} more bytes.  Only {Remaining} left before final position {FinalWithOffset}.");
                return false;
            }
        }
        ex = default!;
        return true;
    }

    /// <summary>
    /// Checks if given reader location is within the frame
    /// </summary>
    /// <param name="loc">Location to query</param>
    /// <returns>True if location within frame's region</returns>
    public bool ContainsPosition(long loc)
    {
        return Position <= loc && FinalLocation >= loc;
    }

    /// <inheritdoc/>
    public void SetPosition(long pos)
    {
        Position = pos;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public void SetToFinalPosition()
    {
        Reader.Position = FinalLocation;
    }

    /// <inheritdoc/>
    public byte[] ReadRemainingBytes()
    {
        return Reader.ReadBytes(checked((int)Remaining));
    }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> ReadRemainingSpan(bool readSafe)
    {
        return Reader.ReadSpan(checked((int)Remaining), readSafe: readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlyMemorySlice<byte> ReadRemainingMemory(bool readSafe)
    {
        return Reader.ReadMemory(checked((int)Remaining), readSafe: readSafe);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"0x{PositionWithOffset.ToString("X")} - 0x{(FinalWithOffset - 1).ToString("X")} (0x{Remaining.ToString("X")})";
    }

    /// <summary>
    /// Creates a frame around reader with set final position
    /// </summary>
    /// <param name="reader">Reader to frame</param>
    /// <param name="finalPosition">Absolute position to be marked as frame end</param>
    /// <returns>Frame with current position as start, and given final position</returns>
    [DebuggerStepThrough]
    public static MutagenFrame ByFinalPosition(
        IMutagenReadStream reader,
        long finalPosition)
    {
        return new MutagenFrame(
            reader: reader,
            finalPosition: finalPosition);
    }

    /// <summary>
    /// Creates a frame around reader of a given length
    /// </summary>
    /// <param name="reader">Reader to frame</param>
    /// <param name="length">Size of frame</param>
    /// <returns>Frame with current position as start, and given length</returns>
    [DebuggerStepThrough]
    public static MutagenFrame ByLength(
        IMutagenReadStream reader,
        long length)
    {
        return new MutagenFrame(
            reader: reader,
            finalPosition: reader.Position + length);
    }

    /// <summary>
    /// Creates a new frame around reader with set final position
    /// </summary>
    /// <param name="finalPosition">Absolute position to be marked as frame end</param>
    /// <returns>Frame with current position as start, and given final position</returns>
    [DebuggerStepThrough]
    public MutagenFrame SpawnWithFinalPosition(long finalPosition)
    {
        return new MutagenFrame(
            Reader,
            finalPosition);
    }
        
    /// <summary>
    /// Creates a new frame around reader of a given length
    /// </summary>
    /// <param name="length">Size of frame</param>
    /// <param name="checkFraming">Whethr to do a check that frame doesn't exceed reader's final position</param>
    /// <returns>Frame with current position as start, and given length</returns>
    /// <exception cref="ArgumentException">If checkFraming is true, and frame exceeds reader's final position</exception>
    public MutagenFrame SpawnWithLength(long length, bool checkFraming = true)
    {
        if (checkFraming 
            && Remaining < length)
        {
            throw new ArgumentException($"Frame did not have enough remaining to allocate for desired length at {PositionWithOffset}. Desired {length} more bytes, but only had {Remaining}.");
        }
        return new MutagenFrame(
            Reader,
            Reader.Position + length);
    }

    public MutagenFrame SpawnAll()
    {
        return new MutagenFrame(Reader, Reader.Length);
    }

    /// <summary>
    /// Decompresses frame content into a new backing stream.
    /// Will read an integer to determine how large the compressed data is, and will read that amount.
    /// </summary>
    /// <returns>New frame with a new backing stream with uncompressed content</returns>
    public MutagenFrame Decompress()
    {
        var resultLen = Reader.ReadUInt32();
        var bytes = Reader.ReadBytes((int)Remaining);
        var res = Decompression.Decompress(bytes, resultLen);
        return new MutagenFrame(
            new MutagenMemoryReadStream(res, MetaData));
    }

    /// <summary>
    /// Reads some bytes and reframes them into a new frame with a new backing stream.
    /// </summary>
    /// <param name="length">Amount of bytes to read from source frame</param>
    /// <returns>New frame with a new backing stream</returns>
    public MutagenFrame ReadAndReframe(int length)
    {
        var offset = PositionWithOffset;
        return new MutagenFrame(
            new MutagenMemoryReadStream(
                ReadMemory(length, readSafe: true),
                MetaData,
                offsetReference: offset));
    }

    /// <inheritdoc/>
    IMutagenReadStream IMutagenReadStream.ReadAndReframe(int length) => ReadAndReframe(length);

    /// <inheritdoc/>
    public int Read(byte[] buffer, int offset, int amount)
    {
        return Reader.Read(buffer, offset, amount);
    }

    /// <inheritdoc/>
    public int Read(byte[] buffer)
    {
        return Reader.Read(buffer);
    }

    /// <inheritdoc/>
    public byte[] ReadBytes(int amount)
    {
        return Reader.ReadBytes(amount);
    }

    /// <inheritdoc/>
    public bool ReadBoolean()
    {
        return Reader.ReadBoolean();
    }

    /// <inheritdoc/>
    public byte ReadUInt8()
    {
        return Reader.ReadUInt8();
    }

    /// <inheritdoc/>
    public ushort ReadUInt16()
    {
        return Reader.ReadUInt16();
    }

    /// <inheritdoc/>
    public uint ReadUInt32()
    {
        return Reader.ReadUInt32();
    }

    /// <inheritdoc/>
    public ulong ReadUInt64()
    {
        return Reader.ReadUInt64();
    }

    /// <inheritdoc/>
    public sbyte ReadInt8()
    {
        return Reader.ReadInt8();
    }

    /// <inheritdoc/>
    public short ReadInt16()
    {
        return Reader.ReadInt16();
    }

    /// <inheritdoc/>
    public int ReadInt32()
    {
        return Reader.ReadInt32();
    }

    /// <inheritdoc/>
    public long ReadInt64()
    {
        return Reader.ReadInt64();
    }

    /// <inheritdoc/>
    public float ReadFloat()
    {
        return Reader.ReadFloat();
    }

    /// <inheritdoc/>
    public double ReadDouble()
    {
        return Reader.ReadDouble();
    }

    /// <inheritdoc/>
    public string ReadStringUTF8(int amount)
    {
        return Reader.ReadStringUTF8(amount);
    }

    /// <inheritdoc/>
    public void WriteTo(Stream stream, int amount)
    {
        Reader.WriteTo(stream, amount);
    }

    /// <inheritdoc/>
    public int Get(byte[] buffer, int offset, int amount)
    {
        return Reader.Get(buffer, offset, amount);
    }

    /// <inheritdoc/>
    public byte[] GetBytes(int amount)
    {
        return Reader.GetBytes(amount);
    }

    /// <inheritdoc/>
    public int Get(byte[] buffer, int offset)
    {
        return Reader.Get(buffer, offset);
    }

    /// <inheritdoc/>
    public bool GetBoolean(int offset)
    {
        return Reader.GetBoolean(offset);
    }

    /// <inheritdoc/>
    public byte GetUInt8(int offset)
    {
        return Reader.GetUInt8(offset);
    }

    /// <inheritdoc/>
    public ushort GetUInt16(int offset)
    {
        return Reader.GetUInt16(offset);
    }

    /// <inheritdoc/>
    public uint GetUInt32(int offset)
    {
        return Reader.GetUInt32(offset);
    }

    /// <inheritdoc/>
    public ulong GetUInt64(int offset)
    {
        return Reader.GetUInt64(offset);
    }

    /// <inheritdoc/>
    public sbyte GetInt8(int offset)
    {
        return Reader.GetInt8(offset);
    }

    /// <inheritdoc/>
    public short GetInt16(int offset)
    {
        return Reader.GetInt16(offset);
    }

    /// <inheritdoc/>
    public int GetInt32(int offset)
    {
        return Reader.GetInt32(offset);
    }

    /// <inheritdoc/>
    public long GetInt64(int offset)
    {
        return Reader.GetInt64(offset);
    }

    /// <inheritdoc/>
    public float GetFloat(int offset)
    {
        return Reader.GetFloat(offset);
    }

    /// <inheritdoc/>
    public double GetDouble(int offset)
    {
        return Reader.GetDouble(offset);
    }

    /// <inheritdoc/>
    public string GetStringUTF8(int amount, int offset)
    {
        return Reader.GetStringUTF8(amount, offset);
    }

    /// <inheritdoc/>
    public bool GetBoolean()
    {
        return Reader.GetBoolean();
    }

    /// <inheritdoc/>
    public byte GetUInt8()
    {
        return Reader.GetUInt8();
    }

    /// <inheritdoc/>
    public ushort GetUInt16()
    {
        return Reader.GetUInt16();
    }

    /// <inheritdoc/>
    public uint GetUInt32()
    {
        return Reader.GetUInt32();
    }

    /// <inheritdoc/>
    public ulong GetUInt64()
    {
        return Reader.GetUInt64();
    }

    /// <inheritdoc/>
    public sbyte GetInt8()
    {
        return Reader.GetInt8();
    }

    /// <inheritdoc/>
    public short GetInt16()
    {
        return Reader.GetInt16();
    }

    /// <inheritdoc/>
    public int GetInt32()
    {
        return Reader.GetInt32();
    }

    /// <inheritdoc/>
    public long GetInt64()
    {
        return Reader.GetInt64();
    }

    /// <inheritdoc/>
    public float GetFloat()
    {
        return Reader.GetFloat();
    }

    /// <inheritdoc/>
    public double GetDouble()
    {
        return Reader.GetDouble();
    }

    /// <inheritdoc/>
    public string GetStringUTF8(int amount)
    {
        return Reader.GetStringUTF8(amount);
    }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> ReadSpan(int amount, bool readSafe = true)
    {
        return Reader.ReadSpan(amount, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> ReadSpan(int amount, int offset, bool readSafe = true)
    {
        return Reader.ReadSpan(amount, offset, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> GetSpan(int amount, bool readSafe = true)
    {
        return Reader.GetSpan(amount, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> GetSpan(int amount, int offset, bool readSafe = true)
    {
        return Reader.GetSpan(amount, offset, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, bool readSafe = true)
    {
        return Reader.ReadMemory(amount, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset, bool readSafe = true)
    {
        return Reader.ReadMemory(amount, offset, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlyMemorySlice<byte> GetMemory(int amount, bool readSafe = true)
    {
        return Reader.GetMemory(amount, readSafe);
    }

    /// <inheritdoc/>
    public ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset, bool readSafe = true)
    {
        return Reader.GetMemory(amount, offset, readSafe);
    }
}