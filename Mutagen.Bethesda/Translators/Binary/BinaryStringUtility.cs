using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class BinaryStringUtility
    {
        // ToDo
        // Utilize string.Create when it comes out
        public unsafe static string ToZString(ReadOnlySpan<byte> bytes)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            ToZStringBuffer(bytes, chars);
            return chars.ToString();
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
            if (bytes[bytes.Length - 1] == 0)
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

        public static string ParseUnknownLengthString(IBinaryReadStream stream)
        {
            var span = stream.RemainingSpan;
            var index = span.IndexOf(default(byte));
            if (index == -1)
            {
                throw new ArgumentException();
            }
            stream.Position += index + 1;
            return BinaryStringUtility.ToZString(span.Slice(0, index));
        }
    }
}
