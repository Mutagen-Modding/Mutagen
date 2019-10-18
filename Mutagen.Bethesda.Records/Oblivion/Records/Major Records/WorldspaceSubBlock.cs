using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Binary;
using Noggog;

namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class WorldspaceSubBlockBinaryWrapper
    {
        public IReadOnlySetList<ICellGetter> Items { get; private set; } = EmptySetList<CellBinaryWrapper>.Instance;

        partial void ItemsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            this.Items = BinaryWrapperSetList<CellBinaryWrapper>.FactoryByArray(
                mem: stream.RemainingMemory,
                package: _package,
                recordTypeConverter: null,
                getter: (s, p, recConv) => CellBinaryWrapper.CellFactory(new BinaryMemoryReadStream(s), p, recConv),
                locs: CellBinaryWrapper.ParseRecordLocations(
                    stream: stream,
                    package: _package));
        }
    }
}
