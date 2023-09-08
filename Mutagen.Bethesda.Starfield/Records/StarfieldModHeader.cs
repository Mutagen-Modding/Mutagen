using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.Diagnostics;

namespace Mutagen.Bethesda.Starfield;

public partial class StarfieldModHeader
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
    uint IModHeaderCommon.MinimumCustomFormID => StarfieldMod.DefaultInitialNextFormID;

    IExtendedList<MasterReference> IModHeaderCommon.MasterReferences => this.MasterReferences;
}

public partial interface IStarfieldModHeader : IModHeaderCommon
{
}

partial class StarfieldModHeaderBinaryCreateTranslation
{
    public static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, IStarfieldModHeader item, PreviousParse lastParsed)
    {
        item.MasterReferences.SetTo(
            ListBinaryTranslation<MasterReference>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: RecordTypes.MAST,
                transl: MasterReference.TryCreateFromBinary));
        frame.MetaData.MasterReferences.SetTo(item.MasterReferences);
    }
}

partial class StarfieldModHeaderBinaryWriteTranslation
{
    public static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, IStarfieldModHeaderGetter item)
    {
        ListBinaryTranslation<IMasterReferenceGetter>.Instance.Write(
            writer: writer,
            items: item.MasterReferences,
            transl: (MutagenWriter subWriter, IMasterReferenceGetter subItem, TypedWriteParams conv) =>
            {
                var Item = subItem;
                ((MasterReferenceBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
    }
}