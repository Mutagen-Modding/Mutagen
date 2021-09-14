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
        public static ModPath SkyrimBodtLength8With42 = ModPath.FromPath("Skyrim/BodyTemplate/BodtLength8With42.esp");
        public static ModPath SkyrimBodtLength12With42 = ModPath.FromPath("Skyrim/BodyTemplate/BodtLength12With42.esp");
        public static ModPath SkyrimBodtLength8With43 = ModPath.FromPath("Skyrim/BodyTemplate/BodtLength8With43.esp");
        public static ModPath SkyrimBodtLength12With43 = ModPath.FromPath("Skyrim/BodyTemplate/BodtLength12With43Normal.esp");
        public static ModPath SkyrimBodtLength8With44 = ModPath.FromPath("Skyrim/BodyTemplate/BodtLength8With44.esp");
        public static ModPath SkyrimBodtLength12With44 = ModPath.FromPath("Skyrim/BodyTemplate/BodtLength12With44.esp");
        public static ModPath SkyrimBod2Length8With42 = ModPath.FromPath("Skyrim/BodyTemplate/Bod2Length8With42.esp");
        public static ModPath SkyrimBod2Length12With42 = ModPath.FromPath("Skyrim/BodyTemplate/Bod2Length12With42.esp");
        public static ModPath SkyrimBod2Length12With43 = ModPath.FromPath("Skyrim/BodyTemplate/Bod2Length12With43.esp");
        public static ModPath SkyrimBod2Length8With43 = ModPath.FromPath("Skyrim/BodyTemplate/Bod2Length8With43.esp");
        public static ModPath SkyrimBod2Length8With44 = ModPath.FromPath("Skyrim/BodyTemplate/Bod2Length8With44Normal.esp");
        public static ModPath SkyrimBod2Length12With44 = ModPath.FromPath("Skyrim/BodyTemplate/Bod2Length12With44.esp");

        public static MutagenFrame GetReadFrame(ModPath path, GameRelease release)
        {
            return new MutagenFrame(
                new MutagenBinaryReadStream(
                    File.OpenRead(path),
                    new ParsingBundle(
                        release,
                        new MasterReferenceReader("SomeMod.esp"))));
        }

        public static OverlayStream GetOverlayStream(ModPath path, GameRelease release)
        {
            return new OverlayStream(
                File.ReadAllBytes(path),
                new ParsingBundle(
                    GameConstants.Get(release),
                    new MasterReferenceReader(
                        path.ModKey)));
        }
    }
}