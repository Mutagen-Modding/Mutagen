using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking
{
    public abstract partial class ALinkingTests : IClassFixture<LinkingTestInit>, IClassFixture<LoquiUse>
    {
        private readonly LinkingTestInit _testInit;

        public static readonly IEnumerable<object[]> ContextTestSources = new[]
        {
            new object[] { LinkCachePreferences.RetentionType.OnlyIdentifiers, new NormalContextRetriever() },
            new object[] { LinkCachePreferences.RetentionType.WholeRecord, new NormalContextRetriever() },
            new object[] { LinkCachePreferences.RetentionType.OnlyIdentifiers, new SimpleContextRetriever() },
            new object[] { LinkCachePreferences.RetentionType.WholeRecord, new SimpleContextRetriever() }
        };

        public static FormKey UnusedFormKey = new FormKey(TestConstants.PluginModKey, 123456);
        public static string UnusedEditorID = "Unused";
        public static FormKey TestFileFormKey = new FormKey(TestDataPathing.SkyrimTestMod.ModKey, 0x800);
        public static FormKey TestFileFormKey2 = new FormKey(TestDataPathing.SkyrimTestMod.ModKey, 0x801);
        public static string TestFileEditorID = "Record1";
        public static string TestFileEditorID2 = "Record2";

        public ALinkingTests(LinkingTestInit testInit)
        {
            _testInit = testInit;
        }

        public abstract IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter);
        
        public abstract bool ReadOnly { get; }

        protected abstract (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs);

        protected abstract (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs);

        protected LinkCachePreferences GetPrefs(LinkCachePreferences.RetentionType type)
        {
            return new LinkCachePreferences
            {
                Retention = type,
                LinkInterfaceMapGetterOverride = _testInit.LinkInterfaceMapping
            };
        }

        protected (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences.RetentionType type) => GetLinkCache(modGetter, GetPrefs(type));

        protected (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences.RetentionType type) => GetLinkCache(loadOrder, GetPrefs(type));

        protected void WrapPotentialThrow(LinkCachePreferences.RetentionType cacheType, LinkCacheStyle style, Action a)
        {
            switch (cacheType)
            {
                case LinkCachePreferences.RetentionType.OnlyIdentifiers when style != LinkCacheStyle.OnlyDirect:
                    Assert.Throws<ArgumentException>(a);
                    break;
                default:
                    a();
                    break;
            }
        }
    }
}
