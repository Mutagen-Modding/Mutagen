using Loqui.Internal;
using Loqui.Xml;
using Noggog.Notifying;
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

        public void ParseInto<T>(XElement node, int fieldIndex, IFormIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(node, out FormKey val, errorMask))
                    {
                        item.Set(val);
                    }
                    else
                    {
                        item.Unset();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void ParseInto<T>(XElement node, int fieldIndex, IFormIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(node, out FormKey val, errorMask))
                    {
                        item.Set(val);
                    }
                    else
                    {
                        item.Unset();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void ParseInto<T>(XElement node, int fieldIndex, IEDIDLink<T> item, ErrorMaskBuilder errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(node, out FormKey val, errorMask))
                    {
                        item.Set(val);
                    }
                    else
                    {
                        item.Unset();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void ParseInto<T>(XElement node, int fieldIndex, IEDIDSetLink<T> item, ErrorMaskBuilder errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(node, out FormKey val, errorMask))
                    {
                        item.Set(val);
                    }
                    else
                    {
                        item.Unset();
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
            out IFormIDLink<T> item, 
            ErrorMaskBuilder errorMask)
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
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask)
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
            ErrorMaskBuilder errorMask)
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
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            return this.Parse(
                node: node,
                item: out item,
                errorMask: errorMask);
        }

        public bool Parse<T>(
            XElement node, 
            out IEDIDLink<T> item,
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out FormKey id, errorMask))
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
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask)
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
            ErrorMaskBuilder errorMask)
            where T : class, IMajorRecordCommonGetter
        {
            if (Parse(node, out FormKey id, errorMask))
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
            ErrorMaskBuilder errorMask,
            TranslationCrystal translationMask)
            where T : class, IMajorRecordCommonGetter
        {
            return this.Parse(
                node: node,
                item: out item,
                errorMask: errorMask);
        }

        protected override bool ParseNonNullString(string str, out FormKey value, ErrorMaskBuilder errorMask)
        {
            if (FormKey.TryFactory(str, out FormKey parsed))
            {
                value = parsed;
                return true;
            }
            errorMask.ReportExceptionOrThrow(
                new ArgumentException($"Could not convert to {NullableName}: {str}"));
            value = FormKey.NULL;
            return false;
        }
    }
}
