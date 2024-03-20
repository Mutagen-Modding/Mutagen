using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Strings;
using Noggog;
using System.Buffers.Binary;
using System.Reactive.Subjects;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings.DI;
using System.Linq;

namespace Mutagen.Bethesda.Tests;

public abstract class Processor
{
    public abstract GameRelease GameRelease { get; }
    public readonly GameConstants Meta;
    protected RecordLocatorResults _alignedFileLocs;
    public BinaryFileProcessor.ConfigConstructor Instructions = new();
    private readonly Dictionary<long, uint> _lengthTracker = new();
    protected byte _numMasters;
    public IMasterReferenceCollection Masters;
    protected ParsingBundle Bundle;
    protected ModPath SourcePath;
    protected DirectoryPath TempFolder;
    public bool DoMultithreading = true;
    public ModKey ModKey => SourcePath.ModKey;
    protected DirectoryPath DataFolder => new DirectoryInfo(Path.GetDirectoryName(SourcePath));

    public delegate void DynamicProcessor(MajorRecordFrame majorFrame, long fileOffset);

    public delegate void DynamicStreamProcessor(IMutagenReadStream stream, MajorRecordFrame majorFrame,
        long fileOffset);

    protected Dictionary<RecordType, List<DynamicProcessor>> DynamicProcessors = new();
    protected Dictionary<RecordType, List<DynamicStreamProcessor>> DynamicStreamProcessors = new();
    public readonly ParallelOptions ParallelOptions;
    protected Subject<string> Logging;
    public abstract bool StrictStrings { get; }

    public virtual KeyValuePair<RecordType, FormKey>[] TrimmedRecords =>
        Array.Empty<KeyValuePair<RecordType, FormKey>>();

