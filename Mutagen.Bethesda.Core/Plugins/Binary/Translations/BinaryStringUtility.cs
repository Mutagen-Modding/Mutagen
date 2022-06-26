using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

/// <summary>
/// Static class with string-related utility functions
/// </summary>
public static class BinaryStringUtility
{
    /// <summary>
    /// Converts span to a string.
    /// </summary>
    /// <param name="bytes">Bytes to turn into a string</param>
    /// <param name="encoding">Encoding to use</param>
    /// <returns>string containing a character for every byte in the input span</returns>
    public static string ToZString(ReadOnlySpan<byte> bytes, IMutagenEncoding encoding)
    {
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// Trims the last byte if it is 0.
    /// </summary>
    /// <param name="bytes">Bytes to trim</param>
    /// <returns>Trimmed bytes</returns>
    public static ReadOnlySpan<byte> ProcessNullTermination(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0) return bytes;
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
    /// <param name="encoding">Encoding to use</param>
    /// <returns>String representation of bytes</returns>
    public static string ProcessWholeToZString(ReadOnlySpan<byte> bytes, IMutagenEncoding encoding)
    {
        bytes = ProcessNullTermination(bytes);
        return ToZString(bytes, encoding);
    }

    /// <summary>
    /// Reads bytes from a stream until a null termination character occurs.
    /// Converts results to a string.
    /// </summary>
    /// <param name="stream">Stream to read from</param>
    /// <param name="encoding">Stream to read from</param>
    /// <returns>First null terminated string read</returns>
    public static string ParseUnknownLengthString<TReader>(TReader stream, IMutagenEncoding encoding)
        where TReader : IBinaryReadStream
    {
        var mem = stream.RemainingMemory;
        var index = mem.Span.IndexOf(default(byte));
        if (index == -1)
        {
            throw new ArgumentException();
        }
        var ret = ToZString(mem[0..index], encoding);
        stream.Position += index + 1;
        return ret;
    }

    /// <summary>
    /// Reads bytes from a stream until a null termination character occurs.
    /// Converts results to a string.
    /// </summary>
    /// <param name="bytes">Bytes to convert</param>
    /// <param name="encoding">Encoding to use</param>
    /// <returns>First null terminated string read</returns>
    public static string ParseUnknownLengthString(ReadOnlySpan<byte> bytes, IMutagenEncoding encoding)
    {
        return ToZString(ExtractUnknownLengthString(bytes), encoding);
    }

    /// <summary>
    /// Reads bytes from a stream until a null termination character occurs.
    /// </summary>
    /// <param name="bytes">Bytes to convert</param>
    /// <returns>Initial span of bytes up until the first null byte</returns>
    public static ReadOnlySpan<byte> ExtractUnknownLengthString(ReadOnlySpan<byte> bytes)
    {
        var index = bytes.IndexOf(default(byte));
        if (index == -1)
        {
            throw new ArgumentException();
        }
        return bytes[..index];
    }

    /// <summary>
    /// Read string of known length, which is prepended by bytes denoting its length.
    /// Converts results to a string.
    /// </summary>
    /// <param name="span">Bytes to retrieve string from</param>
    /// <param name="lengthLength">Amount of bytes containing length information</param>
    /// <param name="encoding">Encoding to use</param>
    /// <returns>String of length denoted by initial bytes</returns>
    public static string ParsePrependedString(ReadOnlySpan<byte> span, byte lengthLength, IMutagenEncoding encoding)
    {
        return ProcessWholeToZString(ExtractPrependedString(span, lengthLength), encoding);
    }

    /// <summary>
    /// Read string of known length, which is prepended by bytes denoting its length.
    /// Converts results to a string.
    /// </summary>
    /// <param name="span">Bytes to retrieve string from</param>
    /// <param name="lengthLength">Amount of bytes containing length information</param>
    /// <returns>String of length denoted by initial bytes</returns>
    public static ReadOnlySpan<byte> ExtractPrependedString(ReadOnlySpan<byte> span, byte lengthLength)
    {
        switch (lengthLength)
        {
            case 2:
            {
                var length = BinaryPrimitives.ReadUInt16LittleEndian(span);
                return span.Slice(2, length);
            }
            case 4:
            {
                var length = BinaryPrimitives.ReadUInt32LittleEndian(span);
                return span.Slice(4, checked((int)length));
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
    /// <param name="encoding">Encoding to use</param>
    /// <returns>String of length denoted by initial bytes</returns>
    public static string ReadPrependedString<TStream>(this TStream stream, byte lengthLength, IMutagenEncoding encoding)
        where TStream : IBinaryReadStream
    {
        switch (lengthLength)
        {
            case 2:
                {
                    var length = stream.ReadUInt16();
                    return ToZString(stream.ReadSpan(length), encoding);
                }
            case 4:
                {
                    var length = checked((int)stream.ReadUInt32());
                    return ToZString(stream.ReadSpan(length), encoding);
                }
            default:
                throw new NotImplementedException();
        }
    }

    public static void Write<TStream>(this TStream stream, ReadOnlySpan<char> str, StringBinaryType binaryType, IMutagenEncoding encoding)
        where TStream : IBinaryWriteStream
    {
        switch (binaryType)
        {
            case StringBinaryType.Plain:
                Write(stream, str, encoding);
                break;
            case StringBinaryType.NullTerminate:
                Write(stream, str, encoding);
                stream.Write((byte)0);
                break;
            case StringBinaryType.PrependLength:
            {
                var len = encoding.GetByteCount(str);
                stream.Write(len);
                Write(stream, str, encoding, len);
                break;
            }
            case StringBinaryType.PrependLengthUShort:
            {
                var len = encoding.GetByteCount(str);
                stream.Write(checked((ushort)len));
                Write(stream, str, encoding, len);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public static void Write<TStream>(TStream stream, ReadOnlySpan<char> str, IMutagenEncoding encoding)
        where TStream : IBinaryWriteStream
    {
        Write(stream, str, encoding, encoding.GetByteCount(str));
    }

    public static void Write<TStream>(TStream stream, ReadOnlySpan<char> str, IMutagenEncoding encoding, int byteCount)
        where TStream : IBinaryWriteStream
    {
        Span<byte> bytes = stackalloc byte[byteCount];
        encoding.GetBytes(str, bytes);
        stream.Write(bytes);
    }
}
