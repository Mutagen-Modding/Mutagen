namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginListingsParserFactory
    {
        IPluginListingsParser Create(GameRelease release);
    }

    public class PluginListingsParserFactory : IPluginListingsParserFactory
    {
        private readonly IModListingParserFactory _modListingParserFactory;

        public PluginListingsParserFactory(IModListingParserFactory modListingParserFactory)
        {
            _modListingParserFactory = modListingParserFactory;
        }
        
        public IPluginListingsParser Create(GameRelease release)
        {
            return new PluginListingsParser(release, _modListingParserFactory.Create(release));
        }
    }
}