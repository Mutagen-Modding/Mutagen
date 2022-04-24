using Noggog;

namespace Mutagen.Bethesda.Inis;

// Private until API can be made more mature
class Ini
{
    public static FilePath GetTypicalPath(GameRelease release)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", ToMyDocumentsString(release), $"{ToIniName(release)}.ini");
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