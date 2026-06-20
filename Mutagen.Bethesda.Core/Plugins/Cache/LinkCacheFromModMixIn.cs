using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache;

/// <summary>
/// Extension methods that resolve records against the contribution of one specific mod within a cache,
/// located by its ModKey.<br/>
/// <br/>
/// Unlike <see cref="ResolveTarget.Winner"/> or <see cref="ResolveTarget.Origin"/> resolution, these
/// methods do not search the load order for the winning override or the origin definition.  They instead
/// scope resolution to the record as it exists within the single mod matching the given ModKey.<br/>
/// <br/>
/// If the cache does not contain a mod matching the given ModKey, the Try variants return false and the
/// throwing variants throw a <see cref="MissingRecordException"/>.
/// </summary>
public static class LinkCacheFromModMixIn
{
    #region TryResolveFromMod (Generic)

    /// <summary>
    /// Retrieves the record that matches the FormKey within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    public static bool TryResolveFromMod<TMajor>(this ILinkCache cache, FormKey formKey, ModKey modKey, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolve(formKey, out majorRec);
    }

    /// <summary>
    /// Retrieves the record that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    public static bool TryResolveFromMod<TMajor>(this ILinkCache cache, IFormLinkGetter<TMajor> formLink, ModKey modKey, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordGetter
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolve(formLink, out majorRec);
    }

    /// <summary>
    /// Retrieves the record that matches the EditorID within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    public static bool TryResolveFromMod<TMajor>(this ILinkCache cache, string editorId, ModKey modKey, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolve(editorId, out majorRec);
    }

    #endregion

    #region TryResolveFromMod (Type)

    /// <summary>
    /// Retrieves the record that matches the FormKey and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveFromMod(this ILinkCache cache, FormKey formKey, Type type, ModKey modKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolve(formKey, type, out majorRec);
    }

    /// <summary>
    /// Retrieves the record that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveFromMod(this ILinkCache cache, IFormLinkIdentifier formLink, ModKey modKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolve(formLink, out majorRec);
    }

    /// <summary>
    /// Retrieves the record that matches the EditorID and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveFromMod(this ILinkCache cache, string editorId, Type type, ModKey modKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolve(editorId, type, out majorRec);
    }

    #endregion

    #region ResolveFromMod (Generic)

    /// <summary>
    /// Retrieves the record that matches the FormKey within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static TMajor ResolveFromMod<TMajor>(this ILinkCache cache, FormKey formKey, ModKey modKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveFromMod<TMajor>(cache, formKey, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    /// <summary>
    /// Retrieves the record that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static TMajor ResolveFromMod<TMajor>(this ILinkCache cache, IFormLinkGetter<TMajor> formLink, ModKey modKey)
        where TMajor : class, IMajorRecordGetter
    {
        if (TryResolveFromMod<TMajor>(cache, formLink, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formLink.FormKey, typeof(TMajor));
    }

    /// <summary>
    /// Retrieves the record that matches the EditorID within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static TMajor ResolveFromMod<TMajor>(this ILinkCache cache, string editorId, ModKey modKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveFromMod<TMajor>(cache, editorId, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    #endregion

    #region ResolveFromMod (Type)

    /// <summary>
    /// Retrieves the record that matches the FormKey and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IMajorRecordGetter ResolveFromMod(this ILinkCache cache, FormKey formKey, Type type, ModKey modKey)
    {
        if (TryResolveFromMod(cache, formKey, type, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <summary>
    /// Retrieves the record that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IMajorRecordGetter ResolveFromMod(this ILinkCache cache, IFormLinkIdentifier formLink, ModKey modKey)
    {
        if (TryResolveFromMod(cache, formLink, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formLink);
    }

    /// <summary>
    /// Retrieves the record that matches the EditorID and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IMajorRecordGetter ResolveFromMod(this ILinkCache cache, string editorId, Type type, ModKey modKey)
    {
        if (TryResolveFromMod(cache, editorId, type, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, type);
    }

    #endregion

    #region TryResolveSimpleContextFromMod (Generic)

    /// <summary>
    /// Retrieves the record context that matches the FormKey within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    public static bool TryResolveSimpleContextFromMod<TMajor>(this ILinkCache cache, FormKey formKey, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveSimpleContext(formKey, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    public static bool TryResolveSimpleContextFromMod<TMajor>(this ILinkCache cache, IFormLinkGetter<TMajor> formLink, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec)
        where TMajor : class, IMajorRecordGetter
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveSimpleContext(formLink, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    public static bool TryResolveSimpleContextFromMod<TMajor>(this ILinkCache cache, string editorId, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveSimpleContext(editorId, out majorRec);
    }

    #endregion

    #region TryResolveSimpleContextFromMod (Type)

    /// <summary>
    /// Retrieves the record context that matches the FormKey and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveSimpleContextFromMod(this ILinkCache cache, FormKey formKey, Type type, ModKey modKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveSimpleContext(formKey, type, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveSimpleContextFromMod(this ILinkCache cache, IFormLinkIdentifier formLink, ModKey modKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveSimpleContext(formLink, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveSimpleContextFromMod(this ILinkCache cache, string editorId, Type type, ModKey modKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (!cache.TryGetLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveSimpleContext(editorId, type, out majorRec);
    }

    #endregion

    #region ResolveSimpleContextFromMod (Generic)

    /// <summary>
    /// Retrieves the record context that matches the FormKey within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMajor> ResolveSimpleContextFromMod<TMajor>(this ILinkCache cache, FormKey formKey, ModKey modKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContextFromMod<TMajor>(cache, formKey, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMajor> ResolveSimpleContextFromMod<TMajor>(this ILinkCache cache, IFormLinkGetter<TMajor> formLink, ModKey modKey)
        where TMajor : class, IMajorRecordGetter
    {
        if (TryResolveSimpleContextFromMod<TMajor>(cache, formLink, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formLink.FormKey, typeof(TMajor));
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <typeparam name="TMajor">The type of Major Record to look up</typeparam>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMajor> ResolveSimpleContextFromMod<TMajor>(this ILinkCache cache, string editorId, ModKey modKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContextFromMod<TMajor>(cache, editorId, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    #endregion

    #region ResolveSimpleContextFromMod (Type)

    /// <summary>
    /// Retrieves the record context that matches the FormKey and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<IMajorRecordGetter> ResolveSimpleContextFromMod(this ILinkCache cache, FormKey formKey, Type type, ModKey modKey)
    {
        if (TryResolveSimpleContextFromMod(cache, formKey, type, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<IMajorRecordGetter> ResolveSimpleContextFromMod(this ILinkCache cache, IFormLinkIdentifier formLink, ModKey modKey)
    {
        if (TryResolveSimpleContextFromMod(cache, formLink, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formLink);
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<IMajorRecordGetter> ResolveSimpleContextFromMod(this ILinkCache cache, string editorId, Type type, ModKey modKey)
    {
        if (TryResolveSimpleContextFromMod(cache, editorId, type, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, type);
    }

    #endregion

    #region TryResolveContextFromMod (Generic)

    /// <summary>
    /// Retrieves the record context that matches the FormKey within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(this ILinkCache<TMod, TModGetter> cache, FormKey formKey, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (!cache.TryGetTypedLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(this ILinkCache<TMod, TModGetter> cache, IFormLinkGetter<TMajorGetter> formLink, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (!cache.TryGetTypedLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveContext<TMajor, TMajorGetter>(formLink, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(this ILinkCache<TMod, TModGetter> cache, string editorId, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (!cache.TryGetTypedLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveContext<TMajor, TMajorGetter>(editorId, out majorRec);
    }

    #endregion

    #region TryResolveContextFromMod (Type)

    /// <summary>
    /// Retrieves the record context that matches the FormKey and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveContextFromMod<TMod, TModGetter>(this ILinkCache<TMod, TModGetter> cache, FormKey formKey, Type type, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (!cache.TryGetTypedLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveContext(formKey, type, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveContextFromMod<TMod, TModGetter>(this ILinkCache<TMod, TModGetter> cache, IFormLinkIdentifier formLink, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (!cache.TryGetTypedLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveContext(formLink, out majorRec);
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <param name="majorRec">Out parameter containing the record context if successful</param>
    /// <returns>True if the mod was found and contained a matching record</returns>
    public static bool TryResolveContextFromMod<TMod, TModGetter>(this ILinkCache<TMod, TModGetter> cache, string editorId, Type type, ModKey modKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (!cache.TryGetTypedLinkCacheForMod(modKey, out var modCache))
        {
            majorRec = default;
            return false;
        }

        return modCache.TryResolveContext(editorId, type, out majorRec);
    }

    #endregion

    #region ResolveContextFromMod (Generic)

    /// <summary>
    /// Retrieves the record context that matches the FormKey within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(this ILinkCache<TMod, TModGetter> cache, FormKey formKey, ModKey modKey)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(cache, formKey, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formKey, typeof(TMajorGetter));
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(this ILinkCache<TMod, TModGetter> cache, IFormLinkGetter<TMajorGetter> formLink, ModKey modKey)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (TryResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(cache, formLink, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formLink.FormKey, typeof(TMajorGetter));
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(this ILinkCache<TMod, TModGetter> cache, string editorId, ModKey modKey)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContextFromMod<TMod, TModGetter, TMajor, TMajorGetter>(cache, editorId, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(TMajorGetter));
    }

    #endregion

    #region ResolveContextFromMod (Type)

    /// <summary>
    /// Retrieves the record context that matches the FormKey and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formKey">FormKey to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContextFromMod<TMod, TModGetter>(this ILinkCache<TMod, TModGetter> cache, FormKey formKey, Type type, ModKey modKey)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (TryResolveContextFromMod(cache, formKey, type, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <summary>
    /// Retrieves the record context that matches the FormLink within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="formLink">FormLink to look for</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContextFromMod<TMod, TModGetter>(this ILinkCache<TMod, TModGetter> cache, IFormLinkIdentifier formLink, ModKey modKey)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (TryResolveContextFromMod(cache, formLink, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(formLink);
    }

    /// <summary>
    /// Retrieves the record context that matches the EditorID and Type within the contribution of the mod matching the given ModKey.
    /// </summary>
    /// <param name="cache">LinkCache to resolve against</param>
    /// <param name="editorId">EditorID to look for</param>
    /// <param name="type">Type of record to look up</param>
    /// <param name="modKey">ModKey of the mod to scope resolution to</param>
    /// <returns>Matching record context</returns>
    /// <exception cref="MissingRecordException">If the mod was not found, or did not contain a matching record</exception>
    public static IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContextFromMod<TMod, TModGetter>(this ILinkCache<TMod, TModGetter> cache, string editorId, Type type, ModKey modKey)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (TryResolveContextFromMod(cache, editorId, type, modKey, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, type);
    }

    #endregion
}
