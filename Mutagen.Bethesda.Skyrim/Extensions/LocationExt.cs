using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
namespace Mutagen.Bethesda.Skyrim;

public static class LocationExt
{
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

    public static IEnumerable<ILocationRefTypeReferenceGetter> AllLocationRefTypesReferences(this ILocationGetter location, ILinkCache linkCache)
    {
        var previousOverrides = linkCache
            .ResolveAll(location, ResolveTarget.Origin)
            .TakeWhile(x => !Equals(x, location))
            .Append(location)
            .ToArray();

        var refTypes = new HashSet<ILocationRefTypeReferenceGetter>();
        foreach (var previousOverride in previousOverrides)
        {
            refTypes.Add(Added(previousOverride));
            var removed = Removed(previousOverride);
            refTypes.RemoveWhere(x => removed.Any(r => r.FormKey == x.Ref.FormKey));
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

    public static HashSet<IPersistentActorReferenceGetter> AllPersistentActorReferences(this ILocationGetter location, ILinkCache linkCache)
    {
        var previousOverrides = linkCache
            .ResolveAll(location, ResolveTarget.Origin)
            .TakeWhile(x => !Equals(x, location))
            .Append(location)
            .ToArray();

        var actors = new HashSet<IPersistentActorReferenceGetter>();
        foreach (var previousOverride in previousOverrides)
        {
            actors.Add(Added(previousOverride));
            var removed = Removed(previousOverride);
            actors.RemoveWhere(x => removed.Any(r => r.FormKey == x.Actor.FormKey));
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

    public static HashSet<IUniqueActorReferenceGetter> AllUniqueActorReferences(this ILocationGetter location, ILinkCache linkCache)
    {
        var previousOverrides = linkCache
            .ResolveAll(location, ResolveTarget.Origin)
            .TakeWhile(x => !Equals(x, location))
            .Append(location)
            .ToArray();

        var actors = new HashSet<IUniqueActorReferenceGetter>();
        foreach (var previousOverride in previousOverrides)
        {
            actors.Add(Added(previousOverride));
            var removed = Removed(previousOverride);
            actors.RemoveWhere(x => removed.Any(r => r.FormKey == x.Actor.FormKey));
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

    public static HashSet<IFormLinkGetter<IPlacedGetter>> AllInitiallyDisabledReferences(this ILocationGetter location, ILinkCache linkCache)
    {
        var previousOverrides = linkCache
            .ResolveAll(location, ResolveTarget.Origin)
            .TakeWhile(x => !Equals(x, location))
            .Append(location)
            .ToArray();

        var actors = new HashSet<IFormLinkGetter<IPlacedGetter>>();
        foreach (var previousOverride in previousOverrides)
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

    public static HashSet<IEnableParentReferenceGetter> AllEnableParentReferences(this ILocationGetter location, ILinkCache linkCache)
    {
        var previousOverrides = linkCache
            .ResolveAll(location, ResolveTarget.Origin)
            .TakeWhile(x => !Equals(x, location))
            .Append(location)
            .ToArray();

        var actors = new HashSet<IEnableParentReferenceGetter>();
        foreach (var previousOverride in previousOverrides)
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