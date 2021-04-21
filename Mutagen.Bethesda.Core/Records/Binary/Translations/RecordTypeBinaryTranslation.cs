using Mutagen.Bethesda.Records.Binary.Streams;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class RecordTypeBinaryTranslation : PrimitiveBinaryTranslation<RecordType>
    {
        public readonly static RecordTypeBinaryTranslation Instance = new RecordTypeBinaryTranslation();
        public override int ExpectedLength => 4;

        public override RecordType ParseValue(MutagenFrame reader)
        {
            return HeaderTranslation.ReadNextRecordType(reader.Reader);
        }

        public bool Parse<T>(
            MutagenFrame reader,
            out EDIDLink<T> item,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
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
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
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
            where T : class, IMajorRecordCommonGetter
        {
            this.Write(
                writer,
                item.EDID);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            IEDIDLinkGetter<T> item,
            RecordType header)
            where T : class, IMajorRecordCommonGetter
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
            where T : class, IMajorRecordCommonGetter
        {
            this.Write(
                writer,
                item.EDID,
                header);
        }
    }
}
