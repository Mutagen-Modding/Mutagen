using Loqui.Internal;
using Loqui.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public class RecordTypeXmlTranslation : PrimitiveXmlTranslation<RecordType>
    {
        public readonly static RecordTypeXmlTranslation Instance = new RecordTypeXmlTranslation();
         
        public void ParseInto<T>(XElement node, int fieldIndex, IEDIDLink<T> item, ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(node, out RecordType val, errorMask))
                    {
                        item.EDID = val;
                    }
                    else
                    {
                        item.EDID = EDIDLink<T>.Null;
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void ParseInto<T>(XElement node, int fieldIndex, IEDIDSetLink<T> item, ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(node, out RecordType val, errorMask))
                    {
                        item.EDID = val;
                    }
                    else
                    {
                        item.EDID = EDIDLink<T>.Null;
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public bool Parse<T>(
            XElement node, 
            out IEDIDLink<T> item,
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
            out IEDIDLink<T> item,
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
            out IEDIDSetLink<T> item,
            ErrorMaskBuilder? errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out RecordType id, errorMask))
            {
                item = new EDIDSetLink<T>(id);
                return true;
            }
            item = new EDIDSetLink<T>();
            return false;
        }

        public bool Parse<T>(
            XElement node, 
            out IEDIDSetLink<T> item, 
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
