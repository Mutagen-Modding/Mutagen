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
        private BinaryProcessorInstructions GetOblivionInstructions(
            Dictionary<long, uint> lengthTracker,
            MajorRecordLocator.FileLocations fileLocs)
        {
            var instructions = new BinaryProcessorInstructions();
            instructions.Instruction.Substitutions.Add(new DataTarget()
            {
                Location = 0xC46695,
                Data = new byte[] { 0x66, 0xDC, 0x05, 0x00 }
            });
            instructions.Instruction.Substitutions.Add(new DataTarget()
            {
                Location = 0xCA88D9,
                Data = new byte[] { 0xDB, 0xBC, 0x04, 0x00 }
            });
            instructions.Instruction.Substitutions.Add(new DataTarget()
            {
                Location = 0xCEAEB5,
                Data = new byte[] { 0x76, 0x0A, 0x00, 0x00 }
            });
            return instructions;
        }

        /*
         * Some records that seem older have an odd record order.  Rather than accommodating, dynamically mark as exceptions
         */
        private void AddDynamicProcessorInstructions(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool compressed,
            bool processing)
        {
            ProcessNPC_Mismatch(rec, instr, filePath, loc, compressed, processing);
            ProcessCreature_Mismatch(rec, instr, filePath, loc, compressed, processing);
            ProcessLeveledItemDataFields(rec, instr, filePath, loc, processing);
            ProcessRegions(rec, instr, filePath, loc, processing);
            ProcessPlacedObject_Mismatch(rec, instr, filePath, loc, fileLocs, lengthTracker, processing);
            ProcessCells(rec, instr, filePath, loc, fileLocs, lengthTracker, processing);
        }

        private void ProcessNPC_Mismatch(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            RangeInt64 loc,
            bool compressed,
            bool processing)
        {
            if (!processing) return;
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
            RangeInt64 loc,
            bool compressed,
            bool processing)
        {
            if (!processing) return;
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
            RangeInt64 loc,
            bool processing)
        {
            if (!processing) return;
            if (!(rec is LeveledItem)) return;
            using (var stream = new BinaryReadStream(filePath))
            {
                stream.Position = loc.Min;
                var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
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

        private void ProcessRegions(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            RangeInt64 loc,
            bool processing)
        {
            if (!processing) return;
            if (!(rec is Region)) return;
            using (var stream = new BinaryReadStream(filePath))
            {
                stream.Position = loc.Min;
                var lenToRead = (int)loc.Width + Constants.RECORD_HEADER_LENGTH;
                var str = stream.ReadString(lenToRead);
                var rdatIndex = str.IndexOf("RDAT");
                if (rdatIndex == -1) return;
                SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
                while (rdatIndex != -1)
                {
                    var nextRdat = str.IndexOf("RDAT", rdatIndex + 1);
                    stream.Position = rdatIndex + 6 + loc.Min;
                    var index = stream.ReadUInt32();
                    rdats[index] =
                        new RangeInt64(
                            rdatIndex + loc.Min,
                            nextRdat == -1 ? loc.Max : nextRdat - 1 + loc.Min);
                    rdatIndex = nextRdat;
                }
                foreach (var item in rdats.Reverse())
                {
                    if (item.Key == (int)RegionData.RegionDataType.Icon) continue;
                    instr.Moves.Add(
                        new Move()
                        {
                            LocationToMove = loc.Max + 1,
                            SectionToMove = item.Value
                        });
                }

                if (rdats.ContainsKey((int)RegionData.RegionDataType.Icon))
                { // Need to create icon record
                    var edidIndex = str.IndexOf("EDID");
                    if (edidIndex == -1)
                    {
                        throw new ArgumentException();
                    }
                    stream.Position = edidIndex + loc.Min + Constants.HEADER_LENGTH;
                    var edidLen = stream.ReadUInt16();
                    stream.Position += edidLen;
                    var locToPlace = stream.Position;

                    // Get icon string
                    var iconLoc = rdats[(int)RegionData.RegionDataType.Icon];
                    stream.Position = iconLoc.Min + Region.RDAT_LEN + 6;
                    var iconStr = stream.ReadString((int)(iconLoc.Max - stream.Position));

                    // Get icon bytes
                    MemoryStream memStream = new MemoryStream();
                    using (var writer = new MutagenWriter(memStream))
                    {
                        using (HeaderExport.ExportHeader(
                            writer,
                            new RecordType("ICON"),
                            ObjectType.Subrecord))
                        {
                            writer.Write(iconStr);
                            writer.Write(default(byte));
                        }
                    }

                    instr.Additions.Add(
                        new DataTarget()
                        {
                            Location = locToPlace,
                            Data = memStream.ToArray()
                        });
                    instr.Moves.Add(
                        new Move()
                        {
                            LocationToMove = long.MaxValue,
                            SectionToMove = iconLoc
                        });
                }
            }
        }

        private static byte[] ZeroFloat = new byte[] { 0, 0, 0, 0x80 };
        private void ProcessPlacedObject_Mismatch(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!(rec is PlacedObject)) return;
            if (processing)
            {
                using (var stream = new BinaryReadStream(filePath))
                {
                    stream.Position = loc.Min;
                    var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
                    var datIndex = str.IndexOf("XLOC");
                    if (datIndex == -1) return;
                    stream.Position = loc.Min + datIndex;
                    stream.Position += 4;
                    var len = stream.ReadUInt16();
                    if (len == 16)
                    {
                        lengthTracker[loc.Min] = lengthTracker[loc.Min] - 4;
                        var removeStart = loc.Min + datIndex + 6 + 12;
                        instr.Substitutions.Add(
                            new DataTarget()
                            {
                                Location = loc.Min + datIndex + 4,
                                Data = new byte[] { 12, 0 }
                            });
                        instr.Moves.Add(
                            new Move()
                            {
                                SectionToMove = new RangeInt64(
                                     removeStart,
                                     removeStart + 3),
                                LocationToMove = long.MaxValue,
                            });
                        foreach (var k in fileLocs.GetContainingGroupLocations(rec.FormID))
                        {
                            lengthTracker[k] = lengthTracker[k] - 4;
                        }
                    }
                }
            }
            else
            {
                using (var stream = new BinaryReadStream(filePath))
                {
                    stream.Position = loc.Min;
                    var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
                    var datIndex = str.IndexOf("DATA");
                    if (datIndex != -1)
                    {
                        stream.Position = loc.Min + datIndex;
                        stream.Position += 6;
                        for (int i = 0; i < 6; i++)
                        {
                            var bytes = stream.ReadBytes(4);
                            if (bytes.SequenceEqual(ZeroFloat))
                            {
                                instr.IgnoreDifferenceSections.Add(new RangeInt64(stream.Position - 4, stream.Position - 1));
                            }
                        }
                    }

                    datIndex = str.IndexOf("XLOC");
                    if (datIndex != -1)
                    {
                        instr.IgnoreDifferenceSections.Add(
                            new RangeInt64(
                                loc.Min + datIndex + 7,
                                loc.Min + datIndex + 9));
                    }

                    datIndex = str.IndexOf("XTEL");
                    if (datIndex != -1)
                    {
                        stream.Position = loc.Min + datIndex + 10;
                        for (int i = 0; i < 6; i++)
                        {
                            var bytes = stream.ReadBytes(4);
                            if (bytes.SequenceEqual(ZeroFloat))
                            {
                                instr.IgnoreDifferenceSections.Add(new RangeInt64(stream.Position - 4, stream.Position - 1));
                            }
                        }
                    }
                }
            }
        }

        private void ProcessCells(
            MajorRecord rec,
            Instruction instr,
            string filePath,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!processing) return;
            if (!(rec is Cell cell)) return;

            // Clean empty child groups
            List<RangeInt64> moves = new List<RangeInt64>();
            long grupPos;
            using (var stream = new BinaryReadStream(filePath))
            {
                stream.Position = loc.Min + 4;
                var len = stream.ReadUInt32();
                stream.Position += len + 12;
                grupPos = stream.Position;
                var grup = stream.ReadString(4);
                if (!grup.Equals("GRUP")) return;
                var grupLen = stream.ReadUInt32();
                if (grupLen == 0x14)
                {
                    moves.Add(new RangeInt64(grupPos, grupPos + 0x14));
                }
                else
                {
                    stream.Position += 4;
                    var grupType = (GroupTypeEnum)stream.ReadUInt32();
                    if (grupType != GroupTypeEnum.CellChildren) return;
                    stream.Position += 4;
                    for (int i = 0; i < 3; i++)
                    {
                        var startPos = stream.Position;
                        var subGrup = stream.ReadString(4);
                        if (!subGrup.Equals("GRUP")) break;
                        var subGrupLen = stream.ReadUInt32();
                        stream.Position = startPos + subGrupLen;
                        if (subGrupLen == 0x14)
                        { // Empty group
                            lengthTracker[grupPos] = lengthTracker[grupPos] - 0x14;
                            moves.Add(new RangeInt64(stream.Position - 0x14, stream.Position - 1));
                        }
                    }
                }
            }

            if (moves.Count == 0) return;
            var parentGrups = fileLocs.GetContainingGroupLocations(rec.FormID);
            foreach (var move in moves)
            {
                instr.Moves.Add(
                    new Move()
                    {
                        LocationToMove = long.MaxValue,
                        SectionToMove = move
                    });
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] - move.Width);
                }
            }

            if (lengthTracker[grupPos] == 0x14)
            {
                var move = new RangeInt64(grupPos, grupPos + 0x13);
                instr.Moves.Add(
                    new Move()
                    {
                        LocationToMove = long.MaxValue,
                        SectionToMove = move
                    });
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] - move.Width);
                }
            }
        }

        private bool DynamicMove(
            Instruction instr,
            string filePath,
            RangeInt64 loc,
            IEnumerable<RecordType> offendingIndices,
            IEnumerable<RecordType> offendingLimits,
            IEnumerable<RecordType> locationsToMove)
        {
            using (var stream = new BinaryReadStream(filePath))
            {
                stream.Position = loc.Min;
                var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
                if (!LocateFirstOf(
                    str,
                    loc.Min,
                    offendingIndices,
                    out var offender)) return false;
                if (!LocateFirstOf(
                    str,
                    loc.Min,
                    offendingLimits,
                    out var limit)) return false;
                if (!LocateFirstOf(
                    str,
                    loc.Min,
                    locationsToMove,
                    out var locToMove))
                {
                    locToMove = loc.Min + str.Length;
                }
                if (limit == locToMove) return false;
                if (offender < limit)
                {
                    if (locToMove < offender)
                    {
                        throw new ArgumentException();
                    }
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
            RangeInt64 loc,
            IEnumerable<RecordType> rectypes)
        {
            using (var stream = new BinaryReadStream(filePath))
            {
                stream.Position = loc.Min;
                var bytes = stream.ReadBytes((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
                var str = BinaryUtility.BytesToString(bytes);
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

        private bool LocateFirstOf(
            string str,
            long offset,
            IEnumerable<RecordType> types,
            out long loc)
        {
            List<int> indices = new List<int>(types.Select((r) => str.IndexOf(r.Type)).Where((i) => i != -1));
            if (indices.Count == 0)
            {
                loc = default(long);
                return false;
            }
            loc = MathExt.Min(indices) + offset;
            return true;
        }
        
        public Task OblivionESM_Binary()
        {
            return OblivionESM_Binary_Internal(deleteAfter: true);
        }

        public async Task OblivionESM_Binary_Internal(bool deleteAfter)
        {
            await OblivionESM_Typical(deleteAfter: deleteAfter);
        }

        public ModAligner.AlignmentRules GetAlignmentRules()
        {
            var ret = new ModAligner.AlignmentRules();
            ret.Alignments[Cell_Registration.CELL_HEADER] = new List<RecordType>()
            {
                new RecordType("EDID"),
                new RecordType("FULL"),
                new RecordType("DATA"),
                new RecordType("XCLC"),
                new RecordType("XCLL"),
                new RecordType("XCLR"),
                new RecordType("XCMT"),
                new RecordType("XCLW"),
                new RecordType("XCCM"),
                new RecordType("XCWT"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XGLB"),
            };
            ret.Alignments[PlacedObject_Registration.REFR_HEADER] = new List<RecordType>()
            {
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XTEL"),
                new RecordType("XLOC"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XGLB"),
                new RecordType("XESP"),
                new RecordType("XTRG"),
                new RecordType("XSED"),
                new RecordType("XLOD"),
                new RecordType("XCHG"),
                new RecordType("XHLT"),
                new RecordType("XLCM"),
                new RecordType("XRTM"),
                new RecordType("XACT"),
                new RecordType("XCNT"),
                new RecordType("XMRK"),
                new RecordType("FNAM"),
                new RecordType("FULL"),
                new RecordType("TNAM"),
                new RecordType("ONAM"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("XSOL"),
                new RecordType("DATA"),
            };
            ret.Alignments[PlacedCreature_Registration.ACRE_HEADER] = new List<RecordType>()
            {
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XGLB"),
                new RecordType("XESP"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("DATA"),
            };
            ret.Alignments[PlacedNPC_Registration.ACHR_HEADER] = new List<RecordType>()
            {
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XLOD"),
                new RecordType("XESP"),
                new RecordType("XMRC"),
                new RecordType("XHRS"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("DATA"),
            };
            return ret;
        }

        private async Task OblivionESM_Typical(bool deleteAfter)
        {
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Mutagen_Oblivion_Binary")), deleteAfter: deleteAfter))
            {
                var oblivionOutputPath = Path.Combine(tmp.Dir.FullName, Constants.OBLIVION_ESM);
                var uncompressedPath = Path.Combine(tmp.Dir.FullName, $"{Constants.OBLIVION_ESM}_Uncompressed");
                var alignedPath = Path.Combine(tmp.Dir.FullName, $"{Constants.OBLIVION_ESM}_Aligned");
                var processedPath = Path.Combine(tmp.Dir.FullName, $"{Constants.OBLIVION_ESM}_Processed");

                ModDecompressor.Decompress(
                    inputPath: Properties.Settings.Default.OblivionESM,
                    outputPath: uncompressedPath,
                    interest: new RecordInterest(
                        uninterestingTypes: OblivionMod.NonTypeGroups));
                
                var mod = OblivionMod.Create_Binary(
                    uncompressedPath,
                    out var inputErrMask);
                Assert.False(inputErrMask?.IsInError() ?? false);

                foreach (var record in mod.MajorRecords.Values)
                {
                    if (record.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
                    {
                        record.MajorRecordFlags &= ~MajorRecord.MajorRecordFlag.Compressed;
                    }
                }

                ModAligner.Align(
                    inputPath: uncompressedPath,
                    outputPath: alignedPath,
                    alignmentRules: GetAlignmentRules());

                var alignedFileLocs = MajorRecordLocator.GetFileLocations(
                    alignedPath,
                    interest: new RecordInterest(
                        uninterestingTypes: OblivionMod.NonTypeGroups));

                Dictionary<long, uint> lengthTracker = new Dictionary<long, uint>();

                using (var reader = new BinaryReadStream(alignedPath))
                {
                    foreach (var grup in alignedFileLocs.GrupLocations.And(alignedFileLocs.ListedRecords.Keys))
                    {
                        reader.Position = grup + 4;
                        lengthTracker[grup] = reader.ReadUInt32();
                    }
                }

                var instructions = GetOblivionInstructions(
                    lengthTracker,
                    alignedFileLocs);

                foreach (var rec in mod.MajorRecords.Values)
                {
                    AddDynamicProcessorInstructions(
                        rec: rec,
                        instr: instructions.Instruction,
                        loc: alignedFileLocs[rec.FormID],
                        filePath: alignedPath,
                        fileLocs: alignedFileLocs,
                        lengthTracker: lengthTracker,
                        compressed: false,
                        processing: true);
                }

                using (var reader = new BinaryReadStream(alignedPath))
                {
                    foreach (var grup in lengthTracker)
                    {
                        reader.Position = grup.Key + 4;
                        if (grup.Value == reader.ReadUInt32()) continue;
                        instructions.Instruction.Substitutions.Add(
                            new DataTarget()
                            {
                                Location = grup.Key + 4,
                                Data = BitConverter.GetBytes(grup.Value)
                            });
                    }
                }

                var binConfig = instructions.Instruction.ToProcessorConfig();
                using (var processor = new BinaryFileProcessor(
                    new FileStream(alignedPath, FileMode.Open, FileAccess.Read),
                    binConfig))
                {
                    using (var outStream = new FileStream(processedPath, FileMode.Create, FileAccess.Write))
                    {
                        processor.CopyTo(outStream);
                    }
                }

                var processedFileLocs = MajorRecordLocator.GetFileLocations(
                    processedPath,
                    interest: new RecordInterest(
                        uninterestingTypes: OblivionMod.NonTypeGroups));
                foreach (var rec in mod.MajorRecords.Values)
                {
                    AddDynamicProcessorInstructions(
                        rec: rec,
                        instr: instructions.Instruction,
                        loc: processedFileLocs[rec.FormID],
                        filePath: processedPath,
                        fileLocs: processedFileLocs,
                        lengthTracker: null,
                        compressed: false,
                        processing: false);
                }

                mod.Write_Binary(oblivionOutputPath, out var outputErrMask);
                using (var stream = new FileStream(processedPath, FileMode.Open, FileAccess.Read))
                {
                    var ret = Passthrough_Tests.AssertFilesEqual(
                        stream,
                        oblivionOutputPath,
                        ignoreList: new RangeCollection(instructions.Instruction.IgnoreDifferenceSections),
                        sourceSkips: new RangeCollection(instructions.Instruction.SkipSourceSections),
                        targetSkips: new RangeCollection(instructions.Instruction.SkipOutputSections),
                        amountToReport: 15);
                    Assert.False(outputErrMask?.IsInError() ?? false);
                    if (ret.Exception != null)
                    {
                        throw ret.Exception;
                    }
                }
            }
        }
        
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
                var fileLocs = MajorRecordLocator.GetFileLocations(oblivionOutputPath);
                using (var reader = new BinaryReadStream(oblivionOutputPath))
                {
                    foreach (var rec in fileLocs.ListedRecords.Keys)
                    {
                        reader.Position = rec;
                        var t = HeaderTranslation.ReadNextRecordType(reader);
                        if (!t.Equals(NPC_Registration.NPC__HEADER))
                        {
                            throw new ArgumentException("Exported a non-NPC record.");
                        }
                    }
                }
            }
        }
        
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
                var fileLocs = MajorRecordLocator.GetFileLocations(oblivionOutputPath);
                using (var reader = new BinaryReadStream(oblivionOutputPath))
                {
                    foreach (var rec in fileLocs.ListedRecords.Keys)
                    {
                        reader.Position = rec;
                        var t = HeaderTranslation.ReadNextRecordType(reader);
                        if (!t.Equals(NPC_Registration.NPC__HEADER))
                        {
                            throw new ArgumentException("Exported a non-NPC record.");
                        }
                    }
                }
            }
        }

        private void CopyOverOffendingRecords(
            OblivionMod mod,
            IEnumerable<(RangeInt64 Source, RangeInt64? Output)> sections,
            string tmpFolder,
            string origPath,
            string processedPath,
            string outputPath,
            MajorRecordLocator.FileLocations originalFileLocs)
        {
            if (!sections.Any()) return;

            var outputFileLocs = MajorRecordLocator.GetFileLocations(outputPath);

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
                    using (var inStream = new BinaryReadStream(File.OpenRead(origPath)))
                    {
                        inStream.Position = sourceLoc.Min;
                        using (var outStream = new MutagenWriter(origRecOutputPath))
                        {
                            outStream.Write(inStream.ReadBytes((int)sourceLoc.Width));
                        }
                    }
                    output = Path.Combine(tmpFolder, $"{majorRec.TitleString} - Processed");
                    using (var inStream = new BinaryReadStream(File.OpenRead(processedPath)))
                    {
                        inStream.Position = sourceLoc.Min;
                        using (var outStream = new MutagenWriter(output))
                        {
                            outStream.Write(inStream.ReadBytes((int)sourceLoc.Width));
                        }
                    }
                    if (!Output.HasValue) continue;

                    if (!outputFileLocs.TryGetSection(id, out var outputLoc))
                    {
                        throw new NotImplementedException();
                    }
                    output = Path.Combine(tmpFolder, $"{majorRec.TitleString} - Output");
                    using (var inStream = new BinaryReadStream(File.OpenRead(outputPath)))
                    {
                        inStream.Position = outputLoc.Min;
                        using (var outStream = new MutagenWriter(output))
                        {
                            outStream.Write(inStream.ReadBytes((int)outputLoc.Width));
                        }
                    }
                }
            }
        }

        private async Task OblivionESM_Compression(
            OblivionMod mod,
            BinaryProcessorInstructions instructions,
            Task<MajorRecordLocator.FileLocations> longsTasks,
            bool deleteAfter)
        {
            MajorRecordLocator.FileLocations fileLocs = await longsTasks;

            using (var tmp = new TempFolder("Mutagen_Oblivion_Binary_CompressionTests", deleteAfter: deleteAfter))
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
                            using (var stream = new BinaryReadStream(File.OpenRead(outputPath)))
                            {
                                majorRec.Write_Binary(outputPath);

                                if (!fileLocs.TryGetSection(majorRec.FormID, out var majorLoc))
                                {
                                    throw new ArgumentException($"Trying to process a compressed major record that is not in the locations dictionary: {majorRec}");
                                }

                                stream.Position = majorLoc.Min + 4;
                                var majorLen = stream.ReadUInt32();
                                stream.Position = majorLoc.Min;

                                var memStream = new MemoryStream();
                                using (var outputStream = new MutagenWriter(memStream, dispose: false))
                                {
                                    var recType = stream.ReadString(4);
                                    using (HeaderExport.ExportRecordHeader(outputStream, new RecordType(recType)))
                                    {
                                        stream.Position += 4;
                                        outputStream.Write(stream.ReadBytes(Constants.RECORD_META_LENGTH - 4));
                                        using (var frame = MutagenFrame.ByLength(stream, majorLen))
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

                                AddDynamicProcessorInstructions(
                                    rec: majorRec,
                                    instr: processorConfig,
                                    filePath: origPath,
                                    lengthTracker: null,
                                    fileLocs: null,
                                    loc: new RangeInt64(0, memStream.Length),
                                    compressed: true,
                                    processing: true);
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

        public void OblivionESM_Equals()
        {
            throw new NotImplementedException();
        }

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
