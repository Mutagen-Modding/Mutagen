using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public struct MutagenFrame : IDisposable
    {
        public static bool ErrorOnFinalPosition;

        public readonly MutagenReader Reader;
        public readonly FileLocation FinalPosition;
        public readonly bool SnapToFinalPosition;

        public bool Complete => this.Position >= this.FinalPosition;
        public FileLocation Position
        {
            get => this.Reader.Position;
            set => this.Reader.Position = value;
        }
        public ContentLength Length => this.FinalPosition - this.Position;

        [DebuggerStepThrough]
        public MutagenFrame(MutagenReader reader)
        {
            this.Reader = reader;
            this.FinalPosition = reader.Length;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            FileLocation finalPosition)
        {
            this.Reader = reader;
            this.FinalPosition = finalPosition;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            FileLocation finalPosition,
            bool snapToFinalPosition)
        {
            this.Reader = reader;
            this.FinalPosition = finalPosition;
            this.SnapToFinalPosition = snapToFinalPosition;
        }

        public bool TryCheckUpcomingRead(ContentLength length)
        {
            return this.Position + length <= this.FinalPosition;
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
                    ex = new ArgumentException($"Frame was complete, so did not have any remaining bytes to parse. At {this.Position}. Desired {length} more bytes. {this.Length} past the final position {this.FinalPosition}.");
                    return false;
                }
                else
                {
                    ex = new ArgumentException($"Frame did not have enough remaining bytes to parse. At {this.Position}. Desired {length} more bytes.  Only {this.Length} left before final position {this.FinalPosition}.");
                    return false;
                }
            }
            ex = null;
            return true;
        }

        public bool ContainsPosition(FileLocation loc)
        {
            return this.Position <= loc && this.FinalPosition >= loc;
        }

        public void Dispose()
        {
            if (this.SnapToFinalPosition 
                && this.Reader.Position != FinalPosition)
            {
                if (ErrorOnFinalPosition)
                {
                    var err = $"Did not read expected amount of bytes. Position: {this.Reader.Position}, Expected: {this.FinalPosition}";
                    this.Reader.Position = this.FinalPosition;
                    throw new ArgumentException(err);
                }
                else
                {
                    this.Reader.Position = this.FinalPosition;
                }
            }
        }

        public override string ToString()
        {
            return $"{this.Position} - {this.FinalPosition - 1} ({this.Length})";
        }

        [DebuggerStepThrough]
        public MutagenFrame Spawn(FileLocation finalPosition)
        {
            return new MutagenFrame(
                this.Reader,
                finalPosition);
        }

        [DebuggerStepThrough]
        public MutagenFrame Spawn(ContentLength length)
        {
            return new MutagenFrame(
                this.Reader,
                this.Reader.Position + length);
        }

        [DebuggerStepThrough]
        public MutagenFrame Spawn(bool snapToFinalPosition)
        {
            return new MutagenFrame(
                this.Reader,
                this.FinalPosition,
                snapToFinalPosition);
        }
    }
}
