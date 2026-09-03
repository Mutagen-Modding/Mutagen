using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
namespace Mutagen.Bethesda.Skyrim;

public static class LocationExt
{
    /// <summary>
    /// Gets the location's reference type references, including added static entries and excluding removed entries.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <returns>Active reference type references</returns>
    public static IEnumerable<ILocationRefTypeReferenceGetter> LocationRefTypesReferences(this ILocationGetter location)
    {
        IEnumerable<ILocationRefTypeReferenceGetter> Added()
        {
            var staticReferences = location.LocationRefTypeReferencesStatic;
            if (staticReferences is not null)
            {
                foreach (var reference in staticReferences)
                {
                    yield return reference;
                }
            }

            var addedReferences = location.LocationRefTypeReferencesAdded;
            if (addedReferences is not null)
            {
                foreach (var reference in addedReferences)
                {
                    yield return reference;
                }
            }
        }

        var removed = location.LocationRefTypeReferencesRemoved?.Select(x => x.FormKey).ToArray();
        if (removed is null) return Added();

        return Added()
            .Where(x => !removed.Contains(x.Ref.FormKey));
    }

    /// <summary>
    /// Gets all location reference type references across the override chain up to the specified mod key.
    /// Starting from the first definition of the location, it aggregates all added references and removes any that are marked as removed in subsequent overrides.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <param name="linkCache">Link cache used to walk overrides</param>
    /// <param name="highestOverrideModKey">Highest override mod key to include</param>
    /// <returns>Aggregated location reference type references</returns>
    public static IEnumerable<ILocationRefTypeReferenceGetter> AllLocationRefTypesReferences(this ILocationGetter location, ILinkCache linkCache, ModKey highestOverrideModKey)
    {
        var refTypes = new HashSet<ILocationRefTypeReferenceGetter>();
        foreach (var previousOverride in linkCache.GetPreviousOverrides(location, highestOverrideModKey))
        {
            refTypes.Add(Added(previousOverride));
            var removed = Removed(previousOverride).Select(x => x.FormKey).ToHashSet();
            refTypes.RemoveWhere(x => removed.Contains(x.Ref.FormKey));
        }
        return refTypes;

        IEnumerable<ILocationRefTypeReferenceGetter> Added(ILocationGetter loc)
        {
            var referencesStatic = loc.LocationRefTypeReferencesStatic;
            if (referencesStatic is not null) return referencesStatic;

            var typeReferencesAdded = loc.LocationRefTypeReferencesAdded;
            return typeReferencesAdded ?? [];
        }

        IReadOnlyList<IFormLinkGetter<IPlacedSimpleGetter>> Removed(ILocationGetter loc)
        {
            var referencesRemoved = loc.LocationRefTypeReferencesRemoved;
            if (referencesRemoved is not null) return referencesRemoved;

            return [];
        }
    }

    /// <summary>
    /// Gets the location's persistent actor references, including added static entries and excluding removed entries.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <returns>Active persistent actor references</returns>
    public static IEnumerable<IPersistentActorReferenceGetter> PersistentActorReferences(this ILocationGetter location)
    {
        IEnumerable<IPersistentActorReferenceGetter> Added()
        {
            var staticReferences = location.PersistentActorReferencesStatic;
            if (staticReferences is not null)
            {
                foreach (var reference in staticReferences)
                {
                    yield return reference;
                }
            }

            var addedReferences = location.PersistentActorReferencesAdded;
            if (addedReferences is not null)
            {
                foreach (var reference in addedReferences)
                {
                    yield return reference;
                }
            }
        }

        var removed = location.PersistentActorReferencesRemoved?.Select(x => x.FormKey).ToArray();
        if (removed is null) return Added();

        return Added()
            .Where(x => !removed.Contains(x.Actor.FormKey));
    }

