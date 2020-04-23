using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A wrapper around IBinaryWriter with extra Mutagen-specific reference data
    /// </summary>
    public class MutagenWriter : IDisposable
    {
        private readonly bool dispose = true;
        private const byte Zero = 0;
        
        /// <summary>
        /// Wrapped writer
        /// </summary>
        public System.IO.BinaryWriter Writer;
        
        /// <summary>
        /// Base stream that the writer wraps
        /// </summary>
        public Stream BaseStream { get; }
        
        /// <summary>
        /// Game metadata content to refer to for header information
        /// </summary>
        public GameConstants Meta { get; }
        
        /// <summary>
        /// Optional master references for easy access during write operations
        /// </summary>
        public MasterReferenceReader? MasterReferences { get; set; }

        /// <inheritdoc/>
        public long Position
        {
            get => this.BaseStream.Position;
            set => this.BaseStream.Position = value;
        }

        /// <inheritdoc/>
        public long Length
        {
            get => this.BaseStream.Length;
        }

        public MutagenWriter(string path, GameConstants meta, MasterReferenceReader? masterReferences = null)
        {
            this.BaseStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            this.Writer = new BinaryWriter(this.BaseStream);
            this.Meta = meta;
            this.MasterReferences = masterReferences;
        }

        public MutagenWriter(Stream stream, GameConstants meta, MasterReferenceReader? masterReferences = null, bool dispose = true)
        {
            this.dispose = dispose;
            this.BaseStream = stream;
            this.Writer = new BinaryWriter(stream);
            this.Meta = meta;
            this.MasterReferences = masterReferences;
        }

        public MutagenWriter(System.IO.BinaryWriter writer, GameConstants meta)
        {
            this.BaseStream = writer.BaseStream;
            this.Writer = writer;
            this.Meta = meta;
        }

        /// <inheritdoc/>
        public void Write(bool b)
        {
            this.Writer.Write(b);
        }

        /// <inheritdoc/>
        public void Write(bool? b)
        {
            if (!b.HasValue) return;
            this.Writer.Write(b.Value);
        }

        /// <inheritdoc/>
        public void Write(byte b)
        {
            this.Writer.Write(b);
        }

        /// <inheritdoc/>
        public void Write(byte? b)
        {
            if (!b.HasValue) return;
            this.Writer.Write(b.Value);
        }

        /// <inheritdoc/>
        public void Write(byte[]? b)
        {
            if (b == null) return;
            this.Writer.Write(b);
        }

        public void Write(ReadOnlyMemorySlice<byte> b)
        {
            this.Writer.Write(b.Span);
        }

        /// <inheritdoc/>
        public void Write(ReadOnlySpan<byte> b)
        {
            this.Writer.Write(b);
        }

        /// <inheritdoc/>
        public void Write(ushort b)
        {
            this.Writer.Write(b);
        }

        /// <inheritdoc/>
        public void Write(ushort? b)
        {
            if (!b.HasValue) return;
            this.Writer.Write(b.Value);
        }

        /// <inheritdoc/>
        public void Write(uint b)
        {
            this.Writer.Write(b);
        }

        /// <inheritdoc/>
        public void Write(uint? b)
        {
            if (!b.HasValue) return;
            this.Writer.Write(b.Value);
        }

        /// <inheritdoc/>
        public void Write(ulong b)
        {
            this.Writer.Write(b);
        }

        /// <inheritdoc/>
        public void Write(ulong? b)
        {
            if (!b.HasValue) return;
            this.Writer.Write(b.Value);
        }

        public void Write(sbyte s)
        {
            this.Writer.Write(s);
        }

        /// <inheritdoc/>
        public void Write(sbyte? s)
        {
            if (!s.HasValue) return;
            this.Writer.Write(s.Value);
        }

        /// <inheritdoc/>
        public void Write(short s)
        {
            this.Writer.Write(s);
        }

        /// <inheritdoc/>
        public void Write(short? s)
        {
            if (!s.HasValue) return;
            this.Writer.Write(s.Value);
        }

        /// <inheritdoc/>
        public void Write(int i)
        {
            this.Writer.Write(i);
        }

        /// <inheritdoc/>
        public void Write(int? i)
        {
            if (!i.HasValue) return;
            this.Writer.Write(i.Value);
        }

        /// <inheritdoc/>
        public void Write(long i)
        {
            this.Writer.Write(i);
        }

        /// <inheritdoc/>
        public void Write(long? i)
        {
            if (!i.HasValue) return;
            this.Writer.Write(i.Value);
        }

        /// <inheritdoc/>
        public void Write(float i)
        {
            this.Writer.Write(i);
        }

        /// <inheritdoc/>
        public void Write(float? i)
        {
            if (!i.HasValue) return;
            this.Writer.Write(i.Value);
        }

        /// <inheritdoc/>
        public void Write(double i)
        {
            this.Writer.Write(i);
        }

        /// <inheritdoc/>
        public void Write(double? i)
        {
            if (!i.HasValue) return;
            this.Writer.Write(i.Value);
        }

        /// <inheritdoc/>
        public void Write(char c)
        {
            this.Writer.Write(c);
        }

        public void Write(char? c)
        {
            if (!c.HasValue) return;
            this.Writer.Write(c.Value);
        }

        /// <inheritdoc/>
        public void WriteZeros(uint num)
        {
            for (uint i = 0; i < num; i++)
            {
                this.Write(Zero);
            }
        }
        /// <inheritdoc/>

        public void Write(ReadOnlySpan<char> str)
        {
            Span<byte> bytes = stackalloc byte[str.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                var c = str[i];
                bytes[i] = (byte)c;
            }
            this.Writer.Write(bytes);
        }

        /// <inheritdoc/>
        public void Write(string str, StringBinaryType binaryType)
        {
            switch (binaryType)
            {
                case StringBinaryType.Plain:
                    Write(str.AsSpan());
                    break;
                case StringBinaryType.NullTerminate:
                    Write(str.AsSpan());
                    this.Write((byte)0);
                    break;
                case StringBinaryType.PrependLength:
                    Write(str.Length);
                    Write(str.AsSpan());
                    break;
                case StringBinaryType.PrependLengthUShort:
                    Write(checked((ushort)str.Length));
                    Write(str.AsSpan());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public void Write(Color color, bool extraByte)
        {
            this.Writer.Write(color.R);
            this.Writer.Write(color.G);
            this.Writer.Write(color.B);
            if (extraByte)
            {
                this.Writer.Write(color.A);
            }
        }

        /// <summary>
        /// Disposes of Writer if applicable
        /// </summary>
        public void Dispose()
        {
            if (dispose)
            {
                this.Writer.Dispose();
            }
        }
    }
}
