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
        public static ModPath OblivionTestMod = new ModPath(ModKey.FromNameAndExtension("test.esp"), "Files/Oblivion/oblivion_test.esp");
        public static ModPath OblivionOverrideMod = new ModPath(ModKey.FromNameAndExtension("override.esp"), "Files/Oblivion/oblivion_override.esp");
        public static ModPath SkyrimTestMod = ModPath.FromPath("Files/Skyrim/skyrim_test.esp");
        public static ModPath SkyrimOverrideMod = ModPath.FromPath("Files/Skyrim/skyrim_override.esp");
        public static ModPath SizeOverflow = ModPath.FromPath("Files/Skyrim/size_overflow_test.esp");
        public static ModPath SubObjectSizeOverflow = ModPath.FromPath("Files/Skyrim/subobject_size_overflow_test.esp");
        public static ModPath RaceHeadPartDanglingMaster = ModPath.FromPath("Files/Skyrim/RaceHeadPartDanglingMarker.esp");
        public static ModPath SkyrimPerkFunctionParametersTypeNone = ModPath.FromPath("Files/Skyrim/SkyrimPerkFunctionParametersTypeNone.esp");
        public static string SkyrimBodtLength8With42 = "Files/Skyrim/BodyTemplate/BodtLength8With42.esp";
        public static string SkyrimBodtLength12With42 = "Files/Skyrim/BodyTemplate/BodtLength12With42.esp";
        public static string SkyrimBodtLength8With43 = "Files/Skyrim/BodyTemplate/BodtLength8With43.esp";
        public static string SkyrimBodtLength12With43 = "Files/Skyrim/BodyTemplate/BodtLength12With43Normal.esp";
        public static string SkyrimBodtLength8With44 = "Files/Skyrim/BodyTemplate/BodtLength8With44.esp";
        public static string SkyrimBodtLength12With44 = "Files/Skyrim/BodyTemplate/BodtLength12With44.esp";
        public static string SkyrimBod2Length8With42 = "Files/Skyrim/BodyTemplate/Bod2Length8With42.esp";
        public static string SkyrimBod2Length12With42 = "Files/Skyrim/BodyTemplate/Bod2Length12With42.esp";
        public static string SkyrimBod2Length12With43 = "Files/Skyrim/BodyTemplate/Bod2Length12With43.esp";
        public static string SkyrimBod2Length8With43 = "Files/Skyrim/BodyTemplate/Bod2Length8With43.esp";
        public static string SkyrimBod2Length8With44 = "Files/Skyrim/BodyTemplate/Bod2Length8With44Normal.esp";
        public static string SkyrimBod2Length12With44 = "Files/Skyrim/BodyTemplate/Bod2Length12With44.esp";
        public static string SkyrimBodtTypicalOutput = "Files/Skyrim/BodyTemplate/BodtTypicalOutput";
        public static string SkyrimBodtEmptyFlagsOutput = "Files/Skyrim/BodyTemplate/BodtEmptyFlagsOutput";
        public static string SkyrimBod2TypicalOutput = "Files/Skyrim/BodyTemplate/Bod2TypicalOutput";
        public static string SkyrimSoundRegionDataWithNoSecondSubrecord = "Files/Skyrim/Region/SoundRegionDataWithNoSecondSubrecord.esp";
        public static string FrenchSeString = "Files/Core/Strings/FrenchSeString";
        public static string RussianLeString = "Files/Core/Strings/RussianLeString";
        public static string PrependedString = "Files/Core/Strings/PrependedString";
        public static string ZeroContentPrependedString = "Files/Core/Strings/ZeroContentPrependedString";
        public static string SkyrimPlacedObjectReflectedWaterMissingData = "Files/Skyrim/PlacedObjectReflectedWaterMissingData.esp";

        public static MutagenFrame GetReadFrame(ModPath path, GameRelease release, ModKey? modKey = null)
        {
            return new MutagenFrame(
                new MutagenBinaryReadStream(
                    File.OpenRead(path),
                    new ParsingBundle(
                        release,
                        new MasterReferenceCollection(modKey ?? path.ModKey))));
        }

        public static OverlayStream GetOverlayStream(ModPath path, GameRelease release, ModKey? modKey = null)
        {
            return new OverlayStream(
                File.ReadAllBytes(path),
                new ParsingBundle(
                    GameConstants.Get(release),
                    new MasterReferenceCollection(
                        modKey ?? path.ModKey)));
        }
    }
}