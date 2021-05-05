using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Pex.Binary.Translations
{
    public class StringBinaryTranslation
    {
        public readonly static StringBinaryTranslation Instance = new StringBinaryTranslation();

        public string Parse(PexReader reader)
        {
            return reader.ReadString();
        }

        public void Write(PexWriter writer, string? str)
        {
            writer.Write(str);
        }
    }
}
