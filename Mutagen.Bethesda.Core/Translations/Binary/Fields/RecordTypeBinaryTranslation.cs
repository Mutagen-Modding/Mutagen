using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
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
            MutagenFrame frame,
            out IEDIDLink<T> item,
            MasterReferenceReader? masterReferences = null,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class, IMajorRecordCommonGetter
        {
            if (!frame.TryCheckUpcomingRead(4, out var ex))
            {
                throw ex;
            }

            item = new EDIDLink<T>(HeaderTranslation.ReadNextRecordType(frame));
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
            RecordType header,
            bool nullable)
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
            MasterReferenceReader masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt);
        }
    }
}
