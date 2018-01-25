using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class Oblivion_Passthrough_Tests
    {
        private BinaryProcessorInstructions GetOblivionInstructions()
        {
            var instructions = new BinaryProcessorInstructions();
            instructions.Instruction.Substitutions.Add(new Substitution(0x5A98, 0));
            instructions.Instruction.Substitutions.Add(new Substitution(0x5E46, 0));
            instructions.SkipSourceSections.Add(new RangeInt64(0x93A648, 0xB44656));
            instructions.SkipOutputSections.Add(new RangeInt64(0x93A648, 0xB8BA09));
            return instructions;
        }

        private void ProcessNPC_CNTOMismatch(
            MajorRecord rec,
            RecordInstruction instr,
            string filePath)
        {
            if (!(rec is NPC)) return;
            using (var stream = new MutagenReader(filePath))
            {
                var str = stream.ReadString(stream.RemainingLength);
                List<int> indices = new List<int>()
                {
                    str.IndexOf("CNTO"),
                    str.IndexOf("SCRI"),
                    str.IndexOf("AIDT")
                };
                var min = MathExt.Min(indices.Where((i) => i != -1));

                int acbs = str.IndexOf("ACBS");
                int cnam = str.IndexOf("CNAM");
                if (min < acbs)
                {
                    instr.Moves.Add(
                        new Move()
                        {
                            SectionToMove = new RangeInt64(min, acbs - 1),
                            LocationToMove = cnam
                        });
                }
            }
        }

        [Fact]
        public async Task OblivionESM_Binary()
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

            var instructions = GetOblivionInstructions();

            // Test compressions separately
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary_CompressionTests"))))
            {
                List<Task> tasks = new List<Task>();
                int i = 0;
                foreach (var majorRec in mod.MajorRecords)
                {
                    if (!majorRec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;

                    var origPath = Path.Combine(tmp.Dir.FullName, $"{i} - {majorRec.TitleString}");
                    var outputPath = Path.Combine(tmp.Dir.FullName, $"{i} - {majorRec.TitleString} - Output");
                    i++;

                    if (!instructions.CompressionInstructions.TryGetValue(majorRec.FormID, out var processorConfig))
                    {
                        processorConfig = new RecordInstruction()
                        {
                            Record = majorRec.FormID
                        };
                    }

                    tasks.Add(
                        Task.Run(() =>
                        {
                            using (var stream = new MutagenReader(Properties.Settings.Default.OblivionESM))
                            {
                                majorRec.Write_Binary(outputPath);

                                if (!fileLocs.TryGetValue(majorRec.FormID, out var majorLoc))
                                {
                                    throw new ArgumentException($"Trying to process a compressed major record that is not in the locations dictionary: {majorRec}");
                                }

                                stream.Position = majorLoc + 4;
                                var majorLen = new ContentLength(stream.ReadUInt32());
                                stream.Position = majorLoc;

                                var memStream = new MemoryStream();
                                using (var outputStream = new MutagenWriter(memStream, dispose: false))
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

                                memStream.Position = 0;
                                using (FileStream filewriteStream = new FileStream(origPath, FileMode.Create, FileAccess.Write))
                                {
                                    memStream.CopyTo(filewriteStream);
                                }
                                memStream.Position = 0;

                                ProcessNPC_CNTOMismatch(majorRec, processorConfig, origPath);
                                var binaryConfig = processorConfig.ToProcessorConfig();

                                string sourcePath;
                                if (binaryConfig.HasProcessing)
                                {
                                    sourcePath = $"{origPath} - Processed";
                                    using (var proccessedStream = new BinaryFileProcessor(
                                            memStream,
                                            processorConfig.ToProcessorConfig()))
                                    {
                                        using (FileStream filewriteStream = new FileStream(sourcePath, FileMode.Create, FileAccess.Write))
                                        {
                                            proccessedStream.CopyTo(filewriteStream);
                                        }
                                    }
                                }
                                else
                                {
                                    sourcePath = origPath;
                                }
                                memStream.Dispose();

                                using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                                {
                                    Passthrough_Tests.AssertFilesEqual(
                                            sourceStream,
                                            outputPath);
                                }
                            }
                        }));
                }
                await Task.WhenAll(tasks);
            }

            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary"))))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);

                mod.Write_Binary(oblivionOutputPath, out outputErrMask);
                var binConfig = instructions.Instruction.ToProcessorConfig();
                using (var processor = new BinaryFileProcessor(
                    new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open, FileAccess.Read),
                    binConfig))
                {
                    var lowPrioEx = Passthrough_Tests.AssertFilesEqual(
                        processor,
                        oblivionOutputPath,
                        ignoreList: new RangeCollection(instructions.IgnoreDifferenceSections),
                        sourceSkips: new RangeCollection(instructions.SkipSourceSections),
                        targetSkips: new RangeCollection(instructions.SkipOutputSections));
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
    }
}
