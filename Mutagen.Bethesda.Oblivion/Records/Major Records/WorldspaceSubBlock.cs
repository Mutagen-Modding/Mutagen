using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Oblivion.Internals
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
                getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new OverlayStream(s, p), p, recConv),
                locs: CellBinaryOverlay.ParseRecordLocations(
                    stream: stream,
                    package: _package));
        }
    }
}
