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
        public readonly FileLocation FinalPosition;
        public readonly bool SnapToFinalPosition;

        public bool Complete => this.Position >= this.FinalPosition;
        public FileLocation Position
        {
            get => this.Reader.Position;
            set => this.Reader.Position = value;
        }
        public ContentLength TotalLength => this.FinalPosition - this.InitialPosition;
        public ContentLength RemainingLength => this.FinalPosition - this.Position;

        [DebuggerStepThrough]
        public MutagenFrame(MutagenReader reader)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalPosition = reader.Length;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            FileLocation finalPosition)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalPosition = finalPosition;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            ContentLength length)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalPosition = reader.Position + length;
            this.SnapToFinalPosition = true;
        }

        [DebuggerStepThrough]
        public MutagenFrame(
            MutagenReader reader,
            FileLocation finalPosition,
            bool snapToFinalPosition)
        {
            this.Reader = reader;
            this.InitialPosition = reader.Position;
            this.FinalPosition = finalPosition;
            this.SnapToFinalPosition = snapToFinalPosition;
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

        public byte[] ReadRemaining()
        {
            return this.Reader.ReadBytes(this.RemainingLength);
        }

        public override string ToString()
        {
            return $"{this.Position} - {this.FinalPosition - 1} ({this.RemainingLength})";
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

        public MutagenFrame Decompress()
        {
            var resultLen = this.Reader.ReadUInt32();
            var bytes = this.Reader.ReadBytes((int)this.RemainingLength.Value);
            var res = ZlibStream.UncompressBuffer(bytes);
            return new MutagenFrame(
                new MutagenReader(
                    new BinaryReader(
                        new MemoryStream(res))));
        }
    }
}
