using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;
namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class AddCellTests
{
	[Theory]
	[MutagenModAutoData]
	public void TestAddingMultipleInteriorCells(SkyrimMod mod)
	{
		// Add add to 5 -> 1
		var cell = new Cell(FormKey.Factory($"00000F:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE)
		{
			Flags = Cell.Flag.IsInteriorCell
		};

		mod.Cells.AddInteriorCell(cell);

		mod.Cells.ShouldHaveCount(1);
		var block = mod.Cells.FirstOrDefault(b => b.BlockNumber == 5);
		block.ShouldNotBeNull();
		block!.SubBlocks.ShouldHaveCount(1);
		var subBlock = block.SubBlocks[0];
		subBlock.BlockNumber.ShouldBe(1);
		subBlock.Cells.ShouldHaveCount(1);
		subBlock.Cells[0].ShouldBe(cell);

		// Add add to 8 -> 0
		cell = new Cell(FormKey.Factory($"000008:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE)
		{
			Flags = Cell.Flag.IsInteriorCell
		};

		mod.Cells.AddInteriorCell(cell);

		mod.Cells.ShouldHaveCount(2);
		block = mod.Cells.FirstOrDefault(b => b.BlockNumber == 8);
		block.ShouldNotBeNull();
		block!.SubBlocks.ShouldHaveCount(1);
		subBlock = block.SubBlocks[0];
		subBlock.BlockNumber.ShouldBe(0);
		subBlock.Cells.ShouldHaveCount(1);
		subBlock.Cells[0].ShouldBe(cell);

		// Add add to 5 -> 0
		cell = new Cell(FormKey.Factory($"000005:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE)
		{
			Flags = Cell.Flag.IsInteriorCell
		};

		mod.Cells.AddInteriorCell(cell);

		mod.Cells.ShouldHaveCount(2);
		block = mod.Cells.FirstOrDefault(b => b.BlockNumber == 5);
		block.ShouldNotBeNull();
		block!.SubBlocks.ShouldHaveCount(2);
		subBlock = block.SubBlocks[1];
		subBlock.BlockNumber.ShouldBe(0);
		subBlock.Cells.ShouldHaveCount(1);
		subBlock.Cells[0].ShouldBe(cell);
	}

	[Theory]
	[MutagenModAutoData()]
	public void TestAddingMultipleExteriorCells(SkyrimMod mod, Worldspace worldspace)
	{
		worldspace.SubCells.Clear();

		// Add add to 1,-1 -> 4,-1
		var cell = new Cell(FormKey.Null, SkyrimRelease.SkyrimSE)
		{
			Grid = new CellGrid
			{
				Point = new P2Int(34, -3),
			}
		};

		worldspace.AddCell(cell);

		worldspace.SubCells.ShouldHaveCount(1);
		var block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == 1 && b.BlockNumberY == -1);
		block.ShouldNotBeNull();
		block!.Items.ShouldHaveCount(1);
		var subBlock = block.Items[0];
		subBlock.BlockNumberX.ShouldEqual(4);
		subBlock.BlockNumberY.ShouldEqual(-1);
		subBlock.Items.ShouldHaveCount(1);
		subBlock.Items[0].ShouldBe(cell);

		// Add to 0,0 -> 0,0
		cell = new Cell(FormKey.Null, SkyrimRelease.SkyrimSE)
		{
			Grid = new CellGrid
			{
				Point = new P2Int(1, 5),
			}
		};

		worldspace.AddCell(cell);

		worldspace.SubCells.ShouldHaveCount(2);
		block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == 0 && b.BlockNumberY == 0);
		block.ShouldNotBeNull();
		block!.Items.ShouldHaveCount(1);
		subBlock = block.Items[0];
		subBlock.BlockNumberX.ShouldEqual(0);
		subBlock.BlockNumberY.ShouldEqual(0);
		subBlock.Items.ShouldHaveCount(1);
		subBlock.Items[0].ShouldBe(cell);

		// Add to 1,-1 -> 5,-1
		cell = new Cell(FormKey.Null, SkyrimRelease.SkyrimSE)
		{
			Grid = new CellGrid
			{
				Point = new P2Int(40, -7),
			}
		};

		worldspace.AddCell(cell);

		worldspace.SubCells.ShouldHaveCount(2);
		block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == 1 && b.BlockNumberY == -1);
		block.ShouldNotBeNull();
		block!.Items.ShouldHaveCount(2);
		subBlock = block.Items[1];
		subBlock.BlockNumberX.ShouldEqual(5);
		subBlock.BlockNumberY.ShouldEqual(-1);
		subBlock.Items.ShouldHaveCount(1);
		subBlock.Items[0].ShouldBe(cell);
	}
}