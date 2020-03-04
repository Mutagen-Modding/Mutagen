using Noggog;
using System;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class DateTimeBinaryTranslation : PrimitiveBinaryTranslation<DateTime>
    {
        public readonly static DateTimeBinaryTranslation Instance = new DateTimeBinaryTranslation();
        public override int ExpectedLength => 4;

        public override DateTime ParseValue(MutagenFrame reader)
        {
            reader.Reader.ReadBytes(4);
            return DateTime.Now;
        }

        public override void Write(MutagenWriter writer, DateTime item)
        {
            writer.Write(new byte[4]);
        }
    }
}
