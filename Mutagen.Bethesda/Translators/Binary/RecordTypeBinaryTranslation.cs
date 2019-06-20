using Loqui;
using Loqui.Internal;
using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class RecordTypeBinaryTranslation : PrimitiveBinaryTranslation<RecordType>
    {
        public readonly static RecordTypeBinaryTranslation Instance = new RecordTypeBinaryTranslation();
        public override int ExpectedLength => 4;

        public void ParseInto<T>(MutagenFrame frame, EDIDSetLink<T> item)
            where T : class, IMajorRecordInternalGetter
        {
            if (Parse(frame, ExpectedLength, out RecordType val))
            {
                item.Set(val);
            }
            else
            {
                item.Unset();
            }
        }

        public void ParseInto<T>(MutagenFrame frame, EDIDLink<T> item)
            where T : class, IMajorRecordInternalGetter
        {
            if (Parse(frame, ExpectedLength, out RecordType val))
            {
                item.Set(val);
            }
            else
            {
                item.Unset();
            }
        }

        public override RecordType ParseValue(MutagenFrame reader)
        {
            return HeaderTranslation.ReadNextRecordType(reader.Reader);
        }

        public bool Parse<T>(
            MutagenFrame frame,
            out EDIDLink<T> item,
            MasterReferences masterReferences)
            where T : class, IMajorRecordInternalGetter
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
            out EDIDSetLink<T> item,
            MasterReferences masterReferences)
            where T : class, IMajorRecordInternalGetter
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

        public void Write<T>(MutagenWriter writer, IEDIDLink<T> item)
            where T : class, IMajorRecordInternalGetter
        {
            this.WriteValue(
                writer,
                item.Linked ? item.EDID : EDIDLink<T>.UNLINKED);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item)
            where T : class, IMajorRecordInternalGetter
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item);
        }

        public void Write<M, T>(
            MutagenWriter writer,
            IEDIDLink<T> item,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            this.Write(
                writer,
                item.EDID,
                header,
                nullable: nullable);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDSetLink<T> item,
            RecordType header,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
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
            IEDIDLink<T> item,
            RecordType header,
            bool nullable)
            where T : class, IMajorRecordInternalGetter
        {
            this.Write(
                writer,
                item.EDID,
                header,
                nullable);
        }

        public void Write<T>(
            MutagenWriter writer,
            EDIDLink<T> item,
            MasterReferences masterReferences,
            bool nullable = false)
            where T : class, IMajorRecordInternalGetter
        {
            Int32BinaryTranslation.Instance.Write(
                writer,
                item.EDID.TypeInt);
        }
    }
}
