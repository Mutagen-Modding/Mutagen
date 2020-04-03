using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;

namespace Mutagen.Bethesda.Tests
{
    public class SkyrimProcessor : Processor
    {
        public override GameMode GameMode => GameMode.Skyrim;

        protected override void AddDynamicProcessorInstructions(IMutagenReadStream stream, FormID formID, RecordType recType)
        {
            base.AddDynamicProcessorInstructions(stream, formID, recType);
            var loc = this._AlignedFileLocs[formID];
            ProcessGameSettings(stream, formID, recType, loc);
            ProcessRaces(stream, formID, recType, loc);
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
            var edidLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("EDID"), navigateToContent: true);
            if (edidLoc == -1) return;
            if ((char)majorFrame.Content[edidLoc] != 'f') return;

            var dataIndex = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("DATA"), navigateToContent: true);
            if (dataIndex == -1) return;
            stream.Position = loc.Min + majorFrame.Header.HeaderLength + dataIndex;
            ProcessZeroFloat(stream);
        }

        private void ProcessRaces(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Race_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            ProcessPhonemes(stream, formID, majorFrame, loc);
        }

        private void ProcessPhonemes(
            IMutagenReadStream stream,
            FormID formID,
            MajorRecordFrame majorFrame,
            RangeInt64 loc)
        {
            var phonemeListLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("PHTN"));
            if (phonemeListLoc == -1) return;
            stream.Position = loc.Min + phonemeListLoc + majorFrame.Header.HeaderLength;

            var phonemeSpan = majorFrame.Content.Slice(phonemeListLoc);
            var finds = UtilityTranslation.FindRepeatingSubrecord(
                phonemeSpan, 
                this.Meta, 
                new RecordType("PHTN"),
                out var lenParsed);
            if (finds?.Length != 16) return;
            HashSet<FaceFxPhonemes.Target> targets = new HashSet<FaceFxPhonemes.Target>();
            targets.Add(EnumExt.GetValues<FaceFxPhonemes.Target>());
            foreach (var find in finds)
            {
                var subRecord = this.Meta.SubrecordFrame(phonemeSpan.Slice(find));
                var str = BinaryStringUtility.ProcessWholeToZString(subRecord.Content);
                var target = FaceFxPhonemesMixIn.GetTarget(str, out var lipMode);
                if (lipMode) return;
                targets.Remove(target);
            }
            if (targets.Count > 0) return;

            // Remove fully populated phonemes list
            this._Instructions.SetRemove(RangeInt64.FactoryFromLength(stream.Position, lenParsed));
            ModifyLengths(stream, -lenParsed, formID, loc.Min, null);
        }
    }
}
