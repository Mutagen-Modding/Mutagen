using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda;
using Noggog;
using Xunit;
using Xunit.Abstractions;
using Constants = Mutagen.Bethesda.Plugins.Internals.Constants;
using Mutagen.Bethesda.UnitTests;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.UnitTests.Plugins.Cache;

namespace Mutagen.Bethesda.UnitTests.Api
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
            npc.Race.SetTo(race);
            Assert.True(npc.Race.TryResolve(cache, out var _));
        }

        [Fact]
        public static void FormLinkListCovariance()
        {
            SkyrimMod sourceMod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            Armor armor = sourceMod.Armors.AddNew();

            void Tester(IReadOnlyList<IFormLinkGetter<IKeywordGetter>> tester)
            {
            }

            armor.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
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
            armor.Keywords = new ExtendedList<IFormLinkGetter<IKeywordGetter>>();
            var test = armor.Keywords;
            test.Add(key);
            test.Add(keyword);
        }

        [Fact]
        public static void FormLinkSetToNull()
        {
            var cameraShot = new CameraShot(Utility.Form1, SkyrimRelease.SkyrimSE);
            cameraShot.ImageSpaceModifier.Clear();
            cameraShot.ImageSpaceModifier.SetTo(FormKey.Null);
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

            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            IKeyworded<IKeywordGetter> keyworded = mod.Armors.AddNew();
            keyworded.TryResolveKeyword(Utility.Form2, mod.ToImmutableLinkCache(), out var keyword);
        }

        [Fact]
        public static void TypeSolidifier()
        {
            IEnumerable<IModListingGetter<ISkyrimModGetter>> listings = Enumerable.Empty<IModListingGetter<ISkyrimModGetter>>();
            IEnumerable<IAmmunitionGetter> ammun = listings.Ammunition().WinningOverrides();
            IEnumerable<IPlacedGetter> placed = listings.IPlaced().WinningOverrides();
            IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>> cells = listings.Cell().WinningContextOverrides(linkCache: null!);
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
            Assert.True(EnumExt.HasFlag(disabledObj.MajorRecordFlagsRaw, Constants.InitiallyDisabled));
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
            IPlaced abstractPlaced = placedTrap;
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
            IFormLinkGetter<ISkyrimMajorRecordGetter> iLink = link;

            // Normal resolution
            link.TryResolve(cache, out var _);
            link.TryResolve<ISkyrimMajorRecordGetter>(cache, out var _);
            link.Resolve(cache);
            link.TryResolve<ILightGetter>(cache, out var _);
            link.TryResolve(cache, out ILightGetter _);
            link.Resolve<ILightGetter>(cache);

            nullableLink.TryResolve(cache, out var _);
            nullableLink.TryResolve<ISkyrimMajorRecordGetter>(cache, out var _);
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
            link.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, out var _);
            link.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, out IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> _);
            link.ResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache);
            link.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache, out var _);
            link.TryResolveContext(cache, out IModContext<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter> _);
            link.ResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache);

            nullableLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, out var _);
            nullableLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, out IModContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter> _);
            nullableLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache);
            nullableLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache, out var _);
            nullableLink.TryResolveContext(cache, out IModContext<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter> _);
            nullableLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache);

            iLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache, out var _);
            iLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(cache);
            iLink.TryResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache, out var _);
            iLink.TryResolveContext(cache, out IModContext<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter> _);
            iLink.ResolveContext<ISkyrimMod, ISkyrimModGetter, ISkyrimMajorRecordGetter, ILight, ILightGetter>(cache);

            // Getter interface conversion
            IPerkGetter getter = new Perk(Utility.Form1, SkyrimRelease.SkyrimLE);
            Perk direct = new Perk(Utility.Form2, SkyrimRelease.SkyrimLE);
            IPerk setter = new Perk(Utility.Form2, SkyrimRelease.SkyrimLE);
            IFormLink<IPerkGetter> formLink = new FormLink<IPerkGetter>();
            formLink = getter.AsLink();
            formLink = direct.AsLink();
            formLink = setter.AsLink();
            formLink.SetTo(direct);
            formLink.SetTo(getter);
            formLink.SetTo(setter);
            formLink.SetTo(formLink);

            IObjectEffectGetter objGetter = null!;
            IFormLink<IEffectRecordGetter> aLink = new FormLink<IEffectRecordGetter>();
            aLink.SetTo(objGetter);

            IFormLink<ISkyrimMajorRecordGetter> majRecordLink = new FormLink<ISkyrimMajorRecordGetter>();
            IFormLink<IKeywordGetter> keywordLink = new FormLink<IKeywordGetter>();
            majRecordLink.SetTo(keywordLink);
            majRecordLink.TryResolve<IKeywordGetter>(cache, out var keyw);
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

        [Fact]
        public static void ImplicitsApi()
        {
            Implicits.Listings.Skyrim(SkyrimRelease.SkyrimSE).Contains(Utility.PluginModKey);
        }

        public static void LoadOrderOnlyEnabledAndExisting()
        {
            ILoadOrderGetter<IModListing<ISkyrimMod>>? lo = null!;
            IModListing<ISkyrimMod>[]? test = lo?.PriorityOrder.OnlyEnabledAndExisting().ToArray();
        }
    }
}
