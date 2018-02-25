using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mutagen.Bethesda.Binary
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
        public bool Complete => this.Position >= this.Length;
        public FileLocation FinalLocation => new FileLocation(this.reader.BaseStream.Length);
        public ContentLength RemainingLength => this.FinalLocation - this.Position;

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

        public MutagenReader(byte[] bytes)
            : this(new BinaryReader(
                new MemoryStream(bytes)))
        {
        }

        public bool ReadBoolean()
        {
            return this.reader.ReadBoolean();
        }

        public byte[] ReadBytes(int count)
        {
            return this.reader.ReadBytes(count);
        }

        public static String BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }
            return new string(chars);
        }

        public string ReadString(int count)
        {
            return BytesToString(this.reader.ReadBytes(count));
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

        public Color ReadColor()
        {
            var ret = Color.FromRgb(
                this.reader.ReadByte(),
                this.reader.ReadByte(),
                this.reader.ReadByte());
            return ret;
        }

        public void ReadInto(byte[] b)
        {
            this.reader.Read(b, 0, b.Length);
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

        public override string ToString()
        {
            return $"MutagenReader ({this.Position}-{this.Length})";
        }

        public void Dispose()
        {
            this.reader.Dispose();
        }
    }
}
