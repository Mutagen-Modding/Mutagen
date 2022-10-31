using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ITimestampedPluginListingsProvider : IListingsProvider
{
}

public sealed class TimestampedPluginListingsProvider : ITimestampedPluginListingsProvider
{
    public ITimestampAligner Aligner { get; }
    public ITimestampedPluginListingsPreferences Prefs { get; }
    public IPluginRawListingsReader RawListingsReader { get; }
    public IDataDirectoryProvider DirectoryProvider { get; }
    public IPluginListingsPathContext ListingsPathContext { get; }

    public TimestampedPluginListingsProvider(
        ITimestampAligner timestampAligner,
        ITimestampedPluginListingsPreferences prefs,
        IPluginRawListingsReader rawListingsReader,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathContext pluginListingsPathContext)
    {
        Aligner = timestampAligner;
        Prefs = prefs;
        RawListingsReader = rawListingsReader;
        DirectoryProvider = dataDirectoryProvider;
        ListingsPathContext = pluginListingsPathContext;
    }

    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        var mods = RawListingsReader.Read(ListingsPathContext.Path);
        return Aligner.AlignToTimestamps(
            mods,
            DirectoryProvider.Path, 
            throwOnMissingMods: Prefs.ThrowOnMissingMods);
    }
}