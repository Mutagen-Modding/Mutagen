using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class Oblivion_Passthrough_Test
    {
        public string Nickname { get; }
        public FilePath FilePath { get; set; }
        public byte NumMasters { get; }
        public string ProcessedPath(TempFolder tmp) => Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Processed");
        private PassthroughSettings settings;
        private HashSet<string> magicEffectEDIDs = new HashSet<string>();

        public Oblivion_Passthrough_Test(PassthroughSettings settings, Passthrough passthrough)
        {
            this.FilePath = Path.Combine(settings.DataFolder, passthrough.Path);
            this.Nickname = passthrough.Path;
            this.NumMasters = passthrough.NumMasters;
            this.settings = settings;
        }
        
        public static ModRecordAligner.AlignmentRules GetAlignmentRules()
        {
            var ret = new ModRecordAligner.AlignmentRules();
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
                new ModRecordAligner.AlignmentStraightRecord("EDID"),
                new ModRecordAligner.AlignmentStraightRecord("NAME"),
                new ModRecordAligner.AlignmentStraightRecord("XPCI"),
                new ModRecordAligner.AlignmentStraightRecord("FULL"),
                new ModRecordAligner.AlignmentStraightRecord("XTEL"),
                new ModRecordAligner.AlignmentStraightRecord("XLOC"),
                new ModRecordAligner.AlignmentStraightRecord("XOWN"),
                new ModRecordAligner.AlignmentStraightRecord("XRNK"),
                new ModRecordAligner.AlignmentStraightRecord("XGLB"),
                new ModRecordAligner.AlignmentStraightRecord("XESP"),
                new ModRecordAligner.AlignmentStraightRecord("XTRG"),
                new ModRecordAligner.AlignmentStraightRecord("XSED"),
                new ModRecordAligner.AlignmentStraightRecord("XLOD"),
                new ModRecordAligner.AlignmentStraightRecord("XCHG"),
                new ModRecordAligner.AlignmentStraightRecord("XHLT"),
                new ModRecordAligner.AlignmentStraightRecord("XLCM"),
                new ModRecordAligner.AlignmentStraightRecord("XRTM"),
                new ModRecordAligner.AlignmentStraightRecord("XACT"),
                new ModRecordAligner.AlignmentStraightRecord("XCNT"),
                new ModRecordAligner.AlignmentSubRule(
                    new RecordType("XMRK"),
                    new RecordType("FNAM"),
                    new RecordType("FULL"),
                    new RecordType("TNAM")),
                new ModRecordAligner.AlignmentStraightRecord("ONAM"),
                new ModRecordAligner.AlignmentStraightRecord("XRGD"),
                new ModRecordAligner.AlignmentStraightRecord("XSCL"),
                new ModRecordAligner.AlignmentStraightRecord("XSOL"),
                new ModRecordAligner.AlignmentStraightRecord("DATA"));
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
            RecordLocator.FileLocations fileLocs)
        {
            return new BinaryFileProcessor.Config();
        }

        #region Dynamic Processing
        /*
         * Some records that seem older have an odd record order.  Rather than accommodating, dynamically mark as exceptions
         */
        protected virtual void AddDynamicProcessorInstructions(
            BinaryReadStream stream,
            byte numMasters,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            ProcessNPC(stream, recType, instr, loc);
            ProcessCreature(stream, recType, instr, loc);
            ProcessLeveledItemDataFields(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessRegions(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessPlacedObject(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessPlacedCreature(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessPlacedNPC(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessCells(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessDialogTopics(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessDialogItems(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessIdleAnimations(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessAIPackages(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessCombatStyle(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessWater(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessGameSettings(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessBooks(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessLights(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessSpell(stream, formID, recType, instr, loc, fileLocs, numMasters, lengthTracker);
            ProcessMisindexedRecords(stream, formID, instr, loc, numMasters);
            ProcessMagicEffects(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessEnchantments(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessIngredient(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessPotion(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
            ProcessSigilStone(stream, formID, recType, instr, loc, fileLocs, lengthTracker);
        }

        private void ProcessLengths(
            BinaryReadStream stream,
            int amount,
            RangeInt64 loc,
            FormID formID,
            BinaryFileProcessor.Config instr,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker,
            bool doRecordLen = true)
        {
            if (amount == 0) return;
            foreach (var k in fileLocs.GetContainingGroupLocations(formID))
            {
                lengthTracker[k] = (uint)(lengthTracker[k] + amount);
            }

            if (!doRecordLen) return;
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

        private void ProcessNPC(
            BinaryReadStream stream,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc)
        {
            if (!NPC_Registration.NPC__HEADER.Equals(recType)) return;
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

        private void ProcessCreature(
            BinaryReadStream stream,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc)
        {
            if (!Creature_Registration.CREA_HEADER.Equals(recType)) return;
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
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!LeveledItem_Registration.LVLI_HEADER.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = str.IndexOf("DATA");
            if (dataIndex == -1) return;

            int amount = 0;
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
                amount += 7;
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
            var dataRange = new RangeInt64(dataIndex + loc.Min, dataIndex + loc.Min + 7 - 1);
            instr.SetRemove(dataRange);
            amount -= (int)dataRange.Width;

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessRegions(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Region_Registration.REGN_HEADER.Equals(recType)) return;
            stream.Position = loc.Min;
            var lenToRead = (int)loc.Width + Constants.RECORD_HEADER_LENGTH;
            var str = stream.ReadString(lenToRead);
            int amount = 0;
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
                stream.Position = iconLoc.Min + RegionBinaryTranslation.RDAT_LEN + 6;
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

                var arr = memStream.ToArray();
                instr.SetAddition(
                    loc: locToPlace,
                    addition: arr);
                instr.SetRemove(
                    section: iconLoc);
                amount += arr.Length;
                amount -= (int)iconLoc.Width;
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private static byte[] ZeroFloat = new byte[] { 0, 0, 0, 0x80 };

        private void ProcessPlacedObject(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!PlacedObject_Registration.REFR_HEADER.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width);
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
                    amount -= 4;
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
                    amount -= 3;
                }
            }

            datIndex = str.IndexOf("DATA");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6;
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
            }

            datIndex = str.IndexOf("XTEL");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6 + 4;
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessPlacedCreature(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!PlacedCreature_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width);

            var datIndex = str.IndexOf("DATA");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6;
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessPlacedNPC(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!PlacedNPC_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width);

            var datIndex = str.IndexOf("DATA");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6;
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessCells(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Cell_Registration.CELL_HEADER.Equals(recType)) return;

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

            int amount = 0;
            foreach (var remove in removes)
            {
                instr.SetRemove(
                    section: remove);
                amount -= (int)remove.Width;
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker,
                doRecordLen: false);
        }

        private void ProcessDialogTopics(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!DialogTopic_Registration.DIAL_HEADER.Equals(recType)) return;

            // Clean empty child groups
            stream.Position = loc.Min + 4;
            var len = stream.ReadUInt32();
            stream.Position += len + 12;
            var grupPos = stream.Position;
            var grup = stream.ReadString(4);
            int amount = 0;
            if (grup.Equals("GRUP"))
            {
                var grupLen = stream.ReadUInt32();
                if (grupLen == 0x14)
                {
                    instr.SetRemove(
                        section: new RangeInt64(grupPos, grupPos + 0x14 - 1));
                    amount -= 0x14;
                }
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker,
                doRecordLen: false);
        }

        private void ProcessDialogItems(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!DialogItem_Registration.INFO_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = -1;
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
                amount -= diff;
            }

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessIdleAnimations(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!IdleAnimation_Registration.IDLE_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = -1;
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

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessAIPackages(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!AIPackage_Registration.PACK_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = -1;
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

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessCombatStyle(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!CombatStyle_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
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
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Water_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            var dataIndex = str.IndexOf("DATA");
            stream.Position = loc.Min + dataIndex + 4;
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

            ProcessLengths(
                stream,
                amount,
                loc,
                formID,
                instr,
                fileLocs,
                lengthTracker);
        }

        private void ProcessGameSettings(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!GameSetting_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);

            var edidIndex = str.IndexOf("EDID");
            stream.Position = loc.Min + edidIndex + 6;
            if ((char)stream.ReadUInt8() != 'f') return;

            var dataIndex = str.IndexOf("DATA");
            if (dataIndex != -1)
            {
                stream.Position = loc.Min + dataIndex + 6;
                ProcessZeroFloat(stream, instr);
            }
        }

        private void ProcessBooks(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Book_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);

            var dataIndex = str.IndexOf("DATA");
            if (dataIndex != -1)
            {
                stream.Position = loc.Min + dataIndex + 8;
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
            }
        }

        private void ProcessLights(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Light_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);

            var dataIndex = str.IndexOf("DATA");
            if (dataIndex != -1)
            {
                stream.Position = loc.Min + dataIndex + 6 + 16;
                ProcessZeroFloat(stream, instr);
                ProcessZeroFloat(stream, instr);
                stream.Position += 4;
                ProcessZeroFloat(stream, instr);
            }
        }

        private static void ProcessZeroFloat(BinaryReadStream stream, BinaryFileProcessor.Config instr)
        {
            var f = stream.ReadFloat();
            if (f == float.Epsilon)
            {
                instr.SetSubstitution(
                    stream.Position - 4,
                    new byte[4]);
                return;
            }
            stream.Position -= 4;
            uint floatInt = stream.ReadUInt32();
            if (floatInt == 0x80000000)
            {
                instr.SetSubstitution(
                    stream.Position - 4,
                    new byte[4]);
                return;
            }
        }

        private void ProcessSpell(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            byte numMasters,
            Dictionary<long, uint> lengthTracker)
        {
            if (!SpellUnleveled_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);
            foreach (var scitIndex in IterateTypes(str, new RecordType("SCIT")))
            {
                stream.Position = loc.Min + scitIndex + 4;
                var len = stream.ReadUInt16();
                ProcessFormID(
                    stream,
                    instr,
                    pos: loc.Min + scitIndex + 6,
                    numMasters: numMasters);
            }
            ProcessEffectsList(stream, formID, recType, instr, loc);
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

        private void ProcessMisindexedRecords(
            BinaryReadStream stream,
            FormID formID,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            byte numMasters)
        {
            ProcessFormID(
                stream,
                instr,
                loc.Min + 12,
                numMasters);
        }

        private void ProcessFormID(
            BinaryReadStream stream,
            BinaryFileProcessor.Config instr,
            long pos,
            byte numMasters)
        {
            stream.Position = pos;
            FormID formID = new FormID(stream.ReadUInt32());
            if (formID.ModID.ID <= numMasters) return;
            instr.SetSubstitution(
                pos + 3,
                numMasters);
        }

        private IEnumerable<int> IterateTypes(string str, RecordType type)
        {
            int index = 0;
            int dataIndex;
            while ((dataIndex = str.IndexOf(type.Type, index)) != -1)
            {
                yield return dataIndex;
                index = dataIndex + 4;
            }
        }

        private void LookForMagicEffects(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc)
        {
            if (!MagicEffect_Registration.MGEF_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);

            var edidIndex = str.IndexOf("EDID");
            if (edidIndex != -1)
            {
                stream.Position = loc.Min + edidIndex;
                stream.Position += 4;
                var len = stream.ReadUInt16();
                var edid = stream.ReadString(len - 1);
                magicEffectEDIDs.Add(edid);
            }
        }

        private void ProcessMagicEDID(
            BinaryReadStream stream,
            BinaryFileProcessor.Config instr)
        {
            var startLoc = stream.Position;
            var edid = stream.ReadString(4);
            if (!magicEffectEDIDs.Contains(edid))
            {
                instr.SetSubstitution(
                    startLoc,
                    new byte[4]);
            }
        }

        private void ProcessMagicEffects(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!MagicEffect_Registration.MGEF_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);

            var edidForms = str.IndexOf("ESCE");
            if (edidForms != -1)
            {
                stream.Position = loc.Min + edidForms;
                stream.Position += 4;
                var len = stream.ReadUInt16();
                if (len % 4 != 0)
                {
                    throw new ArgumentException();
                }
                while (len > 0)
                {
                    ProcessMagicEDID(
                        stream,
                        instr);
                    len -= 4;
                }
            }
        }

        private void ProcessEffectsList(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc)
        {
            stream.Position = loc.Min;
            var str = stream.ReadString((int)loc.Width + Constants.RECORD_HEADER_LENGTH);

            foreach (var index in IterateTypes(str, new RecordType("EFIT")))
            {
                stream.Position = loc.Min + index + 6;
                ProcessMagicEDID(
                    stream,
                    instr);
            }

            foreach (var index in IterateTypes(str, new RecordType("EFID")))
            {
                stream.Position = loc.Min + index + 6;
                ProcessMagicEDID(
                    stream,
                    instr);
            }

            foreach (var index in IterateTypes(str, new RecordType("SCIT")))
            {
                stream.Position = loc.Min + index + 4;
                var len = stream.ReadUInt16();
                if (len <= 4) continue;
                stream.Position = loc.Min + index + 14;
                ProcessMagicEDID(
                    stream,
                    instr);
            }
        }

        private void ProcessEnchantments(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Enchantment_Registration.ENCH_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, instr, loc);
        }

        private void ProcessIngredient(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Ingredient_Registration.INGR_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, instr, loc);
        }

        private void ProcessPotion(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!Potion_Registration.ALCH_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, instr, loc);
        }

        private void ProcessSigilStone(
            BinaryReadStream stream,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
            if (!SigilStone_Registration.SGST_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, instr, loc);
        }
        #endregion

        public async Task<TempFolder> SetupProcessedFiles()
        {
            var tmp = new TempFolder(new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"Mutagen_Binary_Tests/{Nickname}")), deleteAfter: settings.DeleteCachesAfter);

            var outputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport");
            var observableOutputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_ObservableExport");
            var uncompressedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Uncompressed");
            var alignedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Aligned");
            var orderedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered");
            var preprocessedPath = alignedPath;
            var processedPath = ProcessedPath(tmp);

            if (!settings.ReuseCaches || !File.Exists(uncompressedPath))
            {
                using (var outStream = new FileStream(uncompressedPath, FileMode.Create, FileAccess.Write))
                {
                    ModDecompressor.Decompress(
                        streamCreator: () => File.OpenRead(this.FilePath.Path),
                        outputStream: outStream);
                }
            }

            if (!settings.ReuseCaches || !File.Exists(orderedPath))
            {
                using (var outStream = new FileStream(orderedPath, FileMode.Create))
                {
                    ModRecordSorter.Sort(
                        streamCreator: () => File.OpenRead(uncompressedPath),
                        outputStream: outStream);
                }
            }

            if (!settings.ReuseCaches || !File.Exists(alignedPath))
            {
                ModRecordAligner.Align(
                    inputPath: orderedPath,
                    outputPath: alignedPath,
                    alignmentRules: GetAlignmentRules(),
                    temp: tmp);
            }

            BinaryFileProcessor.Config instructions;
            if (!settings.ReuseCaches || !File.Exists(processedPath))
            {
                var alignedFileLocs = RecordLocator.GetFileLocations(preprocessedPath);

                Dictionary<long, uint> lengthTracker = new Dictionary<long, uint>();

                using (var reader = new BinaryReadStream(preprocessedPath))
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

                using (var stream = new BinaryReadStream(preprocessedPath))
                {
                    var fileLocs = RecordLocator.GetFileLocations(this.FilePath.Path);
                    foreach (var rec in fileLocs.ListedRecords)
                    {
                        LookForMagicEffects(
                            stream: stream,
                            formID: rec.Value.FormID,
                            recType: rec.Value.Record,
                            instr: instructions,
                            loc: alignedFileLocs[rec.Value.FormID]);
                    }
                    foreach (var rec in fileLocs.ListedRecords)
                    {
                        AddDynamicProcessorInstructions(
                            stream: stream,
                            formID: rec.Value.FormID,
                            recType: rec.Value.Record,
                            instr: instructions,
                            loc: alignedFileLocs[rec.Value.FormID],
                            fileLocs: alignedFileLocs,
                            lengthTracker: lengthTracker,
                            numMasters: this.NumMasters);
                    }
                }

                using (var reader = new BinaryReadStream(preprocessedPath))
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
                    new FileStream(preprocessedPath, FileMode.Open, FileAccess.Read),
                    instructions))
                {
                    using (var outStream = new FileStream(processedPath, FileMode.Create, FileAccess.Write))
                    {
                        processor.CopyTo(outStream);
                    }
                }
            }

            return tmp;
        }

        public async Task BinaryPassthroughTest()
        {
            using (var tmp = await SetupProcessedFiles())
            {
                var outputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_NormalExport");
                var processedPath = ProcessedPath(tmp);
                var orderedPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_Ordered");
                var observableOutputPath = Path.Combine(tmp.Dir.Path, $"{this.Nickname}_ObservableExport");
                
                // Do normal
                if (settings.TestNormal)
                {
                    ModKey modKey = ModKey.Factory(this.FilePath.Name);
                    var mod = await OblivionMod.Create_Binary(
                        orderedPath,
                        modKey: modKey);

                    foreach (var record in mod.MajorRecords.Items)
                    {
                        record.IsCompressed = false;
                    }
                    mod.Write_Binary(
                        outputPath,
                        Mutagen.Bethesda.Oblivion.Constants.Oblivion,
                        errorMask: out var outputMask);

                    Assert.False(outputMask?.IsInError() ?? false);
                    GC.Collect();
                
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

                if (settings.TestObservable)
                {
                    using (var stream = new BinaryReadStream(processedPath))
                    {
                        var ret = Passthrough_Tests.AssertFilesEqual(
                            stream,
                            observableOutputPath,
                            amountToReport: 15);
                        if (ret.Exception != null)
                        {
                            throw ret.Exception;
                        }
                    }
                }
            }
        }

        public async Task TestImport()
        {
            ModKey modKey = ModKey.Factory(this.FilePath.Name);
            var mod = await OblivionMod.Create_Binary(
                this.FilePath.Path,
                modKey: modKey);
        }
    }
}
