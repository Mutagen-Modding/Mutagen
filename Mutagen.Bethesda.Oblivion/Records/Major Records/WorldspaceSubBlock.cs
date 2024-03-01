using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Oblivion;

internal partial class WorldspaceSubBlockBinaryOverlay
{
    public IReadOnlyList<ICellGetter> Items { get; private set; } = Array.Empty<ICellGetter>();

    partial void ItemsCustomParse(OverlayStream stream, int finalPos, int offset, RecordType type, PreviousParse lastParsed)
    {
        this.Items = BinaryOverlayList.FactoryByArray<ICellGetter>(
            mem: stream.RemainingMemory,
            package: _package,
            translationParams: null,
            getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, recConv),
            locs: CellBinaryOverlay.ParseRecordLocations(
                stream: stream,
                package: _package));
    }
}