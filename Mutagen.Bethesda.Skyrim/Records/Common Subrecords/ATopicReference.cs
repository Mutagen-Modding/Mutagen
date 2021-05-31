using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ATopicReference
    {
        public enum TopicType
        {
            Ref,
            Subtype
        }
    }

    namespace Internals
    {
        public partial class ATopicReferenceBinaryCreateTranslation
        {
            public static IEnumerable<ATopicReference> Factory(MutagenFrame frame)
            {
                while (frame.TryReadSubrecord(RecordTypes.PDTO, out var _))
                {
                    var type = (ATopicReference.TopicType)frame.ReadInt32();
                    switch (type)
                    {
                        case ATopicReference.TopicType.Ref:
                            yield return new TopicReference()
                            {
                                Reference = new FormLink<IDialogTopicGetter>(FormLinkBinaryTranslation.Instance.Parse(frame))
                            };
                            break;
                        case ATopicReference.TopicType.Subtype:
                            yield return new TopicReferenceSubtype()
                            {
                                Subtype = new RecordType(frame.ReadInt32())
                            };
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        public partial class ATopicReferenceBinaryWriteTranslation
        {
            public static void Write(MutagenWriter writer, IEnumerable<IATopicReferenceGetter> topics)
            {
                foreach (var item in topics)
                {
                    using var header = HeaderExport.Subrecord(writer, RecordTypes.PDTO);
                    switch (item)
                    {
                        case ITopicReferenceGetter refGetter:
                            writer.Write((int)ATopicReference.TopicType.Ref);
                            FormLinkBinaryTranslation.Instance.Write(writer, refGetter.Reference);
                            break;
                        case ITopicReferenceSubtypeGetter subType:
                            writer.Write((int)ATopicReference.TopicType.Subtype);
                            writer.Write(subType.Subtype.TypeInt);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
