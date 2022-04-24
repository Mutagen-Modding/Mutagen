using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ITimestampedPluginListingsProvider : IListingsProvider
{
}

public class TimestampedPluginListingsProvider : ITimestampedPluginListingsProvider
{
    public ITimestampAligner Aligner { get; }
    public ITimestampedPluginListingsPreferences Prefs { get; }
    public IPluginRawListingsReader RawListingsReader { get; }
    public IDataDirectoryProvider DirectoryProvider { get; }
    public IPluginListingsPathProvider ListingsPathProvider { get; }

    public TimestampedPluginListingsProvider(
        ITimestampAligner timestampAligner,
        ITimestampedPluginListingsPreferences prefs,
        IPluginRawListingsReader rawListingsReader,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathProvider pluginListingsPathProvider)
    {
        Aligner = timestampAligner;
        Prefs = prefs;
        RawListingsReader = rawListingsReader;
        DirectoryProvider = dataDirectoryProvider;
        ListingsPathProvider = pluginListingsPathProvider;
    }

    public IEnumerable<IModListingGetter> Get()
    {
        var mods = RawListingsReader.Read(ListingsPathProvider.Path);
        return Aligner.AlignToTimestamps(
            mods,
            DirectoryProvider.Path, 
            throwOnMissingMods: Prefs.ThrowOnMissingMods);
    }
}