using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

internal static class TransitiveMasterLocator
{
    public static IReadOnlyCollection<ModKey> GetAllMasters(
        GameRelease release,
        ModKey self,
        IEnumerable<ModKey> mods,
        IReadOnlyCache<IModListingGetter<IModGetter>, ModKey>? alreadyLocatedMods,
        DirectoryPath? dataFolder,
        IFileSystem? fileSystem)
    {
        // Collect all transitive masters
        var masters = new HashSet<ModKey>();
        var remainingMasters = new Queue<ModKey>(mods);
          
        while (remainingMasters.Count > 0)
        {
            var master = remainingMasters.Dequeue();
            masters.Add(master);

            IEnumerable<ModKey> mastersOfMod;
            if (alreadyLocatedMods != null 
                && alreadyLocatedMods.TryGetValue(master, out var locatedMod)
                && locatedMod.Mod != null)
            {
                mastersOfMod = locatedMod.Mod.MasterReferences
                    .Select(x => x.Master)
                    .ToArray();
            }
            else
            {
                if (dataFolder == null)
                {
                    throw new ArgumentNullException("Data folder source was not set");
                }
                var modPath = new ModPath(Path.Combine(dataFolder, master.FileName));
                var header = ModHeaderFrame.FromPath(modPath, release, fileSystem: fileSystem);
                mastersOfMod = header.Masters(master).Select(x => x.Master).ToArray();
            }
            
            foreach (var parent in mastersOfMod)
            {
                var masterKey = parent;

                if (masterKey != self && !masters.Add(parent))
                {
                    remainingMasters.Enqueue(parent);
                }
            }
        }

        return masters;
    }
}