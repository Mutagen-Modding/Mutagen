using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen;
using Noggog.Utility;
using Xunit;
using Mutagen.Oblivion;
using Mutagen.Oblivion.Internals;
using Noggog;
using Mutagen.Binary;
using Mutagen.Internals;

namespace Mutagen.Tests
{
    public class Passthrough_Tests
    {
        class Substitutions
        {
            public Dictionary<RangeInt64, List<byte[]>> RawSubstitutions = new Dictionary<RangeInt64, List<byte[]>>();

            public void Add(long offset, byte value)
            {
                RawSubstitutions.TryCreateValue(new RangeInt64(offset), () => new List<byte[]>()).Add(
                    new byte[]
                    {
                        value
                    });
            }
        }


        [Fact]
        public void OblivionESM_Binary()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out inputErrMask);
            var substitutions = new Substitutions();
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);
                mod.Write_Binary(
                    outputPath,
                    out outputErrMask);
                substitutions.Add(23192, 0);
                substitutions.Add(24134, 0);
                RangeCollection sourceSkip = new RangeCollection();
                sourceSkip.Add(new RangeInt64(0x4F60D, 0x3B810A));
                RangeCollection targetSkip = new RangeCollection();
                targetSkip.Add(new RangeInt64(0x4F60D, 0x3B4816));
                AssertFilesEqual(
                    Properties.Settings.Default.OblivionESM, 
                    outputPath, 
                    substitutions,
                    sourceSkips: sourceSkip,
                    targetSkips: targetSkip);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        [Fact]
        public void KnightsESP_Binary()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.KnightsESP,
                out inputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Knights_Binary"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Constants.KNIGHTS_ESP);
                mod.Write_Binary(
                    outputPath,
                    out outputErrMask);
                AssertFilesEqual(Properties.Settings.Default.KnightsESP, outputPath);
                Assert.Null(inputErrMask);
                Assert.Null(outputErrMask);
            }
        }
        [Fact]
        public void OblivionESM_XML()
        {
            var modFromBinary = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var binInputErrMask);
            Assert.Null(binInputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_XML"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                modFromBinary.Write_XML(
                    outputPath,
                    out var outputErrMask);
                var modFromXML = OblivionMod.Create_XML(
                    outputPath,
                    out var xmlInputErrMask);
                Assert.Equal(modFromBinary, modFromXML);
                Assert.Null(xmlInputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        [Fact]
        public void KnightsESP_XML()
        {
            var modFromBinary = OblivionMod.Create_Binary(
                Properties.Settings.Default.KnightsESP,
                out var binInputErrMask);
            Assert.Null(binInputErrMask);
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Knights_XML"))))
            {
                var outputPath = Path.Combine(tmp.Dir.FullName, Path.GetRandomFileName());
                modFromBinary.Write_XML(
                    outputPath,
                    out var outputErrMask);
                var modFromXML = OblivionMod.Create_XML(
                    outputPath,
                    out var xmlInputErrMask);
                Assert.Equal(modFromBinary, modFromXML);
                Assert.Null(xmlInputErrMask);
                Assert.Null(outputErrMask);
            }
        }

        private void AssertFilesEqual(
            string prototypePath,
            string path2,
            Substitutions substitutions = null,
            RangeCollection sourceSkips = null,
            RangeCollection targetSkips = null)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var prototypeReader = new MutagenReader(prototypePath))
            {
                using (var reader2 = new MutagenReader(path2))
                {
                    var errs = ProcessDifferences(
                        RangeInt64.ConstructRanges(
                            GetDifferences(prototypeReader, reader2, sourceSkips, targetSkips),
                            b => !b),
                        substitutions.RawSubstitutions,
                        reader2).First(5).ToArray();
                    if (errs.Length > 0)
                    {
                        throw new ArgumentException($"Bytes did not match at positions: {string.Join(" ", errs.Select((r) => r.ToString("X")))}");
                    }
                    if (prototypeReader.Position != prototypeReader.Length)
                    {
                        throw new ArgumentException($"Stream {prototypePath} had more data past position 0x{prototypeReader.Position} than {path2}");
                    }
                    if (reader2.Position != reader2.Length)
                    {
                        throw new ArgumentException($"Stream {path2} had more data past position 0x{reader2.Position} than {prototypePath}");
                    }
                }
            }
        }

        private IEnumerable<RangeInt64> ProcessDifferences(
            IEnumerable<RangeInt64> incoming,
            Dictionary<RangeInt64, List<byte[]>> substitutions,
            MutagenReader stream)
        {
            foreach (var range in incoming)
            {
                if (substitutions == null)
                {
                    yield return range;
                    continue;
                }
                if (!substitutions.TryGetValue(range, out var subs))
                {
                    yield return range;
                    continue;
                }
                if (!TestSub(range, subs, stream)) yield return range;
            }
        }

        private bool TestSub(
            RangeInt64 range,
            IEnumerable<byte[]> subs,
            MutagenReader stream)
        {
            var curPos = stream.Position;
            stream.Position = new FileLocation(range.Min);
            var bytes = new byte[range.Width];
            stream.ReadInto(bytes);
            stream.Position = curPos;
            foreach (var sub in subs)
            {
                if (sub.SequenceEqual(bytes))
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<KeyValuePair<long, bool>> GetDifferences(
            MutagenReader reader1,
            MutagenReader reader2,
            RangeCollection reader1Skips,
            RangeCollection reader2Skips)
        {
            while (reader1.Position < reader1.Length
                && reader2.Position < reader2.Length)
            {
                if (reader1Skips != null
                    && reader1Skips.TryGetCurrentRange(reader1.Position, out var range1))
                {
                    reader1.Position = new FileLocation(range1.Max + 1);
                    continue;
                }
                if (reader2Skips != null
                    && reader2Skips.TryGetCurrentRange(reader2.Position, out var range2))
                {
                    reader2.Position = new FileLocation(range2.Max + 1);
                    continue;
                }
                var b1 = reader1.ReadByte();
                var b2 = reader2.ReadByte();
                yield return new KeyValuePair<long, bool>(
                    reader1.Position - 1,
                    b1 == b2);
            }
        }
    }
}
