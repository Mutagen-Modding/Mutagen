using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda;

public static class DuplicateFromMixIn
{
    /// <summary>
    /// Duplicates records into a given mod 'modToDuplicateInto', which originated from target ModKey 'modKeyToDuplicateFrom'.<br />
    /// Only considers records that are currently within the target modToDuplicateInto, which are then duplicated. <br/>
    /// Records from the modKeyToDuplicateFrom that are not within or referenced by records in the target mod are skipped.<br />
    /// <br />
    /// End result will be that all records that the given modToDuplicateInto contains or references that originate from the target modKeyToDuplicateFrom will be duplicated in
    /// and replace the records they duplicated.  No references to the modKeyToDuplicateFrom will remain.
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    /// <typeparam name="TModGetter"></typeparam>
    /// <param name="modToDuplicateInto">Mod to duplicate into and originate new records from</param>
    /// <param name="linkCache">LinkCache for lookup</param>
    /// <param name="modKeyToDuplicateFrom">ModKey to search modToDuplicateInto for, and duplicate records that originate from modKeyToDuplicateFrom</param>
    /// <param name="typesToInspect">
    /// Types to iterate and look at within modToDuplicateInto for references to modKeyToDuplicateFrom<br />
    /// Only use if you know specifically the types that can reference modKeyToDuplicateFrom, and want a little bit of speed
    /// by not checking uninteresting records.
    /// </param>
    public static void DuplicateFromOnlyReferenced<TMod, TModGetter>(
        this TMod modToDuplicateInto,
        ILinkCache<TMod, TModGetter> linkCache,
        ModKey modKeyToDuplicateFrom,
        params Type[] typesToInspect)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        DuplicateFromOnlyReferenced(
            modToDuplicateInto,
            linkCache,
            modKeyToDuplicateFrom,
            out _,
            typesToInspect);
    }

    /// <summary>
    /// Duplicates records into a given mod 'modToDuplicateInto', which originated from target ModKey 'modKeyToDuplicateFrom'.<br />
    /// Only considers records that are currently within the target modToDuplicateInto, which are then duplicated. <br/>
    /// Records from the modKeyToDuplicateFrom that are not within or referenced by records in the target mod are skipped.<br />
    /// <br />
    /// End result will be that all records that the given modToDuplicateInto contains or references that originate from the target modKeyToDuplicateFrom will be duplicated in
    /// and replace the records they duplicated.  No references to the modKeyToDuplicateFrom will remain.
    /// </summary>
    /// <typeparam name="TMod"></typeparam>
    /// <typeparam name="TModGetter"></typeparam>
    /// <param name="modToDuplicateInto">Mod to duplicate into and originate new records from</param>
    /// <param name="linkCache">LinkCache for lookup</param>
    /// <param name="modKeyToDuplicateFrom">ModKey to search modToDuplicateInto for, and duplicate records that originate from modKeyToDuplicateFrom</param>
    /// <param name="mapping">Out parameter to store the resulting duplication mappings that were made</param>
    /// <param name="typesToInspect">
    /// Types to iterate and look at within modToDuplicateInto for references to modKeyToDuplicateFrom<br />
    /// Only use if you know specifically the types that can reference modKeyToDuplicateFrom, and want a little bit of speed
    /// by not checking uninteresting records.
    /// </param>
    public static void DuplicateFromOnlyReferenced<TMod, TModGetter>(
        this TMod modToDuplicateInto,
        ILinkCache<TMod, TModGetter> linkCache, 
        ModKey modKeyToDuplicateFrom,
        out Dictionary<FormKey, FormKey> mapping,
        params Type[] typesToInspect)
        where TModGetter : class, IModGetter
        where TMod : class, TModGetter, IMod
    {
        if (modKeyToDuplicateFrom == modToDuplicateInto.ModKey)
        {
            throw new ArgumentException("Cannot pass the target mod's Key as the one to extract and self contain");
        }

        // Compile list of things to duplicate
        HashSet<IFormLinkGetter> identifiedLinks = new();
        HashSet<FormKey> passedLinks = new();
        var implicits = Implicits.Get(modToDuplicateInto.GameRelease);

        void AddAllLinks(IFormLinkGetter link)
        {
            if (link.FormKey.IsNull) return;
            if (!passedLinks.Add(link.FormKey)) return;
            if (implicits.RecordFormKeys.Contains(link.FormKey)) return;

            if (link.FormKey.ModKey == modKeyToDuplicateFrom)
            {
                identifiedLinks.Add(link);
            }

            if (!linkCache.TryResolve(link.FormKey, link.Type, out var linkRec))
            {
                return;
            }

            foreach (var containedLink in linkRec.ContainedFormLinks)
            {
                if (containedLink.FormKey.ModKey != modKeyToDuplicateFrom) continue;
                AddAllLinks(containedLink);
            }
        }

        var enumer = typesToInspect == null || typesToInspect.Length == 0
            ? modToDuplicateInto.EnumerateMajorRecords()
            : typesToInspect.SelectMany(x => modToDuplicateInto.EnumerateMajorRecords(x));
        foreach (var rec in enumer)
        {
            AddAllLinks(new FormLinkInformation(rec.FormKey, rec.Registration.GetterType));
        }

        // Duplicate in the records
        mapping = new();
        foreach (var identifiedRec in identifiedLinks)
        {
            if (!linkCache.TryResolveContext(identifiedRec.FormKey, identifiedRec.Type, out var rec))
            {
                throw new KeyNotFoundException($"Could not locate record to make self contained: {identifiedRec}");
            }

            var dup = rec.DuplicateIntoAsNewRecord(modToDuplicateInto, rec.Record.EditorID);
            mapping[rec.Record.FormKey] = dup.FormKey;

            // ToDo
            // Move this out of loop, and remove off a new IEnumerable<IFormLinkGetter> call
            modToDuplicateInto.Remove(identifiedRec.FormKey, identifiedRec.Type);
        }

        // Remap links
        modToDuplicateInto.RemapLinks(mapping);
    }
}