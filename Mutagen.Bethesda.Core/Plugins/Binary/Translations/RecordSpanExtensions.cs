using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class RecordSpanExtensions
{
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
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static SubrecordPinFrame[] ParseRepeatingSubrecord(ReadOnlyMemorySlice<byte> span, GameConstants meta,
        RecordType recordType, out int lenParsed)
    {
        lenParsed = 0;
        List<SubrecordPinFrame> list = new List<SubrecordPinFrame>();
        while (span.Length > lenParsed)
        {
            var subMeta = meta.Subrecord(span.Slice(lenParsed));
            if (subMeta.RecordType != recordType) break;
            list.Add(new SubrecordPinFrame(meta, span.Slice(lenParsed), lenParsed));
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
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="recordTypes">Record types to locate</param>
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static SubrecordPinFrame?[] FindFirstSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta,
        params RecordType[] recordTypes)
    {
        return FindFirstSubrecords(data, meta, out _, recordTypes);
    }

    /// <summary>
    /// Locates the first encountered instances of all given subrecord types, and returns an array of their locations
    /// -1 represents a recordtype that was not found.
    /// 
    /// Not suggested to use with high numbers of record types, as it is an N^2 algorithm
    /// </summary>
    /// <param name="data">Subrecord data to be parsed</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="lenParsed">Amount parsed after processing</param>
    /// <param name="recordTypes">Record types to locate</param>
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static SubrecordPinFrame?[] FindFirstSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta, out int lenParsed,
        params RecordType[] recordTypes)
    {
        lenParsed = 0;
        SubrecordPinFrame?[] ret = new SubrecordPinFrame?[recordTypes.Length];
        while (data.Length > lenParsed)
        {
            var subMeta = meta.Subrecord(data.Slice(lenParsed));
            var recType = subMeta.RecordType;
            for (int i = 0; i < recordTypes.Length; i++)
            {
                if (recordTypes[i] == recType && ret[i] == null)
                {
                    ret[i] = new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed);
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

            lenParsed += subMeta.TotalLength;
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
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static SubrecordPinFrame?[] FindNextSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta, out int lenParsed,
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
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static SubrecordPinFrame?[] FindNextSubrecords(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        out int lenParsed,
        bool stopOnAlreadyEncounteredRecord,
        params RecordType[] recordTypes)
    {
        lenParsed = 0;
        SubrecordPinFrame?[] ret = new SubrecordPinFrame?[recordTypes.Length];
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
                        ret[i] = new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed);
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

    public static SubrecordPinFrame? FindFirstSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        RecordType recordType,
        int? offset = null)
    {
        int loc = offset ?? 0;
        while (data.Length > loc)
        {
            var subMeta = meta.Subrecord(data.Slice(loc));
            if (subMeta.RecordType == recordType)
            {
                return new SubrecordPinFrame(meta, data.Slice(loc), loc);
            }

            loc += subMeta.TotalLength;
        }

        return null;
    }

    public static SubrecordPinFrame? FindFirstSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        ICollectionGetter<RecordType> recordTypes,
        int? offset = null)
    {
        int loc = offset ?? 0;
        while (data.Length > loc)
        {
            var subMeta = meta.Subrecord(data.Slice(loc));
            if (recordTypes.Contains(subMeta.RecordType))
            {
                return new SubrecordPinFrame(meta, data.Slice(loc), loc);
            }

            loc += subMeta.TotalLength;
        }

        return null;
    }

    public static SubrecordPinFrame[] FindAllOfSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        RecordType recordType)
    {
        List<SubrecordPinFrame> ret = new List<SubrecordPinFrame>();
        int lenParsed = 0;
        while (data.Length > lenParsed)
        {
            var subMeta = meta.Subrecord(data.Slice(lenParsed));
            if (subMeta.RecordType == recordType)
            {
                ret.Add(new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed));
            }

            lenParsed += subMeta.TotalLength;
        }

        return ret.ToArray();
    }

    public static SubrecordPinFrame[] FindAllOfSubrecords(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        ICollectionGetter<RecordType> recordTypes)
    {
        List<SubrecordPinFrame> ret = new List<SubrecordPinFrame>();
        int lenParsed = 0;
        while (data.Length > lenParsed)
        {
            var subMeta = meta.Subrecord(data.Slice(lenParsed));
            if (recordTypes.Contains(subMeta.RecordType))
            {
                ret.Add(new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed));
            }

            lenParsed += subMeta.TotalLength;
        }

        return ret.ToArray();
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
}
