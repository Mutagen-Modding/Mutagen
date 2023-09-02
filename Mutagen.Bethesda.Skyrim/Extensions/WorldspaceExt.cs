using Noggog;
namespace Mutagen.Bethesda.Skyrim;

public static class WorldspaceExt
{
	/// <summary>
	/// Adds an exterior cell to the worldspace in the correct block and subblock.
	/// </summary>
	/// <param name="worldspace">Worldspace to add the cell to</param>
	/// <param name="cell">Cell to add, must be exterior cell with a valid Grid coordinate</param>
	/// <exception cref="ArgumentException">The grid coordinate is not available</exception>
	public static void AddCell(this IWorldspace worldspace, Cell cell)
	{
		if (cell.Grid is null) throw new ArgumentException(nameof(cell.Grid));

		var subBlock = worldspace.GetOrAddSubBlock(cell.Grid.Point);
		subBlock.Items.Add(cell);
	}

	/// <summary>
	/// Adds a block to the worldspace if it does not already exist, and returns it.
	/// </summary>
	/// <param name="worldspace">Worldspace to add the block to</param>
	/// <param name="blockCoordinates">Coordinates of the cell to add a block for</param>
	/// <returns>The block for the specified cell coordinates</returns>
	public static WorldspaceBlock GetOrAddBlock(this IWorldspace worldspace, P2Int blockCoordinates)
	{
		// Formula as it can be seen here https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/CELL
		var blockNumberX = (short) Math.Floor(blockCoordinates.X / 32.0);
		var blockNumberY = (short) Math.Floor(blockCoordinates.Y / 32.0);

		var block = worldspace.SubCells.FirstOrDefault(b => b.BlockNumberX == blockNumberX && b.BlockNumberY == blockNumberY);
		if (block is null)
		{
			block = new WorldspaceBlock 
			{
				BlockNumberX = blockNumberX,
				BlockNumberY = blockNumberY,
				GroupType = GroupTypeEnum.ExteriorCellBlock,
			};
			worldspace.SubCells.Add(block);
		}

		return block;
	}

	/// <summary>
	/// Adds a subblock to the worldspace if it does not already exist, and returns it.
	/// This may also create a new block in the process.
	/// </summary>
	/// <param name="worldspace">Worldspace to add the block to</param>
	/// <param name="blockCoordinates">Coordinates of the cell to add a subblock for</param>
	/// <returns>The subblock for the specified cell coordinates</returns>
	public static WorldspaceSubBlock GetOrAddSubBlock(this IWorldspace worldspace, P2Int blockCoordinates)
	{
		// Formula as it can be seen here https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/CELL
		var subBlockNumberX = (short) Math.Floor(blockCoordinates.X / 8.0);
		var subBlockNumberY = (short) Math.Floor(blockCoordinates.Y / 8.0);

		var block = worldspace.GetOrAddBlock(blockCoordinates);

		var subBlock = block.Items.FirstOrDefault(b => b.BlockNumberX == subBlockNumberX && b.BlockNumberY == subBlockNumberY);
		if (subBlock is null)
		{
			subBlock = new WorldspaceSubBlock
			{
				BlockNumberX = subBlockNumberX,
				BlockNumberY = subBlockNumberY,
				GroupType = GroupTypeEnum.ExteriorCellSubBlock,
			};
			block.Items.Add(subBlock);
		}

		return subBlock;
	}
}
