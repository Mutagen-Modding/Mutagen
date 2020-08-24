using Mutagen.Bethesda.Oblivion;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Remove_Tests
    {
        [Fact]
        public void Typeless_Typical()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var npc1 = mod.Npcs.AddNew();
            var npc2 = mod.Npcs.AddNew();
            mod.Remove(npc2.FormKey);
            Assert.Single(mod.Npcs);
            Assert.Same(npc1, mod.Npcs.First());
        }

        [Fact]
        public void Typeless_Blocked()
        {
            var mod = new OblivionMod(Utility.ModKey);
            var cell = new Cell(mod.GetNextFormKey());
            var subBlock = new CellSubBlock()
            {
                BlockNumber = 1,
                Cells =
                {
                    cell
                }
            };
            var cell2 = new Cell(mod.GetNextFormKey());
            var subBlock2 = new CellSubBlock()
            {
                BlockNumber = 2,
                Cells =
                {
                    cell2
                }
            };
            var block = new CellBlock()
            {
                BlockNumber = 1,
                SubBlocks =
                {
                    subBlock,
                    subBlock2,
                }
            };
            mod.Cells.Records.Add(block);
            mod.Remove(cell2.FormKey);
            Assert.Single(mod.Cells.Records);
            Assert.Single(mod.Cells.Records.First().SubBlocks);
            Assert.Same(subBlock, mod.Cells.Records.First().SubBlocks.First());
            Assert.Single(mod.Cells.Records.First().SubBlocks.First().Cells);
            Assert.Same(cell, mod.Cells.Records.First().SubBlocks.First().Cells.First());
        }
    }
}
