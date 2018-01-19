using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen;
using Noggog.Utility;
using Xunit;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;

namespace Mutagen.Bethesda.Tests
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

            Dictionary<RawFormID, FileLocation> fileLocs;
            using (var stream = new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open))
            {
                fileLocs = MajorRecordLocator.GetFileLocations(
                    stream: stream,
                    uninterestingTypes: OblivionMod.NonTypeGroups);
            }

            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary"))))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);

                // Test compressions separately
                using (var stream = new MutagenReader(Properties.Settings.Default.OblivionESM))
                {
                    foreach (var majorRec in mod.MajorRecords)
                    {
                        if (!majorRec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;

                        var origPath = Path.Combine(tmp.Dir.FullName, majorRec.TitleString);
                        var outputPath = Path.Combine(tmp.Dir.FullName, $"{majorRec.TitleString}Output");
                        majorRec.Write_Binary(outputPath);

                        if (!fileLocs.TryGetValue(majorRec.FormID, out var majorLoc))
                        {
                            throw new ArgumentException($"Trying to process a compressed major record that is not in the locations dictionary: {majorRec}");
                        }

                        stream.Position = majorLoc + 4;
                        var majorLen = new ContentLength(stream.ReadUInt32());
                        stream.Position = majorLoc;

                        using (var outputStream = new BinaryWriter(new FileStream(origPath, FileMode.Create)))
                        {
                            outputStream.Write(stream.ReadBytes(Constants.RECORD_HEADER_LENGTH + 4));
                            using (var frame = new MutagenFrame(stream, majorLen))
                            {
                                using (var decomp = frame.Decompress())
                                {
                                    outputStream.Write(decomp.ReadRemaining());
                                }
                            }
                        }

                        AssertFilesEqual(
                            origPath,
                            outputPath,
                            substitutions: null,
                            sourceSkips: null,
                            targetSkips: null);
                    }
                }

                mod.Write_Binary(
                    oblivionOutputPath,
                    out outputErrMask);
                var substitutions = new Substitutions();
                substitutions.Add(23192, 0);
                substitutions.Add(24134, 0);
                RangeCollection sourceSkip = new RangeCollection();
                RangeCollection targetSkip = new RangeCollection();
                var lowPrioEx = AssertFilesEqual(
                    Properties.Settings.Default.OblivionESM,
                    oblivionOutputPath,
                    substitutions,
                    sourceSkips: sourceSkip,
                    targetSkips: targetSkip);
                Assert.False(inputErrMask?.IsInError() ?? false);
                Assert.False(outputErrMask?.IsInError() ?? false);
                if (lowPrioEx != null)
                {
                    throw lowPrioEx;
                }
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

        private Exception AssertFilesEqual(
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
                        substitutions?.RawSubstitutions,
                        reader2).First(5).ToArray();
                    if (errs.Length > 0)
                    {
                        throw new ArgumentException($"Bytes did not match at positions: {string.Join(" ", errs.Select((r) => r.ToString("X")))}");
                    }
                    if (prototypeReader.Position != prototypeReader.Length)
                    {
                        return new ArgumentException($"Stream {prototypePath} had more data past position 0x{prototypeReader.Position} than {path2}");
                    }
                    if (reader2.Position != reader2.Length)
                    {
                        return new ArgumentException($"Stream {path2} had more data past position 0x{reader2.Position} than {prototypePath}");
                    }
                }
            }
            return null;
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
            var reader1Len = reader1.Length;
            var reader2Len = reader2.Length;
            while (reader1.Position < reader1Len
                && reader2.Position < reader2Len)
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
