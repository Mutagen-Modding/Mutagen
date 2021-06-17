using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Translations.Binary;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public class FormKeyBinaryTranslation
    {
        public readonly static FormKeyBinaryTranslation Instance = new FormKeyBinaryTranslation();

        public FormKey Parse(
            ReadOnlySpan<byte> span,
            IMasterReferenceReader masterReferences)
        {
            var id = BinaryPrimitives.ReadUInt32LittleEndian(span);
            return FormKey.Factory(masterReferences, id);
        }

        public bool Parse(
            MutagenFrame reader,
            out FormKey item)
        {
            item = Parse(
                reader.ReadSpan(4),
                reader.MetaData.MasterReferences!);
            return true;
        }

        public FormKey Parse(MutagenFrame reader)
        {
            return Parse(
                reader.ReadSpan(4),
                reader.MetaData.MasterReferences!);
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
            UInt32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
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
