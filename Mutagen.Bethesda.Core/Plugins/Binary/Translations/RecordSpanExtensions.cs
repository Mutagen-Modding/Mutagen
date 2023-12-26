using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class RecordSpanExtensions
{
    private static SubrecordPinFrame HandleOverflow(
        ReadOnlyMemorySlice<byte> data,
        SubrecordPinFrame overflow)
    {
        var nextLen = overflow.AsUInt32();
        var loc = overflow.EndLocation;
        var nextSpan = data.Slice(loc, checked((int)(nextLen + overflow.Meta.SubConstants.HeaderLength)));
        var subHeader = new SubrecordHeader(overflow.Meta, nextSpan);
        return SubrecordPinFrame.FactoryWithOverrideLength(subHeader, nextSpan, loc, overflow.Location);
    }
    
    /// <summary>
    /// Enumerates SubrecordPinFrames of all subrecords within span
    /// 
    /// It is assumed the span contains only subrecords
    /// </summary>
    /// <param name="span">Bytes containing subrecords</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="offset">Amount to offset the pin location by</param>
    /// <returns>Enumerable of SubrecordPinFrames</returns>
    public static IEnumerable<SubrecordPinFrame> EnumerateSubrecords(
        ReadOnlyMemorySlice<byte> span,
        GameConstants meta,
        int offset = 0)
    {
        int loc = offset;
        while (span.Length > loc)
        {
            var subFrame = meta.Subrecord(span.Slice(loc));
            if (meta.HeaderOverflow.Contains(subFrame.RecordType))
            { // Length overflow record
                var ret = HandleOverflow(span, subFrame.Pin(loc));
                yield return ret;
                loc = ret.EndLocation;
                continue;
            }
            yield return new SubrecordPinFrame(subFrame, loc);
            loc += subFrame.TotalLength;
        }
    }
    
    /// <summary>
    /// Enumerates SubrecordPinFrames of all subrecords within span
    /// 
    /// It is assumed the span contains only subrecords
    /// </summary>
    /// <param name="span">Bytes containing subrecords</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="action">Action to run on each enumerated subrecord</param>
    /// <param name="offset">Amount to offset the pin location by</param>
    public static void EnumerateSubrecords(
        ReadOnlyMemorySlice<byte> span,
        GameConstants meta,
        Action<SubrecordPinFrame> action,
        int offset = 0)
    {
        int loc = offset;
        while (span.Length > loc)
        {
            var subFrame = meta.Subrecord(span.Slice(loc));
            if (meta.HeaderOverflow.Contains(subFrame.RecordType))
            { // Length overflow record
                var ret = HandleOverflow(span, subFrame.Pin(loc));
                action(ret);
                loc = ret.EndLocation;
                continue;
            }
            action(new SubrecordPinFrame(subFrame, loc));
            loc += subFrame.TotalLength;
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
    public static IReadOnlyList<SubrecordPinFrame> ParseRepeatingSubrecord(
        ReadOnlyMemorySlice<byte> span, 
        GameConstants meta,
        RecordType recordType, 
        out int lenParsed)
    {
        lenParsed = 0;
        List<SubrecordPinFrame> list = new List<SubrecordPinFrame>();
        while (span.Length > lenParsed)
        {
            var subMeta = meta.SubrecordHeader(span.Slice(lenParsed));
            if (subMeta.RecordType != recordType) break;
            list.Add(new SubrecordPinFrame(meta, span.Slice(lenParsed), lenParsed));
            lenParsed += subMeta.TotalLength;
        }

        return list;
    }

    /// <summary>
    /// Parses span data and locates all uninterrupted repeating instances of target record type
    /// 
    /// It is assumed the span contains only subrecords
    /// </summary>
    /// <param name="span">Bytes containing subrecords</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="recordType">Repeating type to locate</param>
    /// <param name="offset">Location in the source span to begin looking</param>
    /// <param name="lenParsed">The amount of data located subrecords cover</param>
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static IReadOnlyList<SubrecordPinFrame> ParseRepeatingSubrecord(
        ReadOnlyMemorySlice<byte> span, 
        GameConstants meta,
        RecordType recordType, 
        int offset,
        out int lenParsed)
    {
        lenParsed = offset;
        
        List<SubrecordPinFrame> list = new List<SubrecordPinFrame>();
        while (span.Length > lenParsed)
        {
            var subMeta = meta.SubrecordHeader(span.Slice(lenParsed));
            if (subMeta.RecordType != recordType) break;
            list.Add(new SubrecordPinFrame(meta, span.Slice(lenParsed), lenParsed));
            lenParsed += subMeta.TotalLength;
        }

        lenParsed -= offset;
        return list;
    }

    /// <summary>
    /// Locates the first encountered instances of all given subrecord types, and returns an array of SubrecordPinFrames
    /// of the ones located.<br/>
    /// <br/>
    /// If a subrecord is encountered that is not of the target types, it will stop looking for more matches
    /// 
    /// </summary>
    /// <param name="data">Subrecord data to be parsed</param>
    /// <param name="recordTypes">Record types to locate</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="lenParsed">Amount of data contained in located records</param>
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static IReadOnlyList<SubrecordPinFrame?> TryFindNextSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta, out int lenParsed,
        params RecordType[] recordTypes)
    {
        return TryFindNextSubrecords(
            data: data,
            meta: meta,
            lenParsed: out lenParsed,
            stopOnAlreadyEncounteredRecord: false,
            recordTypes: recordTypes);
    }

    /// <summary>
    /// Locates the first encountered instances of all given subrecord types, and returns an array of SubrecordPinFrames
    /// of the ones located.<br/>
    /// <br/>
    /// If a subrecord is encountered that is not of the target types, it will stop looking for more matches
    /// 
    /// </summary>
    /// <param name="data">Subrecord data to be parsed</param>
    /// <param name="recordTypes">Record types to locate</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static IReadOnlyList<SubrecordPinFrame?> TryFindNextSubrecords(ReadOnlyMemorySlice<byte> data, GameConstants meta,
        params RecordType[] recordTypes)
    {
        return TryFindNextSubrecords(
            data: data,
            meta: meta,
            lenParsed: out _,
            stopOnAlreadyEncounteredRecord: false,
            recordTypes: recordTypes);
    }

    /// <summary>
    /// Locates the first encountered instances of all given subrecord types, and returns an array of SubrecordPinFrames
    /// of the ones located.<br/>
    /// <br/>
    /// If a subrecord is encountered that is not of the target types, it will stop looking for more matches. <br/>
    /// If a subrecord is encountered that was already seen, it will stop looking for more matches.
    /// </summary>
    /// <param name="data">Subrecord data to be parsed</param>
    /// <param name="recordTypes">Record types to locate</param>
    /// <param name="meta">Metadata to use in subrecord parsing</param>
    /// <param name="lenParsed">Amount of data contained in located records</param>
    /// <param name="stopOnAlreadyEncounteredRecord">Whether to stop looking if encountering a record type that has already been seen</param>
    /// <returns>SubrecordPinFrames of located records relative to given span</returns>
    public static IReadOnlyList<SubrecordPinFrame?> TryFindNextSubrecords(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        out int lenParsed,
        bool stopOnAlreadyEncounteredRecord,
        params RecordType[] recordTypes)
    {
        lenParsed = 0;
        SubrecordPinFrame? overflow = null;
        SubrecordPinFrame?[] ret = new SubrecordPinFrame?[recordTypes.Length];
        int numFound = 0;
        while (data.Length > lenParsed)
        {
            var subMeta = meta.Subrecord(data.Slice(lenParsed));
            var recType = subMeta.RecordType;
            
            if (meta.HeaderOverflow.Contains(recType))
            {
                overflow = subMeta.Pin(lenParsed);
                lenParsed += subMeta.TotalLength;
                continue;
            }
            
            bool breakOut = true;
            for (int i = 0; i < recordTypes.Length; i++)
            {
                if (recordTypes[i] == recType)
                {
                    breakOut = false;
                    if (ret[i] == null)
                    {
                        if (overflow.HasValue && meta.HeaderOverflow.Contains(overflow.Value.RecordType))
                        {
                            var rec = HandleOverflow(data, overflow.Value);
                            ret[i] = rec;
                            lenParsed += rec.ContentLength;
                        }
                        else
                        {
                            ret[i] = new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed);
                        }

                        if (++numFound >= ret.Length)
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

            overflow = null;

            if (breakOut)
            {
                return ret;
            }

            lenParsed += subMeta.TotalLength;
        }

        return ret;
    }

    public static SubrecordPinFrame? TryFindSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        RecordType recordType,
        int offset = 0)
    {
        int loc = offset;
        SubrecordPinFrame? overflow = null;
        while (data.Length > loc)
        {
            var subMeta = meta.Subrecord(data.Slice(loc));
            var nextRecType = subMeta.RecordType;
            if (nextRecType == recordType)
            {
                if (overflow.HasValue && meta.HeaderOverflow.Contains(overflow.Value.RecordType))
                {
                    return HandleOverflow(data, overflow.Value);
                }
                return new SubrecordPinFrame(meta, data.Slice(loc), loc);
            }
            else if (meta.HeaderOverflow.Contains(nextRecType))
            {
                overflow = subMeta.Pin(loc);
            }
            else if (overflow.HasValue)
            {
                loc += overflow.Value.AsInt32() + meta.SubConstants.HeaderLength;
                overflow = null;
                continue;
            }
            loc += subMeta.TotalLength;
        }

        return null;
    }

    public static SubrecordPinFrame? TryFindSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        IReadOnlyCollection<RecordType> recordTypes,
        int offset = 0)
    {
        int loc = offset;
        SubrecordPinFrame? overflow  = null;
        while (data.Length > loc)
        {
            var subFrame = new SubrecordPinFrame(meta.Subrecord(data.Slice(loc)), loc);
            var nextRecType = subFrame.RecordType;
            if (recordTypes.Contains(nextRecType))
            {
                if (overflow.HasValue && meta.HeaderOverflow.Contains(overflow.Value.RecordType))
                {
                    return HandleOverflow(data, overflow.Value);
                }
                return new SubrecordPinFrame(meta, data.Slice(loc), loc);
            }
            else if (meta.HeaderOverflow.Contains(nextRecType))
            {
                overflow = subFrame;
            }
            else if (overflow.HasValue)
            {
                loc += overflow.Value.AsInt32() + meta.SubConstants.HeaderLength;
                overflow = null;
                continue;
            }

            loc += subFrame.TotalLength;
        }

        return null;
    }

    public static SubrecordPinFrame? TryFindSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        params RecordType[] recordTypes)
    {
        return TryFindSubrecord(data, meta, (IReadOnlyCollection<RecordType>)recordTypes);
    }

    public static IReadOnlyList<SubrecordPinFrame> FindAllOfSubrecord(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        RecordType recordType)
    {
        List<SubrecordPinFrame> ret = new List<SubrecordPinFrame>();
        SubrecordPinFrame? overflow = null;
        int lenParsed = 0;
        while (data.Length > lenParsed)
        {
            var subFrame = meta.Subrecord(data.Slice(lenParsed));
            var nextRecType = subFrame.RecordType;
            if (nextRecType == recordType)
            {
                if (overflow.HasValue && meta.HeaderOverflow.Contains(overflow.Value.RecordType))
                {
                    var rec = HandleOverflow(data, overflow.Value);
                    lenParsed += rec.ContentLength;
                    ret.Add(rec);
                }
                else
                {
                    ret.Add(new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed));
                }
                overflow = null;
            }
            else if (meta.HeaderOverflow.Contains(nextRecType))
            {
                overflow = subFrame.Pin(lenParsed);
            }
            else if (overflow.HasValue)
            {
                lenParsed += overflow.Value.AsInt32() + meta.SubConstants.HeaderLength;
                overflow = null;
                continue;
            }

            lenParsed += subFrame.TotalLength;
        }

        return ret;
    }

    public static IReadOnlyList<SubrecordPinFrame> FindAllOfSubrecords(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        IReadOnlyCollection<RecordType> recordTypes)
    {
        List<SubrecordPinFrame> ret = new List<SubrecordPinFrame>();
        SubrecordPinFrame? overflow = null;
        int lenParsed = 0;
        while (data.Length > lenParsed)
        {
            var subFrame = meta.Subrecord(data.Slice(lenParsed));
            var nextRecType = subFrame.RecordType;
            if (recordTypes.Contains(nextRecType))
            {
                if (overflow.HasValue && meta.HeaderOverflow.Contains(overflow.Value.RecordType))
                {
                    var rec = HandleOverflow(data, overflow.Value);
                    lenParsed += rec.ContentLength;
                    ret.Add(rec);
                }
                else
                {
                    ret.Add(new SubrecordPinFrame(meta, data.Slice(lenParsed), lenParsed));
                }
                overflow = null;
            }
            else if (meta.HeaderOverflow.Contains(nextRecType))
            {
                overflow = subFrame.Pin(lenParsed);
            }
            else if (overflow.HasValue)
            {
                lenParsed += overflow.Value.AsInt32() + meta.SubConstants.HeaderLength;
                overflow = null;
                continue;
            }

            lenParsed += subFrame.TotalLength;
        }

        return ret;
    }

    public static IReadOnlyList<SubrecordPinFrame> FindAllOfSubrecords(
        ReadOnlyMemorySlice<byte> data,
        GameConstants meta,
        params RecordType[] recordTypes)
    {
        return FindAllOfSubrecords(data, meta, (IReadOnlyCollection<RecordType>)recordTypes);
    }
    
    public static int SkipPastAll(ReadOnlyMemorySlice<byte> data, GameConstants constants, RecordType toSkip,
        out int numRecordsPassed)
    {
        var pos = 0;
        numRecordsPassed = 0;
        while (pos < data.Length)
        {
            var subHeader = constants.SubrecordHeader(data.Slice(pos));
            if (subHeader.RecordType != toSkip) break;
            pos += subHeader.TotalLength;
            numRecordsPassed++;
        }

        return pos;
    }
}
