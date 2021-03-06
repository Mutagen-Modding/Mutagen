using Loqui.Internal;
using Loqui.Xml;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Plugins.Xml
{
    public class FormKeyXmlTranslation : TypicalXmlTranslation<FormKey>
    {
        public readonly static FormKeyXmlTranslation Instance = new FormKeyXmlTranslation();

        public bool Parse<T>(
            XElement node,
            out FormLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            throw new NotImplementedException();
        }

        public bool Parse<T>(
            XElement node,
            out FormLink<T> item,
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

        public void Write(XElement node, string? name, FormKey? item, int fieldIndex, ErrorMaskBuilder? errorMask)
        {
            if (!item.HasValue) return;
            Write(node, name, item.Value, fieldIndex, errorMask);
        }

        public void Write(XElement node, string? name, FormKey? item, ErrorMaskBuilder? errorMask)
        {
            if (!item.HasValue) return;
            Write(node, name, item.Value, errorMask);
        }
    }
}
