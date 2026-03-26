using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout76;

partial class WorldspaceSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Items { get; private set; } = [];

    partial void ItemsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Items = BinaryOverlayList.FactoryByArray<ICellGetter>(
            mem: stream.RemainingMemory,
            package: _package,
            translationParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, recConv),
            locs: CellSubBlockBinaryOverlay.ParseCellRecordLocations(
                stream: stream,
                package: _package));
    }
}
