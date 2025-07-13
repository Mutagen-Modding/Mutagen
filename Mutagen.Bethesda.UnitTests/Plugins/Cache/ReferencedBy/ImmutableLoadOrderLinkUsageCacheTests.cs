using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using Shouldly;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.ReferencedBy;

public class ImmutableLoadOrderLinkUsageCacheTests
{
    [Theory, MutagenModAutoData]
    public void EmptyMod(
        SkyrimMod mod,
        Race r)
    {
        var linkCache = mod.ToImmutableLinkCache();
        
        void TestUnypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<IMajorRecordGetter>> referencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.UsageLinks.ShouldBeEmpty();
        }
        
        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<Npc>> referencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.UsageLinks.ShouldBeEmpty();
        }
        
        TestUnypedResult(c => c.GetUsagesOf(r.FormKey));
        TestTypedResult(c => c.GetUsagesOf<Npc>(r));
        TestUnypedResult(c => c.GetUsagesOf(r));
        TestTypedResult(c => c.GetUsagesOf<Npc>(r.ToStandardizedIdentifier()));
        TestUnypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()));
        TestTypedResult(c => c.GetUsagesOf<Npc>(r.ToLinkGetter()));
        TestTypedResult(c => c.GetUsagesOf<Npc>(r.ToLink()));
    }

    [Theory, MutagenModAutoData]
    public void SelfModReference(
        SkyrimMod mod,
        Npc n,
        Npc n2,
        Race r)
    {
        n.Race.SetTo(r);
        var linkCache = mod.ToImmutableLinkCache();

        void TestUntypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<IMajorRecordGetter>> referencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.UsageLinks.ShouldHaveCount(1);
            referenced.UsageLinks.ShouldEqualEnumerable(n.ToLinkGetter());
            referenced.Contains(n).ShouldBeTrue();
            referenced.Contains(n.FormKey).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n.FormKey, typeof(INpcGetter))).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n.FormKey, typeof(IWeaponGetter))).ShouldBeFalse();
            referenced.Contains(new FormLink<INpcGetter>(n.FormKey)).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n.FormKey, typeof(IMajorRecordGetter))).ShouldBeTrue();
            referenced.Contains(new FormLink<IMajorRecordGetter>(n.FormKey)).ShouldBeTrue();
        }

        TestUntypedResult(c => c.GetUsagesOf(r.FormKey));

        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<INpcGetter>> npcReferencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var npcReferenced = npcReferencedCall(cache);
            npcReferenced.UsageLinks.ShouldHaveCount(1);
            npcReferenced.UsageLinks.ShouldEqualEnumerable(n.ToLink<INpcGetter>());
            npcReferenced.Contains(n).ShouldBeTrue();
            npcReferenced.Contains(n.FormKey).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n.FormKey, typeof(INpcGetter))).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n.FormKey, typeof(IWeaponGetter))).ShouldBeFalse();
            npcReferenced.Contains(new FormLink<INpcGetter>(n.FormKey)).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n.FormKey, typeof(IMajorRecordGetter))).ShouldBeTrue();
            npcReferenced.Contains(new FormLink<IMajorRecordGetter>(n.FormKey)).ShouldBeTrue();
        }

        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r));
        TestUntypedResult(c => c.GetUsagesOf(r));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()));
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()));
    }

    [Theory, MutagenModAutoData]
    public void MultiModReference(
        ModKey modKey,
        ModKey modKey2,
        Race r)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var n11 = mod.Npcs.AddNew();
        var n12 = mod.Npcs.AddNew();
        var mod2 = new SkyrimMod(modKey2, SkyrimRelease.SkyrimSE);
        var n21 = mod2.Npcs.AddNew();
        var n22 = mod2.Npcs.AddNew();
        n12.Race.SetTo(r);
        n22.Race.SetTo(r);
        var linkCache = new[] { mod, mod2 }.ToImmutableLinkCache();
        var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);

        void TestUntypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<IMajorRecordGetter>> referencedCall)
        {
            var referenced = referencedCall(cache);
            referenced.UsageLinks.ShouldHaveCount(2);
            referenced.UsageLinks.ShouldBeSubsetOf(
            [
                n12.ToLink<IMajorRecordGetter>(),
                n22.ToLink<IMajorRecordGetter>()
            ]);
            referenced.Contains(n12).ShouldBeTrue();
            referenced.Contains(n12.FormKey).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n12.FormKey, typeof(INpcGetter))).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n12.FormKey, typeof(IWeaponGetter))).ShouldBeFalse();
            referenced.Contains(new FormLink<INpcGetter>(n12.FormKey)).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n12.FormKey, typeof(IMajorRecordGetter))).ShouldBeTrue();
            referenced.Contains(new FormLink<IMajorRecordGetter>(n12.FormKey)).ShouldBeTrue();
            referenced.Contains(n22).ShouldBeTrue();
            referenced.Contains(n22.FormKey).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n22.FormKey, typeof(INpcGetter))).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n22.FormKey, typeof(IWeaponGetter))).ShouldBeFalse();
            referenced.Contains(new FormLink<INpcGetter>(n22.FormKey)).ShouldBeTrue();
            referenced.Contains(new FormLinkInformation(n22.FormKey, typeof(IMajorRecordGetter))).ShouldBeTrue();
            referenced.Contains(new FormLink<IMajorRecordGetter>(n22.FormKey)).ShouldBeTrue();
        }
        
        TestUntypedResult(c => c.GetUsagesOf(r.FormKey));

        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<INpcGetter>> npcReferencedCall)
        {
            var npcReferenced = npcReferencedCall(cache);
            npcReferenced.UsageLinks.ShouldHaveCount(2);
            npcReferenced.UsageLinks.ShouldBeSubsetOf(
            [
                n12.ToLink<INpcGetter>(),
                n22.ToLink<INpcGetter>()
            ]);
            npcReferenced.Contains(n12).ShouldBeTrue();
            npcReferenced.Contains(n12.FormKey).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n12.FormKey, typeof(INpcGetter))).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n12.FormKey, typeof(IWeaponGetter))).ShouldBeFalse();
            npcReferenced.Contains(new FormLink<INpcGetter>(n12.FormKey)).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n12.FormKey, typeof(IMajorRecordGetter))).ShouldBeTrue();
            npcReferenced.Contains(new FormLink<IMajorRecordGetter>(n12.FormKey)).ShouldBeTrue();
            npcReferenced.Contains(n22).ShouldBeTrue();
            npcReferenced.Contains(n22.FormKey).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n22.FormKey, typeof(INpcGetter))).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n22.FormKey, typeof(IWeaponGetter))).ShouldBeFalse();
            npcReferenced.Contains(new FormLink<INpcGetter>(n22.FormKey)).ShouldBeTrue();
            npcReferenced.Contains(new FormLinkInformation(n22.FormKey, typeof(IMajorRecordGetter))).ShouldBeTrue();
            npcReferenced.Contains(new FormLink<IMajorRecordGetter>(n22.FormKey)).ShouldBeTrue();
        }

        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r));
        TestUntypedResult(c => c.GetUsagesOf(r));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()));
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()));
    }
}