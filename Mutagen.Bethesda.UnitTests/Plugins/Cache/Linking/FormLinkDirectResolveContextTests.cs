using FluentAssertions;
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
        WrapPotentialThrow(cacheType, style, () =>
        {
            contextRetriever.TryResolveContext<IEffectRecord, IEffectRecordGetter>(formLink, package, out _).Should().BeFalse();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveContexts_Typed_Empty(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            contextRetriever.TryResolveContext<IPlacedGetter, IPlacedNpc, IPlacedNpcGetter>(formLink, package, out _).Should().BeFalse();
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
        WrapPotentialThrow(cacheType, style, () =>
        {
            contextRetriever.TryResolveContext<INpc, INpcGetter>(formLink, package, out var resolved).Should().BeTrue();
            resolved.Should().NotBeNull();
            resolved!.Record.Should().BeSameAs(npc);
            resolved!.ModKey.Should().Be(TestConstants.PluginModKey);
            resolved!.Parent.Should().BeNull();
        });
    }
}