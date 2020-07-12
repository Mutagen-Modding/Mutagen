using Microsoft.VisualBasic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Binary;
using Noggog;
using Noggog.Utility;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public abstract class Processor
    {
        public abstract GameRelease GameRelease { get; }
        public readonly GameConstants Meta;
        protected RecordLocator.FileLocations _SourceFileLocs;
        protected RecordLocator.FileLocations _AlignedFileLocs;
        protected BinaryFileProcessor.Config _Instructions = new BinaryFileProcessor.Config();
        protected Dictionary<long, uint> _LengthTracker = new Dictionary<long, uint>();
        protected byte _NumMasters;
        protected string SourcePath;
        protected TempFolder TempFolder;
        public bool DoMultithreading = true;
        public ModKey ModKey => ModKey.Factory(Path.GetFileName(SourcePath));

        public Processor()
        {
            this.Meta = GameConstants.Get(this.GameRelease);
        }

        public async Task Process(
            TempFolder tmpFolder,
            string sourcePath,
            string preprocessedPath,
            string outputPath,
            byte numMasters)
        {
            this.TempFolder = tmpFolder;
            this.SourcePath = sourcePath;
            this._NumMasters = numMasters;
            this._SourceFileLocs = RecordLocator.GetFileLocations(sourcePath, this.GameRelease);
            this._AlignedFileLocs = RecordLocator.GetFileLocations(preprocessedPath, this.GameRelease);

            var preprocessedBytes = File.ReadAllBytes(preprocessedPath);
            IMutagenReadStream streamGetter() => new MutagenMemoryReadStream(preprocessedBytes, this.GameRelease);
            using (var stream = streamGetter())
            {
                foreach (var grup in this._AlignedFileLocs.GrupLocations.And(this._AlignedFileLocs.ListedRecords.Keys))
                {
                    stream.Position = grup + 4;
                    this._LengthTracker[grup] = stream.ReadUInt32();
                }

                await this.PreProcessorJobs(streamGetter);

                await Task.WhenAll(ExtraJobs(streamGetter));

                foreach (var rec in this._SourceFileLocs.ListedRecords)
                {
                    this.AddDynamicProcessorInstructions(
                        stream: stream,
                        formID: rec.Value.FormID,
                        recType: rec.Value.Record);
                }

                foreach (var grup in this._LengthTracker)
                {
                    stream.Position = grup.Key + 4;
                    if (grup.Value == stream.ReadUInt32()) continue;
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
                    using var outStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                    processor.CopyTo(outStream);
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

        protected virtual async Task PreProcessorJobs(Func<IMutagenReadStream> streamGetter)
        {
        }

        protected virtual IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
        {
            yield return TaskExt.Run(DoMultithreading, () => RemoveEmptyGroups(streamGetter));
        }

        protected virtual void AddDynamicProcessorInstructions(
            IMutagenReadStream stream,
            FormID formID,
            RecordType recType)
        {
            var loc = this._AlignedFileLocs[formID];
            ProcessEDID(stream, loc);
            ProcessMajorRecordFormIDOverflow(stream, loc);
        }

        public void ProcessEDID(
            IMutagenReadStream stream,
            RangeInt64 loc)
        {
            stream.Position = loc.Min;
            var majorFrame = stream.ReadMajorRecordFrame();
            var edidLoc = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, this.Meta, Mutagen.Bethesda.Internals.Constants.EditorID);
            if (edidLoc == null) return;
            ProcessStringTermination(
                stream,
                loc.Min + majorFrame.HeaderLength + edidLoc.Value,
                majorFrame.FormID);
        }

        public void ProcessMajorRecordFormIDOverflow(
            IMutagenReadStream stream,
            RangeInt64 loc)
        {
            stream.Position = loc.Min;
            var majorMeta = stream.GetMajorRecord();
            var formID = majorMeta.FormID;
            if (formID.ModIndex.ID <= this._NumMasters) return;
            // Need to zero out master
            this._Instructions.SetSubstitution(
                loc.Min + this.Meta.MajorConstants.FormIDLocationOffset + 3,
                0);
        }

        public void ProcessFormIDOverflow(
            IMutagenReadStream stream,
            RangeInt64? loc)
        {
            var formID = new FormID(stream.ReadUInt32());
            if (formID.ModIndex.ID <= this._NumMasters) return;
            // Need to zero out master
            this._Instructions.SetSubstitution(
                (loc?.Min ?? 0) + stream.Position - 1,
                _NumMasters);
        }

        public void ProcessStringTermination(
            IMutagenReadStream stream,
            long subrecordLoc,
            FormID formID)
        {
            stream.Position = subrecordLoc;
            var subFrame = stream.ReadSubrecordFrame();
            var nullIndex = MemoryExtensions.IndexOf<byte>(subFrame.Content, default(byte));
            if (nullIndex == -1) throw new ArgumentException();
            if (nullIndex == subFrame.Content.Length - 1) return;
            // Extra content pass null terminator.  Trim
            this._Instructions.SetRemove(
                section: RangeInt64.FactoryFromLength(
                    subrecordLoc + subFrame.HeaderLength + nullIndex + 1,
                    subFrame.Content.Length - nullIndex));
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
            var subMeta = stream.ReadSubrecord();
            byte[] lenData = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(subMeta.ContentLength + amount));
            this._Instructions.SetSubstitution(
                loc: loc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                sub: lenData);
        }

        public void ModifyLengths(
            IMutagenReadStream stream,
            int amount,
            FormID formID,
            long recordLoc,
            long? subRecordLoc)
        {
            if (amount == 0) return;
            foreach (var k in this._AlignedFileLocs.GetContainingGroupLocations(formID))
            {
                this._LengthTracker[k] = (uint)(this._LengthTracker[k] + amount);
            }

            stream.Position = recordLoc;
            var majorMeta = stream.ReadMajorRecord();
            byte[] lenData = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(majorMeta.ContentLength + amount));
            this._Instructions.SetSubstitution(
                loc: recordLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                sub: lenData);

            if (subRecordLoc != null)
            {
                stream.Position = subRecordLoc.Value;
                var subMeta = stream.ReadSubrecord();
                lenData = new byte[2];
                BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(subMeta.ContentLength + amount));
                this._Instructions.SetSubstitution(
                    loc: subRecordLoc.Value + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                    sub: lenData);
            }
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

        public void ProcessColorFloat(IMutagenReadStream stream)
        {
            var inBytes = stream.ReadSpan(12);
            var color = IBinaryStreamExt.ReadColor(inBytes, ColorBinaryType.NoAlphaFloat);
            var outBytes = new byte[12];
            using (var writer = new MutagenWriter(new BinaryWriter(new MemoryStream(outBytes)), null!))
            {
                writer.Write(color, ColorBinaryType.NoAlphaFloat);
            }
            if (inBytes.SequenceEqual(outBytes)) return;
            this._Instructions.SetSubstitution(
                stream.Position - 12,
                outBytes);
        }

        public void RemoveEmptyGroups(Func<IMutagenReadStream> streamGetter)
        {
            using var stream = streamGetter();
            foreach (var loc in this._AlignedFileLocs.GrupLocations)
            {
                stream.Position = loc;
                var groupMeta = stream.ReadGroup();
                if (groupMeta.ContentLength != 0 || groupMeta.GroupType != 0) continue;
                this._Instructions.SetRemove(RangeInt64.FactoryFromLength(loc, groupMeta.HeaderLength));
            }
        }

        public int FixMissingCounters(
            MajorRecordFrame frame,
            RangeInt64 loc,
            RecordType counterType,
            RecordType containedType)
        {
            var pos = 0;
            bool prevWasCounter = false;
            int sizeChange = 0;
            while (pos < frame.Content.Length)
            {
                var subRec = frame.Meta.SubrecordFrame(frame.Content.Slice(pos));
                if (subRec.RecordType == counterType)
                {
                    prevWasCounter = true;
                    pos += subRec.TotalLength;
                    continue;
                }
                if (subRec.RecordType != containedType)
                {
                    prevWasCounter = false;
                    pos += subRec.TotalLength;
                    continue;
                }
                var passedLen = UtilityTranslation.SkipPastAll(frame.Content.Slice(pos), frame.Meta, containedType, out var numPassed);
                // Found contained record
                if (!prevWasCounter)
                {
                    byte[] bytes = new byte[10];
                    BinaryPrimitives.WriteInt32LittleEndian(bytes, counterType.TypeInt);
                    BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan().Slice(4), 4);
                    BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan().Slice(6), numPassed);
                    // Add counter
                    _Instructions.SetAddition(
                        loc.Min + frame.HeaderLength + pos,
                        bytes);
                    sizeChange += 10;

                }
                prevWasCounter = false;
                pos += passedLen;
            }
            return sizeChange;
        }

        public abstract class AStringsAlignment
        {
            public delegate void Handle(IMutagenReadStream stream, MajorRecordHeader major, BinaryFileProcessor.Config instr, List<KeyValuePair<uint, uint>> processedStrings, IStringsLookup overlay, ref uint newIndex);

            public RecordType MajorType;

            public abstract Handle Handler { get; }

            public static implicit operator AStringsAlignment(RecordType[] types)
            {
                return new StringsAlignmentTypical(types.Skip(1).ToArray())
                {
                    MajorType = types[0],
                };
            }

            public static void ProcessStringLink(
                IMutagenReadStream stream,
                BinaryFileProcessor.Config instr,
                List<KeyValuePair<uint, uint>> processedStrings,
                IStringsLookup overlay,
                ref uint newIndex)
            {
                var sub = stream.ReadSubrecord();
                if (sub.ContentLength != 4)
                {
                    throw new ArgumentException();
                }
                var curIndex = BinaryPrimitives.ReadUInt32LittleEndian(stream.GetSpan(4));
                if (!overlay.TryLookup(curIndex, out var str)
                    || string.IsNullOrEmpty(str))
                {
                    instr.SetSubstitution(stream.Position, new byte[4]);
                }
                else if (curIndex != 0)
                {
                    var assignedIndex = newIndex++;
                    processedStrings.Add(new KeyValuePair<uint, uint>(curIndex, assignedIndex));
                    byte[] b = new byte[4];
                    BinaryPrimitives.WriteUInt32LittleEndian(b, assignedIndex);
                    instr.SetSubstitution(stream.Position, b);
                }
                stream.Position -= sub.HeaderLength;
            }
        }

        public class StringsAlignmentCustom : AStringsAlignment
        {
            public override Handle Handler { get; }

            public StringsAlignmentCustom(RecordType majorType, Handle handler)
            {
                MajorType = majorType;
                Handler = handler;
            }
        }

        public class StringsAlignmentTypical : AStringsAlignment
        {
            public HashSet<RecordType> StringTypes = new HashSet<RecordType>();

            public StringsAlignmentTypical(RecordType[] types)
            {
                StringTypes.Add(types);
            }

            public override AStringsAlignment.Handle Handler => Align;

            private void Align(
                IMutagenReadStream stream,
                MajorRecordHeader major,
                BinaryFileProcessor.Config instr,
                List<KeyValuePair<uint, uint>> processedStrings,
                IStringsLookup overlay,
                ref uint newIndex)
            {
                var majorCompletePos = stream.Position + major.ContentLength;
                while (stream.Position < majorCompletePos)
                {
                    var sub = stream.GetSubrecord();
                    if (StringTypes.Contains(sub.RecordType))
                    {
                        ProcessStringLink(stream, instr, processedStrings, overlay, ref newIndex);
                    }
                    stream.Position += sub.TotalLength;
                }
            }
        }

        public IReadOnlyList<KeyValuePair<uint, uint>> RenumberStringsFileEntries(
            ModKey modKey,
            IMutagenReadStream stream,
            DirectoryInfo dataFolder,
            Language language,
            StringsSource source,
            params AStringsAlignment[] recordTypes)
        {
            var folderOverlay = StringsFolderLookupOverlay.TypicalFactory(dataFolder.FullName, null, modKey);
            var overlay = folderOverlay.Get(source)[language].Value;
            var ret = new List<KeyValuePair<uint, uint>>();
            var dict = new Dictionary<RecordType, AStringsAlignment>();
            foreach (var item in recordTypes)
            {
                dict[item.MajorType] = item;
            }

            stream.Position = 0;
            var mod = stream.ReadMod();
            if (!EnumExt.HasFlag(mod.Flags, Mutagen.Bethesda.Internals.Constants.LocalizedFlag)) return ListExt.Empty<KeyValuePair<uint, uint>>();

            stream.Position = 0;
            var locs = RecordLocator.GetFileLocations(
                stream,
                interest: new Mutagen.Bethesda.RecordInterest(dict.Keys));

            uint newIndex = 1;
            foreach (var rec in locs.ListedRecords)
            {
                stream.Position = rec.Key;
                var major = stream.ReadMajorRecord();
                if (!dict.TryGetValue(major.RecordType, out var instructions))
                {
                    continue;
                }
                instructions.Handler(stream, major, _Instructions, ret, overlay, ref newIndex);
            }

            return ret;
        }

        public void ProcessStringsFiles(
            ModKey modKey,
            DirectoryInfo dataFolder,
            Language language,
            StringsSource source,
            IReadOnlyList<KeyValuePair<uint, uint>> reindexing)
        {
            if (reindexing.Count == 0) return;

            var outFolder = Path.Combine(this.TempFolder.Dir.Path, "Strings/Processed");
            var stringsOverlay = StringsFolderLookupOverlay.TypicalFactory(dataFolder.FullName, null, modKey);
            using var writer = new StringsWriter(ModKey.Factory(Path.GetFileName(this.SourcePath)), outFolder);
            var dict = stringsOverlay.Get(source);
            foreach (var lang in dict)
            {
                if (lang.Key != language) continue;
                var overlay = lang.Value.Value;
                foreach (var item in reindexing)
                {
                    if (!overlay.TryLookup(item.Key, out var str))
                    {
                        throw new ArgumentException();
                    }
                    var regis = writer.Register(str, lang.Key, source);
                    if (item.Value != regis)
                    {
                        throw new ArgumentException();
                    }
                }
            }
        }

        public void CleanEmptyCellGroups(
            IMutagenReadStream stream,
            FormID formID,
            RangeInt64 loc,
            int numSubGroups)
        {
            List<RangeInt64> removes = new List<RangeInt64>();
            stream.Position = loc.Min;
            // Skip Major Record
            var majorHeader = stream.ReadMajorRecord();
            stream.Position += majorHeader.ContentLength;
            var blockGroupPos = stream.Position;
            if (!stream.TryReadGroup(out var blockGroup)) return;
            var blockGrupType = (GroupTypeEnum)blockGroup.GroupType;
            if (blockGrupType != GroupTypeEnum.CellChildren) return;
            if (blockGroup.ContentLength == 0)
            {
                removes.Add(RangeInt64.FactoryFromLength(blockGroupPos, blockGroup.HeaderLength));
            }
            else if (numSubGroups > 0)
            {
                var amountRemoved = 0;
                for (int i = 0; i < numSubGroups; i++)
                {
                    var subBlockGroupPos = stream.Position;
                    if (!stream.TryReadGroup(out var subBlockGroup)) break;
                    switch ((GroupTypeEnum)subBlockGroup.GroupType)
                    {
                        case GroupTypeEnum.CellPersistentChildren:
                        case GroupTypeEnum.CellTemporaryChildren:
                        case GroupTypeEnum.CellVisibleDistantChildren:
                            break;
                        default:
                            goto Break;
                    }
                    if (subBlockGroup.ContentLength == 0)
                    { // Empty group
                        this._LengthTracker[blockGroupPos] = checked((uint)(this._LengthTracker[blockGroupPos] - subBlockGroup.HeaderLength));
                        removes.Add(RangeInt64.FactoryFromLength(subBlockGroupPos, subBlockGroup.HeaderLength));
                        amountRemoved++;
                    }
                    stream.Position = subBlockGroupPos + subBlockGroup.TotalLength;
                }
                Break:

                // Check to see if removed subgroups left parent empty
                if (amountRemoved > 0
                    && blockGroup.ContentLength - (blockGroup.HeaderLength * amountRemoved) == 0)
                {
                    removes.Add(RangeInt64.FactoryFromLength(blockGroupPos, blockGroup.HeaderLength));
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

        public void CleanEmptyDialogGroups(
            IMutagenReadStream stream,
            FormID formID,
            RangeInt64 loc)
        {
            List<RangeInt64> removes = new List<RangeInt64>();
            stream.Position = loc.Min;
            // Skip Major Record
            var majorHeader = stream.ReadMajorRecord();
            stream.Position += majorHeader.ContentLength;
            var blockGroupPos = stream.Position;
            if (!stream.TryReadGroup(out var blockGroup)) return;
            var blockGrupType = (GroupTypeEnum)blockGroup.GroupType;
            if (blockGrupType != GroupTypeEnum.TopicChildren) return;
            if (blockGroup.ContentLength == 0)
            {
                removes.Add(RangeInt64.FactoryFromLength(blockGroupPos, blockGroup.HeaderLength));
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

        protected bool DynamicMove(
            MajorRecordFrame majorFrame,
            RangeInt64 loc,
            ICollection<RecordType> offendingIndices,
            ICollection<RecordType> offendingLimits,
            ICollection<RecordType> locationsToMove,
            bool enforcePast = false)
        {
            var offender = UtilityTranslation.FindFirstSubrecord(
                majorFrame.Content,
                majorFrame.Meta,
                recordTypes: offendingIndices.ToGetter());
            if (offender == null) return false;
            var limit = UtilityTranslation.FindFirstSubrecord(
                majorFrame.Content,
                majorFrame.Meta,
                recordTypes: offendingLimits.ToGetter());
            if (limit == null) return false;
            long? locToMove = UtilityTranslation.FindFirstSubrecord(
                majorFrame.Content.Slice(enforcePast ? offender.Value : 0),
                majorFrame.Meta,
                recordTypes: locationsToMove.ToGetter());
            if (locToMove == null)
            {
                locToMove = majorFrame.TotalLength;
            }
            if (limit == locToMove) return false;
            if (offender < limit)
            {
                if (locToMove < offender)
                {
                    throw new ArgumentException();
                }
                this._Instructions.SetMove(
                    section: new RangeInt64(
                        loc.Min + majorFrame.HeaderLength + offender,
                        loc.Min + majorFrame.HeaderLength + limit - 1),
                    loc: loc.Min + majorFrame.HeaderLength + locToMove.Value);
                return true;
            }
            return false;
        }
    }
}
