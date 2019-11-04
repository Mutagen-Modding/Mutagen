using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public abstract class BinaryWrapper
    {
        public delegate TryGet<int?> RecordTypeFillWrapper(
            BinaryMemoryReadStream stream,
            int finalPos,
            int offset,
            RecordType type,
            int? lastParsed,
            RecordTypeConverter recordTypeConverter);
        public delegate TryGet<int?> ModTypeFillWrapper(
            IBinaryReadStream stream,
            long finalPos,
            int offset,
            RecordType type,
            int? lastParsed,
            RecordTypeConverter recordTypeConverter);

        protected ReadOnlyMemorySlice<byte> _data;
        protected BinaryWrapperFactoryPackage _package;

        protected BinaryWrapper(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryWrapperFactoryPackage package)
        {
            this._data = bytes;
            this._package = package;
        }

        public static void FillModTypes(
            IBinaryReadStream stream,
            ModTypeFillWrapper fill,
            BinaryWrapperFactoryPackage package)
        {
            int? lastParsed = null;
            ModHeaderMeta headerMeta = package.Meta.GetHeader(stream);
            var minimumFinalPos = checked((int)(stream.Position + headerMeta.TotalLength));
            fill(
                stream: stream,
                finalPos: minimumFinalPos,
                offset: 0,
                type: headerMeta.RecordType,
                lastParsed: lastParsed,
                recordTypeConverter: null);
            stream.Position = (int)headerMeta.TotalLength;
            while (!stream.Complete)
            {
                GroupRecordMeta groupMeta = package.Meta.GetGroup(stream);
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
                    recordTypeConverter: null);
                if (parsed.Failed) break;
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = checked((int)minimumFinalPos);
                }
                lastParsed = parsed.Value;
            }
        }

        public void FillMajorRecords(
            BinaryMemoryReadStream stream,
            int finalPos,
            int offset,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                MajorRecordMeta majorMeta = _package.Meta.MajorRecord(stream.RemainingSpan);
                var minimumFinalPos = stream.Position + majorMeta.TotalLength;
                var parsed = fill(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset,
                    type: majorMeta.RecordType,
                    lastParsed: lastParsed,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = checked((int)minimumFinalPos);
                }
                lastParsed = parsed.Value;
            }
        }

        public void FillGroupRecordsForWrapper(
            BinaryMemoryReadStream stream,
            int finalPos,
            int offset,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                GroupRecordMeta groupMeta = _package.Meta.Group(stream.RemainingSpan);
                if (!groupMeta.IsGroup)
                {
                    throw new DataMisalignedException();
                }
                var minimumFinalPos = stream.Position + groupMeta.TotalLength;
                var parsed = fill(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset,
                    type: groupMeta.RecordType,
                    lastParsed: lastParsed,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = checked((int)minimumFinalPos);
                }
                lastParsed = parsed.Value;
            }
        }

        public void FillSubrecordTypes(
            BinaryMemoryReadStream stream,
            int finalPos,
            int offset,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                SubRecordMeta subMeta = _package.Meta.SubRecord(stream.RemainingSpan);
                var minimumFinalPos = stream.Position + subMeta.TotalLength;
                var parsed = fill(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset,
                    type: subMeta.RecordType,
                    lastParsed: lastParsed,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = minimumFinalPos;
                }
                lastParsed = parsed.Value;
            }
        }

        public void FillTypelessSubrecordTypes(
            BinaryMemoryReadStream stream,
            int finalPos,
            int offset,
            RecordTypeConverter recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                SubRecordMeta subMeta = _package.Meta.SubRecord(stream.RemainingSpan);
                var minimumFinalPos = stream.Position + subMeta.TotalLength;
                var parsed = fill(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset,
                    type: subMeta.RecordType,
                    lastParsed: lastParsed,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (minimumFinalPos > stream.Position)
                {
                    stream.Position = minimumFinalPos;
                }
                lastParsed = parsed.Value;
            }
        }

        public static int[] ParseRecordLocations(
            BinaryMemoryReadStream stream,
            long finalPos,
            RecordType trigger,
            RecordConstants constants,
            bool skipHeader)
        {
            List<int> ret = new List<int>();
            var startingPos = stream.Position;
            while (!stream.Complete && stream.Position < finalPos)
            {
                var varMeta = constants.GetVariableMeta(stream);
                if (varMeta.RecordType != trigger) break;
                if (skipHeader)
                {
                    stream.Position += varMeta.HeaderLength;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.RecordLength;
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
            BinaryMemoryReadStream stream,
            long finalPos,
            ICollection<RecordType> triggers,
            RecordConstants constants,
            bool skipHeader)
        {
            List<int> ret = new List<int>();
            var startingPos = stream.Position;
            while (!stream.Complete && stream.Position < finalPos)
            {
                var varMeta = constants.GetVariableMeta(stream);
                if (!triggers.Contains(varMeta.RecordType)) break;
                if (skipHeader)
                {
                    stream.Position += varMeta.HeaderLength;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.RecordLength;
                }
                else
                {
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)varMeta.TotalLength;
                }
            }
            return ret.ToArray();
        }

        public delegate T Factory<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package);

        public delegate T ConverterFactory<T>(
            BinaryMemoryReadStream stream,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter);

        public delegate T StreamTypedFactory<T>(
            BinaryMemoryReadStream stream,
            RecordType recordType,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter);

        public delegate T SpanFactory<T>(
            ReadOnlyMemorySlice<byte> span,
            BinaryWrapperFactoryPackage package);

        public delegate T SpanRecordFactory<T>(
            ReadOnlyMemorySlice<byte> span,
            BinaryWrapperFactoryPackage package,
            RecordTypeConverter recordTypeConverter);

        public IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            ICollectionGetter<RecordType> trigger,
            StreamTypedFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            var ret = new ReadOnlySetList<T>();
            while (!stream.Complete)
            {
                var subMeta = _package.Meta.GetSubRecord(stream);
                var recType = subMeta.RecordType;
                if (!trigger.Contains(recType)) break;
                var minimumFinalPos = stream.Position + subMeta.TotalLength;
                ret.Add(factory(stream, recType, _package, recordTypeConverter));
                if (stream.Position < minimumFinalPos)
                {
                    stream.Position = minimumFinalPos;
                }
            }
            return ret;
        }

        public IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            ICollectionGetter<RecordType> trigger,
            ConverterFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            return ParseRepeatedTypelessSubrecord(
                stream,
                trigger,
                (s, r, p, recConv) => factory(s, p, recConv),
                recordTypeConverter);
        }

        public IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            RecordType trigger,
            StreamTypedFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            var ret = new ReadOnlySetList<T>();
            while (!stream.Complete)
            {
                var subMeta = _package.Meta.GetSubRecord(stream);
                var recType = subMeta.RecordType;
                if (trigger != recType) break;
                var minimumFinalPos = stream.Position + subMeta.TotalLength;
                ret.Add(factory(stream, recType, _package, recordTypeConverter));
                if (stream.Position < minimumFinalPos)
                {
                    stream.Position = minimumFinalPos;
                }
            }
            return ret;
        }

        public IReadOnlySetList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            RecordType trigger,
            ConverterFactory<T> factory,
            RecordTypeConverter recordTypeConverter)
        {
            return ParseRepeatedTypelessSubrecord(
                stream,
                trigger,
                (s, r, p, recConv) => factory(s, p, recConv),
                recordTypeConverter);
        }

        public static ReadOnlyMemorySlice<byte> LockExtractMemory(IBinaryReadStream stream, long min, long max)
        {
            lock (stream)
            {
                stream.Position = min;
                var size = checked((int)(max - min));
                if (stream is BinaryMemoryReadStream memReadStream)
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
