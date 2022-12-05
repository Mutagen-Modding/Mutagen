using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim.Records.Tooling;
using System.Diagnostics.CodeAnalysis;

using PlacedContext = Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Skyrim.IPlaced, Mutagen.Bethesda.Skyrim.IPlacedGetter>;

namespace Mutagen.Bethesda.Skyrim.Extensions {
    public static class PlacedContextExt {
        //
        // Summary:
        //     Attempts to locate placedContext's containing cell.
        //
        // Parameters:
        //   placedContext:
        //     Context of the placed reference.
        //
        //   worldspaceCellLocationCache:
        //     WorldspaceCellLocationCache to use for cell lookup
        //
        //   containingCell:
        //     Context of the containing cell if located
        //
        // Returns:
        //     True if containing cell was found
        public static bool TryGetContainingCell(this PlacedContext placedContext,
                                                WorldspaceCellLocationCache worldspaceCellLocationCache,
                                                [MaybeNullWhen(false)] out IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> containingCell) {
            return GetContainingCell.TryGetContainingCell(placedContext, worldspaceCellLocationCache, out containingCell, out _);
        }

        //
        // Summary:
        //     Attempts to locate placedContext's containing cell.
        //
        // Parameters:
        //   placedContext:
        //     Context of the placed reference.
        //
        //   worldspaceCellLocationCache:
        //     WorldspaceCellLocationCache to use for cell lookup
        //
        //   containingCell:
        //     Context of the containing cell if located
        //
        //   parentCell:
        //     Context of the parent cell if located
        //
        // Returns:
        //     True if containing cell was found
        public static bool TryGetContainingCell(PlacedContext placedContext,
                                                WorldspaceCellLocationCache worldspaceCellLocationCache,
                                                [MaybeNullWhen(false)] out IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> containingCell,
                                                [MaybeNullWhen(false)] out IModContext<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter> parentCell) {
            return GetContainingCell.TryGetContainingCell(placedContext, worldspaceCellLocationCache, out containingCell, out parentCell);
        }
    }
}
