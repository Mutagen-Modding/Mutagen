using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class WorldspaceSubBlockBinaryOverlay
    {
        public IReadOnlyList<ICellGetter> Items { get; private set; } = ListExt.Empty<ICellGetter>();

        partial void ItemsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            this.Items = BinaryOverlayList.FactoryByArray<CellBinaryOverlay>(
                mem: stream.RemainingMemory,
                package: _package,
                recordTypeConverter: null,
                getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, insideWorldspace: true),
                locs: CellBinaryOverlay.ParseRecordLocations(
                    stream: stream,
                    package: _package));
        }
    }
}
