using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Strings;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Strings_Test
    {
        static List<string> _strs;
        static byte[] _stringsFormat;
        static byte[] _ILstringsFormat;

        static Strings_Test()
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
                GameConstants.SkyrimLE);
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
                GameConstants.SkyrimLE);
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
            var overlay = new StringsLookupOverlay(_stringsFormat, StringsFileFormat.Normal);
            Assert.Equal(_strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(_strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(_strs[3], str);
        }

        [Fact]
        public void Strings_OutOfRange()
        {
            var overlay = new StringsLookupOverlay(_stringsFormat, StringsFileFormat.Normal);
            Assert.False(overlay.TryLookup(56, out _));
        }

        [Fact]
        public void ILStrings_Typical()
        {
            var overlay = new StringsLookupOverlay(_ILstringsFormat, StringsFileFormat.LengthPrepended);
            Assert.Equal(_strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(_strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(_strs[3], str);
        }

        [Fact]
        public void ILStrings_OutOfRange()
        {
            var overlay = new StringsLookupOverlay(_ILstringsFormat, StringsFileFormat.LengthPrepended);
            Assert.False(overlay.TryLookup(56, out _));
        }

        [Fact]
        public void TryRetrieveInfoFromString_Full()
        {
            Assert.True(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.FullName,
                "Skyrim_French.ILSTRINGS", 
                out var source, 
                out var language, 
                out var modName));
            Assert.Equal(StringsSource.IL, source);
            Assert.Equal(Language.French, language);
            Assert.Equal("Skyrim", modName.ToString());
        }

        [Fact]
        public void TryRetrieveInfoFromString_Full_Fail()
        {
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.FullName,
                "Skyrim_FrenchILSTRINGS",
                out var _, out var _, out var _));
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.FullName,
                "SkyrimFrench.ILSTRINGS",
                out var _, out var _, out var _));
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.FullName,
                "Skyrim_fr.ILSTRINGS",
                out var _, out var _, out var _));
        }

        [Fact]
        public void TryRetrieveInfoFromString_Iso()
        {
            Assert.True(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.Iso,
                "Skyrim_fr.ILSTRINGS",
                out var source, 
                out var language,
                out var modName));
            Assert.Equal(StringsSource.IL, source);
            Assert.Equal(Language.French, language);
            Assert.Equal("Skyrim", modName.ToString());
        }

        [Fact]
        public void TryRetrieveInfoFromString_Iso_Fail()
        {
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.Iso,
                "Skyrim_frILSTRINGS",
                out var _, out var _, out var _));
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.Iso, 
                "Skyrimfr.ILSTRINGS",
                out var _, out var _, out var _));
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.Iso, 
                "Skyrim_French.ILSTRINGS",
                out var _, out var _, out var _));
            Assert.False(StringsUtility.TryRetrieveInfoFromString(
                StringsLanguageFormat.Iso,
                "Skyrim_zz.ILSTRINGS",
                out var _, out var _, out var _));
        }
    }
}
