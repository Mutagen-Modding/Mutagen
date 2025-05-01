using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class DuplicateFromTests : IClassFixture<LoquiUse>
{
    [Fact]
    public void DoNothing()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        mod.DuplicateFromOnlyReferenced(mod.ToMutableLinkCache(), TestConstants.PluginModKey2);
        mod.EnumerateMajorRecords().ShouldHaveCount(1);
        mod.Npcs.ShouldHaveCount(1);
        mod.Npcs.First().ShouldBeSameAs(npc);
    }

    [Fact]
    public void SelfPass()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            mod.DuplicateFromOnlyReferenced(mod.ToMutableLinkCache(), mod.ModKey);
        });
    }

    [Fact]
    public void TypicalExtraction()
    {
        var modToExtract = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = modToExtract.Npcs.AddNew();
        var race = modToExtract.Races.AddNew();
        race.CloseLootSound.SetTo(modToExtract.SoundDescriptors.AddNew());
        var unneededRace = modToExtract.Races.AddNew();

        var targetMod = new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimSE);
        targetMod.Npcs.Add(npc);
        var safeNpc = targetMod.Npcs.AddNew();
        safeNpc.Race = new FormLink<IRaceGetter>(race.FormKey);

        var linkCache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(modToExtract, targetMod);

        targetMod.DuplicateFromOnlyReferenced(linkCache, modToExtract.ModKey, out var mapping);
        targetMod.Npcs.ShouldHaveCount(2);
        targetMod.Races.ShouldHaveCount(1);
        targetMod.SoundDescriptors.ShouldHaveCount(1);
        targetMod.EnumerateMajorRecords().ShouldHaveCount(4);
        targetMod.Npcs.ShouldContain(safeNpc);
        var newRaceFormKey = mapping[race.FormKey];
        var newRace = targetMod.Races.First();
        newRace.FormKey.ShouldBe(newRaceFormKey);
        safeNpc.Race.FormKey.ShouldBe(newRaceFormKey);
    }

    [Fact]
    public void DuplicateReference()
    {
        var modToExtract = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = modToExtract.Npcs.AddNew();
        var race = modToExtract.Races.AddNew();
        race.CloseLootSound.SetTo(modToExtract.SoundDescriptors.AddNew());
        var unneededRace = modToExtract.Races.AddNew();

        var targetMod = new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimSE);
        targetMod.Npcs.Add(npc);
        var safeNpc = targetMod.Npcs.AddNew();
        safeNpc.Race = new FormLink<IRaceGetter>(race.FormKey);
        var safeNpc2 = targetMod.Npcs.AddNew();
        safeNpc2.Race = new FormLink<IRaceGetter>(race.FormKey);

        var linkCache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(modToExtract, targetMod);

        targetMod.DuplicateFromOnlyReferenced(linkCache, modToExtract.ModKey, out var mapping);
        targetMod.Npcs.ShouldHaveCount(3);
        targetMod.Races.ShouldHaveCount(1);
        targetMod.SoundDescriptors.ShouldHaveCount(1);
        targetMod.EnumerateMajorRecords().ShouldHaveCount(5);
        targetMod.Npcs.ShouldContain(safeNpc);
        targetMod.Npcs.ShouldContain(safeNpc2);
        var newRaceFormKey = mapping[race.FormKey];
        var newRace = targetMod.Races.First();
        newRace.FormKey.ShouldBe(newRaceFormKey);
        safeNpc.Race.FormKey.ShouldBe(newRaceFormKey);
    }

    [Fact]
    public void TypedExtraction()
    {
        var modToExtract = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = modToExtract.Npcs.AddNew();
        var race = modToExtract.Races.AddNew();
        race.CloseLootSound.SetTo(modToExtract.SoundDescriptors.AddNew());
        var unneededRace = modToExtract.Races.AddNew();

        var targetMod = new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimSE);
        targetMod.Npcs.Add(npc);
        var safeNpc = targetMod.Npcs.AddNew();
        safeNpc.Race = new FormLink<IRaceGetter>(race.FormKey);

        var linkCache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(modToExtract, targetMod);

        targetMod.DuplicateFromOnlyReferenced(linkCache, modToExtract.ModKey, out var mapping, typeof(INpcGetter));
        targetMod.Npcs.ShouldHaveCount(2);
        targetMod.Races.ShouldHaveCount(1);
        targetMod.SoundDescriptors.ShouldHaveCount(1);
        targetMod.EnumerateMajorRecords().ShouldHaveCount(4);
        targetMod.Npcs.ShouldContain(safeNpc);
        var newRaceFormKey = mapping[race.FormKey];
        var newRace = targetMod.Races.First();
        newRace.FormKey.ShouldBe(newRaceFormKey);
        safeNpc.Race.FormKey.ShouldBe(newRaceFormKey);
    }

    [Fact]
    public void MistypedExtraction()
    {
        var modToExtract = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = modToExtract.Npcs.AddNew();
        var race = modToExtract.Races.AddNew();
        race.CloseLootSound.SetTo(modToExtract.SoundDescriptors.AddNew());
        var unneededRace = modToExtract.Races.AddNew();

        var targetMod = new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimSE);
        targetMod.Npcs.Add(npc);
        var safeNpc = targetMod.Npcs.AddNew();
        safeNpc.Race = new FormLink<IRaceGetter>(race.FormKey);

        var linkCache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(modToExtract, targetMod);

        targetMod.DuplicateFromOnlyReferenced(linkCache, modToExtract.ModKey, out var mapping, typeof(IAmmunitionGetter));
        targetMod.EnumerateMajorRecords().ShouldHaveCount(2);
        targetMod.Npcs.ShouldHaveCount(2);
        targetMod.Npcs.ShouldContain(safeNpc);
        targetMod.Npcs.ShouldContain(npc);
    }
}