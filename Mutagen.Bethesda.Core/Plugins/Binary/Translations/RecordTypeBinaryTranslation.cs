using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal class RecordTypeBinaryTranslation : PrimitiveBinaryTranslation<RecordType, MutagenFrame, MutagenWriter>
{
    public static readonly RecordTypeBinaryTranslation Instance = new();
    public override int ExpectedLength => 4;

    public override RecordType Parse(MutagenFrame reader)
    {
        return HeaderTranslation.ReadNextRecordType(reader);
    }

    public bool Parse<T>(
        MutagenFrame reader,
        out EDIDLink<T> item,
        TypedParseParams? translationParams = null)
        where T : class, IMajorRecordGetter
    {
        if (!reader.TryCheckUpcomingRead(4, out var ex))
        {
            throw ex;
        }

        item = new EDIDLink<T>(HeaderTranslation.ReadNextRecordType(reader));
        return true;
    }

    public bool Parse<T>(
        MutagenFrame reader,
        out IEDIDLinkGetter<T> item,
        TypedParseParams? translationParams = null)
        where T : class, IMajorRecordGetter
    {
        if (!reader.TryCheckUpcomingRead(4, out var ex))
        {
            throw ex;
        }

        item = new EDIDLink<T>(HeaderTranslation.ReadNextRecordType(reader));
        return true;
    }

    public override void Write(MutagenWriter writer, RecordType item)
    {
        writer.Write(item.TypeInt);
    }

    public void Write<T>(MutagenWriter writer, IEDIDLinkGetter<T> item)
        where T : class, IMajorRecordGetter
    {
        Write(
            writer,
            item.EDID);
    }

    public void Write<M, T>(
        MutagenWriter writer,
        IEDIDLinkGetter<T> item,
        RecordType header)
        where T : class, IMajorRecordGetter
    {
        this.Write(
            writer,
            item.EDID,
            header);
    }

    public void Write<T>(
        MutagenWriter writer,
        IEDIDLinkGetter<T> item,
        RecordType header)
        where T : class, IMajorRecordGetter
    {
        this.Write(
            writer,
            item.EDID,
            header);
    }
}