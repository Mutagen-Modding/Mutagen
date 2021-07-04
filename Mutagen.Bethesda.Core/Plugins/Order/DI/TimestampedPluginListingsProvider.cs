using System.Collections.Generic;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ITimestampedPluginListingsProvider : IListingsProvider
    {
    }

    public class TimestampedPluginListingsProvider : ITimestampedPluginListingsProvider
    {
        private readonly ITimestampAligner _timestampAligner;
        private readonly ITimestampedPluginListingsPreferences _prefs;
        private readonly IPluginRawListingsReader _rawListingsReader;
        private readonly IDataDirectoryProvider _dataDirectoryProvider;
        private readonly IPluginListingsPathProvider _pluginListingsPathProvider;

        public TimestampedPluginListingsProvider(
            ITimestampAligner timestampAligner,
            ITimestampedPluginListingsPreferences prefs,
            IPluginRawListingsReader rawListingsReader,
            IDataDirectoryProvider dataDirectoryProvider,
            IPluginListingsPathProvider pluginListingsPathProvider)
        {
            _timestampAligner = timestampAligner;
            _prefs = prefs;
            _rawListingsReader = rawListingsReader;
            _dataDirectoryProvider = dataDirectoryProvider;
            _pluginListingsPathProvider = pluginListingsPathProvider;
        }

        public IEnumerable<IModListingGetter> Get()
        {
            var mods = _rawListingsReader.Read(_pluginListingsPathProvider.Path);
            return _timestampAligner.AlignToTimestamps(
                mods,
                _dataDirectoryProvider.Path, 
                throwOnMissingMods: _prefs.ThrowOnMissingMods);
        }
    }
}