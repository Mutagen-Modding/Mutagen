using System;
using System.Buffers.Binary;
using System.IO;

namespace Mutagen.Bethesda.Pex
{
    internal static class BinaryReaderExtensions
    {
        internal static string ReadWStringLE(this BinaryReader br)
        {
            return br.ReadWStringAsSpan(false).ToString();
        }

        internal static string ReadWStringBE(this BinaryReader br)
        {
            return br.ReadWStringAsSpan(true).ToString();
        }
        
        internal static ReadOnlySpan<char> ReadWStringAsSpan(this BinaryReader br, bool isBigEndian)
        {
            var length = isBigEndian ? br.ReadUInt16BE() : br.ReadUInt16();
            if (length == 0) return ReadOnlySpan<char>.Empty;
            var chars = br.ReadChars(length);
            return chars.AsSpan();
        }

        internal static ushort ReadUInt16BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(ushort));
            return BinaryPrimitives.ReadUInt16BigEndian(bytes);
        }

        internal static uint ReadUInt32BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(uint));
            return BinaryPrimitives.ReadUInt32BigEndian(bytes);
        }

        internal static ulong ReadUInt64BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(ulong));
            return BinaryPrimitives.ReadUInt64BigEndian(bytes);
        }

        internal static int ReadInt32BE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            return BinaryPrimitives.ReadInt32BigEndian(bytes);
        }

        internal static float ReadSingleBE(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(float));
            return BinaryPrimitives.ReadSingleBigEndian(bytes);
        }
    }
}
