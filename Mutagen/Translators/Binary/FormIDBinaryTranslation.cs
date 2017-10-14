using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class FormIDBinaryTranslation : PrimitiveBinaryTranslation<FormID>
    {
        public readonly static FormIDBinaryTranslation Instance = new FormIDBinaryTranslation();
        public override byte? ExpectedLength => 4;

        protected override FormID ParseValue(BinaryReader reader)
        {
            return FormID.Factory(reader.ReadBytes(ExpectedLength.Value));
        }

        protected override void WriteValue(BinaryWriter writer, FormID item)
        {
            writer.Write(item.ToBytes());
        }
    }
}
