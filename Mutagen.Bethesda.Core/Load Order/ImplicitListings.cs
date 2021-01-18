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
            "Fallout4.esm",
        };

        public static IEnumerable<ModKey> GetListings(GameRelease release)
        {
            return release switch
            {
                GameRelease.SkyrimSE => _sseImplicitMods,
                GameRelease.SkyrimVR => _sseImplicitMods,
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
