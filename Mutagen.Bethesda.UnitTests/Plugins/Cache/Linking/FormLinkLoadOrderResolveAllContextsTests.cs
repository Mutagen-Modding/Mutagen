using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog.Testing.Extensions;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Fact]
    public void FormLink_LoadOrder_ResolveAllContexts_Empty()
    {
        var package = TypicalLoadOrder().ToImmutableLinkCache();
        var formLink = new FormLink<INpcGetter>(UnusedFormKey);
        formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_LoadOrder_ResolveAllContexts_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        var resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved.ShouldHaveCount(1);
        resolved.First().Record.ShouldBeSameAs(npc);
        resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey);
        resolved.First().Parent.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_LoadOrder_ResolveAllContexts_MultipleLinks(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
        var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
        npcOverride.FaceParts = new NpcFaceParts();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            mod2
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        var resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved.ShouldHaveCount(2);
        resolved.First().Record.ShouldBeSameAs(npcOverride);
        resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey3);
        resolved.First().Parent.ShouldBeNull();
        resolved.Last().Record.ShouldBeSameAs(npc);
        resolved.Last().ModKey.ShouldBe(TestConstants.PluginModKey);
        resolved.Last().Parent.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_LoadOrder_ResolveAllContexts_DoubleQuery(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
        var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
        npcOverride.FaceParts = new NpcFaceParts();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            mod2
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        var resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved.ShouldHaveCount(2);
        resolved.First().Record.ShouldBeSameAs(npcOverride);
        resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey3);
        resolved.First().Parent.ShouldBeNull();
        resolved.Last().Record.ShouldBeSameAs(npc);
        resolved.Last().ModKey.ShouldBe(TestConstants.PluginModKey);
        resolved.Last().Parent.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_LoadOrder_ResolveAllContexts_UnrelatedNotIncluded(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var unrelatedNpc = mod.Npcs.AddNew();
        var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
        var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
        npcOverride.FaceParts = new NpcFaceParts();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            mod2
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        var resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved.ShouldHaveCount(2);
        resolved.First().Record.ShouldBeSameAs(npcOverride);
        resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey3);
        resolved.First().Parent.ShouldBeNull();
        resolved.Last().Record.ShouldBeSameAs(npc);
        resolved.Last().ModKey.ShouldBe(TestConstants.PluginModKey);
        resolved.Last().Parent.ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_LoadOrder_ResolveAllContexts_SeparateQueries(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var unrelatedNpc = mod.Npcs.AddNew();
        var mod2 = new SkyrimMod(TestConstants.PluginModKey3, SkyrimRelease.SkyrimLE);
        var npcOverride = mod2.Npcs.GetOrAddAsOverride(npc);
        npcOverride.FaceParts = new NpcFaceParts();
        var loadOrder = new LoadOrder<ISkyrimModGetter>()
        {
            mod,
            new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE),
            mod2
        };
        var (style, package) = GetLinkCache(loadOrder, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        var resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved.ShouldHaveCount(2);
        resolved.First().Record.ShouldBeSameAs(npcOverride);
        resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey3);
        resolved.First().Parent.ShouldBeNull();
        resolved.Last().Record.ShouldBeSameAs(npc);
        resolved.Last().ModKey.ShouldBe(TestConstants.PluginModKey);
        resolved.Last().Parent.ShouldBeNull();
        formLink = new FormLink<INpcGetter>(unrelatedNpc.FormKey);
        resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
        resolved.ShouldHaveCount(1);
        resolved.First().Record.ShouldBeSameAs(unrelatedNpc);
        resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey);
        resolved.First().Parent.ShouldBeNull();
    }
}