using Mutagen.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class MutagenWriter : IDisposable
    {
        private System.IO.BinaryWriter writer;

        public FileLocation Position
        {
            get => this.writer.BaseStream.Position;
            set => this.writer.BaseStream.Position = value;
        }

        public FileLocation Length
        {
            get => this.writer.BaseStream.Length;
        }

        public MutagenWriter(string path)
        {
            this.writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
        }

        public MutagenWriter(Stream stream)
        {
            this.writer = new BinaryWriter(stream);
        }

        public MutagenWriter(System.IO.BinaryWriter reader)
        {
            this.writer = reader;
        }

        public void Write(bool b)
        {
            this.writer.Write(b);
        }

        public void Write(byte b)
        {
            this.writer.Write(b);
        }

        public void Write(byte[] b)
        {
            this.writer.Write(b);
        }

        public void Write(ushort b)
        {
            this.writer.Write(b);
        }

        public void Write(uint b)
        {
            this.writer.Write(b);
        }

        public void Write(ulong b)
        {
            this.writer.Write(b);
        }

        public void Write(sbyte s)
        {
            this.writer.Write(s);
        }

        public void Write(short s)
        {
            this.writer.Write(s);
        }

        public void Write(int i)
        {
            this.writer.Write(i);
        }

        public void Write(long i)
        {
            this.writer.Write(i);
        }

        public void Write(float i)
        {
            this.writer.Write(i);
        }

        public void Write(double i)
        {
            this.writer.Write(i);
        }

        public void Write(char c)
        {
            this.writer.Write(c);
        }

        public void Write(char[] c)
        {
            this.writer.Write(c);
        }

        public void Dispose()
        {
            this.writer.Dispose();
        }
    }
}
