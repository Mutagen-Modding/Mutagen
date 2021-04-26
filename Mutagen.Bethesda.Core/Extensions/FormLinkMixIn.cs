using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;

namespace Mutagen.Bethesda
{
    public static class FormLinkMixIn
    {
        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLink<TGetter> AsLink<TGetter>(this IMapsToGetter<TGetter> rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLink<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLinkGetter<TGetter> AsLinkGetter<TGetter>(this IMapsToGetter<TGetter> rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLink<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLink<TGetter> AsLink<TGetter>(this TGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLink<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLinkGetter<TGetter> AsLinkGetter<TGetter>(this TGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLink<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function needs an explicitly defined generic
        /// when calling it, as it doesn't know what link type it should convert to automatically.
        /// </summary>
        public static IFormLink<TGetter> AsLink<TGetter>(this IMajorRecordCommonGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLink<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function needs an explicitly defined generic
        /// when calling it, as it doesn't know what link type it should convert to automatically.
        /// </summary>
        public static IFormLinkGetter<TGetter> AsLinkGetter<TGetter>(this IMajorRecordCommonGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLink<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLinkNullable<TGetter> AsNullableLink<TGetter>(this IMapsToGetter<TGetter> rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLinkNullableGetter<TGetter> AsNullableLinkGetter<TGetter>(this IMapsToGetter<TGetter> rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLinkNullable<TGetter> AsNullableLink<TGetter>(this TGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function shouldn't need an explicitly defined generic
        /// when calling it.  It only works with non-abstract class types, though.
        /// </summary>
        public static IFormLinkNullableGetter<TGetter> AsNullableLinkGetter<TGetter>(this TGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function needs an explicitly defined generic
        /// when calling it, as it doesn't know what link type it should convert to automatically.
        /// </summary>
        public static IFormLinkNullable<TGetter> AsNullableLink<TGetter>(this IMajorRecordCommonGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TGetter>(rec.FormKey);
        }

        /// <summary>
        /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
        /// available.  This particular extension function needs an explicitly defined generic
        /// when calling it, as it doesn't know what link type it should convert to automatically.
        /// </summary>
        public static IFormLinkNullableGetter<TGetter> AsNullableLinkGetter<TGetter>(this IMajorRecordCommonGetter rec)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return new FormLinkNullable<TGetter>(rec.FormKey);
        }

        public static IFormLinkGetter<TGetter> AsGetter<TGetter>(this IFormLink<TGetter> link)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return link;
        }

        public static IFormLinkNullableGetter<TGetter> AsGetter<TGetter>(this IFormLinkNullable<TGetter> link)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return link;
        }

        public static bool Contains<TGetter>(this IReadOnlyCollection<IFormLinkGetter<TGetter>> coll, TGetter record)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return coll.Contains(new FormLink<TGetter>(record.FormKey));
        }

        public static bool Contains<TGetter>(this IReadOnlyCollection<FormLink<TGetter>> coll, TGetter record)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return coll.Contains(new FormLink<TGetter>(record.FormKey));
        }

        public static bool Contains<TGetter>(this IReadOnlyCollection<FormLinkNullable<TGetter>> coll, TGetter record)
            where TGetter : class, IMajorRecordCommonGetter
        {
            return coll.Contains(new FormLinkNullable<TGetter>(record.FormKey));
        }
    }
}
