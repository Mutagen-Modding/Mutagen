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
            List<ModKey> loadOrder,
            Func<FilePath, ModKey, Task<TryGet<TMod>>> importer,
            Func<ModKey, ModList<TMod>, Task<TMod>> processor)
            where TMod : IMod
        {
            return TypicalPatch(
                dataFolder: new Noggog.DirectoryPath(mainArgs[0]),
                outModKey: outputMod,
                loadOrder: loadOrder,
                processor: processor,
                importer: importer);
        }

        public static async Task TypicalPatch<TMod>(
            DirectoryPath dataFolder,
            ModKey outModKey,
            List<ModKey> loadOrder,
            Func<FilePath, ModKey, Task<TryGet<TMod>>> importer,
            Func<ModKey, ModList<TMod>, Task<TMod>> processor)
            where TMod : IMod
        {
            loadOrder.Remove(outModKey);
            var modList = new ModList<TMod>();
            await modList.Import(
                dataFolder,
                loadOrder,
                importer).ConfigureAwait(false);
            var outMod = await processor(outModKey, modList).ConfigureAwait(false);
            foreach (var npc in outMod.MajorRecords.Items)
            {
                npc.MajorRecordFlagsRaw &= ~(int)MajorRecord.MajorRecordFlag.Compressed;
            }
            outMod.MasterReferences.SetTo(
                outMod.Links
                    .Select(l => l.FormKey)
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
                path: Path.Combine(dataFolder.Path, outModKey.FileName),
                modKey: outModKey);
        }
    }
}
