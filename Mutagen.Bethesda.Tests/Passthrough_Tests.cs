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

            var instructions = BinaryProcessorInstructions.Create_XML("../../../Mutagen.Bethesda.Tests/OblivionModPassthroughInstructions.xml");

            Dictionary<RawFormID, BinaryFileProcessor.Config> compressionProcessing = new Dictionary<RawFormID, BinaryFileProcessor.Config>();
            var config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x7A, 0xC9), new FileLocation(0x10C));
            compressionProcessing[RawFormID.Factory("0x0000C51A")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x63, 0x9A), new FileLocation(0xF5));
            compressionProcessing[RawFormID.Factory("0x0000C23E")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x73, 0x108), new FileLocation(0x195));
            compressionProcessing[RawFormID.Factory("0x0000B513")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x73, 0x108), new FileLocation(0x195));
            compressionProcessing[RawFormID.Factory("0x0000B512")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x72, 0x107), new FileLocation(0x194));
            compressionProcessing[RawFormID.Factory("0x0000B511")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x72, 0x115), new FileLocation(0x1AC));
            compressionProcessing[RawFormID.Factory("0x0000B510")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x72, 0x107), new FileLocation(0x19E));
            compressionProcessing[RawFormID.Factory("0x0000B50F")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x71, 0x106), new FileLocation(0x19D));
            compressionProcessing[RawFormID.Factory("0x0000B50E")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x74, 0x16B), new FileLocation(0x1EE));
            compressionProcessing[RawFormID.Factory("0x0000B50D")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x74, 0x16B), new FileLocation(0x1EE));
            compressionProcessing[RawFormID.Factory("0x0000B50C")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x73, 0x16A), new FileLocation(0x1ED));
            compressionProcessing[RawFormID.Factory("0x0000B50B")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x70, 0xE9), new FileLocation(0x176));
            compressionProcessing[RawFormID.Factory("0x0000B50A")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x71, 0x106), new FileLocation(0x193));
            compressionProcessing[RawFormID.Factory("0x0000B509")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x72, 0x14D), new FileLocation(0x1D0));
            compressionProcessing[RawFormID.Factory("0x0000B508")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x72, 0x14D), new FileLocation(0x19E));
            compressionProcessing[RawFormID.Factory("0x00009ADF")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x72, 0x14D), new FileLocation(0x19E));
            compressionProcessing[RawFormID.Factory("0x00009ADE")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x71, 0x14C), new FileLocation(0x19D));
            compressionProcessing[RawFormID.Factory("0x00009ADD")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6E, 0x12D), new FileLocation(0x15C));
            compressionProcessing[RawFormID.Factory("0x00009ADC")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6E, 0x12D), new FileLocation(0x15C));
            compressionProcessing[RawFormID.Factory("0x00009ADB")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6D, 0x12C), new FileLocation(0x15B));
            compressionProcessing[RawFormID.Factory("0x00009ADA")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6C, 0x10F), new FileLocation(0x152));
            compressionProcessing[RawFormID.Factory("0x00009AD9")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6C, 0x10F), new FileLocation(0x152));
            compressionProcessing[RawFormID.Factory("0x00009AD8")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6B, 0x10E), new FileLocation(0x151));
            compressionProcessing[RawFormID.Factory("0x00009AD7")] = config;
            config = new BinaryFileProcessor.Config();
            config.SetMove(new RangeInt64(0x6D, 0x12C), new FileLocation(0x151));
            compressionProcessing[RawFormID.Factory("0x00009AD7")] = config;

            // Test compressions separately
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary_CompressionTests"))))
            {
                using (var stream = new MutagenReader(Properties.Settings.Default.OblivionESM))
                {
                    int i = 0;
                    foreach (var majorRec in mod.MajorRecords)
                    {
                        if (!majorRec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;

                        i++;
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

                        FileStream fileSourceStream = new FileStream(origPath, FileMode.Open, FileAccess.Read);
                        Stream sourceStream;
                        if (compressionProcessing.TryGetValue(majorRec.FormID, out var processorConfig))
                        {
                            sourceStream = new BinaryFileProcessor(
                                fileSourceStream,
                                processorConfig);
                        }
                        else
                        {
                            sourceStream = fileSourceStream;
                        }
                        using (sourceStream)
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
                var processorConfig = new BinaryFileProcessor.Config();
                processorConfig.SetSubstitution(new FileLocation(0x5A98), 0);
                processorConfig.SetSubstitution(new FileLocation(0x5E46), 0);
                using (var processor = new BinaryFileProcessor(
                    new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open, FileAccess.Read),
                    processorConfig))
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
