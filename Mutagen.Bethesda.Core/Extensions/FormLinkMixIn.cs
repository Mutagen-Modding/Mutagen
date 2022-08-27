using Loqui;
using Loqui.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda;

public static class FormLinkMixIn
{
    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToLink instead")]
    public static IFormLink<TGetter> AsLink<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }
    
    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLink<TGetter> ToLink<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToLinkGetter instead")]
    public static IFormLinkGetter<TGetter> AsLinkGetter<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkGetter<TGetter> ToLinkGetter<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToLink instead")]
    public static IFormLink<TGetter> AsLink<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLink<TGetter> ToLink<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToLinkGetter instead")]
    public static IFormLinkGetter<TGetter> AsLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkGetter<TGetter> ToLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    [Obsolete("Use ToLink instead")]
    public static IFormLink<TGetter> AsLink<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    public static IFormLink<TGetter> ToLink<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Checks the record type at runtime and returns a FormLink of the Getter interface based
    /// on the concrete type of the record given.
    /// </summary>
    public static IFormLinkGetter ToLinkFromRuntimeType<T>(this T rec)
        where T : IMajorRecordGetter, ILoquiObject
    {
        return new FormLinkInformation(rec.FormKey, rec.Registration.GetterType);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    [Obsolete("Use ToLinkGetter instead")]
    public static IFormLinkGetter<TGetter> AsLinkGetter<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    public static IFormLinkGetter<TGetter> ToLinkGetter<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLink<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToNullableLink instead")]
    public static IFormLinkNullable<TGetter> AsNullableLink<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkNullable<TGetter> ToNullableLink<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToNullableLinkGetter instead")]
    public static IFormLinkNullableGetter<TGetter> AsNullableLinkGetter<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkNullableGetter<TGetter> ToNullableLinkGetter<TSetter, TGetter>(this TSetter rec)
        where TGetter : class, IMajorRecordGetter
        where TSetter : IMapsToGetter<TGetter>, TGetter, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToNullableLink instead")]
    public static IFormLinkNullable<TGetter> AsNullableLink<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkNullable<TGetter> ToNullableLink<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    [Obsolete("Use ToNullableLinkGetter instead")]
    public static IFormLinkNullableGetter<TGetter> AsNullableLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkNullableGetter<TGetter> ToNullableLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    [Obsolete("Use ToNullableLink instead")]
    public static IFormLinkNullable<TGetter> AsNullableLink<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    public static IFormLinkNullable<TGetter> ToNullableLink<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    [Obsolete("Use ToNullableLinkGetter instead")]
    public static IFormLinkNullableGetter<TGetter> AsNullableLinkGetter<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    public static IFormLinkNullableGetter<TGetter> ToNullableLinkGetter<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TGetter>(rec.FormKey);
    }

    public static IFormLinkGetter<TGetter> AsGetter<TGetter>(this IFormLink<TGetter> link)
        where TGetter : class, IMajorRecordGetter
    {
        return link;
    }

    public static IFormLinkNullableGetter<TGetter> AsGetter<TGetter>(this IFormLinkNullable<TGetter> link)
        where TGetter : class, IMajorRecordGetter
    {
        return link;
    }

    public static bool Contains<TGetter>(this IReadOnlyCollection<IFormLinkGetter<TGetter>> coll, TGetter record)
        where TGetter : class, IMajorRecordGetter
    {
        return coll.Contains(new FormLink<TGetter>(record.FormKey));
    }

    public static bool Contains<TGetter>(this IReadOnlyCollection<FormLink<TGetter>> coll, TGetter record)
        where TGetter : class, IMajorRecordGetter
    {
        return coll.Contains(new FormLink<TGetter>(record.FormKey));
    }

    public static bool Contains<TGetter>(this IReadOnlyCollection<FormLinkNullable<TGetter>> coll, TGetter record)
        where TGetter : class, IMajorRecordGetter
    {
        return coll.Contains(new FormLinkNullable<TGetter>(record.FormKey));
    }
}