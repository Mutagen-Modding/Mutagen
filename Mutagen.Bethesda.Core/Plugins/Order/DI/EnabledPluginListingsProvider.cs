namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IEnabledPluginListingsProvider
{
    IEnumerable<ILoadOrderListingGetter> Get();
}

public sealed class EnabledPluginListingsProvider : IEnabledPluginListingsProvider, IListingsProvider
{
    public IPluginRawListingsReader Reader { get; }
    public IPluginListingsPathContext PluginListingsPath { get; }

    public EnabledPluginListingsProvider(
        IPluginRawListingsReader reader,
        IPluginListingsPathContext pluginListingsPath)
    {
        Reader = reader;
        PluginListingsPath = pluginListingsPath;
    }
        
    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        return Reader.Read(PluginListingsPath.Path);
    }
}