using Ionic.Zlib;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class UtilityTranslation
    {
        private static readonly byte[] _Zeros = new byte[8];
        public static ReadOnlyMemorySlice<byte> Zeros => new ReadOnlyMemorySlice<byte>(_Zeros);

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

        public static M MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            ErrorMaskBuilder errorMask,
            RecordType recType,
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            RecordStructFill<M> fillStructs,
            RecordTypeFill<M> fillTyped)
            where M : IMajorRecordCommonGetter
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
            if (record.IsCompressed)
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

        public static ReadOnlyMemorySlice<byte> DecompressSpan(ReadOnlyMemorySlice<byte> slice, MetaDataConstants meta)
        {
            var majorMeta = meta.MajorRecord(slice);
            if (majorMeta.IsCompressed)
            {
                uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(slice.Slice(majorMeta.HeaderLength));
                byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
                // Copy major meta bytes over
                slice.Span.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
                // Set length bytes
                BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HEADER_LENGTH), uncompressedLength);
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

        public static BinaryMemoryReadStream DecompressStream(BinaryMemoryReadStream stream, MetaDataConstants meta)
        {
            var majorMeta = meta.GetMajorRecord(stream);
            if (majorMeta.IsCompressed)
            {
                uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(stream.RemainingSpan.Slice(majorMeta.HeaderLength));
                byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
                // Copy major meta bytes over
                stream.RemainingSpan.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
                // Set length bytes
                BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HEADER_LENGTH), uncompressedLength);
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

        public static void FillEdidLinkCache<T>(IModGetter mod, RecordType recordType, BinaryWrapperFactoryPackage package)
            where T : IMajorRecordCommonGetter
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
        public static int[] FindFirstSubrecords(ReadOnlySpan<byte> data, MetaDataConstants meta, params RecordType[] recordTypes)
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

        public static int FindFirstSubrecord(ReadOnlySpan<byte> data, MetaDataConstants meta, RecordType recordType, bool navigateToContent = false)
        {
            int loc = 0;
            while (data.Length > loc)
            {
                var subMeta = meta.SubRecord(data.Slice(loc));
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
            where M : IMajorRecordCommonGetter
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
            if (record.IsCompressed)
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
                }).ConfigureAwait(false);
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
