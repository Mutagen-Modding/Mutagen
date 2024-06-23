using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4;

internal class DeletedObjectModification : AObjectModification
{
    protected override Type LinkType => typeof(IAObjectModification);
}

partial class AObjectModification
{
    [Flags]
    public enum MajorFlag
    {
        LegendaryMod = 0x08,
        ModCollection = 0x40,
    }

    public enum NoneProperty
    {
    }

    public static AObjectModification CreateFromBinary(
        MutagenFrame frame,
        TypedParseParams translationParams)
    {
        var majorMeta = frame.GetMajorRecord();
        try
        {
            if (!majorMeta.TryFindSubrecord(RecordTypes.DATA, out var data))
            {
                if (majorMeta.IsDeleted)
                {
                    var ret = new DeletedObjectModification();
                    ret.CopyInFromBinary(frame);
                    return ret;
                }
                else
                {
                    var ret = UnknownObjectModification.CreateFromBinary(frame);
                    ret.ModificationType = RecordType.Null;
                    return ret;
                }
            }
            var type = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(data.Content.Slice(10)));
            switch (type.TypeInt)
            {
                case RecordTypeInts.ARMO:
                    return ArmorModification.CreateFromBinary(frame);
                case RecordTypeInts.NPC_:
                    return NpcModification.CreateFromBinary(frame);
                case RecordTypeInts.WEAP:
                    return WeaponModification.CreateFromBinary(frame);
                case RecordTypeInts.NONE:
                    return ObjectModification.CreateFromBinary(frame);
                default:
                    var unknown = UnknownObjectModification.CreateFromBinary(frame);
                    unknown.ModificationType = type;
                    return unknown;
            }
        }
        catch (Exception e)
        {
            throw RecordException.Enrich(
                e,
                FormKey.Factory(frame.MetaData.MasterReferences.Raw, majorMeta.FormID.ID),
                typeof(AObjectModification));
        }
    }
}

partial class AObjectModificationBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryDataParseCustom(
        MutagenFrame frame,
        IAObjectModificationInternal item,
        PreviousParse lastParsed)
    {
        frame.ReadSubrecordHeader(RecordTypes.DATA);
        var includeCount = frame.ReadInt32();
        var propertyCount = frame.ReadInt32();
        item.Unknown = frame.ReadUInt16();
        var formType = new RecordType(frame.ReadInt32());
        item.MaxRank = frame.ReadUInt8();
        item.LevelTierScaledOffset = frame.ReadUInt8();
        item.AttachPoint.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader: frame));
        item.AttachParentSlots.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFormLinkGetter<IKeywordGetter>>.Instance.Parse(
                reader: frame,
                amount: frame.ReadInt32(),
                transl: FormLinkBinaryTranslation.Instance.Parse));
        item.Items.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<ObjectModItem>.Instance.Parse(
                reader: frame,
                amount: frame.ReadInt32(),
                transl: ObjectModItem.TryCreateFromBinary));
        item.Includes.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<ObjectModInclude>.Instance.Parse(
                reader: frame,
                amount: includeCount,
                transl: ObjectModInclude.TryCreateFromBinary));
        switch (item)
        {
            case IArmorModificationInternal armorMod:
                armorMod.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<Armor.Property>.ReadProperties(frame, propertyCount));
                break;
            case INpcModificationInternal npcMod:
                npcMod.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<Npc.Property>.ReadProperties(frame, propertyCount));
                break;
            case IWeaponModificationInternal weaponMod:
                weaponMod.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<Weapon.Property>.ReadProperties(frame, propertyCount));
                break;
            case IObjectModificationInternal objMod:
                objMod.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<AObjectModification.NoneProperty>.ReadProperties(frame, propertyCount));
                break;
            case IUnknownObjectModificationInternal unknown:
                unknown.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<AObjectModification.NoneProperty>.ReadProperties(frame, propertyCount));
                unknown.ModificationType = formType;
                break;
            case DeletedObjectModification del:
                if (!del.IsDeleted)
                {
                    throw new MalformedDataException(
                        "Do not mark a DeletedObjectModification as no longer deleted.  Instead, make a new" +
                        "ObjectModification record");
                }
                break;
            default:
                throw new NotImplementedException();
        }
        return (int)AObjectModification_FieldIndex.Model;
    }
}

