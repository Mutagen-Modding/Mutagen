using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Binary
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
        /// <param name="binaryType">Format to read the color as</param>
        /// <returns>Bytes converted to a Color object</returns>
        public static Color ReadColor(this IBinaryReadStream stream, ColorBinaryType binaryType)
        {
            switch (binaryType)
            {
                case ColorBinaryType.NoAlpha:
                    return ReadColor(stream.ReadSpan(3), binaryType);
                case ColorBinaryType.Alpha:
                    return ReadColor(stream.ReadSpan(4), binaryType);
                case ColorBinaryType.NoAlphaFloat:
                    return ReadColor(stream.ReadSpan(12), binaryType);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Extracts a color from binary span. 
        /// If the span more than 3 bytes, the 4th byte will be interpreted as alpha.
        /// Will throw an exception if there is not at least 3 bytes.
        /// </summary>
        /// <param name="span">Span to read from</param>
        /// <param name="binaryType">Format to read the color as</param>
        /// <returns>Bytes converted to a Color object</returns>
        public static Color ReadColor(this ReadOnlySpan<byte> span, ColorBinaryType binaryType)
        {
            switch (binaryType)
            {
                case ColorBinaryType.NoAlpha:
                    return Color.FromArgb(
                        alpha: 0,
                        red: span[0],
                        green: span[1],
                        blue: span[2]);
                case ColorBinaryType.Alpha:
                    return Color.FromArgb(
                        alpha: span.Length >= 4 ? span[3] : 0,
                        red: span[0],
                        green: span[1],
                        blue: span[2]);
                case ColorBinaryType.NoAlphaFloat:
                    return Color.FromArgb(
                        alpha: 0,
                        red: checked((byte)SpanExt.GetFloat(span.Slice(0, 4))),
                        green: checked((byte)SpanExt.GetFloat(span.Slice(4, 8))),
                        blue: checked((byte)SpanExt.GetFloat(span.Slice(8, 12))));
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Extracts a color from binary span. 
        /// If the span more than 3 bytes, the 4th byte will be interpreted as alpha.
        /// Will throw an exception if there is not at least 3 bytes.
        /// </summary>
        /// <param name="span">Span to read from</param>
        /// <returns>Bytes converted to a Color object</returns>
        public static Color ReadColor(this ReadOnlyMemorySlice<byte> span, ColorBinaryType binaryType)
        {
            return span.Span.ReadColor(binaryType);
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
