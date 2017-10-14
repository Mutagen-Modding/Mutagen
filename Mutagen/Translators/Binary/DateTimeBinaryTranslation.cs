using Noggog;
using System;
using System.IO;

namespace Mutagen.Binary
{
    public class DateTimeBinaryTranslation : PrimitiveBinaryTranslation<DateTime>
    {
        public readonly static DateTimeBinaryTranslation Instance = new DateTimeBinaryTranslation();
        public override byte? ExpectedLength => 4;

        protected override DateTime ParseValue(BinaryReader reader)
        {
            reader.ReadBytes(4);
            return DateTime.Now;
        }

        protected override void WriteValue(BinaryWriter writer, DateTime item)
        {
            writer.Write(new byte[4]);
        }
    }
}