partial class AObjectModificationBinaryWriteTranslation
{
    public static partial void WriteBinaryDataParseCustom(
        MutagenWriter writer,
        IAObjectModificationGetter item)
    {
        using var header = HeaderExport.Subrecord(writer, RecordTypes.DATA);
        var includes = item.Includes;
        PluginUtilityTranslation.BinaryMasterWriteDelegate<IBinaryItem> export;
        IReadOnlyList<IBinaryItem> properties;
        RecordType type;
        switch (item)
        {
            case IArmorModificationGetter armorMod:
                properties = armorMod.Properties;
                type = RecordTypes.ARMO;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Armor.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Armor.Property>)item);
                break;
            case INpcModificationGetter npcMod:
                properties = npcMod.Properties;
                type = RecordTypes.NPC_;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Npc.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Npc.Property>)item);
                break;
            case IWeaponModificationGetter weaponMod:
                properties = weaponMod.Properties;
                type = RecordTypes.WEAP;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Weapon.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Weapon.Property>)item);
                break;
            case IObjectModificationGetter objMod:
                properties = objMod.Properties;
                type = RecordTypes.NONE;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<AObjectModification.NoneProperty>(
                        writer,
                        (IAObjectModPropertyGetter<AObjectModification.NoneProperty>)item);
                break;
            case IUnknownObjectModificationGetter objMod:
                properties = objMod.Properties;
                type = objMod.ModificationType;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<AObjectModification.NoneProperty>(
                        writer,
                        (IAObjectModPropertyGetter<AObjectModification.NoneProperty>)item);
                break;
            default:
                throw new NotImplementedException();
        }
        writer.Write(includes.Count);
        writer.Write(properties.Count);
        writer.Write(item.Unknown);
        writer.Write(type.TypeInt);
        writer.Write(item.MaxRank);
        writer.Write(item.LevelTierScaledOffset);
        FormLinkBinaryTranslation.Instance.Write(
            writer: writer,
            item: item.AttachPoint);
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFormLinkGetter<IKeywordGetter>>.Instance.Write(
            writer: writer,
            items: item.AttachParentSlots,
            countLengthLength: 4,
            transl: (MutagenWriter subWriter, IFormLinkGetter<IKeywordGetter> subItem, TypedWriteParams conv) =>
            {
                FormLinkBinaryTranslation.Instance.Write(
                    writer: subWriter,
                    item: subItem);
            });
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IObjectModItemGetter>.Instance.Write(
            writer: writer,
            items: item.Items,
            countLengthLength: 4,
            transl: (MutagenWriter subWriter, IObjectModItemGetter subItem, TypedWriteParams conv) =>
            {
                var Item = subItem;
                ((ObjectModItemBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                    item: Item,
                    writer: subWriter,
                    translationParams: conv);
            });
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IObjectModIncludeGetter>.Instance.Write(
            writer: writer,
            items: includes,
            transl: (MutagenWriter subWriter, IObjectModIncludeGetter subItem, TypedWriteParams conv) =>
            {
                subItem.WriteToBinary(subWriter);
            });
        Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IBinaryItem>.Instance.Write(
            writer: writer,
            items: properties,
            transl: export);
    }
}

partial class AObjectModificationBinaryOverlay
{
    private ReadOnlyMemorySlice<byte> _dataBytes;
    public ushort Unknown => BinaryPrimitives.ReadUInt16LittleEndian(_dataBytes.Slice(0x8, 2));

    public byte MaxRank => _dataBytes[0xE];

    public byte LevelTierScaledOffset => _dataBytes[0xF];

    public IFormLinkGetter<IKeywordGetter> AttachPoint => FormLinkBinaryTranslation.Instance.OverlayFactory<IKeywordGetter>(_package, _dataBytes.Slice(0x10, 0x4));

    public IReadOnlyList<IFormLinkGetter<IKeywordGetter>> AttachParentSlots { get; private set; } = Array.Empty<IFormLinkGetter<IKeywordGetter>>();

    public IReadOnlyList<IObjectModItemGetter> Items { get; private set; } = Array.Empty<IObjectModItemGetter>();

    public IReadOnlyList<IObjectModIncludeGetter> Includes { get; private set; } = Array.Empty<IObjectModIncludeGetter>();

