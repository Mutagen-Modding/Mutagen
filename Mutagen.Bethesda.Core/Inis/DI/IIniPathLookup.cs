using Noggog;

namespace Mutagen.Bethesda.Inis.DI;

public interface IIniPathLookup
{
    FilePath Get(GameRelease release);
}

public class IniPathLookup : IIniPathLookup
{
    public FilePath Get(GameRelease release)
    {
        var docsString = ToMyDocumentsString(release);
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
            GameRelease.SkyrimSEGog => "Skyrim Special Edition GOG",
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
            GameRelease.SkyrimSEGog => "Skyrim",
            GameRelease.EnderalLE => "Enderal",
            GameRelease.EnderalSE => "Enderal",
            GameRelease.SkyrimVR => "SkyrimVR",
            GameRelease.Fallout4 => "Fallout4",
            _ => throw new NotImplementedException(),
        };
    }
}