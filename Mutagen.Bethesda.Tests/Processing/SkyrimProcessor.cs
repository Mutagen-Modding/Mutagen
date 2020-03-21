using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Binary;
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
        }

        private void ProcessGameSettings(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType,
            RangeInt64 loc)
        {
            if (!GameSetting_Registration.TRIGGERING_RECORD_TYPE.Equals(recType)) return;
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
    }
}
