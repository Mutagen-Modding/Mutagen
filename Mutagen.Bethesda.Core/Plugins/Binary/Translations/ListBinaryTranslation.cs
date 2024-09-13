using Noggog; 
using Loqui; 
using System.Buffers.Binary; 
using Mutagen.Bethesda.Plugins.Binary.Streams; 
using static Mutagen.Bethesda.Plugins.Binary.Translations.PluginUtilityTranslation; 
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation; 
using Mutagen.Bethesda.Translations.Binary; 
using Mutagen.Bethesda.Plugins.Exceptions; 
using Mutagen.Bethesda.Plugins.Internals; 
 
namespace Mutagen.Bethesda.Plugins.Binary.Translations; 
 
internal sealed class ListBinaryTranslation<T> : ListBinaryTranslation<MutagenWriter, MutagenFrame, T> 
{ 
    public new static readonly ListBinaryTranslation<T> Instance = new(); 
 
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
                var subHeader = reader.ReadSubrecordHeader(); 
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
                var subHeader = reader.ReadSubrecordHeader(); 
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
        RecordTriggerSpecs? triggeringRecord = null) 
    { 
        var ret = new ExtendedList<T>(); 
        while (!reader.Complete) 
        { 
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader); 
            if (!triggeringRecord?.TriggeringRecordTypes.Contains(nextRecord) ?? false) break; 
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
        RecordTriggerSpecs? triggeringRecord = null, 
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var ret = new ExtendedList<T>(); 
        while (!reader.Complete) 
        { 
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader); 
            nextRecord = translationParams.ConvertToStandard(nextRecord); 
            if (!triggeringRecord?.TriggeringRecordTypes.Contains(nextRecord) ?? false) break; 
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
        TypedParseParams translationParams = default, 
        bool skipHeader = false) 
    { 
        var ret = new ExtendedList<T>(); 
        translationParams = translationParams.ShortCircuit(); 
        triggeringRecord = translationParams.ConvertToCustom(triggeringRecord); 
        while (!reader.Complete && !reader.Reader.Complete) 
        { 
            if (!reader.Reader.TryGetSubrecordHeader(triggeringRecord, out var header)) break; 
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
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var frames = new List<MutagenFrame>(); 
        triggeringRecord = translationParams.ConvertToCustom(triggeringRecord); 
        while (!reader.Complete && !reader.Reader.Complete) 
        { 
            var header = reader.Reader.GetVariableHeader(subRecords: false); 
            if (header.RecordType != triggeringRecord) break; 
            if (!IsLoqui) 
            { 
                throw new NotImplementedException(); 
            } 
            var totalLen = header.TotalLength; 
            var subFrame = reader.ReadAndReframe(checked((int)totalLen)); 
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
        TypedParseParams translationParams = default) 
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
        RecordTriggerSpecs triggeringRecord) 
    { 
        var ret = new ExtendedList<T>(); 
        while (!reader.Complete) 
        { 
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader); 
            if (!triggeringRecord?.TriggeringRecordTypes.Contains(nextRecord) ?? false) break; 
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
        RecordTriggerSpecs triggeringRecord, 
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var ret = new ExtendedList<T>(); 
        while (!reader.Complete) 
        { 
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader); 
            nextRecord = translationParams.ConvertToStandard(nextRecord); 
            if (!triggeringRecord?.TriggeringRecordTypes.Contains(nextRecord) ?? false) break; 
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
    
    public ExtendedList<T> Parse( 
        MutagenFrame reader, 
        RecordType itemStartMarker,
        RecordType itemEndMarker,
        BinaryMasterParseDelegate<T> transl, 
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var ret = new ExtendedList<T>(); 
        while (!reader.Complete) 
        { 
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader); 
            nextRecord = translationParams.ConvertToStandard(nextRecord); 
            if (nextRecord != itemStartMarker) break; 
            reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength; 
            var startingPos = reader.Position; 
            if (transl(reader, out var subIitem, translationParams)) 
            { 
                ret.Add(subIitem); 
            }

            reader.TryReadSubrecord(itemEndMarker, out _);
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
        TypedParseParams translationParams = default,
        RecordType? endMarker = null) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var ret = new ExtendedList<T>(); 
        while (!reader.Complete) 
        {
            if (endMarker != null)
            {
                var rec = reader.GetSubrecord();
                if (rec.RecordType == endMarker.Value)
                {
                    reader.Position += rec.TotalLength;
                    break;
                }
            }
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
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
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
        byte expectedLengthLength, 
        uint expectedLength, 
        BinaryMasterParseDelegate<T> transl, 
        TypedParseParams translationParams = default)
    {
        uint readLength;
        switch (expectedLengthLength)
        {
            case 1:
                readLength = reader.ReadUInt8();
                break;
            case 2:
                readLength = reader.ReadUInt16();
                break;
            case 4:
                readLength = reader.ReadUInt32();
                break;
            default:
                throw new NotImplementedException();
        }

        if (readLength != expectedLength)
        {
            throw new ArgumentException(
                $"Expected length did not match listed length: {expectedLength} != {readLength}");
        }
        return Parse(reader, amount: amount, transl, translationParams);
    } 
 
    public ExtendedList<T> Parse( 
        MutagenFrame reader, 
        int amount, 
        RecordType triggeringRecord, 
        BinaryMasterParseDelegate<T> transl, 
        TypedParseParams translationParams = default) 
    { 
        if (amount == 0) return new ExtendedList<T>(); 
        translationParams = translationParams.ShortCircuit(); 
        var subHeader = reader.GetSubrecordHeader(); 
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
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var subHeader = reader.GetSubrecord(); 
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
        var subHeader = reader.GetSubrecordHeader(); 
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
        var subHeader = reader.GetSubrecord(); 
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
            reader.Position += subHeader.HeaderLength; 
            return Parse( 
                reader.SpawnWithLength(subHeader.ContentLength), 
                transl); 
        } 
    } 
 
    public ExtendedList<T> ParsePerItem( 
        MutagenFrame reader, 
        int amount, 
        RecordType triggeringRecord, 
        BinaryMasterParseDelegate<T> transl, 
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var ret = new ExtendedList<T>(); 
        if (amount == 0) return ret; 
        var startingPos = reader.Position; 
        for (int i = 0; i < amount; i++) 
        { 
            var subHeader = reader.GetSubrecordHeader(); 
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
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var subHeader = reader.GetSubrecord(); 
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
            var subHeader = reader.GetSubrecordHeader(); 
            if (subHeader.RecordType != triggeringRecord) 
            { 
                throw SubrecordException.Enrich( 
                    new MalformedDataException($"Unexpected record encountered: {subHeader.RecordType}"), 
                    triggeringRecord); 
            } 
            if (!IsLoqui) 
            { 
                reader.Position += reader.MetaData.Constants.SubConstants.HeaderLength;
                if (transl(reader.SpawnWithLength(subHeader.ContentLength), out var subIitem)) 
                { 
                    ret.Add(subIitem); 
                } 
            }
            else
            {
                if (transl(reader, out var subIitem)) 
                { 
                    ret.Add(subIitem); 
                } 
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
        var subHeader = reader.GetSubrecord(); 
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
        RecordTriggerSpecs triggeringRecord, 
        TypedParseParams translationParams = default, 
        bool nullIfZero = true,
        RecordType? endMarker = null) 
    { 
        translationParams = translationParams.ShortCircuit(); 
        var subHeader = reader.GetSubrecord(); 
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
                nullIfZero: nullIfZero,
                endMarker: endMarker); 
        } 
        else if (recType == endMarker)
        {
            reader.Position += subHeader.TotalLength;
            return Array.Empty<T>();
        }
        else
        { 
            return Parse( 
                reader, 
                transl,
                endMarker: endMarker); 
        } 
    } 
 
    public IEnumerable<T> ParsePerItem( 
        MutagenFrame reader, 
        int amount, 
        BinaryMasterParseDelegate<T> transl, 
        RecordTriggerSpecs? triggeringRecord = null, 
        TypedParseParams translationParams = default, 
        bool nullIfZero = false,
        RecordType? endMarker = null) 
    { 
        if (amount == 0 && nullIfZero) return Enumerable.Empty<T>(); 
        translationParams = translationParams.ShortCircuit(); 
        var ret = new ExtendedList<T>(); 
        var startingPos = reader.Position; 
        for (int i = 0; i < amount; i++) 
        { 
            var nextRecord = HeaderTranslation.GetNextRecordType(reader.Reader); 
            if (endMarker == nextRecord)
            {
                reader.ReadSubrecord();
                break;
            }
            if (!triggeringRecord?.TriggeringRecordTypes.Contains(nextRecord) ?? false) break;
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

        if (endMarker.HasValue)
        {
            reader.TryReadSubrecord(endMarker.Value, out _);
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
        TypedWriteParams translationParams = default) 
    { 
        if (items == null) return; 
        foreach (var item in items) 
        {
            transl(writer, item, translationParams); 
        } 
    } 
 
    public void Write( 
        MutagenWriter writer, 
        IEnumerable<T>? items, 
        RecordType itemStartMarker,
        RecordType itemEndMarker,
        BinaryMasterWriteDelegate<T> transl, 
        TypedWriteParams translationParams = default) 
    { 
        if (items == null) return; 
        foreach (var item in items) 
        {
            using (HeaderExport.Subrecord(writer, itemStartMarker))
            {
            }
            transl(writer, item, translationParams); 
            using (HeaderExport.Subrecord(writer, itemEndMarker))
            {
            }
        } 
    } 
 
    public void Write( 
        MutagenWriter writer, 
        IReadOnlyList<T>? items, 
        RecordType recordType, 
        BinarySubWriteDelegate<MutagenWriter, T> transl,
        bool writeNullSuffix = false) 
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

                    if (writeNullSuffix)
                    {
                        writer.WriteZeros(1);
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
        ReadOnlyMemorySlice<T>? items, 
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
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Length} items.", 
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
        TypedWriteParams translationParams = default) 
    { 
        if (items == null) return; 
        try 
        { 
            try 
            { 
                using (HeaderExport.Subrecord( 
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
        ReadOnlyMemorySlice<T>? items, 
        RecordType recordType, 
        RecordType overflowRecord, 
        BinaryMasterWriteDelegate<T> transl, 
        TypedWriteParams translationParams = default) 
    { 
        if (items == null) return; 
        try 
        { 
            try 
            { 
                using (HeaderExport.Subrecord( 
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
                    $"{recordType} List<{typeof(T)}> had an overflow with {items?.Length} items.", 
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
        TypedWriteParams translationParams = default) 
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
        TypedWriteParams translationParams = default) 
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
        TypedWriteParams translationParams = default) 
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
 
    public void Write( 
        MutagenWriter writer, 
        IReadOnlyList<T>? items, 
        int countLengthLength, 
        byte expectedLengthLength, 
        uint expectedLength, 
        BinaryMasterWriteDelegate<T> transl, 
        TypedWriteParams translationParams = default) 
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

        switch (expectedLengthLength)
        {
            case 0:
                break;
            case 1:
                writer.Write(checked((byte)expectedLength)); 
                break;
            case 2:
                writer.Write(checked((ushort)expectedLength)); 
                break;
            case 4:
                writer.Write(expectedLength); 
                break;
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
 
    private OverflowException EnrichOverflowException( 
        OverflowException overflow, 
        RecordType recordType, 
        IReadOnlyList<T>? items) 
    { 
        return new OverflowException(  
            $"{recordType} List<{typeof(T)}> had an overflow.  Too many items to fit in the counter subrecord: {items?.Count}",  
            overflow);  
    }

    private void WriteCounter(
        MutagenWriter writer,
        int count, 
        byte counterLength,
        RecordType counterType)
    {
        using (HeaderExport.Subrecord(writer, counterType))
        {
            int maxCount;
            // Fall back to 0 if overflow
            switch (counterLength)
            {
                case 1:
                    maxCount = byte.MaxValue;
                    break;
                case 2:
                    maxCount = ushort.MaxValue;
                    break;
                default:
                    maxCount = int.MaxValue;
                    break;
            }

            if (count > maxCount)
            {
                count = 0;
            }
            writer.Write(count, counterLength); 
        } 
    }

    public void WriteWithCounter( 
        MutagenWriter writer, 
        IReadOnlyList<T>? items, 
        RecordType counterType, 
        RecordType recordType, 
        BinarySubWriteDelegate<MutagenWriter, T> transl, 
        byte counterLength, 
        bool subRecordPerItem = false, 
        bool writeCounterIfNull = false) 
    { 
        try 
        { 
            if (items == null) 
            { 
                if (writeCounterIfNull)
                {
                    WriteCounter(writer, 0, counterLength: counterLength, counterType);
                } 
                return; 
            } 

            WriteCounter(writer, items.Count, counterLength: counterLength, counterType);
        } 
        catch (OverflowException overflow)
        {
            throw SubrecordException.Enrich(
                EnrichOverflowException(overflow, counterType, items),
                counterType);
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
                        transl(writer, item); 
                    } 
                } 
            } 
            else 
            { 
                using (HeaderExport.Subrecord(writer, recordType)) 
                { 
                    foreach (var item in items) 
                    { 
                        transl(writer, item); 
                    } 
                } 
            } 
        } 
        catch (OverflowException overflow)
        {
            throw SubrecordException.Enrich(
                EnrichOverflowException(overflow, recordType, items),
                recordType);
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
        TypedWriteParams translationParams = default) 
    { 
        try 
        { 
            if (items == null) 
            { 
                if (writeCounterIfNull) 
                { 
                    WriteCounter(writer, 0, counterLength: counterLength, counterType);
                } 
                return; 
            } 
            
            WriteCounter(writer, items.Count, counterLength: counterLength, counterType);
        } 
        catch (OverflowException overflow)
        {
            throw SubrecordException.Enrich(
                EnrichOverflowException(overflow, counterType, items),
                counterType);
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
                using (HeaderExport.Subrecord(writer, recordType)) 
                { 
                    foreach (var item in items) 
                    { 
                        transl(writer, item, translationParams); 
                    } 
                } 
            } 
        } 
        catch (OverflowException overflow)
        {
            throw SubrecordException.Enrich(
                EnrichOverflowException(overflow, recordType, items),
                recordType);
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
        bool alwaysWriteEndMarker = false,
        RecordType? endMarker = null, 
        RecordTypeConverter? recordTypeConverter = null) 
    { 
        try 
        { 
            if (items != null || writeCounterIfNull) 
            { 
                WriteCounter(writer, items?.Count ?? 0, counterLength: counterLength, counterType);
            } 
        } 
        catch (OverflowException overflow)
        {
            throw SubrecordException.Enrich(
                EnrichOverflowException(overflow, counterType, items),
                counterType);
        } 
        catch (Exception ex) 
        { 
            throw SubrecordException.Enrich(ex, counterType); 
        }

        if (items != null)
        {
            foreach (var item in items) 
            { 
                transl(writer, item, recordTypeConverter); 
            } 
        }
        
        if (endMarker != null && (alwaysWriteEndMarker || items?.Count > 0)) 
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
 
internal sealed class PluginListAsyncBinaryTranslation<T> 
{ 
    public static readonly PluginListAsyncBinaryTranslation<T> Instance = new PluginListAsyncBinaryTranslation<T>(); 
 
    public delegate Task<TryGet<T>> BinarySubParseDelegate(MutagenFrame reader); 
    public delegate Task<TryGet<T>> BinaryMasterParseDelegate( 
        MutagenFrame reader, 
        TypedParseParams translationParams); 
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
        TypedParseParams translationParams = default) 
    { 
        var ret = new ExtendedList<T>(); 
        translationParams = translationParams.ShortCircuit(); 
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
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
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
        TypedParseParams translationParams = default) 
    { 
        translationParams = translationParams.ShortCircuit(); 
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