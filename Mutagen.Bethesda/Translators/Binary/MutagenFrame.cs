using Ionic.Zlib;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public struct MutagenFrame : IDisposable
    {
        public static bool ErrorOnFinalPosition;

        public readonly MutagenReader Reader;
        public readonly FileLocation InitialPosition;
        public readonly FileLocation FinalLocation;
        public readonly bool SnapToFinalPosition;

        public bool Complete => this.Position >= this.FinalLocation;
        public FileLocation Position
        {
            get => this.Reader.Position;
            set => this.Reader.Position = value;
        }
        public ContentLength TotalLength => this.FinalLocation - this.InitialPosition;
        public ContentLength RemainingLength => this.FinalLocation - this.Position;

        [DebuggerStepThrough]
        public MutagenFrame(MutagenReader reader)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = reader.Length;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            FileLocation finalPosition,
            bool snapToFinalPosition = true)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = finalPosition;
            this.SnapToFinalPosition = snapToFinalPosition;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            ContentLength length,
            bool snapToFinalPosition = true)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalLocation = reader.Position + length;
            this.SnapToFinalPosition = snapToFinalPosition;
        }

        public bool TryCheckUpcomingRead(ContentLength length)
        {
            return this.Position + length <= this.FinalLocation;
        }

        public void CheckUpcomingRead(ContentLength length)
        {
            if (!TryCheckUpcomingRead(length, out var ex))
            {
                throw ex;
            }
        }

        public bool TryCheckUpcomingRead(ContentLength length, out Exception ex)
        {
            if (!TryCheckUpcomingRead(length))
            {
                if (Complete)
                {
                    ex = new ArgumentException($"Frame was complete, so did not have any remaining bytes to parse. At {this.Position}. Desired {length} more bytes. {this.RemainingLength} past the final position {this.FinalLocation}.");
                    return false;
                }
                else
                {
                    ex = new ArgumentException($"Frame did not have enough remaining bytes to parse. At {this.Position}. Desired {length} more bytes.  Only {this.RemainingLength} left before final position {this.FinalLocation}.");
                    return false;
                }
            }
            ex = null;
            return true;
        }

        public bool ContainsPosition(FileLocation loc)
        {
            return this.Position <= loc && this.FinalLocation >= loc;
        }

        public void SetPosition(FileLocation pos)
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
            return this.Reader.ReadBytes(this.RemainingLength);
        }

        public override string ToString()
        {
            return $"{this.Position} - {this.FinalLocation - 1} ({this.RemainingLength})";
        }

        [DebuggerStepThrough]
        public MutagenFrame SpawnWithFinalPosition(FileLocation finalPosition)
        {
            return new MutagenFrame(
                this.Reader,
                finalPosition);
        }

        [DebuggerStepThrough]
        public MutagenFrame SpawnWithLength(ContentLength length, bool snapToFinalPosition = true)
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
            var bytes = this.Reader.ReadBytes((int)this.RemainingLength.Value);
            var res = ZlibStream.UncompressBuffer(bytes);
            return new MutagenFrame(
                new MutagenReader(res));
        }
    }
}
