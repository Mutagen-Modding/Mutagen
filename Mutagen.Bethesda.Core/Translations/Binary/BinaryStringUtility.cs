using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Static class with string-related utility functions
    /// </summary>
    public static class BinaryStringUtility
    {
        /// <summary>
        /// Converts span to a string.  Should be slower than ToZString with ReadOnlyMemorySlice parameter.
        /// </summary>
        /// <param name="bytes">Bytes to turn into a string</param>
        /// <returns>string containing a character for every byte in the input span</returns>
        public static string ToZString(ReadOnlySpan<byte> bytes)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            ToZStringBuffer(bytes, chars);
            return chars.ToString();
        }

        /// <summary>
        /// Converts memory slice to a string.  Should be faster than ToZString with ReadOnlySpan parameter.
        /// </summary>
        /// <param name="bytes">Bytes to turn into a string</param>
        /// <returns>string containing a character for every byte in the input span</returns>
        public static string ToZString(ReadOnlyMemorySlice<byte> bytes)
        {
            if (bytes.Length <= 0) return string.Empty;
            return string.Create(bytes.Length, bytes, (chars, state) =>
            {
                for (int i = 0; i < state.Length; i++)
                {
                    chars[i] = (char)state[i];
                }
            });
        }

        /// <summary>
        /// Translates a span of bytes into a span of chars
        /// </summary>
        /// <param name="bytes">Bytes to convert to chars</param>
        /// <param name="temporaryCharBuffer">Char span to put bytes into</param>
        public static void ToZStringBuffer(ReadOnlySpan<byte> bytes, Span<char> temporaryCharBuffer)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                temporaryCharBuffer[i] = (char)bytes[i];
            }
        }

        /// <summary>
        /// Trims the last byte if it is 0.
        /// </summary>
        /// <param name="bytes">Bytes to trim</param>
        /// <returns>Trimmed bytes</returns>
        public static ReadOnlySpan<byte> ProcessNullTermination(ReadOnlySpan<byte> bytes)
        {
            // If null terminated, don't include
            if (bytes[^1] == 0)
            {
                return bytes[0..^1];
            }
            return bytes;
        }

        /// <summary>
        /// Trims the last byte if it is 0.
        /// </summary>
        /// <param name="bytes">Bytes to trim</param>
        /// <returns>Trimmed bytes</returns>
        public static ReadOnlyMemorySlice<byte> ProcessNullTermination(ReadOnlyMemorySlice<byte> bytes)
        {
            // If null terminated, don't include
            if (bytes[^1] == 0)
            {
                return bytes[0..^1];
            }
            return bytes;
        }

        /// <summary>
        /// Null trims and then processes bytes into a string
        /// </summary>
        /// <param name="bytes">Bytes to convert</param>
        /// <returns>String representation of bytes</returns>
        public static string ProcessWholeToZString(ReadOnlySpan<byte> bytes)
        {
            bytes = ProcessNullTermination(bytes);
            return ToZString(bytes);
        }

        /// <summary>
        /// Null trims and then processes bytes into a string
        /// </summary>
        /// <param name="bytes">Bytes to convert</param>
        /// <returns>String representation of bytes</returns>
        public static string ProcessWholeToZString(ReadOnlyMemorySlice<byte> bytes)
        {
            bytes = ProcessNullTermination(bytes);
            return ToZString(bytes);
        }

        /// <summary>
        /// Reads bytes from a stream until a null termination character occurs.
        /// Converts results to a string.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>First null terminated string read</returns>
        public static string ParseUnknownLengthString(IBinaryReadStream stream)
        {
            var mem = stream.RemainingMemory;
            var index = mem.Span.IndexOf(default(byte));
            if (index == -1)
            {
                throw new ArgumentException();
            }
            var ret = BinaryStringUtility.ToZString(mem[0..index]);
            stream.Position += index + 1;
            return ret;
        }

        /// <summary>
        /// Reads bytes from a stream until a null termination character occurs.
        /// Converts results to a string.
        /// </summary>
        /// <param name="bytes">Bytes to convert</param>
        /// <returns>First null terminated string read</returns>
        public static string ParseUnknownLengthString(ReadOnlySpan<byte> bytes)
        {
            var index = bytes.IndexOf(default(byte));
            if (index == -1)
            {
                throw new ArgumentException();
            }
            return BinaryStringUtility.ToZString(bytes[0..index]);
        }

        /// <summary>
        /// Read string of known length, which is prepended by bytes denoting its length.
        /// Converts results to a string.
        /// </summary>
        /// <param name="span">Bytes to retrieve string from</param>
        /// <param name="lengthLength">Amount of bytes containing length information</param>
        /// <returns>String of length denoted by initial bytes</returns>
        public static string ParsePrependedString(ReadOnlyMemorySlice<byte> span, byte lengthLength)
        {
            switch (lengthLength)
            {
                case 2:
                    {
                        var length = BinaryPrimitives.ReadUInt16LittleEndian(span);
                        return ProcessWholeToZString(span.Slice(2, length));
                    }
                case 4:
                    {
                        var length = BinaryPrimitives.ReadUInt32LittleEndian(span);
                        return ProcessWholeToZString(span.Slice(4, checked((int)length)));
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Read string of known length, which is prepended by bytes denoting its length.
        /// Converts results to a string.
        /// </summary>
        /// <param name="stream">Stream to retrieve string from</param>
        /// <param name="lengthLength">Amount of bytes containing length information</param>
        /// <returns>String of length denoted by initial bytes</returns>
        public static string ReadPrependedString(this IBinaryReadStream stream, byte lengthLength)
        {
            switch (lengthLength)
            {
                case 2:
                    {
                        var length = stream.ReadUInt16();
                        return ToZString(stream.ReadSpan(length));
                    }
                case 4:
                    {
                        var length = checked((int)stream.ReadUInt32());
                        return ToZString(stream.ReadSpan(length));
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
