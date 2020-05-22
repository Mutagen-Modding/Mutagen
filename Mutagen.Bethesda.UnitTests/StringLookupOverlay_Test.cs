using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class StringLookupOverlay_Test
    {
        static List<string> _strs;
        static byte[] _stringsFormat;
        static byte[] _ILstringsFormat;

        static StringLookupOverlay_Test()
        {
            _strs = new List<string>()
            {
                "Hello",
                "There",
                "Kind",
                "Sir"
            };

            _stringsFormat = new byte[100];
            MutagenWriter writer = new MutagenWriter(
                new BinaryWriter(new MemoryStream(_stringsFormat)),
                GameConstants.Skyrim);
            writer.Write((uint)_strs.Count);
            writer.Write((uint)_strs.Sum(s => s.Length + 1));
            int sum = 0;
            // Write index
            for (int i = 0; i < _strs.Count; i++)
            {
                writer.Write(i + 1);
                writer.Write(sum);
                sum += _strs[i].Length + 1;
            }
            // Write strings
            for (int i = 0; i < _strs.Count; i++)
            {
                writer.Write(_strs[i], StringBinaryType.NullTerminate);
            }

            _ILstringsFormat = new byte[100];
            writer = new MutagenWriter(
                new BinaryWriter(new MemoryStream(_ILstringsFormat)),
                GameConstants.Skyrim);
            writer.Write((uint)_strs.Count);
            writer.Write((uint)_strs.Sum(s => s.Length + 5));
            sum = 0;
            // Write index
            for (int i = 0; i < _strs.Count; i++)
            {
                writer.Write(i + 1);
                writer.Write(sum);
                sum += _strs[i].Length + 5;
            }
            // Write strings
            for (int i = 0; i < _strs.Count; i++)
            {
                writer.Write(_strs[i].Length + 1);
                writer.Write(_strs[i], StringBinaryType.NullTerminate);
            }
        }

        [Fact]
        public void Strings_Typical()
        {
            var overlay = new StringsLookupOverlay(_stringsFormat, StringsLookupOverlay.Type.Normal);
            Assert.Equal(_strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(_strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(_strs[3], str);
        }

        [Fact]
        public void Strings_OutOfRange()
        {
            var overlay = new StringsLookupOverlay(_stringsFormat, StringsLookupOverlay.Type.Normal);
            Assert.False(overlay.TryLookup(56, out _));
        }

        [Fact]
        public void ILStrings_Typical()
        {
            var overlay = new StringsLookupOverlay(_ILstringsFormat, StringsLookupOverlay.Type.LengthPrepended);
            Assert.Equal(_strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(_strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(_strs[3], str);
        }

        [Fact]
        public void ILStrings_OutOfRange()
        {
            var overlay = new StringsLookupOverlay(_ILstringsFormat, StringsLookupOverlay.Type.LengthPrepended);
            Assert.False(overlay.TryLookup(56, out _));
        }
    }
}
