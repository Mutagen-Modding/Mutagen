using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
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
            ProcessFurniture(stream, formID, recType, loc);
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
            if (edidLoc == null) return;
            if ((char)majorFrame.Content[edidLoc.Value] != 'f') return;

            var dataIndex = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, new RecordType("DATA"), navigateToContent: true);
            if (dataIndex == null) return;
            stream.Position = loc.Min + majorFrame.Header.HeaderLength + dataIndex.Value;
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
            if (phonemeListLoc == null) return;
            stream.Position = loc.Min + phonemeListLoc.Value + majorFrame.Header.HeaderLength;

            var phonemeSpan = majorFrame.Content.Slice(phonemeListLoc.Value);
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

        private void ProcessFurniture(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!Furniture_Registration.TriggeringRecordType.Equals(recType)) return;
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordMemoryFrame(readSafe: true);

            // Find and store marker data
            var data = new Dictionary<int, ReadOnlyMemorySlice<byte>>();
            var indices = new List<int>();
            var initialPos = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, stream.MetaData, Furniture_Registration.ENAM_HEADER);
            if (initialPos == null) return;
            var pos = initialPos.Value;
            while (pos < majorFrame.Content.Length)
            {
                var positions = UtilityTranslation.FindNextSubrecords(
                    majorFrame.Content.Slice(pos),
                    stream.MetaData,
                    out var lenParsed,
                    stopOnAlreadyEncounteredRecord: true,
                    new RecordType[]
                    {
                        Furniture_Registration.ENAM_HEADER,
                        new RecordType("NAM0"),
                        new RecordType("FNMK"),
                    });
                var enamPos = positions[0];
                if (enamPos == null) break;
                var enamFrame = stream.MetaData.SubrecordFrame(majorFrame.Content.Slice(pos + enamPos.Value));
                var index = BinaryPrimitives.ReadInt32LittleEndian(enamFrame.Content);
                data.Add(index, majorFrame.Content.Slice(pos + enamPos.Value, lenParsed));
                indices.Add(index);
                pos += lenParsed;
            }

            if (indices.SequenceEqual(indices.OrderBy(i => i))) return;
            byte[] reordered = new byte[data.Values.Select(l => l.Length).Sum()];
            int transferPos = 0;
            foreach (var index in indices.OrderBy(i => i))
            {
                var bytes = data[index];
                bytes.Span.CopyTo(reordered.AsSpan().Slice(transferPos));
                transferPos += bytes.Length;
            }
            this._Instructions.SetSubstitution(loc.Min + majorFrame.Header.HeaderLength + initialPos.Value, reordered);
        }
    }
}
