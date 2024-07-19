using System.Reactive.Disposables;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog.IO;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Implementations;

public abstract class MutableOverlayTests : ALinkingTests
{
    public override bool ReadOnly => true;

    public MutableOverlayTests(LinkingTestInit testInit) : base(testInit)
    {
    }

    public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
    {
        var tempFile = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath);
        var path = new ModPath(mod.ModKey, tempFile.File.Path);
        mod.BeginWrite
            .WithNoLoadOrder()
            .ToPath(path)
            .NoModKeySync()
            .Write();
        var overlay = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimLE);
        getter = overlay;
        return Disposable.Create(() =>
        {
            overlay.Dispose();
            tempFile.Dispose();
        });
    }

    protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(ISkyrimModGetter modGetter, LinkCachePreferences prefs)
    {
        return (LinkCacheStyle.OnlyDirect, new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter));
    }

    protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
    {
        return (LinkCacheStyle.OnlyDirect, new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>()));
    }
}
    
public class MutableResolveOverlayTests : MutableOverlayTests
{
    public MutableResolveOverlayTests(LinkingTestInit testInit)
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
    
public class MutableContextResolveOverlayTests : MutableOverlayTests
{
    public MutableContextResolveOverlayTests(LinkingTestInit testInit)
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