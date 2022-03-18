using System;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Implementations;

public abstract class AMutableDirectTests : ALinkingTests
{
    public override bool ReadOnly => false;

    public AMutableDirectTests(LinkingTestInit testInit) 
        : base(testInit)
    {
    }

    public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
    {
        getter = mod;
        return Disposable.Empty;
    }

    protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
    {
        return (LinkCacheStyle.OnlyDirect, new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter));
    }

    protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
    {
        return (LinkCacheStyle.HasCaching, new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs)));
    }
}
    
public class MutableResolveDirectTests : AMutableDirectTests
{
    public MutableResolveDirectTests(LinkingTestInit testInit)
        : base(testInit)
    {
    }
        
    public override bool TryTest<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out TTarget target, ResolveTarget resolve)
    {
        return linkCache.TryResolve<TTarget>(formKey, out target!, resolve);
    }
        
    public override bool TryTest(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out IMajorRecordGetter target, ResolveTarget resolve)
    {
        return linkCache.TryResolve(formKey, out target!, resolve);
    }
        
    public override bool TryTest<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out TTarget target)
    {
        return linkCache.TryResolve<TTarget>(editorId, out target!);
    }
        
    public override bool TryTest(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out IMajorRecordGetter target)
    {
        return linkCache.TryResolve(editorId, out target!);
    }
}
    
public class MutableContextResolveDirectTests : AMutableDirectTests
{
    public MutableContextResolveDirectTests(LinkingTestInit testInit)
        : base(testInit)
    {
    }
        
    public override bool TryTest<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out TTarget target, ResolveTarget resolve)
    {
        return TryTestContext<TSetter, TTarget>(linkCache, formKey, out target, resolve);
    }
        
    public override bool TryTest(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, FormKey formKey, out IMajorRecordGetter target, ResolveTarget resolve)
    {
        return TryTestContext(linkCache, formKey, out target, resolve);
    }
        
    public override bool TryTest<TSetter, TTarget>(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out TTarget target)
    {
        return TryTestContext<TSetter, TTarget>(linkCache, editorId, out target);
    }
        
    public override bool TryTest(ILinkCache<ISkyrimMod, ISkyrimModGetter> linkCache, string editorId, out IMajorRecordGetter target)
    {
        return TryTestContext(linkCache, editorId, out target);
    }
}