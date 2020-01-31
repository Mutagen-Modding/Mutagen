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

        public void ParseInto<T>(MutagenFrame frame, IEDIDSetLink<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(frame, ExpectedLength, out RecordType val))
            {
                item.EDID = val;
            }
            else
            {
                item.EDID = EDIDLink<T>.UNLINKED;
            }
        }

        public void ParseInto<T>(MutagenFrame frame, IEDIDLink<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(frame, ExpectedLength, out RecordType val))
            {
                item.EDID = val;
            }
            else
            {
                item.EDID = EDIDLink<T>.UNLINKED;
            }
        }

        public override RecordType ParseValue(MutagenFrame reader)
        {
            return HeaderTranslation.ReadNextRecordType(reader.Reader);
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out IEDIDLink<T> item,
            MasterReferences masterReferences = null)
            where T : class, IMajorRecordCommonGetter
        {
            if (!frame.TryCheckUpcomingRead(4, out var ex))
            {
                throw ex;
            }

            item = new EDIDLink<T>(HeaderTranslation.ReadNextRecordType(frame));
            return true;
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out IEDIDSetLink<T> item,
            MasterReferences masterReferences = null)
            where T : class, IMajorRecordCommonGetter
        {
            if (!frame.TryCheckUpcomingRead(4, out var ex))
            {
                throw ex;
            }

            item = new EDIDSetLink<T>(HeaderTranslation.ReadNextRecordType(frame));
            return true;
        }

        public override void WriteValue(MutagenWriter writer, RecordType item)
        {
            writer.Write(item.TypeInt);
        }

        public void Write<T>(MutagenWriter writer, IEDIDLinkGetter<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            this.WriteValue(
                writer,
                item.EDID);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDSetLinkGetter<T> item)
            where T : class, IMajorRecordCommonGetter
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            IEDIDLinkGetter<T> item,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            this.Write(
                writer,
                item.EDID,
                header,
                nullable: nullable);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDSetLinkGetter<T> item,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.EDID,
                header,
                nullable: nullable);
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
                header,
                nullable);
        }

        public void Write<T>(
            MutagenWriter writer,
            IEDIDLinkGetter<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordCommonGetter
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt);
        }
    }
}
