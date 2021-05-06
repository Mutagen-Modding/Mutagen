using System;
using System.IO;

namespace Mutagen.Bethesda.Inis
{
    // Private until API can be made more mature
    class Ini
    {
        public static string GetTypicalPath(GameRelease release)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", ToMyDocumentsString(release), $"{ToIniName(release)}.ini");
        }

        public static string ToMyDocumentsString(GameRelease release)
        {
            return release switch
            {
                GameRelease.Oblivion => "Oblivion",
                GameRelease.SkyrimLE => "Skyrim",
                GameRelease.EnderalLE => "Skyrim",
                GameRelease.SkyrimSE => "Skyrim Special Edition",
                GameRelease.EnderalSE => "Skyrim Special Edition",
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
                GameRelease.EnderalLE => "Skyrim",
                GameRelease.EnderalSE => "Skyrim",
                GameRelease.SkyrimVR => "SkyrimVR",
                GameRelease.Fallout4 => "Fallout4",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
