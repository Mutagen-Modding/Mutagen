using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Strings
{
    public class StringsLookupOverlayTests
    {
        private static readonly List<string> Strs;
        private static byte[] StringsFormat;
        private static byte[] IlStringsFormat;
        
        private const string FrenchString = "Livre de sort - Paralysie générale";

        private static byte[] FrenchStringsFormat;
        // private static byte[] FrenchIlStringsFormat;

        static StringsLookupOverlayTests()
        {
            Strs = new List<string>()
            {
                "Hello",
                "There",
                "Kind",
                "Sir"
            };

            var encodings = new Mutagen.Bethesda.Strings.DI.MutagenEncodingProvider();

            FrenchStringsFormat = CreateNormalStrings(
                new string[] { FrenchString }, 
                encodings.GetEncoding(GameRelease.SkyrimSE, Language.French));
            StringsFormat = CreateNormalStrings(Strs, encodings.GetEncoding(GameRelease.SkyrimSE, Language.English));
            IlStringsFormat = CreateIlStrings(Strs, encodings.GetEncoding(GameRelease.SkyrimSE, Language.English));
        }

        private static byte[] CreateIlStrings(IReadOnlyList<string> strs, IMutagenEncoding encoding)
        {
            var ret = new byte[100];
            var writer = new MutagenWriter(
                new BinaryWriter(new MemoryStream(ret)),
                GameConstants.SkyrimSE);
            writer.Write((uint)strs.Count);
            writer.Write((uint)strs.Sum(s => s.Length + 5));
            var sum = 0;
            // Write index
            for (int i = 0; i < strs.Count; i++)
            {
                writer.Write(i + 1);
                writer.Write(sum);
                sum += strs[i].Length + 5;
            }

            // Write strings
            for (int i = 0; i < Strs.Count; i++)
            {
                writer.Write(strs[i].Length + 1);
                writer.Write(strs[i], StringBinaryType.NullTerminate, encoding);
            }

            return ret;
        }

        private static byte[] CreateNormalStrings(IReadOnlyList<string> strs, IMutagenEncoding encoding)
        {
            var ret = new byte[100];
            var writer = new MutagenWriter(
                new BinaryWriter(new MemoryStream(ret)),
                GameConstants.SkyrimSE);
            writer.Write((uint)strs.Count);
            writer.Write((uint)strs.Sum(s => encoding.GetByteCount(s) + 1));
            int sum = 0;
            // Write index
            for (int i = 0; i < strs.Count; i++)
            {
                writer.Write(i + 1);
                writer.Write(sum);
                sum += encoding.GetByteCount(strs[i]) + 1;
            }

            // Write strings
            for (int i = 0; i < strs.Count; i++)
            {
                writer.Write(strs[i], StringBinaryType.NullTerminate, encoding);
            }

            return ret;
        }

        [Fact]
        public void Typical()
        {
            var overlay = new StringsLookupOverlay(
                StringsFormat,
                StringsFileFormat.Normal, 
                new MutagenEncodingProvider()
                    .GetEncoding(GameRelease.SkyrimSE, Language.English));
            Assert.Equal(Strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(Strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(Strs[3], str);
        }

        [Fact]
        public void OutOfRange()
        {
            var overlay = new StringsLookupOverlay(
                StringsFormat,
                StringsFileFormat.Normal, 
                new MutagenEncodingProvider()
                    .GetEncoding(GameRelease.SkyrimSE, Language.English));
            Assert.False(overlay.TryLookup(56, out _));
        }

        [Fact]
        public void IlTypical()
        {
            var overlay = new StringsLookupOverlay(
                IlStringsFormat,
                StringsFileFormat.LengthPrepended, 
                new MutagenEncodingProvider()
                    .GetEncoding(GameRelease.SkyrimSE, Language.English));
            Assert.Equal(Strs.Count, overlay.Count);
            Assert.True(overlay.TryLookup(1, out var str));
            Assert.Equal(Strs[0], str);
            Assert.True(overlay.TryLookup(4, out str));
            Assert.Equal(Strs[3], str);
        }

        [Fact]
        public void IlOutOfRange()
        {
            var overlay = new StringsLookupOverlay(
                IlStringsFormat, 
                StringsFileFormat.LengthPrepended,
                new MutagenEncodingProvider()
                    .GetEncoding(GameRelease.SkyrimSE, Language.English));
            Assert.False(overlay.TryLookup(56, out _));
        }

        [Fact]
        public void FrenchStrings()
        {
            var overlay = new StringsLookupOverlay(
                FrenchStringsFormat,
                StringsFileFormat.Normal,
                new MutagenEncodingProvider()
                    .GetEncoding(GameRelease.SkyrimSE, Language.French));
            overlay.Count.Should().Be(1);
            overlay.TryLookup(1, out var str)
                .Should().BeTrue();
            str.Should().Be(FrenchString);
        }
    }
}