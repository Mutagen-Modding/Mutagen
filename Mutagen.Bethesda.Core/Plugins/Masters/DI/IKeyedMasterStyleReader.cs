using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

public interface IKeyedMasterStyleReader
{
    public KeyedMasterStyle ReadFrom(ModPath path);
}

public class KeyedMasterStyleReader : IKeyedMasterStyleReader
{
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly IFileSystem _fileSystem;
    
    public KeyedMasterStyleReader(
        IGameReleaseContext gameReleaseContext,
        IFileSystem fileSystem)
    {
        _gameReleaseContext = gameReleaseContext;
        _fileSystem = fileSystem;
    }

    public static KeyedMasterStyle ReadFrom(
        GameRelease release,
        ModPath modPath,
        IFileSystem? fileSystem = null)
    {
        return KeyedMasterStyle.FromPath(modPath, release, fileSystem);
    }
    
    KeyedMasterStyle IKeyedMasterStyleReader.ReadFrom(
        ModPath path)
    {
        return ReadFrom(_gameReleaseContext.Release, path, _fileSystem);
    }
}