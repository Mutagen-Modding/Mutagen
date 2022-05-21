using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class CellSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Cells { get; private set; } = Array.Empty<ICellGetter>();

    partial void CellsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Cells = BinaryOverlayList.FactoryByArray<CellBinaryOverlay>(
            mem: stream.RemainingMemory,
            package: _package,
            parseParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, recConv),
            locs: CellBinaryOverlay.ParseRecordLocations(
                stream: stream,
                package: _package));
    }
}