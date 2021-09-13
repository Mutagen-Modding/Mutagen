using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Testing
{
    public class TestDataPathing
    {
        public static ModPath OblivionTestMod = new ModPath(ModKey.FromNameAndExtension("test.esp"), "Oblivion/oblivion_test.esp");
        public static ModPath OblivionOverrideMod = new ModPath(ModKey.FromNameAndExtension("override.esp"), "Oblivion/oblivion_override.esp");
        public static ModPath SkyrimTestMod = ModPath.FromPath("Skyrim/skyrim_test.esp");
        public static ModPath SkyrimOverrideMod = ModPath.FromPath("Skyrim/skyrim_override.esp");
        public static ModPath SizeOverflow = ModPath.FromPath("Skyrim/size_overflow_test.esp");
        public static ModPath SubObjectSizeOverflow = ModPath.FromPath("Skyrim/subobject_size_overflow_test.esp");
        public static ModPath RaceHeadPartDanglingMaster = ModPath.FromPath("Skyrim/RaceHeadPartDanglingMarker.esp");
        public static ModPath SkyrimPerkFunctionParametersTypeNone = ModPath.FromPath("Skyrim/SkyrimPerkFunctionParametersTypeNone.esp");
    }
}