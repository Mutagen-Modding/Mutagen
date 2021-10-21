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
        public static readonly IEnumerable<object[]> ContextTestSources = new[]
        {
            new object[] { LinkCacheTestTypes.Identifiers, new NormalContextRetriever() },
            new object[] { LinkCacheTestTypes.WholeRecord, new NormalContextRetriever() },
            new object[] { LinkCacheTestTypes.Identifiers, new SimpleContextRetriever() },
            new object[] { LinkCacheTestTypes.WholeRecord, new SimpleContextRetriever() }
        };

        public static FormKey UnusedFormKey = new FormKey(TestConstants.PluginModKey, 123456);
        public static string UnusedEditorID = "Unused";
        public static FormKey TestFileFormKey = new FormKey(TestDataPathing.SkyrimTestMod.ModKey, 0x800);
        public static FormKey TestFileFormKey2 = new FormKey(TestDataPathing.SkyrimTestMod.ModKey, 0x801);
        public static string TestFileEditorID = "Record1";
        public static string TestFileEditorID2 = "Record2";

        public abstract IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter);
        
        public abstract bool ReadOnly { get; }

        protected abstract (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs);

        protected abstract (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs);

        protected LinkCachePreferences GetPrefs(LinkCacheTestTypes type) => type switch
        {
            LinkCacheTestTypes.Identifiers => LinkCachePreferences.OnlyIdentifiers(),
            LinkCacheTestTypes.WholeRecord => LinkCachePreferences.WholeRecord(),
            _ => throw new NotImplementedException()
        };

        protected (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCacheTestTypes type) => GetLinkCache(modGetter, GetPrefs(type));

        protected (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCacheTestTypes type) => GetLinkCache(loadOrder, GetPrefs(type));

        protected void WrapPotentialThrow(LinkCacheTestTypes cacheType, LinkCacheStyle style, Action a)
        {
            switch (cacheType)
            {
                case LinkCacheTestTypes.Identifiers when style != LinkCacheStyle.OnlyDirect:
                    Assert.Throws<ArgumentException>(a);
                    break;
                default:
                    a();
                    break;
            }
        }
    }
}
