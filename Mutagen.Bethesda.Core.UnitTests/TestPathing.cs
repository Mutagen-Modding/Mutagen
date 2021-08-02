using System.IO;
using System.Runtime.CompilerServices;
using Mutagen.Bethesda.Plugins;
using Noggog.Utility;

namespace Mutagen.Bethesda.Core.UnitTests
{
    public static class TestPathing
    {
        public static ModPath OblivionTestMod = new ModPath(ModKey.FromNameAndExtension("test.esp"), "../../../Plugins/Records/Files/oblivion_test.esp");
        public static ModPath OblivionOverrideMod = new ModPath(ModKey.FromNameAndExtension("override.esp"), "../../../Plugins/Records/Files/oblivion_override.esp");
        public static ModPath SkyrimTestMod = ModPath.FromPath("../../../Plugins/Records/Files/skyrim_test.esp");
        public static ModPath SkyrimOverrideMod = ModPath.FromPath("../../../Plugins/Records/Files/skyrim_override.esp");
        
        public static readonly string TempFolderPath = "MutagenUnitTests";

        public static TempFolder GetTempFolder(string folderName, [CallerMemberName] string? testName = null)
        {
            return TempFolder.FactoryByAddedPath(Path.Combine(TempFolderPath, folderName, testName!));
        }
    }
}
