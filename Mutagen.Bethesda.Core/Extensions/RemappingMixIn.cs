using System;
using System.Collections.Generic;
using System.Text;
using Noggog;

namespace Mutagen.Bethesda
{
    public static class RemappingMixIn
    {
        public static FormLink<TMajorGetter> Relink<TMajorGetter>(this FormLink<TMajorGetter> link, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!link.IsNull
                && mapping.TryGetValue(link.FormKey, out var replacement))
            {
                return new FormLink<TMajorGetter>(replacement);
            }
            return link;
        }

        public static FormLinkNullable<TMajorGetter> Relink<TMajorGetter>(this FormLinkNullable<TMajorGetter> link, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (link.FormKeyNullable.HasValue
                && !link.IsNull
                && mapping.TryGetValue(link.FormKey, out var replacement))
            {
                return new FormLinkNullable<TMajorGetter>(replacement);
            }
            return link;
        }

        public static IFormLink<TMajorGetter> Relink<TMajorGetter>(this IFormLink<TMajorGetter> link, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!link.IsNull
                && mapping.TryGetValue(link.FormKey, out var replacement))
            {
                return new FormLink<TMajorGetter>(replacement);
            }
            return link;
        }

        public static IFormLinkNullable<TMajorGetter> Relink<TMajorGetter>(this IFormLinkNullable<TMajorGetter> link, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (link.FormKeyNullable.HasValue
                && !link.IsNull
                && mapping.TryGetValue(link.FormKey, out var replacement))
            {
                return new FormLinkNullable<TMajorGetter>(replacement);
            }
            return link;
        }

        public static void RemapLinks<TMajorGetter>(this IList<IFormLink<TMajorGetter>> linkList, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            for (int i = 0; i < linkList.Count; i++)
            {
                linkList[i] = linkList[i].Relink(mapping);
            }
        }

        public static void RemapLinks<TItem>(this IList<TItem> linkList, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TItem : IFormLinkContainer
        {
            foreach (var item in linkList)
            {
                item.RemapLinks(mapping);
            }
        }

        public static void RemapLinks<TItem>(this IGenderedItemGetter<TItem?> gendered, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TItem : class, IFormLinkContainer
        {
            gendered.Male?.RemapLinks(mapping);
            gendered.Female?.RemapLinks(mapping);
        }

        public static void RemapLinks<TMajorGetter>(this IGenderedItem<IFormLink<TMajorGetter>> gendered, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            gendered.Male = gendered.Male.Relink(mapping);
            gendered.Female = gendered.Female.Relink(mapping);
        }

        public static void RemapLinks<TMajorGetter>(this IGenderedItem<IFormLinkNullable<TMajorGetter>> gendered, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            gendered.Male = gendered.Male.Relink(mapping);
            gendered.Female = gendered.Female.Relink(mapping);
        }

        public static void RemapLinks<TMajorGetter>(this IReadOnlyCache<TMajorGetter, FormKey> cache, IReadOnlyDictionary<FormKey, FormKey> mapping)
            where TMajorGetter : class, IMajorRecordCommonGetter, IFormLinkContainer
        {
            foreach (var item in cache.Items)
            {
                item.RemapLinks(mapping);
            }
        }
    }
}
