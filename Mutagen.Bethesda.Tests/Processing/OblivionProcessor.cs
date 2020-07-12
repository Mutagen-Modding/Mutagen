using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public class OblivionProcessor : Processor
    {
        public override GameRelease GameRelease => GameRelease.Oblivion;

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
            ProcessNPC(stream, recType, loc);
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
        }

        private void ProcessNPC(
            IMutagenReadStream stream,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.NPC_.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            this.DynamicMove(
                majorFrame,
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
            if (!RecordTypes.CREA.Equals(recType)) return;
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
            if (!RecordTypes.LVLI.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (!majorFrame.TryLocateSubrecordFrame(RecordTypes.DATA, out var dataFrame, out var dataIndex)) return;

            int amount = 0;
            var dataFlag = dataFrame.AsUInt8();
            if (dataFlag == 1)
            {
                var lvld = majorFrame.LocateSubrecord(RecordTypes.LVLD, out var index);
                index += lvld.HeaderLength + 1;
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
                using (var writer = new MutagenWriter(new MemoryStream(lenData), this.GameRelease))
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

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessRegions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.REGN.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (!majorFrame.TryLocateSubrecordFrame(RecordTypes.RDAT, out var rdatFrame, out var rdatIndex)) return;
            int amount = 0;
            SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
            bool foundNext = true;
            while (foundNext)
            {
                foundNext = majorFrame.TryLocateSubrecordFrame(RecordTypes.RDAT, offset: rdatIndex + rdatFrame.TotalLength, out var nextRdatFrame, out var nextRdatIndex);
                var index = rdatFrame.Content.UInt32();
                rdats[index] =
                    new RangeInt64(
                        rdatIndex + loc.Min,
                        foundNext ? nextRdatIndex - 1 + loc.Min : loc.Max);
                rdatFrame = nextRdatFrame;
                rdatIndex = nextRdatIndex;
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
                if (!majorFrame.TryLocateSubrecordFrame("EDID", out var edidFrame, out var edidLoc))
                {
                    throw new ArgumentException();
                }
                var locToPlace = loc.Min + edidLoc + edidFrame.TotalLength;

                // Get icon string
                var iconLoc = rdats[(int)RegionData.RegionDataType.Icon];
                stream.Position = iconLoc.Min + 20;
                var iconStr = stream.ReadZString((int)(iconLoc.Max - stream.Position));

                // Get icon bytes
                MemoryStream memStream = new MemoryStream();
                using (var writer = new MutagenWriter(memStream, this.GameRelease))
                {
                    using (HeaderExport.Header(
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

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessPlacedObject(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.REFR.Equals(recType)) return;

            int amount = 0;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.XLOC, out var xlocFrame, out var xlocLoc)
                && xlocFrame.ContentLength == 16)
            {
                this._LengthTracker[loc.Min] = this._LengthTracker[loc.Min] - 4;
                var removeStart = loc.Min + xlocLoc + xlocFrame.HeaderLength + 12;
                this._Instructions.SetSubstitution(
                    loc: loc.Min + xlocLoc + 4,
                    sub: new byte[] { 12, 0 });
                this._Instructions.SetRemove(
                    section: new RangeInt64(
                        removeStart,
                        removeStart + 3));
                amount -= 4;
            }
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.XSED, out var xsedFrame, out var xsedLoc))
            {
                stream.Position = loc.Min + xsedLoc;
                stream.Position += 4;
                var len = stream.ReadUInt16();
                if (len == 4)
                {
                    this._LengthTracker[loc.Min] = this._LengthTracker[loc.Min] - 3;
                    var removeStart = loc.Min + xsedLoc + xsedFrame.HeaderLength + 1;
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + xsedLoc + 4,
                        sub: new byte[] { 1, 0 });
                    this._Instructions.SetRemove(
                        section: new RangeInt64(
                            removeStart,
                            removeStart + 2));
                    amount -= 3;
                }
            }

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                ProcessZeroFloats(dataRec, loc.Min, 6);
            }

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.XTEL, out var xtelFrame))
            {
                ProcessZeroFloats(xtelFrame, loc.Min, 6);
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
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
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                ProcessZeroFloats(dataRec, loc.Min, 6);
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
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
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                ProcessZeroFloats(dataRec, loc.Min, 6);
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessCells(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.CELL.Equals(recType)) return;
            CleanEmptyCellGroups(
                stream,
                formID,
                loc,
                numSubGroups: 3);
        }

        private void ProcessDialogTopics(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.DIAL.Equals(recType)) return;
            CleanEmptyDialogGroups(
                stream,
                formID,
                loc);
        }

        private void ProcessDialogItems(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.INFO.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            int amount = 0;
            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
            {
                this._Instructions.SetSubstitution(
                    loc: ctdt.Location + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: ctdt.Location + loc.Min + 0x1A);
                amount += 4;
            }

            foreach (var schd in majorFrame.FindEnumerateSubrecords(RecordTypes.SCHD))
            {
                stream.Position = loc.Min + schd.Location + 4;
                var existingLen = stream.ReadUInt16();
                var diff = existingLen - 0x14;
                this._Instructions.SetSubstitution(
                    loc: schd.Location + loc.Min + 3,
                    sub: new byte[] { (byte)'R', 0x14 });
                if (diff == 0) continue;
                var locToRemove = loc.Min + schd.Location + schd.HeaderLength + 0x14;
                this._Instructions.SetRemove(
                    section: new RangeInt64(
                        locToRemove,
                        locToRemove + diff - 1));
                amount -= diff;
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessIdleAnimations(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.IDLE.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            int amount = 0;
            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
            {
                this._Instructions.SetSubstitution(
                    loc: ctdt.Location + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: ctdt.Location + loc.Min + 0x1A);
                amount += 4;
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessAIPackages(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!RecordTypes.PACK.Equals(recType)) return;

            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            int amount = 0;
            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
            {
                this._Instructions.SetSubstitution(
                    loc: ctdt.Location + loc.Min + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: ctdt.Location + loc.Min + 0x1A);
                amount += 4;
            }

            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.PKDT))
            {
                if (ctdt.ContentLength != 4) continue;
                this._Instructions.SetSubstitution(
                    loc: loc.Min + ctdt.Location + 4,
                    sub: new byte[] { 0x8 });
                stream.Position = loc.Min + ctdt.Location + ctdt.HeaderLength;
                var first1 = stream.ReadUInt8();
                var first2 = stream.ReadUInt8();
                var second1 = stream.ReadUInt8();
                var second2 = stream.ReadUInt8();
                this._Instructions.SetSubstitution(
                    loc: loc.Min + ctdt.Location + 6,
                    sub: new byte[] { first1, first2, 0, 0 });
                this._Instructions.SetAddition(
                    loc: loc.Min + ctdt.Location + 10,
                    addition: new byte[] { second1, 0, 0, 0 });
                amount += 4;
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessCombatStyle(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!CombatStyle_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (majorFrame.TryLocateSubrecord(RecordTypes.CSTD, out var ctsd, out var ctsdLoc))
            {
                var len = ctsd.ContentLength;
                var move = 2;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + ctsdLoc + ctsd.HeaderLength + move);
                move = 38;
                if (len < 2 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + ctsdLoc + ctsd.HeaderLength + move);
                move = 53;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + ctsdLoc + ctsd.HeaderLength + 53);
                move = 69;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + ctsdLoc + ctsd.HeaderLength + 69);
                move = 82;
                if (len < 2 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: loc.Min + ctsdLoc + ctsd.HeaderLength + 82);
                move = 113;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: loc.Min + ctsdLoc + ctsd.HeaderLength + 113);
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
            var majorFrame = stream.ReadMajorRecordFrame();
            var amount = 0;
            if (majorFrame.TryLocateSubrecord(RecordTypes.DATA, out var dataRec, out var dataLoc))
            {
                var len = dataRec.ContentLength;
                if (len == 0x02)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataLoc + dataRec.HeaderLength,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x56)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x54, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataLoc + dataRec.HeaderLength + 0x54,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x2A)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x28, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataLoc + dataRec.HeaderLength + 0x28,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x3E)
                {
                    this._Instructions.SetSubstitution(
                        loc: loc.Min + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x3C, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: loc.Min + dataLoc + dataRec.HeaderLength + 0x3C,
                            length: 2));
                    amount -= 2;
                }

                var move = 0x39;
                if (len >= 3 + move)
                {
                    this._Instructions.SetSubstitution(
                        sub: new byte[] { 0, 0, 0 },
                        loc: loc.Min + dataLoc + dataRec.HeaderLength + move);
                }
            }

            ProcessLengths(
                majorFrame,
                amount,
                loc.Min);
        }

        private void ProcessGameSettings(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!GameSetting_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();

            var edidRec = majorFrame.LocateSubrecord("EDID", out var edidIndex);
            stream.Position = loc.Min + edidIndex + edidRec.HeaderLength;
            if ((char)stream.ReadUInt8() != 'f') return;

            if (majorFrame.TryLocateSubrecord(RecordTypes.DATA, out var dataRec, out var dataIndex))
            {
                dataIndex += dataRec.HeaderLength;
                ProcessZeroFloat(majorFrame, loc.Min, ref dataIndex);
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
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                var offset = 2;
                ProcessZeroFloats(dataRec, loc.Min, ref offset, 2);
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
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                var offset = 16;
                ProcessZeroFloats(dataRec, loc.Min, ref offset, 2);
                offset += 4;
                ProcessZeroFloat(dataRec, loc.Min, ref offset);
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
            var majorRecordFrame = stream.ReadMajorRecordFrame();
            foreach (var scit in majorRecordFrame.FindEnumerateSubrecords(RecordTypes.SCIT))
            {
                stream.Position = loc.Min + scit.Location + scit.HeaderLength;
                ProcessFormID(
                    stream,
                    pos: stream.Position);
            }
        }

        private void AlignRecords(
            IMutagenReadStream stream,
            RangeInt64 loc,
            IEnumerable<RecordType> rectypes)
        {
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            List<(RecordType rec, int sourceIndex, int loc)> list = new List<(RecordType rec, int sourceIndex, int loc)>();
            int recTypeIndex = -1;
            foreach (var rec in rectypes)
            {
                recTypeIndex++;
                if (!majorFrame.TryLocateSubrecord(rec, out var subRec, out var subLoc)) continue;
                list.Add((rec, recTypeIndex, subLoc));
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
                    len = checked((int)(majorFrame.TotalLength - item.loc));
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
                    data[index] = majorFrame.HeaderAndContentData[item.loc + index];
                }
                this._Instructions.SetSubstitution(
                    loc: start + loc.Min,
                    sub: data);
                start += len;
            }
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
        #endregion
    }
}
