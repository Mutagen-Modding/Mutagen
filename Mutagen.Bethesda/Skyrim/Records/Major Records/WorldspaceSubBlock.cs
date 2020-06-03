using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class WorldspaceSubBlockBinaryOverlay
    {
        public IReadOnlyList<ICellGetter> Items { get; private set; } = ListExt.Empty<ICellGetter>();

        partial void ItemsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            this.Items = BinaryOverlayList<CellBinaryOverlay>.FactoryByArray(
                mem: stream.RemainingMemory,
                package: _package,
                recordTypeConverter: null,
                getter: (s, p, recConv) => CellBinaryOverlay.CellFactory(new BinaryMemoryReadStream(s), p, recConv),
                locs: CellBinaryOverlay.ParseRecordLocations(
                    stream: stream,
                    package: _package));
        }
    }
}
