using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters;

internal static class TransitiveMasterLocator
{
    public static IReadOnlyCollection<ModKey> GetAllMasters(
        GameRelease release,
        IEnumerable<ModKey> mods,
        DirectoryPath dataFolder,
        IFileSystem? fileSystem)
    {
        // Collect all transitive masters
        var masters = new HashSet<ModKey>();
        var remainingMasters = new Queue<ModKey>(mods);
          
        while (remainingMasters.Count > 0)
        {
            var master = remainingMasters.Dequeue();
            masters.Add(master);

            var modPath = Path.Combine(dataFolder, master.FileName);

            var masterMod = ModInstantiator.ImportGetter(modPath, release, new BinaryReadParameters()
            {
                FileSystem = fileSystem
            });

            foreach (var parent in masterMod.MasterReferences)
            {
                remainingMasters.Enqueue(parent.Master);
                masters.Add(parent.Master);
            }
        }

        return masters;
    }
}