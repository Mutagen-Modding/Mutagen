using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class DuplicateFrom_Tests
    {
        [Fact]
        public void DoNothing()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var npc = mod.Npcs.AddNew();
            mod.DuplicateFromOnlyReferenced(mod.ToMutableLinkCache(), Utility.PluginModKey2);
            mod.EnumerateMajorRecords().Should().HaveCount(1);
            mod.Npcs.Should().HaveCount(1);
            mod.Npcs.First().Should().BeSameAs(npc);
        }

        [Fact]
        public void SelfPass()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
                mod.DuplicateFromOnlyReferenced(mod.ToMutableLinkCache(), mod.ModKey);
            });
        }

        [Fact]
        public void TypicalExtraction()
        {
            var modToExtract = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var npc = modToExtract.Npcs.AddNew();
            var race = modToExtract.Races.AddNew();
            var unneededRace = modToExtract.Races.AddNew();

            var targetMod = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            targetMod.Npcs.Add(npc);
            var safeNpc = targetMod.Npcs.AddNew();
            safeNpc.Race = new FormLink<IRaceGetter>(race.FormKey);

            var linkCache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(modToExtract, targetMod);

            targetMod.DuplicateFromOnlyReferenced(linkCache, modToExtract.ModKey, out var mapping);
            targetMod.EnumerateMajorRecords().Should().HaveCount(3);
            targetMod.Npcs.Should().HaveCount(2);
            targetMod.Races.Should().HaveCount(1);
            targetMod.Npcs.Should().Contain(safeNpc);
            var newRaceFormKey = mapping[race.FormKey];
            var newRace = targetMod.Races.First();
            newRace.FormKey.Should().Be(newRaceFormKey);
            safeNpc.Race.FormKey.Should().Be(newRaceFormKey);
        }
    }
}
