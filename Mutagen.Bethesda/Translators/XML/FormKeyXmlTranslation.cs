using Loqui.Internal;
using Loqui.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public class FormKeyXmlTranslation : TypicalXmlTranslation<FormKey>
    {
        public readonly static FormKeyXmlTranslation Instance = new FormKeyXmlTranslation();

        public bool Parse<T>(
            XElement node,
            out IFormLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public bool Parse<T>(
            XElement node,
            out IFormLink<T> item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public bool Parse<T>(
            XElement node,
            out IFormSetLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public bool Parse<T>(
            XElement node,
            out IFormSetLink<T> item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        protected override FormKey Parse(string str)
        {
            throw new NotImplementedException();
        }

        public FormKey Parse(XElement node, ErrorMaskBuilder? errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
