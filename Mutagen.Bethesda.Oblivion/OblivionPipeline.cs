using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public static class OblivionPipeline
    {
        public static Task TypicalPatch(
            string[] mainArgs,
            ModKey outputMod,
            Func<ModKey, ModList<OblivionMod>, Task<OblivionMod>> processor,
            GroupMask importMask = null)
        {
            return TypicalPatch(
                dataFolder: new Noggog.DirectoryPath(mainArgs[0]),
                outModKey: outputMod,
                processor: processor,
                importMask: importMask);
        }

        public static Task TypicalPatch(
            DirectoryPath dataFolder,
            ModKey outModKey,
            Func<ModKey, ModList<OblivionMod>, Task<OblivionMod>> processor,
            GroupMask importMask = null)
        {
            if (!LoadOrder.TryGetUsualLoadOrder(dataFolder, out var loadOrder))
            {
                throw new ArgumentException("Could not retrieve load order.");
            }
            return Pipeline.TypicalPatch(
                dataFolder: dataFolder,
                outModKey: outModKey,
                loadOrder: loadOrder,
                processor: processor,
                importer: async (p, mk) =>
                {
                    var mod = await OblivionMod.CreateFromBinary(
                        p.Path,
                        mk,
                        importMask: importMask);
                    return TryGet<OblivionMod>.Succeed(mod);
                });
        }

        public static async Task<TryGet<ModList<OblivionMod>>> TryImportUsualLoadOrder(
            DirectoryPath dataFolder,
            GroupMask importMask = null,
            ModKey modKeyExclusionHint = null)
        {
            if (!LoadOrder.TryGetUsualLoadOrder(dataFolder, out var loadOrder))
            {
                return TryGet<ModList<OblivionMod>>.Fail(null);
            }
            if (modKeyExclusionHint != null)
            {
                loadOrder.Remove(modKeyExclusionHint);
            }
            var modList = new ModList<OblivionMod>();
            await modList.Import(
                dataFolder,
                loadOrder,
                importer: async (path, modKey) =>
                {
                    var mod = await OblivionMod.CreateFromBinary(
                        path.Path,
                        modKey,
                        importMask: importMask);
                    return TryGet<OblivionMod>.Succeed(mod);
                });
            return TryGet<ModList<OblivionMod>>.Succeed(modList);
        }

        public static async Task<ModList<OblivionMod>> ImportUsualLoadOrder(
            DirectoryPath dataFolder,
            GroupMask importMask = null,
            ModKey modKeyExclusionHint = null)
        {
            var tg = await TryImportUsualLoadOrder(
                dataFolder,
                importMask,
                modKeyExclusionHint);
            return tg.Value;
        }

        public static OblivionMod Flatten(this ModList<OblivionMod> modList, ModKey modKey = null)
        {
            if (modKey == null)
            {
                modKey = new ModKey("Flattened", master: false);
            }
            OblivionMod ret = new OblivionMod(modKey);
            foreach (var mod in modList)
            {
                ret.AddRecords(mod.Mod);
            }
            return ret;
        }
    }
}
