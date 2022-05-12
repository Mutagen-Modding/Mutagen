using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.Diagnostics;

namespace Mutagen.Bethesda.Oblivion;

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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    uint IModHeaderCommon.NextFormID
    {
        get => this.Stats.NextFormID;
        set => this.Stats.NextFormID = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    uint IModHeaderCommon.MinimumCustomFormID => OblivionMod.DefaultInitialNextFormID;

    IExtendedList<MasterReference> IModHeaderCommon.MasterReferences => this.MasterReferences;
}

public partial interface IOblivionModHeader : IModHeaderCommon
{
}

partial class OblivionModHeaderBinaryCreateTranslation
{
    public static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, IOblivionModHeader item)
    {
        item.MasterReferences.SetTo(
            ListBinaryTranslation<MasterReference>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: RecordTypes.MAST,
                transl: MasterReference.TryCreateFromBinary));
        frame.MetaData.MasterReferences.SetTo(item.MasterReferences);
    }
}

partial class OblivionModHeaderBinaryWriteTranslation
{
    public static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, IOblivionModHeaderGetter item)
    {
        ListBinaryTranslation<IMasterReferenceGetter>.Instance.Write(
            writer: writer,
            items: item.MasterReferences,
            transl: (MutagenWriter subWriter, IMasterReferenceGetter subItem, TypedWriteParams? conv) =>
            {
                var Item = subItem;
                ((MasterReferenceBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}