using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Internals
{
    public static class EnumerableExt
    {
        public static IExtendedList<T> CastExtendedList<T>(this IExtendedList<T> enumer)
        {
            return enumer;
        }

        public static IExtendedList<T> CastExtendedList<T>(this IEnumerable<T> enumer)
        {
            if (enumer is IExtendedList<T> rhs) return rhs;
            return new ExtendedList<T>(enumer);
        }

        public static IExtendedList<T>? CastExtendedListIfAny<T>(this IEnumerable<T> enumer)
        {
            if (enumer is IExtendedList<T> rhs)
            {
                if (rhs.Count == 0) return null;
                return rhs;
            }
            if (!enumer.Any()) return null;
            return new ExtendedList<T>(enumer);
        }

        public static IExtendedList<T>? CastExtendedListIfAny<T>(this IExtendedList<T> enumer)
        {
            if (enumer.Count == 0) return null;
            return enumer;
        }
    }
}
