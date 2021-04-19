using Mutagen.Bethesda.Internals;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Binary
{
    public class FormKeyBinaryTranslation
    {
        public readonly static FormKeyBinaryTranslation Instance = new FormKeyBinaryTranslation();

        public FormKey Parse(
            ReadOnlySpan<byte> span,
            MasterReferenceReader masterReferences)
        {
            var id = BinaryPrimitives.ReadUInt32LittleEndian(span);
            return FormKey.Factory(masterReferences, id);
        }

        public bool Parse(
            MutagenFrame frame,
            out FormKey item)
        {
            item = Parse(
                frame.ReadSpan(4),
                frame.MetaData.MasterReferences!);
            return true;
        }

        public FormKey Parse(MutagenFrame frame)
        {
            return Parse(
                frame.ReadSpan(4),
                frame.MetaData.MasterReferences!);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            bool nullable = false)
        {
            if (writer.MetaData.CleanNulls && item.IsNull)
            {
                item = FormKey.Null;
            }
            UInt32BinaryTranslation.Instance.Write(
                writer: writer,
                item: writer.MetaData.MasterReferences!.GetFormID(item).Raw);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            RecordType header,
            bool nullable = false)
        {
            try
            {
                using (HeaderExport.Header(writer, header, ObjectType.Subrecord))
                {
                    this.Write(
                        writer,
                        item);
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, header);
            }
        }
    }
}
