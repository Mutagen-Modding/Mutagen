using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
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
    private readonly IProtonPrefixProvider _protonPrefixProvider;

    public IniPathLookup(
        IGameDirectoryLookup gameDirectoryLookup,
        IProtonPrefixProvider protonPrefixProvider)
    {
        _gameDirectoryLookup = gameDirectoryLookup;
        _protonPrefixProvider = protonPrefixProvider;
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
        if (envPath.IsNullOrWhitespace())
        {
            envPath = _protonPrefixProvider.TryGetProtonMyDocuments(release);
        }
        if (envPath.IsNullOrWhitespace()) return null;

        return Path.Combine(
            envPath,
            "My Games",
            docsString,
            ToIniFileName(release));
    }

    public FilePath Get(GameRelease release)
    {
        return TryGet(release)
               ?? throw new DirectoryNotFoundException("Could not find INI path for " + release);
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
