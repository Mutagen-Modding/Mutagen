using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class ArmorObjectTemplate
    {
        public enum Property
        {
            Enchantments,
            BashImpactDataSet,
            BlockMaterial,
            Keywords,
            Weight,
            Value,
            Rating,
            AddonIndex,
            BodyPart,
            DamageTypeValue,
            ActorValues,
            Health,
            ColorRemappingIndex,
            MaterialSwaps
        }
    }
    
    namespace Internals
    {
        partial class ArmorObjectTemplateBinaryCreateTranslation
        {
            public static partial ParseResult FillBinaryOBTSLogicCustom(MutagenFrame frame, IArmorObjectTemplate item,
                PreviousParse lastParsed)
            {
                frame.ReadSubrecord(RecordTypes.OBTS);
                var includeCount = frame.ReadUInt32();
                var propertyCount = frame.ReadUInt32();
                item.LevelMin = frame.ReadUInt8();
                frame.Position += 1;
                item.LevelMax = frame.ReadUInt8();
                frame.Position += 1;
                item.AddonIndex = frame.ReadInt16();
                item.Default = BooleanBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(frame, 1);
                item.Keywords.SetTo(
                    Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFormLinkGetter<IKeywordGetter>>
                        .Instance.Parse(
                            amount: frame.ReadUInt8(),
                            reader: frame,
                            transl: FormLinkBinaryTranslation.Instance.Parse));
                item.MinLevelForRanks = frame.ReadUInt8();
                item.AltLevelsPerTier = frame.ReadUInt8();
                item.Includes.SetTo(
                    Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<ObjectTemplateInclude>.Instance
                        .Parse(
                            reader: frame,
                            amount: checked((int)includeCount),
                            transl: ObjectTemplateInclude.TryCreateFromBinary));
                item.Properties.SetTo(ReadProperties(frame, checked((int)propertyCount)));
                return (int)ArmorObjectTemplate_FieldIndex.Properties;
            }

            public static IEnumerable<AArmorObjectModProperty> ReadProperties(MutagenFrame stream, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return ReadProperty(stream.MetaData.MasterReferences, stream.ReadSpan(24));
                }
            }

            public static AArmorObjectModProperty ReadProperty(IMasterReferenceReader masters, ReadOnlySpan<byte> data)
            {
                AArmorObjectModProperty ret;
                var type = (ObjectModProperty.ValueType)data[0];
                var enumVal = data[4];
                var armorProp = (ArmorObjectTemplate.Property)BinaryPrimitives.ReadUInt16LittleEndian(data[8..]);
                switch (type)
                {
                    case ObjectModProperty.ValueType.Int:
                        ret = new ArmorObjectModIntProperty()
                        {
                            FunctionType = (ObjectModProperty.FloatFunctionType)enumVal,
                            Value = BinaryPrimitives.ReadUInt32LittleEndian(data[12..]),
                            Value2 = BinaryPrimitives.ReadUInt32LittleEndian(data[16..]),
                        };
                        break;
                    case ObjectModProperty.ValueType.Float:
                        ret = new ArmorObjectModFloatProperty()
                        {
                            FunctionType = (ObjectModProperty.FloatFunctionType)enumVal,
                            Value = BinaryPrimitives.ReadSingleLittleEndian(data[12..]),
                            Value2 = BinaryPrimitives.ReadSingleLittleEndian(data[16..]),
                        };
                        break;
                    case ObjectModProperty.ValueType.Bool:
                        ret = new ArmorObjectModBoolProperty()
                        {
                            FunctionType = (ObjectModProperty.BoolFunctionType)enumVal,
                            Value = data[12] > 0,
                            Value2 = data[16] > 0,
                        };
                        break;
                    case ObjectModProperty.ValueType.String:
                        ret = new ArmorObjectModStringProperty()
                        {
                            FunctionType = (ObjectModProperty.FloatFunctionType)enumVal,
                            Value = data.Slice(12, 4).ToString(),
                            Unused = BinaryPrimitives.ReadUInt32LittleEndian(data[16..]),
                        };
                        break;
                    case ObjectModProperty.ValueType.FormIdInt:
                    {
                        var prop = new ArmorObjectModFormLinkIntProperty()
                        {
                            FunctionType = (ObjectModProperty.FormLinkFunctionType)enumVal,
                        };
                        prop.Record.SetTo(FormKeyBinaryTranslation.Instance.Parse(data[12..], masters));
                        prop.Value = BinaryPrimitives.ReadUInt32LittleEndian(data[16..]);
                        ret = prop;
                        break;
                    }
                    case ObjectModProperty.ValueType.FormIdFloat:
                    {
                        var prop = new ArmorObjectModFormLinkFloatProperty()
                        {
                            FunctionType = (ObjectModProperty.FloatFunctionType)enumVal,
                        };
                        prop.Record.SetTo(FormKeyBinaryTranslation.Instance.Parse(data[12..], masters));
                        prop.Value = BinaryPrimitives.ReadSingleLittleEndian(data[16..]);
                        ret = prop;
                        break;
                    }
                    default:
                        throw new NotImplementedException();
                }

                ret.Property = armorProp;
                ret.Step = BinaryPrimitives.ReadSingleLittleEndian(data[20..]);
                return ret;
            }
        }

        partial class ArmorObjectTemplateBinaryWriteTranslation
        {
            public static partial void WriteBinaryOBTSLogicCustom(MutagenWriter writer, IArmorObjectTemplateGetter item)
            {
                using var header = HeaderExport.Subrecord(writer, RecordTypes.OBTS);
                writer.Write(item.Includes.Count);
                writer.Write(item.Properties.Count);
                writer.Write(item.LevelMin);
                writer.Write((byte)0);
                writer.Write(item.LevelMax);
                writer.Write((byte)0);
                writer.Write(item.AddonIndex);
                BooleanBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, item.Default);
                Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IFormLinkGetter<IKeywordGetter>>.Instance.Write(
                    writer: writer,
                    items: item.Keywords,
                    countLengthLength: 1,
                    transl: (MutagenWriter subWriter, IFormLinkGetter<IKeywordGetter> subItem, TypedWriteParams? conv) =>
                    {
                        FormLinkBinaryTranslation.Instance.Write(
                            writer: subWriter,
                            item: subItem);
                    });
                writer.Write(item.MinLevelForRanks);
                writer.Write(item.AltLevelsPerTier);
                Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IObjectTemplateIncludeGetter>.Instance.Write(
                    writer: writer,
                    items: item.Includes,
                    transl: (MutagenWriter subWriter, IObjectTemplateIncludeGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
                Mutagen.Bethesda.Plugins.Binary.Translations.ListBinaryTranslation<IAArmorObjectModPropertyGetter>.Instance.Write(
                    writer: writer,
                    items: item.Properties,
                    transl: (MutagenWriter subWriter, IAArmorObjectModPropertyGetter subItem) =>
                    {
                        WriteProperty(subWriter, subItem);
                    });
            }

            public static void WriteProperty(MutagenWriter writer, IAArmorObjectModPropertyGetter property)
            {
                switch (property)
                {
                    case IArmorObjectModIntPropertyGetter intProp:
                        WritePropertyFields(writer, property, ObjectModProperty.ValueType.Int, intProp.FunctionType);
                        writer.Write(intProp.Value);
                        writer.Write(intProp.Value2);
                        break;
                    case IArmorObjectModFloatPropertyGetter floatProp:
                        WritePropertyFields(writer, property, ObjectModProperty.ValueType.Float, floatProp.FunctionType);
                        writer.Write(floatProp.Value);
                        writer.Write(floatProp.Value2);
                        break;
                    case IArmorObjectModBoolPropertyGetter boolProp:
                        WritePropertyFields(writer, property, ObjectModProperty.ValueType.Bool, boolProp.FunctionType);
                        BooleanBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, boolProp.Value, 4);
                        BooleanBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(writer, boolProp.Value2, 4);
                        break;
                    case IArmorObjectModStringPropertyGetter stringProp:
                        WritePropertyFields(writer, property, ObjectModProperty.ValueType.String, stringProp.FunctionType);
                        StringBinaryTranslation.Instance.Write(writer, stringProp.Value.AsSpan().Slice(0, 4));
                        writer.Write(stringProp.Unused);
                        break;
                    case IArmorObjectModFormLinkIntPropertyGetter linkIntProp:
                        WritePropertyFields(writer, property, ObjectModProperty.ValueType.FormIdInt, linkIntProp.FunctionType);
                        FormLinkBinaryTranslation.Instance.Write(writer, linkIntProp.Record);
                        writer.Write(linkIntProp.Value);
                        break;
                    case IArmorObjectModFormLinkFloatPropertyGetter linkFloatProp:
                        WritePropertyFields(writer, property, ObjectModProperty.ValueType.FormIdFloat, linkFloatProp.FunctionType);
                        FormLinkBinaryTranslation.Instance.Write(writer, linkFloatProp.Record);
                        writer.Write(linkFloatProp.Value);
                        break;
                    default:
                        break;
                }
                writer.Write(property.Step);
            }

            private static void WritePropertyFields<TEnumType, TEnumFunction>(MutagenWriter writer, IAArmorObjectModPropertyGetter prop, TEnumType type, TEnumFunction func)
                where TEnumType : Enum, IConvertible
                where TEnumFunction : Enum, IConvertible
            {
                writer.Write((byte)type.ToInt32(null));
                writer.WriteZeros(3);
                writer.Write((byte)func.ToInt32(null));
                writer.WriteZeros(3);
                writer.Write((ushort)prop.Property);
                writer.WriteZeros(2);
            }
        }

        partial class ArmorObjectTemplateBinaryOverlay
        {
            private int? _obtsLoc;
            private uint _includeCount => BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(_obtsLoc!.Value));
            private uint _propertyCount => BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(_obtsLoc!.Value + 4));
            private byte? _keywordCount;
            private int? _postKeywordLoc;
            private int IncludeLoc => _postKeywordLoc!.Value + 2;
            
            public partial ParseResult OBTSLogicCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
            {
                _obtsLoc = (stream.Position - offset) + stream.MetaData.Constants.SubConstants.HeaderLength;
                _keywordCount = _data[_obtsLoc.Value + 15];
                stream.ReadSubrecordFrame(RecordTypes.OBTS);
                Keywords = BinaryOverlayList.FactoryByStartIndex(
                    _data.Slice(_obtsLoc.Value + 16, _keywordCount!.Value * 4),
                    _package,
                    itemLength: 4,
                    getter: (s, p) => new FormLink<IKeywordGetter>(FormKey.Factory(p.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(s))));
                _postKeywordLoc = _obtsLoc!.Value + 16 + (4 * _keywordCount!.Value);
                var includeLen = checked((int)(7 * _includeCount));
                Includes = BinaryOverlayList.FactoryByStartIndex(
                    _data.Slice(IncludeLoc, includeLen),
                    _package,
                    itemLength: 7,
                    getter: (s, p) => ObjectTemplateIncludeBinaryOverlay.ObjectTemplateIncludeFactory(s, p));
                var propLen = checked((int)(24 * _propertyCount));
                Properties = BinaryOverlayList.FactoryByStartIndex(
                    _data.Slice(IncludeLoc + includeLen, propLen),
                    _package,
                    itemLength: 24,
                    getter: (s, p) => ArmorObjectTemplateBinaryCreateTranslation.ReadProperty(stream.MetaData.MasterReferences, s));
                return (int)ArmorObjectTemplate_FieldIndex.Properties;
            }

            public byte LevelMin => _data[_obtsLoc!.Value + 8];
            public byte LevelMax => _data[_obtsLoc!.Value + 10];
            public short AddonIndex => BinaryPrimitives.ReadInt16LittleEndian(_data.Slice(_obtsLoc!.Value + 12));
            public bool Default => _data[_obtsLoc!.Value + 14] > 0;
            public IReadOnlyList<IFormLinkGetter<IKeywordGetter>> Keywords { get; private set; } = null!;
            IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? IKeywordedGetter.Keywords => Keywords;
            public byte MinLevelForRanks => _data[_postKeywordLoc!.Value];
            public byte AltLevelsPerTier => _data[_postKeywordLoc!.Value + 1];
            public IReadOnlyList<IObjectTemplateIncludeGetter> Includes { get; private set; } = null!;
            public IReadOnlyList<IAArmorObjectModPropertyGetter> Properties { get; private set; } = null!;
        }
    }
}