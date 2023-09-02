namespace Mutagen.Bethesda.Skyrim;

public static class CellGroupExt
{
	/// <summary>
	/// Adds an interior cell to the mod using the correct block and subblock.
	/// </summary>
	/// <param name="cellGroup">Cell group of a mod</param>
	/// <param name="cell">Cell to add, must be interior cell</param>
	/// <exception cref="ArgumentException">The cell is not an interior cell</exception>
	public static void AddInteriorCell(this SkyrimListGroup<CellBlock> cellGroup, Cell cell)
	{
		if ((cell.Flags & Cell.Flag.IsInteriorCell) == 0) throw new ArgumentException(nameof(cell));

		// Formula as it can be seen here https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/CELL
		var formKeyID = cell.FormKey.ID;
		var blockNumber = (int) formKeyID % 10;
		var subBlockNumber = (int) formKeyID / 10 % 10;

		// Get or add block
		var block = cellGroup.FirstOrDefault(b => b.BlockNumber == blockNumber);
		if (block is null)
		{
			block = new CellBlock
			{
				BlockNumber = blockNumber,
				GroupType = GroupTypeEnum.InteriorCellBlock,
			};
			cellGroup.Add(block);
		}

		// Get or add subblock
		var subBlock = block.SubBlocks.FirstOrDefault(b => b.BlockNumber == subBlockNumber);
		if (subBlock is null)
		{
			subBlock = new CellSubBlock
			{
				BlockNumber = subBlockNumber,
				GroupType = GroupTypeEnum.InteriorCellSubBlock,
			};
			block.SubBlocks.Add(subBlock);
		}

		// Add cell
		subBlock.Cells.Add(cell);
	}
}
