using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class ItpoMixIns
    {
        public static void RemoveIdentical<TMod>(this TMod mod, ILinkCache linkCache)
            where TMod : class, IMod
        {
            var toRemove = new Dictionary<Type, HashSet<FormKey>>();
            foreach (var major in mod.EnumerateMajorRecords())
            {
                var form = major.FormKey;
                var type = major.GetType();
                if (!linkCache.TryLookup(form, type, out var prevRec)) continue;
                if (prevRec.Equals(major))
                {
                    toRemove.GetOrAdd(type).Add(form);
                }
            }
            foreach (var type in toRemove)
            {
                mod.Remove(type.Value, type.Key);
            }
        }
    }
}
