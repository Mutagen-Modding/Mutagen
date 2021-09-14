using System.IO;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;

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
        public static string SkyrimBodtLength8With42 = "Skyrim/BodyTemplate/BodtLength8With42.esp";
        public static string SkyrimBodtLength12With42 = "Skyrim/BodyTemplate/BodtLength12With42.esp";
        public static string SkyrimBodtLength8With43 = "Skyrim/BodyTemplate/BodtLength8With43.esp";
        public static string SkyrimBodtLength12With43 = "Skyrim/BodyTemplate/BodtLength12With43Normal.esp";
        public static string SkyrimBodtLength8With44 = "Skyrim/BodyTemplate/BodtLength8With44.esp";
        public static string SkyrimBodtLength12With44 = "Skyrim/BodyTemplate/BodtLength12With44.esp";
        public static string SkyrimBod2Length8With42 = "Skyrim/BodyTemplate/Bod2Length8With42.esp";
        public static string SkyrimBod2Length12With42 = "Skyrim/BodyTemplate/Bod2Length12With42.esp";
        public static string SkyrimBod2Length12With43 = "Skyrim/BodyTemplate/Bod2Length12With43.esp";
        public static string SkyrimBod2Length8With43 = "Skyrim/BodyTemplate/Bod2Length8With43.esp";
        public static string SkyrimBod2Length8With44 = "Skyrim/BodyTemplate/Bod2Length8With44Normal.esp";
        public static string SkyrimBod2Length12With44 = "Skyrim/BodyTemplate/Bod2Length12With44.esp";
        public static string SkyrimBodtTypicalOutput = "Skyrim/BodyTemplate/BodtTypicalOutput";
        public static string SkyrimBodtEmptyFlagsOutput = "Skyrim/BodyTemplate/BodtEmptyFlagsOutput";
        public static string SkyrimBod2TypicalOutput = "Skyrim/BodyTemplate/Bod2TypicalOutput";

        public static MutagenFrame GetReadFrame(ModPath path, GameRelease release, ModKey? modKey = null)
        {
            return new MutagenFrame(
                new MutagenBinaryReadStream(
                    File.OpenRead(path),
                    new ParsingBundle(
                        release,
                        new MasterReferenceReader(modKey ?? path.ModKey))));
        }

        public static OverlayStream GetOverlayStream(ModPath path, GameRelease release, ModKey? modKey = null)
        {
            return new OverlayStream(
                File.ReadAllBytes(path),
                new ParsingBundle(
                    GameConstants.Get(release),
                    new MasterReferenceReader(
                        modKey ?? path.ModKey)));
        }
    }
}