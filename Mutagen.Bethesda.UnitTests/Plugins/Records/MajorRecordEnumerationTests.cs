using System.Collections.Generic;
using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.Linq;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Testing;
using Xunit;
using Ammunition = Mutagen.Bethesda.Skyrim.Ammunition;
using Cell = Mutagen.Bethesda.Skyrim.Cell;
using CellBlock = Mutagen.Bethesda.Skyrim.CellBlock;
using CellSubBlock = Mutagen.Bethesda.Skyrim.CellSubBlock;
using GroupTypeEnum = Mutagen.Bethesda.Skyrim.GroupTypeEnum;
using IAmmunition = Mutagen.Bethesda.Skyrim.IAmmunition;
using IAmmunitionGetter = Mutagen.Bethesda.Skyrim.IAmmunitionGetter;
using ICellGetter = Mutagen.Bethesda.Skyrim.ICellGetter;
using IFactionGetter = Mutagen.Bethesda.Skyrim.IFactionGetter;
using INpc = Mutagen.Bethesda.Skyrim.INpc;
using INpcGetter = Mutagen.Bethesda.Skyrim.INpcGetter;
using IOwner = Mutagen.Bethesda.Skyrim.IOwner;
using IOwnerGetter = Mutagen.Bethesda.Skyrim.IOwnerGetter;
using IPlaced = Mutagen.Bethesda.Skyrim.IPlaced;
using IPlacedGetter = Mutagen.Bethesda.Skyrim.IPlacedGetter;
using Npc = Mutagen.Bethesda.Skyrim.Npc;
using PlacedNpc = Mutagen.Bethesda.Skyrim.PlacedNpc;
using Worldspace = Mutagen.Bethesda.Skyrim.Worldspace;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public abstract class AMajorRecordEnumerationTests
{
    protected abstract ISkyrimModGetter ConvertMod(SkyrimMod mod);

    public abstract bool Getter { get; }

    public abstract IEnumerable<object> RunTest(ISkyrimModGetter mod);

    public abstract IEnumerable<TTarget> RunTest<TSetter, TTarget>(ISkyrimModGetter mod)
        where TSetter : class, IMajorRecordQueryable, TTarget
        where TTarget : class, IMajorRecordQueryableGetter;

    [Fact]
    public void Empty()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var conv = ConvertMod(mod);
        Assert.Empty(RunTest(conv));
        Assert.Empty(RunTest<IMajorRecord, IMajorRecord>(conv));
        Assert.Empty(RunTest<IMajorRecord, IMajorRecordGetter>(conv));
        Assert.Empty(RunTest<INpc, INpc>(conv));
        Assert.Empty(RunTest<INpc, INpcGetter>(conv));
        Assert.Empty(RunTest<Npc, Npc>(conv));
    }

    [Fact]
    public void EnumerateAll()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        mod.Npcs.AddNew();
        mod.Ammunitions.AddNew();
        var conv = ConvertMod(mod);
        Assert.Equal(2, RunTest(conv).Count());
    }

    [Fact]
    public void EnumerateAllViaGeneric()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        mod.Npcs.AddNew();
        mod.Ammunitions.AddNew();
        var conv = ConvertMod(mod);
        Assert.Equal(Getter ? 0 : 2, RunTest<IMajorRecord, IMajorRecord>(conv).Count());
        Assert.Equal(2, RunTest<IMajorRecord, IMajorRecordGetter>(conv).Count());
    }

    [Fact]
    public void EnumerateSpecificType_Matched()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        mod.Npcs.AddNew();
        mod.Ammunitions.AddNew();
        var conv = ConvertMod(mod);
        Assert.Equal(Getter ? 0 : 1, RunTest<INpc, INpc>(conv).Count());
        Assert.Equal(Getter ? 0 : 1, RunTest<Npc, Npc>(conv).Count());
        Assert.Single(RunTest<INpc, INpcGetter>(conv));
    }

    [Fact]
    public void EnumerateSpecificType_Unmatched()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        mod.Npcs.AddNew();
        var conv = ConvertMod(mod);
        Assert.Empty(RunTest<IAmmunition, IAmmunition>(conv));
        Assert.Empty(RunTest<Ammunition, Ammunition>(conv));
        Assert.Empty(RunTest<IAmmunition, IAmmunitionGetter>(conv));
    }

    [Fact]
    public void EnumerateLinkInterface()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        mod.Factions.AddNew();
        var conv = ConvertMod(mod);
        Assert.NotEmpty(RunTest<IFaction, IFactionGetter>(conv));
        Assert.NotEmpty(RunTest<IOwner, IOwnerGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<IOwner, IOwner>(conv).Count());
    }

    [Fact]
    public void EnumerateDeepLinkInterface()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var placed = new PlacedNpc(mod);
        mod.Cells.Records.Add(new CellBlock()
        {
            BlockNumber = 0,
            GroupType = GroupTypeEnum.InteriorCellBlock,
            LastModified = 4,
            SubBlocks =
            {
                new CellSubBlock()
                {
                    BlockNumber = 0,
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    LastModified = 4,
                    Cells =
                    {
                        new Cell(mod)
                        {
                            Persistent =
                            {
                                placed
                            }
                        }
                    }
                }
            }
        });
        var conv = ConvertMod(mod);
        Assert.NotEmpty(RunTest<ICell, ICellGetter>(conv));
        Assert.NotEmpty(RunTest<IPlaced, IPlacedGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<IPlaced, IPlaced>(conv).Count());
    }

    [Fact]
    public void EnumerateDeepLinkInterface2()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var placed = new PlacedNpc(mod);
        mod.Worldspaces.Add(new Worldspace(mod)
        {
            TopCell = 
                new Cell(mod)
                {
                    Temporary = new ExtendedList<IPlaced>()
                    {
                        placed
                    }
                }
        });
        var conv = ConvertMod(mod);
        Assert.NotEmpty(RunTest<ICell, ICellGetter>(conv));
        Assert.NotEmpty(RunTest<ILocationTargetable, ILocationTargetableGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<ILocationTargetable, ILocationTargetable>(conv).Count());
    }

    [Fact]
    public void EnumerateDeepMajorRecordType()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var placed = new PlacedNpc(mod);
        mod.Cells.Records.Add(new CellBlock()
        {
            BlockNumber = 0,
            GroupType = GroupTypeEnum.InteriorCellBlock,
            LastModified = 4,
            SubBlocks =
            {
                new CellSubBlock()
                {
                    BlockNumber = 0,
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    LastModified = 4,
                    Cells =
                    {
                        new Cell(mod)
                        {
                            Persistent =
                            {
                                placed
                            }
                        }
                    }
                }
            }
        });
        var conv = ConvertMod(mod);
        RunTest<ISkyrimMajorRecord, ISkyrimMajorRecordGetter>(conv).Any(p => p.FormKey == placed.FormKey).Should().BeTrue();
    }

    [Fact]
    public void EnumerateAspectInterface()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var light = mod.Lights.AddNew();
        light.Icons = new Icons()
        {
            LargeIconFilename = "Hello",
            SmallIconFilename = "World"
        };
        var conv = ConvertMod(mod);
        Assert.Equal(Getter ? 0 : 1, RunTest<IHasIcons, IHasIcons>(conv).Count());
        Assert.Single(RunTest<IHasIcons, IHasIconsGetter>(conv));
        var item = RunTest<IHasIcons, IHasIconsGetter>(conv).First();
        item.Icons.Should().NotBeNull();
        item.Icons!.LargeIconFilename.Should().Be("Hello");
        item.Icons!.SmallIconFilename.Should().Be("World");
    }

    [Fact]
    public void EnumerateNullableAspectInterfaceWithNpc()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        npc.Name = "Hello";
        var conv = ConvertMod(mod);
        Assert.Equal(Getter ? 0 : 1, RunTest<ITranslatedNamed, ITranslatedNamed>(conv).Count());
        Assert.Single(RunTest<ITranslatedNamed, ITranslatedNamedGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<ITranslatedNamedRequired, ITranslatedNamedRequired>(conv).Count());
        Assert.Single(RunTest<ITranslatedNamedRequired, ITranslatedNamedRequiredGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<INamed, INamed>(conv).Count());
        Assert.Single(RunTest<INamed, INamedGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<INamedRequired, INamedRequired>(conv).Count());
        Assert.Single(RunTest<INamedRequired, INamedRequiredGetter>(conv));
        RunTest<INamed, INamedGetter>(conv).First().Name.Should().Be("Hello");
    }

    [Fact]
    public void EnumerateNonNullableAspectInterfaceWithClass()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var classObj = mod.Classes.AddNew();
        classObj.Name = "Hello";
        var conv = ConvertMod(mod);
        Assert.Empty(RunTest<INamed, INamed>(conv));
        Assert.Empty(RunTest<INamed, INamedGetter>(conv));
        Assert.Empty(RunTest<ITranslatedNamed, ITranslatedNamed>(conv));
        Assert.Empty(RunTest<ITranslatedNamed, ITranslatedNamedGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<INamedRequired, INamedRequired>(conv).Count());
        Assert.Single(RunTest<INamedRequired, INamedRequiredGetter>(conv));
        Assert.Equal(Getter ? 0 : 1, RunTest<ITranslatedNamedRequired, ITranslatedNamedRequired>(conv).Count());
        Assert.Single(RunTest<ITranslatedNamedRequired, ITranslatedNamedRequiredGetter>(conv));
        RunTest<INamedRequired, INamedRequiredGetter>(conv).First().Name.Should().Be("Hello");
    }

    [Fact]
    public void EnumerateNonMajorAspectInterfaceWithPackage()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var package = mod.Packages.AddNew();

        package.Data[4] = new PackageDataFloat()
        {
            Name = "Hello"
        };
        var conv = ConvertMod(mod);
        Assert.Empty(RunTest<INamed, INamed>(conv));
        Assert.Empty(RunTest<INamed, INamedGetter>(conv));
        Assert.Empty(RunTest<ITranslatedNamed, ITranslatedNamed>(conv));
        Assert.Empty(RunTest<ITranslatedNamed, ITranslatedNamedGetter>(conv));
        Assert.Empty(RunTest<INamedRequired, INamedRequired>(conv));
        Assert.Empty(RunTest<INamedRequired, INamedRequiredGetter>(conv));
        Assert.Empty(RunTest<ITranslatedNamedRequired, ITranslatedNamedRequired>(conv));
        Assert.Empty(RunTest<ITranslatedNamedRequired, ITranslatedNamedRequiredGetter>(conv));
    }
}

