using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IPluginRawListingsReader
{
    IEnumerable<ILoadOrderListingGetter> Read(FilePath pluginPath);
}

public sealed class PluginRawListingsReader : IPluginRawListingsReader
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
            throw new FileNotFoundException($"Could not locate plugins file: {pluginPath}", pluginPath);
        }
        using var stream = _fileSystem.FileStream.New(pluginPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Parser.Parse(stream).ToList();
    }
}