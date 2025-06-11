using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using Shouldly;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.ReferencedBy;

public class ImmutableLoadOrderReferencedByCacheTests
{
    [Theory, MutagenModAutoData]
    public void EmptyMod(
        SkyrimMod mod,
        Race r)
    {
        var linkCache = mod.ToImmutableLinkCache();
        var cache = new ImmutableLoadOrderReferencedByCache(linkCache);
        cache.GetReferencedBy(r.FormKey)
            .ShouldBeEmpty();
        cache.GetReferencedBy<Npc>(r)
            .ShouldBeEmpty();
        cache.GetReferencedBy<Npc>(r.ToStandardizedIdentifier())
            .ShouldBeEmpty();
        cache.GetReferencedBy<Npc>(r.ToLinkGetter())
            .ShouldBeEmpty();
        cache.GetReferencedBy<Npc>(r.ToLink())
            .ShouldBeEmpty();
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
        var cache = new ImmutableLoadOrderReferencedByCache(linkCache);
        var referenced = cache.GetReferencedBy(r.FormKey);
        referenced.ShouldHaveCount(1);
        referenced.ShouldEqualEnumerable(n.FormKey);
        
        void TestResult(IReadOnlyCollection<IFormLinkGetter<INpcGetter>> npcReferenced)
        {
            npcReferenced.ShouldHaveCount(1);
            npcReferenced.ShouldEqualEnumerable(n.ToLink<INpcGetter>());
        }

        TestResult(cache.GetReferencedBy<INpcGetter>(r));
        TestResult(cache.GetReferencedBy<INpcGetter>(r.ToStandardizedIdentifier()));
        TestResult(cache.GetReferencedBy<INpcGetter>(r.ToLinkGetter()));
        TestResult(cache.GetReferencedBy<INpcGetter>(r.ToLink()));
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
        var cache = new ImmutableLoadOrderReferencedByCache(linkCache);
        var referenced = cache.GetReferencedBy(r.FormKey);
        referenced.ShouldHaveCount(2);
        referenced.ShouldBeSubsetOf([n12.FormKey, n22.FormKey]);
        
        void TestResult(IReadOnlyCollection<IFormLinkGetter<INpcGetter>> npcReferenced)
        {
            npcReferenced.ShouldHaveCount(2);
            npcReferenced.ShouldBeSubsetOf(
            [
                n12.ToLink<INpcGetter>(),
                n22.ToLink<INpcGetter>()
            ]);
        }

        TestResult(cache.GetReferencedBy<INpcGetter>(r));
        TestResult(cache.GetReferencedBy<INpcGetter>(r.ToStandardizedIdentifier()));
        TestResult(cache.GetReferencedBy<INpcGetter>(r.ToLinkGetter()));
        TestResult(cache.GetReferencedBy<INpcGetter>(r.ToLink()));
    }
}