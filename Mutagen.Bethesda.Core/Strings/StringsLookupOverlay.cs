using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Strings
{
    /// <summary>
    /// Class that does minimal processing on string file data, exposing lookup queries in a lazy on-demand fashion.
    /// </summary>
    public class StringsLookupOverlay : IStringsLookup
    {
        private readonly Dictionary<uint, int> _locations = new();
        private ReadOnlyMemorySlice<byte> _stringData;
        private Encoding _encoding = null!;
        
        public StringsFileFormat Type { get; private set; }
        public int Count => _locations.Count;

        /// <summary>
        /// Overlays onto a set of bytes assumed to be in Strings file format
        /// </summary>
        /// <param name="data">Data to wrap</param>
        /// <param name="type">Strings file format</param>
        /// <param name="encoding">Encoding to read strings with</param>
        public StringsLookupOverlay(ReadOnlyMemorySlice<byte> data, StringsFileFormat type, Encoding encoding)
        {
            Init(data, type, encoding);
        }

        /// <summary>
        /// Overlays onto a set of bytes assumed to be in Strings file format
        /// </summary>
        /// <param name="data">Data to wrap</param>
        /// <param name="source">Source type</param>
        /// <param name="encoding">Encoding to read strings with</param>
        public StringsLookupOverlay(ReadOnlyMemorySlice<byte> data, StringsSource source, Encoding encoding)
        {
            Init(data, StringsUtility.GetFormat(source), encoding);
        }

        /// <summary>
        /// Reads all bytes from a file, and overlays them
        /// </summary>
        /// <param name="path">Path to read in</param>
        /// <param name="type">Strings file format</param>
        /// <param name="encoding">Encoding to read strings with</param>
        public StringsLookupOverlay(string path, StringsFileFormat type, Encoding encoding)
        {
            Init(File.ReadAllBytes(path), type, encoding);
        }

        /// <summary>
        /// Reads all bytes from a file, and overlays them
        /// </summary>
        /// <param name="path">Path to read in</param>
        /// <param name="source">Source type</param>
        /// <param name="encoding">Encoding to read strings with</param>
        public StringsLookupOverlay(string path, StringsSource source, Encoding encoding)
        {
            Init(File.ReadAllBytes(path), StringsUtility.GetFormat(source), encoding);
        }

        private void Init(ReadOnlyMemorySlice<byte> data, StringsFileFormat type, Encoding encoding)
        {
            try
            {
                _encoding = encoding;
                Type = type;
                var count = BinaryPrimitives.ReadUInt32LittleEndian(data);
                var dataSize = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(4)));
                var indexData = data.Slice(8, checked((int)(count * 2 * 4)));
                int loc = 0;
                for (int i = 0; i < count; i++)
                {
                    _locations.Add(
                        BinaryPrimitives.ReadUInt32LittleEndian(indexData.Slice(loc)),
                        checked((int)BinaryPrimitives.ReadUInt32LittleEndian(indexData.Slice(loc + 4))));
                    loc += 8;
                }
                _stringData = data.Slice(8 + indexData.Length, dataSize);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Strings file had duplicate entries.");
            }
            catch (OverflowException)
            {
                throw new ArgumentException("Strings file was too big for current systems");
            }
        }

        /// <inheritdoc />
        public bool TryLookup(uint key, [MaybeNullWhen(false)] out string str)
        {
            if (!_locations.TryGetValue(key, out var loc))
            {
                str = default;
                return false;
            }

            str = Get(loc);
            return true;
        }

        private string Get(int loc)
        {
            switch (Type)
            {
                case StringsFileFormat.Normal:
                    return _encoding.GetString(
                        BinaryStringUtility.ExtractUnknownLengthString(
                            _stringData.Slice(loc)));
                case StringsFileFormat.LengthPrepended:
                    try
                    {
                        var extract = BinaryStringUtility.ExtractPrependedString(_stringData.Slice(loc), 4);
                        extract = BinaryStringUtility.ProcessNullTermination(extract);
                        return _encoding.GetString(extract);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new ArgumentOutOfRangeException("Strings file malformed.");
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerator<KeyValuePair<uint, string>> GetEnumerator()
        {
            foreach (var loc in _locations)
            {
                yield return new KeyValuePair<uint, string>(loc.Key, Get(loc.Value));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
