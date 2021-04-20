using Mutagen.Bethesda.Core;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Noggog;
using System;
using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Fallout4ModHeader
    {
        [Flags]
        public enum HeaderFlag
        {
            Master = 0x0000_0001,
            Localized = 0x0000_0080,
            LightMaster = 0x0000_0200,
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IModHeaderCommon.NextFormID
        {
            get => this.Stats.NextFormID;
            set => this.Stats.NextFormID = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IModHeaderCommon.MinimumCustomFormID => Fallout4Mod.DefaultInitialNextFormID;

        IExtendedList<MasterReference> IModHeaderCommon.MasterReferences => this.MasterReferences;
    }

    public partial interface IFallout4ModHeader : IModHeaderCommon
    {
    }

    namespace Internals
    {
        public partial class Fallout4ModHeaderBinaryCreateTranslation
        {
            static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, IFallout4ModHeader item)
            {
                item.MasterReferences.SetTo(
                    ListBinaryTranslation<MasterReference>.Instance.Parse(
                        frame: frame.SpawnAll(),
                        triggeringRecord: RecordTypes.MAST,
                        transl: MasterReference.TryCreateFromBinary));
                frame.MetaData.MasterReferences.SetTo(item.MasterReferences);
            }
        }

        public partial class Fallout4ModHeaderBinaryWriteTranslation
        {
            static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, IFallout4ModHeaderGetter item)
            {
                ListBinaryTranslation<IMasterReferenceGetter>.Instance.Write(
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
