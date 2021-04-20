using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Skyrim
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
                var subFrame = frame.GetSubrecordFrame();
                var version = frame.MetaData.FormVersion!.Value;
                switch (subFrame.RecordTypeInt)
                {
                    case RecordTypeInts.BODT:
                        if (version >= 44)
                        {
                            throw new ArgumentException("BODT type not expected on versions >= 44");
                        }
                        return BodyTemplate.CreateFromBinary(frame);
                    case RecordTypeInts.BOD2:
                        if (version < 43)
                        {
                            throw new ArgumentException("BOD2 type not expected on versions < 43");
                        }
                        if (version >= 44)
                        {
                            return BodyTemplate.CreateFromBinary(frame);
                        }
                        frame.MetaData.FormVersion = 44;
                        var template = BodyTemplate.CreateFromBinary(frame);
                        template.ActsLike44 = true;
                        frame.MetaData.FormVersion = 43;
                        return template;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public partial class BodyTemplateBinaryWriteTranslation
        {
            public static void Write(MutagenWriter writer, IBodyTemplateGetter template)
            {
                if (writer.MetaData.FormVersion != 43
                    || !template.ActsLike44)
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
                var subFrame = stream.GetSubrecordFrame();
                var version = package.FormVersion!.FormVersion!.Value;
                switch (subFrame.RecordTypeInt)
                {
                    case RecordTypeInts.BODT:
                        if (version >= 44)
                        {
                            throw new ArgumentException("BODT type not expected on versions >= 44");
                        }
                        return BodyTemplateBinaryOverlay.BodyTemplateFactory(stream, package);
                    case RecordTypeInts.BOD2:
                        if (version < 43)
                        {
                            throw new ArgumentException("BOD2 type not expected on versions < 43");
                        }
                        if (version >= 44)
                        {
                            return BodyTemplateBinaryOverlay.BodyTemplateFactory(stream, package);
                        }
                        var cur = package.FormVersion;
                        package.FormVersion = new FormVersionGetter()
                        {
                            FormVersion = 44
                        };
                        var template = BodyTemplateBinaryOverlay.BodyTemplateFactory(stream, package);
                        template.ActsLike44 = true;
                        package.FormVersion = cur;
                        return template;
                    default:
                        throw new ArgumentException();
                }
            }
        }
    }
}
