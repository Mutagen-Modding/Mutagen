using Loqui;
using Loqui.Internal;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FormKeyBinaryTranslation
    {
        public readonly static FormKeyBinaryTranslation Instance = new FormKeyBinaryTranslation();

        public bool Parse(
            MutagenFrame frame,
            out FormKey item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            var id = frame.ReadUInt32();
            var formID = new FormID(id);
            if (masterReferences.Masters.TryGet(formID.ModID.ID, out var master))
            {
                item = new FormKey(
                    master.Master,
                    id);
                return true;
            }
            item = new FormKey(
                masterReferences.CurrentMod,
                id);
            return true;
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
        {
            UInt32BinaryTranslation.Instance.Write(
                writer: writer,
                item: item.GetFormID(masterReferences).Raw,
                errorMask: errorMask);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer,
                        item,
                        masterReferences,
                        errorMask);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferences masterReferences,
            RecordType header,
            ErrorMaskBuilder errorMask,
            bool nullable = false)
        {
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                this.Write(
                    writer,
                    item,
                    masterReferences,
                    errorMask);
            }
        }
    }
}
