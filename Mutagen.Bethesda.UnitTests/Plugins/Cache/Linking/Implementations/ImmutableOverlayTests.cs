using System;
using System.Reactive.Disposables;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Noggog.Utility;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Implementations;

public class ImmutableOverlayTests : ALinkingTests
{
    public override bool ReadOnly => true;

    public ImmutableOverlayTests(LinkingTestInit testInit) : base(testInit)
    {
    }

    public override IDisposable ConvertMod(SkyrimMod mod, out ISkyrimModGetter getter)
    {
        return ConvertModToOverlay(mod, out getter);
    }

    public static IDisposable ConvertModToOverlay(SkyrimMod mod, out ISkyrimModGetter getter)
    {
        var tempFile = new TempFile(extraDirectoryPaths: TestPathing.TempFolderPath);
        var path = new ModPath(mod.ModKey, tempFile.File.Path);
        mod.WriteToBinaryParallel(
            path,
            new BinaryWriteParameters()
            {
                ModKey = ModKeyOption.NoCheck,
            });
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
        return (LinkCacheStyle.HasCaching, new ImmutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(modGetter, prefs));
    }

    protected override (LinkCacheStyle Style, ILinkCache<ISkyrimMod, ISkyrimModGetter> Cache) GetLinkCache(LoadOrder<ISkyrimModGetter> loadOrder, LinkCachePreferences prefs)
    {
        return (LinkCacheStyle.HasCaching, loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(prefs));
    }
}
    
public class ImmutableResolveOverlayTests : ImmutableOverlayTests
{
    public ImmutableResolveOverlayTests(LinkingTestInit testInit)
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
    
public class ImmutableContextResolveOverlayTests : ImmutableOverlayTests
{
    public ImmutableContextResolveOverlayTests(LinkingTestInit testInit)
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