    /// <summary>
    /// Gets all persistent actor references across the override chain up to the specified mod key.
    /// Starting from the first definition of the location, it aggregates all added references and removes any that are marked as removed in subsequent overrides.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <param name="linkCache">Link cache used to walk overrides</param>
    /// <param name="highestOverrideModKey">Highest override mod key to include</param>
    /// <returns>Aggregated persistent actor references</returns>
    public static IEnumerable<IPersistentActorReferenceGetter> AllPersistentActorReferences(this ILocationGetter location, ILinkCache linkCache, ModKey highestOverrideModKey)
    {
        var actors = new HashSet<IPersistentActorReferenceGetter>();
        foreach (var previousOverride in linkCache.GetPreviousOverrides(location, highestOverrideModKey))
        {
            actors.Add(Added(previousOverride));
            var removed = Removed(previousOverride).Select(x => x.FormKey).ToHashSet();
            actors.RemoveWhere(x => removed.Contains(x.Actor.FormKey));
        }
        return actors;

        IEnumerable<IPersistentActorReferenceGetter> Added(ILocationGetter loc)
        {
            var referencesStatic = loc.PersistentActorReferencesStatic;
            if (referencesStatic is not null) return referencesStatic;

            var typeReferencesAdded = loc.PersistentActorReferencesAdded;
            return typeReferencesAdded ?? [];
        }

        IReadOnlyList<IFormLinkGetter<IPlacedSimpleGetter>> Removed(ILocationGetter loc)
        {
            var referencesRemoved = loc.PersistentActorReferencesRemoved;
            if (referencesRemoved is not null) return referencesRemoved;

            return [];
        }
    }

    /// <summary>
    /// Gets the location's unique actor references, including added static entries and excluding removed entries.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <returns>Active unique actor references</returns>
    public static IEnumerable<IUniqueActorReferenceGetter> UniqueActorReferences(this ILocationGetter location)
    {
        IEnumerable<IUniqueActorReferenceGetter> Added()
        {
            var staticReferences = location.UniqueActorReferencesStatic;
            if (staticReferences is not null)
            {
                foreach (var reference in staticReferences)
                {
                    yield return reference;
                }
            }

            var addedReferences = location.UniqueActorReferencesAdded;
            if (addedReferences is not null)
            {
                foreach (var reference in addedReferences)
                {
                    yield return reference;
                }
            }
        }

        var removed = location.UniqueActorReferencesRemoved?.Select(x => x.FormKey).ToArray();
        if (removed is null) return Added();

        return Added()
            .Where(x => !removed.Contains(x.Actor.FormKey));
    }

    /// <summary>
    /// Gets all unique actor references across the override chain up to the specified mod key.
    /// Starting from the first definition of the location, it aggregates all added references and removes any that are marked as removed in subsequent overrides.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <param name="linkCache">Link cache used to walk overrides</param>
    /// <param name="highestOverrideModKey">Highest override mod key to include</param>
    /// <returns>Aggregated unique actor references</returns>
    public static IEnumerable<IUniqueActorReferenceGetter> AllUniqueActorReferences(this ILocationGetter location, ILinkCache linkCache, ModKey highestOverrideModKey)
    {
        var actors = new HashSet<IUniqueActorReferenceGetter>();
        foreach (var previousOverride in linkCache.GetPreviousOverrides(location, highestOverrideModKey))
        {
            actors.Add(Added(previousOverride));
            var removed = Removed(previousOverride).Select(x => x.FormKey).ToHashSet();
            actors.RemoveWhere(x => removed.Contains(x.Actor.FormKey));
        }
        return actors;

        IReadOnlyList<IUniqueActorReferenceGetter> Added(ILocationGetter loc)
        {
            var referencesStatic = loc.UniqueActorReferencesStatic;
            if (referencesStatic is not null) return referencesStatic;

            var typeReferencesAdded = loc.UniqueActorReferencesAdded;
            return typeReferencesAdded ?? [];
        }

        IReadOnlyList<IFormLinkGetter<INpcGetter>> Removed(ILocationGetter loc)
        {
            var referencesRemoved = loc.UniqueActorReferencesRemoved;
            if (referencesRemoved is not null) return referencesRemoved;

            return [];
        }
    }

