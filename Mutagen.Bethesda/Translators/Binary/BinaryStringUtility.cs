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
        public unsafe static string ToBethesdaString(ReadOnlySpan<byte> bytes)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            ToBethesdaStringBuffer(bytes, chars);
            return chars.ToString();
        }

        public static void ToBethesdaStringBuffer(ReadOnlySpan<byte> bytes, Span<char> temporaryCharBuffer)
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

    }
}
