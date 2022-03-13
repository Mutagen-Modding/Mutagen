using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
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

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public abstract class AMajorRecordEnumeration_Tests
    {
        protected abstract ISkyrimModGetter ConvertMod(SkyrimMod mod);

        public abstract bool Getter { get; }

        [Fact]
        public void Empty()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            var conv = ConvertMod(mod);
            Assert.Empty(conv.EnumerateMajorRecords());
            Assert.Empty(conv.EnumerateMajorRecords<IMajorRecord>());
            Assert.Empty(conv.EnumerateMajorRecords<IMajorRecordGetter>());
            Assert.Empty(conv.EnumerateMajorRecords<INpc>());
            Assert.Empty(conv.EnumerateMajorRecords<INpcGetter>());
            Assert.Empty(conv.EnumerateMajorRecords<Npc>());
        }

        [Fact]
        public void EnumerateAll()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            mod.Npcs.AddNew();
            mod.Ammunitions.AddNew();
            var conv = ConvertMod(mod);
            Assert.Equal(2, conv.EnumerateMajorRecords().Count());
        }

        [Fact]
        public void EnumerateAllViaGeneric()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            mod.Npcs.AddNew();
            mod.Ammunitions.AddNew();
            var conv = ConvertMod(mod);
            Assert.Equal(Getter ? 0 : 2, conv.EnumerateMajorRecords<IMajorRecord>().Count());
            Assert.Equal(2, conv.EnumerateMajorRecords<IMajorRecordGetter>().Count());
        }

        [Fact]
        public void EnumerateSpecificType_Matched()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            mod.Npcs.AddNew();
            mod.Ammunitions.AddNew();
            var conv = ConvertMod(mod);
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<INpc>().Count());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<Npc>().Count());
            Assert.Single(conv.EnumerateMajorRecords<INpcGetter>());
        }

        [Fact]
        public void EnumerateSpecificType_Unmatched()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            mod.Npcs.AddNew();
            var conv = ConvertMod(mod);
            Assert.Empty(conv.EnumerateMajorRecords<IAmmunition>());
            Assert.Empty(conv.EnumerateMajorRecords<Ammunition>());
            Assert.Empty(conv.EnumerateMajorRecords<IAmmunitionGetter>());
        }

        [Fact]
        public void EnumerateLinkInterface()
        {
            var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
            mod.Factions.AddNew();
            var conv = ConvertMod(mod);
            Assert.NotEmpty(conv.EnumerateMajorRecords<IFactionGetter>());
            Assert.NotEmpty(conv.EnumerateMajorRecords<IOwnerGetter>());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<IOwner>().Count());
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
            Assert.NotEmpty(conv.EnumerateMajorRecords<ICellGetter>());
            Assert.NotEmpty(conv.EnumerateMajorRecords<IPlacedGetter>());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<IPlaced>().Count());
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
            Assert.NotEmpty(conv.EnumerateMajorRecords<ICellGetter>());
            Assert.NotEmpty(conv.EnumerateMajorRecords<ILocationTargetableGetter>());
            Assert.Equal(Getter ? 0 : 1, conv.EnumerateMajorRecords<ILocationTargetable>().Count());
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
            conv.EnumerateMajorRecords<ISkyrimMajorRecordGetter>().Any(p => p.FormKey == placed.FormKey).Should().BeTrue();
        }
    }

    public class MajorRecordEnumeration_Tests_Direct : AMajorRecordEnumeration_Tests
    {
        public override bool Getter => false;

        protected override ISkyrimModGetter ConvertMod(SkyrimMod mod)
        {
            return mod;
        }
    }

    public class MajorRecordEnumeration_Tests_Overlay : AMajorRecordEnumeration_Tests
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
}
