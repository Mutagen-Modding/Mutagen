using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
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

		mod.Cells.Should().HaveCount(1);
		var block = mod.Cells.FirstOrDefault(b => b.BlockNumber == 5);
		block.Should().NotBeNull();
		block!.SubBlocks.Should().HaveCount(1);
		var subBlock = block.SubBlocks[0];
		subBlock.BlockNumber.Should().Be(1);
		subBlock.Cells.Should().HaveCount(1);
		subBlock.Cells[0].Should().Be(cell);

		// Add add to 8 -> 0
		cell = new Cell(FormKey.Factory($"000008:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE)
		{
			Flags = Cell.Flag.IsInteriorCell
		};

		mod.Cells.AddInteriorCell(cell);

		mod.Cells.Should().HaveCount(2);
		block = mod.Cells.FirstOrDefault(b => b.BlockNumber == 8);
		block.Should().NotBeNull();
		block!.SubBlocks.Should().HaveCount(1);
		subBlock = block.SubBlocks[0];
		subBlock.BlockNumber.Should().Be(0);
		subBlock.Cells.Should().HaveCount(1);
		subBlock.Cells[0].Should().Be(cell);

		// Add add to 5 -> 0
		cell = new Cell(FormKey.Factory($"000005:{mod.ModKey.FileName}"), SkyrimRelease.SkyrimSE)
		{
			Flags = Cell.Flag.IsInteriorCell
		};

		mod.Cells.AddInteriorCell(cell);

		mod.Cells.Should().HaveCount(2);
		block = mod.Cells.FirstOrDefault(b => b.BlockNumber == 5);
		block.Should().NotBeNull();
		block!.SubBlocks.Should().HaveCount(2);
		subBlock = block.SubBlocks[1];
		subBlock.BlockNumber.Should().Be(0);
		subBlock.Cells.Should().HaveCount(1);
		subBlock.Cells[0].Should().Be(cell);
	}

	[Theory]
	[MutagenAutoData(ConfigureMembers: true)]
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

		worldspace.SubCells.Should().HaveCount(1);
		var block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == 1 && b.BlockNumberY == -1);
		block.Should().NotBeNull();
		block!.Items.Should().HaveCount(1);
		var subBlock = block.Items[0];
		subBlock.BlockNumberX.Should().Be(4);
		subBlock.BlockNumberY.Should().Be(-1);
		subBlock.Items.Should().HaveCount(1);
		subBlock.Items[0].Should().Be(cell);

		// Add to 0,0 -> 0,0
		cell = new Cell(FormKey.Null, SkyrimRelease.SkyrimSE)
		{
			Grid = new CellGrid
			{
				Point = new P2Int(1, 5),
			}
		};

		worldspace.AddCell(cell);

		worldspace.SubCells.Should().HaveCount(2);
		block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == 0 && b.BlockNumberY == 0);
		block.Should().NotBeNull();
		block!.Items.Should().HaveCount(1);
		subBlock = block.Items[0];
		subBlock.BlockNumberX.Should().Be(0);
		subBlock.BlockNumberY.Should().Be(0);
		subBlock.Items.Should().HaveCount(1);
		subBlock.Items[0].Should().Be(cell);

		// Add to 1,-1 -> 5,-1
		cell = new Cell(FormKey.Null, SkyrimRelease.SkyrimSE)
		{
			Grid = new CellGrid
			{
				Point = new P2Int(40, -7),
			}
		};

		worldspace.AddCell(cell);

		worldspace.SubCells.Should().HaveCount(2);
		block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == 1 && b.BlockNumberY == -1);
		block.Should().NotBeNull();
		block!.Items.Should().HaveCount(2);
		subBlock = block.Items[1];
		subBlock.BlockNumberX.Should().Be(5);
		subBlock.BlockNumberY.Should().Be(-1);
		subBlock.Items.Should().HaveCount(1);
		subBlock.Items[0].Should().Be(cell);
	}
}