public abstract class AMajorRecordEnumerationDirectTests : AMajorRecordEnumerationTests
{
    public override bool Getter => false;

    protected override ISkyrimModGetter ConvertMod(SkyrimMod mod)
    {
        return mod;
    }
}

public class MajorRecordEnumerationDirectTests : AMajorRecordEnumerationDirectTests
{
    public override IEnumerable<object> RunTest(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecords();
    }

    public override IEnumerable<TTarget> RunTest<TSetter, TTarget>(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecords<TTarget>();
    }
}

public class MajorRecordContextEnumerationDirectTests : AMajorRecordEnumerationDirectTests
{
    public override IEnumerable<object> RunTest(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(mod.ToImmutableLinkCache());
    }

    public override IEnumerable<TTarget> RunTest<TSetter, TTarget>(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecordContexts<TSetter, TTarget>(mod.ToImmutableLinkCache())
            .Select(x => x.Record);
    }
}

public abstract class AMajorRecordEnumerationOverlayTests : AMajorRecordEnumerationTests
{
    public override bool Getter => true;

    protected override ISkyrimModGetter ConvertMod(SkyrimMod mod)
    {
        var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;
        return SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);
    }
}

public class MajorRecordEnumerationOverlayTests : AMajorRecordEnumerationOverlayTests
{
    public override IEnumerable<object> RunTest(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecords();
    }

    public override IEnumerable<TTarget> RunTest<TSetter, TTarget>(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecords<TTarget>();
    }
}

public class MajorRecordContextEnumerationOverlayTests : AMajorRecordEnumerationOverlayTests
{
    public override IEnumerable<object> RunTest(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(mod.ToImmutableLinkCache());
    }

    public override IEnumerable<TTarget> RunTest<TSetter, TTarget>(ISkyrimModGetter mod)
    {
        return mod.EnumerateMajorRecordContexts<TSetter, TTarget>(mod.ToImmutableLinkCache())
            .Select(x => x.Record);
    }
}