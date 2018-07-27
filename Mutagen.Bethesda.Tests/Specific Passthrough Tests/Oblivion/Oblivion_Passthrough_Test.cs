using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public abstract class Oblivion_Passthrough_Test
    {
        public abstract string Nickname { get; }
        public string FilePath { get; set; }

        public Oblivion_Passthrough_Test(string path = null)
        {
            this.FilePath = path;
        }

        public Dictionary<Type, List<FormID>> ImportExport(
            string inputPath,
            string outputPath)
        {
            var mod = OblivionMod.Create_Binary(
                inputPath);
            foreach (var record in mod.MajorRecords.Values)
            {
                if (record.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
                {
                    record.MajorRecordFlags &= ~MajorRecord.MajorRecordFlag.Compressed;
                }
            }

            mod.Write_Binary(outputPath);

            Dictionary<Type, List<FormID>> ret = new Dictionary<Type, List<FormID>>();
            foreach (var rec in mod.MajorRecords.Values)
            {
                ret.TryCreateValue(rec.GetType()).Add(rec.FormID);
            }
            return ret;
        }

        public ModAligner.AlignmentRules GetAlignmentRules()
        {
            var ret = new ModAligner.AlignmentRules();
            ret.AddAlignments(
                Cell_Registration.CELL_HEADER,
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
                new RecordType("XGLB"));
            ret.AddAlignments(
                Worldspace_Registration.WRLD_HEADER,
                new RecordType("EDID"),
                new RecordType("FULL"),
                new RecordType("WNAM"),
                new RecordType("CNAM"),
                new RecordType("NAM2"),
                new RecordType("ICON"),
                new RecordType("MNAM"),
                new RecordType("DATA"),
                new RecordType("NAM0"),
                new RecordType("NAM9"),
                new RecordType("SNAM"),
                new RecordType("XXXX"));
            ret.StopMarkers[Worldspace_Registration.WRLD_HEADER] = new List<RecordType>()
            {
                new RecordType("OFST"),
            };
            ret.AddAlignments(
                PlacedObject_Registration.REFR_HEADER,
                new ModAligner.AlignmentStraightRecord("EDID"),
                new ModAligner.AlignmentStraightRecord("NAME"),
                new ModAligner.AlignmentStraightRecord("XPCI"),
                new ModAligner.AlignmentStraightRecord("FULL"),
                new ModAligner.AlignmentStraightRecord("XTEL"),
                new ModAligner.AlignmentStraightRecord("XLOC"),
                new ModAligner.AlignmentStraightRecord("XOWN"),
                new ModAligner.AlignmentStraightRecord("XRNK"),
                new ModAligner.AlignmentStraightRecord("XGLB"),
                new ModAligner.AlignmentStraightRecord("XESP"),
                new ModAligner.AlignmentStraightRecord("XTRG"),
                new ModAligner.AlignmentStraightRecord("XSED"),
                new ModAligner.AlignmentStraightRecord("XLOD"),
                new ModAligner.AlignmentStraightRecord("XCHG"),
                new ModAligner.AlignmentStraightRecord("XHLT"),
                new ModAligner.AlignmentStraightRecord("XLCM"),
                new ModAligner.AlignmentStraightRecord("XRTM"),
                new ModAligner.AlignmentStraightRecord("XACT"),
                new ModAligner.AlignmentStraightRecord("XCNT"),
                new ModAligner.AlignmentSubRule(
                    new RecordType("XMRK"),
                    new RecordType("FNAM"),
                    new RecordType("FULL"),
                    new RecordType("TNAM")),
                new ModAligner.AlignmentStraightRecord("ONAM"),
                new ModAligner.AlignmentStraightRecord("XRGD"),
                new ModAligner.AlignmentStraightRecord("XSCL"),
                new ModAligner.AlignmentStraightRecord("XSOL"),
                new ModAligner.AlignmentStraightRecord("DATA"));
            ret.AddAlignments(
                PlacedCreature_Registration.ACRE_HEADER,
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XGLB"),
                new RecordType("XESP"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("DATA"));
            ret.AddAlignments(
                PlacedNPC_Registration.ACHR_HEADER,
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XPCI"),
                new RecordType("FULL"),
                new RecordType("XLOD"),
                new RecordType("XESP"),
                new RecordType("XMRC"),
                new RecordType("XHRS"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("DATA"));
            ret.SetGroupAlignment(
                GroupTypeEnum.CellTemporaryChildren,
                new RecordType("LAND"),
                new RecordType("PGRD"));
            return ret;
        }

        protected virtual BinaryFileProcessor.Config GetInstructions(
            Dictionary<long, uint> lengthTracker,
            MajorRecordLocator.FileLocations fileLocs)
        {
            return new BinaryFileProcessor.Config();
        }

        #region Dynamic Processing
        /*
         * Some records that seem older have an odd record order.  Rather than accommodating, dynamically mark as exceptions
         */
        protected virtual void AddDynamicProcessorInstructions(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            ProcessNPC_Mismatch(stream, recType, instr, loc, processing);
            ProcessCreature_Mismatch(stream, recType, instr, loc, processing);
            ProcessLeveledItemDataFields(stream, recType, instr, loc, processing);
            ProcessRegions(stream, recType, instr, loc, processing);
            ProcessPlacedObject_Mismatch(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessCells(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessDialogTopics(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessDialogItems(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessIdleAnimations(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessAIPackages(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessCombatStyle(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
            ProcessWater(stream, formID, recType, instr, loc, fileLocs, lengthTracker, processing);
        }

        private void ProcessNPC_Mismatch(
            BinaryReadStream stream,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            bool processing)
        {
            if (!processing) return;
            if (!typeof(NPC).Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width);
            this.DynamicMove(
                str,
                instr,
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
            BinaryReadStream stream,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            bool processing)
        {
            if (!processing) return;
            if (!typeof(Creature).Equals(recType)) return;
            this.AlignRecords(
                stream,
                instr,
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
            BinaryReadStream stream,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            bool processing)
        {
            if (!processing) return;
            if (!typeof(LeveledItem).Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = str.IndexOf("DATA");
            if (dataIndex == -1) return;

            var dataFlag = str[dataIndex + 6];
            if (dataFlag == 1)
            {
                var index = str.IndexOf("LVLD");
                index += 7;
                instr.SetAddition(
                    loc: index + loc.Min,
                    addition: new byte[]
                    {
                            (byte)'L',
                            (byte)'V',
                            (byte)'L',
                            (byte)'F',
                            0x1,
                            0x0,
                            0x2
                    });
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
                instr.SetSubstitution(
                    loc: loc.Min + Constants.HEADER_LENGTH,
                    sub: lenData);
            }

            // Remove DATA
            instr.SetRemove(
                new RangeInt64(dataIndex + loc.Min, dataIndex + loc.Min + 7 - 1));
        }

        private void ProcessRegions(
            BinaryReadStream stream,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            bool processing)
        {
            if (!processing) return;
            if (!typeof(Region).Equals(recType)) return;
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
                instr.SetMove(
                    loc: loc.Max + 1,
                    section: item.Value);
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

                instr.SetAddition(
                    loc: locToPlace,
                    addition: memStream.ToArray());
                instr.SetRemove(
                    section: iconLoc);
            }
        }

        private static byte[] ZeroFloat = new byte[] { 0, 0, 0, 0x80 };

        private void ProcessPlacedObject_Mismatch(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(PlacedObject).Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width);
            if (processing)
            {
                var datIndex = str.IndexOf("XLOC");
                if (datIndex != -1)
                {
                    stream.Position = loc.Min + datIndex;
                    stream.Position += 4;
                    var len = stream.ReadUInt16();
                    if (len == 16)
                    {
                        lengthTracker[loc.Min] = lengthTracker[loc.Min] - 4;
                        var removeStart = loc.Min + datIndex + 6 + 12;
                        instr.SetSubstitution(
                            loc: loc.Min + datIndex + 4,
                            sub: new byte[] { 12, 0 });
                        instr.SetRemove(
                            section: new RangeInt64(
                                removeStart,
                                removeStart + 3));
                        foreach (var k in fileLocs.GetContainingGroupLocations(formID))
                        {
                            lengthTracker[k] = lengthTracker[k] - 4;
                        }
                    }
                }
                datIndex = str.IndexOf("XSED");
                if (datIndex != -1)
                {
                    stream.Position = loc.Min + datIndex;
                    stream.Position += 4;
                    var len = stream.ReadUInt16();
                    if (len == 4)
                    {
                        lengthTracker[loc.Min] = lengthTracker[loc.Min] - 3;
                        var removeStart = loc.Min + datIndex + 6 + 1;
                        instr.SetSubstitution(
                            loc: loc.Min + datIndex + 4,
                            sub: new byte[] { 1, 0 });
                        instr.SetRemove(
                            section: new RangeInt64(
                                removeStart,
                                removeStart + 2));
                        foreach (var k in fileLocs.GetContainingGroupLocations(formID))
                        {
                            lengthTracker[k] = lengthTracker[k] - 3;
                        }
                    }
                }
            }
            else
            {
                //var datIndex = str.IndexOf("DATA");
                //if (datIndex != -1)
                //{
                //    stream.Position = loc.Min + datIndex;
                //    stream.Position += 6;
                //    for (int i = 0; i < 6; i++)
                //    {
                //        var bytes = stream.ReadBytes(4);
                //        if (bytes.SequenceEqual(ZeroFloat))
                //        {
                //            instr.IgnoreDifferenceSections.Add(new RangeInt64(stream.Position - 4, stream.Position - 1));
                //        }
                //    }
                //}

                //datIndex = str.IndexOf("XLOC");
                //if (datIndex != -1)
                //{
                //    instr.IgnoreDifferenceSections.Add(
                //        new RangeInt64(
                //            loc.Min + datIndex + 7,
                //            loc.Min + datIndex + 9));
                //}

                //datIndex = str.IndexOf("XTEL");
                //if (datIndex != -1)
                //{
                //    stream.Position = loc.Min + datIndex + 10;
                //    for (int i = 0; i < 6; i++)
                //    {
                //        var bytes = stream.ReadBytes(4);
                //        if (bytes.SequenceEqual(ZeroFloat))
                //        {
                //            instr.IgnoreDifferenceSections.Add(new RangeInt64(stream.Position - 4, stream.Position - 1));
                //        }
                //    }
                //}
            }
        }

        private void ProcessCells(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!processing) return;
            if (!typeof(Cell).Equals(recType)) return;

            // Clean empty child groups
            List<RangeInt64> removes = new List<RangeInt64>();
            stream.Position = loc.Min + 4;
            var len = stream.ReadUInt32();
            stream.Position += len + 12;
            var grupPos = stream.Position;
            var grup = stream.ReadString(4);
            if (!grup.Equals("GRUP")) return;
            var grupLen = stream.ReadUInt32();
            if (grupLen == 0x14)
            {
                removes.Add(new RangeInt64(grupPos, grupPos + 0x13));
            }
            else
            {
                stream.Position += 4;
                var grupType = (GroupTypeEnum)stream.ReadUInt32();
                if (grupType != GroupTypeEnum.CellChildren) return;
                stream.Position += 4;
                var amountRemoved = 0;
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
                        removes.Add(new RangeInt64(stream.Position - 0x14, stream.Position - 1));
                        amountRemoved++;
                    }
                }

                // Check to see if removed subgroups left parent empty
                if (amountRemoved > 0
                    && grupLen - 0x14 * amountRemoved == 0x14)
                {
                    removes.Add(new RangeInt64(grupPos, grupPos + 0x13));
                }
            }

            if (removes.Count == 0) return;
            var parentGrups = fileLocs.GetContainingGroupLocations(formID);
            foreach (var remove in removes)
            {
                instr.SetRemove(
                    section: remove);
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] - remove.Width);
                }
            }
        }

        private void ProcessDialogTopics(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(DialogTopic).Equals(recType)) return;

            // Clean empty child groups
            stream.Position = loc.Min + 4;
            var len = stream.ReadUInt32();
            stream.Position += len + 12;
            var grupPos = stream.Position;
            var grup = stream.ReadString(4);
            if (grup.Equals("GRUP"))
            {
                var parentGrups = fileLocs.GetContainingGroupLocations(formID);
                var grupLen = stream.ReadUInt32();
                if (grupLen == 0x14)
                {
                    instr.SetRemove(
                        section: new RangeInt64(grupPos, grupPos + 0x14 - 1));

                    foreach (var parentGroup in parentGrups)
                    {
                        lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] - 0x14);
                    }
                }
            }
        }

        private void ProcessDialogItems(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(DialogItem).Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = -1;
            var parentGrups = fileLocs.GetContainingGroupLocations(formID);
            int amount = 0;
            while ((dataIndex = str.IndexOf("CTDT", dataIndex + 1)) != -1)
            {
                instr.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                instr.SetAddition(
                    addition: new byte[4],
                    loc: dataIndex + loc.Min + 0x1A);
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] + 4);
                }
                amount += 4;
            }

            dataIndex = -1;
            while ((dataIndex = str.IndexOf("SCHD", dataIndex + 1)) != -1)
            {
                stream.Position = loc.Min + dataIndex + 4;
                var existingLen = stream.ReadUInt16();
                var diff = existingLen - 0x14;
                instr.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'R', 0x14 });
                if (diff == 0) continue;
                var locToRemove = loc.Min + dataIndex + 6 + 0x14;
                instr.SetRemove(
                    section: new RangeInt64(
                        locToRemove,
                        locToRemove + diff - 1));
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] - diff);
                }
                amount -= diff;
            }

            if (amount != 0)
            {
                // Modify Length
                stream.Position = loc.Min + Constants.HEADER_LENGTH;
                var existingLen = stream.ReadUInt16();
                byte[] lenData = new byte[2];
                using (var writer = new MutagenWriter(new MemoryStream(lenData)))
                {
                    writer.Write((ushort)(existingLen + amount));
                }
                instr.SetSubstitution(
                    loc: loc.Min + Constants.HEADER_LENGTH,
                    sub: lenData);
            }
        }

        private void ProcessIdleAnimations(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(IdleAnimation).Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = -1;
            var parentGrups = fileLocs.GetContainingGroupLocations(formID);
            int amount = 0;
            while ((dataIndex = str.IndexOf("CTDT", dataIndex + 1)) != -1)
            {
                instr.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                instr.SetAddition(
                    addition: new byte[4],
                    loc: dataIndex + loc.Min + 0x1A);
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] + 4);
                }
                amount += 4;
            }

            if (amount != 0)
            {
                // Modify Length
                stream.Position = loc.Min + Constants.HEADER_LENGTH;
                var existingLen = stream.ReadUInt16();
                byte[] lenData = new byte[2];
                using (var writer = new MutagenWriter(new MemoryStream(lenData)))
                {
                    writer.Write((ushort)(existingLen + amount));
                }
                instr.SetSubstitution(
                    loc: loc.Min + Constants.HEADER_LENGTH,
                    sub: lenData);
            }
        }

        private void ProcessAIPackages(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(AIPackage).Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = -1;
            var parentGrups = fileLocs.GetContainingGroupLocations(formID);
            int amount = 0;
            while ((dataIndex = str.IndexOf("CTDT", dataIndex + 1)) != -1)
            {
                instr.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                instr.SetAddition(
                    addition: new byte[4],
                    loc: dataIndex + loc.Min + 0x1A);
                amount += 4;
            }

            if ((dataIndex = str.IndexOf("PKDT", dataIndex + 1)) != -1)
            {
                stream.Position = loc.Min + dataIndex + 4;
                var len = stream.ReadUInt16();
                if (len == 4)
                {
                    instr.SetSubstitution(
                        loc: loc.Min + dataIndex + 4,
                        sub: new byte[] { 0x8 });
                    var first1 = stream.ReadUInt8();
                    var first2 = stream.ReadUInt8();
                    var second1 = stream.ReadUInt8();
                    var second2 = stream.ReadUInt8();
                    instr.SetSubstitution(
                        loc: loc.Min + dataIndex + 6,
                        sub: new byte[] { first1, first2, 0, 0 });
                    instr.SetAddition(
                        loc: loc.Min + dataIndex + 10,
                        addition: new byte[] { second1, 0, 0, 0 });
                    amount += 4;
                }
            }

            if (amount != 0)
            {
                // Modify Length
                stream.Position = loc.Min + Constants.HEADER_LENGTH;
                var existingLen = stream.ReadUInt16();
                byte[] lenData = new byte[2];
                using (var writer = new MutagenWriter(new MemoryStream(lenData)))
                {
                    writer.Write((ushort)(existingLen + amount));
                }
                instr.SetSubstitution(
                    loc: loc.Min + Constants.HEADER_LENGTH,
                    sub: lenData);
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] + amount);
                }
            }
        }

        private void ProcessCombatStyle(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(CombatStyle).Equals(recType)) return;

            if (!processing) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = str.IndexOf("CSTD");
            stream.Position = loc.Min + dataIndex + 4;
            var len = stream.ReadUInt16();
            if (dataIndex != -1)
            {
                var move = 2;
                instr.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + dataIndex + 6 + move);
                move = 38;
                if (len < 2 + move) return;
                instr.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + dataIndex + 6 + move);
                move = 53;
                if (len < 3 + move) return;
                instr.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 53);
                move = 69;
                if (len < 3 + move) return;
                instr.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 69);
                move = 82;
                if (len < 2 + move) return;
                instr.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 82);
                move = 113;
                if (len < 3 + move) return;
                instr.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 113);
            }
        }

        private void ProcessWater(
            BinaryReadStream stream,
            FormID formID,
            Type recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            MajorRecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool processing)
        {
            if (!typeof(Water).Equals(recType)) return;

            if (!processing) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = str.IndexOf("DATA");
            stream.Position = loc.Min + dataIndex + 4;
            var parentGrups = fileLocs.GetContainingGroupLocations(formID);
            var amount = 0;
            var len = stream.ReadUInt16();
            if (dataIndex != -1)
            {
                if (len == 0x02)
                {
                    instr.SetSubstitution(
                        loc: loc.Min + dataIndex + Constants.HEADER_LENGTH,
                        sub: new byte[] { 0, 0 });
                    instr.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x56)
                {
                    instr.SetSubstitution(
                        loc: loc.Min + dataIndex + Constants.HEADER_LENGTH,
                        sub: new byte[] { 0x54, 0 });
                    instr.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6 + 0x54,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x2A)
                {
                    instr.SetSubstitution(
                        loc: loc.Min + dataIndex + Constants.HEADER_LENGTH,
                        sub: new byte[] { 0x28, 0 });
                    instr.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6 + 0x28,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x3E)
                {
                    instr.SetSubstitution(
                        loc: loc.Min + dataIndex + Constants.HEADER_LENGTH,
                        sub: new byte[] { 0x3C, 0 });
                    instr.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6 + 0x3C,
                            length: 2));
                    amount -= 2;
                }

                var move = 0x39;
                if (len >= 3 + move)
                {
                    instr.SetSubstitution(
                        sub: new byte[] { 0, 0, 0 },
                        loc: loc.Min + dataIndex + 6 + move);
                }
            }

            if (amount != 0)
            {
                // Modify Length
                stream.Position = loc.Min + Constants.HEADER_LENGTH;
                var existingLen = stream.ReadUInt16();
                byte[] lenData = new byte[2];
                using (var writer = new MutagenWriter(new MemoryStream(lenData)))
                {
                    writer.Write((ushort)(existingLen + amount));
                }
                instr.SetSubstitution(
                    loc: loc.Min + Constants.HEADER_LENGTH,
                    sub: lenData);
                foreach (var parentGroup in parentGrups)
                {
                    lengthTracker[parentGroup] = (uint)(lengthTracker[parentGroup] + amount);
                }
            }
        }

        private bool DynamicMove(
            string str,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            IEnumerable<RecordType> offendingIndices,
            IEnumerable<RecordType> offendingLimits,
            IEnumerable<RecordType> locationsToMove,
            bool enforcePast = false)
        {
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
                out var locToMove,
                past: enforcePast ? offender : default(long?)))
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
                instr.SetMove(
                    section: new RangeInt64(offender, limit - 1),
                    loc: locToMove);
                return true;
            }
            return false;
        }

        private void AlignRecords(
            BinaryReadStream stream,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            IEnumerable<RecordType> rectypes)
        {
            stream.Position = loc.Min;
            var bytes = stream.ReadBytes((int)loc.Width);
            var str = BinaryUtility.BytesToString(bytes);
            List<(RecordType rec, int sourceIndex, int loc)> list = new List<(RecordType rec, int sourceIndex, int loc)>();
            int recTypeIndex = -1;
            foreach (var rec in rectypes)
            {
                recTypeIndex++;
                var index = str.IndexOf(rec.Type, Constants.RECORD_HEADER_LENGTH);
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
                instr.SetSubstitution(
                    loc: start + loc.Min,
                    sub: data);
                start += len;
            }
        }

        private bool LocateFirstOf(
            string str,
            long offset,
            IEnumerable<RecordType> types,
            out long loc,
            long? past = null)
        {
            List<int> indices = new List<int>(types
                .Select((r) => str.IndexOf(r.Type))
                .Where((i) => i != -1)
                .Where((i) => !past.HasValue || i > past));
            if (indices.Count == 0)
            {
                loc = default(long);
                return false;
            }
            loc = MathExt.Min(indices) + offset;
            return true;
        }
        #endregion

        public async Task BinaryPassthroughTest(
            bool reuseOld = true,
            bool deleteAfter = false)
        {
            using (var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"Mutagen_Binary_Tests/{Nickname}")), deleteAfter: deleteAfter))
            {
                var outputPath = Path.Combine(tmp.Dir.Path, this.Nickname);
                var uncompressedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Uncompressed");
                var alignedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Aligned");
                var processedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Processed");

                if (!reuseOld || !File.Exists(uncompressedPath))
                {
                    ModDecompressor.Decompress(
                        inputPath: this.FilePath,
                        outputPath: uncompressedPath);
                }

                var importRecs = ImportExport(uncompressedPath, outputPath);
                return;

                if (!reuseOld || !File.Exists(alignedPath))
                {
                    ModAligner.Align(
                        inputPath: uncompressedPath,
                        outputPath: alignedPath,
                        alignmentRules: GetAlignmentRules(),
                        temp: tmp);
                }

                BinaryFileProcessor.Config instructions;
                if (!reuseOld || !File.Exists(processedPath))
                {
                    var alignedFileLocs = MajorRecordLocator.GetFileLocations(
                    alignedPath);

                    Dictionary<long, uint> lengthTracker = new Dictionary<long, uint>();

                    using (var reader = new BinaryReadStream(alignedPath))
                    {
                        foreach (var grup in alignedFileLocs.GrupLocations.And(alignedFileLocs.ListedRecords.Keys))
                        {
                            reader.Position = grup + 4;
                            lengthTracker[grup] = reader.ReadUInt32();
                        }
                    }

                    instructions = GetInstructions(
                        lengthTracker,
                        alignedFileLocs);

                    using (var stream = new BinaryReadStream(alignedPath))
                    {
                        foreach (var recType in importRecs)
                        {
                            foreach (var id in recType.Value)
                            {
                                AddDynamicProcessorInstructions(
                                    stream: stream,
                                    formID: id,
                                    recType: recType.Key,
                                    instr: instructions,
                                    loc: alignedFileLocs[id],
                                    fileLocs: alignedFileLocs,
                                    lengthTracker: lengthTracker,
                                    processing: true);
                            }
                        }
                    }

                    using (var reader = new BinaryReadStream(alignedPath))
                    {
                        foreach (var grup in lengthTracker)
                        {
                            reader.Position = grup.Key + 4;
                            if (grup.Value == reader.ReadUInt32()) continue;
                            instructions.SetSubstitution(
                                loc: grup.Key + 4,
                                sub: BitConverter.GetBytes(grup.Value));
                        }
                    }
                    
                    using (var processor = new BinaryFileProcessor(
                        new FileStream(alignedPath, FileMode.Open, FileAccess.Read),
                        instructions))
                    {
                        using (var outStream = new FileStream(processedPath, FileMode.Create, FileAccess.Write))
                        {
                            processor.CopyTo(outStream);
                        }
                    }
                }

                instructions = new BinaryFileProcessor.Config();
                var processedFileLocs = MajorRecordLocator.GetFileLocations(
                    processedPath);
                using (var stream = new BinaryReadStream(processedPath))
                {
                    foreach (var recType in importRecs)
                    {
                        foreach (var id in recType.Value)
                        {
                            AddDynamicProcessorInstructions(
                                stream: stream,
                                formID: id,
                                recType: recType.Key,
                                instr: instructions,
                                loc: processedFileLocs[id],
                                fileLocs: processedFileLocs,
                                lengthTracker: null,
                                processing: false);
                        }
                    }
                }

                using (var stream = new BinaryReadStream(processedPath))
                {
                    var ret = Passthrough_Tests.AssertFilesEqual(
                        stream,
                        outputPath,
                        amountToReport: 15);
                    if (ret.Exception != null)
                    {
                        throw ret.Exception;
                    }
                }
            }
        }
    }
}
