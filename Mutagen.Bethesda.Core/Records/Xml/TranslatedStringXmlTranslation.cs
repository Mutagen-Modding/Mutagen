using Loqui.Internal;
using Mutagen.Bethesda.Strings;
using System;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Records.Xml
{
    public class TranslatedStringXmlTranslation
    {
        public readonly static TranslatedStringXmlTranslation Instance = new TranslatedStringXmlTranslation();

        public void Write(
            XElement node,
            string name,
            TranslatedString item,
            int fieldIndex,
            ErrorMaskBuilder? errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
