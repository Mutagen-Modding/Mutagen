using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class RawFormIDBinaryTranslation : PrimitiveBinaryTranslation<RawFormID>
    {
        public readonly static RawFormIDBinaryTranslation Instance = new RawFormIDBinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(4);

        protected override RawFormID ParseValue(MutagenFrame reader)
        {
            return RawFormID.Factory(reader.Reader.ReadBytes(ExpectedLength.Value));
        }

        protected override void WriteValue(MutagenWriter writer, RawFormID item)
        {
            writer.Write(item.ToBytes());
        }
    }
}
