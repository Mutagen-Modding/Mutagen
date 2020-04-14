using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public class OblivionProcessor : Processor
    {
        public override GameMode GameMode => GameMode.Oblivion;
        private HashSet<string> magicEffectEDIDs = new HashSet<string>();

        #region Dynamic Processing
        /*
         * Some records that seem older have an odd record order.  Rather than accommodating, dynamically mark as exceptions
         */
        protected override void AddDynamicProcessorInstructions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType)
        {
            base.AddDynamicProcessorInstructions(stream, formID, recType);
            var loc = this._AlignedFileLocs[formID];
            ProcessNPC(stream, recType,loc);
            ProcessCreature(stream, recType, loc);
            ProcessLeveledItemDataFields(stream, formID, recType, loc);
            ProcessRegions(stream, formID, recType, loc);
            ProcessPlacedObject(stream, formID, recType, loc);
            ProcessPlacedCreature(stream, formID, recType, loc);
            ProcessPlacedNPC(stream, formID, recType, loc);
            ProcessCells(stream, formID, recType, loc);
            ProcessDialogTopics(stream, formID, recType, loc);
            ProcessDialogItems(stream, formID, recType, loc);
            ProcessIdleAnimations(stream, formID, recType, loc);
            ProcessAIPackages(stream, formID, recType, loc);
            ProcessCombatStyle(stream, formID, recType, loc);
            ProcessWater(stream, formID, recType, loc);
            ProcessGameSettings(stream, formID, recType, loc);
            ProcessBooks(stream, formID, recType, loc);
            ProcessLights(stream, formID, recType, loc);
            ProcessSpell(stream, formID, recType, loc);
            ProcessMisindexedRecords(stream, formID, loc);
            ProcessMagicEffects(stream, formID, recType, loc);
            ProcessEnchantments(stream, formID, recType, loc);
            ProcessIngredient(stream, formID, recType, loc);
            ProcessPotion(stream, formID, recType, loc);
            ProcessSigilStone(stream, formID, recType, loc);
        }

        protected override void PreProcessorJobs(
            IMutagenReadStream stream)
        {
            base.PreProcessorJobs(stream);
            foreach (var rec in this._SourceFileLocs.ListedRecords)
            {
                LookForMagicEffects(
                    stream: stream,
                    formID: rec.Value.FormID,
                    recType: rec.Value.Record,
                    loc: this._AlignedFileLocs[rec.Value.FormID]);
            }
        }

        private void ProcessNPC(
            IMutagenReadStream stream,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Npc_Registration.NPC__HEADER.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width);
            this.DynamicMove(
                str,
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
            IMutagenReadStream stream,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Creature_Registration.CREA_HEADER.Equals(recType)) return;
            this.AlignRecords(
                stream,
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
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!LeveledItem_Registration.LVLI_HEADER.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            var dataIndex = str.IndexOf("DATA");
            if (dataIndex == -1) return;

            int amount = 0;
            var dataFlag = str[dataIndex + 6];
            if (dataFlag == 1)
            {
                var index = str.IndexOf("LVLD");
                index += 7;
                this._Instructions.SetAddition(
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
                stream.Position = loc.Min + Mutagen.Bethesda.Internals.Constants.HeaderLength;
                var existingLen = stream.ReadUInt16();
                byte[] lenData = new byte[2];
                using (var writer = new MutagenWriter(new MemoryStream(lenData), this.GameMode))
                {
                    writer.Write((ushort)(existingLen - 7));
                }
                this._Instructions.SetSubstitution(
                    loc: loc.Min + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                    sub: lenData);
            }

            // Remove DATA
            var dataRange = new RangeInt64(dataIndex + loc.Min, dataIndex + loc.Min + 7 - 1);
            this._Instructions.SetRemove(dataRange);
            amount -= (int)dataRange.Width;

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessRegions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Region_Registration.REGN_HEADER.Equals(recType)) return;
            stream.Position = loc.Min;
            var lenToRead = (int)loc.Width + Meta.MajorConstants.HeaderLength;
            var str = stream.ReadZString(lenToRead);
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
                this._Instructions.SetMove(
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
                stream.Position = edidIndex + loc.Min + Mutagen.Bethesda.Internals.Constants.HeaderLength;
                var edidLen = stream.ReadUInt16();
                stream.Position += edidLen;
                var locToPlace = stream.Position;

                // Get icon string
                var iconLoc = rdats[(int)RegionData.RegionDataType.Icon];
                stream.Position = iconLoc.Min + 20;
                var iconStr = stream.ReadZString((int)(iconLoc.Max - stream.Position));

                // Get icon bytes
                MemoryStream memStream = new MemoryStream();
                using (var writer = new MutagenWriter(memStream, this.GameMode))
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
                this._Instructions.SetAddition(
                    loc: locToPlace,
                    addition: arr);
                this._Instructions.SetRemove(
                    section: iconLoc);
                amount += arr.Length;
                amount -= (int)iconLoc.Width;
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private static byte[] ZeroFloat = new byte[] { 0, 0, 0, 0x80 };

        private void ProcessPlacedObject(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!PlacedObject_Registration.REFR_HEADER.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width);
            var datIndex = str.IndexOf("XLOC");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex;
                stream.Position += 4;
                var len = stream.ReadUInt16();
                if (len == 16)
                {
                    this._LengthTracker[loc.Min] = this._LengthTracker[loc.Min] - 4;
                    var removeStart = loc.Min + datIndex + 6 + 12;
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + datIndex + 4,
                        sub: new byte[] { 12, 0 });
                    this._Instructions.SetRemove(
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
                    this._LengthTracker[loc.Min] = this._LengthTracker[loc.Min] - 3;
                    var removeStart = loc.Min + datIndex + 6 + 1;
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + datIndex + 4,
                        sub: new byte[] { 1, 0 });
                    this._Instructions.SetRemove(
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
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }

            datIndex = str.IndexOf("XTEL");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6 + 4;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessPlacedCreature(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!PlacedCreature_Registration.TriggeringRecordType.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width);

            var datIndex = str.IndexOf("DATA");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessPlacedNPC(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!PlacedNpc_Registration.TriggeringRecordType.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width);

            var datIndex = str.IndexOf("DATA");
            if (datIndex != -1)
            {
                stream.Position = loc.Min + datIndex + 6;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessCells(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Cell_Registration.CELL_HEADER.Equals(recType)) return;

            // Clean empty child groups
            List<RangeInt64> removes = new List<RangeInt64>();
            stream.Position = loc.Min + 4;
            var len = stream.ReadUInt32();
            stream.Position += len + 12;
            var grupPos = stream.Position;
            var grup = stream.ReadZString(4);
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
                    var subGrup = stream.ReadZString(4);
                    if (!subGrup.Equals("GRUP")) break;
                    var subGrupLen = stream.ReadUInt32();
                    stream.Position = startPos + subGrupLen;
                    if (subGrupLen == 0x14)
                    { // Empty group
                        this._LengthTracker[grupPos] = this._LengthTracker[grupPos] - 0x14;
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
                this._Instructions.SetRemove(
                    section: remove);
                amount -= (int)remove.Width;
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID,
                doRecordLen: false);
        }

        private void ProcessDialogTopics(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!DialogTopic_Registration.DIAL_HEADER.Equals(recType)) return;

            // Clean empty child groups
            stream.Position = loc.Min + 4;
            var len = stream.ReadUInt32();
            stream.Position += len + 12;
            var grupPos = stream.Position;
            var grup = stream.ReadZString(4);
            int amount = 0;
            if (grup.Equals("GRUP"))
            {
                var grupLen = stream.ReadUInt32();
                if (grupLen == 0x14)
                {
                    this._Instructions.SetRemove(
                        section: new RangeInt64(grupPos, grupPos + 0x14 - 1));
                    amount -= 0x14;
                }
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID,
                doRecordLen: false);
        }

        private void ProcessDialogItems(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!DialogItem_Registration.INFO_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            var dataIndex = -1;
            int amount = 0;
            while ((dataIndex = str.IndexOf("CTDT", dataIndex + 1)) != -1)
            {
                this._Instructions.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
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
                this._Instructions.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'R', 0x14 });
                if (diff == 0) continue;
                var locToRemove = loc.Min + dataIndex + 6 + 0x14;
                this._Instructions.SetRemove(
                    section: new RangeInt64(
                        locToRemove,
                        locToRemove + diff - 1));
                amount -= diff;
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessIdleAnimations(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!IdleAnimation_Registration.IDLE_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            var dataIndex = -1;
            int amount = 0;
            while ((dataIndex = str.IndexOf("CTDT", dataIndex + 1)) != -1)
            {
                this._Instructions.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: dataIndex + loc.Min + 0x1A);
                amount += 4;
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessAIPackages(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!AIPackage_Registration.PACK_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            var dataIndex = -1;
            int amount = 0;
            while ((dataIndex = str.IndexOf("CTDT", dataIndex + 1)) != -1)
            {
                this._Instructions.SetSubstitution(
                    loc: dataIndex + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
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
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataIndex + 4,
                        sub: new byte[] { 0x8 });
                    var first1 = stream.ReadUInt8();
                    var first2 = stream.ReadUInt8();
                    var second1 = stream.ReadUInt8();
                    var second2 = stream.ReadUInt8();
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataIndex + 6,
                        sub: new byte[] { first1, first2, 0, 0 });
                    this._Instructions.SetAddition(
                        loc: loc.Min + dataIndex + 10,
                        addition: new byte[] { second1, 0, 0, 0 });
                    amount += 4;
                }
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessCombatStyle(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!CombatStyle_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            var dataIndex = str.IndexOf("CSTD");
            stream.Position = loc.Min + dataIndex + 4;
            var len = stream.ReadUInt16();
            if (dataIndex != -1)
            {
                var move = 2;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + dataIndex + 6 + move);
                move = 38;
                if (len < 2 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + dataIndex + 6 + move);
                move = 53;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 53);
                move = 69;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 69);
                move = 82;
                if (len < 2 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 82);
                move = 113;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + dataIndex + 6 + 113);
            }
        }

        private void ProcessWater(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Water_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            var dataIndex = str.IndexOf("DATA");
            stream.Position = loc.Min + dataIndex + 4;
            var amount = 0;
            var len = stream.ReadUInt16();
            if (dataIndex != -1)
            {
                if (len == 0x02)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataIndex + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x56)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataIndex + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x54, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6 + 0x54,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x2A)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataIndex + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x28, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6 + 0x28,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x3E)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataIndex + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x3C, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataIndex + 6 + 0x3C,
                            length: 2));
                    amount -= 2;
                }

                var move = 0x39;
                if (len >= 3 + move)
                {
                    this._Instructions.SetSubstitution(
                        sub: new byte[] { 0, 0, 0 },
                        loc: loc.Min + dataIndex + 6 + move);
                }
            }

            ProcessSubrecordLengths(
                stream,
                amount,
                loc.Min,
                formID);
        }

        private void ProcessGameSettings(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!GameSetting_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);

            var edidIndex = str.IndexOf("EDID");
            stream.Position = loc.Min + edidIndex + 6;
            if ((char)stream.ReadUInt8() != 'f') return;

            var dataIndex = str.IndexOf("DATA");
            if (dataIndex != -1)
            {
                stream.Position = loc.Min + dataIndex + 6;
                ProcessZeroFloat(stream);
            }
        }

        private void ProcessBooks(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Book_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);

            var dataIndex = str.IndexOf("DATA");
            if (dataIndex != -1)
            {
                stream.Position = loc.Min + dataIndex + 8;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
            }
        }

        private void ProcessLights(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Light_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);

            var dataIndex = str.IndexOf("DATA");
            if (dataIndex != -1)
            {
                stream.Position = loc.Min + dataIndex + 6 + 16;
                ProcessZeroFloat(stream);
                ProcessZeroFloat(stream);
                stream.Position += 4;
                ProcessZeroFloat(stream);
            }
        }

        private void ProcessSpell(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!SpellUnleveled_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);
            foreach (var scitIndex in IterateTypes(str, new RecordType("SCIT")))
            {
                stream.Position = loc.Min + scitIndex + 4;
                var len = stream.ReadUInt16();
                ProcessFormID(
                    stream,
                    pos: loc.Min + scitIndex + 6);
            }
            ProcessEffectsList(stream, formID, recType, loc);
        }

        private bool DynamicMove(
            string str,
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
                this._Instructions.SetMove(
                    section: new RangeInt64(offender, limit - 1),
                    loc: locToMove);
                return true;
            }
            return false;
        }

        private void AlignRecords(
            IMutagenReadStream stream,
            RangeInt64 loc,
            IEnumerable<RecordType> rectypes)
        {
            stream.Position = loc.Min;
            var bytes = stream.ReadBytes((int)loc.Width);
            var str = BinaryStringUtility.ToZString(bytes);
            List<(RecordType rec, int sourceIndex, int loc)> list = new List<(RecordType rec, int sourceIndex, int loc)>();
            int recTypeIndex = -1;
            foreach (var rec in rectypes)
            {
                recTypeIndex++;
                var index = str.IndexOf(rec.Type, Meta.MajorConstants.HeaderLength);
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
                this._Instructions.SetSubstitution(
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
            IMutagenReadStream stream,
            FormID formID,
            RangeInt64 loc)
        {
            ProcessFormID(
                stream,
                loc.Min + 12);
        }

        private void ProcessFormID(
            IMutagenReadStream stream,
            long pos)
        {
            stream.Position = pos;
            FormID formID = new FormID(stream.ReadUInt32());
            if (formID.ModIndex.ID <= this._NumMasters) return;
            this._Instructions.SetSubstitution(
                pos + 3,
                this._NumMasters);
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
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!MagicEffect_Registration.MGEF_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);

            var edidIndex = str.IndexOf("EDID");
            if (edidIndex != -1)
            {
                stream.Position = loc.Min + edidIndex;
                stream.Position += 4;
                var len = stream.ReadUInt16();
                var edid = stream.ReadZString(len - 1);
                magicEffectEDIDs.Add(edid);
            }
        }

        private void ProcessMagicEDID(
            IMutagenReadStream stream)
        {
            return; // Disabled now that EDID links no longer auto-clear
            var startLoc = stream.Position;
            var edid = stream.ReadZString(4);
            if (!magicEffectEDIDs.Contains(edid))
            {
                this._Instructions.SetSubstitution(
                    startLoc,
                    new byte[4]);
            }
        }

        private void ProcessMagicEffects(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!MagicEffect_Registration.MGEF_HEADER.Equals(recType)) return;

            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);

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
                        stream);
                    len -= 4;
                }
            }
        }

        private void ProcessEffectsList(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            stream.Position = loc.Min;
            var str = stream.ReadZString((int)loc.Width + Meta.MajorConstants.HeaderLength);

            foreach (var index in IterateTypes(str, new RecordType("EFIT")))
            {
                stream.Position = loc.Min + index + 6;
                ProcessMagicEDID(
                    stream);
            }

            foreach (var index in IterateTypes(str, new RecordType("EFID")))
            {
                stream.Position = loc.Min + index + 6;
                ProcessMagicEDID(
                    stream);
            }

            foreach (var index in IterateTypes(str, new RecordType("SCIT")))
            {
                stream.Position = loc.Min + index + 4;
                var len = stream.ReadUInt16();
                if (len <= 4) continue;
                stream.Position = loc.Min + index + 14;
                ProcessMagicEDID(
                    stream);
            }
        }

        private void ProcessEnchantments(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Enchantment_Registration.ENCH_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, loc);
        }

        private void ProcessIngredient(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Ingredient_Registration.INGR_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, loc);
        }

        private void ProcessPotion(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Potion_Registration.ALCH_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, loc);
        }

        private void ProcessSigilStone(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!SigilStone_Registration.SGST_HEADER.Equals(recType)) return;
            ProcessEffectsList(stream, formID, recType, loc);
        }
        #endregion
    }
}
