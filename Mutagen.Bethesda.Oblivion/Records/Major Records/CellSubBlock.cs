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
        public static CellSubBlock_CopyMask duplicateMask = new CellSubBlock_CopyMask(true)
        {
            Items = new Loqui.MaskItem<Loqui.CopyOption, Cell_CopyMask>(Loqui.CopyOption.Skip, null)
        };

        public void WriteToXmlFolder(
            string path,
            ErrorMaskBuilder errorMask)
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

        public object Duplicate(Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecordTracker = null)
        {
            var ret = new CellSubBlock();
            ret.CopyFieldsFrom(this, duplicateMask);
            ret.Items.SetTo(this.Items.Select(i => (Cell)i.Duplicate(getNextFormKey, duplicatedRecordTracker)));
            return ret;
        }
    }

    namespace Internals
    {
        partial class CellSubBlockBinaryWrapper
        {
            public IReadOnlySetList<ICellInternalGetter> Items { get; private set; } = EmptySetList<CellBinaryWrapper>.Instance;

            partial void ItemsCustomParse(BinaryMemoryReadStream stream, int offset, RecordType type, int? lastParsed)
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
}
