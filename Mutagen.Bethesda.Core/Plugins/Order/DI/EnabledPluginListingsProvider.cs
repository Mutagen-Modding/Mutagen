using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface IEnabledPluginListingsProvider
    {
        IEnumerable<IModListingGetter> Get();
    }

    public class EnabledPluginListingsProvider : IEnabledPluginListingsProvider, IListingsProvider
    {
        private readonly IPluginRawListingsReader _reader;
        private readonly IPluginListingsPathProvider _pluginListingsPath;

        public EnabledPluginListingsProvider(
            IPluginRawListingsReader reader,
            IPluginListingsPathProvider pluginListingsPath)
        {
            _reader = reader;
            _pluginListingsPath = pluginListingsPath;
        }
        
        public IEnumerable<IModListingGetter> Get()
        {
            return _reader.Read(_pluginListingsPath.Path);
        }
    }
}