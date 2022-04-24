using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal static class PluginUtilityTranslation
{
    internal delegate bool BinaryMasterParseDelegate<TItem>(
        MutagenFrame reader,
        [MaybeNullWhen(false)] out TItem item,
        TypedParseParams? translationParams);

    internal delegate bool BinarySubParseRecordDelegate<TItem>(
        MutagenFrame reader,
        RecordType header,
        [MaybeNullWhen(false)] out TItem item);

    internal delegate bool BinaryMasterParseRecordDelegate<TItem>(
        MutagenFrame reader,
        RecordType header,
        [MaybeNullWhen(false)] out TItem item,
        TypedParseParams? translationParams);

    internal delegate void BinaryMasterWriteDelegate<TItem>(
        MutagenWriter writer,
        TItem item,
        TypedWriteParams? translationParams);

    internal delegate void RecordStructFill<R>(
        R record,
        MutagenFrame frame);

    internal delegate ParseResult RecordTypeFill<R>(
        R record,
        MutagenFrame frame,
        Dictionary<RecordType, int>? recordParseCount,
        RecordType nextRecordType,
        int contentLength,
        TypedParseParams? translationParams);

    internal delegate ParseResult MajorRecordFill<R>(
        R record,
        MutagenFrame frame,
        PreviousParse lastParsed,
        Dictionary<RecordType, int>? recordParseCount,
        RecordType nextRecordType,
        int contentLength,
        TypedParseParams? translationParams);

    internal delegate ParseResult SubrecordFill<R>(
        R record,
        MutagenFrame frame,
        PreviousParse previousSubrecordParse,
        Dictionary<RecordType, int>? recordParseCount,
        RecordType nextRecordType,
        int contentLength,
        TypedParseParams? translationParams);

    internal delegate ParseResult ModRecordTypeFill<TRecord, TImportMask>(
        TRecord record,
        MutagenFrame frame,
        RecordType nextRecordType,
        int contentLength,
        TImportMask importMask,
        TypedParseParams? translationParams);

    internal static M MajorRecordParse<M>(
        M record,
        MutagenFrame frame,
        TypedParseParams? translationParams,
        RecordStructFill<M> fillStructs,
        MajorRecordFill<M> fillTyped)
        where M : IMajorRecordGetter
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
                var subMeta = targetFrame.GetSubrecordHeader();
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

    internal static M RecordParse<M>(
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

    internal static M RecordParse<M>(
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
            var subMeta = frame.GetSubrecordHeader();
            var finalPos = frame.Position + subMeta.TotalLength;
            var parsed = fillTyped(
                record: record,
                frame: frame,
                lastParsed: previousParse,
                recordParseCount: recordParseCount,
                nextRecordType: subMeta.RecordType,
                contentLength: previousParse.LengthOverride ?? subMeta.ContentLength,
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

    internal static M SubrecordParse<M>(
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

    internal static M SubrecordParse<M>(
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
            var subMeta = frame.GetSubrecordHeader();
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

    internal static G GroupParse<G>(
        G record,
        MutagenFrame frame,
        TypedParseParams? translationParams,
        RecordStructFill<G> fillStructs,
        RecordTypeFill<G> fillTyped)
    {
        var groupMeta = frame.GetGroupHeader();
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

    internal static TMod ModParse<TMod, TImportMask>(
        TMod record,
        MutagenFrame frame,
        TImportMask importMask,
        RecordStructFill<TMod> fillStructs,
        ModRecordTypeFill<TMod, TImportMask> fillTyped)
        where TMod : IMod, IClearable
    {
        record.Clear();
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
            var groupHeader = frame.GetGroupHeader();
            if (!groupHeader.IsGroup)
            {
                throw new ArgumentException("Did not see GRUP header as expected.");
            }

            var len = checked((int)groupHeader.ContentLength);
            var finalPos = frame.Position + groupHeader.TotalLength;

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

    internal static async Task CompileStreamsInto(IEnumerable<Task<IEnumerable<Stream>>> inStreams, Stream outStream)
    {
        var streams = await Task.WhenAll(inStreams).ConfigureAwait(false);
        foreach (var s in streams.SelectMany(s => s))
        {
            s.Position = 0;
            s.CopyTo(outStream);
        }
    }

    internal static async Task CompileStreamsInto(Task<IEnumerable<Stream>> inStreams, Stream outStream)
    {
        var streams = await inStreams.ConfigureAwait(false);
        foreach (var s in streams)
        {
            s.Position = 0;
            s.CopyTo(outStream);
        }
    }

    internal static async Task CompileStreamsInto(IEnumerable<Task<Stream>> inStreams, Stream outStream)
    {
        foreach (var sTask in inStreams)
        {
            var s = await sTask.ConfigureAwait(false);
            s.Position = 0;
            s.CopyTo(outStream);
        }
    }

    internal static void CompileStreamsInto(IEnumerable<Stream> inStreams, Stream outStream)
    {
        foreach (var s in inStreams)
        {
            s.Position = 0;
            s.CopyTo(outStream);
        }
    }

    internal static void SetGroupLength(
        byte[] bytes,
        uint len)
    {
        var bytesSpan = bytes.AsSpan();
        BinaryPrimitives.WriteUInt32LittleEndian(bytesSpan.Slice(4), len);
    }

    internal static async Task<IEnumerable<Stream>> CompileSetGroupLength(
        IEnumerable<Task<Stream>> streams,
        byte[] bytes)
    {
        var ret = await Task.WhenAll(streams).ConfigureAwait(false);
        PluginUtilityTranslation.SetGroupLength(bytes, (uint)ret.Sum(i => i.Length));
        return ret;
    }

    internal static void CompileSetGroupLength(
        IEnumerable<Stream> streams,
        byte[] bytes)
    {
        PluginUtilityTranslation.SetGroupLength(bytes, (uint)streams.NotNull().Sum(i => i.Length));
    }

    internal static void SkipPastAll(IBinaryReadStream stream, GameConstants meta, RecordType recordType)
    {
        while (stream.TryReadSubrecord(meta, recordType, out var _))
        {
        }
    }

    internal static RecordType GetRecordType<T>()
    {
        return (RecordType)LoquiRegistration.GetRegister(typeof(T))!.GetType()
            .GetField(Constants.TriggeringRecordTypeMember)!.GetValue(null)!;
    }

    internal static ReadOnlyMemorySlice<byte>? ReadByteArrayWithOverflow(
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

    internal static int HandleOverlayRecordOverflow(
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