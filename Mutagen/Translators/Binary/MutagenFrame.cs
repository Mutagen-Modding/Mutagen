using Mutagen.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public struct MutagenFrame : IDisposable
    {
        public static bool ErrorOnFinalPosition;
        public readonly MutagenReader Reader;
        public readonly FileLocation FinalPosition;
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
        }

        public MutagenFrame(
            MutagenReader reader,
            FileLocation finalPosition)
        {
            this.Reader = reader;
            this.FinalPosition = finalPosition;
        }

        public bool TryCheckUpcomingRead(ContentLength length)
        {
            return this.Position + length <= this.FinalPosition;
        }

        public void CheckUpcomingRead(ContentLength length)
        {
            if (!TryCheckUpcomingRead(length))
            {
                throw new ArgumentException($"Frame did not have enough remaining bytes to parse. At {this.Position}. Desired {length} more bytes.  Only {this.FinalPosition - this.Position} left before final position {this.FinalPosition}.");
            }
        }

        public void Dispose()
        {
            if (this.Reader.Position != FinalPosition)
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
    }
}
