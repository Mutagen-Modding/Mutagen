using Mutagen.Bethesda.Skyrim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class ModContext_Tests
    {
        [Fact]
        public void SimpleGroup()
        {
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<INpc, INpcGetter>().ToArray();
            Assert.Equal(2, contexts.Length);
            Assert.Same(contexts[0].Record, npc1);
            Assert.Same(contexts[1].Record, npc2);
            var npc2Override = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(npc2.FormKey, npc2Override.FormKey);
            Assert.Equal(1, mod2.Npcs.Count);
            Assert.Equal(2, mod.Npcs.Count);
        }

        [Fact]
        public void Cell()
        {
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cell1 = new Cell(mod.GetNextFormKey());
            var cell2 = new Cell(mod.GetNextFormKey());
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
            var cell3 = new Cell(mod.GetNextFormKey());
            subBlock2.Cells.Add(cell3);

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<ICell, ICellGetter>().ToArray();
            Assert.Equal(3, contexts.Length);
            Assert.Same(contexts[0].Record, cell1);
            Assert.Same(contexts[1].Record, cell2);
            Assert.Same(contexts[2].Record, cell3);
            var cell2Override = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(cell2.FormKey, cell2Override.FormKey);
            Assert.Equal(1, mod2.Cells.Records.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.First().Cells.Count);
        }

        [Fact]
        public void PlacedObjectInCell()
        {
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cell1 = new Cell(mod.GetNextFormKey());
            var cell2 = new Cell(mod.GetNextFormKey());
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
            var cell3 = new Cell(mod.GetNextFormKey());
            subBlock2.Cells.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey());
            cell2.Persistent.Add(placedObj);

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<IPlacedObject, IPlacedObjectGetter>().ToArray();
            Assert.Equal(1, contexts.Length);
            var placedObjOverride = contexts[0].GetOrAddAsOverride(mod2);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            Assert.Equal(1, mod2.Cells.Records.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.First().Cells.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.Count);
            Assert.Same(placedObjOverride, mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.First());
        }

        [Fact]
        public void IPlacedInCell()
        {
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cell1 = new Cell(mod.GetNextFormKey());
            var cell2 = new Cell(mod.GetNextFormKey());
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
            var cell3 = new Cell(mod.GetNextFormKey());
            subBlock2.Cells.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey());
            cell2.Persistent.Add(placedObj);

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<IPlaced, IPlacedGetter>().ToArray();
            Assert.Equal(2, contexts.Length);
            var placedNpcOverride = contexts[0].GetOrAddAsOverride(mod2);
            var placedObjOverride = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(placedNpc.FormKey, placedNpcOverride.FormKey);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            Assert.Equal(1, mod2.Cells.Records.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.Count);
            Assert.Equal(1, mod2.Cells.Records.First().SubBlocks.First().Cells.Count);
            Assert.Equal(2, mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.Count);
            Assert.Same(placedNpcOverride, mod2.Cells.Records.First().SubBlocks.First().Cells.First().Persistent.First());
        }

        [Fact]
        public void CellInWorldspace()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cell1 = new Cell(mod.GetNextFormKey());
            var cell2 = new Cell(mod.GetNextFormKey());
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
            var cell3 = new Cell(mod.GetNextFormKey());
            subBlock2.Items.Add(cell3);

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<ICell, ICellGetter>().ToArray();
            Assert.Equal(3, contexts.Length);
            Assert.Same(contexts[0].Record, cell1);
            Assert.Same(contexts[1].Record, cell2);
            Assert.Same(contexts[2].Record, cell3);
            var cell2Override = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(cell2.FormKey, cell2Override.FormKey);
            Assert.Equal(1, mod2.Worldspaces.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.First().Items.Count);
            Assert.Same(cell2Override, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First());
        }

        [Fact]
        public void PlacedInWorldspace()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cell1 = new Cell(mod.GetNextFormKey());
            var cell2 = new Cell(mod.GetNextFormKey());
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
            var cell3 = new Cell(mod.GetNextFormKey());
            subBlock2.Items.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey());
            cell2.Persistent.Add(placedObj);

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<IPlacedObject, IPlacedObjectGetter>().ToArray();
            Assert.Equal(1, contexts.Length);
            var placedObjOverride = contexts[0].GetOrAddAsOverride(mod2);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            Assert.Equal(1, mod2.Worldspaces.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.First().Items.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.Count);
            Assert.Same(placedObjOverride, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.First());
        }

        [Fact]
        public void IPlacedInWorldspace()
        {
            WarmupSkyrim.Init();
            var mod = new SkyrimMod(Utility.ModKey, SkyrimRelease.SkyrimSE);
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
            var cell1 = new Cell(mod.GetNextFormKey());
            var cell2 = new Cell(mod.GetNextFormKey());
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
            var cell3 = new Cell(mod.GetNextFormKey());
            subBlock2.Items.Add(cell3);

            var placedNpc = new PlacedNpc(mod.GetNextFormKey());
            cell2.Persistent.Add(placedNpc);
            var placedObj = new PlacedObject(mod.GetNextFormKey());
            cell2.Persistent.Add(placedObj);

            var mod2 = new SkyrimMod(Utility.ModKey2, SkyrimRelease.SkyrimSE);
            var contexts = mod.EnumerateMajorRecordContexts<IPlaced, IPlacedGetter>().ToArray();
            Assert.Equal(2, contexts.Length);
            var placedNpcOverride = contexts[0].GetOrAddAsOverride(mod2);
            var placedObjOverride = contexts[1].GetOrAddAsOverride(mod2);
            Assert.Equal(placedNpc.FormKey, placedNpcOverride.FormKey);
            Assert.Equal(placedObj.FormKey, placedObjOverride.FormKey);
            Assert.Equal(1, mod2.Worldspaces.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.Count);
            Assert.Equal(1, mod2.Worldspaces.First().SubCells.First().Items.First().Items.Count);
            Assert.Equal(2, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.Count);
            Assert.Same(placedNpcOverride, mod2.Worldspaces.First().SubCells.First().Items.First().Items.First().Persistent.First());
        }
    }
}
