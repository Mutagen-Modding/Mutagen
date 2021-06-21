using System.IO.Abstractions;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginPathProviderFactory
    {
        IPluginPathProvider Create(GameRelease release);
    }

    public class PluginPathProviderFactory : IPluginPathProviderFactory
    {
        private readonly IFileSystem _fileSystem;

        public PluginPathProviderFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public IPluginPathProvider Create(GameRelease release)
        {
            return new PluginPathProvider(
                _fileSystem,
                release);
        }
    }
}