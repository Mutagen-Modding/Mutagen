using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Linq;
using Xunit;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache
{
    public class ModContext_Tests
    {
        [Fact]
        public void SimpleGroup()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<INpc, INpcGetter>(linkCache: null!).ToArray();
            contexts.Should().HaveCount(2);
            contexts[0].Record.Should().Be(npc1);
            contexts[1].Record.Should().Be(npc2);
            var npc2Override = contexts[1].GetOrAddAsOverride(mod2);
            npc2.FormKey.Should().BeEquivalentTo(npc2Override.FormKey);
            mod2.Npcs.Should().HaveCount(1);
            mod.Npcs.Should().HaveCount(2);
        }

        [Fact]
        public void Cell()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var block = new CellBlock()
            {
                BlockNumber = 2,
                GroupType = GroupTypeEnum.InteriorCellBlock,
            };
            var subBlock = new CellSubBlock()
            {
                BlockNumber = 4,
                GroupType = GroupTypeEnum.InteriorCellSubBlock,
            };
            block.SubBlocks.Add(subBlock);
            mod.Cells.Records.Add(block);
            var cell1 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            var cell2 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Cells.Add(cell1);
            subBlock.Cells.Add(cell2);
            var block2 = new CellBlock()
            {
                BlockNumber = 5,
                GroupType = GroupTypeEnum.InteriorCellBlock,
            };
            var subBlock2 = new CellSubBlock()
            {
                BlockNumber = 8,
                GroupType = GroupTypeEnum.InteriorCellSubBlock,
            };
            block2.SubBlocks.Add(subBlock2);
            mod.Cells.Records.Add(block2);
            var cell3 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock2.Cells.Add(cell3);

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<ICell, ICellGetter>(linkCache: null!).ToArray();
            contexts.Should().HaveCount(3);
            Assert.Same(contexts[0].Record, cell1);
            Assert.Same(contexts[1].Record, cell2);
            Assert.Same(contexts[2].Record, cell3);
            var cell2Override = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(cell2.FormKey, cell2Override.FormKey);
            mod2.Cells.Records.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.First().Cells.Should().HaveCount(1);
        }

        [Fact]
        public void PlacedObjectInCell()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var block = new CellBlock()
            {
                BlockNumber = 2,
                GroupType = GroupTypeEnum.InteriorCellBlock,
            };
            var subBlock = new CellSubBlock()
            {
                BlockNumber = 4,
                GroupType = GroupTypeEnum.InteriorCellSubBlock,
            };
            block.SubBlocks.Add(subBlock);
            mod.Cells.Records.Add(block);
            var cell1 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            var cell2 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Cells.Add(cell1);
            subBlock.Cells.Add(cell2);
            var block2 = new CellBlock()
            {
                BlockNumber = 5,
                GroupType = GroupTypeEnum.InteriorCellBlock,
            };
            var subBlock2 = new CellSubBlock()
            {
                BlockNumber = 8,
                GroupType = GroupTypeEnum.InteriorCellSubBlock,
            };
            block2.SubBlocks.Add(subBlock2);
            mod.Cells.Records.Add(block2);
            var cell3 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock2.Cells.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedObj);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<IPlacedObject, IPlacedObjectGetter>(linkCache: cache).ToArray();
            contexts.Should().HaveCount(1);

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var placedObjOverride = contexts[0].GetOrAddAsOverride(mod2);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            mod2.Cells.Records.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.First().Cells.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.Should().HaveCount(1);
            Assert.Same(placedObjOverride, mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.First());
        }

        [Fact]
        public void IPlacedInCell()
        {
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var block = new CellBlock()
            {
                BlockNumber = 2,
                GroupType = GroupTypeEnum.InteriorCellBlock,
            };
            var subBlock = new CellSubBlock()
            {
                BlockNumber = 4,
                GroupType = GroupTypeEnum.InteriorCellSubBlock,
            };
            block.SubBlocks.Add(subBlock);
            mod.Cells.Records.Add(block);
            var cell1 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            var cell2 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Cells.Add(cell1);
            subBlock.Cells.Add(cell2);
            var block2 = new CellBlock()
            {
                BlockNumber = 5,
                GroupType = GroupTypeEnum.InteriorCellBlock,
            };
            var subBlock2 = new CellSubBlock()
            {
                BlockNumber = 8,
                GroupType = GroupTypeEnum.InteriorCellSubBlock,
            };
            block2.SubBlocks.Add(subBlock2);
            mod.Cells.Records.Add(block2);
            var cell3 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock2.Cells.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedObj);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<IPlaced, IPlacedGetter>(linkCache: cache).ToArray();
            Assert.Equal(2, contexts.Length);

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var placedNpcOverride = contexts[0].GetOrAddAsOverride(mod2);
            var placedObjOverride = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(placedNpc.FormKey, placedNpcOverride.FormKey);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            mod2.Cells.Records.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.First().Cells.Should().HaveCount(1);
            mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.Should().HaveCount(2);
            Assert.Same(placedNpcOverride, mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.First());
        }

        [Fact]
        public void CellInWorldspace()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var worldspace = mod.Worldspaces.AddNew();
            var block = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var cell1 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            var cell2 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Items.Add(cell1);
            subBlock.Items.Add(cell2);
            var block2 = new WorldspaceBlock()
            {
                BlockNumberX = 5,
                BlockNumberY = 6,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock2 = new WorldspaceSubBlock()
            {
                BlockNumberX = 8,
                BlockNumberY = 9,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block2.Items.Add(subBlock2);
            worldspace.SubCells.Add(block2);
            var cell3 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock2.Items.Add(cell3);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<ICell, ICellGetter>(linkCache: cache).ToArray();
            Assert.Equal(3, contexts.Length);
            Assert.Same(contexts[0].Record, cell1);
            Assert.Same(contexts[1].Record, cell2);
            Assert.Same(contexts[2].Record, cell3);

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var cell2Override = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(cell2.FormKey, cell2Override.FormKey);
            mod2.Worldspaces.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.First().Items.Should().HaveCount(1);
            Assert.Same(cell2Override, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First());
        }

        [Fact]
        public void PlacedInWorldspace()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var worldspace = mod.Worldspaces.AddNew();
            var block = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var cell1 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            var cell2 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Items.Add(cell1);
            subBlock.Items.Add(cell2);
            var block2 = new WorldspaceBlock()
            {
                BlockNumberX = 5,
                BlockNumberY = 6,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock2 = new WorldspaceSubBlock()
            {
                BlockNumberX = 8,
                BlockNumberY = 9,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block2.Items.Add(subBlock2);
            worldspace.SubCells.Add(block2);
            var cell3 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock2.Items.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedObj);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<IPlacedObject, IPlacedObjectGetter>(linkCache: cache).ToArray();
            contexts.Should().HaveCount(1);

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var placedObjOverride = contexts[0].GetOrAddAsOverride(mod2);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            mod2.Worldspaces.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.First().Items.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.Should().HaveCount(1);
            Assert.Same(placedObjOverride, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.First());
        }

        [Fact]
        public void IPlacedInWorldspace()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var worldspace = mod.Worldspaces.AddNew();
            var block = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var cell1 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            var cell2 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Items.Add(cell1);
            subBlock.Items.Add(cell2);
            var block2 = new WorldspaceBlock()
            {
                BlockNumberX = 5,
                BlockNumberY = 6,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock2 = new WorldspaceSubBlock()
            {
                BlockNumberX = 8,
                BlockNumberY = 9,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block2.Items.Add(subBlock2);
            worldspace.SubCells.Add(block2);
            var cell3 = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock2.Items.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell2.Persistent.Add(placedObj);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<IPlaced, IPlacedGetter>(linkCache: cache).ToArray();
            Assert.Equal(2, contexts.Length);

            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var placedNpcOverride = contexts[0].GetOrAddAsOverride(mod2);
            var placedObjOverride = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(placedNpc.FormKey, placedNpcOverride.FormKey);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            mod2.Worldspaces.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.First().Items.Should().HaveCount(1);
            mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.Should().HaveCount(2);
            Assert.Same(placedNpcOverride, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.First());
        }

        [Fact]
        public void ComplexOverrides()
        {
            WarmupSkyrim.Init();

            // Construct base mod
            const string Mod1Name = "Mod1";
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var worldspace = mod.Worldspaces.AddNew();
            worldspace.EditorID = Mod1Name;
            var block = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell.EditorID = Mod1Name;
            subBlock.Items.Add(cell);

            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            placedObj.EditorID = Mod1Name;
            cell.Persistent.Add(placedObj);

            // Override cell in 2nd mod
            const string Mod2Name = "Mod2";
            var mod2 = new SkyrimMod(Utility.PluginModKey2, SkyrimRelease.SkyrimSE);
            var worldspace2 = new Worldspace(worldspace.FormKey, SkyrimRelease.SkyrimSE);
            worldspace2.EditorID = Mod2Name;
            mod2.Worldspaces.Add(worldspace2);
            var block2 = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock2 = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block2.Items.Add(subBlock2);
            worldspace2.SubCells.Add(block2);
            var cell2 = new Cell(cell.FormKey, SkyrimRelease.SkyrimSE);
            cell2.EditorID = Mod2Name;
            subBlock2.Items.Add(cell2);

            // Override worldspace in 3rd mod
            const string Mod3Name = "Mod3";
            var mod3 = new SkyrimMod(Utility.PluginModKey3, SkyrimRelease.SkyrimSE);
            var worldspace3 = new Worldspace(worldspace.FormKey, SkyrimRelease.SkyrimSE);
            worldspace3.EditorID = Mod3Name;
            mod3.Worldspaces.Add(worldspace3);

            // Override in 4th, and check sources
            var loadOrder = new LoadOrder<ISkyrimModGetter>(mod.AsEnumerable().And(mod2).And(mod3));
            var cache = loadOrder.ToImmutableLinkCache();

            var contexts = mod.EnumerateMajorRecordContexts<IPlaced, IPlacedGetter>(linkCache: cache).ToArray();
            contexts.Should().HaveCount(1);

            var mod4 = new SkyrimMod(Utility.PluginModKey4, SkyrimRelease.SkyrimSE);
            var placedObjOverride = contexts[0].GetOrAddAsOverride(mod4);
            placedObjOverride.EditorID.Should().BeEquivalentTo(Mod1Name);
            var cellOverride = mod4.Worldspaces.First().SubCells.First().Items.First().Items.First();
            cellOverride.EditorID.Should().BeEquivalentTo(Mod2Name);
            var worldspaceOverride = mod4.Worldspaces.First();
            worldspaceOverride.EditorID.Should().BeEquivalentTo(Mod3Name);
        }

        [Fact]
        public void SetModKeys()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var worldspace = mod.Worldspaces.AddNew();
            var block = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Items.Add(cell);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell.Persistent.Add(placedObj);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<IPlacedObject, IPlacedObjectGetter>(linkCache: cache).ToArray();
            contexts.Should().HaveCount(1);
            contexts[0].ModKey.Should().BeEquivalentTo(Utility.PluginModKey);
        }

        [Fact]
        public void ParentRefs()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.PluginModKey, SkyrimRelease.SkyrimSE);
            var worldspace = mod.Worldspaces.AddNew();
            var block = new WorldspaceBlock()
            {
                BlockNumberX = 2,
                BlockNumberY = 3,
                GroupType = GroupTypeEnum.ExteriorCellBlock,
            };
            var subBlock = new WorldspaceSubBlock()
            {
                BlockNumberX = 4,
                BlockNumberY = 5,
                GroupType = GroupTypeEnum.ExteriorCellSubBlock,
            };
            block.Items.Add(subBlock);
            worldspace.SubCells.Add(block);
            var cell = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            subBlock.Items.Add(cell);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
            cell.Persistent.Add(placedObj);

            var cache = mod.ToImmutableLinkCache();
            var contexts = mod.EnumerateMajorRecordContexts<IPlacedObject, IPlacedObjectGetter>(linkCache: cache).ToArray();
            contexts.Should().HaveCount(1);
            var baseContext = contexts[0];
            var cellContext = baseContext.Parent;
            cellContext.Should().BeOfType(typeof(ModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>));
            cellContext!.Record.Should().Be(cell);
            var subBlockContext = cellContext.Parent;
            subBlockContext!.Record.Should().Be(subBlock);
            var blockContext = subBlockContext.Parent;
            blockContext!.Record.Should().Be(block);
            var worldspaceContext = blockContext.Parent;
            worldspaceContext!.Record.Should().Be(worldspace);
            baseContext.IsUnderneath<IWorldspaceGetter>().Should().BeTrue();
            baseContext.TryGetParent<IWorldspaceGetter>(out var worldParent).Should().BeTrue();
            worldParent.Should().Be(worldspace);
            baseContext.TryGetParentContext<IWorldspace, IWorldspaceGetter>(out var worldParentContext).Should().BeTrue();
            worldParentContext!.Record.Should().Be(worldspace);
        }
    }
}
