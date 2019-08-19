using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Tests
{
    public abstract class Processor
    {
        public abstract GameMode GameMode { get; }
        public readonly MetaDataConstants Meta;
        protected RecordLocator.FileLocations _SourceFileLocs;
        protected RecordLocator.FileLocations _AlignedFileLocs;
        protected BinaryFileProcessor.Config _Instructions = new BinaryFileProcessor.Config();
        protected Dictionary<long, uint> _LengthTracker = new Dictionary<long, uint>();
        protected byte _NumMasters;

        public Processor()
        {
            this.Meta = MetaDataConstants.Get(this.GameMode);
        }

        public void Process(
            string sourcePath,
            string preprocessedPath,
            string outputPath,
            byte numMasters)
        {
            this._NumMasters = numMasters;
            this._SourceFileLocs = RecordLocator.GetFileLocations(sourcePath, this.GameMode);
            this._AlignedFileLocs = RecordLocator.GetFileLocations(preprocessedPath, this.GameMode);

            using (var reader = new MutagenBinaryReadStream(preprocessedPath, this.GameMode))
            {
                foreach (var grup in this._AlignedFileLocs.GrupLocations.And(this._AlignedFileLocs.ListedRecords.Keys))
                {
                    reader.Position = grup + 4;
                    this._LengthTracker[grup] = reader.ReadUInt32();
                }
            }

            using (var stream = new MutagenBinaryReadStream(preprocessedPath, this.GameMode))
            {
                this.PreProcessorJobs(stream);
                foreach (var rec in this._SourceFileLocs.ListedRecords)
                {
                    this.AddDynamicProcessorInstructions(
                        stream: stream,
                        formID: rec.Value.FormID,
                        recType: rec.Value.Record);
                }
            }

            using (var reader = new MutagenBinaryReadStream(preprocessedPath, this.GameMode))
            {
                foreach (var grup in this._LengthTracker)
                {
                    reader.Position = grup.Key + 4;
                    if (grup.Value == reader.ReadUInt32()) continue;
                    this._Instructions.SetSubstitution(
                        loc: grup.Key + 4,
                        sub: BitConverter.GetBytes(grup.Value));
                }
            }

            using (var processor = new BinaryFileProcessor(
                new FileStream(preprocessedPath, FileMode.Open, FileAccess.Read),
                this._Instructions))
            {
                try
                {
                    using (var outStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        processor.CopyTo(outStream);
                    }
                }
                catch (Exception)
                {
                    if (File.Exists(outputPath))
                    {
                        File.Delete(outputPath);
                    }
                    throw;
                }
            }
        }

        protected virtual void PreProcessorJobs(
            IMutagenReadStream stream)
        {
        }

        protected virtual void AddDynamicProcessorInstructions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType)
        {
            var loc = this._AlignedFileLocs[formID];
            ProcessEDID(stream, loc);
            ProcessFormIDOverflow(stream, loc);
        }

        public void ProcessEDID(
            IMutagenReadStream stream,
            RangeInt64 loc)
        {
            stream.Position = loc.Min;
            var majorFrame = this.Meta.ReadMajorRecordFrame(stream);
            var edidLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.ContentSpan, this.Meta, Mutagen.Bethesda.Constants.EditorID);
            if (edidLoc == -1) return;
            ProcessStringTermination(
                stream,
                loc.Min + majorFrame.Header.HeaderLength + edidLoc,
                majorFrame.Header.FormID);
        }

        public void ProcessFormIDOverflow(
            IMutagenReadStream stream,
            RangeInt64 loc)
        {
            stream.Position = loc.Min;
            var majorMeta = this.Meta.GetMajorRecord(stream);
            var formID = majorMeta.FormID;
            if (formID.ModID.ID < this._NumMasters) return;
            // Need to zero out master
            this._Instructions.SetSubstitution(
                loc.Min + this.Meta.MajorConstants.FormIDLocationOffset + 3,
                0);
        }

        public void ProcessStringTermination(
            IMutagenReadStream stream,
            long subrecordLoc,
            FormID formID)
        {
            stream.Position = subrecordLoc;
            var subFrame = this.Meta.ReadSubRecordFrame(stream);
            var nullIndex = MemoryExtensions.IndexOf<byte>(subFrame.ContentSpan, default(byte));
            if (nullIndex == -1) throw new ArgumentException();
            if (nullIndex == subFrame.ContentSpan.Length - 1) return;
            // Extra content pass null terminator.  Trim
            this._Instructions.SetRemove(
                section: RangeInt64.FactoryFromLength(
                    subrecordLoc + subFrame.Header.HeaderLength + nullIndex + 1,
                    subFrame.ContentSpan.Length - nullIndex));
            ProcessSubrecordLengths(
                stream: stream,
                amount: nullIndex + 1,
                loc: subrecordLoc,
                formID: formID);
        }

        public void ProcessSubrecordLengths(
            IMutagenReadStream stream,
            int amount,
            long loc,
            FormID formID,
            bool doRecordLen = true)
        {
            if (amount == 0) return;
            foreach (var k in this._AlignedFileLocs.GetContainingGroupLocations(formID))
            {
                this._LengthTracker[k] = (uint)(this._LengthTracker[k] + amount);
            }

            if (!doRecordLen) return;
            // Modify Length
            stream.Position = loc;
            var subMeta = this.Meta.ReadSubRecord(stream);
            byte[] lenData = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(subMeta.RecordLength + amount));
            this._Instructions.SetSubstitution(
                loc: loc + Constants.HEADER_LENGTH,
                sub: lenData);
        }
        
        public void ProcessZeroFloat(IMutagenReadStream stream)
        {
            var f = stream.ReadFloat();
            if (f == float.Epsilon)
            {
                this._Instructions.SetSubstitution(
                    stream.Position - 4,
                    new byte[4]);
                return;
            }
            stream.Position -= 4;
            uint floatInt = stream.ReadUInt32();
            if (floatInt == 0x80000000)
            {
                this._Instructions.SetSubstitution(
                    stream.Position - 4,
                    new byte[4]);
                return;
            }
        }
    }
}
