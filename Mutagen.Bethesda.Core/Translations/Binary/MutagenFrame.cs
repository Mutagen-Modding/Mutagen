using Ionic.Zlib;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Struct representing a begin and end location within a stream.
    /// It does not enforce reading within those bounds, but simply acts as a reference marker.
    /// It also implements input stream interfaces, which will read from the source stream.
    /// </summary>
    public struct MutagenFrame : IMutagenReadStream
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
        public bool Complete => this.Position >= this.FinalLocation;

        /// <inheritdoc/>
        public long Position
        {
            get => this.Reader.Position;
            set => this.Reader.Position = value;
        }
        
        /// <inheritdoc/>
        public long PositionWithOffset => this.Position + this.Reader.OffsetReference;
        
        /// <inheritdoc/>
        public long FinalWithOffset => this.FinalLocation + this.Reader.OffsetReference;
        
        /// <inheritdoc/>
        public long TotalLength => this.FinalLocation - this.InitialPosition;
        
        /// <inheritdoc/>
        public long Remaining => this.FinalLocation - this.Position;

        /// <inheritdoc/>
        public long Length => Reader.Length;

        /// <inheritdoc/>
        public long OffsetReference => this.Reader.OffsetReference;

        /// <inheritdoc/>
        public ReadOnlySpan<byte> RemainingSpan => this.Reader.RemainingSpan;
        
        /// <inheritdoc/>
        public ReadOnlyMemorySlice<byte> RemainingMemory => this.Reader.RemainingMemory;

        /// <inheritdoc/>
        public GameConstants MetaData => this.Reader.MetaData;
        
        /// <inheritdoc/>
        public MasterReferenceReader? MasterReferences
        {
            get => this.Reader.MasterReferences;
            set => this.Reader.MasterReferences = value;
        }

        /// <summary>
        /// Constructs new frame around current reader position until its completion
        /// </summary>
        [DebuggerStepThrough]
        public MutagenFrame(IMutagenReadStream reader)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = reader.Length;
        }

        [DebuggerStepThrough]
        private MutagenFrame(
            IMutagenReadStream reader,
            long finalPosition)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = finalPosition;
        }

        /// <inheritdoc/>
        public bool TryCheckUpcomingRead(long length)
        {
            return this.Position + length <= this.FinalLocation;
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
                    ex = new ArgumentException($"Frame was complete, so did not have any remaining bytes to parse. At {this.PositionWithOffset}. Desired {length} more bytes. {this.Remaining} past the final position {this.FinalWithOffset}.");
                    return false;
                }
                else
                {
                    ex = new ArgumentException($"Frame did not have enough remaining bytes to parse. At {this.PositionWithOffset}. Desired {length} more bytes.  Only {this.Remaining} left before final position {this.FinalWithOffset}.");
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
            return this.Position <= loc && this.FinalLocation >= loc;
        }

        /// <inheritdoc/>
        public void SetPosition(long pos)
        {
            this.Position = pos;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void SetToFinalPosition()
        {
            this.Reader.Position = this.FinalLocation;
        }

        /// <inheritdoc/>
        public byte[] ReadRemainingBytes()
        {
            return this.Reader.ReadBytes(checked((int)this.Remaining));
        }

        /// <inheritdoc/>
        public ReadOnlySpan<byte> ReadRemainingSpan(bool readSafe)
        {
            return this.Reader.ReadSpan(checked((int)this.Remaining), readSafe: readSafe);
        }

        /// <inheritdoc/>
        public ReadOnlyMemorySlice<byte> ReadRemainingMemory(bool readSafe)
        {
            return this.Reader.ReadMemory(checked((int)this.Remaining), readSafe: readSafe);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"0x{this.PositionWithOffset.ToString("X")} - 0x{(this.FinalWithOffset - 1).ToString("X")} (0x{this.Remaining.ToString("X")})";
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
                this.Reader,
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
                && this.Remaining < length)
            {
                throw new ArgumentException($"Frame did not have enough remaining to allocate for desired length at {this.PositionWithOffset}. Desired {length} more bytes, but only had {this.Remaining}.");
            }
            return new MutagenFrame(
                this.Reader,
                this.Reader.Position + length);
        }

        /// <summary>
        /// Decompresses frame content into a new backing stream.
        /// Will read an integer to determine how large the compressed data is, and will read that amount.
        /// </summary>
        /// <returns>New frame with a new backing stream with uncompressed content</returns>
        public MutagenFrame Decompress()
        {
            return Decompress(this.Reader.ReadUInt32());
        }

        /// <summary>
        /// Decompresses into a new backing stream
        /// </summary>
        /// <param name="resultLen">Number of bytes to read for decompression</param>
        /// <returns>New frame with a new backing stream with uncompressed content</returns>
        public MutagenFrame Decompress(uint resultLen)
        {
            // ToDo
            // Swap to span version if Zlib updates
            var bytes = this.Reader.ReadBytes((int)this.Remaining);
            try
            {
                var res = ZlibStream.UncompressBuffer(bytes);
                return new MutagenFrame(
                    new MutagenMemoryReadStream(res, this.MetaData, this.MasterReferences));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Reads some bytes and reframes them into a new frame with a new backing stream.
        /// </summary>
        /// <param name="length">Amount of bytes to read from source frame</param>
        /// <returns>New frame with a new backing stream</returns>
        public MutagenFrame ReadAndReframe(int length)
        {
            var offset = this.PositionWithOffset;
            return new MutagenFrame(new MutagenMemoryReadStream(this.ReadMemory(length, readSafe: true), this.MetaData, this.MasterReferences, offsetReference: offset));
        }

        /// <inheritdoc/>
        IMutagenReadStream IMutagenReadStream.ReadAndReframe(int length) => this.ReadAndReframe(length);

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
        public bool ReadBool()
        {
            return Reader.ReadBool();
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
        public bool ReadBoolean()
        {
            return Reader.ReadUInt8() == 1;
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
        public bool GetBool(int offset)
        {
            return Reader.GetBool(offset);
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
        public bool GetBool()
        {
            return Reader.GetBool();
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
}
