using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
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
            void TestContains(Npc theNpc, bool shouldContain)
            {
                referenced.Contains(theNpc).ShouldBe(shouldContain);
                referenced.Contains(theNpc.FormKey).ShouldBe(shouldContain);
                referenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(INpcGetter))).ShouldBe(shouldContain);
                referenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IWeaponGetter))).ShouldBe(false);
                referenced.Contains(new FormLink<INpcGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
                referenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IMajorRecordGetter))).ShouldBe(shouldContain);
                referenced.Contains(new FormLink<IMajorRecordGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
            }
            TestContains(n, true);
            TestContains(n2, false);
        }

        TestUntypedResult(c => c.GetUsagesOf(r.FormKey));

        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<INpcGetter>> npcReferencedCall)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var npcReferenced = npcReferencedCall(cache);
            npcReferenced.UsageLinks.ShouldHaveCount(1);
            npcReferenced.UsageLinks.ShouldEqualEnumerable(n.ToLink<INpcGetter>());
            void TestContains(Npc theNpc, bool shouldContain)
            {
                npcReferenced.Contains(theNpc).ShouldBe(shouldContain);
                npcReferenced.Contains(theNpc.FormKey).ShouldBe(shouldContain);
                npcReferenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(INpcGetter))).ShouldBe(shouldContain);
                npcReferenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IWeaponGetter))).ShouldBe(false);
                npcReferenced.Contains(new FormLink<INpcGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
                npcReferenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IMajorRecordGetter))).ShouldBe(shouldContain);
                npcReferenced.Contains(new FormLink<IMajorRecordGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
            }
            TestContains(n, true);
            TestContains(n2, false);
        }

        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r));
        TestUntypedResult(c => c.GetUsagesOf(r));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()));
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()));
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()));
    }

    [Theory, MutagenModAutoData]
    public void MultiSelfModReference(
        SkyrimMod mod,
        Npc n,
        Npc n2,
        Npc n3,
        Race r,
        Race r2)
    {
        n.Race.SetTo(r);
        n2.Race.SetTo(r2);
        var linkCache = mod.ToImmutableLinkCache();

        void TestUntypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<IMajorRecordGetter>> referencedCall, Npc theNpc, bool shouldContain)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var referenced = referencedCall(cache);
            referenced.UsageLinks.ShouldHaveCount(1);
            if (shouldContain)
            {
                referenced.UsageLinks.ShouldEqualEnumerable(theNpc.ToLinkGetter());
            }
            referenced.Contains(theNpc).ShouldBe(shouldContain);
            referenced.Contains(theNpc.FormKey).ShouldBe(shouldContain);
            referenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(INpcGetter))).ShouldBe(shouldContain);
            referenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IWeaponGetter))).ShouldBe(false);
            referenced.Contains(new FormLink<INpcGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
            referenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IMajorRecordGetter))).ShouldBe(shouldContain);
            referenced.Contains(new FormLink<IMajorRecordGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
        }

        TestUntypedResult(c => c.GetUsagesOf(r.FormKey), n, true);
        TestUntypedResult(c => c.GetUsagesOf(r.FormKey), n2, false);
        TestUntypedResult(c => c.GetUsagesOf(r.FormKey), n3, false);
        TestUntypedResult(c => c.GetUsagesOf(r2.FormKey), n, false);
        TestUntypedResult(c => c.GetUsagesOf(r2.FormKey), n2, true);
        TestUntypedResult(c => c.GetUsagesOf(r2.FormKey), n3, false);

        void TestTypedResult(Func<ImmutableLoadOrderLinkUsageCache, ILinkUsageResults<INpcGetter>> npcReferencedCall, Npc theNpc, bool shouldContain)
        {
            var cache = new ImmutableLoadOrderLinkUsageCache(linkCache);
            var npcReferenced = npcReferencedCall(cache);
            npcReferenced.UsageLinks.ShouldHaveCount(1);
            if (shouldContain)
            {
                npcReferenced.UsageLinks.ShouldEqualEnumerable(theNpc.ToLink<INpcGetter>());
            }
            npcReferenced.Contains(theNpc).ShouldBe(shouldContain);
            npcReferenced.Contains(theNpc.FormKey).ShouldBe(shouldContain);
            npcReferenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(INpcGetter))).ShouldBe(shouldContain);
            npcReferenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IWeaponGetter))).ShouldBe(false);
            npcReferenced.Contains(new FormLink<INpcGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
            npcReferenced.Contains(new FormLinkInformation(theNpc.FormKey, typeof(IMajorRecordGetter))).ShouldBe(shouldContain);
            npcReferenced.Contains(new FormLink<IMajorRecordGetter>(theNpc.FormKey)).ShouldBe(shouldContain);
        }

        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r), n, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r), n2, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2), n, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2), n2, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2), n3, false);
        TestUntypedResult(c => c.GetUsagesOf(r), n, true);
        TestUntypedResult(c => c.GetUsagesOf(r), n2, false);
        TestUntypedResult(c => c.GetUsagesOf(r), n3, false);
        TestUntypedResult(c => c.GetUsagesOf(r2), n, false);
        TestUntypedResult(c => c.GetUsagesOf(r2), n2, true);
        TestUntypedResult(c => c.GetUsagesOf(r2), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()), n, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()), n2, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToStandardizedIdentifier()), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToStandardizedIdentifier()), n, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToStandardizedIdentifier()), n2, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToStandardizedIdentifier()), n3, false);
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()), n, true);
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()), n2, false);
        TestUntypedResult(c => c.GetUsagesOf(r.ToStandardizedIdentifier()), n3, false);
        TestUntypedResult(c => c.GetUsagesOf(r2.ToStandardizedIdentifier()), n, false);
        TestUntypedResult(c => c.GetUsagesOf(r2.ToStandardizedIdentifier()), n2, true);
        TestUntypedResult(c => c.GetUsagesOf(r2.ToStandardizedIdentifier()), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()), n, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()), n2, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLinkGetter()), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToLinkGetter()), n, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToLinkGetter()), n2, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToLinkGetter()), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()), n, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()), n2, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r.ToLink()), n3, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToLink()), n, false);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToLink()), n2, true);
        TestTypedResult(c => c.GetUsagesOf<INpcGetter>(r2.ToLink()), n3, false);
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

    [Theory, MutagenModAutoData]
    public void NestedRecord(
        SkyrimMod mod,
        Cell cell,
        PlacedObject placed,
        Static stat)
    {
        cell.Flags |= Cell.Flag.IsInteriorCell;
        mod.Cells.AddInteriorCell(cell);
        cell.Persistent.Add(placed);

        placed.Base.SetTo(stat);

        var linkCache = mod.ToImmutableLinkCache();
        var usageCache = new ImmutableLoadOrderLinkUsageCache(linkCache);

        usageCache.GetUsagesOf<ICellGetter>(stat).UsageLinks.ShouldBeEmpty();
        usageCache.GetUsagesOf<IPlacedGetter>(stat).UsageLinks.ShouldEqualEnumerable([placed.ToLink()]);
    }

    [Theory, MutagenModAutoData]
    public void NestedRecordUntypedScope(
        SkyrimMod mod,
        Cell cell,
        PlacedObject placed,
        Static stat)
    {
        cell.Flags |= Cell.Flag.IsInteriorCell;
        mod.Cells.AddInteriorCell(cell);
        cell.Persistent.Add(placed);

        placed.Base.SetTo(stat);

        var linkCache = mod.ToImmutableLinkCache();
        var usageCache = new ImmutableLoadOrderLinkUsageCache(linkCache);

        var usages = usageCache.GetUsagesOf(stat);
        usages.UsageLinks.ShouldEqualEnumerable([placed.ToLink<IMajorRecordGetter>()]);
        usages.Contains(placed).ShouldBeTrue();
        usages.Contains(cell).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void ContainingRecordOwnLinksStillCounted(
        SkyrimMod mod,
        Cell cell,
        PlacedObject placed,
        Static stat,
        LightingTemplate lightingTemplate)
    {
        cell.Flags |= Cell.Flag.IsInteriorCell;
        mod.Cells.AddInteriorCell(cell);
        cell.Persistent.Add(placed);

        cell.LightingTemplate.SetTo(lightingTemplate);
        placed.Base.SetTo(stat);

        var linkCache = mod.ToImmutableLinkCache();
        var usageCache = new ImmutableLoadOrderLinkUsageCache(linkCache);

        usageCache.GetUsagesOf<ICellGetter>(lightingTemplate).UsageLinks.ShouldEqualEnumerable([cell.ToLink<ICellGetter>()]);
        usageCache.GetUsagesOf<ICellGetter>(stat).UsageLinks.ShouldBeEmpty();
    }

    [Theory, MutagenModAutoData]
    public void SubRecordLinksStillCounted(
        SkyrimMod mod,
        Npc npc,
        Weapon weapon)
    {
        npc.Items = new ExtendedList<ContainerEntry>()
        {
            new ContainerEntry()
            {
                Item = new ContainerItem()
                {
                    Item = weapon.ToLink<IItemGetter>(),
                    Count = 1
                }
            }
        };

        var linkCache = mod.ToImmutableLinkCache();
        var usageCache = new ImmutableLoadOrderLinkUsageCache(linkCache);

        usageCache.GetUsagesOf<INpcGetter>(weapon).UsageLinks.ShouldEqualEnumerable([npc.ToLink<INpcGetter>()]);
    }

    [Theory, MutagenModAutoData]
    public void NestedRecordUsesWinningOverride(
        ModKey modKey,
        ModKey modKey2,
        Static statA,
        Static statB,
        Static statC)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var cell = new Cell(FormKey.Factory($"000800:{modKey.FileName}"), SkyrimRelease.SkyrimSE)
        {
            Flags = Cell.Flag.IsInteriorCell
        };
        mod.Cells.AddInteriorCell(cell);
        var placedA = new PlacedObject(FormKey.Factory($"000801:{modKey.FileName}"), SkyrimRelease.SkyrimSE);
        placedA.Base.SetTo(statA);
        var placedB = new PlacedObject(FormKey.Factory($"000802:{modKey.FileName}"), SkyrimRelease.SkyrimSE);
        placedB.Base.SetTo(statB);
        cell.Persistent.Add(placedA);
        cell.Persistent.Add(placedB);

        var mod2 = new SkyrimMod(modKey2, SkyrimRelease.SkyrimSE);
        var cellOverride = new Cell(cell.FormKey, SkyrimRelease.SkyrimSE)
        {
            Flags = Cell.Flag.IsInteriorCell
        };
        mod2.Cells.AddInteriorCell(cellOverride);
        var placedAOverride = new PlacedObject(placedA.FormKey, SkyrimRelease.SkyrimSE);
        placedAOverride.Base.SetTo(statC);
        cellOverride.Persistent.Add(placedAOverride);

        var linkCache = new[] { mod, mod2 }.ToImmutableLinkCache();
        var usageCache = new ImmutableLoadOrderLinkUsageCache(linkCache);

        usageCache.GetUsagesOf<IPlacedGetter>(statA).UsageLinks.ShouldBeEmpty();
        usageCache.GetUsagesOf<IPlacedGetter>(statC).UsageLinks.ShouldEqualEnumerable([placedA.ToLink<IPlacedGetter>()]);
        usageCache.GetUsagesOf<IPlacedGetter>(statB).UsageLinks.ShouldEqualEnumerable([placedB.ToLink<IPlacedGetter>()]);

        usageCache.GetUsagesOf<ICellGetter>(statA).UsageLinks.ShouldBeEmpty();
        usageCache.GetUsagesOf<ICellGetter>(statB).UsageLinks.ShouldBeEmpty();
        usageCache.GetUsagesOf<ICellGetter>(statC).UsageLinks.ShouldBeEmpty();
    }
}