using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class OverrideMixIns
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

        /// <summary>
        /// Takes in an existing record definition, and either returns the existing override definition
        /// from the Group, or copies the given record, inserts it, and then returns it as an override.
        /// </summary>
        /// <param name="group">Group to retrieve and/or insert from</param>
        /// <param name="major">Major record to query and potententially copy</param>
        /// <returns>Existing override record, or a copy of the given record that has already been inserted into the group</returns>
        public static TMajor GetOrAddAsOverride<TMajor, TMajorGetter>(this IGroupCommon<TMajor> group, TMajorGetter major)
            where TMajor : class, IMajorRecordInternal, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter, IBinaryItem
        {
            if (group.RecordCache.TryGetValue(major.FormKey, out var existingMajor))
            {
                return existingMajor;
            }
            existingMajor = (major.DeepCopy() as TMajor)!;
            group.RecordCache.Set(existingMajor);
            return existingMajor;
        }
    }
}
