using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class BodyTemplate
    {
        [Flags]
        public enum Flag
        {
            ModulatesVoice = 0x01,
            NonPlayable = 0x10,
        }
    }

    namespace Internals
    {
        public partial class BodyTemplateBinaryCreateTranslation
        {
            public static BodyTemplate Parse(MutagenFrame frame)
            {
                var subFrame = frame.ReadSubrecord();
                var version = frame.MetaData.FormVersion!.Value;
                switch (subFrame.RecordTypeInt)
                {
                    case RecordTypeInts.BODT:
                        return ParseBodt(version, frame, subFrame);
                    case RecordTypeInts.BOD2:
                        return ParseBod2(version, frame, subFrame);
                    default:
                        throw new ArgumentException();
                }
            }

            public static BodyTemplate ParseBod2(ushort version, IMutagenReadStream frame, SubrecordHeader subrecordHeader)
            {
                var len = subrecordHeader.ContentLength;
                if (version <= 22 && len <= 8)
                {
                    throw SubrecordException.Create("BOD2 can not be parsed on Form Versions <= 22 with length <= 8", RecordTypes.BOD2);
                }

                var item = new BodyTemplate();
                item.ActsLike44 = true;
                item.FirstPersonFlags = EnumBinaryTranslation<BipedObjectFlag, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                    reader: frame,
                    length: 4);
                if (len == 8)
                {
                    item.ArmorType = EnumBinaryTranslation<ArmorType, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                        reader: frame,
                        length: 4);
                }
                else
                {
                    item.Flags = EnumBinaryTranslation<BodyTemplate.Flag, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                        reader: frame,
                        length: 4);
                    item.ArmorType = EnumBinaryTranslation<ArmorType, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                        reader: frame,
                        length: 4);
                }
                return item;
            }

            public static BodyTemplate ParseBodt(ushort version, IMutagenReadStream frame, SubrecordHeader subrecordHeader)
            {
                var len = subrecordHeader.ContentLength;
                if (version == 44 && len <= 8)
                {
                    throw SubrecordException.Create("BODT can not be parsed on versions == 44 if length is <= 8", RecordTypes.BODT);
                }

                var item = new BodyTemplate();

                item.FirstPersonFlags = EnumBinaryTranslation<BipedObjectFlag, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                    reader: frame,
                    length: 4);
                if (len == 8)
                {
                    if (version < 22)
                    {
                        item.Flags = EnumBinaryTranslation<BodyTemplate.Flag, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                            reader: frame,
                            length: 4);
                    }
                    else
                    {
                        item.ArmorType = EnumBinaryTranslation<ArmorType, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                            reader: frame,
                            length: 4);
                    }
                }
                else
                {
                    item.Flags = EnumBinaryTranslation<BodyTemplate.Flag, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                        reader: frame,
                        length: 4);
                    item.ArmorType = EnumBinaryTranslation<ArmorType, IMutagenReadStream, MutagenWriter>.Instance.Parse(
                        reader: frame,
                        length: 4);
                }
                return item;
            }
        }

        public partial class BodyTemplateBinaryWriteTranslation
        {
            public static void Write(MutagenWriter writer, IBodyTemplateGetter template)
            {
                if (!template.ActsLike44)
                {
                    template.WriteToBinary(writer);
                    return;
                }
                writer.MetaData.FormVersion = 44;
                template.WriteToBinary(writer);
                writer.MetaData.FormVersion = 43;
            }
        }

        public partial class BodyTemplateBinaryOverlay
        {
            public bool ActsLike44 { get; private set; }

            public static IBodyTemplateGetter? CustomFactory(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                var subFrame = stream.ReadSubrecord();
                var version = package.FormVersion!.FormVersion!.Value;
                switch (subFrame.RecordTypeInt)
                {
                    case RecordTypeInts.BODT:
                        return BodyTemplateBinaryCreateTranslation.ParseBodt(version, stream, subFrame);
                    case RecordTypeInts.BOD2:
                        return BodyTemplateBinaryCreateTranslation.ParseBod2(version, stream, subFrame);
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}
