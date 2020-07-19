using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimModHeader
    {
        [Flags]
        public enum HeaderFlag
        {
            Master = 0x01
        }
    }

    namespace Internals
    {
        public partial class SkyrimModHeaderBinaryCreateTranslation
        {
            static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, ISkyrimModHeader item)
            {
                item.MasterReferences.SetTo(
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<MasterReference>.Instance.Parse(
                        frame: frame.SpawnAll(),
                        triggeringRecord: RecordTypes.MAST,
                        transl: MasterReference.TryCreateFromBinary));
                frame.MetaData.MasterReferences = new MasterReferenceReader(frame.MetaData.ModKey, item.MasterReferences);
            }
        }

        public partial class SkyrimModHeaderBinaryWriteTranslation
        {
            static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, ISkyrimModHeaderGetter item)
            {
                Mutagen.Bethesda.Binary.ListBinaryTranslation<IMasterReferenceGetter>.Instance.Write(
                    writer: writer,
                    items: item.MasterReferences,
                    transl: (MutagenWriter subWriter, IMasterReferenceGetter subItem, RecordTypeConverter? conv) =>
                    {
                        var Item = subItem;
                        ((MasterReferenceBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                            item: Item,
                            writer: subWriter,
                            recordTypeConverter: conv);
                    });
            }
        }
    }
}
