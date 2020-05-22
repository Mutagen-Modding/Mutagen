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
        static byte[] _exampleStrings;

        static StringLookupOverlay_Test()
        {
            _strs = new List<string>()
            {
                "Hello",
                "There",
                "Kind",
                "Sir"
            };

            _exampleStrings = new byte[100];
            MutagenWriter writer = new MutagenWriter(
                new BinaryWriter(new MemoryStream(_exampleStrings)),
                GameConstants.Skyrim);
            writer.Write((uint)_strs.Count);
            writer.Write((uint)_strs.Sum(s => s.Length + 1));
            int sum = 0;
            for (int i = 0; i < _strs.Count; i++)
            {
                writer.Write(i + 1);
                writer.Write(sum);
                sum += _strs[i].Length + 1;
            }
            for (int i = 0; i < _strs.Count; i++)
            {
                writer.Write(_strs[i], StringBinaryType.NullTerminate);
            }
        }

        [Fact]
        public void Typical()
        {
            var overlay = new StringsLookupOverlay(_exampleStrings);
            Assert.Equal(_strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(_strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(_strs[3], str);
        }

        [Fact]
        public void OutOfRange()
        {
            var overlay = new StringsLookupOverlay(_exampleStrings);
            Assert.False(overlay.TryLookup(56, out _));
        }
    }
}
