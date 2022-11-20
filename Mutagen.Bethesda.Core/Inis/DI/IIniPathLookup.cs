using Noggog;

namespace Mutagen.Bethesda.Inis.DI;

public interface IIniPathLookup
{
    FilePath Get(GameRelease release, GameInstallMode gameInstallMode);
}

public class IniPathLookup : IIniPathLookup
{
    public FilePath Get(GameRelease release, GameInstallMode gameInstallMode)
    {
        var docsString = ToMyDocumentsString(release);
        if (gameInstallMode == GameInstallMode.Gog)
        {
            docsString += " GOG";
        }
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            docsString, 
            $"{ToIniName(release)}.ini");
    }

    public static string ToMyDocumentsString(GameRelease release)
    {
        return release switch
        {
            GameRelease.Oblivion => "Oblivion",
            GameRelease.SkyrimLE => "Skyrim",
            GameRelease.EnderalLE => "Enderal",
            GameRelease.SkyrimSE => "Skyrim Special Edition",
            GameRelease.EnderalSE => "Enderal Special Edition",
            GameRelease.SkyrimVR => "Skyrim VR",
            GameRelease.Fallout4 => "Fallout4",
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
            GameRelease.EnderalLE => "Enderal",
            GameRelease.EnderalSE => "Enderal",
            GameRelease.SkyrimVR => "SkyrimVR",
            GameRelease.Fallout4 => "Fallout4",
            _ => throw new NotImplementedException(),
        };
    }
}