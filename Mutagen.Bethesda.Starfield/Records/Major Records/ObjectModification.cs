using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Starfield;

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

    internal const string StarshipObjectModificationName = "Spaceship_InstanceData";
    internal const string NoneObjectModificationName = " NONE";

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
                    ret.ObjectModificationTargetName = null;
                    return ret;
                }
            }

            var strLen = BinaryPrimitives.ReadInt32LittleEndian(data.Content.Slice(10));
            var dataName = StringBinaryTranslation.Instance.Parse(data.Content.Slice(14, strLen), frame.MetaData.Encodings.NonTranslated, parseWhole: true);
            switch (dataName)
            {
                case Weapon.ObjectModificationName:
                    return WeaponModification.CreateFromBinary(frame);
                case Armor.ObjectModificationName:
                    return ArmorModification.CreateFromBinary(frame);
                case Npc.ObjectModificationName:
                    return NpcModification.CreateFromBinary(frame);
                case Flora.ObjectModificationName:
                    return FloraModification.CreateFromBinary(frame);
                case AObjectModification.NoneObjectModificationName:
                    return ObjectModification.CreateFromBinary(frame);
                default:
                {
                    var unknown = UnknownObjectModification.CreateFromBinary(frame);
                    unknown.ObjectModificationTargetName = dataName;
                    return unknown;
                }
            }
        }
        catch (Exception e)
        {
            throw RecordException.Enrich(
                e,
                FormKey.Factory(frame.MetaData.MasterReferences, majorMeta.FormID, reference: false),
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
        var dataName = StringBinaryTranslation.Instance.Parse(frame, StringBinaryType.PrependLengthWithNullIfContent, parseWhole: true);
        item.Unknown2 = frame.ReadUInt16();
        item.AttachPoint.SetTo(FormLinkBinaryTranslation.Instance.Parse(reader: frame));
        item.AttachParentSlots.SetTo(
            Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFormLinkGetter<IKeywordGetter>>.Instance.Parse(
                reader: frame,
                amount: frame.ReadInt32(),
                transl: FormLinkBinaryTranslation.Instance.Parse));
        item.Unknown3 = frame.ReadUInt32();
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
            case IFloraModificationInternal floraMod:
                floraMod.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<Flora.Property>.ReadProperties(frame, propertyCount));
                break;
            case IObjectModificationInternal objMod:
                objMod.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<AObjectModification.NoneProperty>.ReadProperties(frame, propertyCount));
                break;
            case IUnknownObjectModificationInternal unknown:
                unknown.Properties.SetTo(
                    ObjectTemplateBinaryCreateTranslation<AObjectModification.NoneProperty>.ReadProperties(frame, propertyCount));
                unknown.ObjectModificationTargetName = dataName;
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
        string? dataName;
        switch (item)
        {
            case IArmorModificationGetter armorMod:
                properties = armorMod.Properties;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Armor.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Armor.Property>)item);
                dataName = Armor.ObjectModificationName;
                break;
            case INpcModificationGetter npcMod:
                properties = npcMod.Properties;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Npc.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Npc.Property>)item);
                dataName = Npc.ObjectModificationName;
                break;
            case IWeaponModificationGetter weaponMod:
                properties = weaponMod.Properties;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Weapon.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Weapon.Property>)item);
                dataName = Weapon.ObjectModificationName;
                break;
            case IFloraModificationGetter floraModification:
                properties = floraModification.Properties;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<Flora.Property>(
                        writer,
                        (IAObjectModPropertyGetter<Flora.Property>)item);
                dataName = Flora.ObjectModificationName;
                break;
            case IObjectModificationGetter objMod:
                properties = objMod.Properties;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<AObjectModification.NoneProperty>(
                        writer,
                        (IAObjectModPropertyGetter<AObjectModification.NoneProperty>)item);
                dataName = AObjectModification.NoneObjectModificationName;
                break;
            case IUnknownObjectModificationGetter objMod:
                properties = objMod.Properties;
                export = (subFrame, item, _) => 
                    ObjectTemplateBinaryWriteTranslation.WriteProperty<AObjectModification.NoneProperty>(
                        writer,
                        (IAObjectModPropertyGetter<AObjectModification.NoneProperty>)item);
                dataName = objMod.ObjectModificationTargetName;
                break;
            default:
                throw new NotImplementedException();
        }
        writer.Write(includes.Count);
        writer.Write(properties.Count);
        writer.Write(item.Unknown);
        if (dataName == null)
        {
            throw new NotImplementedException();
        }
        StringBinaryTranslation.Instance.Write(writer, dataName, StringBinaryType.PrependLengthWithNullIfContent);
        writer.Write(item.Unknown2);
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
        writer.Write(item.Unknown3);
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

    private int StringLen => BinaryPrimitives.ReadInt32LittleEndian(_dataBytes.Slice(10));
    private int StringEnd => StringLen + 14;
    private int AttachSlotsStart => StringEnd + 6;
    private int AttachSlotsEnd => 4 + AttachSlotsStart + AttachParentSlots.Count * 4;
    public ushort Unknown => BinaryPrimitives.ReadUInt16LittleEndian(_dataBytes.Slice(0x8, 2));
    public ushort Unknown2 => BinaryPrimitives.ReadUInt16LittleEndian(_dataBytes.Slice(StringEnd));
    public string DataName => StringBinaryTranslation.Instance.Parse(_dataBytes.Slice(10, StringLen), 
        _package.MetaData.Encodings.NonTranslated, parseWhole: true);

    public byte MaxRank => _dataBytes[0xE];

    public byte LevelTierScaledOffset => _dataBytes[0xF];

    public IFormLinkGetter<IKeywordGetter> AttachPoint => FormLinkBinaryTranslation.Instance.OverlayFactory<IKeywordGetter>(_package, _dataBytes.Slice(StringEnd + 2, 0x4));

    public IReadOnlyList<IFormLinkGetter<IKeywordGetter>> AttachParentSlots { get; private set; } = Array.Empty<IFormLinkGetter<IKeywordGetter>>();
    public uint Unknown3 => BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes.Slice(AttachSlotsEnd));

    public IReadOnlyList<IObjectModIncludeGetter> Includes { get; private set; } = Array.Empty<IObjectModIncludeGetter>();

    public partial ParseResult DataParseCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        _dataBytes = HeaderTranslation.ExtractSubrecordMemory(_recordData, stream.Position - offset, _package.MetaData.Constants);
        var includeCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes);
        var propertyCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes.Slice(4));
        var attachSlotsStart = StringEnd + 6;
        var attachParentSlotsCount = BinaryPrimitives.ReadUInt32LittleEndian(_dataBytes.Slice(attachSlotsStart));
        AttachParentSlots = BinaryOverlayList.FactoryByCount<IFormLinkGetter<IKeywordGetter>>(
            _dataBytes.Slice(4 + attachSlotsStart, checked((int)(attachParentSlotsCount * 4))),
            _package,
            itemLength: 4,
            count: attachParentSlotsCount,
            (s, p) => FormLinkBinaryTranslation.Instance.OverlayFactory<IKeywordGetter>(p, s));
        var includesStart = 4 + AttachSlotsEnd;
        Includes = BinaryOverlayList.FactoryByCount<IObjectModIncludeGetter>(
            _dataBytes.Slice(includesStart, checked((int)(7 * includeCount))), 
            _package, 
            itemLength: 7,
            count: includeCount, 
            (s, p) => ObjectModIncludeBinaryOverlay.ObjectModIncludeFactory(s, p));
        var includesEndingPos = includesStart + Includes.Count * 7;
    
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
            case FloraModificationBinaryOverlay flora:
                flora.Properties = BinaryOverlayList.FactoryByCount(
                    _dataBytes.Slice(includesEndingPos, checked((int)(24 * propertyCount))),
                    _package,
                    itemLength: 24,
                    count: propertyCount,
                    getter: (s, p) => ObjectTemplateBinaryCreateTranslation<Flora.Property>.ReadProperty(stream.MetaData.MasterReferences, s));
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
            unknown.ObjectModificationTargetName = null;
            return unknown;
        }
        
        var strLen = BinaryPrimitives.ReadInt32LittleEndian(data.Content.Slice(10));
        var dataName = StringBinaryTranslation.Instance.Parse(data.Content.Slice(14, strLen), stream.MetaData.Encodings.NonTranslated, parseWhole: true);
        switch (dataName)
        {
            case Weapon.ObjectModificationName:
                return WeaponModificationBinaryOverlay.WeaponModificationFactory(stream, package);
            case Armor.ObjectModificationName:
                return ArmorModificationBinaryOverlay.ArmorModificationFactory(stream, package);
            case Npc.ObjectModificationName:
                return NpcModificationBinaryOverlay.NpcModificationFactory(stream, package);
            case Flora.ObjectModificationName:
                return FloraModificationBinaryOverlay.FloraModificationFactory(stream, package);
            case AObjectModification.NoneObjectModificationName:
                return ObjectModificationBinaryOverlay.ObjectModificationFactory(stream, package);
            default:
            {
                var unknown = (UnknownObjectModificationBinaryOverlay)UnknownObjectModificationBinaryOverlay.UnknownObjectModificationFactory(stream, package);
                unknown.ObjectModificationTargetName = dataName;
                return unknown;
            }
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

partial class FloraModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<Flora.Property>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<Flora.Property>>();
}

partial class ContainerModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<Container.Property>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<Container.Property>>();
}

partial class ObjectModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<AObjectModification.NoneProperty>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<AObjectModification.NoneProperty>>();
}

partial class UnknownObjectModificationBinaryOverlay
{
    public IReadOnlyList<IAObjectModPropertyGetter<AObjectModification.NoneProperty>> Properties { get; internal set; } = Array.Empty<IAObjectModPropertyGetter<AObjectModification.NoneProperty>>();
    
    public string? ObjectModificationTargetName { get; set; }
}