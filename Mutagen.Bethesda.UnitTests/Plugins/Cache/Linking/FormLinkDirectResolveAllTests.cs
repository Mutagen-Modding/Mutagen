using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

public partial class ALinkingTests
{
    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_Direct_ResolveAll_Empty(LinkCachePreferences.RetentionType cacheType)
    {
        var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        formLink.ResolveAll(package).ShouldBeEmpty();
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_Direct_ResolveAll_Typed_Empty(LinkCachePreferences.RetentionType cacheType)
    {
        var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
        var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
        formLink.ResolveAll<IPlacedGetter, IPlacedNpcGetter>(package).ShouldBeEmpty();
    }

    [Theory]
    [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
    [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
    public void FormLink_Direct_ResolveAll_Linked(LinkCachePreferences.RetentionType cacheType)
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var (style, package) = GetLinkCache(mod, cacheType);
        FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
        WrapPotentialThrow(cacheType, style, null, () =>
        {
            var resolved = formLink.ResolveAll(package).ToArray();
            resolved.ShouldHaveCount(1);
            resolved.First().ShouldBeSameAs(npc);
        });
    }
}