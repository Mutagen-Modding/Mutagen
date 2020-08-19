using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.UnitTests
{
    public static class Utility
    {
        public static readonly ModKey ModKey = new ModKey("MutagenDummyKey", ModType.Plugin);
        public static readonly ModKey ModKey2 = new ModKey("MutagenDummyKey2", ModType.Plugin);
        public static readonly ModKey ModKey3 = new ModKey("MutagenDummyKey3", ModType.Plugin);
        public static readonly ModKey ModKey4 = new ModKey("MutagenDummyKey4", ModType.Plugin);
        public static readonly ModKey Skyrim = new ModKey("Skyrim", ModType.Master);
        public static readonly ModKey Update = new ModKey("Update", ModType.Master);
        public static readonly ModKey Dawnguard = new ModKey("Dawnguard", ModType.Master);
        public static readonly ModKey Hearthfires = new ModKey("HearthFires", ModType.Master);
        public static readonly ModKey Dragonborn = new ModKey("Dragonborn", ModType.Master);
        public static readonly string Edid1 = "AnEdid1";
        public static readonly string Edid2 = "AnEdid2";
        public static readonly string Edid3 = "AnEdid2";
        public static readonly FormKey Form1 = new FormKey(ModKey, 0x123456);
        public static readonly FormKey Form2 = new FormKey(ModKey, 0x12345F);
        public static readonly FormKey Form3 = new FormKey(ModKey, 0x223456);
        public static readonly string TempFolderPath = "MutagenUnitTests";
        public static ModPath OblivionTestMod = new ModPath(ModKey.FromNameAndExtension("test.esp"), "../../../oblivion_test.esp");
        public static ModPath OblivionOverrideMod = new ModPath(ModKey.FromNameAndExtension("override.esp"), "../../../oblivion_override.esp");
        public static ModPath SkyrimTestMod = new ModPath(ModKey.FromNameAndExtension("test.esp"), "../../../skyrim_test.esp");
    }
}
