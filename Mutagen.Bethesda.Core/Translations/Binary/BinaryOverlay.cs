using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public abstract class BinaryOverlay
    {
        public delegate TryGet<int?> RecordTypeFillWrapper(
            BinaryMemoryReadStream stream,
            int finalPos,
            int offset,
            RecordType type,
            int? lastParsed,
            RecordTypeConverter? recordTypeConverter);
        public delegate TryGet<int?> ModTypeFillWrapper(
            IBinaryReadStream stream,
            long finalPos,
            int offset,
            RecordType type,
            int? lastParsed,
            RecordTypeConverter? recordTypeConverter);

        protected ReadOnlyMemorySlice<byte> _data;
        protected BinaryOverlayFactoryPackage _package;

        protected BinaryOverlay(
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
            int? lastParsed = null;
            ModHeader headerMeta = package.Meta.GetMod(stream);
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
                GroupHeader groupMeta = package.Meta.GetGroup(stream);
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
            RecordTypeConverter? recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                MajorRecordHeader majorMeta = _package.Meta.MajorRecord(stream.RemainingSpan);
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
            RecordTypeConverter? recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                GroupHeader groupMeta = _package.Meta.Group(stream.RemainingSpan);
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
            RecordTypeConverter? recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                SubrecordHeader subMeta = _package.Meta.Subrecord(stream.RemainingSpan);
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
            RecordTypeConverter? recordTypeConverter,
            RecordTypeFillWrapper fill)
        {
            int? lastParsed = null;
            while (!stream.Complete && stream.Position < finalPos)
            {
                SubrecordHeader subMeta = _package.Meta.Subrecord(stream.RemainingSpan);
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
            RecordHeaderConstants constants,
            bool skipHeader,
            RecordTypeConverter? recordTypeConverter = null)
        {
            List<int> ret = new List<int>();
            var startingPos = stream.Position;
            while (!stream.Complete && stream.Position < finalPos)
            {
                var varMeta = constants.GetVariableMeta(stream);
                var recType = recordTypeConverter.ConvertToStandard(varMeta.RecordType);
                if (recType != trigger) break;
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
            RecordHeaderConstants constants,
            bool skipHeader,
            RecordTypeConverter? recordTypeConverter = null)
        {
            List<int> ret = new List<int>();
            var startingPos = stream.Position;
            while (!stream.Complete && stream.Position < finalPos)
            {
                var varMeta = constants.GetVariableMeta(stream);
                var recType = recordTypeConverter.ConvertToStandard(varMeta.RecordType);
                if (!triggers.Contains(recType)) break;
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
            ICollection<RecordType> includeTriggers,
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
                        stream.Position += (int)varMeta.RecordLength;
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
            BinaryMemoryReadStream stream,
            long finalPos,
            RecordType trigger,
            ICollection<RecordType> includeTriggers,
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
                        stream.Position += (int)varMeta.RecordLength;
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
            BinaryMemoryReadStream stream,
            long finalPos,
            ICollection<RecordType> triggers,
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
                        stream.Position += (int)varMeta.RecordLength;
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
            BinaryMemoryReadStream stream,
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
                        stream.Position += (int)varMeta.RecordLength;
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
            BinaryMemoryReadStream stream,
            BinaryOverlayFactoryPackage package);

        public delegate T ConverterFactory<T>(
            BinaryMemoryReadStream stream,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter);

        public delegate T StreamTypedFactory<T>(
            BinaryMemoryReadStream stream,
            RecordType recordType,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter);

        public delegate T SpanFactory<T>(
            ReadOnlyMemorySlice<byte> span,
            BinaryOverlayFactoryPackage package);

        public delegate T SpanRecordFactory<T>(
            ReadOnlyMemorySlice<byte> span,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter);

        public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            ICollectionGetter<RecordType> trigger,
            StreamTypedFactory<T> factory,
            RecordTypeConverter? recordTypeConverter)
        {
            var ret = new List<T>();
            while (!stream.Complete)
            {
                var subMeta = _package.Meta.GetSubrecord(stream);
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

        public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            ICollectionGetter<RecordType> trigger,
            ConverterFactory<T> factory,
            RecordTypeConverter? recordTypeConverter)
        {
            return ParseRepeatedTypelessSubrecord(
                stream,
                trigger,
                (s, r, p, recConv) => factory(s, p, recConv),
                recordTypeConverter);
        }

        public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            RecordType trigger,
            StreamTypedFactory<T> factory,
            RecordTypeConverter? recordTypeConverter)
        {
            var ret = new List<T>();
            while (!stream.Complete)
            {
                var subMeta = _package.Meta.GetSubrecord(stream);
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

        public IReadOnlyList<T> ParseRepeatedTypelessSubrecord<T>(
            BinaryMemoryReadStream stream,
            RecordType trigger,
            ConverterFactory<T> factory,
            RecordTypeConverter? recordTypeConverter)
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
