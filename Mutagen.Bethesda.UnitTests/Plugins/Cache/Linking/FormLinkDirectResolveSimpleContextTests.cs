using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog;
using Shouldly;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveSimpleContext_Empty(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        package.TryResolveSimpleContext<IEffectRecordGetter>(formLink.FormKey, out _).ShouldBeFalse();
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveSimpleContext_DialogResponses(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        // This test reproduces the specific issue with DialogResponses
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var dialogTopic = mod.DialogTopics.AddNew();
        var dialogResponse = new DialogResponses(mod);
        dialogTopic.Responses.Add(dialogResponse);

        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<IDialogResponsesGetter>(dialogResponse.FormKey);

        // Test the specific issue: TryResolveSimpleContext should work for DialogResponses
        package.TryResolveSimpleContext<IDialogResponsesGetter>(dialogResponse.FormKey, out var resolved).ShouldBeTrue();
        resolved.ShouldNotBeNull();
        resolved!.Record.ShouldBeSameAs(dialogResponse);
        resolved!.ModKey.ShouldBe(TestConstants.PluginModKey);

        // Also test the non-generic version
        package.TryResolveSimpleContext(dialogResponse.FormKey, out var resolvedGeneral).ShouldBeTrue();
        resolvedGeneral.ShouldNotBeNull();
        resolvedGeneral!.Record.ShouldBeSameAs(dialogResponse);
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveSimpleContext_DialogResponses_ByRecord(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        // Test the exact pattern from user's code
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var dialogTopic = mod.DialogTopics.AddNew();
        var dialogResponse = new DialogResponses(mod);
        dialogTopic.Responses.Add(dialogResponse);

        var (style, package) = GetLinkCache(mod, cacheType);

        // Enumerate DialogResponses records like in user's code
        foreach (var record in mod.EnumerateMajorRecords<IDialogResponsesGetter>())
        {
            // This should resolve but currently doesn't according to the user
            var result = package.TryResolveSimpleContext(record, out var resolved);
            result.ShouldBeTrue($"Could not resolve DialogResponses record {record.FormKey}");
            resolved.ShouldNotBeNull();
            resolved!.Record.FormKey.ShouldBe(record.FormKey);
        }
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveSimpleContext_Npc_ShouldWork(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        // Control test - make sure simple context works for other record types
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<INpcGetter>(npc.FormKey);

        package.TryResolveSimpleContext<INpcGetter>(npc.FormKey, out var resolved).ShouldBeTrue();
        resolved.ShouldNotBeNull();
        resolved!.Record.ShouldBeSameAs(npc);
        resolved!.ModKey.ShouldBe(TestConstants.PluginModKey);
    }

    [Theory]
    [MemberData(nameof(ContextTestSources))]
    public void FormLink_Direct_ResolveSimpleContext_DialogResponses_ShouldWork(LinkCachePreferences.RetentionType cacheType, AContextRetriever contextRetriever)
    {
        // Control test - make sure simple context works for other record types
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var topic = mod.DialogTopics.AddNew();
        var responses = topic.Responses.AddReturn(new DialogResponses(mod));
        var (style, package) = GetLinkCache(mod, cacheType);
        var formLink = new FormLink<IDialogResponsesGetter>(responses.FormKey);

        package.TryResolveSimpleContext<IDialogResponsesGetter>(formLink.FormKey, out var resolved).ShouldBeTrue();
        resolved.ShouldNotBeNull();
        resolved!.Record.ShouldBeSameAs(responses);
        resolved!.ModKey.ShouldBe(TestConstants.PluginModKey);
    }
}