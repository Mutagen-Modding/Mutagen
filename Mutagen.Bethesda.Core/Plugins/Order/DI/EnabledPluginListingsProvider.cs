namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IEnabledPluginListingsProvider
{
    IEnumerable<ILoadOrderListingGetter> Get();
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
        
    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        return Reader.Read(PluginListingsPath.Path);
    }
}