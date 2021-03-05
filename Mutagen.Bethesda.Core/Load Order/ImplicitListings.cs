using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    public static class ImplicitListings
    {
        private readonly static ModKey[] _sseImplicitMods = new ModKey[]
        {
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm",
        };

        private readonly static ModKey[] _sseVrImplicitMods = new ModKey[]
        {
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm",
            "SkyrimVR.esm"
        };

        private readonly static ModKey[] _fo4ImplicitMods = new ModKey[]
        {
            "Fallout4.esm",
            "DLCRobot.esm",
            "DLCworkshop01.esm",
            "DLCCoast.esm",
            "DLCworkshop02.esm",
            "DLCworkshop03.esm",
            "DLCNukaWorld.esm",
        };

        public static IEnumerable<ModKey> GetListings(GameRelease release)
        {
            return release switch
            {
                GameRelease.SkyrimSE => _sseImplicitMods,
                GameRelease.SkyrimVR => _sseVrImplicitMods,
                GameRelease.Fallout4 => _fo4ImplicitMods,
                _ => Enumerable.Empty<ModKey>(),
            };
        }

        public static IEnumerable<ModKey> GetListings(GameRelease release, DirectoryPath dataPath)
        {
            return GetListings(release)
                .Where(x => File.Exists(Path.Combine(dataPath.Path, x.FileName)))
                .ToList();
        }
    }
}
