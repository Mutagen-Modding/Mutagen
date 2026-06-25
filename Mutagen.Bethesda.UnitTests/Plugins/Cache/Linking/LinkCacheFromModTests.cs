using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public class LinkCacheFromModTests
{
    private record LoadOrderSetup(
        SkyrimMod Master,
        INpc MasterNpc,
        INpc MasterOnlyNpc,
        SkyrimMod Plugin,
        INpc PluginOverride,
        ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache);

    /// <summary>
    /// Builds a load order where a master defines two NPCs, and a plugin overrides only one of them.
    /// </summary>
    private static LoadOrderSetup BuildLoadOrder()
    {
        var master = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var masterNpc = master.Npcs.AddNew();
        masterNpc.EditorID = "SharedNpc";
        var masterOnlyNpc = master.Npcs.AddNew();
        masterOnlyNpc.EditorID = "MasterOnlyNpc";

        var plugin = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var pluginOverride = plugin.Npcs.GetOrAddAsOverride(masterNpc);

        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            master,
            plugin,
        };
        var cache = loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        return new LoadOrderSetup(master, masterNpc, masterOnlyNpc, plugin, pluginOverride, cache);
    }

    [Fact]
    public void LoadOrder_ResolvesScopedToRequestedMod()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.ShouldBeSameAs(setup.MasterNpc);

        setup.Cache.TryResolveFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.PluginModKey, out var fromPlugin)
            .ShouldBeTrue();
        fromPlugin.ShouldBeSameAs(setup.PluginOverride);
    }

    [Fact]
    public void LoadOrder_ReturnsFalseWhenModNotOverriding()
    {
        var setup = BuildLoadOrder();

        // The plugin does not contribute this record, even though it is present in the load order
        setup.Cache.TryResolveFromMod<INpcGetter>(setup.MasterOnlyNpc.FormKey, TestConstants.PluginModKey, out _)
            .ShouldBeFalse();

        setup.Cache.TryResolveFromMod<INpcGetter>(setup.MasterOnlyNpc.FormKey, TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.ShouldBeSameAs(setup.MasterOnlyNpc);
    }

    [Fact]
    public void LoadOrder_ReturnsFalseWhenModNotInCache()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.PluginModKey2, out _)
            .ShouldBeFalse();
    }

    [Fact]
    public void LoadOrder_ReturnsFalseWhenTypeMismatch()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveFromMod<IWeaponGetter>(setup.MasterNpc.FormKey, TestConstants.MasterModKey, out _)
            .ShouldBeFalse();
    }

    [Fact]
    public void LoadOrder_ResolvesByEditorId()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveFromMod<INpcGetter>("SharedNpc", TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.ShouldBeSameAs(setup.MasterNpc);

        setup.Cache.TryResolveFromMod<INpcGetter>("SharedNpc", TestConstants.PluginModKey, out var fromPlugin)
            .ShouldBeTrue();
        fromPlugin.ShouldBeSameAs(setup.PluginOverride);
    }

    [Fact]
    public void LoadOrder_ResolveThrowsWhenMissing()
    {
        var setup = BuildLoadOrder();

        setup.Cache.ResolveFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.MasterModKey)
            .ShouldBeSameAs(setup.MasterNpc);

        Should.Throw<MissingRecordException>(() =>
            setup.Cache.ResolveFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.PluginModKey2));
    }

    [Fact]
    public void LoadOrder_ResolvesContextScopedToRequestedMod()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveContextFromMod<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(setup.MasterNpc.FormKey, TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.Record.ShouldBeSameAs(setup.MasterNpc);
        fromMaster.ModKey.ShouldBe(TestConstants.MasterModKey);

        setup.Cache.TryResolveContextFromMod<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(setup.MasterNpc.FormKey, TestConstants.PluginModKey, out var fromPlugin)
            .ShouldBeTrue();
        fromPlugin.Record.ShouldBeSameAs(setup.PluginOverride);
        fromPlugin.ModKey.ShouldBe(TestConstants.PluginModKey);
    }

    [Fact]
    public void LoadOrder_ResolvesIdentifierScopedToRequestedMod()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveIdentifierFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.ShouldBe("SharedNpc");

        // Both the master and the plugin contribute this record's identifier
        setup.Cache.TryResolveIdentifierFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.PluginModKey, out _)
            .ShouldBeTrue();

        // The plugin does not contribute the master-only record
        setup.Cache.TryResolveIdentifierFromMod<INpcGetter>(setup.MasterOnlyNpc.FormKey, TestConstants.PluginModKey, out _)
            .ShouldBeFalse();
    }

    [Fact]
    public void LoadOrder_ResolvesIdentifierByEditorId()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryResolveIdentifierFromMod<INpcGetter>("SharedNpc", TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.ShouldBe(setup.MasterNpc.FormKey);

        setup.Cache.TryResolveIdentifierFromMod<INpcGetter>("MasterOnlyNpc", TestConstants.PluginModKey, out _)
            .ShouldBeFalse();
    }

    [Fact]
    public void LoadOrder_ResolveIdentifierThrowsWhenMissing()
    {
        var setup = BuildLoadOrder();

        setup.Cache.ResolveIdentifierFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.MasterModKey)
            .ShouldBe("SharedNpc");

        Should.Throw<MissingRecordException>(() =>
            setup.Cache.ResolveIdentifierFromMod<INpcGetter>(setup.MasterNpc.FormKey, TestConstants.PluginModKey2));
    }

    [Fact]
    public void TryGetLinkCacheForMod_ReturnsScopedCache()
    {
        var setup = BuildLoadOrder();

        setup.Cache.TryGetLinkCacheForMod(TestConstants.MasterModKey, out var modCache).ShouldBeTrue();
        modCache.ListedOrder.ShouldHaveSingleItem().ModKey.ShouldBe(TestConstants.MasterModKey);

        setup.Cache.TryGetTypedLinkCacheForMod(TestConstants.PluginModKey, out var typedModCache).ShouldBeTrue();
        typedModCache.ListedOrder.ShouldHaveSingleItem().ModKey.ShouldBe(TestConstants.PluginModKey);

        setup.Cache.TryGetLinkCacheForMod(TestConstants.PluginModKey2, out _).ShouldBeFalse();
    }

    [Fact]
    public void SingleMod_ResolvesWhenModKeyMatches()
    {
        var mod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();

        cache.TryResolveFromMod<INpcGetter>(npc.FormKey, TestConstants.MasterModKey, out var resolved)
            .ShouldBeTrue();
        resolved.ShouldBeSameAs(npc);
    }

    [Fact]
    public void SingleMod_ReturnsFalseWhenModKeyDiffers()
    {
        var mod = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();

        cache.TryGetLinkCacheForMod(TestConstants.PluginModKey, out _).ShouldBeFalse();
        cache.TryResolveFromMod<INpcGetter>(npc.FormKey, TestConstants.PluginModKey, out _)
            .ShouldBeFalse();
    }

    [Fact]
    public void Mutable_ResolvesScopedToRequestedMod()
    {
        var master = new SkyrimMod(TestConstants.MasterModKey, SkyrimRelease.SkyrimSE);
        var masterNpc = master.Npcs.AddNew();
        var mutablePlugin = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var pluginOverride = mutablePlugin.Npcs.GetOrAddAsOverride(masterNpc);

        var immutableBase = new LoadOrder<ISkyrimModGetter>() { master }
            .ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        var cache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(immutableBase, mutablePlugin);

        cache.TryResolveFromMod<INpcGetter>(masterNpc.FormKey, TestConstants.MasterModKey, out var fromMaster)
            .ShouldBeTrue();
        fromMaster.ShouldBeSameAs(masterNpc);

        cache.TryResolveFromMod<INpcGetter>(masterNpc.FormKey, TestConstants.PluginModKey, out var fromPlugin)
            .ShouldBeTrue();
        fromPlugin.ShouldBeSameAs(pluginOverride);

        cache.TryResolveFromMod<INpcGetter>(masterNpc.FormKey, TestConstants.PluginModKey2, out _)
            .ShouldBeFalse();
    }
}