    /// <summary>
    /// Gets the location's initially disabled references.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <returns>Initially disabled references</returns>
    public static IEnumerable<IFormLinkGetter<IPlacedGetter>> InitiallyDisabledReferences(this ILocationGetter location)
    {
        var staticReferences = location.InitiallyDisabledReferencesAdded;
        if (staticReferences is not null)
        {
            foreach (var reference in staticReferences)
            {
                yield return reference;
            }
        }

        var addedReferences = location.InitiallyDisabledReferencesAdded;
        if (addedReferences is not null)
        {
            foreach (var reference in addedReferences)
            {
                yield return reference;
            }
        }
    }

    /// <summary>
    /// Gets all initially disabled references across the override chain up to the specified mod key.
    /// Starting from the first definition of the location, it aggregates all added references and removes any that are marked as removed in subsequent overrides.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <param name="linkCache">Link cache used to walk overrides</param>
    /// <param name="highestOverrideModKey">Highest override mod key to include</param>
    /// <returns>Aggregated initially disabled references</returns>
    public static IEnumerable<IFormLinkGetter<IPlacedGetter>> AllInitiallyDisabledReferences(this ILocationGetter location, ILinkCache linkCache, ModKey highestOverrideModKey)
    {
        var actors = new HashSet<IFormLinkGetter<IPlacedGetter>>();
        foreach (var previousOverride in linkCache.GetPreviousOverrides(location, highestOverrideModKey))
        {
            actors.Add(Added(previousOverride));
        }
        return actors;

        IReadOnlyList<IFormLinkGetter<IPlacedGetter>> Added(ILocationGetter loc)
        {
            var referencesStatic = loc.InitiallyDisabledReferencesStatic;
            if (referencesStatic is not null) return referencesStatic;

            var typeReferencesAdded = loc.InitiallyDisabledReferencesAdded;
            return typeReferencesAdded ?? [];
        }
    }

    /// <summary>
    /// Gets the location's enable-parent references.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <returns>Enable-parent references</returns>
    public static IEnumerable<IEnableParentReferenceGetter> EnableParentReferences(this ILocationGetter location)
    {
        var staticReferences = location.EnableParentReferencesStatic;
        if (staticReferences is not null)
        {
            foreach (var reference in staticReferences)
            {
                yield return reference;
            }
        }

        var addedReferences = location.EnableParentReferencesAdded;
        if (addedReferences is not null)
        {
            foreach (var reference in addedReferences)
            {
                yield return reference;
            }
        }
    }

    /// <summary>
    /// Gets all enable-parent references across the override chain up to the specified mod key.
    /// Starting from the first definition of the location, it aggregates all added references and removes any that are marked as removed in subsequent overrides.
    /// </summary>
    /// <param name="location">Location to inspect</param>
    /// <param name="linkCache">Link cache used to walk overrides</param>
    /// <param name="highestOverrideModKey">Highest override mod key to include</param>
    /// <returns>Aggregated enable-parent references</returns>
    public static IEnumerable<IEnableParentReferenceGetter> AllEnableParentReferences(this ILocationGetter location, ILinkCache linkCache, ModKey highestOverrideModKey)
    {
        var actors = new HashSet<IEnableParentReferenceGetter>();
        foreach (var previousOverride in linkCache.GetPreviousOverrides(location, highestOverrideModKey))
        {
            actors.Add(Added(previousOverride));
        }
        return actors;

        IReadOnlyList<IEnableParentReferenceGetter> Added(ILocationGetter loc)
        {
            var referencesStatic = loc.EnableParentReferencesStatic;
            if (referencesStatic is not null) return referencesStatic;

            var typeReferencesAdded = loc.EnableParentReferencesAdded;
            return typeReferencesAdded ?? [];
        }
    }
}
