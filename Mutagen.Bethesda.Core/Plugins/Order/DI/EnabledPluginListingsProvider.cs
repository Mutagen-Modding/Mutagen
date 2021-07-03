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
        private readonly IPluginPathContext _pluginPath;

        public EnabledPluginListingsProvider(
            IPluginRawListingsReader reader,
            IPluginPathContext pluginPath)
        {
            _reader = reader;
            _pluginPath = pluginPath;
        }
        
        public IEnumerable<IModListingGetter> Get()
        {
            return _reader.Read(_pluginPath.Path);
        }
    }
}