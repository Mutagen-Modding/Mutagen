using Ionic.Zlib;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public struct MutagenFrame : IMutagenReadStream
    {
        public readonly IMutagenReadStream Reader;
        public readonly long InitialPosition;
        public readonly long FinalLocation;

        public bool Complete => this.Position >= this.FinalLocation;
        public long Position
        {
            get => this.Reader.Position;
            set => this.Reader.Position = value;
        }
        public long PositionWithOffset => this.Position + this.Reader.OffsetReference;
        public long FinalWithOffset => this.FinalLocation + this.Reader.OffsetReference;
        public long TotalLength => this.FinalLocation - this.InitialPosition;
        public long Remaining => this.FinalLocation - this.Position;

        public long Length => Reader.Length;

        public long OffsetReference => this.Reader.OffsetReference;

        public ReadOnlySpan<byte> RemainingSpan => this.Reader.RemainingSpan;

        public MetaDataConstants MetaData => this.Reader.MetaData;

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

        public bool TryCheckUpcomingRead(long length)
        {
            return this.Position + length <= this.FinalLocation;
        }

        public void CheckUpcomingRead(long length)
        {
            if (!TryCheckUpcomingRead(length, out var ex))
            {
                throw ex;
            }
        }

        public bool TryCheckUpcomingRead(long length, out Exception ex)
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
            ex = null;
            return true;
        }

        public bool ContainsPosition(long loc)
        {
            return this.Position <= loc && this.FinalLocation >= loc;
        }

        public void SetPosition(long pos)
        {
            this.Position = pos;
        }

        public void Dispose()
        {
        }

        public void SetToFinalPosition()
        {
            this.Reader.Position = this.FinalLocation;
        }

        public byte[] ReadRemaining()
        {
            return this.Reader.ReadBytes(checked((int)this.Remaining));
        }

        public override string ToString()
        {
            return $"0x{this.PositionWithOffset.ToString("X")} - 0x{(this.FinalWithOffset - 1).ToString("X")} (0x{this.Remaining.ToString("X")})";
        }

        [DebuggerStepThrough]
        public static MutagenFrame ByFinalPosition(
            IMutagenReadStream reader,
            long finalPosition)
        {
            return new MutagenFrame(
                reader: reader,
                finalPosition: finalPosition);
        }

        [DebuggerStepThrough]
        public static MutagenFrame ByLength(
            IMutagenReadStream reader,
            long length)
        {
            return new MutagenFrame(
                reader: reader,
                finalPosition: reader.Position + length);
        }

        [DebuggerStepThrough]
        public MutagenFrame SpawnWithFinalPosition(long finalPosition)
        {
            return new MutagenFrame(
                this.Reader,
                finalPosition);
        }
        
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

        public MutagenFrame Decompress()
        {
            return Decompress(this.Reader.ReadUInt32());
        }

        public MutagenFrame Decompress(uint resultLen)
        {
            var bytes = this.Reader.ReadBytes((int)this.Remaining);
            try
            {
                var res = ZlibStream.UncompressBuffer(bytes);
                return new MutagenFrame(
                    new MutagenMemoryReadStream(res, this.MetaData));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Read(byte[] buffer, int offset, int amount)
        {
            return Reader.Read(buffer, offset, amount);
        }

        public int Read(byte[] buffer)
        {
            return Reader.Read(buffer);
        }

        public byte[] ReadBytes(int amount)
        {
            return Reader.ReadBytes(amount);
        }

        public bool ReadBool()
        {
            return Reader.ReadBool();
        }

        public byte ReadUInt8()
        {
            return Reader.ReadUInt8();
        }

        public ushort ReadUInt16()
        {
            return Reader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return Reader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return Reader.ReadUInt64();
        }

        public sbyte ReadInt8()
        {
            return Reader.ReadInt8();
        }

        public short ReadInt16()
        {
            return Reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return Reader.ReadInt32();
        }

        public long ReadInt64()
        {
            return Reader.ReadInt64();
        }

        public float ReadFloat()
        {
            return Reader.ReadFloat();
        }

        public double ReadDouble()
        {
            return Reader.ReadDouble();
        }

        public string ReadStringUTF8(int amount)
        {
            return Reader.ReadStringUTF8(amount);
        }

        public void WriteTo(Stream stream, int amount)
        {
            Reader.WriteTo(stream, amount);
        }

        public int Get(byte[] buffer, int offset, int amount)
        {
            return Reader.Get(buffer, offset, amount);
        }

        public byte[] GetBytes(int amount)
        {
            return Reader.GetBytes(amount);
        }

        public int Get(byte[] buffer, int offset)
        {
            return Reader.Get(buffer, offset);
        }

        public bool GetBool(int offset)
        {
            return Reader.GetBool(offset);
        }

        public byte GetUInt8(int offset)
        {
            return Reader.GetUInt8(offset);
        }

        public ushort GetUInt16(int offset)
        {
            return Reader.GetUInt16(offset);
        }

        public uint GetUInt32(int offset)
        {
            return Reader.GetUInt32(offset);
        }

        public ulong GetUInt64(int offset)
        {
            return Reader.GetUInt64(offset);
        }

        public sbyte GetInt8(int offset)
        {
            return Reader.GetInt8(offset);
        }

        public short GetInt16(int offset)
        {
            return Reader.GetInt16(offset);
        }

        public int GetInt32(int offset)
        {
            return Reader.GetInt32(offset);
        }

        public long GetInt64(int offset)
        {
            return Reader.GetInt64(offset);
        }

        public float GetFloat(int offset)
        {
            return Reader.GetFloat(offset);
        }

        public double GetDouble(int offset)
        {
            return Reader.GetDouble(offset);
        }

        public string GetStringUTF8(int amount, int offset)
        {
            return Reader.GetStringUTF8(amount, offset);
        }

        public MutagenFrame ReadAndReframe(int length)
        {
            var offset = this.PositionWithOffset;
            return new MutagenFrame(new MutagenMemoryReadStream(this.ReadBytes(length), this.MetaData, offsetReference: offset));
        }

        IMutagenReadStream IMutagenReadStream.ReadAndReframe(int length) => this.ReadAndReframe(length);

        public bool GetBool()
        {
            return Reader.GetBool();
        }

        public byte GetUInt8()
        {
            return Reader.GetUInt8();
        }

        public ushort GetUInt16()
        {
            return Reader.GetUInt16();
        }

        public uint GetUInt32()
        {
            return Reader.GetUInt32();
        }

        public ulong GetUInt64()
        {
            return Reader.GetUInt64();
        }

        public sbyte GetInt8()
        {
            return Reader.GetInt8();
        }

        public short GetInt16()
        {
            return Reader.GetInt16();
        }

        public int GetInt32()
        {
            return Reader.GetInt32();
        }

        public long GetInt64()
        {
            return Reader.GetInt64();
        }

        public float GetFloat()
        {
            return Reader.GetFloat();
        }

        public double GetDouble()
        {
            return Reader.GetDouble();
        }

        public string GetStringUTF8(int amount)
        {
            return Reader.GetStringUTF8(amount);
        }

        public ReadOnlySpan<byte> ReadSpan(int amount)
        {
            return Reader.ReadSpan(amount);
        }

        public ReadOnlySpan<byte> ReadSpan(int amount, int offset)
        {
            return Reader.ReadSpan(amount, offset);
        }

        public ReadOnlySpan<byte> GetSpan(int amount)
        {
            return Reader.GetSpan(amount);
        }

        public ReadOnlySpan<byte> GetSpan(int amount, int offset)
        {
            return Reader.GetSpan(amount, offset);
        }

        public string ReadZString(int length)
        {
            return Reader.ReadZString(length);
        }
    }
}
