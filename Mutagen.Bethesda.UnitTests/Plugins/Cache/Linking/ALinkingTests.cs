using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

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
            MetaInterfaceMapGetterOverride = _testInit.LinkInterfaceMapping
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

    #region TryResolve

    public virtual bool TryTest<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out TTarget target, ResolveTarget resolve = ResolveTarget.Winner)
        where TSetter : class, TTarget, IMajorRecordQueryable
        where TTarget : class, IMajorRecordQueryableGetter
    {
        return linkCache.TryResolve<TTarget>(formKey, out target, resolve);
    }
        
    public virtual bool TryTest(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out IMajorRecordGetter target, ResolveTarget resolve = ResolveTarget.Winner)
    {
        return linkCache.TryResolve(formKey, out target, resolve);
    }
        
    public virtual bool TryTest<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out TTarget target)
        where TSetter : class, TTarget, IMajorRecordQueryable
        where TTarget : class, IMajorRecordQueryableGetter
    {
        return linkCache.TryResolve<TTarget>(editorId, out target);
    }
        
    public virtual bool TryTest(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out IMajorRecordGetter target)
    {
        return linkCache.TryResolve(editorId, out target);
    }
        
    public bool TryTestContext<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out TTarget target, ResolveTarget resolve)
        where TSetter : class, TTarget, IMajorRecordQueryable
        where TTarget : class, IMajorRecordQueryableGetter
    {
        if (linkCache.TryResolveContext<TSetter, TTarget>(formKey, out var context, resolve))
        {
            target = context.Record;
            return true;
        }

        target = default!;
        return false;
    }
        
    public bool TryTestContext(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out IMajorRecordGetter target, ResolveTarget resolve)
    {
        if (linkCache.TryResolveContext(formKey, out var context, resolve))
        {
            target = context.Record;
            return true;
        }

        target = default!;
        return false;
    }
        
    public bool TryTestContext<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out TTarget target)
        where TSetter : class, TTarget, IMajorRecordQueryable
        where TTarget : class, IMajorRecordQueryableGetter
    {
        if (linkCache.TryResolveContext<TSetter, TTarget>(editorId, out var context))
        {
            target = context.Record;
            return true;
        }

        target = default!;
        return false;
    }
        
    public bool TryTestContext(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out IMajorRecordGetter target)
    {
        if (linkCache.TryResolveContext(editorId, out var context))
        {
            target = context.Record;
            return true;
        }

        target = default!;
        return false;
    }

    #endregion
}