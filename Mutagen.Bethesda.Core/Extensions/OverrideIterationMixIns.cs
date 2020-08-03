using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class OverrideIterationMixIns
    {
        public static IEnumerable<TMajor> WinningOverrides<TMajor, TMod>(this LoadOrder<TMod> loadOrder)
            where TMod : class, IModGetter
            where TMajor : class, IMajorRecordCommonGetter
        {
            return loadOrder.PriorityOrder.WinningOverrides<TMajor>();
        }
        
        public static IEnumerable<IMajorRecordCommonGetter> WinningOverrides<TMod>(this LoadOrder<TMod> loadOrder, Type type)
            where TMod : class, IModGetter
        {
            return loadOrder.PriorityOrder.WinningOverrides(type);
        }

        public static IEnumerable<TMajor> WinningOverrides<TMajor>(this IEnumerable<IModListing<IModGetter>> priorityOrder)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return priorityOrder
                .Select(l => l.Mod)
                .NotNull()
                .WinningOverrides<TMajor>();
        }

        public static IEnumerable<IMajorRecordCommonGetter> WinningOverrides(this IEnumerable<IModListing<IModGetter>> priorityOrder, Type type)
        {
            return priorityOrder
                .Select(l => l.Mod)
                .NotNull()
                .WinningOverrides(type);
        }

        public static IEnumerable<TMajor> WinningOverrides<TMajor>(this IEnumerable<IModGetter> priorityOrder)
            where TMajor : class, IMajorRecordCommonGetter
        {
            var passedRecords = new HashSet<FormKey>();
            foreach (var mod in priorityOrder)
            {
                foreach (var record in mod.EnumerateMajorRecords<TMajor>())
                {
                    if (!passedRecords.Add(record.FormKey)) continue;
                    yield return record;
                }
            }
        }

        public static IEnumerable<IMajorRecordCommonGetter> WinningOverrides(this IEnumerable<IModGetter> priorityOrder, Type type)
        {
            var passedRecords = new HashSet<FormKey>();
            foreach (var mod in priorityOrder)
            {
                foreach (var record in mod.EnumerateMajorRecords(type))
                {
                    if (!passedRecords.Add(record.FormKey)) continue;
                    yield return record;
                }
            }
        }
    }
}
