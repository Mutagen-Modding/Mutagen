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
                new ModKey("Oblivion", ModType.Master),
                new ModKey("Knights", ModType.Plugin)
            };
            LoadOrder<OblivionMod> loadOrder = new LoadOrder<OblivionMod>();
            loadOrder.Import(
                dataFolder: testingSettings.DataFolderLocations.Oblivion,
                loadOrder: loadOrderListing,
                importer: (FilePath filePath, ModKey modKey, out OblivionMod mod) =>
                {
                    mod = OblivionMod.CreateFromBinary(filePath.Path, modKey);
                    return true;
                });
            OblivionMod ret = new OblivionMod(new ModKey("Test", ModType.Plugin));
            foreach (var mod in loadOrder)
            {
                ret.Npcs.RecordCache.Set(mod.Mod.Npcs.Records);
            }
        }
    }
}
