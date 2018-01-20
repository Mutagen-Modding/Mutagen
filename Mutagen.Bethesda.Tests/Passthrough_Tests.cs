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
        [Fact]
        public void OblivionESM_Binary()
        {
            OblivionMod_ErrorMask inputErrMask, outputErrMask;
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out inputErrMask);

            Dictionary<RawFormID, FileLocation> fileLocs;
            using (var stream = new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open, FileAccess.Read))
            {
                fileLocs = MajorRecordLocator.GetFileLocations(
                    stream: stream,
                    uninterestingTypes: OblivionMod.NonTypeGroups);
            }

            // Test compressions separately
            using (var stream = new MutagenReader(Properties.Settings.Default.OblivionESM))
            {
                int i = 0;
                foreach (var majorRec in mod.MajorRecords)
                {
                    if (!majorRec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;

                    i++;
                    using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary_CompressionTests"))))
                    {
                        var origPath = Path.Combine(tmp.Dir.FullName, $"{i} - {majorRec.TitleString}");
                        var outputPath = Path.Combine(tmp.Dir.FullName, $"{i} - {majorRec.TitleString}Output");
                        majorRec.Write_Binary(outputPath);

                        if (!fileLocs.TryGetValue(majorRec.FormID, out var majorLoc))
                        {
                            throw new ArgumentException($"Trying to process a compressed major record that is not in the locations dictionary: {majorRec}");
                        }

                        stream.Position = majorLoc + 4;
                        var majorLen = new ContentLength(stream.ReadUInt32());
                        stream.Position = majorLoc;

                        using (var outputStream = new MutagenWriter(new FileStream(origPath, FileMode.Create)))
                        {
                            var recType = stream.ReadString(4);
                            using (HeaderExport.ExportRecordHeader(outputStream, new RecordType(recType)))
                            {
                                stream.Position += new ContentLength(4);
                                outputStream.Write(stream.ReadBytes(Constants.RECORD_HEADER_LENGTH - 4));
                                using (var frame = new MutagenFrame(stream, majorLen))
                                {
                                    using (var decomp = frame.Decompress())
                                    {
                                        outputStream.Write(decomp.ReadRemaining());
                                    }
                                }
                            }
                        }

                        using (var sourceStream = new FileStream(origPath, FileMode.Open, FileAccess.Read))
                        {
                            AssertFilesEqual(
                                sourceStream,
                                outputPath);
                        }
                    }
                }
            }

            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary"))))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);

                mod.Write_Binary(
                    oblivionOutputPath,
                    out outputErrMask);
                RangeCollection sourceSkip = new RangeCollection();
                RangeCollection targetSkip = new RangeCollection();
                var processor = new BinaryFileProcessor(
                    new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open, FileAccess.Read));
                processor.SetSubstitution(0x5A98, 0);
                processor.SetSubstitution(0x5E56, 0);
                using (processor)
                {
                    var lowPrioEx = AssertFilesEqual(
                        processor,
                        oblivionOutputPath);
                    Assert.False(inputErrMask?.IsInError() ?? false);
                    Assert.False(outputErrMask?.IsInError() ?? false);
                    if (lowPrioEx != null)
                    {
                        throw lowPrioEx;
                    }
                }
            }
        }

        [Fact]
        public void KnightsESP_Binary()
        {
            //OblivionMod_ErrorMask inputErrMask, outputErrMask;
            //var mod = OblivionMod.Create_Binary(
            //    Properties.Settings.Default.KnightsESP,
            //    out inputErrMask);
            //using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Knights_Binary"))))
            //{
            //    var outputPath = Path.Combine(tmp.Dir.FullName, Constants.KNIGHTS_ESP);
            //    mod.Write_Binary(
            //        outputPath,
            //        out outputErrMask);
            //    AssertFilesEqual(Properties.Settings.Default.KnightsESP, outputPath);
            //    Assert.Null(inputErrMask);
            //    Assert.Null(outputErrMask);
            //}
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
            Stream stream,
            string path2)
        {
            List<RangeInt32> errorRanges = new List<RangeInt32>();
            using (var reader2 = new FileStream(path2, FileMode.Open, FileAccess.Read))
            {
                var errs = RangeInt64.ConstructRanges(
                        GetDifferences(stream, reader2),
                        b => !b).First(5).ToArray();
                if (errs.Length > 0)
                {
                    throw new ArgumentException($"{path2} Bytes did not match at positions: {string.Join(" ", errs.Select((r) => r.ToString("X")))}");
                }
                if (stream.Position != stream.Length)
                {
                    return new ArgumentException($"{path2} Stream had more data past position 0x{stream.Position} than {path2}");
                }
                if (reader2.Position != reader2.Length)
                {
                    return new ArgumentException($"{path2} Stream {path2} had more data past position 0x{reader2.Position} than source stream.");
                }
            }
            return null;
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
            Stream reader1,
            Stream reader2)
        {
            var reader1Len = reader1.Length;
            var reader2Len = reader2.Length;
            while (reader1.Position < reader1Len
                && reader2.Position < reader2Len)
            {
                var b1 = reader1.ReadByte();
                var b2 = reader2.ReadByte();
                yield return new KeyValuePair<long, bool>(
                    reader1.Position - 1,
                    b1 == b2);
            }
        }
    }
}
