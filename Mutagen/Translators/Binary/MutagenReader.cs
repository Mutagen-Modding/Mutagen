using Mutagen.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class MutagenReader : IDisposable
    {
        private System.IO.BinaryReader reader;

        public FileLocation Position
        {
            get => new FileLocation(this.reader.BaseStream.Position);
            set => this.reader.BaseStream.Position = value;
        }

        public FileLocation Length => new FileLocation(this.reader.BaseStream.Length);
        public bool Complete => this.Position < this.Length;

        public MutagenReader(string path)
        {
            this.reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        }

        public MutagenReader(Stream stream)
        {
            this.reader = new BinaryReader(stream);
        }

        public MutagenReader(System.IO.BinaryReader reader)
        {
            this.reader = reader;
        }

        public bool ReadBoolean()
        {
            return this.reader.ReadBoolean();
        }

        public byte[] ReadBytes(int count)
        {
            return this.reader.ReadBytes(count);
        }

        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        public ushort ReadUInt16()
        {
            return this.reader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return this.reader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return this.reader.ReadUInt64();
        }

        public sbyte ReadSByte()
        {
            return this.reader.ReadSByte();
        }

        public short ReadInt16()
        {
            return this.reader.ReadInt16();
        }

        public int ReadInt32()
        {
            return this.reader.ReadInt32();
        }

        public long ReadInt64()
        {
            return this.reader.ReadInt64();
        }

        public float ReadFloat()
        {
            return this.reader.ReadSingle();
        }

        public double ReadDouble()
        {
            return this.reader.ReadDouble();
        }

        public char ReadChar()
        {
            return this.reader.ReadChar();
        }

        public void ReadInto(byte[] b)
        {
            this.reader.Read(b, 0, b.Length);
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }
    }
}
