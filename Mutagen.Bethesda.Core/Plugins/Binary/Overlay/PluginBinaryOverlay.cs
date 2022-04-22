using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections.Generic;
using Loqui;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal abstract class PluginBinaryOverlay : ILoquiObject
{
    public delegate ParseResult RecordTypeFillWrapper(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        Dictionary<RecordType, int>? recordParseCount,
        TypedParseParams? parseParams);
    public delegate ParseResult ModTypeFillWrapper(
        IBinaryReadStream stream,
        long finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        TypedParseParams? parseParams);

    ILoquiRegistration ILoquiObject.Registration => throw new NotImplementedException();

    protected ReadOnlyMemorySlice<byte> _data;
    protected BinaryOverlayFactoryPackage _package;

    protected PluginBinaryOverlay(
        ReadOnlyMemorySlice<byte> bytes,
        BinaryOverlayFactoryPackage package)
    {
        this._data = bytes;
        this._package = package;
    }

    public static void FillModTypes(
        IBinaryReadStream stream,
        ModTypeFillWrapper fill,
        BinaryOverlayFactoryPackage package)
    {
        var lastParsed = new PreviousParse();
        ModHeader headerMeta = stream.GetModHeader(package);
        var minimumFinalPos = checked((int)(stream.Position + headerMeta.TotalLength));
        fill(
            stream: stream,
            finalPos: minimumFinalPos,
            offset: 0,
            type: headerMeta.RecordType,
            lastParsed: lastParsed,
            parseParams: null);
        stream.Position = (int)headerMeta.TotalLength;
        while (!stream.Complete)
        {
            GroupHeader groupMeta = stream.GetGroup(package);
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException("Did not see GRUP header as expected.");
            }
            minimumFinalPos = checked((int)(stream.Position + groupMeta.TotalLength));
            var parsed = fill(
                stream: stream,
                finalPos: minimumFinalPos,
                offset: 0,
                type: groupMeta.ContainedRecordType,
                lastParsed: lastParsed,
                parseParams: null);
            if (!parsed.KeepParsing) break;
            if (!parsed.KeepParsing) break;
            if (minimumFinalPos > stream.Position)
            {
                stream.Position = checked((int)minimumFinalPos);
            }
            lastParsed = parsed;
        }
    }

    public void FillMajorRecords(
        OverlayStream stream,
        int finalPos,
        int offset,
        TypedParseParams? parseParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            MajorRecordHeader majorMeta = stream.GetMajorRecord();
            try
            {
                var minimumFinalPos = stream.Position + majorMeta.TotalLength;
                var parsed = fill(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset,
                    recordParseCount: recordParseCount,
                    type: majorMeta.RecordType,
                    lastParsed: lastParsed,
                    parseParams: parseParams);
                if (!parsed.KeepParsing) break;
                if (parsed.DuplicateParseMarker != null)
                {
                    if (recordParseCount == null)
                    {
                        recordParseCount = new Dictionary<RecordType, int>();
                    }

                    recordParseCount[parsed.DuplicateParseMarker!.Value] =
                        recordParseCount.GetOrAdd(parsed.DuplicateParseMarker!.Value) + 1;
                }

                if (minimumFinalPos > stream.Position)
                {
                    if (minimumFinalPos > int.MaxValue || minimumFinalPos < 0)
                    {
                        throw new OverflowException(
                            $"Stream asked to to move to a position that was too large: {minimumFinalPos}.  Major Meta reported a TotalLength of {majorMeta.TotalLength}");
                    }
                    stream.Position = checked((int)minimumFinalPos);
                }

                lastParsed = parsed;
            }
            catch (Exception ex)
            {
                throw RecordException.Enrich(ex, FormKey.Factory(stream.MetaData.MasterReferences, majorMeta.FormID.ID), ((ILoquiObject)this).Registration.ClassType, edid: null);
            }
        }
    }

    public void FillGroupRecordsForWrapper(
        OverlayStream stream,
        int finalPos,
        int offset,
        TypedParseParams? parseParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            if (!stream.TryGetGroup(out var groupMeta))
            {
                throw new DataMisalignedException();
            }
            var subStream = new OverlayStream(stream.RemainingMemory.Slice(0, finalPos - stream.Position), stream.MetaData);
            var parsed = fill(
                stream: subStream,
                finalPos: subStream.Length,
                offset: 0, // unused 
                recordParseCount: recordParseCount,
                type: groupMeta.RecordType,
                lastParsed: lastParsed,
                parseParams: parseParams);
            stream.Position += subStream.Position;
            if (!parsed.KeepParsing) break;
            if (parsed.DuplicateParseMarker != null)
            {
                if (recordParseCount == null)
                {
                    recordParseCount = new Dictionary<RecordType, int>();
                }
                recordParseCount[parsed.DuplicateParseMarker!.Value] = recordParseCount.GetOrAdd(parsed.DuplicateParseMarker!.Value) + 1;
            }
            lastParsed = parsed;
        }
    }

    public void FillSubrecordTypes(
        OverlayStream stream,
        int finalPos,
        int offset,
        TypedParseParams? parseParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        RecordType? lastParsedType = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            try
            {
                SubrecordHeader subMeta = stream.GetSubrecord();
                lastParsedType = subMeta.RecordType;
                var minimumFinalPos = stream.Position;
                if (lastParsed.LengthOverride.HasValue)
                {
                    minimumFinalPos += lastParsed.LengthOverride.Value + subMeta.HeaderLength;
                }
                else
                {
                    minimumFinalPos += subMeta.TotalLength;
                }
                var parsed = fill(
                    stream: stream,
                    finalPos: minimumFinalPos,
                    recordParseCount: recordParseCount,
                    offset: offset,
                    type: lastParsedType.Value,
                    lastParsed: lastParsed,
                    parseParams: parseParams);
                if (!parsed.KeepParsing) break;
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = minimumFinalPos;
                }
                if (parsed.DuplicateParseMarker != null)
                {
                    if (recordParseCount == null)
                    {
                        recordParseCount = new Dictionary<RecordType, int>();
                    }
                    recordParseCount[parsed.DuplicateParseMarker!.Value] = recordParseCount.GetOrAdd(parsed.DuplicateParseMarker!.Value) + 1;
                }

                lastParsed = parsed;
            }
            catch (Exception e)
                when (lastParsedType != null)
            {
                throw SubrecordException.Enrich(e, lastParsedType.Value);
            }
        }
    }

    public void FillSubrecordTypes(
        IMajorRecordGetter majorReference,
        OverlayStream stream,
        int finalPos,
        int offset,
        TypedParseParams? parseParams,
        RecordTypeFillWrapper fill)
    {
        try
        {
            FillSubrecordTypes(
                stream: stream,
                finalPos: finalPos,
                offset: offset,
                parseParams: parseParams,
                fill: fill);
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, majorReference);
        }
    }

    public void FillTypelessSubrecordTypes(
        OverlayStream stream,
        int finalPos,
        int offset,
        TypedParseParams? parseParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            SubrecordHeader subMeta = stream.GetSubrecord();
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            var parsed = fill(
                stream: stream,
                finalPos: minimumFinalPos,
                offset: offset,
                recordParseCount: recordParseCount,
                type: subMeta.RecordType,
                lastParsed: lastParsed,
                parseParams: parseParams);
            if (!parsed.KeepParsing) break;
            if (minimumFinalPos > stream.Position)
            {
                stream.Position = minimumFinalPos;
            }
            if (parsed.DuplicateParseMarker != null)
            {
                if (recordParseCount == null)
                {
                    recordParseCount = new Dictionary<RecordType, int>();
                }
                recordParseCount[parsed.DuplicateParseMarker.Value] = recordParseCount.GetOrAdd(parsed.DuplicateParseMarker.Value) + 1;
            }
            lastParsed = parsed;
        }
    }

    public static int[] ParseRecordLocations(
        OverlayStream stream,
        RecordType trigger,
        RecordHeaderConstants constants,
        bool skipHeader,
        TypedParseParams? parseParams = null)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var varMeta = constants.GetVariableMeta(stream);
            var recType = parseParams.ConvertToStandard(varMeta.RecordType);
            if (recType != trigger) break;
            if (skipHeader)
            {
                stream.Position += varMeta.HeaderLength;
                ret.Add(stream.Position - startingPos);
                stream.Position += (int)varMeta.ContentLength;
            }
            else
            {
                ret.Add(stream.Position - startingPos);
                stream.Position += (int)varMeta.TotalLength;
            }
        }
        return ret.ToArray();
    }

    public static int[] ParseLocationsRecordPerTrigger(
        OverlayStream stream,
        RecordTriggerSpecs triggers,
        RecordHeaderConstants constants,
        bool skipHeader,
        TypedParseParams? parseParams = null)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var varMeta = constants.GetVariableMeta(stream);
            var recType = parseParams.ConvertToStandard(varMeta.RecordType);
            if (!triggers.TriggeringRecordTypes.Contains(recType)) break;
            if (skipHeader)
            {
                stream.Position += varMeta.HeaderLength;
                ret.Add(stream.Position - startingPos);
                stream.Position += (int)varMeta.ContentLength;
            }
            else
            {
                ret.Add(stream.Position - startingPos);
                stream.Position += (int)varMeta.TotalLength;
            }
        }
        return ret.ToArray();
    }

    public static int[] ParseRecordLocations(
        OverlayStream stream,
        long finalPos,
        RecordType trigger,
        RecordTriggerSpecs includeTriggers,
        RecordHeaderConstants constants,
        bool skipHeader)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete && stream.Position < finalPos)
        {
            var varMeta = constants.GetVariableMeta(stream);
            var recType = varMeta.RecordType;
            var isTrigger = trigger == recType;
            var includeTrigger = includeTriggers.TriggeringRecordTypes.Contains(recType);
            if (!isTrigger && !includeTrigger) break;
            if (isTrigger)
            {
                if (skipHeader)
                {
                    stream.Position += varMeta.HeaderLength;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.ContentLength;
                }
                else
                {
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.TotalLength;
                }
            }
            else
            {
                stream.Position += (int)varMeta.TotalLength;
            }
        }
        return ret.ToArray();
    }

    public static int[] ParseRecordLocationsByCount(
        OverlayStream stream,
        uint count,
        RecordType trigger,
        RecordHeaderConstants constants,
        bool skipHeader)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        for (uint i = 0; i < count;)
        {
            var varMeta = constants.GetVariableMeta(stream);
            var recType = varMeta.RecordType;
            if (trigger == recType)
            {
                if (skipHeader)
                {
                    stream.Position += varMeta.HeaderLength;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.ContentLength;
                }
                else
                {
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.TotalLength;
                }
                i++;
            }
            else
            {
                stream.Position += (int)varMeta.TotalLength;
            }

            if (stream.Complete)
            {
                break;
            }
        }
        return ret.ToArray();
    }
        
    private static int[] ParseRecordLocationsInternal(
        OverlayStream stream,
        uint? count,
        RecordTriggerSpecs trigger,
        RecordHeaderConstants constants,
        bool skipHeader,
        TypedParseParams? parseParams = null)
    {
        var ret = new List<int>();
        int? lastParsed = null;
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var varMeta = constants.GetVariableMeta(stream);
            var recType = parseParams.ConvertToStandard(varMeta.RecordType);
            var index = trigger.AllRecordTypes.IndexOf(recType);
            if (index != -1)
            {
                // If new record isn't before one we've already parsed, just continue
                if (lastParsed != null && lastParsed.Value < index)
                {
                    lastParsed = index;
                    stream.Position += (int)varMeta.TotalLength;
                    continue;
                }

                // Otherwise mark as a new record location
                if (trigger.AllAreTriggers
                    || trigger.TriggeringRecordTypes.Contains(recType))
                {
                    if (skipHeader)
                    {
                        stream.Position += varMeta.HeaderLength;
                        ret.Add(stream.Position - startingPos);
                        stream.Position += (int)varMeta.ContentLength;
                    }
                    else
                    {
                        ret.Add(stream.Position - startingPos);
                        stream.Position += (int)varMeta.TotalLength;
                    }
                }
                else
                {
                    stream.Position += (int)varMeta.TotalLength;
                }

                lastParsed = index;
            }
            else if (count.HasValue && ret.Count == count)
            {
                break;
            }
            else
            {
                // Unexpected count
                // Analyzer should warn about this, rather than Mutagen breaking
                break;
            }
        }
        return ret.ToArray();
    }

    /// <summary>
    /// Finds locations of a number of records given by count that match a set of record types.
    /// A new location is marked each time a record type that has already been encounterd is seen
    /// </summary>
    /// <param name="stream">Stream to read and progress</param>
    /// <param name="count">Number of expected records</param>
    /// <param name="trigger">Set of record types expected within one record</param>
    /// <param name="constants">Metadata for reference</param>
    /// <param name="skipHeader">Whether to skip the header in the return location values</param>
    /// <returns>Array of located positions relative to the stream's position at the start</returns>
    public static int[] ParseRecordLocationsByCount(
        OverlayStream stream,
        uint count,
        RecordTriggerSpecs trigger,
        RecordHeaderConstants constants,
        bool skipHeader,
        TypedParseParams? parseParams = null)
    {
        return ParseRecordLocationsInternal(stream, count, trigger, constants, skipHeader, parseParams);
    }

    /// <summary>
    /// Finds locations of a number of records given by count that match a set of record types.
    /// A new location is marked each time a record type that has already been encounterd is seen
    /// </summary>
    /// <param name="stream">Stream to read and progress</param>
    /// <param name="trigger">Set of record types expected within one record</param>
    /// <param name="constants">Metadata for reference</param>
    /// <param name="skipHeader">Whether to skip the header in the return location values</param>
    /// <returns>Array of located positions relative to the stream's position at the start</returns>
    public static int[] ParseRecordLocations(
        OverlayStream stream,
        RecordTriggerSpecs trigger,
        RecordHeaderConstants constants,
        bool skipHeader,
        TypedParseParams? parseParams = null)
    {
        return ParseRecordLocationsInternal(stream, count: null, trigger, constants, skipHeader, parseParams);
    }

    public static int[] ParseRecordLocations(
        OverlayStream stream,
        long finalPos,
        RecordType trigger,
        RecordType includeTrigger,
        RecordHeaderConstants constants,
        bool skipHeader)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete && stream.Position < finalPos)
        {
            var varMeta = constants.GetVariableMeta(stream);
            var recType = varMeta.RecordType;
            var isTrigger = trigger == recType;
            var isIncludeTrigger = includeTrigger == recType;
            if (!isTrigger && !isIncludeTrigger) break;
            if (isTrigger)
            {
                if (skipHeader)
                {
                    stream.Position += varMeta.HeaderLength;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.ContentLength;
                }
                else
                {
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.TotalLength;
                }
            }
            else
            {
                stream.Position += (int)varMeta.TotalLength;
            }
        }
        return ret.ToArray();
    }

    public delegate T Factory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package);

    public delegate T ConverterFactory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams? parseParams);

    public delegate T StreamFactory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package);

    public delegate T StreamTypedFactory<T>(
        OverlayStream stream,
        RecordType recordType,
        BinaryOverlayFactoryPackage package,
        TypedParseParams? parseParams);

    public delegate T SpanFactory<T>(
        ReadOnlyMemorySlice<byte> span,
        BinaryOverlayFactoryPackage package);

    public delegate T SpanRecordFactory<T>(
        ReadOnlyMemorySlice<byte> span,
        BinaryOverlayFactoryPackage package,
        TypedParseParams? parseParams);

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordTriggerSpecs trigger,
        StreamTypedFactory<T> factory,
        TypedParseParams? parseParams)
    {
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecord();
            var recType = subMeta.RecordType;
            if (!trigger.TriggeringRecordTypes.Contains(recType)) break;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            ret.Add(factory(stream, recType, _package, parseParams));
            if (stream.Position < minimumFinalPos)
            {
                stream.Position = minimumFinalPos;
            }
        }
        return ret;
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordTriggerSpecs trigger,
        ConverterFactory<T> factory,
        TypedParseParams? parseParams)
    {
        return ParseRepeatedTypelessSubrecord(
            stream,
            trigger,
            (s, r, p, recConv) => factory(s, p, recConv),
            parseParams);
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordType trigger,
        StreamTypedFactory<T> factory,
        TypedParseParams? parseParams)
    {
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecord();
            var recType = subMeta.RecordType;
            if (trigger != recType) break;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            ret.Add(factory(stream, recType, _package, parseParams));
            if (stream.Position < minimumFinalPos)
            {
                stream.Position = minimumFinalPos;
            }
        }
        return ret;
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordType trigger,
        SpanRecordFactory<T> factory,
        TypedParseParams? parseParams,
        bool skipHeader)
    {
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecord();
            var recType = subMeta.RecordType;
            if (trigger != recType) break;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            if (skipHeader)
            {
                stream.Position += subMeta.HeaderLength;
            }
            ret.Add(factory(stream.ReadMemory(skipHeader ? subMeta.ContentLength : subMeta.TotalLength), _package, parseParams));
            if (stream.Position < minimumFinalPos)
            {
                stream.Position = minimumFinalPos;
            }
        }
        return ret;
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordType trigger,
        ConverterFactory<T> factory,
        TypedParseParams? parseParams)
    {
        return ParseRepeatedTypelessSubrecord(
            stream,
            trigger,
            (s, r, p, recConv) => factory(s, p, recConv),
            parseParams);
    }

    public static ReadOnlyMemorySlice<byte> LockExtractMemory(IBinaryReadStream stream, long min, long max)
    {
        lock (stream)
        {
            stream.Position = min;
            var size = checked((int)(max - min));
            if (stream.IsPersistantBacking)
            {
                return stream.ReadMemory(size);
            }
            else
            {
                byte[] data = new byte[size];
                stream.Read(data);
                return data;
            }
        }
    }
}