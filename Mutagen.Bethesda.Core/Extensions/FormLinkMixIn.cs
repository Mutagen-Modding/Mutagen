using System.Collections.Concurrent;
using Loqui;
using Loqui.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

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
        return ToLinkGetter<TGetter>(rec);
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
        return ToLinkGetter<TGetter>(rec);
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
        return ToLink<TGetter>(rec);
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
    /// when calling it.
    /// </summary>
    [Obsolete("Use ToLinkGetter instead")]
    public static IFormLinkGetter<TGetter> AsLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return ToLinkGetter<TGetter>(rec);
    }
    
    private static ConcurrentDictionary<Type, Func<FormKey, IFormLinkGetter>> _getterLinkFactoryCache = new();

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.
    /// </summary>
    public static IFormLinkGetter<TGetter> ToLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return ToLinkGetterPrivate<TGetter>(rec);
    }
    
    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.
    /// </summary>
    private static IFormLinkGetter<TGetter> ToLinkGetterPrivate<TGetter>(IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        if (_getterLinkFactoryCache.TryGetValue(typeof(TGetter), out var getter))
        {
            return (IFormLinkGetter<TGetter>)getter(rec.FormKey);
        }

        if (!GetterTypeMapping.Instance.TryGetGetterType(rec.GetType(), out var getterType))
        {
            throw new ArgumentException($"Could not find getter type for {rec.GetType()}");
        }

        Func<FormKey, IFormLinkGetter> factory;
        if (!getterType.InheritsFrom(typeof(TGetter)))
        {
            factory = new Func<FormKey, IFormLinkGetter>(formKey =>
            {
                return new FormLinkGetter<TGetter>(formKey);
            });   
        }
        else
        {
            var genericType = typeof(FormLinkGetter<>).MakeGenericType(getterType);
            if (genericType == typeof(FormLinkGetter<TGetter>))
            {
                factory = new Func<FormKey, IFormLinkGetter>(formKey =>
                {
                    return new FormLinkGetter<TGetter>(formKey);
                });   
            }
            else
            {
                factory = new Func<FormKey, IFormLinkGetter>(formKey =>
                {
                    return (IFormLinkGetter)Activator.CreateInstance(genericType, formKey)!;
                });
            }
        }

        _getterLinkFactoryCache[typeof(TGetter)] = factory;
        var ret = factory(rec.FormKey);
        return (IFormLinkGetter<TGetter>)ret;
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
        return ToLink<TGetter>(rec);
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
        return ToLinkGetter<TGetter>(rec);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    public static IFormLinkGetter<TGetter> ToLinkGetter<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return ToLinkGetterPrivate<TGetter>(rec);
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
        return ToNullableLink<TSetter, TGetter>(rec);
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
        return ToNullableLinkGetter<TGetter>(rec);
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
        return ToNullableLinkGetter<TGetter>(rec);
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
        return ToNullableLink<TGetter>(rec);
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
        return ToNullableLinkGetter<TGetter>(rec);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    public static IFormLinkNullableGetter<TGetter> ToNullableLinkGetter<TGetter>(this TGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return ToNullableLinkGetterPrivate<TGetter>(rec);
    }

    private static ConcurrentDictionary<Type, Func<FormKey, IFormLinkGetter>> _nullableGetterLinkFactoryCache = new();

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function shouldn't need an explicitly defined generic
    /// when calling it.  It only works with non-abstract class types, though.
    /// </summary>
    private static IFormLinkNullableGetter<TGetter> ToNullableLinkGetterPrivate<TGetter>(IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        if (_nullableGetterLinkFactoryCache.TryGetValue(typeof(TGetter), out var getter))
        {
            return (IFormLinkNullableGetter<TGetter>)getter(rec.FormKey);
        }

        if (!GetterTypeMapping.Instance.TryGetGetterType(rec.GetType(), out var getterType))
        {
            throw new ArgumentException($"Could not find getter type for {rec.GetType()}");
        }

        Func<FormKey, IFormLinkGetter> factory;
        if (!getterType.InheritsFrom(typeof(TGetter)))
        {
            factory = new Func<FormKey, IFormLinkGetter>(formKey =>
            {
                return new FormLinkNullableGetter<TGetter>(formKey);
            });   
        }
        else
        {
            var genericType = typeof(FormLinkNullableGetter<>).MakeGenericType(getterType);
            if (genericType == typeof(FormLinkNullableGetter<TGetter>))
            {
                factory = new Func<FormKey, IFormLinkGetter>(formKey =>
                {
                    return new FormLinkNullableGetter<TGetter>(formKey);
                });   
            }
            else
            {
                factory = new Func<FormKey, IFormLinkGetter>(formKey =>
                {
                    return (IFormLinkGetter)Activator.CreateInstance(genericType, formKey)!;
                });
            }
        }

        _nullableGetterLinkFactoryCache[typeof(TGetter)] = factory;
        var ret = factory(rec.FormKey);
        return (IFormLinkNullableGetter<TGetter>)ret;
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
        return ToNullableLink<TGetter>(rec);
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
        return ToNullableLinkGetter<TGetter>(rec);
    }

    /// <summary>
    /// Mix in to facilitate converting to FormLinks from interfaces where implicit operators aren't
    /// available.  This particular extension function needs an explicitly defined generic
    /// when calling it, as it doesn't know what link type it should convert to automatically.
    /// </summary>
    public static IFormLinkNullableGetter<TGetter> ToNullableLinkGetter<TGetter>(this IMajorRecordGetter rec)
        where TGetter : class, IMajorRecordGetter
    {
        return ToNullableLinkGetterPrivate<TGetter>(rec);
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

    [Obsolete("Use ToNullableLink instead")]
    public static FormLinkNullable<TMajorGetter> AsNullableLink<TMajorGetter>(this FormKey formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    public static FormLinkNullable<TMajorGetter> ToNullableLink<TMajorGetter>(this FormKey formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    [Obsolete("Use ToNullableLinkGetter instead")]
    public static IFormLinkNullableGetter<TMajorGetter> AsNullableLinkGetter<TMajorGetter>(this FormKey formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    public static IFormLinkNullableGetter<TMajorGetter> ToNullableLinkGetter<TMajorGetter>(this FormKey formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    [Obsolete("Use ToNullableLink instead")]
    public static FormLinkNullable<TMajorGetter> AsNullableLink<TMajorGetter>(this FormKey? formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    public static FormLinkNullable<TMajorGetter> ToNullableLink<TMajorGetter>(this FormKey? formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    [Obsolete("Use ToNullableLinkGetter instead")]
    public static IFormLinkNullableGetter<TMajorGetter> AsNullableLinkGetter<TMajorGetter>(this FormKey? formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
    }

    public static IFormLinkNullableGetter<TMajorGetter> ToNullableLinkGetter<TMajorGetter>(this FormKey? formKey)
        where TMajorGetter : class, IMajorRecordGetter
    {
        return new FormLinkNullable<TMajorGetter>(formKey);
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