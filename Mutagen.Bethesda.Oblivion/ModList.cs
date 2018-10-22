using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public class ModList : ModList<OblivionMod>
    {
        public static async Task<TryGet<ModList>> TryImportUsualLoadOrder(
            DirectoryPath dataFolder, 
            GroupMask importMask = null,
            ModKey? modKeyExclusionHint = null)
        {
            if (!LoadOrder.TryGetUsualLoadOrder(dataFolder, out var loadOrder))
            {
                return TryGet<ModList>.Fail(null);
            }
            if (modKeyExclusionHint != null)
            {
                loadOrder.Remove(modKeyExclusionHint.Value);
            }
            var modList = new ModList();
            await modList.Import(
                dataFolder,
                loadOrder,
                importer: async (path, modKey) =>
                {
                    var mod = OblivionMod.Create_Binary(
                        path.Path,
                        modKey,
                        importMask: importMask);
                    return TryGet<OblivionMod>.Succeed(mod);
                });
            return TryGet<ModList>.Succeed(modList);
        }

        public static async Task<ModList> ImportUsualLoadOrder(
            DirectoryPath dataFolder, 
            GroupMask importMask = null,
            ModKey? modKeyExclusionHint = null)
        {
            var tg = await TryImportUsualLoadOrder(
                dataFolder,
                importMask,
                modKeyExclusionHint);
            return tg.Value;
        }
    }
}
