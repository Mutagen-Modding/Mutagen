using System;
using System.Text;

namespace Mutagen.Bethesda.Strings
{
    public interface IMutagenEncoding
    {
        string GetString(ReadOnlySpan<byte> bytes);
        int GetByteCount(ReadOnlySpan<char> str);
        int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes);
    }

    public class MutagenEncodingWrapper : IMutagenEncoding
    {
        private readonly Encoding _encoding;

        public MutagenEncodingWrapper(Encoding encoding)
        {
            _encoding = encoding;
        }

        public string GetString(ReadOnlySpan<byte> bytes)
        {
            return _encoding.GetString(bytes);
        }

        public int GetByteCount(ReadOnlySpan<char> str)
        {
            return _encoding.GetByteCount(str);
        }

        public int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            return _encoding.GetBytes(chars, bytes);
        }
    }

    public class MutagenEncodingFallbackWrapper : IMutagenEncoding
    {
        private readonly IMutagenEncoding _primaryEncoding;
        private readonly IMutagenEncoding _secondaryEncoding;

        public MutagenEncodingFallbackWrapper(
            IMutagenEncoding primaryEncoding,
            IMutagenEncoding secondaryEncoding)
        {
            _primaryEncoding = primaryEncoding;
            _secondaryEncoding = secondaryEncoding;
        }

        public string GetString(ReadOnlySpan<byte> bytes)
        {
            try
            {
                return _primaryEncoding.GetString(bytes);
            }
            catch (Exception)
            {
                return _secondaryEncoding.GetString(bytes);
            }
        }

        public int GetByteCount(ReadOnlySpan<char> str)
        {
            try
            {
                return _primaryEncoding.GetByteCount(str);
            }
            catch (Exception)
            {
                return _secondaryEncoding.GetByteCount(str);
            }
        }

        public int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            try
            {
                return _primaryEncoding.GetBytes(chars, bytes);
            }
            catch (Exception)
            {
                return _secondaryEncoding.GetBytes(chars, bytes);
            }
        }
    }
}