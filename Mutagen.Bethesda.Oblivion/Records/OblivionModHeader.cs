using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Core;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionModHeader
    {
        [Flags]
        public enum HeaderFlag
        {
            Master = 0x01
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int IModHeaderCommon.RawFlags
        {
            get => (int)this.Flags;
            set => this.Flags = (HeaderFlag)value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IModHeaderCommon.NumRecords
        {
            get => this.Stats.NumRecords;
            set => this.Stats.NumRecords = value;
        }
    }

    public partial interface IOblivionModHeader : IModHeaderCommon
    {
    }

    namespace Internals
    {
        public partial class OblivionModHeaderBinaryCreateTranslation
        {
            static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, IOblivionModHeader item)
            {
                item.MasterReferences.SetTo(
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<MasterReference>.Instance.Parse(
                        frame: frame.SpawnAll(),
                        triggeringRecord: RecordTypes.MAST,
                        transl: MasterReference.TryCreateFromBinary));
                frame.MetaData.MasterReferences = new MasterReferenceReader(frame.MetaData.ModKey, item.MasterReferences);
            }
        }

        public partial class OblivionModHeaderBinaryWriteTranslation
        {
            static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, IOblivionModHeaderGetter item)
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
