using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using static Mutagen.Bethesda.Plugins.Binary.Translations.PluginUtilityTranslation;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal sealed class GenderedItemBinaryTranslation
{
    internal enum GenderEnum
    {
        Male = 0,
        Female = 1,
    }
    
    public static GenderedItem<TItem> ParseRequired<TItem>(
        MutagenFrame frame,
        BinarySubParseDelegate<MutagenFrame, TItem> transl)
    {
        if (!transl(frame, out var male))
        {
            throw new ArgumentException();
        }
        if (!transl(frame, out var female))
        {
            throw new ArgumentException();
        }
        return new GenderedItem<TItem>(male, female);
    }
    
    public static GenderedItem<TItem> ParseRequired<TItem>(
        MutagenFrame frame,
        RecordType genderEnumRecord,
        RecordType contentMarker,
        BinarySubParseDelegate<MutagenFrame, TItem> transl)
        where TItem : new()
    {
        int i = 0;
        TItem? male = default, female = default;
        while (i < 2 && frame.TryReadSubrecord(genderEnumRecord, out var markerRec))
        {
            i++;
            switch ((GenderEnum)markerRec.AsInt32())
            {
                case GenderEnum.Male:
                    if (!transl(frame, out male))
                    {
                        throw new ArgumentException();
                    }
                    break;
                case GenderEnum.Female:
                    if (!transl(frame, out female))
                    {
                        throw new ArgumentException();
                    }
                    break;
            }
        }

        if (male == null)
        {
            male = new TItem();
        }
        if (female == null)
        {
            female = new TItem();
        }
        return new GenderedItem<TItem>(male, female);
    }

    public static GenderedItem<TItem> Parse<TItem>(
        IBinaryReadStream reader,
        BinarySubParseDelegate<IBinaryReadStream, TItem> transl)
    {
        if (!transl(reader, out var male))
        {
            throw new ArgumentException();
        }
        if (!transl(reader, out var female))
        {
            throw new ArgumentException();
        }
        return new GenderedItem<TItem>(male, female);
    }

    public static GenderedItem<TItem?> Parse<TItem>(
        MutagenFrame frame,
        BinaryMasterParseDelegate<TItem> transl,
        RecordTypeConverter femaleRecordConverter,
        RecordTypeConverter? maleRecordConverter = null,
        bool shortCircuit = true)
        where TItem : class
    {
        if (!transl(frame, out var male, new TypedParseParams(
                lengthOverride: null,
                recordTypeConverter: maleRecordConverter,
                doNotShortCircuit: !shortCircuit)))
        {
            male = null;
        }
        if (!transl(frame, out var female, new TypedParseParams(
                lengthOverride: null,
                recordTypeConverter: femaleRecordConverter,
                doNotShortCircuit: !shortCircuit)))
        {
            female = null;
        }
        return new GenderedItem<TItem?>(male, female);
    }

    public static GenderedItem<TItem> ParseRequired<TItem>(
        MutagenFrame frame,
        BinaryMasterParseDelegate<TItem> transl,
        RecordTypeConverter? recordTypeConverter = null)
    {
        var p = new TypedParseParams(
            lengthOverride: null,
            recordTypeConverter: recordTypeConverter,
            doNotShortCircuit: false);
        if (!transl(frame, out var male, p))
        {
            throw new ArgumentException();
        }
        if (!transl(frame, out var female, p))
        {
            throw new ArgumentException();
        }
        return new GenderedItem<TItem>(male, female);
    }

    public static GenderedItem<TItem?> Parse<TItem>(
        MutagenFrame frame,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinarySubParseDelegate<MutagenFrame, TItem> transl,
        bool skipMarker)
        where TItem : class
    {
        TItem? male = default, female = default;
        for (int i = 0; i < 2; i++)
        {
            if (frame.Reader.Complete) break;
            var subHeader = frame.GetSubrecordHeader();
            RecordType type = subHeader.RecordType;
            if (type != maleMarker && type != femaleMarker)
            {
                break;
            }
            TItem? item;
            if (skipMarker)
            {
                frame.Position += subHeader.TotalLength;
                if (!transl(frame, out item))
                {
                    continue;
                }
            }
            else
            {
                frame.Position += subHeader.HeaderLength;
                if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out item))
                {
                    continue;
                }
            }
            if (type == maleMarker)
            {
                male = item;
            }
            else if (type == femaleMarker)
            {
                female = item;
            }
            else
            {
                throw new ArgumentException();
            }
        }
        return new GenderedItem<TItem?>(male, female);
    }

    public static GenderedItem<TItem> ParseRequired<TItem>(
        MutagenFrame frame,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinarySubParseDelegate<MutagenFrame, TItem> transl,
        bool skipMarker,
        TItem fallback)
    {
        TItem male = fallback, female = fallback;
        for (int i = 0; i < 2; i++)
        {
            if (frame.Reader.Complete) break;
            var subHeader = frame.GetSubrecordHeader();
            RecordType type = subHeader.RecordType;
            if (type != maleMarker && type != femaleMarker)
            {
                break;
            }
            TItem item;
            if (skipMarker)
            {
                frame.Position += subHeader.TotalLength;
                if (!transl(frame, out var translItem))
                {
                    continue;
                }
                item = translItem;
            }
            else
            {
                frame.Position += subHeader.HeaderLength;
                if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out var translItem))
                {
                    continue;
                }
                item = translItem;
            }
            if (type == maleMarker)
            {
                male = item;
            }
            else if (type == femaleMarker)
            {
                female = item;
            }
            else
            {
                throw new ArgumentException();
            }
        }
        return new GenderedItem<TItem>(male, female);
    }


    public static GenderedItem<TItem?> Parse<TItem>(
        MutagenFrame frame,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterParseDelegate<TItem> transl,
        TypedParseParams maleRecordConverter = default,
        TypedParseParams femaleRecordConverter = default)
        where TItem : class
    {
        femaleRecordConverter = femaleRecordConverter.ShortCircuit();
        TItem? male = default, female = default;
        for (int i = 0; i < 2; i++)
        {
            if (frame.Reader.Complete) break;
            var subHeader = frame.GetSubrecordHeader();
            RecordType type = subHeader.RecordType;
            if (type == maleMarker)
            {
                frame.Position += subHeader.TotalLength;
                if (!transl(frame, out male, maleRecordConverter))
                {
                    male = null;
                }
            }
            else if (type == femaleMarker)
            {
                frame.Position += subHeader.TotalLength;
                if (!transl(frame, out female, femaleRecordConverter))
                {
                    female = null;
                }
            }
            else
            {
                break;
            }
        }
        return new GenderedItem<TItem?>(male, female);
    }

    public static GenderedItem<TItem?> Parse<TItem>(
        MutagenFrame frame,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterParseDelegate<TItem> transl,
        TypedParseParams translationParams)
        where TItem : class
    {
        return Parse<TItem>(frame, maleMarker, femaleMarker,
            transl, maleRecordConverter: translationParams,
            femaleRecordConverter: translationParams);
    }

    public static GenderedItem<TItem?> Parse<TItem>(
        MutagenFrame frame,
        RecordType maleMarker,
        RecordType femaleMarker,
        RecordType contentMarker,
        BinarySubParseDelegate<MutagenFrame, TItem> transl,
        bool skipMarker)
        where TItem : class
    {
        TItem? male = default, female = default;
        for (int i = 0; i < 2; i++)
        {
            if (frame.Reader.Complete) break;
            var genderedHeader = frame.GetSubrecordHeader();
            RecordType type = genderedHeader.RecordType;
            if (type != maleMarker && type != femaleMarker)
            {
                break;
            }
            frame.Position += genderedHeader.TotalLength;
            var subHeader = frame.GetSubrecordHeader();
            if (contentMarker != subHeader.RecordType)
            {
                break;
            }
            TItem? item;
            if (skipMarker)
            {
                frame.Position += subHeader.TotalLength;
                if (!transl(frame, out item))
                {
                    continue;
                }
            }
            else
            {
                frame.Position += subHeader.HeaderLength;
                if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out item))
                {
                    continue;
                }
            }
            if (type == maleMarker)
            {
                male = item;
            }
            else if (type == femaleMarker)
            {
                female = item;
            }
            else
            {
                throw new ArgumentException();
            }
        }
        return new GenderedItem<TItem?>(male, female);
    }

    public static GenderedItem<TItem?> ParseMarkerAheadOfItem<TItem>(
        MutagenFrame frame,
        RecordType marker,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterParseDelegate<TItem> transl,
        RecordTypeConverter? femaleRecordConverter = null)
        where TItem : class
    {
        TItem? male = default, female = default;
        for (int i = 0; i < 2; i++)
        {
            if (frame.Reader.Complete) break;
            var markerHeader = frame.GetSubrecordHeader();
            if (markerHeader.RecordType != marker) break;
            frame.Position += markerHeader.TotalLength;

            var genderedHeader = frame.GetSubrecordHeader();
            RecordType type = genderedHeader.RecordType;
            if (type != maleMarker && type != femaleMarker)
            {
                break;
            }
            frame.Position += genderedHeader.TotalLength;
            TypedParseParams p = new TypedParseParams(null, 
                recordTypeConverter: type == maleMarker ? null : femaleRecordConverter,
                doNotShortCircuit: false);
            if (!transl(frame, out var item, p))
            {
                continue;
            }
            if (type == maleMarker)
            {
                male = item;
            }
            else if (type == femaleMarker)
            {
                female = item;
            }
            else
            {
                throw new ArgumentException();
            }
        }
        return new GenderedItem<TItem?>(male, female);
    }

    public static GenderedItem<TItem?> ParseMarkerWithinItem<TItem>(
        MutagenFrame frame,
        RecordType marker,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterParseDelegate<TItem> transl,
        RecordTypeConverter? femaleRecordConverter = null)
        where TItem : class
    {
        TItem? male = default, female = default;
        for (int i = 0; i < 2; i++)
        {
            if (frame.Reader.Complete) break;

            var genderedHeader = frame.GetSubrecordHeader();
            RecordType type = genderedHeader.RecordType;
            if (type != maleMarker && type != femaleMarker)
            {
                break;
            }
            frame.Position += genderedHeader.TotalLength;
            
            var markerHeader = frame.GetSubrecordHeader();
            if (markerHeader.RecordType != marker) break;
            frame.Position += markerHeader.TotalLength;
            
            TypedParseParams p = new TypedParseParams(null, 
                recordTypeConverter: type == maleMarker ? null : femaleRecordConverter,
                doNotShortCircuit: false);
            if (!transl(frame, out var item, p))
            {
                continue;
            }
            if (type == maleMarker)
            {
                male = item;
            }
            else if (type == femaleMarker)
            {
                female = item;
            }
            else
            {
                throw new ArgumentException();
            }
        }
        return new GenderedItem<TItem?>(male, female);
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (item == null) return;
        var male = item.Male;
        if (male != null)
        {
            transl(writer, male);
        }
        var female = item.Female;
        if (female != null)
        {
            transl(writer, female);
        }
    }

    public static void WriteGenderedEnumRecord<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T> item,
        RecordType genderEnumRecord,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        var male = item.Male;
        using (HeaderExport.Subrecord(writer, genderEnumRecord))
        {
            writer.Write((int)GenderEnum.Male);
        }
        transl(writer, male);
        using (HeaderExport.Subrecord(writer, genderEnumRecord))
        {
            writer.Write((int)GenderEnum.Female);
        }
        var female = item.Female;
        transl(writer, female);
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        BinaryMasterWriteDelegate<T> transl,
        RecordTypeConverter femaleRecordConverter,
        RecordTypeConverter? maleRecordConverter = null)
    {
        if (item == null) return;
        var male = item.Male;
        if (male != null)
        {
            transl(writer, male, maleRecordConverter);
        }
        var female = item.Female;
        if (female != null)
        {
            transl(writer, female, femaleRecordConverter);
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (item == null) return;
        try
        {
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.Subrecord(writer, maleMarker))
                {
                    transl(writer, male);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, maleMarker);
            throw;
        }
        try
        {
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.Subrecord(writer, femaleMarker))
                {
                    transl(writer, female);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, femaleMarker);
            throw;
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterWriteDelegate<T> transl,
        bool markerWrap = true,
        RecordTypeConverter? maleRecordConverter = null,
        RecordTypeConverter? femaleRecordConverter = null)
    {
        if (item == null) return;
        try
        {
            var male = item.Male;
            using (HeaderExport.Subrecord(writer, maleMarker))
            {
                if (markerWrap)
                {
                    transl(writer, male, maleRecordConverter);
                }
            }
            if (!markerWrap)
            {
                transl(writer, male, maleRecordConverter);
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, maleMarker);
            throw;
        }
        try
        {
            var female = item.Female;
            using (HeaderExport.Subrecord(writer, femaleMarker))
            {
                if (markerWrap)
                {
                    transl(writer, female, femaleRecordConverter);
                }
            }
            if (!markerWrap)
            {
                transl(writer, female, femaleRecordConverter);
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, femaleMarker);
            throw;
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterWriteDelegate<T> transl,
        RecordTypeConverter? recordTypeConverter,
        bool markerWrap = true)
    {
        Write<T>(writer, item, maleMarker, femaleMarker, transl, markerWrap,
            maleRecordConverter: recordTypeConverter, femaleRecordConverter: recordTypeConverter);
    }

    public static void Write<TMajor>(
        MutagenWriter writer,
        IGenderedItemGetter<IFormLinkNullableGetter<TMajor>>? item,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterWriteDelegate<IFormLinkNullableGetter<TMajor>> transl,
        bool markerWrap = true,
        RecordTypeConverter? recordTypeConverter = null)
        where TMajor : class, IMajorRecordGetter
    {
        if (item == null) return;
        try
        {
            var male = item.Male;
            if (male.FormKeyNullable != null)
            {
                using (HeaderExport.Subrecord(writer, maleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, male, recordTypeConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, male, recordTypeConverter);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, maleMarker);
            throw;
        }
        try
        {
            var female = item.Female;
            if (female.FormKeyNullable != null)
            {
                using (HeaderExport.Subrecord(writer, femaleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, female, recordTypeConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, female, recordTypeConverter);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, femaleMarker);
            throw;
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType markerType,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (item == null) return;
        using (HeaderExport.Subrecord(writer, markerType))
        {
        }
        try
        {
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.Subrecord(writer, maleMarker))
                {
                    transl(writer, male);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, maleMarker);
            throw;
        }
        try
        {
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.Subrecord(writer, femaleMarker))
                {
                    transl(writer, female);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, femaleMarker);
            throw;
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType markerType,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterWriteDelegate<T> transl,
        bool markerWrap = true,
        RecordTypeConverter? recordTypeConverter = null)
    {
        if (item == null) return;
        using (HeaderExport.Subrecord(writer, markerType))
        {
        }
        try
        {
            var male = item.Male;
            using (HeaderExport.Subrecord(writer, maleMarker))
            {
                if (male != null && markerWrap)
                {
                    transl(writer, male, recordTypeConverter);
                }
            }
            if (male != null && !markerWrap)
            {
                transl(writer, male, recordTypeConverter);
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, maleMarker);
            throw;
        }
        try
        {
            var female = item.Female;
            using (HeaderExport.Subrecord(writer, femaleMarker))
            {
                if (female != null && markerWrap)
                {
                    transl(writer, female, recordTypeConverter);
                }
            }
            if (female != null && !markerWrap)
            {
                transl(writer, female, recordTypeConverter);
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, femaleMarker);
            throw;
        }
    }

    public static void WriteMarkerAheadOfItem<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType markerType,
        RecordType maleMarker,
        RecordType femaleMarker,
        BinaryMasterWriteDelegate<T> transl,
        bool markerWrap = true,
        RecordTypeConverter? femaleRecordConverter = null)
    {
        if (item == null) return;
        try
        {
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.Subrecord(writer, markerType))
                {
                }
                using (HeaderExport.Subrecord(writer, maleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, male, translationParams: null);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, male, translationParams: null);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, maleMarker);
            throw;
        }
        try
        {
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.Subrecord(writer, markerType))
                {
                }
                using (HeaderExport.Subrecord(writer, femaleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, female, femaleRecordConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, female, femaleRecordConverter);
                }
            }
        }
        catch (Exception ex)
        {
            SubrecordException.EnrichAndThrow(ex, femaleMarker);
            throw;
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType recordType,
        BinarySubWriteDelegate<MutagenWriter, T> transl)
    {
        if (item == null) return;
        using (HeaderExport.Subrecord(writer, recordType))
        {
            var male = item.Male;
            if (male != null)
            {
                transl(writer, male);
            }
            var female = item.Female;
            if (female != null)
            {
                transl(writer, female);
            }
        }
    }

    public static void Write<T>(
        MutagenWriter writer,
        IGenderedItemGetter<T>? item,
        RecordType recordType,
        BinaryMasterWriteDelegate<T> transl,
        RecordTypeConverter? recordTypeConverter = null)
    {
        if (item == null) return;
        using (HeaderExport.Subrecord(writer, recordType))
        {
            var male = item.Male;
            if (male != null)
            {
                transl(writer, male, recordTypeConverter);
            }
            var female = item.Female;
            if (female != null)
            {
                transl(writer, female, recordTypeConverter);
            }
        }
    }
}