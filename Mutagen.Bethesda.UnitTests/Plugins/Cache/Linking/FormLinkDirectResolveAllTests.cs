using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
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
        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAll_Empty(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IEffectRecordGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            formLink.ResolveAll(package).Should().BeEmpty();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAll_Typed_Empty(LinkCacheTestTypes cacheType)
        {
            var formLink = new FormLink<IPlacedGetter>(UnusedFormKey);
            var (style, package) = GetLinkCache(new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE), cacheType);
            formLink.ResolveAll<IPlacedGetter, IPlacedNpcGetter>(package).Should().BeEmpty();
        }

        [Theory]
        [InlineData(LinkCacheTestTypes.Identifiers)]
        [InlineData(LinkCacheTestTypes.WholeRecord)]
        public void FormLink_Direct_ResolveAll_Linked(LinkCacheTestTypes cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var npc = mod.Npcs.AddNew();
            var (style, package) = GetLinkCache(mod, cacheType);
            FormLink<INpc> formLink = new FormLink<INpc>(npc.FormKey);
            WrapPotentialThrow(cacheType, style, () =>
            {
                var resolved = formLink.ResolveAll(package).ToArray();
                resolved.Should().HaveCount(1);
                resolved.First().Should().BeSameAs(npc);
            });
        }
    }
}