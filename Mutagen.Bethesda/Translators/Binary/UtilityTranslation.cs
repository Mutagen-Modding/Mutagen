using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class UtilityTranslation
    {
        public delegate void RecordStructFill<R>(
            R record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);

        public delegate TryGet<int?> RecordTypeFill<R>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter);

        public delegate TryGet<int?> RecordTypelessStructFill<R>(
            R record,
            MutagenFrame frame,
            int? lastParsed,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter);

        public delegate TryGet<int?> ModRecordTypeFill<R, G>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            G importMask,
            RecordTypeConverter recordTypeConverter);

        public delegate TryGet<int?> RecordTypeFillWrapper(
            BinaryMemoryReadStream stream,
            int offset,
            RecordType type,
            int? lastParsed);

        public static M MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            ErrorMaskBuilder errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
            where M : MajorRecord
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(
                frame.Reader,
                recType));
            fillStructs(
                record: record,
                frame: frame,
                masterReferences: masterReferences,
                errorMask: errorMask);
            if (fillTyped == null) return record;
            MutagenFrame targetFrame = frame;
            if (record.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
            {
                targetFrame = frame.Decompress();
            }
            while (!targetFrame.Complete)
            {
                var subMeta = frame.MetaData.GetSubRecord(targetFrame);
                var finalPos = targetFrame.Position + subMeta.TotalLength;
                var parsed = fillTyped(
                    record: record,
                    frame: targetFrame,
                    nextRecordType: subMeta.RecordType,
                    contentLength: subMeta.RecordLength,
                    masterReferences: masterReferences,
                    errorMask: errorMask,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (targetFrame.Position < finalPos)
                {
                    targetFrame.Position = finalPos;
                }
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static M RecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static M RecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var subMeta = frame.MetaData.GetSubRecord(frame);
                    var finalPos = frame.Position + subMeta.TotalLength;
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        nextRecordType: subMeta.RecordType,
                        contentLength: subMeta.RecordLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static M TypelessRecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static M TypelessRecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypelessStructFill<M> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                int? lastParsed = null;
                while (!frame.Complete)
                {
                    var subMeta = frame.MetaData.GetSubRecord(frame);
                    var finalPos = frame.Position + subMeta.TotalLength;
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        lastParsed: lastParsed,
                        nextRecordType: subMeta.RecordType,
                        contentLength: subMeta.RecordLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                    lastParsed = parsed.Value;
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static G GroupParse<G>(
            G record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<G> fillStructs,
            RecordTypeFill<G> fillTyped)
        {
            try
            {
                var groupMeta = frame.MetaData.GetGroup(frame);
                if (!groupMeta.IsGroup)
                {
                    throw new ArgumentException($"Expected GRUP header was not read in: {frame.Position}");
                }
                frame.Position += groupMeta.TypeAndLengthLength;
                frame = frame.ReadAndReframe(checked((int)(groupMeta.TotalLength - groupMeta.TypeAndLengthLength)));

                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    var finalPos = frame.Position + contentLength;
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static M ModParse<M, G>(
            M record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            G importMask,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            ModRecordTypeFill<M, G> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextType(
                        reader: frame.Reader,
                        finalPos: out var finalPos,
                        contentLength: out var contentLength);
                    var parsed = fillTyped(
                        record: record,
                        frame: frame,
                        importMask: importMask,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static void FillModTypesForWrapper(
            BinaryMemoryReadStream stream,
            MetaDataConstants meta,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            ModHeaderMeta headerMeta = meta.Header(stream.RemainingSpan);
            fill(
                stream: stream,
                offset: 0,
                type: headerMeta.RecordType,
                lastParsed: lastParsed);
            while (!stream.Complete)
            {
                GroupRecordMeta groupMeta = meta.Group(stream.RemainingSpan);
                if (!groupMeta.IsGroup)
                {
                    throw new ArgumentException("Did not see GRUP header as expected.");
                }
                var startPos = stream.Position;
                var parsed = fill(
                    stream: stream,
                    offset: 0,
                    type: groupMeta.ContainedRecordType,
                    lastParsed: lastParsed);
                if (parsed.Failed) break;
                if (startPos == stream.Position)
                {
                    stream.Position += checked((int)groupMeta.TotalLength);
                }
                lastParsed = parsed.Value;
            }
        }

        public static void FillRecordTypesForWrapper(
            BinaryMemoryReadStream stream,
            long finalPos,
            int offset,
            MetaDataConstants meta,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (stream.Position < finalPos)
            {
                MajorRecordMeta majorMeta = meta.MajorRecord(stream.RemainingSpan);
                var startPos = stream.Position;
                var parsed = fill(
                    stream: stream,
                    offset: offset,
                    type: recordTypeConverter.ConvertToStandard(majorMeta.RecordType),
                    lastParsed: lastParsed);
                if (parsed.Failed) break;
                if (startPos == stream.Position)
                {
                    stream.Position += checked((int)majorMeta.TotalLength);
                }
                lastParsed = parsed.Value;
            }
        }

        public static void FillSubrecordTypesForWrapper(
            BinaryMemoryReadStream stream,
            long finalPos,
            int offset,
            MetaDataConstants meta,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (stream.Position < finalPos)
            {
                SubRecordMeta subMeta = meta.SubRecord(stream.RemainingSpan);
                var startPos = stream.Position;
                var parsed = fill(
                    stream: stream,
                    offset: offset,
                    type: recordTypeConverter.ConvertToStandard(subMeta.RecordType),
                    lastParsed: lastParsed);
                if (parsed.Failed) break;
                if (startPos == stream.Position)
                {
                    stream.Position += checked((int)subMeta.TotalLength);
                }
                lastParsed = parsed.Value;
            }
        }

        public static void FillTypelessSubrecordTypesForWrapper(
            BinaryMemoryReadStream stream,
            int offset,
            MetaDataConstants meta,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete)
            {
                SubRecordMeta subMeta = meta.SubRecord(stream.RemainingSpan);
                var startPos = stream.Position;
                var parsed = fill(
                    stream: stream,
                    offset: offset,
                    type: recordTypeConverter.ConvertToStandard(subMeta.RecordType),
                    lastParsed: lastParsed);
                if (parsed.Failed) break;
                if (startPos == stream.Position)
                {
                    stream.Position += checked((int)subMeta.TotalLength);
                }
                lastParsed = parsed.Value;
            }
        }

        public static int[] ParseSubrecordLocations(
            BinaryMemoryReadStream stream,
            MetaDataConstants meta,
            RecordType trigger,
            bool skipHeader)
        {
            List<int> ret = new List<int>();
            var startingPos = stream.Position;
            while (!stream.Complete)
            {
                var subMeta = meta.GetSubRecord(stream);
                if (subMeta.RecordType != trigger) break;
                if (skipHeader)
                {
                    stream.Position += subMeta.HeaderLength;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += subMeta.RecordLength;
                }
                else
                {
                    ret.Add(stream.Position - startingPos);
                    stream.Position += subMeta.TotalLength;
                }
            }
            return ret.ToArray();
        }

        public delegate T BinaryWrapperFactory<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package);

        public delegate T BinaryWrapperConverterFactory<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter);

        public delegate T BinaryWrapperStreamTypedFactory<T>(
            BinaryMemoryReadStream stream,
            RecordType recordType,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter);

        public delegate T BinaryWrapperSpanFactory<T>(
            ReadOnlyMemorySlice<byte> span,
            BinaryWrapperFactoryPackage package);

        public delegate T BinaryWrapperSpanRecordFactory<T>(
            ReadOnlyMemorySlice<byte> span,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter);

        public static IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package,
            ICollectionGetter<RecordType> trigger,
            BinaryWrapperStreamTypedFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            var ret = new ReadOnlySetList<T>();
            while (!stream.Complete)
            {
                var subMeta = package.Meta.GetSubRecord(stream);
                var recType = subMeta.RecordType;
                if (!trigger.Contains(recType)) break;
                ret.Add(factory(stream, recType, package, recordTypeConverter));
            }
            return ret;
        }

        public static IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package,
            ICollectionGetter<RecordType> trigger,
            BinaryWrapperConverterFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            return ParseRepeatedTypelessSubrecord(
                stream,
                package,
                trigger,
                (s, r, p, recConv) => factory(s, p, recConv),
                recordTypeConverter);
        }

        public static IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package,
            RecordType trigger,
            BinaryWrapperStreamTypedFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            var ret = new ReadOnlySetList<T>();
            while (!stream.Complete)
            {
                var subMeta = package.Meta.GetSubRecord(stream);
                var recType = subMeta.RecordType;
                if (trigger != recType) break;
                ret.Add(factory(stream, recType, package, recordTypeConverter));
            }
            return ret;
        }

        public static IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package,
            RecordType trigger,
            BinaryWrapperConverterFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            return ParseRepeatedTypelessSubrecord(
                stream,
                package,
                trigger,
                (s, r, p, recConv) => factory(s, p, recConv),
                recordTypeConverter);
        }

        public static void FillEdidLinkCache<T>(IModGetter mod, RecordType recordType, BinaryWrapperFactoryPackage package)
            where T : IMajorRecordInternalGetter
        {
            var group = mod.GetGroupGetter<T>();
            var cache = new Dictionary<RecordType, object>();
            package.EdidLinkCache[recordType] = cache;
            foreach (var item in group)
            {
                var edid = item.Value.EditorID;
                if (edid.Length != Constants.HEADER_LENGTH)
                {
                    throw new ArgumentException($"EDID link record type {recordType} had an EDID of improper length: {edid}");
                }
                cache[new RecordType(edid)] = item.Value;
            }
        }

        /// <summary>
        /// Parses span data and enumerates pairs of record type -> locations
        /// 
        /// It is assumed the span contains only subrecords
        /// </summary>
        /// <param name="span">Bytes containing subrecords</param>
        /// <param name="meta">Metadata to use in subrecord parsing</param>
        /// <returns>Enumerable of KeyValue pairs of encountered RecordTypes and their locations relative to the input span</returns>
        public static IEnumerable<KeyValuePair<RecordType, int>> EnumerateSubrecords(ReadOnlyMemorySlice<byte> span, MetaDataConstants meta)
        {
            int loc = 0;
            while (span.Length > loc)
            {
                var subMeta = meta.SubRecord(span.Slice(loc));
                var len = subMeta.TotalLength;
                yield return new KeyValuePair<RecordType, int>(subMeta.RecordType, loc);
                loc += len;
            }
        }

        /// <summary>
        /// Locates the first encountered instances of all given subrecord types, and returns an array of their locations
        /// -1 represents a recordtype that was not found.
        /// 
        /// Not suggested to use with high numbers of record types, as it is an N^2 algorithm
        /// </summary>
        /// <param name="data">Subrecord data to be parsed</param>
        /// <param name="recordTypes">Record types to locate</param>
        /// <param name="meta">Metadata to use in subrecord parsing</param>
        /// <returns>Array of found record locations</returns>
        public static int[] FindFirstEncounteredSubrecords(ReadOnlySpan<byte> data, MetaDataConstants meta, params RecordType[] recordTypes)
        {
            int loc = 0;
            int[] ret = new int[recordTypes.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = -1;
            }
            while (data.Length > loc)
            {
                var subMeta = meta.SubRecord(data.Slice(loc));
                var recType = subMeta.RecordType;
                for (int i = 0; i < recordTypes.Length; i++)
                {
                    if (recordTypes[i] == recType && ret[i] == -1)
                    {
                        ret[i] = loc;
                        bool breakOut = false;
                        for (int j = 0; j < ret.Length; j++)
                        {
                            if (ret[j] == -1)
                            {
                                breakOut = true;
                                break;
                            }
                        }
                        if (breakOut)
                        {
                            break;
                        }
                    }
                }
                loc += subMeta.TotalLength;
            }
            return ret;
        }

        public static int FindFirstSubrecord(ReadOnlySpan<byte> data, MetaDataConstants meta, RecordType recordType)
        {
            int loc = 0;
            while (data.Length > loc)
            {
                var subMeta = meta.SubRecord(data.Slice(loc));
                if (subMeta.RecordType == recordType) return loc;
                loc += subMeta.TotalLength;
            }
            return -1;
        }
    }

    public static class UtilityAsyncTranslation
    {
        public delegate void RecordStructFill<R>(
            R record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);

        public delegate Task<TryGet<int?>> RecordTypeFill<R>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter);

        public delegate Task<TryGet<int?>> RecordTypelessStructFill<R>(
            R record,
            MutagenFrame frame,
            int? lastParsed,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter);

        public delegate Task<TryGet<int?>> ModRecordTypeFill<R, G>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            G importMask,
            RecordTypeConverter recordTypeConverter);

        public static async Task<M> MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            ErrorMaskBuilder errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
            where M : MajorRecord
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(
                frame.Reader,
                recType));
            fillStructs(
                record: record,
                frame: frame,
                masterReferences: masterReferences,
                errorMask: errorMask);
            if (fillTyped == null) return record;
            MutagenFrame targetFrame = frame;
            if (record.MajorRecordFlags.HasFlag(MajorRecord.MajorRecordFlag.Compressed))
            {
                targetFrame = frame.Decompress();
            }
            while (!targetFrame.Complete)
            {
                var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                    targetFrame.Reader,
                    contentLength: out var contentLength);
                var finalPos = targetFrame.Position + contentLength + frame.MetaData.SubConstants.HeaderLength;
                var parsed = await fillTyped(
                    record: record,
                    frame: targetFrame,
                    nextRecordType: nextRecordType,
                    contentLength: contentLength,
                    masterReferences: masterReferences,
                    errorMask: errorMask,
                    recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
                if (parsed.Failed) break;
                if (targetFrame.Position < finalPos)
                {
                    targetFrame.Position = finalPos;
                }
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static async Task<M> RecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static async Task<M> RecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    var finalPos = frame.Position + contentLength + frame.MetaData.SubConstants.HeaderLength;
                    var parsed = await fillTyped(
                        record: record,
                        frame: frame,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static async Task<M> TypelessRecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static async Task<M> TypelessRecordParse<M>(
            M record,
            MutagenFrame frame,
            bool setFinal,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypelessStructFill<M> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                int? lastParsed = null;
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                        reader: frame.Reader,
                        contentLength: out var contentLength);
                    var finalPos = frame.Position + contentLength + frame.MetaData.SubConstants.HeaderLength;
                    var parsed = await fillTyped(
                        record: record,
                        frame: frame,
                        lastParsed: lastParsed,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
                    if (parsed.Failed) break;
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                    lastParsed = parsed.Value;
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            if (setFinal)
            {
                frame.SetToFinalPosition();
            }
            return record;
        }

        public static async Task<G> GroupParse<G>(
            G record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<G> fillStructs,
            RecordTypeFill<G> fillTyped)
        {
            if (errorMask != null)
            {
                throw new NotImplementedException();
            }
            try
            {
                if (!HeaderTranslation.TryParse(
                    frame,
                    Mutagen.Bethesda.Constants.GRUP,
                    out var grupLen,
                    frame.MetaData.GroupConstants.LengthLength))
                {
                    throw new ArgumentException($"Expected header was not read in: {Mutagen.Bethesda.Constants.GRUP}");
                }
                var groupLen = checked((int)(grupLen - frame.MetaData.GroupConstants.TypeAndLengthLength));
                frame = frame.ReadAndReframe(groupLen);

                return await Task.Run(async () =>
                {
                    fillStructs?.Invoke(
                        record: record,
                        frame: frame,
                        masterReferences: masterReferences,
                        errorMask: errorMask);
                    while (!frame.Complete)
                    {
                        var nextRecordType = HeaderTranslation.GetNextSubRecordType(
                            reader: frame.Reader,
                            contentLength: out var contentLength);
                        var finalPos = frame.Position + contentLength;
                        var parsed = await fillTyped(
                            record: record,
                            frame: frame,
                            nextRecordType: nextRecordType,
                            contentLength: contentLength,
                            masterReferences: masterReferences,
                            errorMask: errorMask,
                            recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
                        if (parsed.Failed) break;
                        if (frame.Position < finalPos)
                        {
                            frame.Position = finalPos;
                        }
                    }
                    return record;
                });
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static async Task<M> ModParse<M, G>(
            M record,
            MutagenFrame frame,
            MasterReferences masterReferences,
            G importMask,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter,
            RecordStructFill<M> fillStructs,
            ModRecordTypeFill<M, G> fillTyped)
        {
            try
            {
                fillStructs?.Invoke(
                    record: record,
                    frame: frame,
                    masterReferences: masterReferences,
                    errorMask: errorMask);
                List<Task> tasks = new List<Task>();
                while (!frame.Complete)
                {
                    var nextRecordType = HeaderTranslation.GetNextType(
                        reader: frame.Reader,
                        finalPos: out var finalPos,
                        contentLength: out var contentLength);
                    tasks.Add(fillTyped(
                        record: record,
                        frame: frame,
                        importMask: importMask,
                        nextRecordType: nextRecordType,
                        contentLength: contentLength,
                        masterReferences: masterReferences,
                        errorMask: errorMask,
                        recordTypeConverter: recordTypeConverter));
                    if (frame.Position < finalPos)
                    {
                        frame.Position = finalPos;
                    }
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            frame.SetToFinalPosition();
            return record;
        }
    }
}
