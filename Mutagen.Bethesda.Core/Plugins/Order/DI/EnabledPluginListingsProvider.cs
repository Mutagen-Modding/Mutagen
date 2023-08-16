using System.IO.Abstractions;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IEnabledPluginListingsProvider
{
    IEnumerable<ILoadOrderListingGetter> Get();
}

public sealed class EnabledPluginListingsProvider : IEnabledPluginListingsProvider, IListingsProvider
{
    private readonly IFileSystem _fileSystem;
    public IPluginRawListingsReader Reader { get; }
    public IPluginListingsPathContext PluginListingsPath { get; }

    public EnabledPluginListingsProvider(
        IFileSystem fileSystem,
        IPluginRawListingsReader reader,
        IPluginListingsPathContext pluginListingsPath)
    {
        _fileSystem = fileSystem;
        Reader = reader;
        PluginListingsPath = pluginListingsPath;
    }
        
    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        if (_fileSystem.File.Exists(PluginListingsPath.Path))
        {
            return Reader.Read(PluginListingsPath.Path);
        }
        else
        {
            return Enumerable.Empty<ILoadOrderListingGetter>();
        }
    }
}