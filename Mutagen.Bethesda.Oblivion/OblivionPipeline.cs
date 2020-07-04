using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public static class OblivionPipeline
    {
        public static void TypicalPatch(
            string[] mainArgs,
            ModKey outputMod,
            Func<ModKey, LoadOrder<OblivionMod>, OblivionMod> processor,
            GroupMask? importMask = null)
        {
            TypicalPatch(
                dataFolder: new Noggog.DirectoryPath(mainArgs[0]),
                outModKey: outputMod,
                processor: processor,
                importMask: importMask);
        }

        public static void TypicalPatch(
            DirectoryPath dataFolder,
            ModKey outModKey,
            Func<ModKey, LoadOrder<OblivionMod>, OblivionMod> processor,
            GroupMask? importMask = null,
            bool allowMissingMods = false)
        {
            var loadOrderList = LoadOrder.GetUsualLoadOrder(GameRelease.Oblivion, dataFolder, allowMissingMods: allowMissingMods);
            Pipeline.TypicalPatch(
                dataFolder: dataFolder,
                outModKey: outModKey,
                loadOrderList: loadOrderList,
                processor: processor,
                importer: (FilePath path, ModKey modKey, out OblivionMod mod) =>
                {
                    mod = OblivionMod.CreateFromBinary(
                        path.Path,
                        modKey,
                        importMask: importMask);
                    return true;
                });
        }

        public static LoadOrder<OblivionMod> ImportUsualLoadOrder(
            DirectoryPath dataFolder,
            GroupMask? importMask = null,
            ModKey? modKeyExclusionHint = null,
            bool allowMissingMods = false)
        {
            var loadOrderListing = LoadOrder.GetUsualLoadOrder(GameRelease.Oblivion, dataFolder, allowMissingMods: allowMissingMods);
            if (modKeyExclusionHint != null)
            {
                loadOrderListing.Remove(modKeyExclusionHint.Value);
            }
            var loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Import(
                dataFolder,
                loadOrderListing,
                importer: (FilePath path, ModKey modKey, out OblivionMod mod) =>
                {
                    mod = OblivionMod.CreateFromBinary(
                        path.Path,
                        modKey,
                        importMask: importMask);
                    return true;
                });
            return loadOrder;
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
                if (mod.Mod == null) continue;
                ret.AddRecords(mod.Mod);
            }
            return ret;
        }
    }
}
