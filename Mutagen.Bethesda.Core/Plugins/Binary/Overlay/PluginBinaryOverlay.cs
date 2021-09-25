using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay
{
    public abstract class PluginBinaryOverlay
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
                if (groupMeta.ContentLength == 0)
                {
                    stream.Position += groupMeta.TotalLength;
                    continue;
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
                    recordParseCount[parsed.DuplicateParseMarker!.Value] = recordParseCount.GetOrAdd(parsed.DuplicateParseMarker!.Value) + 1;
                }
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = checked((int)minimumFinalPos);
                }
                lastParsed = parsed;
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
                    var minimumFinalPos = stream.Position + subMeta.TotalLength;
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
            IMajorRecordCommonGetter majorReference,
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
                    finalPos: finalPos,
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
                    recordParseCount[parsed.DuplicateParseMarker!.Value] = recordParseCount.GetOrAdd(parsed.DuplicateParseMarker!.Value) + 1;
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

        public static int[] ParseRecordLocations(
            OverlayStream stream,
            ICollectionGetter<RecordType> triggers,
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
                if (!triggers.Contains(recType)) break;
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
            ICollectionGetter<RecordType> triggers,
            ICollectionGetter<RecordType> includeTriggers,
            RecordHeaderConstants constants,
            bool skipHeader)
        {
            List<int> ret = new List<int>();
            var startingPos = stream.Position;
            while (!stream.Complete && stream.Position < finalPos)
            {
                var varMeta = constants.GetVariableMeta(stream);
                var recType = varMeta.RecordType;
                var trigger = triggers.Contains(recType);
                var includeTrigger = includeTriggers.Contains(recType);
                if (!trigger && !includeTrigger) break;
                if (trigger)
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

        public static int[] ParseRecordLocations(
            OverlayStream stream,
            long finalPos,
            RecordType trigger,
            ICollectionGetter<RecordType> includeTriggers,
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
                var includeTrigger = includeTriggers.Contains(recType);
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
            ICollectionGetter<RecordType> trigger,
            RecordHeaderConstants constants,
            bool skipHeader)
        {
            var ret = new List<int>();
            var set = new HashSet<RecordType>();
            var startingPos = stream.Position;
            while (!stream.Complete)
            {
                var varMeta = constants.GetVariableMeta(stream);
                var recType = varMeta.RecordType;
                if (trigger.Contains(recType))
                {
                    // If new record type we haven't seen before in our current record, just continue
                    if (set.Add(recType) && ret.Count > 0)
                    {
                        stream.Position += (int)varMeta.TotalLength;
                        continue;
                    }

                    // Otherwise mark as a new record location
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

                    // Clear set of seen types
                    set.Clear();
                    set.Add(recType);
                }
                else if (ret.Count == count)
                {
                    break;
                }
                else
                {
                    throw new ArgumentException($"Unexpected record encountered: {recType}. Was expecting: {string.Join(", ", trigger)}");
                }
            }
            return ret.ToArray();
        }

        public static int[] ParseRecordLocations(
            OverlayStream stream,
            long finalPos,
            ICollectionGetter<RecordType> triggers,
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
                var trigger = triggers.Contains(recType);
                var isIncludeTrigger = recType == includeTrigger;
                if (!trigger && !isIncludeTrigger) break;
                if (trigger)
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
            ICollectionGetter<RecordType> trigger,
            StreamTypedFactory<T> factory,
            TypedParseParams? parseParams)
        {
            var ret = new List<T>();
            while (!stream.Complete)
            {
                var subMeta = stream.GetSubrecord();
                var recType = subMeta.RecordType;
                if (!trigger.Contains(recType)) break;
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
            ICollectionGetter<RecordType> trigger,
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
}
