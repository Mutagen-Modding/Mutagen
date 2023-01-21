using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim.Internals;
using PlacedContext =
    Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod,
        Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Skyrim.IPlaced,
        Mutagen.Bethesda.Skyrim.IPlacedGetter>;

namespace Mutagen.Bethesda.Skyrim;

public static class PlacedContextExt
{
    /// <summary>
    /// Attempts to locate placedContext's containing cell.
    /// </summary>
    /// <param name="placedContext">Context of the placed reference</param>
    /// <param name="worldspaceCellLocationCache">WorldspaceCellLocationCache to use for cell lookup</param>
    /// <param name="containingCell">Context of the containing cell if located</param>
    /// <returns>True if containing cell was found</returns>
    public static bool TryGetContainingCell(this PlacedContext placedContext,
        WorldspaceCellLocationCache worldspaceCellLocationCache,
        [MaybeNullWhen(false)] out IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> containingCell)
    {
        return GetContainingCell.TryGetContainingCell(placedContext, worldspaceCellLocationCache, out containingCell,
            out _);
    }

    /// <summary>
    /// Attempts to locate placedContext's containing cell.
    /// </summary>
    /// <param name="placedContext">Context of the placed reference</param>
    /// <param name="worldspaceCellLocationCache">WorldspaceCellLocationCache to use for cell lookup</param>
    /// <param name="containingCell">Context of the containing cell if located</param>
    /// <param name="parentCell">Context of the parent cell if located</param>
    /// <returns>True if containing cell was found</returns>
    public static bool TryGetContainingCell(PlacedContext placedContext,
        WorldspaceCellLocationCache worldspaceCellLocationCache,
        [MaybeNullWhen(false)] out IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> containingCell,
        [MaybeNullWhen(false)] out IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> parentCell)
    {
        return GetContainingCell.TryGetContainingCell(placedContext, worldspaceCellLocationCache, out containingCell,
            out parentCell);
    }
}