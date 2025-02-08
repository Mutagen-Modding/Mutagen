﻿using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
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
    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveAllContexts_Empty(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            contextRetriever.ResolveAllContexts<IEffectRecord, IEffectRecordGetter>(formLink, package).ShouldBeEmpty();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveAllContexts_Typed_Empty(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        WrapPotentialThrow(cacheType, style, () =>
        {
            contextRetriever.ResolveAllContexts<IPlacedGetter, IPlacedNpc, IPlacedNpcGetter>(formLink, package).ShouldBeEmpty();
        });
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveAllContexts_Linked(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, () =>
        {
            var resolved = contextRetriever.ResolveAllContexts<INpc, INpcGetter>(formLink, package).ToArray();
            resolved.ShouldHaveCount(1);
            resolved.First().Record.ShouldBeSameAs(npc);
            resolved.First().ModKey.ShouldBe(TestConstants.PluginModKey);
            resolved.First().Parent.ShouldBeNull();
        });
    }
}