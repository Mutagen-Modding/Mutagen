using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

partial class CellSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Cells { get; private set; } = Array.Empty<ICellGetter>();

    partial void CellsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Cells = BinaryOverlayList.FactoryByArray<ICellGetter>(
            mem: stream.RemainingMemory,
            package: _package,
            translationParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, recConv),
            locs: CellBinaryOverlay.ParseRecordLocations(
                stream: stream,
                package: _package));
    }
}