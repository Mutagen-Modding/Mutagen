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
    public struct MutagenFrame : IDisposable, IBinaryStream
    {
        public static bool ErrorOnFinalPosition;

        public readonly IBinaryStream Reader;
        public readonly long InitialPosition;
        public readonly long FinalLocation;
        public readonly bool SnapToFinalPosition;

        public bool Complete => this.Position >= this.FinalLocation;
        public long Position
        {
            get => this.Reader.Position;
            set => this.Reader.Position = value;
        }
        public long TotalLength => this.FinalLocation - this.InitialPosition;
        public long Remaining => this.FinalLocation - this.Position;

        public long Length => Reader.Length;

        [DebuggerStepThrough]
        public MutagenFrame(IBinaryStream reader)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = reader.Length;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        private MutagenFrame(
            IBinaryStream reader,
            long finalPosition,
            bool snapToFinalPosition = true)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = finalPosition;
            this.SnapToFinalPosition = snapToFinalPosition;
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
                    ex = new ArgumentException($"Frame was complete, so did not have any remaining bytes to parse. At {this.Position}. Desired {length} more bytes. {this.Remaining} past the final position {this.FinalLocation}.");
                    return false;
                }
                else
                {
                    ex = new ArgumentException($"Frame did not have enough remaining bytes to parse. At {this.Position}. Desired {length} more bytes.  Only {this.Remaining} left before final position {this.FinalLocation}.");
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
            if (this.SnapToFinalPosition
                && this.Reader.Position != FinalLocation)
            {
                if (ErrorOnFinalPosition)
                {
                    var err = $"Did not read expected amount of bytes. Position: {this.Reader.Position}, Expected: {this.FinalLocation}";
                    this.Reader.Position = this.FinalLocation;
                    throw new ArgumentException(err);
                }
                else
                {
                    this.Reader.Position = this.FinalLocation;
                }
            }
        }

        public byte[] ReadRemaining()
        {
            return this.Reader.ReadBytes(checked((int)this.Remaining));
        }

        public override string ToString()
        {
            return $"0x{this.Position.ToString("X")} - 0x{(this.FinalLocation - 1).ToString("X")} (0x{this.Remaining.ToString("X")})";
        }

        [DebuggerStepThrough]
        public static MutagenFrame ByFinalPosition(
            IBinaryStream reader,
            long finalPosition,
            bool snapToFinalPosition = true)
        {
            return new MutagenFrame(
                reader: reader,
                finalPosition: finalPosition,
                snapToFinalPosition: snapToFinalPosition);
        }

        [DebuggerStepThrough]
        public static MutagenFrame ByLength(
            IBinaryStream reader,
            long length,
            bool snapToFinalPosition = true)
        {
            return new MutagenFrame(
                reader: reader,
                finalPosition: reader.Position + length,
                snapToFinalPosition: snapToFinalPosition);
        }

        [DebuggerStepThrough]
        public MutagenFrame SpawnWithFinalPosition(long finalPosition)
        {
            return new MutagenFrame(
                this.Reader,
                finalPosition);
        }

        [DebuggerStepThrough]
        public MutagenFrame SpawnWithLength(long length, bool snapToFinalPosition = true)
        {
            return new MutagenFrame(
                this.Reader,
                this.Reader.Position + length,
                snapToFinalPosition: snapToFinalPosition);
        }

        [DebuggerStepThrough]
        public MutagenFrame Spawn(bool snapToFinalPosition)
        {
            return new MutagenFrame(
                this.Reader,
                this.FinalLocation,
                snapToFinalPosition);
        }

        public MutagenFrame Decompress()
        { 
            var resultLen = this.Reader.ReadUInt32();
            var bytes = this.Reader.ReadBytes((int)this.Remaining);
            try
            {
                var res = ZlibStream.UncompressBuffer(bytes);
                return new MutagenFrame(
                    new BinaryMemoryStream(res));
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

        public string ReadString(int amount)
        {
            return Reader.ReadString(amount);
        }
    }
}
