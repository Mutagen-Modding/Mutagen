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
            Func<ModKey, LoadOrder<OblivionMod>, Task<OblivionMod>> processor,
            GroupMask? importMask = null)
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
            Func<ModKey, LoadOrder<OblivionMod>, Task<OblivionMod>> processor,
            GroupMask? importMask = null)
        {
            if (!LoadOrder.TryGetUsualLoadOrder(GameMode.Oblivion, dataFolder, out var loadOrderListing))
            {
                throw new ArgumentException("Could not retrieve load order.");
            }
            return Pipeline.TypicalPatch(
                dataFolder: dataFolder,
                outModKey: outModKey,
                loadOrderListing: loadOrderListing,
                processor: processor,
                importer: async (p, mk) =>
                {
                    var mod = await OblivionMod.CreateFromBinary(
                        p.Path,
                        mk,
                        importMask: importMask).ConfigureAwait(false);
                    return TryGet<OblivionMod>.Succeed(mod);
                });
        }

        public static async Task<TryGet<LoadOrder<OblivionMod>?>> TryImportUsualLoadOrder(
            DirectoryPath dataFolder,
            GroupMask? importMask = null,
            ModKey? modKeyExclusionHint = null)
        {
            if (!LoadOrder.TryGetUsualLoadOrder(GameMode.Oblivion, dataFolder, out var loadOrderListing))
            {
                return TryGet<LoadOrder<OblivionMod>?>.Fail(null);
            }
            if (modKeyExclusionHint != null)
            {
                loadOrderListing.Remove(modKeyExclusionHint.Value);
            }
            var loadOrder = new LoadOrder<OblivionMod>();
            await loadOrder.Import(
                dataFolder,
                loadOrderListing,
                importer: async (path, modKey) =>
                {
                    var mod = await OblivionMod.CreateFromBinary(
                        path.Path,
                        modKey,
                        importMask: importMask).ConfigureAwait(false);
                    return TryGet<OblivionMod>.Succeed(mod);
                }).ConfigureAwait(false);
            return TryGet<LoadOrder<OblivionMod>?>.Succeed(loadOrder);
        }

        public static async Task<LoadOrder<OblivionMod>?> ImportUsualLoadOrder(
            DirectoryPath dataFolder,
            GroupMask? importMask = null,
            ModKey? modKeyExclusionHint = null)
        {
            var tg = await TryImportUsualLoadOrder(
                dataFolder,
                importMask,
                modKeyExclusionHint).ConfigureAwait(false);
            return tg.Value;
        }

        public static OblivionMod Flatten(this LoadOrder<OblivionMod> loadOrder, ModKey? modKey = null)
        {
            if (modKey == null)
            {
                modKey = new ModKey("Flattened", master: false);
            }
            OblivionMod ret = new OblivionMod(modKey.Value);
            foreach (var mod in loadOrder)
            {
                ret.AddRecords(mod.Mod);
            }
            return ret;
        }
    }
}
