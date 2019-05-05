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
            MasterReferences masterReferences)
        {
            var id = frame.ReadUInt32();
            var formID = new FormID(id);
            if (formID.ModID.ID < masterReferences.Masters.Count)
            {
                item = new FormKey(
                    masterReferences.Masters[formID.ModID.ID].Master,
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
            bool nullable = false)
        {
            UInt32BinaryTranslation.Instance.Write(
                writer: writer,
                item: item.GetFormID(masterReferences).Raw);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferences masterReferences,
            RecordType header,
            bool nullable = false)
        {
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                this.Write(
                    writer,
                    item,
                    masterReferences);
            }
        }
    }
}
