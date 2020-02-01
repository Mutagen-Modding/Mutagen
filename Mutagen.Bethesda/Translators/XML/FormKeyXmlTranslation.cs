using Loqui.Internal;
using Loqui.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public class FormKeyXmlTranslation : PrimitiveXmlTranslation<FormKey>
    {
        public readonly static FormKeyXmlTranslation Instance = new FormKeyXmlTranslation();

        public bool Parse<T>(
            XElement node,
            out IFormIDLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out FormKey id, errorMask))
            {
                item = new FormIDLink<T>(id);
                return true;
            }
            item = new FormIDLink<T>();
            return false;
        }

        public bool Parse<T>(
            XElement node,
            out IFormIDLink<T> item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            return this.Parse(
                node: node,
                item: out item,
                errorMask: errorMask);
        }

        public bool Parse<T>(
            XElement node,
            out IFormIDSetLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out FormKey id, errorMask))
            {
                item = new FormIDSetLink<T>(id);
                return true;
            }
            item = new FormIDSetLink<T>();
            return false;
        }

        public bool Parse<T>(
            XElement node,
            out IFormIDSetLink<T> item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            return this.Parse(
                node: node,
                item: out item,
                errorMask: errorMask);
        }

        protected override bool Parse(string str, out FormKey value, ErrorMaskBuilder? errorMask)
        {
            if (FormKey.TryFactory(str, out FormKey parsed))
            {
                value = parsed;
                return true;
            }
            errorMask.ReportExceptionOrThrow(
                new ArgumentException($"Could not convert to {ElementName}: {str}"));
            value = FormKey.NULL;
            return false;
        }
    }
}
