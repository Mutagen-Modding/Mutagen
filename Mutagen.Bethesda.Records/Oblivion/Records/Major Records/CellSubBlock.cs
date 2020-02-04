using Loqui.Internal;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CellSubBlock
    {
        public static CellSubBlock_TranslationMask duplicateMask = new CellSubBlock_TranslationMask(true)
        {
            Cells = new Loqui.MaskItem<bool, Cell_TranslationMask?>(false, default)
        };

        public void WriteToXmlFolder(
            string path,
            ErrorMaskBuilder? errorMask)
        {
            this.WriteToXml(
                path, 
                errorMask,
                translationMask: null);
        }

        public static CellSubBlock CreateFromXmlFolder(FilePath file, int index)
        {
            return CellSubBlock.CreateFromXml(file.Path);
        }

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecordTracker = null)
        {
            var ret = new CellSubBlock();
            ret.DeepCopyIn(this, duplicateMask);
            ret.Cells.SetTo(this.Cells.Select(i => (Cell)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }
    }

    namespace Internals
    {
        partial class CellSubBlockBinaryOverlay
        {
            public IReadOnlySetList<ICellGetter> Cells { get; private set; } = EmptySetList<CellBinaryOverlay>.Instance;

            partial void CellsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                this.Cells = BinaryOverlaySetList<CellBinaryOverlay>.FactoryByArray(
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
