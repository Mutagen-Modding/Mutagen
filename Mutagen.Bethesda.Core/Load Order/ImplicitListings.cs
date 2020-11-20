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

        internal static IEnumerable<ModKey> GetImplicitMods(GameRelease release)
        {
            return release switch
            {
                GameRelease.SkyrimSE => _sseImplicitMods,
                GameRelease.SkyrimVR => _sseImplicitMods,
                _ => Enumerable.Empty<ModKey>(),
            };
        }

        internal static void AddImplicitMods(
            GameRelease release,
            DirectoryPath dataPath,
            IList<LoadOrderListing> loadOrder)
        {
            foreach (var implicitMod in GetImplicitMods(release).Reverse())
            {
                if (loadOrder.Any(x => x.ModKey == implicitMod)) continue;
                if (!File.Exists(Path.Combine(dataPath.Path, implicitMod.FileName))) continue;
                loadOrder.Insert(0, new LoadOrderListing(implicitMod, true));
            }
        }
    }
}
