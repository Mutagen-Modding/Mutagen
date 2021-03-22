using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    class FormLinkTypelessComparer<TMajorGetter> : IEqualityComparer<IFormLinkGetter<TMajorGetter>>
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        public static readonly FormLinkTypelessComparer<TMajorGetter> Instance = new FormLinkTypelessComparer<TMajorGetter>();

        public bool Equals(IFormLinkGetter<TMajorGetter>? x, IFormLinkGetter<TMajorGetter>? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.FormKey == y.FormKey;
        }

        public int GetHashCode(IFormLinkGetter<TMajorGetter> obj)
        {
            return obj.GetHashCode();
        }
    }
}
