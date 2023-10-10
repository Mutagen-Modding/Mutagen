using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Inis.DI;

public interface IIniPathLookup
{
    FilePath Get(GameRelease release);
}

public class IniPathLookup : IIniPathLookup
{
    private readonly IGameDirectoryLookup _gameDirectoryLookup;

    public IniPathLookup(IGameDirectoryLookup gameDirectoryLookup)
    {
        _gameDirectoryLookup = gameDirectoryLookup;
    }
    
    public FilePath Get(GameRelease release)
    {
        if (release == GameRelease.Starfield)
        {
            return Path.Combine(_gameDirectoryLookup.Get(release), ToIniFileName(release));
        }
        var docsString = GameConstants.Get(release).MyDocumentsString;
        if (docsString == null)
        {
            throw new ArgumentException($"{release} does not have ini in My Documents");
        }
        
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            docsString, 
            ToIniFileName(release));
    }

    public static string ToIniFileName(GameRelease release)
    {
        return $"{GameConstants.Get(release).IniName}.ini";
    }
}