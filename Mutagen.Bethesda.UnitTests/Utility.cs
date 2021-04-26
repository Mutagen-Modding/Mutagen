using Mutagen.Bethesda.Plugins;
using Noggog.Utility;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mutagen.Bethesda.UnitTests
{
    public static class Utility
    {
        public static readonly ModKey PluginModKey = new ModKey("MutagenPluginKey", ModType.Plugin);
        public static readonly ModKey PluginModKey2 = new ModKey("MutagenPluginKey2", ModType.Plugin);
        public static readonly ModKey PluginModKey3 = new ModKey("MutagenPluginKey3", ModType.Plugin);
        public static readonly ModKey PluginModKey4 = new ModKey("MutagenPluginKey4", ModType.Plugin);
        public static readonly ModKey MasterModKey = new ModKey("MutagenMasterKey", ModType.Master);
        public static readonly ModKey MasterModKey2 = new ModKey("MutagenMasterKey2", ModType.Master);
        public static readonly ModKey MasterModKey3 = new ModKey("MutagenMasterKey3", ModType.Master);
        public static readonly ModKey MasterModKey4 = new ModKey("MutagenMasterKey4", ModType.Master);
        public static readonly ModKey LightMasterModKey = new ModKey("MutagenLightMasterKey", ModType.LightMaster);
        public static readonly ModKey LightMasterModKey2 = new ModKey("MutagenLightMasterKey2", ModType.LightMaster);
        public static readonly ModKey LightMasterModKey3 = new ModKey("MutagenLightMasterKey3", ModType.LightMaster);
        public static readonly ModKey LightMasterModKey4 = new ModKey("MutagenLightMasterKey4", ModType.LightMaster);
        public static readonly ModKey Skyrim = new ModKey("Skyrim", ModType.Master);
        public static readonly ModKey Update = new ModKey("Update", ModType.Master);
        public static readonly ModKey Dawnguard = new ModKey("Dawnguard", ModType.Master);
        public static readonly ModKey Hearthfires = new ModKey("HearthFires", ModType.Master);
        public static readonly ModKey Dragonborn = new ModKey("Dragonborn", ModType.Master);
        public static readonly string Edid1 = "AnEdid1";
        public static readonly string Edid2 = "AnEdid2";
        public static readonly string Edid3 = "AnEdid2";
        public static readonly string UnusedEdid = "UnusedEdid";
        public static readonly FormKey Form1 = new FormKey(PluginModKey, 0x123456);
        public static readonly FormKey Form2 = new FormKey(PluginModKey, 0x12345F);
        public static readonly FormKey Form3 = new FormKey(PluginModKey, 0x223456);
        public static readonly FormKey Form4 = new FormKey(PluginModKey, 0x22345F);
        public static readonly FormKey UnusedForm = new FormKey(PluginModKey, 0x323456);
        public static readonly string TempFolderPath = "MutagenUnitTests";
        public static ModPath OblivionTestMod = new ModPath(ModKey.FromNameAndExtension("test.esp"), "../../../oblivion_test.esp");
        public static ModPath OblivionOverrideMod = new ModPath(ModKey.FromNameAndExtension("override.esp"), "../../../oblivion_override.esp");
        public static ModPath SkyrimTestMod = ModPath.FromPath("../../../skyrim_test.esp");
        public static ModPath SkyrimOverrideMod = ModPath.FromPath("../../../skyrim_override.esp");

        public static TempFolder GetTempFolder(string folderName, [CallerMemberName] string? testName = null)
        {
            return TempFolder.FactoryByAddedPath(Path.Combine(Utility.TempFolderPath, folderName, testName!));
        }
    }
}
