using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public record FormLinkInformation(FormKey FormKey, Type Type)
    {
        public override string ToString()
        {
            return $"({Type}) => {FormKey}";
        }

        public static FormLinkInformation Factory<TMajorGetter>(IFormLinkGetter<TMajorGetter> link)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkInformation(link.FormKey, typeof(TMajorGetter));
        }

        public static FormLinkInformation Factory<TMajorGetter>(IFormLinkNullableGetter<TMajorGetter> link)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkInformation(link.FormKey, typeof(TMajorGetter));
        }

        public static FormLinkInformation Factory(IMajorRecordCommonGetter majorRec)
        {
            return new FormLinkInformation(majorRec.FormKey, majorRec.Registration.GetterType);
        }

        public static FormLinkInformation Factory(FormLinkInformation rhs) => rhs;
    }
}
