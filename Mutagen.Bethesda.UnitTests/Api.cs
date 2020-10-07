using Mutagen.Bethesda.Skyrim;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Api
    {
        /// <summary>
        /// This API test is related to #106 on github
        /// Want to confirm that typical given FormLinks can resolve even if backing mod isn't a setter mod.
        /// </summary>
        [Fact]
        public static void TypicalLinksLocate()
        {
            SkyrimMod sourceMod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
            var race = sourceMod.Races.AddNew();
            using var cleanup = Linking_ImmutableOverlay_Tests.ConvertModToOverlay(sourceMod, out var sourceModGetter);
            var cache = sourceModGetter.ToImmutableLinkCache();
            var otherMod = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var npc = otherMod.Npcs.AddNew();
            npc.Race = race;
            Assert.True(npc.Race.TryResolve(cache, out var _));
        }

        [Fact]
        public static void FormLinkListCovariance()
        {
            SkyrimMod sourceMod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            SkyrimMod sourceMod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cameraShot = new CameraShot(Utility.Form1);
            cameraShot.ImageSpaceModifier = default;
            cameraShot.ImageSpaceModifier = FormKey.Null;
        }

        [Fact]
        public static void IKeyworded()
        {
            void TestFunction<T>()
                where T : IKeywordedGetter<Mutagen.Bethesda.Skyrim.IKeywordGetter>
            {
            }
            void TestFunction2<T>()
                where T : IKeywordedGetter
            {
            }
            TestFunction<IWeaponGetter>();
            TestFunction2<IWeaponGetter>();
        }
    }
}
