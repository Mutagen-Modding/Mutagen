using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda;

public static class GetOrAddAsOverrideMixIns
{
    /// <summary>
    /// Takes in an existing record definition, and either returns the existing override definition
    /// from the Group, or copies the given record, inserts it, and then returns it as an override.
    /// </summary>
    /// <param name="group">Group to retrieve and/or insert from</param>
    /// <param name="major">Major record to query and potentially copy</param>
    /// <returns>Existing override record, or a copy of the given record that has already been inserted into the group</returns>
    public static TMajor GetOrAddAsOverride<TMajor, TMajorGetter>(this IGroup<TMajor> group, TMajorGetter major)
        where TMajor : IMajorRecordInternal, TMajorGetter
        where TMajorGetter : IMajorRecordGetter
    {
        try
        {
            if (group.RecordCache.TryGetValue(major.FormKey, out var existingMajor))
            {
                return existingMajor;
            }
            var mask = OverrideMaskRegistrations.Get<TMajor>();
            var copy = major.DeepCopy(mask as MajorRecord.TranslationMask);
            if (copy is not TMajor rhs)
            {
                throw new InvalidOperationException($"DeepCopy did not return a record of the expected type {typeof(TMajor).Name}");
            }
            existingMajor = rhs;
            group.RecordCache.Set(existingMajor);
            return existingMajor;
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow<TMajor>(ex, major.FormKey, major.EditorID);
            throw;
        }
    }

    /// <summary>
    /// Takes in a FormLink, and either returns the existing override definition
    /// from the Group, or attempts to link and copy the given record, inserting it, and then returning it as an override.
    /// </summary>
    /// <param name="group">Group to retrieve and/or insert from</param>
    /// <param name="link">Link to query and add</param>
    /// <param name="cache">Cache to query link against</param>
    /// <param name="rec">Retrieved record if successful</param>
    /// <returns>True if a record was retrieved</returns>
    public static bool TryGetOrAddAsOverride<TMajor, TMajorGetter>(this IGroup<TMajor> group, IFormLinkGetter<TMajorGetter> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor rec)
        where TMajor : class, IMajorRecordInternal, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        try
        {
            if (group.RecordCache.TryGetValue(link.FormKey, out rec))
            {
                return true;
            }
            if (!link.TryResolve<TMajorGetter>(cache, out var getter))
            {
                rec = default;
                return false;
            }
            rec = GetOrAddAsOverride(group, getter);
            return true;
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow<TMajor>(ex, link.FormKey, edid: null);
            throw;
        }
    }

    /// <summary>
    /// Takes in a FormLink, and either returns the existing override definition
    /// from the Group, or attempts to link and copy the given record, inserting it, and then returning it as an override.
    /// </summary>
    /// <param name="group">Group to retrieve and/or insert from</param>
    /// <param name="link">Link to query and add</param>
    /// <param name="cache">Cache to query link against</param>
    /// <returns>Retrieved record if successful</returns>
    public static TMajor GetOrAddAsOverride<TMajor, TMajorGetter>(this IGroup<TMajor> group, IFormLinkGetter<TMajorGetter> link, ILinkCache cache)
        where TMajor : class, IMajorRecordInternal, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (TryGetOrAddAsOverride(group, link, cache, out var rec))
        {
            return rec;
        }
        throw new MissingRecordException(link.FormKey, link.Type);
    }

    /// <summary>
    /// Takes in an existing record definition, and either returns the existing override definition
    /// from the Group, or copies the given record, inserts it, and then returns it as an override.
    /// Returns false if the record type does not match the group's contained type.
    /// </summary>
    /// <param name="group">Group to retrieve and/or insert from</param>
    /// <param name="major">Major record to query and potentially copy</param>
    /// <param name="result">The existing or newly created override record</param>
    /// <returns>True if successful, false if the record type does not match the group</returns>
    public static bool TryGetOrAddAsOverrideUntyped(
        this IGroup group,
        IMajorRecordGetter major,
        [MaybeNullWhen(false)] out IMajorRecord result)
    {
        try
        {
            // Check if the record type is assignable to the group's contained type
            if (!group.ContainedRecordType.IsAssignableFrom(major.GetType()))
            {
                result = null;
                return false;
            }

            // Check if the record already exists in the group
            if (group.RecordCache.TryGetValue(major.FormKey, out var existingMajor))
            {
                if (existingMajor is IMajorRecord existingRecord)
                {
                    result = existingRecord;
                    return true;
                }
                result = null;
                return false;
            }

            // Get the override mask for this record type
            var mask = OverrideMaskRegistrations.Get(major.GetType());

            // Create a deep copy with the override mask
            var copy = major.DeepCopy(mask as MajorRecord.TranslationMask);
            if (copy is not IMajorRecord rhs)
            {
                result = null;
                return false;
            }

            // Add the copy to the group
            group.SetUntyped(rhs);
            result = rhs;
            return true;
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, major.FormKey, major.GetType(), major.EditorID);
            throw;
        }
    }
}