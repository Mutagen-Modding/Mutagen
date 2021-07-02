using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IEnabledPluginListingsProvider
    {
        IEnumerable<IModListingGetter> Get();
    }

    public class EnabledPluginListingsProvider : IEnabledPluginListingsProvider, IListingsProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPluginListingsParser _parser;
        private readonly IPluginPathContext _pluginPath;

        public EnabledPluginListingsProvider(
            IFileSystem fileSystem,
            IPluginListingsParser parser,
            IPluginPathContext pluginPath)
        {
            _fileSystem = fileSystem;
            _parser = parser;
            _pluginPath = pluginPath;
        }
        
        public IEnumerable<IModListingGetter> Get()
        {
            if (!_fileSystem.File.Exists(_pluginPath.Path))
            {
                throw new FileNotFoundException("Could not locate plugins file", _pluginPath.Path);
            }
            using var stream = _fileSystem.FileStream.Create(_pluginPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return _parser.Parse(stream).ToList();
        }
    }
}