using Mutagen.Bethesda.Binary;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class BinaryStringUtility
    {
        // ToDo
        // Can string.Create be used on spans?
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

        public static void ToZStringBuffer(ReadOnlySpan<byte> bytes, Span<char> temporaryCharBuffer)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                temporaryCharBuffer[i] = (char)bytes[i];
            }
        }

        public static ReadOnlySpan<byte> ProcessNullTermination(ReadOnlySpan<byte> bytes)
        {
            // If null terminated, don't include
            if (bytes[^1] == 0)
            {
                return bytes.Slice(0, bytes.Length - 1);
            }
            return bytes;
        }

        public static ReadOnlyMemorySlice<byte> ProcessNullTermination(ReadOnlyMemorySlice<byte> bytes)
        {
            // If null terminated, don't include
            if (bytes[^1] == 0)
            {
                return bytes.Slice(0, bytes.Length - 1);
            }
            return bytes;
        }

        public static string ProcessWholeToZString(ReadOnlySpan<byte> span)
        {
            span = ProcessNullTermination(span);
            return ToZString(span);
        }

        public static string ProcessWholeToZString(ReadOnlyMemorySlice<byte> span)
        {
            span = ProcessNullTermination(span);
            return ToZString(span);
        }

        public static string ParseUnknownLengthString(IBinaryReadStream stream)
        {
            var mem = stream.RemainingMemory;
            var index = mem.Span.IndexOf(default(byte));
            if (index == -1)
            {
                throw new ArgumentException();
            }
            var ret = BinaryStringUtility.ToZString(mem.Slice(0, index));
            stream.Position += index + 1;
            return ret;
        }
    }
}
