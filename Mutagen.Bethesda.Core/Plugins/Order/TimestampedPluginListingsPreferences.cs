namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ITimestampedPluginListingsPreferences
    {
        bool ThrowOnMissingMods { get; }
    }

    public class TimestampedPluginListingsPreferences : ITimestampedPluginListingsPreferences
    {
        public bool ThrowOnMissingMods { get; init; }
    }
}