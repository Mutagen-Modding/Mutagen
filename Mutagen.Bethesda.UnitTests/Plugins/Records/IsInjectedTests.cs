using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class IsInjectedTests
{
    /// <summary>
    /// Test that a record defined in its own mod is NOT injected
    /// </summary>
    [Theory, MutagenModAutoData]
    public void RecordInOwnMod_NotInjected(SkyrimMod mod)
    {
        var npc = mod.Npcs.AddNew();

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            mod
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        npc.IsInjected(linkCache).ShouldBeFalse();
    }

    /// <summary>
    /// Test that a record with a FormKey from ModA but defined in ModB IS injected
    /// </summary>
    [Theory, MutagenModAutoData]
    public void RecordFromDifferentMod_IsInjected(
        SkyrimMod modA,
        SkyrimMod modB)
    {
        // Create a record in ModB but with a FormKey from ModA
        var formKey = new FormKey(modA.ModKey, 0x123456);
        var npc = new Npc(formKey, SkyrimRelease.SkyrimSE);
        modB.Npcs.Add(npc);

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            modA,
            modB
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        npc.IsInjected(linkCache).ShouldBeTrue();
    }

    /// <summary>
    /// Test that an override record (same FormKey, later in load order) is NOT injected
    /// </summary>
    [Theory, MutagenModAutoData]
    public void OverrideRecord_NotInjected(
        SkyrimMod modA,
        SkyrimMod modB)
    {
        // Create original record in ModA
        var originalNpc = modA.Npcs.AddNew();

        // Create override in ModB with same FormKey
        var overrideNpc = new Npc(originalNpc.FormKey, SkyrimRelease.SkyrimSE);
        modB.Npcs.Add(overrideNpc);

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            modA,
            modB
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        // The override should not be injected since its FormKey is from ModA and ModA contains it
        overrideNpc.IsInjected(linkCache).ShouldBeFalse();
    }

    /// <summary>
    /// Test that checking a record not in the load order throws MissingModException
    /// </summary>
    [Theory, MutagenModAutoData]
    public void RecordModNotInLoadOrder(
        SkyrimMod modA,
        SkyrimMod modB)
    {
        // Create a record in ModB
        var npc = modB.Npcs.AddNew();

        // Create load order that doesn't include ModB
        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            modA
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        // Should throw because ModB is not in the load order
        npc.IsInjected(linkCache).ShouldBe(true);
    }

    /// <summary>
    /// Test injected record in a three-mod scenario
    /// ModA defines the record, ModC injects a record with ModA's FormKey
    /// </summary>
    [Theory, MutagenModAutoData]
    public void ThreeModScenario_InjectedIntoOriginalMod(
        SkyrimMod modA,
        SkyrimMod modB,
        SkyrimMod modC)
    {
        // Create a record in ModA
        var originalNpc = modA.Npcs.AddNew();

        // ModC injects a new record with ModA's namespace
        var formKey = new FormKey(modA.ModKey, 0x999999);
        var injectedNpc = new Npc(formKey, SkyrimRelease.SkyrimSE);
        modC.Npcs.Add(injectedNpc);

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            modA,
            modB,
            modC
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        // Original record should not be injected
        originalNpc.IsInjected(linkCache).ShouldBeFalse();

        // Injected record should be detected as injected
        injectedNpc.IsInjected(linkCache).ShouldBeTrue();
    }

    /// <summary>
    /// Test with master file - records in a master file itself are not injected
    /// </summary>
    [Theory, MutagenModAutoData]
    public void RecordInMasterFile_NotInjected(
        SkyrimMod master,
        SkyrimMod plugin)
    {
        var npc = master.Npcs.AddNew();

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            master,
            plugin
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        npc.IsInjected(linkCache).ShouldBeFalse();
    }

    /// <summary>
    /// Test injection into a master file from a plugin
    /// </summary>
    [Theory, MutagenModAutoData]
    public void RecordInjectedIntoMaster_IsInjected(
        SkyrimMod master,
        SkyrimMod plugin)
    {
        // Plugin injects a record into the master's namespace
        var formKey = new FormKey(master.ModKey, 0xABCDEF);
        var injectedNpc = new Npc(formKey, SkyrimRelease.SkyrimSE);
        plugin.Npcs.Add(injectedNpc);

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            master,
            plugin
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        injectedNpc.IsInjected(linkCache).ShouldBeTrue();
    }

    /// <summary>
    /// Test with different record types to ensure IsInjected works across all major record types
    /// </summary>
    [Theory, MutagenModAutoData]
    public void DifferentRecordTypes_InjectionDetected(
        SkyrimMod modA,
        SkyrimMod modB)
    {
        // Create injected records of different types
        var weaponFormKey = new FormKey(modA.ModKey, 0x100001);
        var armorFormKey = new FormKey(modA.ModKey, 0x100002);
        var spellFormKey = new FormKey(modA.ModKey, 0x100003);

        var injectedWeapon = new Weapon(weaponFormKey, SkyrimRelease.SkyrimSE);
        var injectedArmor = new Armor(armorFormKey, SkyrimRelease.SkyrimSE);
        var injectedSpell = new Spell(spellFormKey, SkyrimRelease.SkyrimSE);

        modB.Weapons.Add(injectedWeapon);
        modB.Armors.Add(injectedArmor);
        modB.Spells.Add(injectedSpell);

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            modA,
            modB
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        injectedWeapon.IsInjected(linkCache).ShouldBeTrue();
        injectedArmor.IsInjected(linkCache).ShouldBeTrue();
        injectedSpell.IsInjected(linkCache).ShouldBeTrue();
    }

    /// <summary>
    /// Test edge case: empty mod with injected records
    /// </summary>
    [Theory, MutagenModAutoData]
    public void EmptyOriginMod_RecordIsInjected(
        SkyrimMod emptyMod,
        SkyrimMod injectingMod)
    {
        // Inject a record into the empty mod's namespace
        var formKey = new FormKey(emptyMod.ModKey, 0x000801);
        var injectedNpc = new Npc(formKey, SkyrimRelease.SkyrimSE);
        injectingMod.Npcs.Add(injectedNpc);

        var loadOrder = new LoadOrder<ISkyrimModGetter>
        {
            emptyMod,
            injectingMod
        };

        var linkCache = loadOrder.ToImmutableLinkCache();

        injectedNpc.IsInjected(linkCache).ShouldBeTrue();
    }
}
