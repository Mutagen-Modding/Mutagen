using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.Diagnostics;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public partial class Fallout4ModHeader
{
    [Flags]
    public enum HeaderFlag
    {
        Master = 0x0000_0001,
        Localized = 0x0000_0080,
        Small = 0x0000_0200,
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

    IExtendedList<MasterReference> IModHeaderCommon.MasterReferences => this.MasterReferences;

    public void SetOverriddenForms(IEnumerable<FormKey>? formKeys)
    {
        if (formKeys == null)
        {
            this.OverriddenForms = null;
        }
        else
        {
            this.OverriddenForms ??= new();
            this.OverriddenForms.SetTo(formKeys.Select(f => f.ToLink<IFallout4MajorRecordGetter>()));
        }
    }
}

public partial interface IFallout4ModHeader : IModHeaderCommon
{
}

partial class Fallout4ModHeaderBinaryCreateTranslation
{
    public static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, IFallout4ModHeader item, PreviousParse lastParsed)
    {
        item.MasterReferences.SetTo(
            ListBinaryTranslation<MasterReference>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: RecordTypes.MAST,
                transl: MasterReference.TryCreateFromBinary));
    }
}

partial class Fallout4ModHeaderBinaryWriteTranslation
{
    public static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, IFallout4ModHeaderGetter item)
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