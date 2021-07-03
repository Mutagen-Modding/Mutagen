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
        private readonly IDataDirectoryContext _dataDirectoryContext;
        private readonly IPluginPathContext _pluginPathContext;

        public TimestampedPluginListingsProvider(
            ITimestampAligner timestampAligner,
            ITimestampedPluginListingsPreferences prefs,
            IPluginRawListingsReader rawListingsReader,
            IDataDirectoryContext dataDirectoryContext,
            IPluginPathContext pluginPathContext)
        {
            _timestampAligner = timestampAligner;
            _prefs = prefs;
            _rawListingsReader = rawListingsReader;
            _dataDirectoryContext = dataDirectoryContext;
            _pluginPathContext = pluginPathContext;
        }

        public IEnumerable<IModListingGetter> Get()
        {
            var mods = _rawListingsReader.Read(_pluginPathContext.Path);
            return _timestampAligner.AlignToTimestamps(
                mods,
                _dataDirectoryContext.Path, 
                throwOnMissingMods: _prefs.ThrowOnMissingMods);
        }
    }
}