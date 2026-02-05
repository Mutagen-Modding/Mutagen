using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContexts_Empty(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, contextRetriever, () =>
        {
            contextRetriever.TryResolveContext<IEffectRecord, IEffectRecordGetter>(formLink, package, out _).ShouldBeFalse();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContexts_Typed_Empty(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, contextRetriever, () =>
        {
            contextRetriever.TryResolveContext<IPlacedGetter, IPlacedNpc, IPlacedNpcGetter>(formLink, package, out _).ShouldBeFalse();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContexts_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, contextRetriever, () =>
        {
            contextRetriever.TryResolveContext<INpc, INpcGetter>(formLink, package, out var resolved).ShouldBeTrue();
            resolved.ShouldNotBeNull();
            resolved!.Record.ShouldBeSameAs(npc);
            resolved!.ModKey.ShouldBe(TestConstants.PluginModKey);
            resolved!.Parent.ShouldBeNull();
        });
    }
}