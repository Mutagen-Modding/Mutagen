using Loqui.Internal;
using Mutagen.Bethesda.Strings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Xml
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
