using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class FormLinkExt
    {
        public static FormLink<TRet> ToGetter<TMajor, TRet>(this FormLink<TMajor> link)
            where TMajor : class, IMajorRecordCommonGetter, TRet
            where TRet : class, IMajorRecordCommonGetter
        {
            return new FormLink<TRet>(link.FormKey);
        }

        public static FormLinkNullable<TRet> ToGetter<TMajor, TRet>(this FormLinkNullable<TMajor> link)
            where TMajor : class, IMajorRecordCommonGetter, TRet
            where TRet : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TRet>(link.FormKey);
        }

        public static EDIDLink<TRet> ToGetter<TMajor, TRet>(this EDIDLink<TMajor> link)
            where TMajor : class, IMajorRecordCommonGetter, TRet
            where TRet : class, IMajorRecordCommonGetter
        {
            return new EDIDLink<TRet>(link.EDID);
        }
    }
}
