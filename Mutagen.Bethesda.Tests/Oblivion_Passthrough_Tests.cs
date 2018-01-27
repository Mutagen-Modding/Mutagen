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

        /*
         * Some records that seem older have an odd record order.  Rather than accommodating, dynamically mark as exceptions
         */
        private void AddDynamicProcessorInstructions(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            FileSection loc,
            bool compressed)
        {
            ProcessNPC_Mismatch(rec, instr, filePath, loc, compressed);
            ProcessCreature_Mismatch(rec, instr, filePath, loc, compressed);
        }

        private void ProcessNPC_Mismatch(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            FileSection loc,
            bool compressed)
        {
            if (!(rec is NPC)) return;
            if (compressed != rec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) return;
            this.DynamicMove(
                instr,
                filePath,
                loc,
                offendingIndices: new RecordType[]
                {
                    new RecordType("CNTO"),
                    new RecordType("SCRI"),
                    new RecordType("AIDT")
                },
                offendingLimits: new RecordType[]
                {
                    new RecordType("ACBS")
                },
                locationsToMove: new RecordType[]
                {
                    new RecordType("CNAM")
                });
        }

        private void ProcessCreature_Mismatch(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            FileSection loc,
            bool compressed)
        {
            if (!(rec is Creature)) return;
            if (compressed != rec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) return;
            this.DynamicMove(
                instr,
                filePath,
                loc,
                offendingIndices: new RecordType[]
                {
                    new RecordType("SPLO"),
                    new RecordType("NIFZ"),
                    new RecordType("ACBS"),
                    new RecordType("SNAM"),
                },
                offendingLimits: new RecordType[]
                {
                    new RecordType("INAM"),
                    new RecordType("SCRI"),
                    new RecordType("AIDT"),
                    new RecordType("PKID"),
                    new RecordType("CNTO"),
                },
                locationsToMove: new RecordType[]
                {
                    new RecordType("INAM"),
                    new RecordType("SCRI"),
                    new RecordType("AIDT"),
                    new RecordType("PKID"),
                });
        }

        private void DynamicMove(
            Instruction instr,
            string filePath,
            FileSection loc,
            IEnumerable<RecordType> offendingIndices,
            IEnumerable<RecordType> offendingLimits,
            IEnumerable<RecordType> locationsToMove)
        {
            using (var stream = new MutagenReader(filePath))
            {
                stream.Position = loc.Min;
                var str = stream.ReadString((int)loc.Range.Width);
                var offender = LocateFirstOf(str, loc.Min, offendingIndices);
                var limit = LocateFirstOf(str, loc.Min, offendingLimits);
                var locToMove = LocateFirstOf(str, loc.Min, locationsToMove);
                if (limit == locToMove) return;
                if (offender < limit)
                {
                    instr.Moves.Add(
                        new Move()
                        {
                            SectionToMove = new RangeInt64(offender, limit - 1),
                            LocationToMove = locToMove
                        });
                }
            }
        }

        private FileLocation LocateFirstOf(string str, FileLocation offset, IEnumerable<RecordType> types)
        {
            List<int> indices = new List<int>(types.Select((r) => str.IndexOf(r.Type)));
            return new FileLocation(MathExt.Min(indices.Where((i) => i != -1)) + offset.Offset);
        }

        [Fact]
        public async Task OblivionESM_Binary()
        {
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask);

            var instructions = GetOblivionInstructions();

            // Test compressions separately
            var fileLocations = Task.Run(() => OblivionESM_FileLocs());
            var compressionTest = Task.Run(() => OblivionESM_Compression(mod, instructions, fileLocations));
            var test = Task.Run(() => OblivionESM_Typical(mod, instructions, inputErrMask: inputErrMask, fileLocationsTask: fileLocations));
            await compressionTest;
            await test;
        }

        private async Task OblivionESM_Typical(
            OblivionMod mod,
            BinaryProcessorInstructions instructions,
            OblivionMod_ErrorMask inputErrMask, Task<Dictionary<RawFormID, FileSection>> fileLocationsTask)
        {
            Dictionary<RawFormID, FileSection> fileLocs = await fileLocationsTask;

            foreach (var rec in mod.MajorRecords)
            {
                if (rec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;
                AddDynamicProcessorInstructions(
                    rec,
                    instructions.Instruction,
                    Properties.Settings.Default.OblivionESM,
                    fileLocs[rec.FormID],
                    compressed: false);
            }

            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary"))))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);
                var processedPath = Path.Combine(tmp.Dir.FullName, $"{Constants.OBLIVION_ESM}_Processed");

                var binConfig = instructions.Instruction.ToProcessorConfig();
                using (var processor = new BinaryFileProcessor(
                    new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open, FileAccess.Read),
                    binConfig))
                {
                    using (var outStream = new FileStream(processedPath, FileMode.Create, FileAccess.Write))
                    {
                         processor.CopyTo(outStream);
                    }
                }

                mod.Write_Binary(oblivionOutputPath, out var outputErrMask);
                using (var stream = new FileStream(processedPath, FileMode.Open, FileAccess.Read))
                {
                    var lowPrioEx = Passthrough_Tests.AssertFilesEqual(
                        stream,
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

        private Dictionary<RawFormID, FileSection> OblivionESM_FileLocs()
        {
            Dictionary<RawFormID, FileSection> fileLocs;
            using (var stream = new FileStream(Properties.Settings.Default.OblivionESM, FileMode.Open, FileAccess.Read))
            {
                fileLocs = MajorRecordLocator.GetFileLocations(
                    stream: stream,
                    uninterestingTypes: OblivionMod.NonTypeGroups);
            }
            return fileLocs;
        }

        private async Task OblivionESM_Compression(OblivionMod mod, BinaryProcessorInstructions instructions, Task<Dictionary<RawFormID, FileSection>> fileLocationsTasks)
        {
            Dictionary<RawFormID, FileSection> fileLocs = await fileLocationsTasks;

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

                                stream.Position = majorLoc.Min + 4;
                                var majorLen = new ContentLength(stream.ReadUInt32());
                                stream.Position = majorLoc.Min;

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

                                AddDynamicProcessorInstructions(majorRec, processorConfig, origPath, loc: new FileSection(0, memStream.Length), compressed: true);
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
