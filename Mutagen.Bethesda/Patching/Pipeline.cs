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
        public static Task TypicalPatch<TMod>(
            string[] mainArgs,
            ModKey outputMod,
            List<ModKey> loadOrderListing,
            Func<FilePath, ModKey, Task<TryGet<TMod>>> importer,
            Func<ModKey, LoadOrder<TMod>, Task<TMod>> processor)
            where TMod : IMod
        {
            return TypicalPatch(
                dataFolder: new Noggog.DirectoryPath(mainArgs[0]),
                outModKey: outputMod,
                loadOrderListing: loadOrderListing,
                processor: processor,
                importer: importer);
        }

        public static async Task TypicalPatch<TMod>(
            DirectoryPath dataFolder,
            ModKey outModKey,
            List<ModKey> loadOrderListing,
            Func<FilePath, ModKey, Task<TryGet<TMod>>> importer,
            Func<ModKey, LoadOrder<TMod>, Task<TMod>> processor)
            where TMod : IMod
        {
            loadOrderListing.Remove(outModKey);
            var loadOrder = new LoadOrder<TMod>();
            await loadOrder.Import(
                dataFolder,
                loadOrderListing,
                importer).ConfigureAwait(false);
            var outMod = await processor(outModKey, loadOrder).ConfigureAwait(false);
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
                        return FormKey.NULL;
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
