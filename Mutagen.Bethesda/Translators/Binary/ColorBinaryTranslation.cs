using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mutagen.Bethesda.Binary
{
    public class ColorBinaryTranslation : PrimitiveBinaryTranslation<Color>
    {
        public readonly static ColorBinaryTranslation Instance = new ColorBinaryTranslation();
        public override ContentLength? ExpectedLength => new ContentLength(4);

        protected override Color ParseValue(MutagenFrame reader)
        {
            return reader.Reader.ReadColor();
        }

        protected override void WriteValue(MutagenWriter writer, Color item)
        {
            writer.Write(item);
        }
    }
}
