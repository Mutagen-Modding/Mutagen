using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class NullableExt
    {
        public static bool TryGet<T>(this T? source, [MaybeNullWhen(false)]out T item)
            where T : class
        {
            if (source == null)
            {
                item = default!;
                return false;
            }
            item = source;
            return true;
        }

        public static bool TryGet<T>(this Nullable<T> source, [MaybeNullWhen(false)]out T item)
            where T : struct
        {
            if (source == null)
            {
                item = default!;
                return false;
            }
            item = source.Value;
            return true;
        }
    }
}
