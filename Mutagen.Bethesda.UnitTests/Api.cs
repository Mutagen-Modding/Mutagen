using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;
using Xunit.Abstractions;
using Constants = Mutagen.Bethesda.Internals.Constants;

namespace Mutagen.Bethesda.UnitTests
{
    public class Api
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Api(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// This API test is related to #106 on github
        /// Want to confirm that typical given FormLinks can resolve even if backing mod isn't a setter mod.
        /// </summary>
        [Fact]
        public static void TypicalLinksLocate()
        {
            SkyrimMod sourceMod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var race = sourceMod.Races.AddNew();
            using var cleanup = Linking_ImmutableOverlay_Tests.ConvertModToOverlay(sourceMod, out var sourceModGetter);
            var cache = sourceModGetter.ToImmutableLinkCache();
            var otherMod = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var npc = otherMod.Npcs.AddNew();
            npc.Race = race;
            Assert.True(npc.Race.TryResolve(cache, out var _));
        }

        [Fact]
        public static void FormLinkListCovariance()
        {
            SkyrimMod sourceMod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            Armor armor = sourceMod.Armors.AddNew();

            void Tester(IReadOnlyList<IFormLink<IKeywordGetter>> tester)
            {
            }

            armor.Keywords = new ExtendedList<IFormLink<IKeywordGetter>>();
            Tester(armor.Keywords);
        }

        /// <summary>
        /// This API test is related to #108 on github
        /// Want to ensure we can interact with FormLink lists without requiring users to create FormLinks themselves
        /// </summary>
        [Fact]
        public static void CleanFormLinkListAPI()
        {
            SkyrimMod sourceMod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            FormKey key = sourceMod.GetNextFormKey();
            Keyword keyword = sourceMod.Keywords.AddNew();
            Armor armor = sourceMod.Armors.AddNew();
            armor.Keywords = new ExtendedList<IFormLink<IKeywordGetter>>();
            var test = armor.Keywords;
            test.Add(key);
            test.Add(keyword);
        }

        [Fact]
        public static void FormLinkSetToNull()
        {
            var cameraShot = new CameraShot(Utility.Form1, SkyrimRelease.SkyrimSE);
            cameraShot.ImageSpaceModifier = default;
            cameraShot.ImageSpaceModifier = FormKey.Null;
        }

        [Fact]
        public static void IKeyworded()
        {
            void TestFunction<T>()
                where T : IKeywordedGetter<IKeywordGetter>
            {
            }
            void TestFunction2<T>()
                where T : IKeywordedGetter
            {
            }
            TestFunction<IWeaponGetter>();
            TestFunction2<IWeaponGetter>();
        }

        [Fact]
        public static void TypeSolidifier()
        {
            IEnumerable<IModListing<ISkyrimModGetter>> listings = Enumerable.Empty<IModListing<ISkyrimModGetter>>();
            IEnumerable<IAmmunitionGetter> ammun = listings.Ammunition().WinningOverrides();
            IEnumerable<IPlacedGetter> placed = listings.IPlaced().WinningOverrides();
            IEnumerable<ModContext<ISkyrimMod, ICell, ICellGetter>> cells = listings.Cell().WinningContextOverrides(linkCache: null!);
            IEnumerable<ISkyrimModGetter> mods = Enumerable.Empty<ISkyrimModGetter>();
            ammun = mods.Ammunition().WinningOverrides();
            placed = mods.IPlaced().WinningOverrides();
            cells = mods.Cell().WinningContextOverrides(linkCache: null!);
        }

        [Fact]
        public void DisableAPI()
        {
            // Some calls assuring the Disable() API is accessible and working.
            SkyrimMod sourceMod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            FormKey key = sourceMod.GetNextFormKey();
            PlacedObject placedObject = new PlacedObject(key, SkyrimRelease.SkyrimSE);

            // Simplistic Disable access and verification.
            PlacedObject disabledObj = placedObject;
            disabledObj.Disable();
            //_testOutputHelper.WriteLine($"{disabledPlacedObject.MajorRecordFlagsRaw}");
            Assert.True(EnumExt.HasFlag(disabledObj.MajorRecordFlagsRaw,Constants.InitiallyDisabled));
            MajorRecord majorRecord = placedObject;
            majorRecord.Disable();
            Assert.True(EnumExt.HasFlag(majorRecord.MajorRecordFlagsRaw, Constants.InitiallyDisabled));
            IMajorRecordCommon interfaceRecord = placedObject;
            interfaceRecord.Disable();
            Assert.True(EnumExt.HasFlag(interfaceRecord.MajorRecordFlagsRaw, Constants.InitiallyDisabled));
            IPlaced interfacePlaced = placedObject;
            interfacePlaced.Disable();
            Assert.True(EnumExt.HasFlag(interfacePlaced.MajorRecordFlagsRaw, Constants.InitiallyDisabled));

            // Sanity test both API are invokable under Placed context.
            PlacedTrap placedTrap = new PlacedTrap(key, SkyrimRelease.SkyrimSE);
            placedTrap.Disable(IPlaced.DisableType.DisableWithoutZOffset);
            interfacePlaced = placedTrap;
            interfacePlaced.Disable(IPlaced.DisableType.JustInitiallyDisabled);
            APlaced abstractPlaced = placedTrap;
            abstractPlaced.Disable();
            abstractPlaced.Disable(IPlaced.DisableType.SafeDisable);

            //Try any other object other than Placed (invoke MajorRecord.Disable() and see if it works)
            var armor = new Armor(key, SkyrimRelease.SkyrimSE);
            armor.Disable();
            Assert.True(EnumExt.HasFlag(armor.MajorRecordFlagsRaw, Constants.InitiallyDisabled));
        }

    }
}
