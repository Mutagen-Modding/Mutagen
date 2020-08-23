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
            List<LoadOrderListing> loadOrderListing = new List<LoadOrderListing>()
            {
                new ModKey("Oblivion", ModType.Master),
                new ModKey("Knights", ModType.Plugin)
            };
            var loadOrder = LoadOrder.Import<OblivionMod>(
                dataFolder: testingSettings.DataFolderLocations.Oblivion,
                loadOrder: loadOrderListing,
                gameRelease: GameRelease.Oblivion);
            OblivionMod ret = new OblivionMod(new ModKey("Test", ModType.Plugin));
            foreach (var listing in loadOrder.ListedOrder)
            {
                ret.Npcs.RecordCache.Set(listing.Mod.Npcs.Records);
            }
        }
    }
}
