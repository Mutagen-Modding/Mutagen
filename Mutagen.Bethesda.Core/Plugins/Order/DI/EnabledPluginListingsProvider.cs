using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IEnabledPluginListingsProvider
{
    IEnumerable<IModListingGetter> Get();
}

public class EnabledPluginListingsProvider : IEnabledPluginListingsProvider, IListingsProvider
{
    public IPluginRawListingsReader Reader { get; }
    public IPluginListingsPathProvider PluginListingsPath { get; }

    public EnabledPluginListingsProvider(
        IPluginRawListingsReader reader,
        IPluginListingsPathProvider pluginListingsPath)
    {
        Reader = reader;
        PluginListingsPath = pluginListingsPath;
    }
        
    public IEnumerable<IModListingGetter> Get()
    {
        return Reader.Read(PluginListingsPath.Path);
    }
}