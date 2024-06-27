using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.Diagnostics;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class SkyrimModHeader
{
    [Flags]
    public enum HeaderFlag
    {
        Master = 0x0000_0001,
        Localized = 0x0000_0080,
        Light = 0x0000_0200,
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
            this.OverriddenForms.SetTo(formKeys.Select(f => f.ToLink<ISkyrimMajorRecordGetter>()));
        }
    }
}

public partial interface ISkyrimModHeader : IModHeaderCommon
{
}

partial class SkyrimModHeaderBinaryCreateTranslation
{
    public static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, ISkyrimModHeader item, PreviousParse lastParsed)
    {
        item.MasterReferences.SetTo(
            ListBinaryTranslation<MasterReference>.Instance.Parse(
                reader: frame.SpawnAll(),
                triggeringRecord: RecordTypes.MAST,
                transl: MasterReference.TryCreateFromBinary));
    }
}

partial class SkyrimModHeaderBinaryWriteTranslation
{
    public static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, ISkyrimModHeaderGetter item)
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