    public Processor(bool multithread)
    {
        Meta = GameConstants.Get(GameRelease);
        DoMultithreading = multithread;
        ParallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = multithread ? -1 : 1
        };
    }

    public void Init(
        ModPath sourcePath,
        string previousPath)
    {
        SourcePath = sourcePath;
        Masters = MasterReferenceCollection.FromPath(SourcePath, GameRelease); 
        Bundle = new ParsingBundle(GameRelease, Masters);
        _numMasters = checked((byte)Masters.Masters.Count);
        _alignedFileLocs = RecordLocator.GetLocations(new ModPath(ModKey, previousPath), GameRelease);
        using (var stream = new MutagenBinaryReadStream(File.OpenRead(previousPath), Bundle))
        {
            lock (_lengthTracker)
            {
                foreach (var grup in _alignedFileLocs.GrupLocations.Keys.And(_alignedFileLocs.ListedRecords.Keys))
                {
                    stream.Position = grup + 4;
                    _lengthTracker[grup] = stream.ReadUInt32();
                }
            }
        }
    }

    public async Task Process(
        DirectoryPath tmpFolder,
        Subject<string> logging,
        ModPath sourcePath,
        string preprocessedPath,
        string outputPath)
    {
        Logging = logging;
        TempFolder = tmpFolder;
        SourcePath = sourcePath;
        Init(sourcePath, preprocessedPath);

        IMutagenReadStream streamGetter() => new MutagenBinaryReadStream(File.OpenRead(preprocessedPath), Bundle);
        using (var stream = streamGetter())
        {
            await PreProcessorJobs(streamGetter);

            await Task.WhenAll(ExtraJobs(streamGetter));

            AddDynamicProcessorInstructions();
            Parallel.ForEach(DynamicProcessors.Keys
                    .And(DynamicStreamProcessors.Keys)
                    .And(RecordType.Null)
                    .Distinct(),
                ParallelOptions,
                type => ProcessDynamicType(type, streamGetter));

            ExecuteLengthTrackerChanges(stream);
        }

        var config = Instructions.GetConfig();

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

    public void ExecuteLengthTrackerChanges(IMutagenReadStream stream)
    {
        lock (_lengthTracker)
        {
            foreach (var grup in _lengthTracker)
            {
                stream.Position = grup.Key + 4;
                if (grup.Value == stream.ReadUInt32()) continue;
                Instructions.SetSubstitution(
                    loc: grup.Key + 4,
                    sub: BitConverter.GetBytes(grup.Value));
            }
        }
    }

    protected virtual async Task PreProcessorJobs(Func<IMutagenReadStream> streamGetter)
    {
    }

    protected virtual IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
    {
        yield return TaskExt.Run(DoMultithreading, () => RemoveEmptyGroups(streamGetter));

        if (GameRelease.ToCategory().HasLocalization())
        {
            yield return TaskExt.Run(DoMultithreading, () => RealignStrings(streamGetter));
        }

        yield return TaskExt.Run(DoMultithreading, () => RemoveDeletedContent(streamGetter));
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
        IEnumerable<KeyValuePair<long, RecordLocationMarker>> locs = _alignedFileLocs.ListedRecords;
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
                frame = stream.ReadMajorRecord(readSafe: true);
            }

            if (frame.IsDeleted) return;

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
        if (!majorFrame.TryFindSubrecord("EDID", out var edidFrame)) return;
        var formKey = FormKey.Factory(Masters, majorFrame.FormID.Raw);
        ProcessStringTermination(
            edidFrame,
            fileOffset + majorFrame.HeaderLength + edidFrame.Location,
            formKey);
    }

    public void ProcessMajorRecordFormIDOverflow(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formID = majorFrame.FormID;
        if (formID.ModIndex.ID <= _numMasters) return;
        // Need to zero out master
        Instructions.SetSubstitution(
            fileOffset + Meta.MajorConstants.FormIDLocationOffset + 3,
            0);
    }

    public void ProcessFormIDOverflow(ReadOnlySpan<byte> span, ref long offsetLoc)
    {
        var formID = new FormID(span.UInt32());
        if (formID.ModIndex.ID <= _numMasters) return;
        // Need to zero out master
        Instructions.SetSubstitution(
            offsetLoc + 3,
            _numMasters);
    }

    public FormID ProcessFormIDOverflow(FormID formId)
    {
        if (formId.ModIndex.ID <= _numMasters) return formId;
        return new FormID(new ModIndex(_numMasters), formId.ID);
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

    public bool ProcessFormIDOverflows(SubrecordPinFrame pin, long offsetLoc, ref int loc)
    {
        return ProcessFormIDOverflows(pin, offsetLoc, ref loc, amount: pin.ContentLength / 4);
    }

    public bool ProcessFormIDOverflows(SubrecordPinFrame pin, long offsetLoc)
    {
        int loc = 0;
        return ProcessFormIDOverflows(pin, offsetLoc, ref loc, amount: pin.ContentLength / 4);
    }

    public void ProcessStringTermination(
        SubrecordFrame subFrame,
        long refLoc,
        FormKey formKey)
    {
        var nullIndex = MemoryExtensions.IndexOf<byte>(subFrame.Content, default(byte));
        if (nullIndex == -1) throw new ArgumentException();
        if (nullIndex == subFrame.Content.Length - 1) return;
        // Extra content pass null terminator.  Trim 
        Instructions.SetRemove(
            section: RangeInt64.FromLength(
                refLoc + subFrame.HeaderLength + nullIndex + 1,
                subFrame.Content.Length - nullIndex));
        ProcessLengths(
            frame: subFrame,
            amount: nullIndex + 1,
            refLoc: refLoc,
            formKey: formKey);
    }

    public void ModifyParentGroupLengths(long amount, FormKey formKey)
    {
        if (amount == 0) return;
        lock (_lengthTracker)
        {
            foreach (var k in _alignedFileLocs.GetContainingGroupLocations(formKey))
            {
                _lengthTracker[k] = (uint)(_lengthTracker[k] + amount);
            }
        }
    }

    public void ModifyParentGroupLengths(int amount, GroupLocationMarker group)
    {
        if (amount == 0) return;
        lock (_lengthTracker)
        {
            foreach (var k in group.Parents)
            {
                _lengthTracker[k.Location.Min] = (uint)(_lengthTracker[k.Location.Min] + amount);
            }
        }
    }

    public void SwapSubrecordContent(long fileOffset, MajorRecordFrame majorFrame, SubrecordPinFrame subrecord,
        byte[] b)
    {
        var loc = fileOffset + subrecord.Location + subrecord.HeaderLength;
        if (b.Length == subrecord.ContentLength)
        {
            Instructions.SetSubstitution(loc, b);
        }
        else if (b.Length < subrecord.ContentLength)
        {
            Instructions.SetSubstitution(loc, b);
            var remLength = subrecord.ContentLength - b.Length;
            Instructions.SetRemove(RangeInt64.FromLength(loc + b.Length, remLength));
            ProcessLengths(
                majorFrame,
                subrecord,
                -remLength,
                fileOffset);
        }
        else
        {
            Instructions.SetSubstitution(loc, b.AsSpan().Slice(0, subrecord.ContentLength).ToArray());
            Instructions.SetAddition(loc + subrecord.ContentLength,
                b.AsSpan().Slice(subrecord.ContentLength).ToArray());

            ProcessLengths(
                majorFrame,
                subrecord,
                b.Length - subrecord.ContentLength,
                fileOffset);
        }
    }

    public void ProcessLengths(
        SubrecordFrame frame,
        int amount,
        long refLoc,
        FormKey formKey)
    {
        if (amount == 0) return;
        ModifyParentGroupLengths(amount, formKey);

        // Modify Length 
        byte[] lenData = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(frame.ContentLength + amount));
        Instructions.SetSubstitution(
            loc: refLoc + Constants.HeaderLength,
            sub: lenData);
    }

    public void ProcessLengths(
        MajorRecordFrame frame,
        long amount,
        long refLoc)
    {
        if (amount == 0) return;
        var formKey = FormKey.Factory(Masters, frame.FormID.Raw);
        ModifyParentGroupLengths(amount, formKey);

        // Modify Length 
        byte[] lenData = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(lenData.AsSpan(), checked((uint)(frame.ContentLength + amount)));
        Instructions.SetSubstitution(
            loc: refLoc + Constants.HeaderLength,
            sub: lenData);
    }

    public void Remove(
        MajorRecordFrame majorFrame,
        long refLoc)
    {
        Instructions.SetRemove(RangeInt64.FromLength(refLoc, majorFrame.TotalLength));
        
        var formKey = FormKey.Factory(Masters, majorFrame.FormID.Raw);
        ModifyParentGroupLengths(-majorFrame.TotalLength, formKey);
    }

    public void ProcessLengths(
        MajorRecordFrame frame,
        SubrecordPinFrame subRec,
        int amount,
        long refLoc)
    {
        if (amount == 0) return;
        var formKey = FormKey.Factory(Masters, frame.FormID.Raw);
        ModifyParentGroupLengths(amount, formKey);

        // Modify Length 
        byte[] lenData = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(lenData.AsSpan(), checked((uint)(frame.ContentLength + amount)));
        Instructions.SetSubstitution(
            loc: refLoc + Constants.HeaderLength,
            sub: lenData);

        if (subRec.LengthOverrideRecordLocation == null)
        {
            lenData = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(subRec.ContentLength + amount));
            Instructions.SetSubstitution(
                loc: refLoc + subRec.Location + Constants.HeaderLength,
                sub: lenData);
        }
        else
        {
            lenData = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(lenData.AsSpan(), (uint)(subRec.ContentLength + amount));
            Instructions.SetSubstitution(
                loc: refLoc + subRec.LengthOverrideRecordLocation.Value + subRec.HeaderLength,
                sub: lenData);
            
            lenData = new byte[2];
            Instructions.SetSubstitution(
                loc: refLoc + subRec.Location + Constants.HeaderLength,
                sub: lenData);
        }
    }

    public void ModifyLengths(
        IMutagenReadStream stream,
        int amount,
        FormKey formKey,
        long recordLoc,
        long? subRecordLoc)
    {
        if (amount == 0) return;
        lock (_lengthTracker)
        {
            foreach (var k in _alignedFileLocs.GetContainingGroupLocations(formKey))
            {
                _lengthTracker[k] = (uint)(_lengthTracker[k] + amount);
            }
        }

        stream.Position = recordLoc;
        var majorMeta = stream.ReadMajorRecordHeader();
        byte[] lenData = new byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(majorMeta.ContentLength + amount));
        Instructions.SetSubstitution(
            loc: recordLoc + Constants.HeaderLength,
            sub: lenData);

        if (subRecordLoc != null)
        {
            stream.Position = subRecordLoc.Value;
            var subMeta = stream.ReadSubrecordHeader();
            lenData = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(lenData.AsSpan(), (ushort)(subMeta.ContentLength + amount));
            Instructions.SetSubstitution(
                loc: subRecordLoc.Value + Constants.HeaderLength,
                sub: lenData);
        }
    }

    public bool RemoveUnnecessaryOverflowRecord(
        MajorRecordFrame record,
        SubrecordPinFrame subRecord,
        long fileOffset)
    {
        if (subRecord.LengthOverrideRecordLocation != null && subRecord.ContentLength < ushort.MaxValue)
        {
            RemoveOverflowRecord(record, subRecord, fileOffset, null);
            return true;
        }

        return false;
    }

    public void RemoveOverflowRecord(
        MajorRecordFrame record,
        SubrecordPinFrame subRecord,
        long fileOffset,
        uint? replacementLen)
    {
        var overflowRec =
            new SubrecordFrame(record.Meta, record.HeaderAndContentData.Slice(subRecord.LengthOverrideRecordLocation.Value));
        Instructions.SetRemove(RangeInt64.FromLength(fileOffset + subRecord.LengthOverrideRecordLocation.Value, 10));
        byte[] b = new byte[2];
        replacementLen ??= overflowRec.AsUInt32();
        BinaryPrimitives.WriteUInt16LittleEndian(b, checked((ushort)replacementLen));
        Instructions.SetSubstitution(fileOffset + subRecord.Location + Constants.HeaderLength, b);
    }

    public void ProcessZeroFloat(ReadOnlySpan<byte> span, ref long offsetLoc)
    {
        offsetLoc += 4;
        var f = span.Float();
        if (f == float.Epsilon)
        {
            Instructions.SetSubstitution(
                offsetLoc - 4,
                new byte[4]);
            return;
        }

        uint floatInt = span.UInt32();
        if (floatInt == 0x80000000)
        {
            Instructions.SetSubstitution(
                offsetLoc - 4,
                new byte[4]);
        }
    }

    public bool ProcessRotationFloat(SubrecordPinFrame pin, long offsetLoc, ref int loc, float multiplier = 57.2958f)
    {
        if (loc >= pin.ContentLength) return false;
        long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
        ProcessRotationFloat(pin.Content.Slice(loc), ref longLoc, multiplier);
        loc += 4;
        return true;
    }

    public void ProcessRotationFloat(ReadOnlySpan<byte> span, ref long offsetLoc, float multiplier)
    {
        var origFloat = span.Float();
        var multiplied = origFloat * multiplier;
        var newFloat = multiplied / multiplier;
        if (origFloat != newFloat)
        {
            offsetLoc += 4;
            var b = new byte[4];
            BinaryPrimitives.WriteSingleLittleEndian(b, newFloat);
            Instructions.SetSubstitution(
                offsetLoc - 4,
                b);
        }
        else
        {
            ProcessZeroFloat(span, ref offsetLoc);
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

    public bool ProcessZeroFloats(SubrecordPinFrame pin, long offsetLoc, ref int loc)
    {
        return ProcessZeroFloats(pin, offsetLoc, ref loc, pin.ContentLength / 4);
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

    public bool ProcessZeroFloats(SubrecordPinFrame pin, long offsetLoc)
    {
        int loc = 0;
        return ProcessZeroFloats(pin, offsetLoc, ref loc, pin.ContentLength / 4);
    }

    public bool ProcessZeroFloats(SubrecordPinFrame pin, long offsetLoc, ref int loc, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (!ProcessZeroFloat(pin, offsetLoc, ref loc)) return false;
        }

        return true;
    }

    public void ProcessColorFloat(ReadOnlySpan<byte> span, long offsetLoc, bool alpha)
    {
        var len = alpha ? 16 : 12;
        var type = alpha ? ColorBinaryType.AlphaFloat : ColorBinaryType.NoAlphaFloat;
        span = span.Slice(0, len);
        var color = IBinaryStreamExt.ReadColor(span, type);
        var outBytes = new byte[len];
        using (var writer = new MutagenWriter(new BinaryWriter(new MemoryStream(outBytes)), Meta))
        {
            writer.Write(color, type);
        }

        if (span.SequenceEqual(outBytes)) return;
        Instructions.SetSubstitution(
            offsetLoc,
            outBytes);
    }

    public bool ProcessColorFloat(SubrecordPinFrame pin, long offsetLoc, bool alpha)
    {
        int loc = 0;
        return ProcessColorFloat(pin, offsetLoc, ref loc, alpha);
    }

    public bool ProcessColorFloat(SubrecordPinFrame pin, long offsetLoc, ref int loc, bool alpha)
    {
        if (loc >= pin.ContentLength) return false;
        long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
        ProcessColorFloat(pin.Content.Slice(loc), longLoc, alpha: alpha);
        loc += alpha ? 16 : 12;
        return true;
    }

    public void ProcessColorFloats(SubrecordPinFrame pin, long offsetLoc, bool alpha)
    {
        int loc = 0;
        ProcessColorFloats(pin, offsetLoc, ref loc, alpha);
    }

    public void ProcessColorFloats(SubrecordPinFrame pin, long offsetLoc, ref int loc, bool alpha)
    {
        while (loc < pin.ContentLength)
        {
            long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
            ProcessColorFloat(pin.Content.Slice(loc), longLoc, alpha: alpha);
            loc += alpha ? 16 : 12;
        }
    }

    public void ProcessColorFloats(SubrecordPinFrame pin, long offsetLoc, ref int loc, bool alpha, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
            ProcessColorFloat(pin.Content.Slice(loc), longLoc, alpha: alpha);
            loc += alpha ? 16 : 12;
        }
    }

    public void ProcessBool(ReadOnlySpan<byte> span, long offsetLoc, byte importantBytes)
    {
        if (importantBytes != 1) throw new NotImplementedException();
        if (span[0] > 1)
        {
            Instructions.SetSubstitution(
                offsetLoc,
                1);
        }

        for (int i = 1; i < span.Length; i++)
        {
            if (span[i] != 0)
            {
                var outBytes = new byte[span.Length - 1];
                Instructions.SetSubstitution(
                    offsetLoc + 1,
                    outBytes);
                break;
            }
        }
    }

    public bool ProcessBool(SubrecordPinFrame pin, long offsetLoc, int loc, byte length, byte importantBytes)
    {
        if (loc >= pin.ContentLength) return false;
        long longLoc = offsetLoc + pin.Location + pin.HeaderLength + loc;
        ProcessBool(pin.Content.Slice(loc, length), longLoc, importantBytes);
        return true;
    }

    public void RemoveEndingBytes(SubrecordPinFrame subRec, long offsetLoc, int numBytes)
    {
        Instructions.SetRemove(RangeInt64.FromLength(
            offsetLoc + subRec.Location + subRec.HeaderLength + subRec.ContentLength - numBytes, 
            numBytes));
    }

    public void RemoveEmptyGroups(Func<IMutagenReadStream> streamGetter)
    {
        using var stream = streamGetter();
        foreach (var loc in _alignedFileLocs.GrupLocations.Keys)
        {
            stream.Position = loc;
            var groupMeta = stream.ReadGroupHeader();
            if (groupMeta.ContentLength != 0 || groupMeta.GroupType != 0) continue;
            Instructions.SetRemove(RangeInt64.FromLength(loc, groupMeta.HeaderLength));
        }
    }

    public void RemoveDeletedContent(Func<IMutagenReadStream> streamGetter)
    {
        using var stream = streamGetter();
        foreach (var loc in _alignedFileLocs.ListedRecords)
        {
            stream.Position = loc.Value.Location.Min;
            var majorFrame = stream.ReadMajorRecord();
            if (!majorFrame.IsDeleted || majorFrame.ContentLength == 0) continue;
            Instructions.SetRemove(RangeInt64.FromLength(loc.Value.Location.Min + majorFrame.HeaderLength,
                majorFrame.ContentLength));
            ProcessLengths(majorFrame, -checked((int)majorFrame.ContentLength), loc.Value.Location.Min);
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
            var subRec = frame.Meta.Subrecord(frame.Content.Slice(pos));
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

            var passedLen = RecordSpanExtensions.SkipPastAll(frame.Content.Slice(pos), frame.Meta, containedType,
                out var numPassed);
            // Found contained record
            if (!prevWasCounter)
            {
                byte[] bytes = new byte[10];
                BinaryPrimitives.WriteInt32LittleEndian(bytes, counterType.TypeInt);
                BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan().Slice(4), 4);
                BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan().Slice(6), numPassed);
                // Add counter
                Instructions.SetAddition(
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
        public delegate void Handle(long loc, MajorRecordFrame major, List<StringEntry> processedStrings,
            IStringsLookup overlay);

        public RecordType? MajorType;

        public abstract Handle Handler { get; }

        public static implicit operator AStringsAlignment(RecordType[] types)
        {
            return new StringsAlignmentTypical(types.Skip(1).ToArray())
            {
                MajorType = types[0],
            };
        }

        public static void ProcessStringLink(
            long loc,
            List<StringEntry> processedStrings,
            IStringsLookup overlay,
            MajorRecordFrame majorRecordHeader,
            SubrecordPinFrame sub)
        {
            if (sub.ContentLength != 4) return;
            var curIndex = sub.AsUInt32();
            if (!overlay.TryLookup(curIndex, out var str))
            {
                processedStrings.Add(
                    new StringEntry(
                        OrigIndex: curIndex,
                        FileLocation: loc + sub.Location + sub.HeaderLength,
                        Fill: false,
                        IsInDeletedRecord: majorRecordHeader.IsDeleted));
            }
            else if (curIndex != 0)
            {
                processedStrings.Add(
                    new StringEntry(
                        OrigIndex: curIndex,
                        FileLocation: loc + sub.Location + sub.HeaderLength,
                        Fill: true,
                        IsInDeletedRecord: majorRecordHeader.IsDeleted));
            }
        }
    }

    public class StringsAlignmentCustom : AStringsAlignment
    {
        public override Handle Handler { get; }

        public StringsAlignmentCustom(RecordType? majorType, Handle handler)
        {
            MajorType = majorType;
            Handler = handler;
        }
    }

    public class StringsAlignmentTypical : AStringsAlignment
    {
        public HashSet<RecordType> StringTypes = new();

        public StringsAlignmentTypical(RecordType[] types)
        {
            StringTypes.Add(types);
        }

        public override Handle Handler => Align;

        private void Align(
            long loc,
            MajorRecordFrame major,
            List<StringEntry> processedStrings,
            IStringsLookup overlay)
        {
            foreach (var sub in major.EnumerateSubrecords())
            {
                if (StringTypes.Contains(sub.RecordType))
                {
                    ProcessStringLink(loc, processedStrings, overlay, major, sub);
                }
            }
        }
    }

    public record StringEntry(uint OrigIndex, long FileLocation, bool Fill, bool IsInDeletedRecord)
    {
        public StringsSource Source { get; set; }
    }

    public async Task RealignStrings(Func<IMutagenReadStream> streamGetter)
    {
        using var stream = streamGetter();
        var modHeader = stream.GetModHeader();
        if (!Enums.HasFlag(modHeader.Flags, Constants.Localized)) return;
        var outFolder = Path.Combine(TempFolder, "Strings/Processed");
        var language = Language.English;
        using var writer = new StringsWriter(GameRelease, ModKey.FromNameAndExtension(Path.GetFileName(SourcePath)),
            outFolder, MutagenEncoding.Default);
        var stringEntries = Enums<StringsSource>.Values
            .SelectMany(source =>
            {
                return GetStringFileEntries(
                        stream,
                        language,
                        source,
                        GetStringsFileAlignments(source))
                    .Select(x => x with { Source = source });
            })
            .ToArray();

        var bsaOrder = Archive.GetIniListings(GameRelease).ToList();
        var stringsOverlay = StringsFolderLookupOverlay.TypicalFactory(GameRelease, ModKey, DataFolder,
            new StringsReadParameters()
            {
                BsaOrdering = bsaOrder
            });

        var overlays = Enums<StringsSource>.Values
            .Select(x => (x, stringsOverlay.Get(x)))
            .ToDictionary(x => x.x,
                x => (x.Item2, x.Item2.ToDictionary(x => x.Key, x => x.Value.Value.ToDictionary(x => x.Key, x => x.Value))));

        var deadKeys = KnownDeadStringKeys();

        foreach (var entry in stringEntries
                     .DistinctBy(x => x.FileLocation)
                     .OrderBy(x => x.FileLocation))
        {
            var knownDeadKeys = deadKeys?.GetOrDefault((ModKey, entry.Source));
            var dict = overlays[entry.Source];
            if (dict.Item1.TryGetValue(language, out var overlay))
            {
                var overlayDict = dict.Item2[language];
                if (knownDeadKeys != null)
                {
                    overlayDict.Remove(knownDeadKeys);
                }

                overlayDict.Remove(entry.OrigIndex);
                if (entry.Fill)
                {
                    if (!overlay.Value.TryLookup(entry.OrigIndex, out var str))
                    {
                        throw new ArgumentException();
                    }

                    var bytes = new byte[4];
                    if (!entry.IsInDeletedRecord)
                    {
                        var regis = writer.Register(entry.Source, new KeyValuePair<Language, string>(language, str));
                        BinaryPrimitives.WriteUInt32LittleEndian(bytes, regis);
                    }

                    Instructions.SetSubstitution(entry.FileLocation, bytes);
                    continue;
                }
            }

            Instructions.SetSubstitution(entry.FileLocation, new byte[4]);
        }

        if (StrictStrings)
        {
            foreach (var source in Enums<StringsSource>.Values)
            {
                var dict = overlays[source];
                var langDict = dict.Item2[language];
                if (langDict.Count > 0)
                {
                    foreach (var overlayStr in langDict.First(100))
                    {
                        Logging.OnNext($"Unaccounted for string: 0x{overlayStr.Key:X} {overlayStr.Value}");
                    }

                    throw new ArgumentException($"String unaccounted for in {source}: 0x{langDict.Keys.First():X}");
                }
            }
        }
    }

    private IReadOnlyList<StringEntry> GetStringFileEntries(
        IMutagenReadStream stream,
        Language language,
        StringsSource source,
        params AStringsAlignment[] recordTypes)
    {
        var folderOverlay = StringsFolderLookupOverlay.TypicalFactory(GameRelease, ModKey, DataFolder, null);
        var sourceDict = folderOverlay.Get(source);
        if (!sourceDict.TryGetValue(language, out var overlay)) return Array.Empty<StringEntry>();
        var ret = new List<StringEntry>();
        var stringAlignmentLookup = new Dictionary<RecordType, AStringsAlignment>();
        var stringAlignmentsForAll = new List<AStringsAlignment>();
        foreach (var item in recordTypes)
        {
            if (item.MajorType == null)
            {
                stringAlignmentsForAll.Add(item);
            }
            else
            {
                stringAlignmentLookup[item.MajorType.Value] = item;
            }
        }

        stream.Position = 0;
        var mod = stream.ReadModHeader();
        var localizedIndex = stream.MetaData.Constants.Release.ToCategory().GetLocalizedFlagIndex();
        if (!localizedIndex.HasValue || !Enums.HasFlag(mod.Flags, localizedIndex.Value))
            return Array.Empty<StringEntry>();

        stream.Position = 0;
        var locs = RecordLocator.GetLocations(stream);

        foreach (var rec in locs.ListedRecords)
        {
            stream.Position = rec.Key;
            var major = stream.GetMajorRecord();
            foreach (var alignment in stringAlignmentsForAll)
            {
                alignment.Handler(stream.Position, major, ret, overlay.Value);
            }
            if (stringAlignmentLookup.TryGetValue(major.RecordType, out var instructions))
            {
                instructions.Handler(stream.Position, major, ret, overlay.Value);
            }
        }

        return ret;
    }

    protected abstract AStringsAlignment[] GetStringsFileAlignments(StringsSource source);

    protected virtual Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>? KnownDeadStringKeys() => null;

    public void CleanEmptyCellGroups(
        IMutagenReadStream stream,
        FormKey formKey,
        long fileOffset,
        int numSubGroups)
    {
        List<RangeInt64> removes = new List<RangeInt64>();
        stream.Position = fileOffset;
        // Skip Major Record
        var majorHeader = stream.ReadMajorRecordHeader();
        stream.Position += majorHeader.ContentLength;
        var blockGroupPos = stream.Position;
        if (!stream.TryReadGroupHeader(out var blockGroup)) return;
        var blockGrupType = blockGroup.GroupType;
        if (blockGrupType != stream.MetaData.Constants.GroupConstants.Cell.TopGroupType) return;
        if (blockGroup.ContentLength == 0)
        {
            removes.Add(RangeInt64.FromLength(blockGroupPos, blockGroup.HeaderLength));
        }
        else if (numSubGroups > 0)
        {
            var amountRemoved = 0;
            for (int i = 0; i < numSubGroups; i++)
            {
                var subBlockGroupPos = stream.Position;
                if (!stream.TryReadGroupHeader(out var subBlockGroup)) break;
                if (!stream.MetaData.Constants.GroupConstants.Cell.SubTypes.Contains(subBlockGroup.GroupType))
                {
                    goto Break;
                }

                if (subBlockGroup.ContentLength == 0)
                {
                    // Empty group
                    ModifyLengthTracking(blockGroupPos, -subBlockGroup.HeaderLength);
                    removes.Add(RangeInt64.FromLength(subBlockGroupPos, subBlockGroup.HeaderLength));
                    amountRemoved++;
                }

                stream.Position = subBlockGroupPos + subBlockGroup.TotalLength;
            }

            Break:

            // Check to see if removed subgroups left parent empty
            if (amountRemoved > 0
                && blockGroup.ContentLength - (blockGroup.HeaderLength * amountRemoved) == 0)
            {
                removes.Add(RangeInt64.FromLength(blockGroupPos, blockGroup.HeaderLength));
            }
        }

        if (removes.Count == 0) return;

        int amount = 0;
        foreach (var remove in removes)
        {
            Instructions.SetRemove(
                section: remove);
            amount -= (int)remove.Width;
        }

        ModifyParentGroupLengths(amount, formKey);
    }

    public void CleanEmptyDialogGroups(
        IMutagenReadStream stream,
        FormKey formKey,
        long fileOffset)
    {
        List<RangeInt64> removes = new List<RangeInt64>();
        stream.Position = fileOffset;
        // Skip Major Record
        var majorHeader = stream.ReadMajorRecordHeader();
        stream.Position += majorHeader.ContentLength;
        var blockGroupPos = stream.Position;
        if (!stream.TryReadGroupHeader(out var blockGroup)) return;
        var blockGrupType = blockGroup.GroupType;
        if (blockGrupType != stream.MetaData.Constants.GroupConstants.Topic.TopGroupType) return;
        if (blockGroup.ContentLength == 0)
        {
            removes.Add(RangeInt64.FromLength(blockGroupPos, blockGroup.HeaderLength));
        }

        if (removes.Count == 0) return;

        int amount = 0;
        foreach (var remove in removes)
        {
            Instructions.SetRemove(
                section: remove);
            amount -= (int)remove.Width;
        }

        ModifyParentGroupLengths(amount, formKey);
    }

    public void CleanEmptyQuestGroups(
        IMutagenReadStream stream,
        FormKey formKey,
        long fileOffset)
    {
        List<RangeInt64> removes = new List<RangeInt64>();
        stream.Position = fileOffset;
        // Skip Major Record
        var majorHeader = stream.ReadMajorRecordHeader();
        stream.Position += majorHeader.ContentLength;
        var blockGroupPos = stream.Position;
        if (!stream.TryReadGroupHeader(out var blockGroup)) return;
        var blockGrupType = blockGroup.GroupType;
        if (blockGrupType != stream.MetaData.Constants.GroupConstants.Quest.TopGroupType) return;
        if (blockGroup.ContentLength == 0)
        {
            removes.Add(RangeInt64.FromLength(blockGroupPos, blockGroup.HeaderLength));
        }

        if (removes.Count == 0) return;

        int amount = 0;
        foreach (var remove in removes)
        {
            Instructions.SetRemove(
                section: remove);
            amount -= (int)remove.Width;
        }

        ModifyParentGroupLengths(amount, formKey);
    }

    protected bool DynamicMove(
        MajorRecordFrame majorFrame,
        long fileOffset,
        IReadOnlyCollection<RecordType> offendingIndices,
        IReadOnlyCollection<RecordType> offendingLimits,
        IReadOnlyCollection<RecordType> locationsToMove,
        bool enforcePast = false)
    {
        var offender = RecordSpanExtensions.TryFindSubrecord(
            majorFrame.Content,
            majorFrame.Meta,
            recordTypes: offendingIndices)?.Location;
        if (offender == null) return false;
        var limit = RecordSpanExtensions.TryFindSubrecord(
            majorFrame.Content,
            majorFrame.Meta,
            recordTypes: offendingLimits)?.Location;
        if (limit == null) return false;
        long? locToMove = RecordSpanExtensions.TryFindSubrecord(
            majorFrame.Content.Slice(enforcePast ? offender.Value : 0),
            majorFrame.Meta,
            recordTypes: locationsToMove)?.Location;
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

            Instructions.SetMove(
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
            _lengthTracker[pos] = checked((uint)(_lengthTracker[pos] + amount));
        }
    }
}