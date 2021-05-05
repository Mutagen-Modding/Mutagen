using Noggog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Pex.Binary.Translations
{
    public record PexWriter(IBinaryWriteStream Writer) : IBinaryWriteStream
    {
        public long SizePosition;

        public readonly Dictionary<string, ushort> Strings = new();
        private ushort _next;

        public long Position { get => Writer.Position; set => Writer.Position = value; }

        public long Length => Writer.Length;

        public bool IsLittleEndian => Writer.IsLittleEndian;

        public Stream BaseStream => Writer.BaseStream;

        public ushort RegisterString(string str)
        {
            if (Strings.TryGetValue(str, out var index)) return index;
            index = _next++;
            Strings[str] = index;
            return index;
        }

        public void Write(string? str)
        {
            if (str == null) return;
            Writer.Write(RegisterString(str));
        }

        public void Write(int value)
        {
            Writer.Write(value);
        }

        public void Write(uint value)
        {
            Writer.Write(value);
        }

        public void Write(byte value)
        {
            Writer.Write(value);
        }

        public void Write(ushort value)
        {
            Writer.Write(value);
        }

        public void Write(DateTime dt)
        {
            Writer.Write(dt.ToUInt64());
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public void Write(bool value)
        {
            Writer.Write(value);
        }

        public void Write(ulong value)
        {
            Writer.Write(value);
        }

        public void Write(sbyte value)
        {
            Writer.Write(value);
        }

        public void Write(short value)
        {
            Writer.Write(value);
        }

        public void Write(long value)
        {
            Writer.Write(value);
        }

        public void Write(float value)
        {
            Writer.Write(value);
        }

        public void Write(double value)
        {
            Writer.Write(value);
        }

        public void Write(ReadOnlySpan<char> value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Writer.Dispose();
        }
    }
}
