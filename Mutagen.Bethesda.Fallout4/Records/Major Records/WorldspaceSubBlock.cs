using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class WorldspaceSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Items { get; private set; } = Array.Empty<ICellGetter>();

    partial void ItemsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Items = BinaryOverlayList.FactoryByArray<CellBinaryOverlay>(
            mem: stream.RemainingMemory,
            package: _package,
            translationParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, insideWorldspace: true),
            locs: CellBinaryOverlay.ParseRecordLocations(
                stream: stream,
                package: _package));
    }
}
