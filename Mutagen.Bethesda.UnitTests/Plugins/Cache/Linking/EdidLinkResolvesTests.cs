using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public partial class ALinkingTests
    {
        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void EDIDLink_TryResolve_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var (style, package) = GetLinkCache(mod, cacheType);
            Assert.False(link.TryResolve(package, out var _));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void EDIDLink_TryResolve_Linked(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var (style, package) = GetLinkCache(mod, cacheType);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.True(link.TryResolve(package, out var linkedRec));
            Assert.Same(effect, linkedRec);
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void EDIDLink_Resolve_NoLink(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "NULL";
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            var (style, package) = GetLinkCache(mod, cacheType);
            Assert.Null(link.TryResolve(package));
        }

        [Theory]
        [InlineData(LinkCachePreferences.RetentionType.OnlyIdentifiers)]
        [InlineData(LinkCachePreferences.RetentionType.WholeRecord)]
        public void EDIDLink_Resolve_Linked(LinkCachePreferences.RetentionType cacheType)
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
            var effect = mod.MagicEffects.AddNew();
            effect.EditorID = "LINK";
            var (style, package) = GetLinkCache(mod, cacheType);
            EDIDLink<IMagicEffect> link = new EDIDLink<IMagicEffect>(new RecordType("LINK"));
            Assert.Same(effect, link.TryResolve(package));
        }
    }
}