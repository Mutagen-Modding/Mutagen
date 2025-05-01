using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

public interface IMasterFlagsLookupCompiler
{
    Noggog.Cache<IModMasterStyledGetter, ModKey>? ConstructFor(ModPath path);
}

public class MasterFlagsLookupCompiler : IMasterFlagsLookupCompiler
{
    private readonly IFileSystem _fileSystem;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    
    public MasterFlagsLookupCompiler(
        IFileSystem fileSystem,
        IGameReleaseContext gameReleaseContext,
        IDataDirectoryProvider dataDirectoryProvider)
    {
        _fileSystem = fileSystem;
        _gameReleaseContext = gameReleaseContext;
        _dataDirectoryProvider = dataDirectoryProvider;
    }

    public Noggog.Cache<IModMasterStyledGetter, ModKey>? ConstructFor(ModPath path)
    {
        Noggog.Cache<IModMasterStyledGetter, ModKey>? masterFlagsLookup = null;
        if (GameConstants.Get(_gameReleaseContext.Release).SeparateMasterLoadOrders)
        {
            var dataDirectoryPath = _dataDirectoryProvider.Path;
            var header = ModHeaderFrame.FromPath(path, _gameReleaseContext.Release, _fileSystem);
            masterFlagsLookup = new Noggog.Cache<IModMasterStyledGetter, ModKey>(x => x.ModKey);
            foreach (var master in header.Masters(path.ModKey).Select(x => x.Master))
            {
                var otherPath = Path.Combine(dataDirectoryPath, master.FileName);
                var otherHeader = ModHeaderFrame.FromPath(otherPath, _gameReleaseContext.Release, _fileSystem);
                masterFlagsLookup.Add(new KeyedMasterStyle(master, otherHeader.MasterStyle));
            }
        }
        return masterFlagsLookup;
    }
}