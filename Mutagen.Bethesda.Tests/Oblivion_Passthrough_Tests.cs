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
            instructions.Instruction.SkipSourceSections.Add(new RangeInt64(0x93A648, 0xB44656));
            instructions.Instruction.SkipOutputSections.Add(new RangeInt64(0x93A648, 0xB8BA09));
            instructions.Instruction.IgnoreDifferenceSections.Add(new RangeInt64(0xBFF2E2, 0xBFF2E5));
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
            ProcessLeveledItemDataFields(rec, instr, filePath, loc);
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
            this.AlignRecords(
                instr,
                filePath,
                loc,
                new RecordType[]
                {
                    new RecordType("EDID"),
                    new RecordType("FULL"),
                    new RecordType("MODL"),
                    new RecordType("CNTO"),
                    new RecordType("SPLO"),
                    new RecordType("NIFZ"),
                    new RecordType("ACBS"),
                    new RecordType("SNAM"),
                    new RecordType("INAM"),
                    new RecordType("SCRI"),
                    new RecordType("AIDT"),
                    new RecordType("PKID"),
                    new RecordType("KFFZ"),
                    new RecordType("DATA"),
                    new RecordType("RNAM"),
                    new RecordType("ZNAM"),
                    new RecordType("TNAM"),
                    new RecordType("BNAM"),
                    new RecordType("WNAM"),
                    new RecordType("NAM0"),
                    new RecordType("NAM1"),
                    new RecordType("CSCR"),
                    new RecordType("CSDT"),
                });
        }

        private void ProcessLeveledItemDataFields(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            FileSection loc)
        {
            if (!(rec is LeveledItem)) return;
            using (var stream = new MutagenReader(filePath))
            {
                stream.Position = loc.Min;
                var str = stream.ReadString((int)loc.Range.Width + Constants.RECORD_HEADER_LENGTH);
                var dataIndex = str.IndexOf("DATA");
                if (dataIndex == -1) return;

                var dataFlag = str[dataIndex + 6];
                if (dataFlag == 1)
                {
                    var index = str.IndexOf("LVLD");
                    index += 7;
                    var addition = new DataTarget()
                    {
                        Location = index + loc.Min,
                        Data = new byte[]
                        {
                            (byte)'L',
                            (byte)'V',
                            (byte)'L',
                            (byte)'F',
                            0x1,
                            0x0,
                            0x1
                        }
                    };
                    instr.Additions.Add(addition);
                }
                else
                {
                    // Modify Length
                    stream.Position = loc.Min + Constants.HEADER_LENGTH;
                    var existingLen = stream.ReadUInt16();
                    byte[] lenData = new byte[2];
                    using (var writer = new MutagenWriter(new MemoryStream(lenData)))
                    {
                        writer.Write((ushort)(existingLen - 7));
                    }
                    instr.Substitutions.Add(
                        new DataTarget()
                        {
                            Location = loc.Min + Constants.HEADER_LENGTH,
                            Data = lenData
                        });
                }

                // Remove DATA
                var move = new Move()
                {
                    LocationToMove = long.MaxValue,
                    SectionToMove = new RangeInt64(dataIndex + loc.Min, dataIndex + loc.Min + 7 - 1)
                };
                instr.Moves.Add(move);
            }
        }

        private bool DynamicMove(
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
                var str = stream.ReadString((int)loc.Range.Width + Constants.RECORD_HEADER_LENGTH);
                var offender = LocateFirstOf(str, loc.Min, offendingIndices);
                var limit = LocateFirstOf(str, loc.Min, offendingLimits);
                var locToMove = LocateFirstOf(str, loc.Min, locationsToMove);
                if (limit == locToMove) return false;
                if (offender < limit)
                {
                    instr.Moves.Add(
                        new Move()
                        {
                            SectionToMove = new RangeInt64(offender, limit - 1),
                            LocationToMove = locToMove
                        });
                    return true;
                }
            }
            return false;
        }

        private void AlignRecords(
            Instruction instr,
            string filePath,
            FileSection loc,
            IEnumerable<RecordType> rectypes)
        {
            using (var stream = new MutagenReader(filePath))
            {
                stream.Position = loc.Min;
                var bytes = stream.ReadBytes((int)loc.Range.Width + Constants.RECORD_HEADER_LENGTH);
                var str = MutagenReader.BytesToString(bytes);
                List<(RecordType rec, int sourceIndex, int loc)> list = new List<(RecordType rec, int sourceIndex, int loc)>();
                int recTypeIndex = -1;
                foreach (var rec in rectypes)
                {
                    recTypeIndex++;
                    var index = str.IndexOf(rec.Type);
                    if (index == -1) continue;
                    list.Add((rec, recTypeIndex, index));
                }
                if (list.Count == 0) return;
                List<int> locs = new List<int>(list.OrderBy((l) => l.loc).Select((l) => l.loc));
                var orderedList = list.OrderBy((l) => l.loc).ToList();
                if (list.Select(i => i.rec).SequenceEqual(orderedList.Select(i => i.rec))) return;
                int start = orderedList[0].loc;
                foreach (var item in list)
                {
                    var locIndex = locs.IndexOf(item.loc);
                    int len;
                    if (locIndex == locs.Count - 1)
                    {
                        len = str.Length - item.loc;
                    }
                    else
                    {
                        len = locs[locIndex + 1] - item.loc;
                    }
                    if (item.loc == start)
                    {
                        start += len;
                        continue;
                    }
                    var data = new byte[len];
                    for (int index = 0; index < len; index++)
                    {
                        data[index] = bytes[item.loc + index];
                    }
                    instr.Substitutions.Add(
                        new DataTarget()
                        {
                            Location = start + loc.Min,
                            Data = data
                        });
                    start += len;
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
            Assert.False(inputErrMask?.IsInError() ?? false);

            var instructions = GetOblivionInstructions();

            // Test compressions separately
            var fileLocations = Task.Run(() =>
            {
                return MajorRecordLocator.GetFileLocations(
                    filePath: Properties.Settings.Default.OblivionESM,
                    uninterestingTypes: OblivionMod.NonTypeGroups);
            });
            var compressionTest = Task.Run(() => OblivionESM_Compression(mod, instructions, fileLocations));
            var test = Task.Run(() => OblivionESM_Typical(mod, instructions, inputErrMask: inputErrMask, fileLocationsTask: fileLocations));
            await compressionTest;
            await test;
        }

        private async Task OblivionESM_Typical(
            OblivionMod mod,
            BinaryProcessorInstructions instructions,
            OblivionMod_ErrorMask inputErrMask, Task<MajorRecordLocator.FileLocations> fileLocationsTask)
        {
            MajorRecordLocator.FileLocations fileLocs = await fileLocationsTask;

            foreach (var rec in mod.MajorRecords.Values)
            {
                if (rec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;
                AddDynamicProcessorInstructions(
                    rec,
                    instructions.Instruction,
                    Properties.Settings.Default.OblivionESM,
                    fileLocs[rec.FormID],
                    compressed: false);
            }

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, Constants.OBLIVION_ESM);
                var processedPath = Path.Combine(tmp.Dir.Path, $"{Constants.OBLIVION_ESM}_Processed");

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
                    var ret = Passthrough_Tests.AssertFilesEqual(
                        stream,
                        oblivionOutputPath,
                        ignoreList: new RangeCollection(instructions.Instruction.IgnoreDifferenceSections),
                        sourceSkips: new RangeCollection(instructions.Instruction.SkipSourceSections),
                        targetSkips: new RangeCollection(instructions.Instruction.SkipOutputSections));
                    Assert.False(outputErrMask?.IsInError() ?? false);
                    CopyOverOffendingRecords(
                        mod: mod,
                        sections: ret.Sections,
                        tmpFolder: tmp.Dir.Path,
                        origPath: Properties.Settings.Default.OblivionESM,
                        processedPath: processedPath,
                        outputPath: oblivionOutputPath,
                        originalFileLocs: fileLocs);
                    if (ret.Exception != null)
                    {
                        throw ret.Exception;
                    }
                }
            }
        }

        [Fact]
        public async Task OblivionESM_GroupMask_Import()
        {
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask,
                importMask: new GroupMask()
                {
                    NPCs = true
                });
            Assert.False(inputErrMask?.IsInError() ?? false);

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Import"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, Constants.OBLIVION_ESM);
                mod.Write_Binary(
                    oblivionOutputPath,
                    out var outputErrMask);
                Assert.False(outputErrMask?.IsInError() ?? false);
                foreach (var rec in MajorRecordIterator.GetFileLocations(oblivionOutputPath))
                {
                    if (!rec.Type.Equals(NPC_Registration.NPC__HEADER))
                    {
                        throw new ArgumentException("Exported a non-NPC record.");
                    }
                }
            }
        }

        [Fact]
        public async Task OblivionESM_GroupMask_Export()
        {
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask);
            Assert.False(inputErrMask?.IsInError() ?? false);

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_GroupMask_Export"))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.Path, Constants.OBLIVION_ESM);
                mod.Write_Binary(
                    oblivionOutputPath,
                    out var outputErrMask,
                    importMask: new GroupMask()
                    {
                        NPCs = true
                    });
                Assert.False(outputErrMask?.IsInError() ?? false);
                foreach (var rec in MajorRecordIterator.GetFileLocations(oblivionOutputPath))
                {
                    if (!rec.Type.Equals(NPC_Registration.NPC__HEADER))
                    {
                        throw new ArgumentException("Exported a non-NPC record.");
                    }
                }
            }
        }

        private void CopyOverOffendingRecords(
            OblivionMod mod,
            IEnumerable<(FileSection Source, FileSection? Output)> sections,
            string tmpFolder,
            string origPath,
            string processedPath,
            string outputPath,
            MajorRecordLocator.FileLocations originalFileLocs)
        {
            if (!sections.Any()) return;

            var outputFileLocs = MajorRecordLocator.GetFileLocations(outputPath, uninterestingTypes: OblivionMod.NonTypeGroups);

            HashSet<FormID> ids = new HashSet<FormID>();
            foreach (var (Source, Output) in sections)
            {
                if (!originalFileLocs.TryGetRecords(Source, out var foundIDs)) continue;
                foreach (var id in foundIDs)
                {
                    if (!ids.Add(id)) continue;
                    var majorRec = mod[id];
                    var origRecOutputPath = Path.Combine(tmpFolder, majorRec.TitleString);
                    if (!originalFileLocs.TryGetSection(id, out var sourceLoc))
                    {
                        throw new NotImplementedException();
                    }
                    var output = Path.Combine(tmpFolder, $"{majorRec.TitleString} - Original");
                    using (var inStream = new MutagenReader(origPath))
                    {
                        inStream.Position = sourceLoc.Min;
                        using (var outStream = new MutagenWriter(origRecOutputPath))
                        {
                            outStream.Write(inStream.ReadBytes(sourceLoc.Width));
                        }
                    }
                    output = Path.Combine(tmpFolder, $"{majorRec.TitleString} - Processed");
                    using (var inStream = new MutagenReader(processedPath))
                    {
                        inStream.Position = sourceLoc.Min;
                        using (var outStream = new MutagenWriter(output))
                        {
                            outStream.Write(inStream.ReadBytes(sourceLoc.Width));
                        }
                    }
                    if (!Output.HasValue) continue;

                    if (!outputFileLocs.TryGetSection(id, out var outputLoc))
                    {
                        throw new NotImplementedException();
                    }
                    output = Path.Combine(tmpFolder, $"{majorRec.TitleString} - Output");
                    using (var inStream = new MutagenReader(outputPath))
                    {
                        inStream.Position = outputLoc.Min;
                        using (var outStream = new MutagenWriter(output))
                        {
                            outStream.Write(inStream.ReadBytes(outputLoc.Width));
                        }
                    }
                }
            }
        }

        private async Task OblivionESM_Compression(OblivionMod mod, BinaryProcessorInstructions instructions, Task<MajorRecordLocator.FileLocations> fileLocationsTasks)
        {
            MajorRecordLocator.FileLocations fileLocs = await fileLocationsTasks;

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_CompressionTests"))
            {
                List<Task> tasks = new List<Task>();
                int i = 0;
                foreach (var majorRec in mod.MajorRecords.Values)
                {
                    if (!majorRec.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed)) continue;

                    var origPath = Path.Combine(tmp.Dir.Path, $"{i} - {majorRec.TitleString}");
                    var outputPath = Path.Combine(tmp.Dir.Path, $"{i} - {majorRec.TitleString} - Output");
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

                                if (!fileLocs.TryGetSection(majorRec.FormID, out var majorLoc))
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
                                        outputStream.Write(stream.ReadBytes(Constants.RECORD_META_LENGTH - 4));
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
        public async Task OblivionESM_Folder_Reimport()
        {
            var mod = OblivionMod.Create_Binary(
                Properties.Settings.Default.OblivionESM,
                out var inputErrMask);
            Assert.False(inputErrMask?.IsInError() ?? false);
            using (var tmp = new TempFolder("Mutagen_Oblivion_XmlFolder", deleteAfter: false))
            {
                mod[FormID.Factory("0006371E")].Write_XML(Path.Combine(tmp.Dir.Path, "Test"));
                var exportMask = await mod.Write_XmlFolder(
                    tmp.Dir);
                Assert.False(exportMask?.IsInError() ?? false);
            }
        }

        [Fact]
        public void OblivionESM_Equals()
        {
            throw new NotImplementedException();
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
            using (var tmp = new TempFolder("Mutagen_Oblivion_XML"))
            {
                var outputPath = Path.Combine(tmp.Dir.Path, Path.GetRandomFileName());
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
            using (var tmp = new TempFolder("Mutagen_Knights_XML"))
            {
                var outputPath = Path.Combine(tmp.Dir.Path, Path.GetRandomFileName());
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
