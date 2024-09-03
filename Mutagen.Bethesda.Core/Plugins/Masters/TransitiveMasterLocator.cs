using DynamicData;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

internal static class TransitiveMasterLocator
{
    public static IEnumerable<ModKey> GetAllMasters(
        IModGetter mod,
        DirectoryPath dataFolder,
        ILoadOrderGetter<IModFlagsGetter> loadOrder)
    {
        // Collect all transitive masters
        var masters = new HashSet<ModKey>();
        var remainingMasters = new Queue<ModKey>(
            mod.MasterReferences.Select(x => x.Master));
          
        while (remainingMasters.Count > 0)
        {
            var master = remainingMasters.Dequeue();
            masters.Add(master);
                        
            loadOrder.AssertListsMod(master);

            var modPath = Path.Combine(dataFolder, master.FileName);

            var masterMod = ModInstantiator.ImportGetter(modPath, mod.GameRelease);

            foreach (var parent in masterMod.MasterReferences)
            {
                remainingMasters.Enqueue(parent.Master);
                masters.Add(parent.Master);
            }
        }
        
        // Ensure the masters are in the same order as the load order
        
        // ToDo
        // Can probably be optimized off IndexOf
        var modKeyOrder = loadOrder.ListedOrder.Select(x => x.ModKey).ToArray();
        var sortedMasters = masters.OrderBy(m => modKeyOrder.IndexOf(m));

        return sortedMasters;
    }
}