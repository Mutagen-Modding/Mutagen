using Mutagen.Bethesda.Plugins;
using Noggog;
using CellContext =
    Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod,
        Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Skyrim.ICell, Mutagen.Bethesda.Skyrim.ICellGetter>;

namespace Mutagen.Bethesda.Skyrim.Records.Tooling;

public class WorldspaceCellLocationCache
{
    private readonly Lazy<Dictionary<IFormLink<IWorldspaceGetter>, Dictionary<P2Int, CellContext>>> lazyGrid;

    /// <summary>
    ///     Creates a cell location cache for all worldspaces using the given cell contexts.
    /// </summary>
    /// <param name="allCellContexts">Collection of cell contexts that will be used to build the cell location cache.</param>
    public WorldspaceCellLocationCache(IEnumerable<CellContext> allCellContexts)
    {
        lazyGrid = new Lazy<Dictionary<IFormLink<IWorldspaceGetter>, Dictionary<P2Int, CellContext>>>(() =>
        {
            var currentCellGrid = new Dictionary<IFormLink<IWorldspaceGetter>, Dictionary<P2Int, CellContext>>();
            foreach (var cellContext in allCellContexts)
            {
                var cell = cellContext.Record;
                if (cell.Grid == null || cell.MajorFlags.HasFlag(Cell.MajorFlag.Persistent)) continue;
                if (cellContext.TryGetParent<IWorldspaceGetter>(out var worldspace))
                {
                    if (!currentCellGrid.ContainsKey(worldspace.ToLink()))
                        currentCellGrid.Add(worldspace.ToLink(), new Dictionary<P2Int, CellContext>());
                    var currentCellgrid = currentCellGrid[worldspace.ToLink()];
                    currentCellgrid.Add(cell.Grid.Point, cellContext);
                }
            }

            return currentCellGrid;
        });
    }

    internal Dictionary<IFormLink<IWorldspaceGetter>, Dictionary<P2Int, CellContext>> GetGrid()
    {
        return lazyGrid.Value;
    }
}