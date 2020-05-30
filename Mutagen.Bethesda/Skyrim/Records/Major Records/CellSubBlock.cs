using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class CellSubBlock
    {
        public static CellSubBlock.TranslationMask duplicateMask = new CellSubBlock.TranslationMask(true)
        {
            Cells = new Loqui.MaskItem<bool, Cell.TranslationMask?>(false, default)
        };

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecordTracker = null)
        {
            var ret = new CellSubBlock();
            ret.DeepCopyIn(this, duplicateMask);
            ret.Cells = this.Cells.Select(i => (Cell)i.Duplicate(getNextFormKey, duplicatedRecordTracker)).ToExtendedList();
            return ret;
        }
    }

    namespace Internals
    {
        partial class CellSubBlockBinaryOverlay
        {
            public IReadOnlyList<ICellGetter> Cells { get; private set; } = ListExt.Empty<ICellGetter>();

            partial void CellsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                this.Cells = BinaryOverlayList<CellBinaryOverlay>.FactoryByArray(
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
}
