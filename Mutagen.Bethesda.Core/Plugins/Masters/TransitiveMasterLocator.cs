using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
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

            var modPath = new ModPath(Path.Combine(dataFolder, master.FileName));

            var header = ModHeaderFrame.FromPath(modPath, release, fileSystem: fileSystem);
            
            foreach (var parent in header.Masters(modPath.ModKey))
            {
                remainingMasters.Enqueue(parent.Master);
                masters.Add(parent.Master);
            }
        }

        return masters;
    }
}