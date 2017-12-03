using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FormIDBinaryTranslation : PrimitiveBinaryTranslation<FormID>
    {
        public readonly static FormIDBinaryTranslation Instance = new FormIDBinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(4);

        protected override FormID ParseValue(MutagenFrame reader)
        {
            return FormID.Factory(reader.Reader.ReadBytes(ExpectedLength.Value));
        }

        protected override void WriteValue(MutagenWriter writer, FormID item)
        {
            writer.Write(item.ToBytes());
        }
    }
}
