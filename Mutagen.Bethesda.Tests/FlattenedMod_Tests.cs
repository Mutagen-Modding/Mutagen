using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Tests;

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
        OblivionMod ret = new OblivionMod(new ModKey("Test", ModType.Plugin),
            OblivionRelease.Oblivion);
        foreach (var listing in loadOrder.ListedOrder)
        {
            ret.Npcs.RecordCache.Set(listing.Mod.Npcs.Records);
        }
    }
}