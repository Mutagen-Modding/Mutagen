using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public abstract class Processor
    {
        public abstract GameMode GameMode { get; }
        public readonly MetaDataConstants Meta;

        public Processor()
        {
            this.Meta = MetaDataConstants.Get(this.GameMode);
        }

        public virtual void PreProcessorJobs(
            IMutagenReadStream stream,
            RecordLocator.FileLocations fileLocs,
            BinaryFileProcessor.Config instructions,
            RecordLocator.FileLocations alignedFileLocs)
        {
        }

        public virtual void AddDynamicProcessorInstructions(
            IMutagenReadStream stream,
            byte numMasters,
            FormID formID,
            RecordType recType,
            BinaryFileProcessor.Config instr,
            RangeInt64 loc,
            RecordLocator.FileLocations fileLocs,
            Dictionary<long, uint> lengthTracker)
        {
        }

        //public void ProcessEDID(
        //    IMutagenReadStream stream,
        //    MetaDataConstants meta,
        //    RangeInt64 loc,
        //    BinaryFileProcessor.Config instr)
        //{
        //    stream.Position = loc.Min;
        //    var majorFrame = meta.ReadMajorRecordFrame(stream);
        //    var edidLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.ContentSpan, meta, Mutagen.Bethesda.Constants.EditorID);
        //    if (edidLoc == -1) return;
        //    ProcessStringTermination(
        //        stream,
        //        meta,
        //        loc.Min + majorFrame.Header.HeaderLength + edidLoc,
        //        instr);
        //}

        //public void ProcessStringTermination(
        //    IMutagenReadStream stream,
        //    MetaDataConstants meta,
        //    long subrecordLoc,
        //    FormID formID,
        //    BinaryFileProcessor.Config instr,
        //    RecordLocator.FileLocations fileLocs,
        //    Dictionary<long, uint> lengthTracker)
        //{
        //    stream.Position = subrecordLoc;
        //    var subFrame = meta.ReadSubRecordFrame(stream);
        //    var nullIndex = MemoryExtensions.IndexOf<byte>(subFrame.ContentSpan, default(byte));
        //    if (nullIndex == -1) throw new ArgumentException();
        //    if (nullIndex == subFrame.ContentSpan.Length - 1) return;
        //    // Extra content pass null terminator.  Trim
        //    instr.SetRemove(
        //        section: RangeInt64.FactoryFromLength(
        //            subrecordLoc + subFrame.Header.HeaderLength + nullIndex + 1,
        //            subFrame.ContentSpan.Length - nullIndex));
        //    ProcessLengths(
        //        stream: stream,
        //        amount: nullIndex + 1,
        //        loc: subrecordLoc,
        //        formID: formID,
        //        instr: instr,
        //        fileLocs: fileLocs,
        //        lengthTracker: lengthTracker);
        //}

        public void ProcessLengths(
            IMutagenReadStream stream,
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
            using (var writer = new MutagenWriter(new MemoryStream(lenData), this.GameMode))
            {
                writer.Write((ushort)(existingLen + amount));
            }
            instr.SetSubstitution(
                loc: loc.Min + Constants.HEADER_LENGTH,
                sub: lenData);
        }
    }
}
