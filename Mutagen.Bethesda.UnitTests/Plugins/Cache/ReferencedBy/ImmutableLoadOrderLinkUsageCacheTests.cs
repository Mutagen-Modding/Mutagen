using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
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
        
        void TestUnypedResult(Func<ImmutableLoadOrderLinkUsageCache, IReadOnlyCollection<FormKey>> referencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.ShouldBeEmpty();
        }
        
        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, IReadOnlyCollection<IFormLinkGetter<INpcGetter>>> referencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.ShouldBeEmpty();
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

        void TestUntypedResult(Func<ImmutableLoadOrderLinkUsageCache, IReadOnlyCollection<FormKey>> referencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.ShouldHaveCount(1);
            referenced.ShouldEqualEnumerable(n.FormKey);
        }

        TestUntypedResult(c => c.GetUsagesOf(r.FormKey));

        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, IReadOnlyCollection<IFormLinkGetter<INpcGetter>>> npcReferencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var npcReferenced = npcReferencedCall(cache);
            npcReferenced.ShouldHaveCount(1);
            npcReferenced.ShouldEqualEnumerable(n.ToLink<INpcGetter>());
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

        void TestUntypedResult(Func<ImmutableLoadOrderLinkUsageCache, IReadOnlyCollection<FormKey>> referencedCall)
        {
            var referenced = referencedCall(cache);
            referenced.ShouldHaveCount(2);
            referenced.ShouldBeSubsetOf([n12.FormKey, n22.FormKey]);
        }
        
        TestUntypedResult(c => c.GetUsagesOf(r.FormKey));

        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, IReadOnlyCollection<IFormLinkGetter<INpcGetter>>> npcReferencedCall)
        {
            var npcReferenced = npcReferencedCall(cache);
            npcReferenced.ShouldHaveCount(2);
            npcReferenced.ShouldBeSubsetOf(
            [
                n12.ToLink<INpcGetter>(),
                n22.ToLink<INpcGetter>()
            ]);
        }

        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r));
        TestUntypedResult(c => c.GetUsagesOf(r));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()));
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()));
    }
}