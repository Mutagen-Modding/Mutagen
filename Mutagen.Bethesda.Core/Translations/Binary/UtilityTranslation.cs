using Ionic.Zlib;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class UtilityTranslation
    {
        private static readonly byte[] _Zeros = new byte[8];
        public static ReadOnlyMemorySlice<byte> Zeros => new ReadOnlyMemorySlice<byte>(_Zeros);

        public delegate bool BinarySubParseDelegate<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out T item);
        public delegate bool BinaryMasterParseDelegate<T>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out T item,
            RecordTypeConverter? recordTypeConverter);
        public delegate bool BinarySubParseRecordDelegate<T>(
            MutagenFrame reader,
            RecordType header,
            [MaybeNullWhen(false)] out T item);
        public delegate bool BinaryMasterParseRecordDelegate<T>(
            MutagenFrame reader,
            RecordType header,
            [MaybeNullWhen(false)] out T item,
            RecordTypeConverter? recordTypeConverter);
        public delegate void BinarySubWriteDelegate<T>(
            MutagenWriter writer,
            T item);
        public delegate void BinaryMasterWriteDelegate<T>(
            MutagenWriter writer,
            T item,
            RecordTypeConverter? recordTypeConverter);

        public delegate void RecordStructFill<R>(
            R record,
            MutagenFrame frame);

        public delegate TryGet<int?> RecordTypeFill<R>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            RecordTypeConverter? recordTypeConverter);

        public delegate TryGet<int?> RecordTypelessStructFill<R>(
            R record,
            MutagenFrame frame,
            int? lastParsed,
            RecordType nextRecordType,
            int contentLength,
            RecordTypeConverter? recordTypeConverter);

        public delegate TryGet<int?> ModRecordTypeFill<R, G>(
            R record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            G importMask,
            RecordTypeConverter? recordTypeConverter);

        public static M MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            RecordType recType,
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
            where M : IMajorRecordCommonGetter
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(
                frame.Reader,
                recType));
            fillStructs(
                record: record,
                frame: frame);
            if (fillTyped == null) return record;
            MutagenFrame targetFrame = frame;
            if (record.IsCompressed)
            {
                targetFrame = frame.Decompress();
            }
            while (!targetFrame.Complete)
            {
                var subMeta = targetFrame.GetSubrecord();
                var finalPos = targetFrame.Position + subMeta.TotalLength;
                var parsed = fillTyped(
                    record: record,
                    frame: targetFrame,
                    nextRecordType: subMeta.RecordType,
                    contentLength: subMeta.ContentLength,
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
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
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
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
            while (!frame.Complete)
            {
                var subMeta = frame.GetSubrecord();
                var finalPos = frame.Position + subMeta.TotalLength;
                var parsed = fillTyped(
                    record: record,
                    frame: frame,
                    nextRecordType: subMeta.RecordType,
                    contentLength: subMeta.ContentLength,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }
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
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
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
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs,
            RecordTypelessStructFill<M> fillTyped)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
            int? lastParsed = null;
            while (!frame.Complete)
            {
                var subMeta = frame.GetSubrecord();
                var finalPos = frame.Position + subMeta.TotalLength;
                var parsed = fillTyped(
                    record: record,
                    frame: frame,
                    lastParsed: lastParsed,
                    nextRecordType: subMeta.RecordType,
                    contentLength: subMeta.ContentLength,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }
                lastParsed = parsed.Value;
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
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<G> fillStructs,
            RecordTypeFill<G> fillTyped)
        {
            var groupMeta = frame.GetGroup();
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException($"Expected GRUP header was not read in: {frame.Position}");
            }
            frame.Position += groupMeta.TypeAndLengthLength;
            frame = frame.ReadAndReframe(checked((int)(groupMeta.TotalLength - groupMeta.TypeAndLengthLength)));

            fillStructs?.Invoke(
                record: record,
                frame: frame);
            while (!frame.Complete)
            {
                var nextRecordType = HeaderTranslation.GetNextSubrecordType(
                    reader: frame.Reader,
                    contentLength: out var contentLength);
                var finalPos = frame.Position + contentLength;
                var parsed = fillTyped(
                    record: record,
                    frame: frame,
                    nextRecordType: nextRecordType,
                    contentLength: contentLength,
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static M ModParse<M, G>(
            M record,
            MutagenFrame frame,
            G importMask,
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs,
            ModRecordTypeFill<M, G> fillTyped)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
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
                    recordTypeConverter: recordTypeConverter);
                if (parsed.Failed) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }
            }
            frame.SetToFinalPosition();
            return record;
        }

        public static MasterReferenceReader ConstructWriteMasters(IModGetter mod, BinaryWriteParameters param)
        {
            MasterReferenceReader ret = new MasterReferenceReader(mod.ModKey);
            HashSet<ModKey> modKeys = new HashSet<ModKey>();
            switch (param.MastersListSync)
            {
                case BinaryWriteParameters.MastersListSyncOption.NoCheck:
                    modKeys.Add(mod.MasterReferences.Select(m => m.Master));
                    break;
                case BinaryWriteParameters.MastersListSyncOption.Iterate:
                    modKeys.Add(
                        mod.Links.SelectWhere(l =>
                        {
                            if (l.TryGetModKey(out var modKey))
                            {
                                return TryGet<ModKey>.Succeed(modKey);
                            }
                            return TryGet<ModKey>.Failure;
                        })
                        .And(mod.EnumerateMajorRecords().Select(m => m.FormKey.ModKey)));
                    break;
                default:
                    throw new NotImplementedException();
            }
            modKeys.Remove(mod.ModKey);
            modKeys.Remove(ModKey.Null);
            ret.SetTo(modKeys.Select(m => new MasterReference()
            {
                Master = m
            }));
            return ret;
        }

        public static ReadOnlyMemorySlice<byte> DecompressSpan(ReadOnlyMemorySlice<byte> slice, GameConstants meta)
        {
            var majorMeta = meta.MajorRecord(slice);
            if (majorMeta.IsCompressed)
            {
                uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(slice.Slice(majorMeta.HeaderLength));
                byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
                // Copy major meta bytes over
                slice.Span.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
                // Set length bytes
                BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength), uncompressedLength);
                // Remove compression flag
                BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan().Slice(meta.MajorConstants.FlagLocationOffset), majorMeta.MajorRecordFlags & ~Constants.CompressedFlag);
                // Copy uncompressed data over
                using (var stream = new ZlibStream(new ByteMemorySliceStream(slice.Slice(majorMeta.HeaderLength + 4)), CompressionMode.Decompress))
                {
                    stream.Read(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
                }
                slice = new MemorySlice<byte>(buf);
            }
            return slice;
        }

        public static BinaryMemoryReadStream DecompressStream(BinaryMemoryReadStream stream, GameConstants meta)
        {
            var majorMeta = meta.GetMajorRecord(stream);
            if (majorMeta.IsCompressed)
            {
                uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(stream.RemainingSpan.Slice(majorMeta.HeaderLength));
                byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
                // Copy major meta bytes over
                stream.RemainingSpan.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
                // Set length bytes
                BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength), uncompressedLength);
                // Remove compression flag
                BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan().Slice(meta.MajorConstants.FlagLocationOffset), majorMeta.MajorRecordFlags & ~Constants.CompressedFlag);
                // Copy uncompressed data over
                using (var compessionStream = new ZlibStream(new ByteMemorySliceStream(stream.RemainingMemory.Slice(majorMeta.HeaderLength + 4)), CompressionMode.Decompress))
                {
                    compessionStream.Read(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
                }
                stream.Position += checked((int)majorMeta.TotalLength);
                stream = new BinaryMemoryReadStream(buf);
            }
            return stream;
        }

        public static void FillEdidLinkCache<T>(IModGetter mod, RecordType recordType, BinaryOverlayFactoryPackage package)
            where T : IMajorRecordCommonGetter
        {
            var group = mod.GetGroupGetter<T>();
            var cache = new Dictionary<RecordType, object>();
            package.EdidLinkCache[recordType] = cache;
            foreach (var item in group)
            {
                var edid = item.Value.EditorID;
                if (edid == null) continue;
                if (edid.Length != Constants.HeaderLength)
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
        public static IEnumerable<KeyValuePair<RecordType, int>> EnumerateSubrecords(ReadOnlyMemorySlice<byte> span, GameConstants meta)
        {
            int loc = 0;
            while (span.Length > loc)
            {
                var subMeta = meta.Subrecord(span.Slice(loc));
                var len = subMeta.TotalLength;
                yield return new KeyValuePair<RecordType, int>(subMeta.RecordType, loc);
                loc += len;
            }
        }

        /// <summary>
        /// Parses span data and locates all uninterrupted repeating instances of target record type
        /// 
        /// It is assumed the span contains only subrecords
        /// </summary>
        /// <param name="span">Bytes containing subrecords</param>
        /// <param name="meta">Metadata to use in subrecord parsing</param>
        /// <param name="recordType">Repeating type to locate</param>
        /// <returns>Array of locations of located target types</returns>
        public static int[] FindRepeatingSubrecord(ReadOnlySpan<byte> span, GameConstants meta, RecordType recordType, out int lenParsed)
        {
            lenParsed = 0;
            List<int> list = new List<int>();
            while (span.Length > lenParsed)
            {
                var subMeta = meta.Subrecord(span.Slice(lenParsed));
                if (subMeta.RecordType != recordType) break;
                list.Add(lenParsed);
                lenParsed += subMeta.TotalLength;
            }
            return list.ToArray();
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
        public static int?[] FindFirstSubrecords(ReadOnlySpan<byte> data, GameConstants meta, params RecordType[] recordTypes)
        {
            int loc = 0;
            int?[] ret = new int?[recordTypes.Length];
            while (data.Length > loc)
            {
                var subMeta = meta.Subrecord(data.Slice(loc));
                var recType = subMeta.RecordType;
                for (int i = 0; i < recordTypes.Length; i++)
                {
                    if (recordTypes[i] == recType && ret[i] == null)
                    {
                        ret[i] = loc;
                        bool breakOut = false;

                        // Check to see if there's still more to find
                        for (int j = 0; j < ret.Length; j++)
                        {
                            if (ret[j] == null)
                            {
                                breakOut = true;
                                break;
                            }
                        }
                        if (breakOut)
                        {
                            break;
                        }

                        // Found everything
                        return ret;
                    }
                }
                loc += subMeta.TotalLength;
            }
            return ret;
        }

        /// <summary>
        /// Locates the first encountered instances of all given subrecord types, and returns an array of their locations
        /// -1 represents a recordtype that was not found.
        /// 
        /// If a subrecord is encountered that is not of the target types, it will stop looking for more matches
        /// 
        /// </summary>
        /// <param name="data">Subrecord data to be parsed</param>
        /// <param name="recordTypes">Record types to locate</param>
        /// <param name="meta">Metadata to use in subrecord parsing</param>
        /// <returns>Array of found record locations</returns>
        public static int?[] FindNextSubrecords(ReadOnlySpan<byte> data, GameConstants meta, out int lenParsed, params RecordType[] recordTypes)
        {
            lenParsed = 0;
            int?[] ret = new int?[recordTypes.Length];
            while (data.Length > lenParsed)
            {
                var subMeta = meta.Subrecord(data.Slice(lenParsed));
                var recType = subMeta.RecordType;
                bool matchedSomething = false;
                for (int i = 0; i < recordTypes.Length; i++)
                {
                    if (recordTypes[i] == recType)
                    {
                        matchedSomething = true;
                        if (ret[i] == null)
                        {
                            ret[i] = lenParsed;
                            bool moreToFind = false;
                            for (int j = 0; j < ret.Length; j++)
                            {
                                if (ret[j] == null)
                                {
                                    moreToFind = true;
                                    break;
                                }
                            }
                            if (!moreToFind)
                            {
                                lenParsed += subMeta.TotalLength;
                                // Found everything
                                return ret;
                            }
                        }
                        break;
                    }
                }
                if (!matchedSomething)
                {
                    return ret;
                }
                lenParsed += subMeta.TotalLength;
            }
            return ret;
        }

        public static int FindFirstSubrecord(ReadOnlySpan<byte> data, GameConstants meta, RecordType recordType, bool navigateToContent = false)
        {
            int loc = 0;
            while (data.Length > loc)
            {
                var subMeta = meta.Subrecord(data.Slice(loc));
                if (subMeta.RecordType == recordType) return navigateToContent ? (loc + meta.SubConstants.HeaderLength) : loc;
                loc += subMeta.TotalLength;
            }
            return -1;
        }

        public static async Task CompileStreamsInto(IEnumerable<Task<IEnumerable<Stream>>> inStreams, Stream outStream)
        {
            var streams = await Task.WhenAll(inStreams).ConfigureAwait(false);
            foreach (var s in streams.SelectMany(s => s))
            {
                s.Position = 0;
                s.CopyTo(outStream);
            }
        }

        public static async Task CompileStreamsInto(Task<IEnumerable<Stream>> inStreams, Stream outStream)
        {
            var streams = await inStreams.ConfigureAwait(false);
            foreach (var s in streams)
            {
                s.Position = 0;
                s.CopyTo(outStream);
            }
        }

        public static async Task CompileStreamsInto(IEnumerable<Task<Stream>> inStreams, Stream outStream)
        {
            foreach (var sTask in inStreams)
            {
                var s = await sTask.ConfigureAwait(false);
                s.Position = 0;
                s.CopyTo(outStream);
            }
        }

        public static void CompileStreamsInto(IEnumerable<Stream> inStreams, Stream outStream)
        {
            foreach (var s in inStreams)
            {
                s.Position = 0;
                s.CopyTo(outStream);
            }
        }

        public static void SetGroupLength(
            byte[] bytes,
            uint len)
        {
            var bytesSpan = bytes.AsSpan();
            BinaryPrimitives.WriteUInt32LittleEndian(bytesSpan.Slice(4), len);
        }

        public static async Task<IEnumerable<Stream>> CompileSetGroupLength(
            IEnumerable<Task<Stream>> streams,
            byte[] bytes)
        {
            var ret = await Task.WhenAll(streams).ConfigureAwait(false);
            UtilityTranslation.SetGroupLength(bytes, (uint)ret.Sum(i => i.Length));
            return ret;
        }

        public static void CompileSetGroupLength(
            IEnumerable<Stream> streams,
            byte[] bytes)
        {
            UtilityTranslation.SetGroupLength(bytes, (uint)streams.NotNull().Sum(i => i.Length));
        }

        public static void SkipPastAll(IBinaryReadStream stream, GameConstants meta, RecordType recordType)
        {
            while (meta.TryReadSubrecordFrame(stream, recordType, out var _))
            {
            }
        }
    }
}
