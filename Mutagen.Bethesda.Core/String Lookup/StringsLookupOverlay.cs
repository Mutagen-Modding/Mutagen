using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Class that does minimal processing on string file data, exposing lookup queries in a lazy on-demand fashion.
    /// </summary>
    public class StringsLookupOverlay : IStringsLookup
    {
        private readonly Dictionary<uint, int> _locations = new Dictionary<uint, int>();
        private ReadOnlyMemorySlice<byte> _stringData;
        private StringsFileFormat _type;

        public int Count => _locations.Count;

        /// <summary>
        /// Overlays onto a set of bytes assumed to be in Strings file format
        /// </summary>
        /// <param name="data">Data to wrap</param>
        /// <param name="type">Strings file format</param>
        public StringsLookupOverlay(ReadOnlyMemorySlice<byte> data, StringsFileFormat type)
        {
            Init(data, type);
        }

        /// <summary>
        /// Reads all bytes from a file, and overlays them
        /// </summary>
        /// <param name="path">Path to read in</param>
        /// <param name="type">Strings file format</param>
        public StringsLookupOverlay(string path, StringsFileFormat type)
        {
            Init(File.ReadAllBytes(path), type);
        }

        /// <summary>
        /// Reads all bytes from a file, and overlays them
        /// </summary>
        /// <param name="path">Path to read in</param>
        /// <param name="source">Source type</param>
        public StringsLookupOverlay(string path, StringsSource source)
        {
            Init(File.ReadAllBytes(path), StringsUtility.GetFormat(source));
        }

        private void Init(ReadOnlyMemorySlice<byte> data, StringsFileFormat type)
        {
            try
            {
                _type = type;
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

            switch (_type)
            {
                case StringsFileFormat.Normal:
                    str = BinaryStringUtility.ParseUnknownLengthString(this._stringData.Slice(loc));
                    break;
                case StringsFileFormat.LengthPrepended:
                    try
                    {
                        str = BinaryStringUtility.ParsePrependedString(this._stringData.Slice(loc), 4);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new ArgumentOutOfRangeException("Strings file malformed.");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }
    }
}
