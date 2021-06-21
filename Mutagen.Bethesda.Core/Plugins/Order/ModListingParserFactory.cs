namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IModListingParserFactory
    {
        IModListingParser Create(GameRelease release);
    }

    public class ModListingParserFactory : IModListingParserFactory
    {
        public IModListingParser Create(GameRelease release)
        {
            return new ModListingParser(PluginListings.HasEnabledMarkers(release));
        }
    }
}