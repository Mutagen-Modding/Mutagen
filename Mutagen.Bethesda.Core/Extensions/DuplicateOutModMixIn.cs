using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class DuplicateFromMixIn
    {
        public static void DuplicateFromOnlyReferenced<TMod, TModGetter>(
            this TMod mod,
            ILinkCache<TMod, TModGetter> linkCache,
            ModKey modKey)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IMod
        {
            DuplicateFromOnlyReferenced(
                mod,
                linkCache,
                modKey,
                out _);
        }

        public static void DuplicateFromOnlyReferenced<TMod, TModGetter>(
            this TMod mod,
            ILinkCache<TMod, TModGetter> linkCache, 
            ModKey modKey,
            out Dictionary<FormKey, FormKey> mapping)
            where TModGetter : class, IModGetter
            where TMod : class, TModGetter, IMod
        {
            if (modKey == mod.ModKey)
            {
                throw new ArgumentException("Cannot pass the target mod's Key as the one to extract and self contain");
            }

            // Compile list of things to duplicate
            HashSet<FormLinkInformation> identifiedLinks = new();
            foreach (var rec in mod.EnumerateMajorRecords())
            {
                if (rec.FormKey.ModKey == modKey)
                {
                    identifiedLinks.Add(new FormLinkInformation(rec.FormKey, rec.Registration.GetterType));
                }

                foreach (var containedLink in rec.ContainedFormLinks)
                {
                    if (containedLink.FormKey.ModKey == modKey)
                    {
                        identifiedLinks.Add(containedLink);
                    }
                }
            }

            // Duplicate in the records
            mapping = new();
            foreach (var identifiedRec in identifiedLinks)
            {
                if (!linkCache.TryResolveContext(identifiedRec.FormKey, identifiedRec.Type, out var rec))
                {
                    throw new KeyNotFoundException($"Coult not locate record to make self contained: {identifiedRec}");
                }

                var dup = rec.DuplicateIntoAsNewRecord(mod, rec.Record.EditorID);
                mapping[rec.Record.FormKey] = dup.FormKey;

                // ToDo
                // Move this out of loop, and remove off a new IEnumerable<FormLinkInformation> call
                mod.Remove(identifiedRec.FormKey, identifiedRec.Type);
            }

            // Remap links
            mod.RemapLinks(mapping);
        }
    }
}
