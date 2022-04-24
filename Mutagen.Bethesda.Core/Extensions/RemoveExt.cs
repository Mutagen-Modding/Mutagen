using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda;

internal static class MutagenRemoveExt
{
    public static void Remove<TItem>(this IExtendedList<TItem>? enumer, HashSet<FormKey> removeSet)
        where TItem : IMajorRecordGetter
    {
        enumer.RemoveWhere((i) => removeSet.Contains(i.FormKey));
    }
}