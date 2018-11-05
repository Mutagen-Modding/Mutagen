using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Tests
{
    public static class FlattenedMod_Tests
    {
        public static async Task Oblivion_FlattenMod(TestingSettings testingSettings)
        {
            List<ModKey> loadOrder = new List<ModKey>()
            {
                new ModKey("Oblivion", master: true),
                new ModKey("Knights", master: false)
            };
            ModList<OblivionMod> modList = new ModList<OblivionMod>();
            await modList.Import(
                dataFolder: testingSettings.DataFolder,
                loadOrder: loadOrder,
                importer: async (filePath, modKey) => TryGet<OblivionMod>.Succeed(OblivionMod.Create_Binary(filePath.Path, modKey)));
            OblivionMod ret = new OblivionMod(new ModKey("Test", master: false));
            foreach (var mod in modList)
            {
                ret.NPCs.Items.Set(mod.Mod.NPCs.Items.Items);
            }
            int wer = 23;
            wer++;
        }
    }
}
