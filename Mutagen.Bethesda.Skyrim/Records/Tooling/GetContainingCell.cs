using System.Diagnostics.CodeAnalysis;
using Noggog;
using CellContext =
    Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod,
        Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Skyrim.ICell, Mutagen.Bethesda.Skyrim.ICellGetter>;
using PlacedContext =
    Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod,
        Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Skyrim.IPlaced,
        Mutagen.Bethesda.Skyrim.IPlacedGetter>;

namespace Mutagen.Bethesda.Skyrim.Records.Tooling;

internal static class GetContainingCell
{
    internal static bool TryGetContainingCell(PlacedContext placedContext,
        WorldspaceCellLocationCache worldspaceCellLocationCache, [MaybeNullWhen(false)] out CellContext containingCell,
        [MaybeNullWhen(false)] out CellContext parentCell)
    {
        if (placedContext.TryGetParentContext(out parentCell))
        {
            containingCell = parentCell;
            if (!placedContext.TryGetParent<IWorldspaceGetter>(out var worldspaceGetter))
                // For interior cells, the containing and parent cells are the same
                return true;
            if (parentCell.Record.MajorFlags.HasFlag(Cell.MajorFlag.Persistent))
            {
                // For persistent exterior cells, the containing and parent cells are different
                // Containing cell must be found based on coordinates of the reference
                var foundContainingCell = false;
                var cellX = 0;
                var cellY = 0;
                if (placedContext.Record.Placement != null)
                {
                    var floatX = placedContext.Record.Placement.Position.X;
                    var floatY = placedContext.Record.Placement.Position.Y;
                    cellX = (int)Math.Floor(floatX / 4096);
                    cellY = (int)Math.Floor(floatY / 4096);
                }

                var worldspaceGrid = worldspaceCellLocationCache.GetGrid();
                if (worldspaceGrid.TryGetValue(worldspaceGetter.ToLink(), out var currentCellgrid))
                {
                    var point = new P2Int(cellX, cellY);
                    if (currentCellgrid.TryGetValue(point, out containingCell)) foundContainingCell = true;
                }

                return foundContainingCell;
            }

            // For non-persistent exterior cells, the containing and parent cells are the same
            return true;
        }

        containingCell = null;
        return false;
    }
}