    public partial ParseResult DataParseCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        _dataBytes = HeaderTranslation.ExtractSubrecordMemory(_recordData, stream.Position - offset, _package.MetaData.Constants);
        var includeCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes);
        var propertyCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes.Slice(4));
        var attachParentSlotsCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes.Slice(0x14));
        AttachParentSlots = BinaryOverlayList.FactoryByCount<IFormLinkGetter<IKeywordGetter>>(
            _dataBytes.Slice(0x18, checked((int)(attachParentSlotsCount * 4))),
            _package,
            itemLength: 4,
            count: attachParentSlotsCount,
            (s, p) => FormLinkBinaryTranslation.Instance.OverlayFactory<IKeywordGetter>(p, s));
        var attachParentSlotsEndingPos = 0x18 + AttachParentSlots.Count * 4;
        var itemsCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes.Slice(attachParentSlotsEndingPos));
        Items = BinaryOverlayList.FactoryByCount<IObjectModItemGetter>(
            _dataBytes.Slice(attachParentSlotsEndingPos + 4, checked((int)(itemsCount * 8))),
            _package,
            itemLength: 8,
            count: itemsCount,
            (s, p) => ObjectModItemBinaryOverlay.ObjectModItemFactory(s, p));
        var itemsEndingPos = attachParentSlotsEndingPos + 4 + Items.Count * 8;
        Includes = BinaryOverlayList.FactoryByCount<IObjectModIncludeGetter>(
            _dataBytes.Slice(itemsEndingPos, checked((int)(7 * includeCount))), 
            _package, 
            itemLength: 7,
            count: includeCount, 
            (s, p) => ObjectModIncludeBinaryOverlay.ObjectModIncludeFactory(s, p));
        var includesEndingPos = itemsEndingPos + Includes.Count * 7;

        switch (this)
        {
            case ArmorModificationBinaryOverlay armo:
                armo.Properties = BinaryOverlayList.FactoryByCount(
                    _dataBytes.Slice(includesEndingPos, checked((int)(24 * propertyCount))),
                    _package,
                    itemLength: 24,
                    count: propertyCount,
                    getter: (s, p) => ObjectTemplateBinaryCreateTranslation<Armor.Property>.ReadProperty(stream.MetaData.MasterReferences, s));
                break;
            case NpcModificationBinaryOverlay npc:
                npc.Properties = BinaryOverlayList.FactoryByCount(
                    _dataBytes.Slice(includesEndingPos, checked((int)(24 * propertyCount))),
                    _package,
                    itemLength: 24,
                    count: propertyCount,
                    getter: (s, p) => ObjectTemplateBinaryCreateTranslation<Npc.Property>.ReadProperty(stream.MetaData.MasterReferences, s));
                break;
            case WeaponModificationBinaryOverlay weap:
                weap.Properties = BinaryOverlayList.FactoryByCount(
                    _dataBytes.Slice(includesEndingPos, checked((int)(24 * propertyCount))),
                    _package,
                    itemLength: 24,
                    count: propertyCount,
                    getter: (s, p) => ObjectTemplateBinaryCreateTranslation<Weapon.Property>.ReadProperty(stream.MetaData.MasterReferences, s));
                break;
            case ObjectModificationBinaryOverlay obj:
                obj.Properties = BinaryOverlayList.FactoryByCount(
                    _dataBytes.Slice(includesEndingPos, checked((int)(24 * propertyCount))),
                    _package,
                    itemLength: 24,
                    count: propertyCount,
                    getter: (s, p) => ObjectTemplateBinaryCreateTranslation<AObjectModification.NoneProperty>.ReadProperty(stream.MetaData.MasterReferences, s));
                break;
            case UnknownObjectModificationBinaryOverlay obj:
                obj.Properties = BinaryOverlayList.FactoryByCount(
                    _dataBytes.Slice(includesEndingPos, checked((int)(24 * propertyCount))),
                    _package,
                    itemLength: 24,
                    count: propertyCount,
                    getter: (s, p) => ObjectTemplateBinaryCreateTranslation<AObjectModification.NoneProperty>.ReadProperty(stream.MetaData.MasterReferences, s));
                break;
            default:
                throw new NotImplementedException();
        }

        return (int)AObjectModification_FieldIndex.Includes;
    }

    public static IAObjectModificationGetter AObjectModificationFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams)
    {
        var majorRec = stream.GetMajorRecord();
        if (!majorRec.TryFindSubrecord(RecordTypes.DATA, out var data))
        {
            var unknown = (UnknownObjectModificationBinaryOverlay)UnknownObjectModificationBinaryOverlay.UnknownObjectModificationFactory(stream, package);
            unknown.ModificationType = RecordType.Null;
            return unknown;
        }
        var type = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(data.Content.Slice(10)));
        switch (type.TypeInt)
        {
            case RecordTypeInts.ARMO:
                return ArmorModificationBinaryOverlay.ArmorModificationFactory(stream, package);
            case RecordTypeInts.NPC_:
                return NpcModificationBinaryOverlay.NpcModificationFactory(stream, package);
            case RecordTypeInts.WEAP:
                return WeaponModificationBinaryOverlay.WeaponModificationFactory(stream, package);
            case RecordTypeInts.NONE:
                return ObjectModificationBinaryOverlay.ObjectModificationFactory(stream, package);
            default:
                var unknown = (UnknownObjectModificationBinaryOverlay)UnknownObjectModificationBinaryOverlay.UnknownObjectModificationFactory(stream, package);
                unknown.ModificationType = type;
                return unknown;
        }
    }
}

partial class ArmorModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<Armor.Property>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<Armor.Property>>();
}

partial class WeaponModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<Weapon.Property>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<Weapon.Property>>();
}

partial class NpcModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<Npc.Property>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<Npc.Property>>();
}

partial class ObjectModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<AObjectModification.NoneProperty>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<AObjectModification.NoneProperty>>();
}

partial class UnknownObjectModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<AObjectModification.NoneProperty>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<AObjectModification.NoneProperty>>();
    
    public RecordType ModificationType { get; set; }
}