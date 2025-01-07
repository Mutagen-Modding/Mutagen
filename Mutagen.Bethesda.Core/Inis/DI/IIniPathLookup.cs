using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Inis.DI;

public interface IIniPathLookup
{
    FilePath Get(GameRelease release);
    FilePath? TryGet(GameRelease release);
}

public class IniPathLookup : IIniPathLookup
{
    private readonly IGameDirectoryLookup _gameDirectoryLookup;

    public IniPathLookup(IGameDirectoryLookup gameDirectoryLookup)
    {
        _gameDirectoryLookup = gameDirectoryLookup;
    }
    
    public FilePath? TryGet(GameRelease release)
    {
        var constants = GameConstants.Get(release);
        var docsString = constants.MyDocumentsString;
        if (docsString == null)
        {
            var gameDir = _gameDirectoryLookup.TryGet(release);
            if (gameDir == null) return null;
            
            return Path.Combine(gameDir, ToIniFileName(release));
        }

        var envPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (envPath.IsNullOrWhitespace()) return null;
        
        return Path.Combine(
            envPath,
            "My Games",
            docsString, 
            ToIniFileName(release));
    }
    
    public FilePath Get(GameRelease release)
    {
        var constants = GameConstants.Get(release);
        var docsString = constants.MyDocumentsString;
        if (docsString == null)
        {
            var gameDir = _gameDirectoryLookup.Get(release);
            return Path.Combine(gameDir, ToIniFileName(release));
        }

        var envPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (envPath.IsNullOrWhitespace())
        {
            throw new DirectoryNotFoundException("Could not find MyDocuments environment path");
        }
        
        return Path.Combine(
            envPath,
            "My Games",
            docsString, 
            ToIniFileName(release));
    }

    public static string ToIniFileName(GameRelease release)
    {
        return $"{GameConstants.Get(release).IniName}.ini";
    }
}

internal class IniPathLookupInjection : IIniPathLookup
{
    private readonly string _path;

    public IniPathLookupInjection(string path)
    {
        _path = path;
    }
    
    public FilePath Get(GameRelease release)
    {
        return _path;
    }

    public FilePath? TryGet(GameRelease release)
    {
        return _path;
    }
}