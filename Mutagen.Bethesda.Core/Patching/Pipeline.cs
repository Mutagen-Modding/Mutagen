using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class Pipeline
    {
        public static void TypicalPatch<TMod>(
            string[] mainArgs,
            ModKey outputMod,
            List<ModKey> loadOrderListing,
            LoadOrder<TMod>.Importer importer,
            Func<ModKey, LoadOrder<TMod>, TMod> processor)
            where TMod : class, IMod
        {
            TypicalPatch(
                dataFolder: new Noggog.DirectoryPath(mainArgs[0]),
                outModKey: outputMod,
                loadOrderListing: loadOrderListing,
                processor: processor,
                importer: importer);
        }

        public static void TypicalPatch<TMod>(
            DirectoryPath dataFolder,
            ModKey outModKey,
            List<ModKey> loadOrderListing,
            LoadOrder<TMod>.Importer importer,
            Func<ModKey, LoadOrder<TMod>, TMod> processor)
            where TMod : class, IMod
        {
            loadOrderListing.Remove(outModKey);
            var loadOrder = new LoadOrder<TMod>();
            loadOrder.Import(
                dataFolder,
                loadOrderListing,
                importer);
            var outMod = processor(outModKey, loadOrder);
            foreach (var npc in outMod.EnumerateMajorRecords())
            {
                npc.IsCompressed = false;
            }
            var linkCache = loadOrder.CreateLinkCache();
            outMod.MasterReferences.SetTo(
                outMod.Links
                    .Select(l =>
                    {
                        if (l.TryResolveFormKey(linkCache, out var form)) return form;
                        return FormKey.Null;
                    })
                    .Where(fk => !fk.IsNull)
                    .Select(s => s.ModKey)
                    .Where(modKey => modKey != outModKey)
                    .Distinct()
                    .OrderBy(modKey => loadOrder.IndexOf(modKey))
                    .Select(modKey => new MasterReference()
                    {
                        Master = modKey
                    }));
            outMod.SyncRecordCount();
            outMod.WriteToBinary(
                path: Path.Combine(dataFolder.Path, outModKey.FileName));
        }
    }
}
