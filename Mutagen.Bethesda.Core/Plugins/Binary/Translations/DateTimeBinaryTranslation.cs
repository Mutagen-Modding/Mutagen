using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;
using System;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public class DateTimeBinaryTranslation : PrimitiveBinaryTranslation<DateTime, MutagenFrame, MutagenWriter>
    {
        public readonly static DateTimeBinaryTranslation Instance = new DateTimeBinaryTranslation();
        public override int ExpectedLength => 4;

        public override DateTime Parse(MutagenFrame reader)
        {
            reader.Position += 4;
            return DateTime.Now;
        }

        public override void Write(MutagenWriter writer, DateTime item)
        {
            writer.Write(new byte[4]);
        }
    }
}
