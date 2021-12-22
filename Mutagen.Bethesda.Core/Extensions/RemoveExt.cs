using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Internals
{
    public static class MutagenRemoveExt
    {
        public static void Remove<TItem>(this IExtendedList<TItem>? enumer, HashSet<FormKey> removeSet)
            where TItem : IMajorRecordGetter
        {
            enumer.RemoveWhere((i) => removeSet.Contains(i.FormKey));
        }
    }
}
