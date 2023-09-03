using Mutagen.Bethesda.Environments.DI;
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
        var docsString = ToMyDocumentsString(release);
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            docsString, 
            ToIniFileName(release));
    }

    public static string ToMyDocumentsString(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => "Oblivion",
            GameRelease.SkyrimLE => "Skyrim",
            GameRelease.EnderalLE => "Enderal",
            GameRelease.SkyrimSE => "Skyrim Special Edition",
            GameRelease.SkyrimSEGog => "Skyrim Special Edition GOG",
            GameRelease.EnderalSE => "Enderal Special Edition",
            GameRelease.SkyrimVR => "Skyrim VR",
            GameRelease.Fallout4 => "Fallout4",
            GameRelease.Starfield => throw new ArgumentException("Starfield does not have ini in My Documents"),
            _ => throw new NotImplementedException(),
        };
    }

    public static string ToIniName(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => "Oblivion",
            GameRelease.SkyrimLE => "Skyrim",
            GameRelease.SkyrimSE => "Skyrim",
            GameRelease.SkyrimSEGog => "Skyrim",
            GameRelease.EnderalLE => "Enderal",
            GameRelease.EnderalSE => "Enderal",
            GameRelease.SkyrimVR => "SkyrimVR",
            GameRelease.Fallout4 => "Fallout4",
            GameRelease.Starfield => "Starfield",
            _ => throw new NotImplementedException(),
        };
    }

    public static string ToIniFileName(GameRelease release)
    {
        return $"{ToIniName(release)}.ini";
    }
}