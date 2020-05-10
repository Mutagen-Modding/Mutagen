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
            IReadOnlyList<ModKey> loadOrderList,
            LoadOrder<TMod>.Importer importer,
            Func<ModKey, LoadOrder<TMod>, TMod> processor)
            where TMod : class, IMod
        {
            TypicalPatch(
                dataFolder: new Noggog.DirectoryPath(mainArgs[0]),
                outModKey: outputMod,
                loadOrderList: loadOrderList,
                processor: processor,
                importer: importer);
        }

        public static void TypicalPatch<TMod>(
            DirectoryPath dataFolder,
            ModKey outModKey,
            IReadOnlyList<ModKey> loadOrderList,
            LoadOrder<TMod>.Importer importer,
            Func<ModKey, LoadOrder<TMod>, TMod> processor)
            where TMod : class, IMod
        {
            var loadOrderInternal = loadOrderList.ToList();
            loadOrderInternal.Remove(outModKey);
            var loadOrder = new LoadOrder<TMod>();
            loadOrder.Import(
                dataFolder,
                loadOrderInternal,
                importer);
            var outMod = processor(outModKey, loadOrder);
            foreach (var npc in outMod.EnumerateMajorRecords())
            {
                npc.IsCompressed = false;
            }
            var linkCache = loadOrder.CreateLinkCache();
            outMod.MasterReferences.SetTo(
                outMod.LinkFormKeys
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
