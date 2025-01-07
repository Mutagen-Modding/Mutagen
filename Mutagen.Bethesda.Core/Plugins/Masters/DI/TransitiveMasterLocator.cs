using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

public interface ITransitiveMasterLocator
{
    IReadOnlyCollection<ModKey> GetAllMasters(
        ModKey self,
        IEnumerable<ModKey> mods);
    IReadOnlyCollection<ModKey> GetAllMasters(
        ModKey self,
        IEnumerable<ModKey> mods,
        IReadOnlyCache<IModListingGetter<IModGetter>, ModKey>? alreadyLocatedMods);
}

public class TransitiveMasterLocator : ITransitiveMasterLocator
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IGameReleaseContext _gameReleaseContext;
    
    public TransitiveMasterLocator(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IGameReleaseContext gameReleaseContext)
    {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        _gameReleaseContext = gameReleaseContext;
    }
    
    public IReadOnlyCollection<ModKey> GetAllMasters(
        ModKey self,
        IEnumerable<ModKey> mods)
    {
        return GetAllMasters(self, mods, alreadyLocatedMods: null);
    }
    
    public IReadOnlyCollection<ModKey> GetAllMasters(
        ModKey self,
        IEnumerable<ModKey> mods,
        IReadOnlyCache<IModListingGetter<IModGetter>, ModKey>? alreadyLocatedMods)
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
                var modPath = new ModPath(Path.Combine(_dataDirectoryProvider.Path, master.FileName));
                var header = ModHeaderFrame.FromPath(modPath, _gameReleaseContext.Release, fileSystem: _fileSystem);
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