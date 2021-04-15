using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
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
                GameRelease.SkyrimEnderalLE => "Skyrim",
                GameRelease.SkyrimSE => "Skyrim Special Edition",
                GameRelease.SkyrimEnderalSE => "Skyrim Special Edition",
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
                GameRelease.SkyrimEnderalLE => "Skyrim",
                GameRelease.SkyrimEnderalSE => "Skyrim",
                GameRelease.SkyrimVR => "SkyrimVR",
                GameRelease.Fallout4 => "Fallout4",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
