using Mutagen.Bethesda.Plugins;
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

    public static IEnumerable<ILocationRefTypeReferenceGetter> LocationRefTypeReference(this ILocationGetter location)
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
}