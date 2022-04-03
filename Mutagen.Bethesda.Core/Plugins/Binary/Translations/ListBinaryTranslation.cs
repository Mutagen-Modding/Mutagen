using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Noggog;
using Loqui;
using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using static Mutagen.Bethesda.Plugins.Binary.Translations.PluginUtilityTranslation;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class ListBinaryTranslation<T> : ListBinaryTranslation<MutagenWriter, MutagenFrame, T>
{
    public static readonly ListBinaryTranslation<T> Instance = new();

    #region Out Parameters
    public ExtendedList<T> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinarySubParseDelegate<MutagenFrame, T> transl)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete && !reader.Reader.Complete)
        {
            if (!HeaderTranslation.TryGetRecordType(reader.Reader, triggeringRecord)) break;
            var startingPos = reader.Position;
            MutagenFrame subFrame;
            if (!IsLoqui)
            {
                var subHeader = reader.ReadSubrecord();
                subFrame = reader.ReadAndReframe(subHeader.ContentLength);
            }
            else
            {
                subFrame = reader;
            }
            if (transl(subFrame, out var subItem))
            {
                ret.Add(subItem);
            }

            if (reader.Position == startingPos)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    triggeringRecord);
            }
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinarySubParseDelegate<IBinaryReadStream, T> transl)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete && !reader.Reader.Complete)
        {
            if (!HeaderTranslation.TryGetRecordType(reader.Reader, triggeringRecord)) break;
            var startingPos = reader.Position;
            MutagenFrame subFrame;
            if (!IsLoqui)
            {
                var subHeader = reader.ReadSubrecord();
                subFrame = reader.ReadAndReframe(subHeader.ContentLength);
            }
            else
            {
                subFrame = reader;
            }
            if (transl(subFrame, out var subItem))
            {
                ret.Add(subItem);
            }

            if (reader.Position == startingPos)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    triggeringRecord);
            }
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        BinarySubParseRecordDelegate<T> transl,
        IRecordCollection? triggeringRecord = null)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete)
        {
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader);
            if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            var startingPos = reader.Position;
            if (transl(reader, nextRecord, out var subIitem))
            {
                ret.Add(subIitem);
            }
            if (reader.Position == startingPos)
            {
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    nextRecord);
            }
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        BinaryMasterParseRecordDelegate<T> transl,
        IRecordCollection? triggeringRecord = null,
        TypedParseParams? translationParams = null)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete)
        {
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader);
            nextRecord = translationParams.ConvertToStandard(nextRecord);
            if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            var startingPos = reader.Position;
            if (transl(reader, nextRecord, out var subIitem, translationParams))
            {
                ret.Add(subIitem);
            }
            if (reader.Position == startingPos)
            {
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    nextRecord);
            }
        }
        return ret;
    }
    #endregion

    #region Lengthed Triggering Record
    public ExtendedList<T> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null,
        bool skipHeader = false)
    {
        var ret = new ExtendedList<T>();
        triggeringRecord = translationParams.ConvertToCustom(triggeringRecord);
        while (!reader.Complete && !reader.Reader.Complete)
        {
            if (!reader.Reader.TryGetSubrecord(triggeringRecord, out var header)) break;
            if (!IsLoqui || skipHeader)
            {
                reader.Position += header.HeaderLength;
            }
            var startingPos = reader.Position;
            if (transl(skipHeader ? reader.SpawnWithLength(header.ContentLength) : reader, out var subItem, translationParams))
            {
                ret.Add(subItem);
            }

            if (reader.Position == startingPos)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    triggeringRecord);
            }
        }
        return ret;
    }

    public IEnumerable<T> ParseParallel(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        var frames = new List<MutagenFrame>();
        triggeringRecord = translationParams.ConvertToCustom(triggeringRecord);
        while (!reader.Complete && !reader.Reader.Complete)
        {
            var header = reader.Reader.GetVariableHeader();
            if (header.RecordType != triggeringRecord) break;
            if (!IsLoqui)
            {
                throw new NotImplementedException();
            }
            var totalLen = header.TotalLength;
            var subFrame = new MutagenFrame(reader.ReadAndReframe(checked((int)totalLen)));
            frames.Add(subFrame);
        }
        var ret = new TryGet<T>[frames.Count];
        Parallel.ForEach(frames, (subFrame, state, count) =>
        {
            if (transl(subFrame, out var subItem, translationParams))
            {
                ret[count] = TryGet<T>.Succeed(subItem);
            }
            else
            {
                ret[count] = TryGet<T>.Failure;
            }
        });
        return ret.Where(i => i.Succeeded)
            .Select(i => i.Value);
    }

    public IEnumerable<T> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        bool thread,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        if (thread)
        {
            return ParseParallel(
                reader,
                triggeringRecord,
                transl,
                translationParams);
        }
        else
        {
            return Parse(
                reader,
                triggeringRecord,
                transl,
                translationParams);
        }
    }
    #endregion

    #region Lengthed Triggering Records
    public ExtendedList<T> Parse(
        MutagenFrame reader,
        BinarySubParseDelegate<MutagenFrame, T> transl,
        IRecordCollection triggeringRecord)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete)
        {
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader);
            if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            var startingPos = reader.Position;
            if (transl(reader, out var subIitem))
            {
                ret.Add(subIitem);
            }
            if (reader.Position == startingPos)
            {
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    nextRecord);
            }
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        BinaryMasterParseDelegate<T> transl,
        IRecordCollection triggeringRecord,
        TypedParseParams? translationParams = null)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete)
        {
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader);
            nextRecord = translationParams.ConvertToStandard(nextRecord);
            if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            var startingPos = reader.Position;
            if (transl(reader, out var subIitem, translationParams))
            {
                ret.Add(subIitem);
            }
            if (reader.Position == startingPos)
            {
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    nextRecord);
            }
        }
        return ret;
    }
    #endregion

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete)
        {
            if (transl(reader, out var subItem, translationParams))
            {
                ret.Add(subItem);
            }
            else
            {
                break;
            }
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        int amount,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        var ret = new ExtendedList<T>();
        for (int i = 0; i < amount; i++)
        {
            if (transl(reader, out var subItem, translationParams))
            {
                ret.Add(subItem);
            }
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        int amount,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        if (amount == 0) return new ExtendedList<T>();
        var subHeader = reader.GetSubrecord();
        if (subHeader.RecordType != triggeringRecord)
        {
            throw SubrecordException.Enrich(
                new MalformedDataException($"Unexpected record encountered: {subHeader.RecordType}"),
                triggeringRecord);
        }
        if (!IsLoqui)
        {
            reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
        }
        var ret = new ExtendedList<T>();
        var startingPos = reader.Position;
        for (int i = 0; i < amount; i++)
        {
            if (transl(reader, out var subIitem, translationParams))
            {
                ret.Add(subIitem);
            }
        }
        if (reader.Position == startingPos)
        {
            throw SubrecordException.Enrich(
                new MalformedDataException("Parsed item on the list consumed no data."),
                triggeringRecord);
        }
        return ret;
    }

    public ExtendedList<T> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        RecordType countRecord,
        int countLengthLength,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        var subHeader = reader.GetSubrecordFrame();
        var recType = subHeader.RecordType;
        if (recType == countRecord)
        {
            var count = countLengthLength switch
            {
                1 => subHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(subHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(subHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            reader.Position += subHeader.TotalLength;
            return Parse(
                reader,
                count,
                triggeringRecord,
                transl,
                translationParams);
        }
        else
        {
            return Parse(
                reader,
                triggeringRecord,
                transl,
                translationParams);
        }
    }

    public IEnumerable<T> Parse(
        MutagenFrame reader,
        int amount,
        RecordType triggeringRecord,
        BinarySubParseDelegate<MutagenFrame, T> transl,
        bool nullIfZero = false)
    {
        if (amount == 0 && nullIfZero) return Enumerable.Empty<T>();

        // Don't return early if count is zero, as we're expecting one content record still that is empty
        // But still okay if it doesn't exist
        var subHeader = reader.GetSubrecord();
        if (subHeader.RecordType != triggeringRecord)
        {
            if (amount == 0) return Enumerable.Empty<T>();
            throw SubrecordException.Enrich(
                new MalformedDataException($"List with a non zero counter did not follow up with expected type: {subHeader.RecordType}"),
                triggeringRecord);
        }
        if (!IsLoqui)
        {
            reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
        }
        var ret = new ExtendedList<T>();
        var startingPos = reader.Position;
        for (int i = 0; i < amount; i++)
        {
            if (transl(reader, out var subIitem))
            {
                ret.Add(subIitem);
            }
        }
        if (amount != 0 && reader.Position == startingPos)
        {
            throw SubrecordException.Enrich(
                new MalformedDataException($"Parsed item on the list consumed no data."),
                triggeringRecord);
        }
        return ret;
    }

    public IEnumerable<T> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        RecordType countRecord,
        int countLengthLength,
        BinarySubParseDelegate<MutagenFrame, T> transl,
        bool nullIfZero = false)
    {
        var subHeader = reader.GetSubrecordFrame();
        var recType = subHeader.RecordType;
        if (recType == countRecord)
        {
            var count = countLengthLength switch
            {
                1 => subHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(subHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(subHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            reader.Position += subHeader.TotalLength;
            return Parse(
                reader,
                count,
                triggeringRecord,
                transl,
                nullIfZero: nullIfZero);
        }
        else
        {
            return Parse(
                reader,
                triggeringRecord,
                transl);
        }
    }

    public ExtendedList<T> ParsePerItem(
        MutagenFrame reader,
        int amount,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        var ret = new ExtendedList<T>();
        if (amount == 0) return ret;
        var startingPos = reader.Position;
        for (int i = 0; i < amount; i++)
        {
            var subHeader = reader.GetSubrecord();
            if (subHeader.RecordType != triggeringRecord)
            {
                // Unexpected, but shouldn't throw if we can help it
                // Analyzers should be the ones complaining about count not being accurate
                break;
            }
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            if (transl(reader, out var subIitem, translationParams))
            {
                ret.Add(subIitem);
            }
        }
        if (reader.Position == startingPos)
        {
            throw SubrecordException.Enrich(
                new MalformedDataException($"Parsed item on the list consumed no data."),
                triggeringRecord);
        }
        return ret;
    }

    public ExtendedList<T> ParsePerItem(
        MutagenFrame reader,
        RecordType triggeringRecord,
        RecordType countRecord,
        int countLengthLength,
        BinaryMasterParseDelegate<T> transl,
        TypedParseParams? translationParams = null)
    {
        var subHeader = reader.GetSubrecordFrame();
        var recType = subHeader.RecordType;
        if (recType == countRecord)
        {
            var count = countLengthLength switch
            {
                1 => subHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(subHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(subHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            reader.Position += subHeader.TotalLength;
            return ParsePerItem(
                reader,
                count,
                triggeringRecord,
                transl,
                translationParams);
        }
        else
        {
            return Parse(
                reader,
                triggeringRecord,
                transl,
                translationParams);
        }
    }

    public ExtendedList<T> ParsePerItem(
        MutagenFrame reader,
        int amount,
        RecordType triggeringRecord,
        BinarySubParseDelegate<MutagenFrame, T> transl)
    {
        var ret = new ExtendedList<T>();
        if (amount == 0) return ret;
        var startingPos = reader.Position;
        for (int i = 0; i < amount; i++)
        {
            var subHeader = reader.GetSubrecord();
            if (subHeader.RecordType != triggeringRecord)
            {
                throw SubrecordException.Enrich(
                    new MalformedDataException($"Unexpected record encountered: {subHeader.RecordType}"),
                    triggeringRecord);
            }
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            if (transl(reader, out var subIitem))
            {
                ret.Add(subIitem);
            }
        }
        if (reader.Position == startingPos)
        {
            throw SubrecordException.Enrich(
                new MalformedDataException($"Parsed item on the list consumed no data."),
                triggeringRecord);
        }
        return ret;
    }

    public ExtendedList<T> ParsePerItem(
        MutagenFrame reader,
        RecordType triggeringRecord,
        RecordType countRecord,
        int countLengthLength,
        BinarySubParseDelegate<MutagenFrame, T> transl)
    {
        var subHeader = reader.GetSubrecordFrame();
        var recType = subHeader.RecordType;
        if (recType == countRecord)
        {
            var count = countLengthLength switch
            {
                1 => subHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(subHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(subHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            reader.Position += subHeader.TotalLength;
            return ParsePerItem(
                reader,
                count,
                triggeringRecord,
                transl);
        }
        else
        {
            return Parse(
                reader,
                triggeringRecord,
                transl);
        }
    }

    public IEnumerable<T> ParsePerItem(
        MutagenFrame reader,
        RecordType countRecord,
        int countLengthLength,
        BinaryMasterParseDelegate<T> transl,
        IRecordCollection triggeringRecord,
        TypedParseParams? translationParams = null,
        bool nullIfZero = true)
    {
        var subHeader = reader.GetSubrecordFrame();
        var recType = subHeader.RecordType;
        if (recType == countRecord)
        {
            var count = countLengthLength switch
            {
                1 => subHeader.Content[0],
                2 => (int)BinaryPrimitives.ReadUInt16LittleEndian(subHeader.Content),
                4 => checked((int)BinaryPrimitives.ReadUInt32LittleEndian(subHeader.Content)),
                _ => throw new NotImplementedException(),
            };
            reader.Position += subHeader.TotalLength;
            return ParsePerItem(
                reader,
                count,
                transl,
                triggeringRecord,
                translationParams,
                nullIfZero: nullIfZero);
        }
        else
        {
            return Parse(
                reader,
                transl,
                triggeringRecord,
                translationParams);
        }
    }

    public IEnumerable<T> ParsePerItem(
        MutagenFrame reader,
        int amount,
        BinaryMasterParseDelegate<T> transl,
        IRecordCollection? triggeringRecord = null,
        TypedParseParams? translationParams = null,
        bool nullIfZero = false)
    {
        if (amount == 0 && nullIfZero) return Enumerable.Empty<T>();
        var ret = new ExtendedList<T>();
        var startingPos = reader.Position;
        for (int i = 0; i < amount; i++)
        {
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader);
            if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            if (transl(reader, out var subIitem, translationParams))
            {
                ret.Add(subIitem);
            }
            if (reader.Position == startingPos)
            {
                throw SubrecordException.Enrich(
                    new MalformedDataException($"Parsed item on the list consumed no data."),
                    nextRecord);
            }
        }
        if (reader.Position == startingPos)
        {
            throw new MalformedDataException($"Parsed list of {amount} items consumed no data.");
        }
        return ret;
    }

    public void Write(
        MutagenWriter writer,
        IEnumerable<T>? items,
        BinaryMasterWriteDelegate<T> transl,
        TypedWriteParams? translationParams = null)
    {
        if (items == null) return;
        foreach (var item in items)
        {
            transl(writer, item, translationParams);
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType recordType,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (items == null) return;
        try
        {
            try
            {
                using (HeaderExport.Subrecord(writer, recordType))
                {
                    foreach (var item in items)
                    {
                        transl(writer, item);
                    }
                }
            }
            catch (OverflowException overflow)
            {
                throw new OverflowException(
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Count} items.",
                    overflow);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType recordType,
        RecordType overflowRecord,
        BinaryMasterWriteDelegate<T> transl,
        TypedWriteParams? translationParams = null)
    {
        if (items == null) return;
        try
        {
            try
            {
                using (var header = HeaderExport.Subrecord(
                           writer,
                           recordType, 
                           overflowRecord: overflowRecord,
                           out var writerToUse))
                {
                    foreach (var item in items)
                    {
                        transl(writerToUse, item, translationParams);
                    }
                }
            }
            catch (OverflowException overflow)
            {
                throw new OverflowException(
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Count} items.",
                    overflow);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType recordType,
        BinaryMasterWriteDelegate<T> transl,
        TypedWriteParams? translationParams = null)
    {
        if (items == null) return;
        try
        {
            try
            {
                using (HeaderExport.Subrecord(writer, recordType))
                {
                    foreach (var item in items)
                    {
                        transl(writer, item, translationParams);
                    }
                }
            }
            catch (OverflowException overflow)
            {
                throw new OverflowException(
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Count} items.",
                    overflow);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType recordType,
        int countLengthLength,
        BinaryMasterWriteDelegate<T> transl,
        TypedWriteParams? translationParams = null)
    {
        if (items == null) return;
        try
        {
            try
            {
                using (HeaderExport.Subrecord(writer, recordType))
                {
                    switch (countLengthLength)
                    {
                        case 1:
                            writer.Write(checked((byte)items.Count));
                            break;
                        case 2:
                            writer.Write(checked((ushort)items.Count));
                            break;
                        case 4:
                            writer.Write(items.Count);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    foreach (var item in items)
                    {
                        transl(writer, item, translationParams);
                    }
                }
            }
            catch (OverflowException overflow)
            {
                throw new OverflowException(
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Count} items.",
                    overflow);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void Write(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        int countLengthLength,
        BinaryMasterWriteDelegate<T> transl,
        TypedWriteParams? translationParams = null)
    {
        if (items == null) return;
        try
        {
            switch (countLengthLength)
            {

                case 1:

                    writer.Write(checked((byte)items.Count));

                    break;
                case 2:
                    writer.Write(checked((ushort)items.Count));
                    break;
                case 4:
                    writer.Write(items.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        catch (OverflowException overflow)
        {
            throw new OverflowException(
                $"List<{typeof(T)}> had an overflow with {items?.Count} items.",
                overflow);
        }
        foreach (var item in items)
        {
            transl(writer, item, translationParams);
        }
    }

    public void WritePerItem(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType recordType,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (items == null) return;
        try
        {
            foreach (var item in items)
            {
                using (HeaderExport.Subrecord(writer, recordType))
                {
                    transl(writer, item);
                }
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void WriteWithCounter(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType counterType,
        RecordType recordType,
        BinarySubWriteDelegate<MutagenWriter, T> transl,
        byte counterLength,
        bool writeCounterIfNull = false)
    {
        if (items == null)
        {
            if (writeCounterIfNull)
            {
                using (HeaderExport.Subrecord(writer, counterType))
                {
                    writer.Write(0, counterLength);
                }
            }
            return;
        }
        try
        {
            using (HeaderExport.Subrecord(writer, counterType))
            {
                writer.Write(items.Count, counterLength);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, counterType);
        }
        try
        {
            try
            {
                using (HeaderExport.Subrecord(writer, recordType))
                {
                    foreach (var item in items)
                    {
                        transl(writer, item);
                    }
                }
            }
            catch (OverflowException overflow)
            {
                throw new OverflowException(
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Count} items.",
                    overflow);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void WriteWithCounter(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType counterType,
        RecordType recordType,
        BinaryMasterWriteDelegate<T> transl,
        byte counterLength,
        bool subRecordPerItem = false,
        bool writeCounterIfNull = false,
        TypedWriteParams? translationParams = null)
    {
        if (items == null)
        {
            if (writeCounterIfNull)
            {
                using (HeaderExport.Subrecord(writer, counterType))
                {
                    writer.Write(0, counterLength);
                }
            }
            return;
        }
        try
        {
            using (HeaderExport.Subrecord(writer, counterType))
            {
                writer.Write(items.Count, counterLength);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, counterType);
        }
        try
        {
            if (subRecordPerItem)
            {
                foreach (var item in items)
                {
                    using (HeaderExport.Subrecord(writer, recordType))
                    {
                        transl(writer, item, translationParams);
                    }
                }
            }
            else
            {
                try
                {
                    using (HeaderExport.Subrecord(writer, recordType))
                    {
                        foreach (var item in items)
                        {
                            transl(writer, item, translationParams);
                        }
                    }
                }
                catch (OverflowException overflow)
                {
                    throw new OverflowException(
                        $"{recordType} List<{typeof(T)}> had an overflow with {items?.Count} items.",
                        overflow);
                }
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, recordType);
        }
    }

    public void WriteWithCounter(
        MutagenWriter writer,
        IReadOnlyList<T>? items,
        RecordType counterType,
        BinaryMasterWriteDelegate<T> transl,
        byte counterLength,
        bool writeCounterIfNull = false,
        RecordType? endMarker = null,
        RecordTypeConverter? recordTypeConverter = null)
    {
        if (items == null)
        {
            if (writeCounterIfNull)
            {
                using (HeaderExport.Header(writer, counterType, ObjectType.Subrecord))
                {
                    writer.Write(0, counterLength);
                }
            }
            return;
        }
        try
        {
            using (HeaderExport.Header(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count, counterLength);
            }
        }
        catch (Exception ex)
        {
            throw SubrecordException.Enrich(ex, counterType);
        }
        foreach (var item in items)
        {
            transl(writer, item, recordTypeConverter);
        }
        if (endMarker != null && items.Count > 0)
        {
            using (HeaderExport.Subrecord(writer, endMarker.Value)) { }
        }
    }

    #region Cache Helpers
    public void Parse<K>(
        MutagenFrame reader,
        ICache<T, K> item,
        RecordType triggeringRecord,
        BinarySubParseDelegate<MutagenFrame, T> transl)
    {
        // Should normally be SetTo, but since we want duplicate groups to merge, we're doing an Add.
        // A clear is assumed to be run by the caller ahead of time before starting to fill.
        item.Set(
            Parse(
                reader,
                triggeringRecord,
                transl: transl));
    }
    #endregion
}

public class PluginListAsyncBinaryTranslation<T>
{
    public static readonly PluginListAsyncBinaryTranslation<T> Instance = new PluginListAsyncBinaryTranslation<T>();

    public delegate Task<TryGet<T>> BinarySubParseDelegate(MutagenFrame reader);
    public delegate Task<TryGet<T>> BinaryMasterParseDelegate(
        MutagenFrame reader,
        TypedParseParams? translationParams);
    public delegate Task<TryGet<T>> BinarySubParseRecordDelegate(
        MutagenFrame reader,
        RecordType header);
    public static readonly bool IsLoqui;

    static PluginListAsyncBinaryTranslation()
    {
        IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
    }

    public async Task<IEnumerable<T>> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinarySubParseDelegate transl)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete && !reader.Reader.Complete)
        {
            if (!HeaderTranslation.TryGetRecordType(reader.Reader, triggeringRecord)) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            var startingPos = reader.Position;
            var item = await transl(reader).ConfigureAwait(false);
            if (item.Succeeded)
            {
                ret.Add(item.Value);
            }

            if (reader.Position == startingPos)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    triggeringRecord);
            }
        }
        return ret;
    }

    public async Task<IEnumerable<T>> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate transl,
        TypedParseParams? translationParams = null)
    {
        var ret = new ExtendedList<T>();
        while (!reader.Complete && !reader.Reader.Complete)
        {
            if (!HeaderTranslation.TryGetRecordType(reader.Reader, triggeringRecord)) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }
            var startingPos = reader.Position;
            var item = await transl(reader, translationParams).ConfigureAwait(false);
            if (item.Succeeded)
            {
                ret.Add(item.Value);
            }

            if (reader.Position == startingPos)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
                throw SubrecordException.Enrich(
                    new MalformedDataException("Parsed item on the list consumed no data."),
                    triggeringRecord);
            }
        }
        return ret;
    }

    public async Task<IEnumerable<T>> ParseThreaded(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinarySubParseDelegate transl)
    {
        var tasks = new List<Task<TryGet<T>>>();
        while (!reader.Complete && !reader.Reader.Complete)
        {
            var nextRec = HeaderTranslation.GetNextSubrecordType(
                reader: reader.Reader,
                contentLength: out var contentLen);
            if (nextRec != triggeringRecord) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }

            var toDo = transl(reader);

            tasks.Add(Task.Run(() => toDo));
        }
        var ret = await Task.WhenAll(tasks).ConfigureAwait(false);
        return ret.Where(i => i.Succeeded)
            .Select(i => i.Value);
    }

    public async Task<IEnumerable<T>> ParseThreaded(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate transl,
        TypedParseParams? translationParams = null)
    {
        var tasks = new List<Task<TryGet<T>>>();
        while (!reader.Complete && !reader.Reader.Complete)
        {
            var nextRec = HeaderTranslation.GetNextSubrecordType(
                reader: reader.Reader,
                contentLength: out var contentLen);
            if (nextRec != triggeringRecord) break;
            if (!IsLoqui)
            {
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
            }

            var toDo = transl(reader, translationParams);

            tasks.Add(Task.Run(() => toDo));
        }
        var ret = await Task.WhenAll(tasks).ConfigureAwait(false);
        return ret.Where(i => i.Succeeded)
            .Select(i => i.Value);
    }

    #region Lengthed Triggering Record
    public async Task<ExtendedList<T>> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinarySubParseDelegate transl,
        bool thread = false)
    {
        IEnumerable<T> items;
        if (thread)
        {
            items = await ParseThreaded(
                reader,
                triggeringRecord,
                transl: transl).ConfigureAwait(false);
        }
        else
        {
            items = await Parse(
                reader,
                triggeringRecord,
                transl: transl).ConfigureAwait(false);
        }
        return new ExtendedList<T>(items);
    }

    public async Task<ExtendedList<T>> Parse(
        MutagenFrame reader,
        RecordType triggeringRecord,
        BinaryMasterParseDelegate transl,
        bool thread,
        TypedParseParams? translationParams = null)
    {
        IEnumerable<T> items;
        if (thread)
        {
            items = await ParseThreaded(
                reader,
                triggeringRecord,
                transl: transl,
                translationParams: translationParams).ConfigureAwait(false);
        }
        else
        {
            items = await Parse(
                reader,
                triggeringRecord,
                transl: transl,
                translationParams: translationParams).ConfigureAwait(false);
        }
        return new ExtendedList<T>(items);
    }
    #endregion

    #region Cache Helpers
    public async Task Parse<K>(
        MutagenFrame reader,
        ICache<T, K> item,
        RecordType triggeringRecord,
        BinarySubParseDelegate transl)
    {
        item.SetTo(
            await Parse(
                reader,
                triggeringRecord,
                transl: transl));
    }
    #endregion
}