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
            ProcessNPC(stream, recType, loc.Min);
            ProcessLeveledItemDataFields(stream, formID, recType, loc.Min);
            ProcessRegions(stream, formID, recType, loc.Min);
            ProcessPlacedObject(stream, formID, recType, loc.Min);
            ProcessPlacedCreature(stream, formID, recType, loc.Min);
            ProcessPlacedNPC(stream, formID, recType, loc.Min);
            ProcessCells(stream, formID, recType, loc.Min);
            ProcessDialogTopics(stream, formID, recType, loc.Min);
            ProcessDialogItems(stream, formID, recType, loc.Min);
            ProcessIdleAnimations(stream, formID, recType, loc.Min);
            ProcessAIPackages(stream, formID, recType, loc.Min);
            ProcessCombatStyle(stream, formID, recType, loc.Min);
            ProcessWater(stream, formID, recType, loc.Min);
            ProcessGameSettings(stream, formID, recType, loc.Min);
            ProcessBooks(stream, formID, recType, loc.Min);
            ProcessLights(stream, formID, recType, loc.Min);
            ProcessSpell(stream, formID, recType, loc.Min);
        }

        private void ProcessNPC(
            IMutagenReadStream stream,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.NPC_.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            this.DynamicMove(
                majorFrame,
                fileOffset,
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

        private void ProcessLeveledItemDataFields(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.LVLI.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (!majorFrame.TryLocateSubrecordFrame(RecordTypes.DATA, out var dataFrame, out var dataIndex)) return;

            int amount = 0;
            var dataFlag = dataFrame.AsUInt8();
            if (dataFlag == 1)
            {
                var lvld = majorFrame.LocateSubrecord(RecordTypes.LVLD, out var index);
                index += lvld.HeaderLength + 1;
                this._Instructions.SetAddition(
                    loc: index + fileOffset,
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
                var existingLen = dataFrame.ContentLength;
                byte[] lenData = new byte[2];
                using (var writer = new MutagenWriter(new MemoryStream(lenData), this.GameRelease))
                {
                    writer.Write((ushort)(existingLen - 7));
                }
                this._Instructions.SetSubstitution(
                    loc: fileOffset + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                    sub: lenData);
            }

            // Remove DATA
            var dataRange = new RangeInt64(dataIndex + fileOffset, dataIndex + fileOffset + 7 - 1);
            this._Instructions.SetRemove(dataRange);
            amount -= (int)dataRange.Width;

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessRegions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.REGN.Equals(recType)) return;
            stream.Position = fileOffset;
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
                        rdatIndex + fileOffset,
                        foundNext ? nextRdatIndex - 1 + fileOffset : fileOffset + majorFrame.TotalLength - 1);
                rdatFrame = nextRdatFrame;
                rdatIndex = nextRdatIndex;
            }

            foreach (var item in rdats.Reverse())
            {
                if (item.Key == (int)RegionData.RegionDataType.Icon) continue;
                this._Instructions.SetMove(
                    loc: fileOffset + majorFrame.TotalLength,
                    section: item.Value);
            }

            if (rdats.ContainsKey((int)RegionData.RegionDataType.Icon))
            { // Need to create icon record
                if (!majorFrame.TryLocateSubrecordFrame("EDID", out var edidFrame, out var edidLoc))
                {
                    throw new ArgumentException();
                }
                var locToPlace = fileOffset + edidLoc + edidFrame.TotalLength;

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
                fileOffset);
        }

        private void ProcessPlacedObject(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.REFR.Equals(recType)) return;

            int amount = 0;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.XLOC, out var xlocFrame, out var xlocLoc)
                && xlocFrame.ContentLength == 16)
            {
                this._LengthTracker[fileOffset] = this._LengthTracker[fileOffset] - 4;
                var removeStart = fileOffset + xlocLoc + xlocFrame.HeaderLength + 12;
                this._Instructions.SetSubstitution(
                    loc: fileOffset + xlocLoc + 4,
                    sub: new byte[] { 12, 0 });
                this._Instructions.SetRemove(
                    section: new RangeInt64(
                        removeStart,
                        removeStart + 3));
                amount -= 4;
            }
            if (majorFrame.TryLocateSubrecordFrame(RecordTypes.XSED, out var xsedFrame, out var xsedLoc))
            {
                var len = xsedFrame.ContentLength;
                if (len == 4)
                {
                    this._LengthTracker[fileOffset] = this._LengthTracker[fileOffset] - 3;
                    var removeStart = fileOffset + xsedLoc + xsedFrame.HeaderLength + 1;
                    this._Instructions.SetSubstitution(
                        loc: fileOffset + xsedLoc + 4,
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
                ProcessZeroFloats(dataRec, fileOffset, 6);
            }

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.XTEL, out var xtelFrame))
            {
                ProcessZeroFloats(xtelFrame, fileOffset, 6);
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessPlacedCreature(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!PlacedCreature_Registration.TriggeringRecordType.Equals(recType)) return;

            int amount = 0;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                ProcessZeroFloats(dataRec, fileOffset, 6);
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessPlacedNPC(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!PlacedNpc_Registration.TriggeringRecordType.Equals(recType)) return;

            int amount = 0;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                ProcessZeroFloats(dataRec, fileOffset, 6);
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessCells(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.CELL.Equals(recType)) return;
            CleanEmptyCellGroups(
                stream,
                formID,
                fileOffset,
                numSubGroups: 3);
        }

        private void ProcessDialogTopics(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.DIAL.Equals(recType)) return;
            CleanEmptyDialogGroups(
                stream,
                formID,
                fileOffset);
        }

        private void ProcessDialogItems(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.INFO.Equals(recType)) return;

            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            int amount = 0;
            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
            {
                this._Instructions.SetSubstitution(
                    loc: ctdt.Location + fileOffset + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: ctdt.Location + fileOffset + 0x1A);
                amount += 4;
            }

            foreach (var schd in majorFrame.FindEnumerateSubrecords(RecordTypes.SCHD))
            {
                var existingLen = schd.ContentLength;
                var diff = existingLen - 0x14;
                this._Instructions.SetSubstitution(
                    loc: schd.Location + fileOffset + 3,
                    sub: new byte[] { (byte)'R', 0x14 });
                if (diff == 0) continue;
                var locToRemove = fileOffset + schd.Location + schd.HeaderLength + 0x14;
                this._Instructions.SetRemove(
                    section: new RangeInt64(
                        locToRemove,
                        locToRemove + diff - 1));
                amount -= diff;
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessIdleAnimations(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.IDLE.Equals(recType)) return;

            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            int amount = 0;
            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
            {
                this._Instructions.SetSubstitution(
                    loc: ctdt.Location + fileOffset + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: ctdt.Location + fileOffset + 0x1A);
                amount += 4;
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessAIPackages(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!RecordTypes.PACK.Equals(recType)) return;

            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            int amount = 0;
            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
            {
                this._Instructions.SetSubstitution(
                    loc: ctdt.Location + fileOffset + 3,
                    sub: new byte[] { (byte)'A', 0x18 });
                this._Instructions.SetAddition(
                    addition: new byte[4],
                    loc: ctdt.Location + fileOffset + 0x1A);
                amount += 4;
            }

            foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.PKDT))
            {
                if (ctdt.ContentLength != 4) continue;
                this._Instructions.SetSubstitution(
                    loc: fileOffset + ctdt.Location + 4,
                    sub: new byte[] { 0x8 });
                var first1 = ctdt.Content[0];
                var first2 = ctdt.Content[1];
                var second1 = ctdt.Content[2];
                var second2 = ctdt.Content[3];
                this._Instructions.SetSubstitution(
                    loc: fileOffset + ctdt.Location + 6,
                    sub: new byte[] { first1, first2, 0, 0 });
                this._Instructions.SetAddition(
                    loc: fileOffset + ctdt.Location + 10,
                    addition: new byte[] { second1, 0, 0, 0 });
                amount += 4;
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessCombatStyle(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!CombatStyle_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            if (majorFrame.TryLocateSubrecord(RecordTypes.CSTD, out var ctsd, out var ctsdLoc))
            {
                var len = ctsd.ContentLength;
                var move = 2;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: fileOffset + ctsdLoc + ctsd.HeaderLength + move);
                move = 38;
                if (len < 2 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: fileOffset + ctsdLoc + ctsd.HeaderLength + move);
                move = 53;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: fileOffset + ctsdLoc + ctsd.HeaderLength + 53);
                move = 69;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: fileOffset + ctsdLoc + ctsd.HeaderLength + 69);
                move = 82;
                if (len < 2 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0 },
                    loc: fileOffset + ctsdLoc + ctsd.HeaderLength + 82);
                move = 113;
                if (len < 3 + move) return;
                this._Instructions.SetSubstitution(
                    sub: new byte[] { 0, 0, 0 },
                    loc: fileOffset + ctsdLoc + ctsd.HeaderLength + 113);
            }
        }

        private void ProcessWater(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!Water_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();
            var amount = 0;
            if (majorFrame.TryLocateSubrecord(RecordTypes.DATA, out var dataRec, out var dataLoc))
            {
                var len = dataRec.ContentLength;
                if (len == 0x02)
                {
                    this._Instructions.SetSubstitution(
                        loc: fileOffset + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: fileOffset + dataLoc + dataRec.HeaderLength,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x56)
                {
                    this._Instructions.SetSubstitution(
                        loc: fileOffset + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x54, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: fileOffset + dataLoc + dataRec.HeaderLength + 0x54,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x2A)
                {
                    this._Instructions.SetSubstitution(
                        loc: fileOffset + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x28, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: fileOffset + dataLoc + dataRec.HeaderLength + 0x28,
                            length: 2));
                    amount -= 2;
                }

                if (len == 0x3E)
                {
                    this._Instructions.SetSubstitution(
                        loc: fileOffset + dataLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                        sub: new byte[] { 0x3C, 0 });
                    this._Instructions.SetRemove(
                        section: RangeInt64.FactoryFromLength(
                            loc: fileOffset + dataLoc + dataRec.HeaderLength + 0x3C,
                            length: 2));
                    amount -= 2;
                }

                var move = 0x39;
                if (len >= 3 + move)
                {
                    this._Instructions.SetSubstitution(
                        sub: new byte[] { 0, 0, 0 },
                        loc: fileOffset + dataLoc + dataRec.HeaderLength + move);
                }
            }

            ProcessLengths(
                majorFrame,
                amount,
                fileOffset);
        }

        private void ProcessGameSettings(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!GameSetting_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();

            var edidRec = majorFrame.LocateSubrecordFrame("EDID");
            if ((char)edidRec.Content[0] != 'f') return;

            if (majorFrame.TryLocateSubrecord(RecordTypes.DATA, out var dataRec, out var dataIndex))
            {
                dataIndex += dataRec.HeaderLength;
                ProcessZeroFloat(majorFrame, fileOffset, ref dataIndex);
            }
        }

        private void ProcessBooks(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!Book_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                var offset = 2;
                ProcessZeroFloats(dataRec, fileOffset, ref offset, 2);
            }
        }

        private void ProcessLights(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!Light_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorFrame = stream.ReadMajorRecordFrame();

            if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec))
            {
                var offset = 16;
                ProcessZeroFloats(dataRec, fileOffset, ref offset, 2);
                offset += 4;
                ProcessZeroFloat(dataRec, fileOffset, ref offset);
            }
        }

        private void ProcessSpell(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            long fileOffset)
        {
            if (!SpellUnleveled_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = fileOffset;
            var majorRecordFrame = stream.ReadMajorRecordFrame();
            foreach (var scit in majorRecordFrame.FindEnumerateSubrecords(RecordTypes.SCIT))
            {
                ProcessFormIDOverflow(scit, fileOffset);
            }
        }
        #endregion
    }
}
