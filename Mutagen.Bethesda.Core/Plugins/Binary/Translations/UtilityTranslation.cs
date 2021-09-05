using Ionic.Zlib;
using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public static class PluginUtilityTranslation
    {
        public delegate bool BinaryMasterParseDelegate<TItem>(
            MutagenFrame reader,
            [MaybeNullWhen(false)] out TItem item,
            TypedParseParams? translationParams);

        public delegate bool BinarySubParseRecordDelegate<TItem>(
            MutagenFrame reader,
            RecordType header,
            [MaybeNullWhen(false)] out TItem item);

        public delegate bool BinaryMasterParseRecordDelegate<TItem>(
            MutagenFrame reader,
            RecordType header,
            [MaybeNullWhen(false)] out TItem item,
            TypedParseParams? translationParams);

        public delegate void BinaryMasterWriteDelegate<TItem>(
            MutagenWriter writer,
            TItem item,
            TypedWriteParams? translationParams);

        public delegate void RecordStructFill<R>(
            R record,
            MutagenFrame frame);

        public delegate ParseResult RecordTypeFill<R>(
            R record,
            MutagenFrame frame,
            Dictionary<RecordType, int>? recordParseCount,
            RecordType nextRecordType,
            int contentLength,
            TypedParseParams? translationParams);

        public delegate ParseResult MajorRecordFill<R>(
            R record,
            MutagenFrame frame,
            PreviousParse lastParsed,
            Dictionary<RecordType, int>? recordParseCount,
            RecordType nextRecordType,
            int contentLength,
            TypedParseParams? translationParams);

        public delegate ParseResult SubrecordFill<R>(
            R record,
            MutagenFrame frame,
            PreviousParse previousSubrecordParse,
            Dictionary<RecordType, int>? recordParseCount,
            RecordType nextRecordType,
            int contentLength,
            TypedParseParams? translationParams);

        public delegate ParseResult ModRecordTypeFill<TRecord, TImportMask>(
            TRecord record,
            MutagenFrame frame,
            RecordType nextRecordType,
            int contentLength,
            TImportMask importMask,
            TypedParseParams? translationParams);

        public static M MajorRecordParse<M>(
            M record,
            MutagenFrame frame,
            TypedParseParams? translationParams,
            RecordStructFill<M> fillStructs,
            MajorRecordFill<M> fillTyped)
            where M : IMajorRecordCommonGetter
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseRecord(frame.Reader));
            fillStructs(
                record: record,
                frame: frame);
            try
            {
                MutagenFrame targetFrame = frame;
                if (record.IsCompressed)
                {
                    targetFrame = frame.Decompress();
                }

                Dictionary<RecordType, int>? recordParseCount = null;
                frame.MetaData.FormVersion = record.FormVersion;
                var lastParsed = new PreviousParse();
                while (!targetFrame.Complete)
                {
                    var subMeta = targetFrame.GetSubrecord();
                    var finalPos = targetFrame.Position + subMeta.TotalLength;
                    ParseResult parsed;
                    try
                    {
                        parsed = fillTyped(
                            record: record,
                            frame: targetFrame,
                            lastParsed: lastParsed,
                            recordParseCount: recordParseCount,
                            nextRecordType: subMeta.RecordType,
                            contentLength: lastParsed.LengthOverride ?? subMeta.ContentLength,
                            translationParams: translationParams);
                    }
                    catch (Exception ex)
                    {
                        throw new SubrecordException(
                            subMeta.RecordType,
                            record.FormKey,
                            majorRecordType: record.Registration.ClassType,
                            modKey: frame.Reader.MetaData.ModKey,
                            edid: record.EditorID,
                            innerException: ex);
                    }

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

                    if (targetFrame.Position < finalPos)
                    {
                        targetFrame.Position = finalPos;
                    }

                    lastParsed = parsed;
                }

                frame.SetToFinalPosition();
                frame.MetaData.FormVersion = null;
                return record;
            }
            catch (Exception ex)
            {
                throw RecordException.Enrich(ex, record);
            }
        }

        public static M RecordParse<M>(
            M record,
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter,
            RecordStructFill<M> fillStructs)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
            return record;
        }

        public static M RecordParse<M>(
            M record,
            MutagenFrame frame,
            TypedParseParams? translationParams,
            RecordStructFill<M> fillStructs,
            MajorRecordFill<M> fillTyped)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
            Dictionary<RecordType, int>? recordParseCount = null;
            var previousParse = new PreviousParse();
            while (!frame.Complete)
            {
                var subMeta = frame.GetSubrecord();
                var finalPos = frame.Position + subMeta.TotalLength;
                var parsed = fillTyped(
                    record: record,
                    frame: frame,
                    lastParsed: previousParse,
                    recordParseCount: recordParseCount,
                    nextRecordType: subMeta.RecordType,
                    contentLength: subMeta.ContentLength,
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

                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }

                previousParse = parsed;
            }

            return record;
        }

        public static M SubrecordParse<M>(
            M record,
            MutagenFrame frame,
            TypedParseParams? translationParams,
            RecordStructFill<M> fillStructs)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
            return record;
        }

        public static M SubrecordParse<M>(
            M record,
            MutagenFrame frame,
            TypedParseParams? translationParams,
            RecordStructFill<M> fillStructs,
            SubrecordFill<M> fillTyped)
        {
            fillStructs?.Invoke(
                record: record,
                frame: frame);
            var lastParsed = new PreviousParse();
            Dictionary<RecordType, int>? recordParseCount = null;
            while (!frame.Complete)
            {
                var subMeta = frame.GetSubrecord();
                var finalPos = frame.Position + subMeta.TotalLength;
                var parsed = fillTyped(
                    record: record,
                    frame: frame,
                    previousSubrecordParse: lastParsed,
                    recordParseCount: recordParseCount,
                    nextRecordType: subMeta.RecordType,
                    contentLength: subMeta.ContentLength,
                    translationParams: translationParams);
                if (!parsed.KeepParsing) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }

                if (parsed.DuplicateParseMarker != null)
                {
                    if (recordParseCount == null)
                    {
                        recordParseCount = new Dictionary<RecordType, int>();
                    }

                    recordParseCount[parsed.DuplicateParseMarker!.Value] =
                        recordParseCount.GetOrAdd(parsed.DuplicateParseMarker!.Value) + 1;
                }

                lastParsed = parsed;
            }

            return record;
        }

        public static G GroupParse<G>(
            G record,
            MutagenFrame frame,
            TypedParseParams? translationParams,
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
                    recordParseCount: null,
                    nextRecordType: nextRecordType,
                    contentLength: contentLength,
                    translationParams: translationParams);
                if (!parsed.KeepParsing) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }
            }

            frame.SetToFinalPosition();
            return record;
        }

        public static TMod ModParse<TMod, TImportMask>(
            TMod record,
            MutagenFrame frame,
            TImportMask importMask,
            RecordStructFill<TMod> fillStructs,
            ModRecordTypeFill<TMod, TImportMask> fillTyped)
            where TMod : IMod
        {
            var modHeader = frame.Reader.GetModHeader();
            fillTyped(
                record: record,
                frame: frame,
                importMask: importMask,
                nextRecordType: modHeader.RecordType,
                contentLength: checked((int)modHeader.ContentLength),
                translationParams: null);
            frame.Reader.MetaData.MasterReferences.SetTo(record.MasterReferences);
            while (!frame.Complete)
            {
                var groupHeader = frame.GetGroup();
                if (!groupHeader.IsGroup)
                {
                    throw new ArgumentException("Did not see GRUP header as expected.");
                }

                var len = checked((int)groupHeader.ContentLength);
                var finalPos = frame.Position + groupHeader.TotalLength;
                if (len == 0)
                {
                    frame.Position = finalPos;
                    continue;
                }

                var parsed = fillTyped(
                    record: record,
                    frame: frame,
                    importMask: importMask,
                    nextRecordType: groupHeader.ContainedRecordType,
                    contentLength: len,
                    translationParams: null);
                if (!parsed.KeepParsing) break;
                if (frame.Position < finalPos)
                {
                    frame.Position = finalPos;
                }
            }

            frame.SetToFinalPosition();
            return record;
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
                BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength),
                    uncompressedLength);
                // Remove compression flag
                BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan().Slice(meta.MajorConstants.FlagLocationOffset),
                    majorMeta.MajorRecordFlags & ~Constants.CompressedFlag);
                // Copy uncompressed data over
                using (var stream = new ZlibStream(new ByteMemorySliceStream(slice.Slice(majorMeta.HeaderLength + 4)),
                    CompressionMode.Decompress))
                {
                    stream.Read(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
                }

                slice = new MemorySlice<byte>(buf);
            }

            return slice;
        }

        public static OverlayStream DecompressStream(OverlayStream stream)
        {
            var majorMeta = stream.GetMajorRecord();
            if (majorMeta.IsCompressed)
            {
                uint uncompressedLength =
                    BinaryPrimitives.ReadUInt32LittleEndian(stream.RemainingSpan.Slice(majorMeta.HeaderLength));
                byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
                // Copy major meta bytes over
                stream.RemainingSpan.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
                // Set length bytes
                BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength),
                    uncompressedLength);
                // Remove compression flag
                BinaryPrimitives.WriteInt32LittleEndian(
                    buf.AsSpan().Slice(stream.MetaData.Constants.MajorConstants.FlagLocationOffset),
                    majorMeta.MajorRecordFlags & ~Constants.CompressedFlag);
                // Copy uncompressed data over
                using (var compessionStream =
                    new ZlibStream(new ByteMemorySliceStream(stream.RemainingMemory.Slice(majorMeta.HeaderLength + 4)),
                        CompressionMode.Decompress))
                {
                    compessionStream.Read(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
                }

                stream.Position += checked((int)majorMeta.TotalLength);
                stream = new OverlayStream(buf, stream.MetaData);
            }

            return stream;
        }

        /// <summary>
        /// Parses span data and enumerates pairs of record type -> locations
        /// 
        /// It is assumed the span contains only subrecords
        /// </summary>
        /// <param name="span">Bytes containing subrecords</param>
        /// <param name="meta">Metadata to use in subrecord parsing</param>
        /// <returns>Enumerable of KeyValue pairs of encountered RecordTypes and their locations relative to the input span</returns>
        public static IEnumerable<KeyValuePair<RecordType, int>> EnumerateSubrecords(ReadOnlyMemorySlice<byte> span,
            GameConstants meta)
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
        /// <param name="lenParsed">The amount of data located subrecords cover</param>
        /// <returns>Array of locations of located target types</returns>
        public static int[] ParseRepeatingSubrecord(ReadOnlyMemorySlice<byte> span, GameConstants meta,
            RecordType recordType, out int lenParsed)
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
        public static int?[] FindFirstSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta,
            params RecordType[] recordTypes)
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
        /// <param name="lenParsed">Amount of data contained in located records</param>
        /// <returns>Array of found record locations</returns>
        public static int?[] FindNextSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta, out int lenParsed,
            params RecordType[] recordTypes)
        {
            return FindNextSubrecords(
                data: data,
                meta: meta,
                lenParsed: out lenParsed,
                stopOnAlreadyEncounteredRecord: false,
                recordTypes: recordTypes);
        }

        /// <summary>
        /// Locates the first encountered instances of all given subrecord types, and returns an array of their locations
        /// -1 represents a recordtype that was not found.
        /// 
        /// If a subrecord is encountered that is not of the target types, it will stop looking for more matches.
        /// If a subrecord is encountered that was already seen, it will stop looking for more matches.
        /// 
        /// </summary>
        /// <param name="data">Subrecord data to be parsed</param>
        /// <param name="recordTypes">Record types to locate</param>
        /// <param name="meta">Metadata to use in subrecord parsing</param>
        /// <param name="lenParsed">Amount of data contained in located records</param>
        /// <param name="stopOnAlreadyEncounteredRecord">Whether to stop looking if encountering a record type that has already been seen</param>
        /// <returns>Array of found record locations</returns>
        public static int?[] FindNextSubrecords(
            ReadOnlyMemorySlice<byte> data,
            GameConstants meta,
            out int lenParsed,
            bool stopOnAlreadyEncounteredRecord,
            params RecordType[] recordTypes)
        {
            lenParsed = 0;
            int?[] ret = new int?[recordTypes.Length];
            while (data.Length > lenParsed)
            {
                var subMeta = meta.Subrecord(data.Slice(lenParsed));
                var recType = subMeta.RecordType;
                bool breakOut = true;
                for (int i = 0; i < recordTypes.Length; i++)
                {
                    if (recordTypes[i] == recType)
                    {
                        breakOut = false;
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
                        else if (stopOnAlreadyEncounteredRecord)
                        {
                            breakOut = true;
                        }

                        break;
                    }
                }

                if (breakOut)
                {
                    return ret;
                }

                lenParsed += subMeta.TotalLength;
            }

            return ret;
        }

        public static int? FindFirstSubrecord(
            ReadOnlyMemorySlice<byte> data,
            GameConstants meta,
            RecordType recordType,
            bool navigateToContent = false,
            int? offset = null)
        {
            int loc = offset ?? 0;
            while (data.Length > loc)
            {
                var subMeta = meta.Subrecord(data.Slice(loc));
                if (subMeta.RecordType == recordType)
                {
                    if (navigateToContent)
                    {
                        loc += meta.SubConstants.HeaderLength;
                    }

                    return loc;
                }

                loc += subMeta.TotalLength;
            }

            return null;
        }

        public static int? FindFirstSubrecord(
            ReadOnlyMemorySlice<byte> data,
            GameConstants meta,
            ICollectionGetter<RecordType> recordTypes,
            bool navigateToContent = false,
            int? offset = null)
        {
            int loc = offset ?? 0;
            while (data.Length > loc)
            {
                var subMeta = meta.Subrecord(data.Slice(loc));
                if (recordTypes.Contains(subMeta.RecordType))
                {
                    if (navigateToContent)
                    {
                        loc += meta.SubConstants.HeaderLength;
                    }

                    return loc;
                }

                loc += subMeta.TotalLength;
            }

            return null;
        }

        public static int[] FindAllOfSubrecord(
            ReadOnlyMemorySlice<byte> data,
            GameConstants meta,
            RecordType recordType,
            bool navigateToContent = false)
        {
            List<int> ret = new List<int>();
            int lenParsed = 0;
            while (data.Length > lenParsed)
            {
                var subMeta = meta.Subrecord(data.Slice(lenParsed));
                if (subMeta.RecordType == recordType)
                {
                    if (navigateToContent)
                    {
                        ret.Add(subMeta.HeaderLength + lenParsed);
                    }
                    else
                    {
                        ret.Add(lenParsed);
                    }
                }

                lenParsed += subMeta.TotalLength;
            }

            return ret.ToArray();
        }

        public static int[] FindAllOfSubrecords(
            ReadOnlyMemorySlice<byte> data,
            GameConstants meta,
            ICollectionGetter<RecordType> recordTypes,
            bool navigateToContent = false)
        {
            List<int> ret = new List<int>();
            int lenParsed = 0;
            while (data.Length > lenParsed)
            {
                var subMeta = meta.Subrecord(data.Slice(lenParsed));
                if (recordTypes.Contains(subMeta.RecordType))
                {
                    if (navigateToContent)
                    {
                        ret.Add(subMeta.HeaderLength + lenParsed);
                    }
                    else
                    {
                        ret.Add(lenParsed);
                    }
                }

                lenParsed += subMeta.TotalLength;
            }

            return ret.ToArray();
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
            PluginUtilityTranslation.SetGroupLength(bytes, (uint)ret.Sum(i => i.Length));
            return ret;
        }

        public static void CompileSetGroupLength(
            IEnumerable<Stream> streams,
            byte[] bytes)
        {
            PluginUtilityTranslation.SetGroupLength(bytes, (uint)streams.NotNull().Sum(i => i.Length));
        }

        public static void SkipPastAll(IBinaryReadStream stream, GameConstants meta, RecordType recordType)
        {
            while (stream.TryReadSubrecordFrame(meta, recordType, out var _))
            {
            }
        }

        public static int SkipPastAll(ReadOnlyMemorySlice<byte> data, GameConstants constants, RecordType toSkip,
            out int numRecordsPassed)
        {
            var pos = 0;
            numRecordsPassed = 0;
            while (pos < data.Length)
            {
                var subHeader = constants.Subrecord(data.Slice(pos));
                if (subHeader.RecordType != toSkip) break;
                pos += subHeader.TotalLength;
                numRecordsPassed++;
            }

            return pos;
        }

        public static RecordType GetRecordType<T>()
        {
            return (RecordType)LoquiRegistration.GetRegister(typeof(T))!.GetType()
                .GetField(Constants.TriggeringRecordTypeMember)!.GetValue(null)!;
        }

        public static ReadOnlyMemorySlice<byte>? ReadByteArrayWithOverflow(
            ReadOnlyMemorySlice<byte> bytes,
            GameConstants constants,
            int? loc,
            int? lengthOverride)
        {
            if (!loc.HasValue) return null;
            var header = constants.SubrecordFrame(bytes[loc.Value..]);
            if (lengthOverride != null)
            {
                return bytes.Slice(
                    loc.Value + header.HeaderLength,
                    lengthOverride.Value);
            }
            else
            {
                return header.Content;
            }
        }

        public static int HandleOverlayRecordOverflow(
            int? existingLoc,
            OverlayStream stream,
            int offset,
            ReadOnlyMemorySlice<byte> data,
            GameConstants constants)
        {
            if (existingLoc.HasValue)
            {
                var overflowHeader = constants.SubrecordFrame(data.Slice(existingLoc.Value));
                var len = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(overflowHeader.Content));
                // Need to skip the data record, which doesn't have a proper length
                stream.Position += constants.SubConstants.HeaderLength + len;
                return existingLoc.Value;
            }
            else
            {
                return stream.Position - offset;
            }
        }
    }
}