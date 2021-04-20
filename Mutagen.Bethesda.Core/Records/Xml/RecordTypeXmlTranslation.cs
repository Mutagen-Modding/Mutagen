using Loqui.Internal;
using Loqui.Xml;
using System;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Records.Xml
{
    public class RecordTypeXmlTranslation : PrimitiveXmlTranslation<RecordType>
    {
        public readonly static RecordTypeXmlTranslation Instance = new RecordTypeXmlTranslation();

        public bool Parse<T>(
            XElement node,
            out EDIDLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out RecordType id, errorMask))
            {
                item = new EDIDLink<T>(id);
                return true;
            }
            item = new EDIDLink<T>();
            return false;
        }

        public bool Parse<T>(
            XElement node,
            out IEDIDLinkGetter<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out RecordType id, errorMask))
            {
                item = new EDIDLink<T>(id);
                return true;
            }
            item = new EDIDLink<T>();
            return false;
        }

        public bool Parse<T>(
            XElement node,
            out EDIDLink<T> item,
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
            out IEDIDLinkGetter<T> item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            return this.Parse(
                node: node,
                item: out item,
                errorMask: errorMask);
        }

        protected override bool Parse(string str, out RecordType value, ErrorMaskBuilder? errorMask)
        {
            if (RecordType.TryFactory(str, out RecordType parsed))
            {
                value = parsed;
                return true;
            }
            errorMask.ReportExceptionOrThrow(
                new ArgumentException($"Could not convert to {ElementName}: {str}"));
            value = default;
            return false;
        }
    }
}
