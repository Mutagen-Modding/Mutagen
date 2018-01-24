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
    public class MutagenWriter : IDisposable
    {
        private bool dispose = true;
        private System.IO.BinaryWriter writer;
        private static byte Zero = 0;

        public FileLocation Position
        {
            get => new FileLocation(this.writer.BaseStream.Position);
            set => this.writer.BaseStream.Position = value;
        }

        public FileLocation Length
        {
            get => new FileLocation(this.writer.BaseStream.Length);
        }

        public MutagenWriter(string path)
        {
            this.writer = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
        }

        public MutagenWriter(Stream stream, bool dispose = true)
        {
            this.dispose = dispose;
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

        public void WriteZeros(uint num)
        {
            for (uint i = 0; i < num; i++)
            {
                this.Write(Zero);
            }
        }

        public void Write(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                var c = str[i];
                bytes[i] = (byte)c;
            }
            this.writer.Write(bytes);
        }

        public void Write(Color color)
        {
            this.writer.Write(color.R);
            this.writer.Write(color.G);
            this.writer.Write(color.B);
            this.writer.Write(byte.MinValue);
        }

        public void Dispose()
        {
            if (dispose)
            {
                this.writer.Dispose();
            }
        }
    }
}
