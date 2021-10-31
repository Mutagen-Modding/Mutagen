using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public partial class ALinkingTests
    {
        [Fact]
        public void FormLink_LoadOrder_ResolveAllContexts_Empty()
        {
            var package = TypicalLoadOrder().ToImmutableLinkCache();
            var formLink = new FormLink<INpcGetter>(UnusedFormKey);
            formLink.ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(package).Should().BeEmpty();
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void FormLink_LoadOrder_ResolveAllContexts_Linked(LinkCacheTestTypes cacheType, AContextRetriever contextRetriever)
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
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(npc);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void FormLink_LoadOrder_ResolveAllContexts_MultipleLinks(LinkCacheTestTypes cacheType, AContextRetriever contextRetriever)
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
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void FormLink_LoadOrder_ResolveAllContexts_DoubleQuery(LinkCacheTestTypes cacheType, AContextRetriever contextRetriever)
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
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void FormLink_LoadOrder_ResolveAllContexts_UnrelatedNotIncluded(LinkCacheTestTypes cacheType, AContextRetriever contextRetriever)
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
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(ContextTestSources))]
        public void FormLink_LoadOrder_ResolveAllContexts_SeparateQueries(LinkCacheTestTypes cacheType, AContextRetriever contextRetriever)
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
            resolved.Should().HaveCount(2);
            resolved.First().Record.Should().BeSameAs(npcOverride);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey3);
            resolved.First().Parent.Should().BeNull();
            resolved.Last().Record.Should().BeSameAs(npc);
            resolved.Last().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.Last().Parent.Should().BeNull();
            formLink = new FormLink<INpcGetter>(unrelatedNpc.FormKey);
            resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
            resolved.Should().HaveCount(1);
            resolved.First().Record.Should().BeSameAs(unrelatedNpc);
            resolved.First().ModKey.Should().Be(TestConstants.PluginModKey);
            resolved.First().Parent.Should().BeNull();
        }
    }
}