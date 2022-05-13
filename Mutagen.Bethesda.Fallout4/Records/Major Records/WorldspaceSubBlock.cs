using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class WorldspaceSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Items { get; private set; } = Array.Empty<ICellGetter>();

    partial void ItemsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Items = BinaryOverlayList.FactoryByArray<CellBinaryOverlay>(
            mem: stream.RemainingMemory,
            package: _package,
            parseParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, insideWorldspace: true),
            locs: CellBinaryOverlay.ParseRecordLocations(
                stream: stream,
                package: _package));
    }
}
