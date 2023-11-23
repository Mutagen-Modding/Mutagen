using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Loqui;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal abstract class PluginBinaryOverlay : ILoquiObject
{
    public readonly struct MemoryPair
    {
        public readonly ReadOnlyMemorySlice<byte> StructData;
        public readonly ReadOnlyMemorySlice<byte> RecordData;

        public MemoryPair(ReadOnlyMemorySlice<byte> structData, ReadOnlyMemorySlice<byte> recordData)
        {
            StructData = structData;
            RecordData = recordData;
        }

        public static MemoryPair StructMemory(ReadOnlyMemorySlice<byte> mem)
        {
            return new MemoryPair(structData: mem, recordData: ReadOnlyMemorySlice<byte>.Empty);
        }

        public static MemoryPair RecordMemory(ReadOnlyMemorySlice<byte> mem)
        {
            return new MemoryPair(structData: ReadOnlyMemorySlice<byte>.Empty, recordData: mem);
        }
    }
    
    public delegate ParseResult RecordTypeFillWrapper(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        Dictionary<RecordType, int>? recordParseCount,
        TypedParseParams translationParams);
    public delegate ParseResult ModTypeFillWrapper(
        IBinaryReadStream stream,
        long finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        TypedParseParams translationParams);

    ILoquiRegistration ILoquiObject.Registration => throw new NotImplementedException();

    internal ReadOnlyMemorySlice<byte> _structData;
    internal ReadOnlyMemorySlice<byte> _recordData;
    internal BinaryOverlayFactoryPackage _package;

    protected PluginBinaryOverlay(
        MemoryPair memoryPair,
        BinaryOverlayFactoryPackage package)
    {
        _structData = memoryPair.StructData;
        _recordData = memoryPair.RecordData;
        _package = package;
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
            translationParams: null);
        stream.Position = (int)headerMeta.TotalLength;
        while (!stream.Complete)
        {
            GroupHeader groupMeta = stream.GetGroupHeader(package);
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
                translationParams: null);
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
        TypedParseParams translationParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            MajorRecordHeader majorMeta = stream.GetMajorRecordHeader();
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
                    translationParams: translationParams);
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
        TypedParseParams translationParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            var groupMeta = stream.GetGroupHeader();
            var subStream = new OverlayStream(stream.RemainingMemory.Slice(0, finalPos - stream.Position), stream.MetaData);
            var parsed = fill(
                stream: subStream,
                finalPos: subStream.Length,
                offset: 0, // unused 
                recordParseCount: recordParseCount,
                type: groupMeta.RecordType,
                lastParsed: lastParsed,
                translationParams: translationParams);
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
        TypedParseParams translationParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        RecordType? lastParsedType = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            try
            {
                SubrecordHeader subMeta = stream.GetSubrecordHeader();
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
                    translationParams: translationParams);
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
        TypedParseParams translationParams,
        RecordTypeFillWrapper fill)
    {
        try
        {
            FillSubrecordTypes(
                stream: stream,
                finalPos: finalPos,
                offset: offset,
                translationParams: translationParams,
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
        TypedParseParams translationParams,
        RecordTypeFillWrapper fill)
    {
        var lastParsed = new PreviousParse();
        Dictionary<RecordType, int>? recordParseCount = null;
        while (!stream.Complete && stream.Position < finalPos)
        {
            SubrecordHeader subMeta = stream.GetSubrecordHeader();
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            var parsed = fill(
                stream: stream,
                finalPos: minimumFinalPos,
                offset: offset,
                recordParseCount: recordParseCount,
                type: subMeta.RecordType,
                lastParsed: lastParsed,
                translationParams: translationParams);
            if (!parsed.KeepParsing)
            {
                if (lastParsed.LengthOverride.HasValue)
                {
                    stream.Position -= stream.MetaData.Constants.SubConstants.HeaderLength + 4;
                }
                break;
            }
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
        TypedParseParams translationParams = default,
        // Not needed, just for generation simplification
        bool triggersAlwaysAreNewRecords = false)
    {
        translationParams = translationParams.ShortCircuit();
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var varMeta = stream.MetaData.Constants.GetVariableHeader(stream, constants);
            var recType = translationParams.ConvertToStandard(varMeta.RecordType);
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
        TypedParseParams translationParams = default)
    {
        translationParams = translationParams.ShortCircuit();
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var varMeta = stream.MetaData.Constants.GetVariableHeader(stream, constants);
            var recType = translationParams.ConvertToStandard(varMeta.RecordType);
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
            var varMeta = stream.MetaData.Constants.GetVariableHeader(stream, constants);
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
            var varMeta = stream.MetaData.Constants.GetVariableHeader(stream, constants);
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
        bool triggersAlwaysAreNewRecords = false,
        TypedParseParams translationParams = default)
    {
        translationParams = translationParams.ShortCircuit();
        var ret = new List<int>();
        int? lastParsed = null;
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var varMeta = stream.GetVariableHeader(subRecords: constants.LengthLength == 2);
            var recType = translationParams.ConvertToStandard(varMeta.RecordType);
            if (triggersAlwaysAreNewRecords)
            {
                if (trigger.AllRecordTypes.Contains(recType))
                {
                    // mark as a new record location
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
            else
            {
                var index = trigger.AllRecordTypes.IndexOf(recType);
                if (index != -1)
                {
                    // If new record isn't before one we've already parsed, just continue
                    if (!triggersAlwaysAreNewRecords
                        && lastParsed != null
                        && lastParsed.Value < index)
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
        }
        return ret.ToArray();
    }
        
    public static int[] ParseRecordLocationsEnder(
        OverlayStream stream,
        IReadOnlyRecordCollection startTriggers,
        IReadOnlyRecordCollection endTriggers,
        RecordHeaderConstants constants,
        bool skipHeader,
        TypedParseParams translationParams = default)
    {
        translationParams = translationParams.ShortCircuit();
        var ret = new List<int>();
        var startingPos = stream.Position;
        bool shouldAdd = true;
        while (!stream.Complete)
        {
            var varMeta = stream.GetVariableHeader(subRecords: constants.LengthLength == 2);
            var recType = translationParams.ConvertToStandard(varMeta.RecordType);

            if (endTriggers.Contains(recType))
            {
                shouldAdd = true;
                stream.Position += (int)varMeta.TotalLength;
            }
            else if (shouldAdd)
            {
                if (startTriggers.Contains(recType))
                {
                    shouldAdd = false;
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
                    break;
                }
            }
            else
            {
                stream.Position += (int)varMeta.TotalLength;
            }
        }

        return ret.ToArray();
    }


    /// <summary>
    /// Finds locations of a number of records given by count that match a set of record types.
    /// A new location is marked each time a record type that has already been encountered is seen
    /// </summary>
    /// <param name="stream">Stream to read and progress</param>
    /// <param name="count">Number of expected records</param>
    /// <param name="trigger">Set of record types expected within one record</param>
    /// <param name="constants">Metadata for reference</param>
    /// <param name="skipHeader">Whether to skip the header in the return location values</param>
    /// <param name="triggersAlwaysAreNewRecords">If false, RecordTypes that are triggers but not before the last parsed
    /// RecordType in the order type will be considered part of the last section</param>
    /// <returns>Array of located positions relative to the stream's position at the start</returns>
    public static int[] ParseRecordLocationsByCount(
        OverlayStream stream,
        uint count,
        RecordTriggerSpecs trigger,
        RecordHeaderConstants constants,
        bool skipHeader,
        bool triggersAlwaysAreNewRecords = false,
        TypedParseParams translationParams = default)
    {
        return ParseRecordLocationsInternal(stream, count, trigger, constants, skipHeader, triggersAlwaysAreNewRecords, translationParams);
    }

    /// <summary>
    /// Finds locations of a number of records given by count that match a set of record types.
    /// A new location is marked each time a record type that has already been encountered is seen
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
        bool triggersAlwaysAreNewRecords = false,
        TypedParseParams translationParams = default)
    {
        return ParseRecordLocationsInternal(stream, count: null, trigger, constants, skipHeader, triggersAlwaysAreNewRecords, translationParams);
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
            var varMeta = stream.MetaData.Constants.GetVariableHeader(stream, constants);
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
        TypedParseParams translationParams);

    public delegate T StreamFactory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package);

    public delegate T StreamTypedFactory<T>(
        OverlayStream stream,
        RecordType recordType,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams);

    public delegate T SpanFactory<T>(
        ReadOnlyMemorySlice<byte> span,
        BinaryOverlayFactoryPackage package);

    public delegate T SpanRecordFactory<T>(
        ReadOnlyMemorySlice<byte> span,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams);

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordTriggerSpecs trigger,
        StreamTypedFactory<T> factory,
        TypedParseParams translationParams)
    {
        translationParams = translationParams.ShortCircuit();
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecordHeader();
            var recType = subMeta.RecordType;
            if (!trigger.TriggeringRecordTypes.Contains(recType)) break;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            ret.Add(factory(stream, recType, _package, translationParams));
            if (stream.Position < minimumFinalPos)
            {
                stream.Position = minimumFinalPos;
            }
        }
        return ret;
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordType itemStartMarker,
        RecordType itemEndMarker,
        StreamTypedFactory<T> factory,
        TypedParseParams translationParams)
    {
        translationParams = translationParams.ShortCircuit();
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecordHeader();
            var recType = subMeta.RecordType;
            if (itemStartMarker != recType) break;
            stream.Position += stream.MetaData.Constants.SubConstants.HeaderLength;
            
            subMeta = stream.GetSubrecordHeader();
            recType = subMeta.RecordType;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            ret.Add(factory(stream, recType, _package, translationParams));
            
            stream.TryReadSubrecord(itemEndMarker, out _);
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
        TypedParseParams translationParams)
    {
        return ParseRepeatedTypelessSubrecord(
            stream,
            trigger,
            (s, r, p, recConv) => factory(s, p, recConv),
            translationParams);
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordType itemStartMarker,
        RecordType itemEndMarker,
        ConverterFactory<T> factory,
        TypedParseParams translationParams)
    {
        return ParseRepeatedTypelessSubrecord(
            stream,
            itemStartMarker: itemStartMarker,
            itemEndMarker: itemEndMarker,
            (s, r, p, recConv) => factory(s, p, recConv),
            translationParams);
    }

    public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
        OverlayStream stream,
        RecordType trigger,
        StreamTypedFactory<T> factory,
        TypedParseParams translationParams)
    {
        translationParams = translationParams.ShortCircuit();
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecordHeader();
            var recType = subMeta.RecordType;
            if (trigger != recType) break;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            ret.Add(factory(stream, recType, _package, translationParams));
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
        TypedParseParams translationParams,
        bool skipHeader)
    {
        translationParams = translationParams.ShortCircuit();
        var ret = new List<T>();
        while (!stream.Complete)
        {
            var subMeta = stream.GetSubrecordHeader();
            var recType = subMeta.RecordType;
            if (trigger != recType) break;
            var minimumFinalPos = stream.Position + subMeta.TotalLength;
            if (skipHeader)
            {
                stream.Position += subMeta.HeaderLength;
            }
            ret.Add(factory(stream.ReadMemory(skipHeader ? subMeta.ContentLength : subMeta.TotalLength), _package, translationParams));
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
        TypedParseParams translationParams)
    {
        return ParseRepeatedTypelessSubrecord(
            stream,
            trigger,
            (s, r, p, recConv) => factory(s, p, recConv),
            translationParams);
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

    public static MemoryPair ExtractSubrecordStructMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta, TypedParseParams translationParams = default)
    {
        var subMeta = meta.SubrecordHeader(span);
        return MemoryPair.StructMemory(span.Slice(subMeta.HeaderLength, translationParams.LengthOverride ?? subMeta.ContentLength));
    }

    public static MemoryPair ExtractRecordMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
    {
        var majorFrame = meta.MajorRecord(span);
        return new MemoryPair(majorFrame.HeaderData.Slice(meta.MajorConstants.TypeAndLengthLength), majorFrame.Content);
    }

    public static MemoryPair ExtractGroupMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
    {
        var groupFrame = meta.Group(span);
        return new MemoryPair(groupFrame.HeaderData.Slice(meta.MajorConstants.TypeAndLengthLength), groupFrame.Content);
    }

    public static OverlayStream ExtractSubrecordStructMemory(
        OverlayStream stream,
        GameConstants meta, 
        TypedParseParams translationParams,
        int length,
        out MemoryPair memoryPair,
        out int offset)
    {
        memoryPair = ExtractSubrecordStructMemory(stream.RemainingMemory.SliceUpTo(length), meta, translationParams);
        stream.Position += meta.SubConstants.HeaderLength;
        offset = meta.SubConstants.HeaderLength;
        return stream;
    }

    public static OverlayStream ExtractSubrecordStructMemory(
        OverlayStream stream,
        GameConstants meta, 
        TypedParseParams translationParams,
        out MemoryPair memoryPair,
        out int offset,
        out int finalPos)
    {
        memoryPair = ExtractSubrecordStructMemory(stream.RemainingMemory, meta, translationParams);
        stream.Position += meta.SubConstants.HeaderLength;
        offset = meta.SubConstants.HeaderLength;
        finalPos = stream.Position + memoryPair.RecordData.Length;
        return stream;
    }

    public static OverlayStream ExtractTypelessSubrecordStructMemory(
        OverlayStream stream,
        GameConstants meta, 
        TypedParseParams translationParams,
        out MemoryPair memoryPair,
        out int offset,
        out int finalPos)
    {
        memoryPair = MemoryPair.StructMemory(stream.RemainingMemory);
        offset = stream.Position;
        finalPos = stream.RemainingMemory.Length;
        return stream;
    }

    public static OverlayStream ExtractTypelessSubrecordStructMemory(
        OverlayStream stream,
        GameConstants meta, 
        TypedParseParams translationParams,
        int length,
        out MemoryPair memoryPair,
        out int offset)
    {
        memoryPair = MemoryPair.StructMemory(stream.RemainingMemory.SliceUpTo(length));
        offset = stream.Position;
        return stream;
    }

    public static OverlayStream ExtractTypelessSubrecordRecordMemory(
        OverlayStream stream,
        GameConstants meta, 
        TypedParseParams translationParams,
        out MemoryPair memoryPair,
        out int offset,
        out int finalPos)
    {
        memoryPair = MemoryPair.RecordMemory(stream.RemainingMemory);
        offset = stream.Position;
        finalPos = stream.RemainingMemory.Length;
        return stream;
    }

    public static OverlayStream ExtractRecordMemory(
        OverlayStream stream, 
        GameConstants meta, 
        out MemoryPair memoryPair,
        out int offset,
        out int finalPos)
    {
        // stream = Decompression.DecompressStream(stream);
        memoryPair = ExtractRecordMemory(stream.RemainingMemory, meta);
        stream.Position += meta.MajorConstants.HeaderLength;
        offset = meta.MajorConstants.HeaderLength;
        finalPos = stream.Position + memoryPair.RecordData.Length;
        return stream;
    }

    public static OverlayStream ExtractGroupMemory(
        OverlayStream stream,
        GameConstants meta, 
        out MemoryPair memoryPair,
        out int offset,
        out int finalPos)
    {
        memoryPair = ExtractGroupMemory(stream.RemainingMemory, meta);
        stream.Position += meta.GroupConstants.HeaderLength;
        offset = meta.GroupConstants.HeaderLength;
        finalPos = stream.Position + memoryPair.RecordData.Length;
        return stream;
    }
}