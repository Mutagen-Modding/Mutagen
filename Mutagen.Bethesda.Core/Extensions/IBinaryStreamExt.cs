using Noggog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Extension functions adding Mutagen specific parsing functionality
    /// </summary>
    public static class IBinaryStreamExt
    {
        /// <summary>
        /// Reads a color from the binary stream.
        /// The stream will be advanced 4 bytes (or 3 if only 3 remain). 
        /// If the stream has more than 3 bytes, the 4th byte will be interpreted as alpha.
        /// Will throw an exception if there is not at least 3 bytes remaining.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>Bytes converted to a Color object</returns>
        public static Color ReadColor(this IBinaryReadStream stream)
        {
            return ReadColor(stream.ReadSpan(stream.Remaining >= 4 ? 4 : 3));
        }

        /// <summary>
        /// Extracts a color from binary span. 
        /// If the span more than 3 bytes, the 4th byte will be interpreted as alpha.
        /// Will throw an exception if there is not at least 3 bytes.
        /// </summary>
        /// <param name="span">Span to read from</param>
        /// <returns>Bytes converted to a Color object</returns>
        public static Color ReadColor(this ReadOnlySpan<byte> span)
        {
            return Color.FromArgb(
                alpha: span.Length >= 4 ? span[3] : 0,
                red: span[0],
                green: span[1],
                blue: span[2]);
        }

        /// <summary>
        /// Extracts a color from binary span. 
        /// If the span more than 3 bytes, the 4th byte will be interpreted as alpha.
        /// Will throw an exception if there is not at least 3 bytes.
        /// </summary>
        /// <param name="span">Span to read from</param>
        /// <returns>Bytes converted to a Color object</returns>
        public static Color ReadColor(this ReadOnlyMemorySlice<byte> span)
        {
            return span.Span.ReadColor();
        }

        /// <summary>
        /// Reads a ZString from the binary stream
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="length">Length of the zstring</param>
        /// <returns>ZString of desired length</returns>
        public static string ReadZString(this IBinaryReadStream stream, int length)
        {
            return BinaryStringUtility.ToZString(stream.ReadMemory(length));
        }
    }
}
