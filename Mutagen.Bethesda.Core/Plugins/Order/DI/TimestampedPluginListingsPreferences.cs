namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ITimestampedPluginListingsPreferences
{
    bool ThrowOnMissingMods { get; }
}

public class TimestampedPluginListingsPreferences : ITimestampedPluginListingsPreferences
{
    public bool ThrowOnMissingMods { get; init; }
}