using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginRawListingsReader
{
    IEnumerable<ILoadOrderListingGetter> Read(FilePath pluginPath);
}

public class PluginRawListingsReader : IPluginRawListingsReader
{
    private readonly IFileSystem _fileSystem;
    public IPluginListingsParser Parser { get; }

    public PluginRawListingsReader(
        IFileSystem fileSystem,
        IPluginListingsParser parser)
    {
        _fileSystem = fileSystem;
        Parser = parser;
    }
        
    public IEnumerable<ILoadOrderListingGetter> Read(FilePath pluginPath)
    {
        if (!_fileSystem.File.Exists(pluginPath))
        {
            throw new FileNotFoundException("Could not locate plugins file", pluginPath);
        }
        using var stream = _fileSystem.FileStream.Create(pluginPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Parser.Parse(stream).ToList();
    }
}