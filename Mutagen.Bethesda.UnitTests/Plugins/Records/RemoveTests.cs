using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class RemoveTests
{
    [Fact]
    public void Typeless_Typical()
    {
        var mod = new OblivionMod(TestConstants.PluginModKey,
            OblivionRelease.Oblivion);
        var npc1 = mod.Npcs.AddNew();
        var npc2 = mod.Npcs.AddNew();
        mod.Remove(npc2.FormKey);
        Assert.Single(mod.Npcs);
        Assert.Same(npc1, mod.Npcs.First());
    }

    [Fact]
    public void Typeless_Blocked()
    {
        var mod = new OblivionMod(TestConstants.PluginModKey,
            OblivionRelease.Oblivion);
        var cell = new Cell(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        var subBlock = new CellSubBlock()
        {
            BlockNumber = 1,
            Cells =
            {
                cell
            }
        };
        var cell2 = new Cell(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
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

    [Fact]
    public void Typeless_Deep()
    {
        var mod = new OblivionMod(TestConstants.PluginModKey,
            OblivionRelease.Oblivion);
        var placed1 = new PlacedObject(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        var placed2 = new PlacedObject(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        var cell = new Cell(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        cell.Temporary.Add(placed1);
        cell.Temporary.Add(placed2);
        var subBlock = new CellSubBlock()
        {
            BlockNumber = 1,
            Cells =
            {
                cell
            }
        };
        var block = new CellBlock()
        {
            BlockNumber = 1,
            SubBlocks =
            {
                subBlock,
            }
        };
        mod.Cells.Records.Add(block);
        mod.Remove(placed2.FormKey);
        Assert.Single(mod.Cells.Records);
        Assert.Single(mod.Cells.Records.First().SubBlocks);
        Assert.Same(subBlock, mod.Cells.Records.First().SubBlocks.First());
        Assert.Single(mod.Cells.Records.First().SubBlocks.First().Cells);
        Assert.Same(cell, mod.Cells.Records.First().SubBlocks.First().Cells.First());
        Assert.Single(mod.Cells.Records.First().SubBlocks.First().Cells.First().Temporary);
        Assert.Same(placed1, mod.Cells.Records.First().SubBlocks.First().Cells.First().Temporary.First());
    }

    [Fact]
    public void Typed_Typical()
    {
        var mod = new OblivionMod(TestConstants.PluginModKey,
            OblivionRelease.Oblivion);
        var npc1 = mod.Npcs.AddNew();
        var npc2 = mod.Npcs.AddNew();
        var weapon = mod.Weapons.AddNew();
        mod.Remove(npc2);
        Assert.Single(mod.Npcs);
        Assert.Same(npc1, mod.Npcs.First());
        Assert.Single(mod.Weapons);
        Assert.Same(weapon, mod.Weapons.First());
    }

    [Fact]
    public void Typed_Blocked()
    {
        var mod = new OblivionMod(TestConstants.PluginModKey,
            OblivionRelease.Oblivion);
        var npc1 = mod.Npcs.AddNew();
        var cell = new Cell(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        var subBlock = new CellSubBlock()
        {
            BlockNumber = 1,
            Cells =
            {
                cell
            }
        };
        var cell2 = new Cell(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
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
        mod.Remove(cell2);
        Assert.Single(mod.Cells.Records);
        Assert.Single(mod.Cells.Records.First().SubBlocks);
        Assert.Same(subBlock, mod.Cells.Records.First().SubBlocks.First());
        Assert.Single(mod.Cells.Records.First().SubBlocks.First().Cells);
        Assert.Same(cell, mod.Cells.Records.First().SubBlocks.First().Cells.First());
        Assert.Single(mod.Npcs);
        Assert.Same(npc1, mod.Npcs.First());
    }

    [Fact]
    public void Typed_Deep()
    {
        var mod = new OblivionMod(TestConstants.PluginModKey,
            OblivionRelease.Oblivion);
        var placed1 = new PlacedObject(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        var placed2 = new PlacedObject(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        var cell = new Cell(mod.GetNextFormKey(),
            OblivionRelease.Oblivion);
        cell.Temporary.Add(placed1);
        cell.Temporary.Add(placed2);
        var subBlock = new CellSubBlock()
        {
            BlockNumber = 1,
            Cells =
            {
                cell
            }
        };
        var block = new CellBlock()
        {
            BlockNumber = 1,
            SubBlocks =
            {
                subBlock,
            }
        };
        mod.Cells.Records.Add(block);
        mod.Remove(placed2);
        Assert.Single(mod.Cells.Records);
        Assert.Single(mod.Cells.Records.First().SubBlocks);
        Assert.Same(subBlock, mod.Cells.Records.First().SubBlocks.First());
        Assert.Single(mod.Cells.Records.First().SubBlocks.First().Cells);
        Assert.Same(cell, mod.Cells.Records.First().SubBlocks.First().Cells.First());
        Assert.Single(mod.Cells.Records.First().SubBlocks.First().Cells.First().Temporary);
        Assert.Same(placed1, mod.Cells.Records.First().SubBlocks.First().Cells.First().Temporary.First());
    }
}