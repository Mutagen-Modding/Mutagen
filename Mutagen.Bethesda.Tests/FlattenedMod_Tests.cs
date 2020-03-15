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
            List<ModKey> loadOrderListing = new List<ModKey>()
            {
                new ModKey("Oblivion", master: true),
                new ModKey("Knights", master: false)
            };
            LoadOrder<OblivionMod> loadOrder = new LoadOrder<OblivionMod>();
            await loadOrder.Import(
                dataFolder: testingSettings.DataFolderLocations.Oblivion,
                loadOrder: loadOrderListing,
                importer: async (filePath, modKey) => TryGet<OblivionMod>.Succeed(await OblivionMod.CreateFromBinary(filePath.Path, modKey)));
            OblivionMod ret = new OblivionMod(new ModKey("Test", master: false));
            foreach (var mod in loadOrder)
            {
                ret.Npcs.RecordCache.Set(mod.Mod.Npcs.Records);
            }
        }
    }
}
