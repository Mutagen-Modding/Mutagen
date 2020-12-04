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
    /// <summary>
    /// Some tests that are less about testing correct functionality, and more confirming
    /// that a specific API call is able to compile.
    /// </summary>
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
            IEnumerable<IModContext<ISkyrimMod, ICell, ICellGetter>> cells = listings.Cell().WinningContextOverrides(linkCache: null!);
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


        [Fact]
        public static void FormLink()
        {
            var mod = new SkyrimMod(Utility.LightMasterModKey, SkyrimRelease.SkyrimLE);
            var light = mod.Lights.AddNew();
            var cache = mod.ToImmutableLinkCache();

            var link = new FormLink<ISkyrimMajorRecordGetter>(light.FormKey);
            var nullableLink = new FormLinkNullable<ISkyrimMajorRecordGetter>(light.FormKey);
            IFormLink<ISkyrimMajorRecordGetter> iLink = link;

            // Normal resolution
            link.TryResolve(cache, out var _);
            link.TryResolve(cache, out ISkyrimMajorRecordGetter _);
            link.Resolve(cache);
            link.TryResolve<ILightGetter>(cache, out var _);
            link.TryResolve(cache, out ILightGetter _);
            link.Resolve<ILightGetter>(cache);

            nullableLink.TryResolve(cache, out var _);
            nullableLink.TryResolve(cache, out ISkyrimMajorRecordGetter _);
            nullableLink.Resolve(cache);
            nullableLink.TryResolve<ILightGetter>(cache, out var _);
            nullableLink.TryResolve(cache, out ILightGetter _);
            nullableLink.Resolve<ILightGetter>(cache);

            iLink.TryResolve(cache, out var _);
            iLink.Resolve(cache);
            iLink.TryResolve<ISkyrimMajorRecordGetter, ILightGetter>(cache, out var _);
            iLink.TryResolve(cache, out ILightGetter _);
            iLink.Resolve<ISkyrimMajorRecordGetter, ILightGetter>(cache);

            // Context resolution
            // ToDo
            // Enable when generic querying is supported
            //link.TryResolveContext<ISkyrimMod, ISkyrimMajorRecord>(cache, out var _);
            //link.TryResolveContext<ISkyrimMod, ISkyrimMajorRecord>(cache, out IModContext<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> _);
            //link.ResolveContext<ISkyrimMod, ISkyrimMajorRecord>(cache);
            link.TryResolveContext<ISkyrimMod, ILight, ILightGetter>(cache, out var _);
            link.TryResolveContext(cache, out IModContext<ISkyrimMod, ILight, ILightGetter> _);
            link.ResolveContext<ISkyrimMod, ILight, ILightGetter>(cache);

            //nullableLink.TryResolveContext<ISkyrimMod, ISkyrimMajorRecord>(cache, out var _);
            //nullableLink.TryResolveContext<ISkyrimMod, ISkyrimMajorRecord>(cache, out IModContext<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> _);
            //nullableLink.ResolveContext<ISkyrimMod, ISkyrimMajorRecord>(cache);
            nullableLink.TryResolveContext<ISkyrimMod, ILight, ILightGetter>(cache, out var _);
            nullableLink.TryResolveContext(cache, out IModContext<ISkyrimMod, ILight, ILightGetter> _);
            nullableLink.ResolveContext<ISkyrimMod, ILight, ILightGetter>(cache);

            //iLink.TryResolveContext<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, out var _);
            //iLink.ResolveContext<ISkyrimMod, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache);
            iLink.TryResolveContext<ISkyrimMod, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache, out var _);
            iLink.TryResolveContext(cache, out IModContext<ISkyrimMod, ILight, ILightGetter> _);
            iLink.ResolveContext<ISkyrimMod, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache);
        }

        [Fact]
        public static void LoadOrderTryGetValue()
        {
            var lo = new LoadOrder<ISkyrimModGetter>();
            lo.TryGetValue(Utility.LightMasterModKey, out var item);
        }

        [Fact]
        public static void GroupAccessors()
        {
            var mod = new SkyrimMod(Utility.LightMasterModKey, SkyrimRelease.SkyrimSE);
            var group = new Group<Npc>(mod);
            if (group.TryGetValue(Utility.Form1, out var npc))
            {
            }
        }
    }
}
