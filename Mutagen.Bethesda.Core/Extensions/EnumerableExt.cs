using Noggog;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda;

internal static class EnumerableExt
{
    public static ExtendedList<T> CastExtendedList<T>(this ExtendedList<T> enumer)
    {
        return enumer;
    }

    public static ExtendedList<T> CastExtendedList<T>(this IEnumerable<T> enumer)
    {
        if (enumer is ExtendedList<T> rhs) return rhs;
        return new ExtendedList<T>(enumer);
    }

    public static ExtendedList<T>? CastExtendedListIfAny<T>(this IEnumerable<T> enumer)
    {
        if (enumer is ExtendedList<T> rhs)
        {
            if (rhs.Count == 0) return null;
            return rhs;
        }
        if (!enumer.Any()) return null;
        return new ExtendedList<T>(enumer);
    }

    public static ExtendedList<T>? CastExtendedListIfAny<T>(this ExtendedList<T> enumer)
    {
        if (enumer.Count == 0) return null;
        return enumer;
    }
}