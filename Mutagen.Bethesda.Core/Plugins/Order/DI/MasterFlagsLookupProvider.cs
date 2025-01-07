using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IMasterFlagsLookupProvider
{
    IReadOnlyCache<IModMasterStyledGetter, ModKey>? Get(IReadOnlyList<ILoadOrderListingGetter> listings);
}

public class MasterFlagsLookupProvider : IMasterFlagsLookupProvider
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public MasterFlagsLookupProvider(
        IGameReleaseContext gameReleaseContext,
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider)
    {
        _gameReleaseContext = gameReleaseContext;
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
    }

    public IReadOnlyCache<IModMasterStyledGetter, ModKey>? Get(IReadOnlyList<ILoadOrderListingGetter> listings)
    {
        if (!GameConstants.Get(_gameReleaseContext.Release).SeparateMasterLoadOrders) return null;

        var keyedMasterStyles = listings
            .Select(x => new ModPath(x.ModKey, _dataDirectoryProvider.Path.GetFile(x.ModKey.FileName).Path))
            .AsParallel()
            .Select(x =>
            {
                if (_fileSystem.File.Exists(x))
                {
                    return KeyedMasterStyle.FromPath(x, _gameReleaseContext.Release, _fileSystem);
                }

                return null;
            })
            .ToArray();
        
        var cache = new Cache<IModMasterStyledGetter, ModKey>(x => x.ModKey);
        foreach (var style in keyedMasterStyles)
        {
            if (style == null) return null;
            cache.Add(style);
        }

        return cache;
    }
}