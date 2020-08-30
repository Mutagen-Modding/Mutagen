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
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public abstract class Processor
    {
        public abstract GameRelease GameRelease { get; }
        public readonly GameConstants Meta;
        protected RecordLocator.FileLocations _AlignedFileLocs;
        protected BinaryFileProcessor.ConfigConstructor _Instructions = new BinaryFileProcessor.ConfigConstructor();
        private Dictionary<long, uint> _lengthTracker = new Dictionary<long, uint>();
        protected byte _NumMasters;
        protected string SourcePath;
        protected TempFolder TempFolder;
        public bool DoMultithreading = true;
        public ModKey ModKey => ModKey.FromNameAndExtension(Path.GetFileName(SourcePath));
        public delegate void DynamicProcessor(MajorRecordFrame majorFrame, long fileOffset);
        public delegate void DynamicStreamProcessor(IMutagenReadStream stream, MajorRecordFrame majorFrame, long fileOffset);
        protected Dictionary<RecordType, List<DynamicProcessor>> DynamicProcessors = new Dictionary<RecordType, List<DynamicProcessor>>();
        protected Dictionary<RecordType, List<DynamicStreamProcessor>> DynamicStreamProcessors = new Dictionary<RecordType, List<DynamicStreamProcessor>>();
        public readonly ParallelOptions ParallelOptions;
        protected Subject<string> Logging;

        public Processor(bool multithread)
        {
            this.Meta = GameConstants.Get(this.GameRelease);
            this.DoMultithreading = multithread;
            this.ParallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = multithread ? -1 : 1
            };
        }

        private byte GetNumMasters()
        {
            using var stream = new MutagenBinaryReadStream(SourcePath, GameRelease);
            var modFrame = stream.ReadModHeaderFrame();
            return checked((byte)modFrame.Masters().Count());
        }

        public async Task Process(
            TempFolder tmpFolder,
            Subject<string> logging,
            string sourcePath,
            string preprocessedPath,
            string outputPath)
        {
            this.Logging = logging;
            this.TempFolder = tmpFolder;
            this.SourcePath = sourcePath;
            this._NumMasters = GetNumMasters();
            this._AlignedFileLocs = RecordLocator.GetFileLocations(preprocessedPath, this.GameRelease);

            var preprocessedBytes = File.ReadAllBytes(preprocessedPath);
            IMutagenReadStream streamGetter() => new MutagenMemoryReadStream(preprocessedBytes, this.GameRelease);
            using (var stream = streamGetter())
            {
                lock (_lengthTracker)
                {
                    foreach (var grup in this._AlignedFileLocs.GrupLocations.And(this._AlignedFileLocs.ListedRecords.Keys))
                    {
                        stream.Position = grup + 4;
                        this._lengthTracker[grup] = stream.ReadUInt32();
                    }
                }

                await this.PreProcessorJobs(streamGetter);

                await Task.WhenAll(ExtraJobs(streamGetter));

                this.AddDynamicProcessorInstructions();
                Parallel.ForEach(this.DynamicProcessors.Keys
                    .And(this.DynamicStreamProcessors.Keys)
                    .And(RecordType.Null)
                    .Distinct(),
                    ParallelOptions,
                    type => ProcessDynamicType(type, streamGetter));

                lock (_lengthTracker)
                {
                    foreach (var grup in this._lengthTracker)
                    {
                        stream.Position = grup.Key + 4;
                        if (grup.Value == stream.ReadUInt32()) continue;
                        this._Instructions.SetSubstitution(
                            loc: grup.Key + 4,
                            sub: BitConverter.GetBytes(grup.Value));
                    }
                }
            }

            var config = this._Instructions.GetConfig();

            using (var processor = new BinaryFileProcessor(
                new FileStream(preprocessedPath, FileMode.Open, FileAccess.Read),
                config))
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

        protected virtual void AddDynamicProcessorInstructions()
        {
            AddDynamicProcessing(RecordType.Null, ProcessEDID);
            AddDynamicProcessing(RecordType.Null, ProcessMajorRecordFormIDOverflow);
        }

        protected void AddDynamicProcessing(RecordType type, DynamicProcessor processor)
        {
            DynamicProcessors.GetOrAdd(type).Add(processor);
        }

        protected void AddDynamicProcessing(DynamicProcessor processor, params RecordType[] types)
        {
            foreach (var type in types)
            {
                DynamicProcessors.GetOrAdd(type).Add(processor);
            }
        }

        protected void AddDynamicProcessing(RecordType type, DynamicStreamProcessor processor)
        {
            DynamicStreamProcessors.GetOrAdd(type).Add(processor);
        }

        protected void ProcessDynamicType(RecordType type, Func<IMutagenReadStream> streamGetter)
        {
            IEnumerable<KeyValuePair<long, (FormID FormID, RecordType Record)>> locs = _AlignedFileLocs.ListedRecords;
            if (type != RecordType.Null)
            {
                locs = locs.Where(loc => loc.Value.Record == type);
            }
            if (!DynamicProcessors.TryGetValue(type, out var procs))
            {
                procs = null;
            }
            if (!DynamicStreamProcessors.TryGetValue(type, out var streamProcs))
            {
                streamProcs = null;
            }
            Parallel.ForEach(locs, ParallelOptions, (loc) =>
            {
                MajorRecordFrame frame;
                using (var stream = streamGetter())
                {
                    stream.Position = loc.Key;
                    frame = stream.ReadMajorRecordFrame(readSafe: true);
                }
                if (procs != null)
                {
                    foreach (var proc in procs)
                    {
                        proc(frame, loc.Key);
                    }
                }
                if (streamProcs != null)
                {
                    using var stream = streamGetter();
                    foreach (var proc in streamProcs)
                    {
                        proc(stream, frame, loc.Key);
                    }
                }
            });
        }

        public void ProcessEDID(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            if (!majorFrame.TryLocateSubrecordFrame("EDID", out var edidFrame, out var edidLoc)) return;
            ProcessStringTermination(
                edidFrame,
                fileOffset + majorFrame.HeaderLength + edidLoc,
                majorFrame.FormID);
        }

        public void ProcessMajorRecordFormIDOverflow(
            MajorRecordFrame majorFrame,
            long fileOffset)
        {
            var formID = majorFrame.FormID;
            if (formID.ModIndex.ID <= this._NumMasters) return;
            // Need to zero out master
            this._Instructions.SetSubstitution(
                fileOffset + this.Meta.MajorConstants.FormIDLocationOffset + 3,
                0);
        }

        public void ProcessFormIDOverflow(ReadOnlySpan<byte> span, ref long offsetLoc)
        {
            var formID = new FormID(span.UInt32());
            if (formID.ModIndex.ID <= this._NumMasters) return;
            // Need to zero out master
            this._Instructions.SetSubstitution(
                offsetLoc + 3,
                _NumMasters);
        }

        public bool ProcessFormIDOverflow(SubrecordPinFrame pin, long offsetLoc, ref int loc)
        {
            if (loc >= pin.ContentLength) return false;
            long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
            ProcessFormIDOverflow(pin.Content.Slice(loc), ref longLoc);
            loc += 4;
            return true;
        }

        public bool ProcessFormIDOverflow(SubrecordPinFrame pin, long offsetLoc)
        {
            int loc = 0;
            return ProcessFormIDOverflow(pin, offsetLoc, ref loc);
        }

        public bool ProcessFormIDOverflows(SubrecordPinFrame pin, long offsetLoc, ref int loc, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!ProcessFormIDOverflow(pin, offsetLoc, ref loc)) return false;
            }
            return true;
        }

        public void ProcessStringTermination(
            SubrecordFrame subFrame,
            long refLoc,
            FormID formID)
        {
            var nullIndex = MemoryExtensions.IndexOf<byte>(subFrame.Content, default(byte));
            if (nullIndex == -1) throw new ArgumentException();
            if (nullIndex == subFrame.Content.Length - 1) return;
            // Extra content pass null terminator.  Trim 
            this._Instructions.SetRemove(
                section: RangeInt64.FactoryFromLength(
                    refLoc + subFrame.HeaderLength + nullIndex + 1,
                    subFrame.Content.Length - nullIndex));
            ProcessLengths(
                frame: subFrame,
                amount: nullIndex + 1,
                refLoc: refLoc,
                formID: formID);
        }

        public void ModifyParentGroupLengths(int amount, FormID formID)
        {
            if (amount == 0) return;
            lock (_lengthTracker)
            {
                foreach (var k in this._AlignedFileLocs.GetContainingGroupLocations(formID))
                {
                    this._lengthTracker[k] = (uint)(this._lengthTracker[k] + amount);
                }
            }
        }

        public void ProcessLengths(
            SubrecordFrame frame,
            int amount,
            long refLoc,
            FormID formID)
        {
            if (amount == 0) return;
            ModifyParentGroupLengths(amount, formID);

            // Modify Length 
            byte[] lenData = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(frame.ContentLength + amount));
            this._Instructions.SetSubstitution(
                loc: refLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
                sub: lenData);
        }

        public void ProcessLengths(
            MajorRecordFrame frame,
            int amount,
            long refLoc)
        {
            if (amount == 0) return;
            ModifyParentGroupLengths(amount, frame.FormID);

            // Modify Length 
            byte[] lenData = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(lenData.AsSpan(), (ushort)(frame.ContentLength + amount));
            this._Instructions.SetSubstitution(
                loc: refLoc + Mutagen.Bethesda.Internals.Constants.HeaderLength,
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
            lock (_lengthTracker)
            {
                foreach (var k in this._AlignedFileLocs.GetContainingGroupLocations(formID))
                {
                    this._lengthTracker[k] = (uint)(this._lengthTracker[k] + amount);
                }
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

        public void ProcessZeroFloat(ReadOnlySpan<byte> span, ref long offsetLoc)
        {
            offsetLoc += 4;
            var f = span.Float();
            if (f == float.Epsilon)
            {
                this._Instructions.SetSubstitution(
                    offsetLoc - 4,
                    new byte[4]);
                return;
            }
            uint floatInt = span.UInt32();
            if (floatInt == 0x80000000)
            {
                this._Instructions.SetSubstitution(
                    offsetLoc - 4,
                    new byte[4]);
            }
        }

        public void ProcessZeroFloat(MajorRecordFrame frame, long offsetLoc, ref int loc)
        {
            long longLoc = offsetLoc + loc;
            ProcessZeroFloat(frame.HeaderAndContentData.Slice(loc), ref longLoc);
            loc += 4;
        }

        public void ProcessZeroFloats(MajorRecordFrame frame, long offsetLoc, ref int loc, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                ProcessZeroFloat(frame, offsetLoc, ref loc);
            }
        }

        public bool ProcessZeroFloat(SubrecordPinFrame pin, long offsetLoc, ref int loc)
        {
            if (loc >= pin.ContentLength) return false;
            long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
            ProcessZeroFloat(pin.Content.Slice(loc), ref longLoc);
            loc += 4;
            return true;
        }

        public bool ProcessZeroFloat(SubrecordPinFrame pin, long offsetLoc)
        {
            int loc = 0;
            return ProcessZeroFloat(pin, offsetLoc, ref loc);
        }

        public bool ProcessZeroFloats(SubrecordPinFrame pin, long offsetLoc, int amount)
        {
            int loc = 0;
            return ProcessZeroFloats(pin, offsetLoc, ref loc, amount);
        }

        public bool ProcessZeroFloats(SubrecordPinFrame pin, long offsetLoc, ref int loc, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!ProcessZeroFloat(pin, offsetLoc, ref loc)) return false;
            }
            return true;
        }

        public void ProcessColorFloat(ReadOnlySpan<byte> span, ref long offsetLoc)
        {
            span = span.Slice(0, 12);
            var color = IBinaryStreamExt.ReadColor(span, ColorBinaryType.NoAlphaFloat);
            var outBytes = new byte[12];
            using (var writer = new MutagenWriter(new BinaryWriter(new MemoryStream(outBytes)), null!))
            {
                writer.Write(color, ColorBinaryType.NoAlphaFloat);
            }
            if (span.SequenceEqual(outBytes)) return;
            this._Instructions.SetSubstitution(
                offsetLoc,
                outBytes);
        }

        public bool ProcessColorFloat(SubrecordPinFrame pin, long offsetLoc, ref int loc)
        {
            if (loc >= pin.ContentLength) return false;
            long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
            ProcessColorFloat(pin.Content.Slice(loc), ref longLoc);
            loc += 12;
            return true;
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
            long fileOffset,
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
                        fileOffset + frame.HeaderLength + pos,
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
            public delegate void Handle(IMutagenReadStream stream, MajorRecordHeader major, BinaryFileProcessor.ConfigConstructor instr, List<KeyValuePair<uint, uint>> processedStrings, IStringsLookup overlay, ref uint newIndex);

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
                BinaryFileProcessor.ConfigConstructor instr,
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
                if (!overlay.TryLookup(curIndex, out var str))
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
                BinaryFileProcessor.ConfigConstructor instr,
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
            var sourceDict = folderOverlay.Get(source);
            if (!sourceDict.TryGetValue(language, out var overlay)) return ListExt.Empty<KeyValuePair<uint, uint>>();
            var ret = new List<KeyValuePair<uint, uint>>();
            var dict = new Dictionary<RecordType, AStringsAlignment>();
            foreach (var item in recordTypes)
            {
                dict[item.MajorType] = item;
            }

            stream.Position = 0;
            var mod = stream.ReadModHeader();
            if (!EnumExt.HasFlag(mod.Flags, (int)ModHeaderCommonFlag.Localized)) return ListExt.Empty<KeyValuePair<uint, uint>>();

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
                instructions.Handler(stream, major, _Instructions, ret, overlay.Value, ref newIndex);
            }

            return ret;
        }

        public void ProcessStringsFiles(
            ModKey modKey,
            DirectoryInfo dataFolder,
            Language language,
            StringsSource source,
            bool strict,
            IReadOnlyList<KeyValuePair<uint, uint>> reindexing)
        {
            if (reindexing.Count == 0) return;

            var outFolder = Path.Combine(this.TempFolder.Dir.Path, "Strings/Processed");
            var stringsOverlay = StringsFolderLookupOverlay.TypicalFactory(dataFolder.FullName, null, modKey);
            using var writer = new StringsWriter(ModKey.FromNameAndExtension(Path.GetFileName(this.SourcePath)), outFolder);
            var dict = stringsOverlay.Get(source);
            foreach (var lang in dict)
            {
                if (lang.Key != language) continue;
                var overlay = lang.Value.Value;
                var overlayDict = strict ? overlay.ToDictionary() : null;
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
                    if (strict)
                    {
                        overlayDict.Remove(item.Key);
                    }
                }
                if (strict
                    && overlayDict.Count > 0)
                {
                    foreach (var str in overlayDict.First(100))
                    {
                        Logging.OnNext($"Unaccounted for string: {str.Key} {str.Value}");
                    }
                    throw new ArgumentException($"String unaccounted for: {overlayDict.Keys.First()}");
                }
            }
        }

        public void CleanEmptyCellGroups(
            IMutagenReadStream stream,
            FormID formID,
            long fileOffset,
            int numSubGroups)
        {
            List<RangeInt64> removes = new List<RangeInt64>();
            stream.Position = fileOffset;
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
                        ModifyLengthTracking(blockGroupPos, -subBlockGroup.HeaderLength);
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
            ModifyParentGroupLengths(amount, formID);
        }

        public void CleanEmptyDialogGroups(
            IMutagenReadStream stream,
            FormID formID,
            long fileOffset)
        {
            List<RangeInt64> removes = new List<RangeInt64>();
            stream.Position = fileOffset;
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
            ModifyParentGroupLengths(amount, formID);
        }

        protected bool DynamicMove(
            MajorRecordFrame majorFrame,
            long fileOffset,
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
                        fileOffset + majorFrame.HeaderLength + offender,
                        fileOffset + majorFrame.HeaderLength + limit - 1),
                    loc: fileOffset + majorFrame.HeaderLength + locToMove.Value);
                return true;
            }
            return false;
        }

        protected void ModifyLengthTracking(long pos, int amount)
        {
            lock (_lengthTracker)
            {
                this._lengthTracker[pos] = checked((uint)(this._lengthTracker[pos] + amount));
            }
        }
    }
}
