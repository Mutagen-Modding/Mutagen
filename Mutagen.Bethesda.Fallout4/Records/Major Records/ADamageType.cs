using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class ADamageType
    {
        public static ADamageType CreateFromBinary(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter)
        {
            var majorMeta = frame.GetMajorRecordFrame();
            if (majorMeta.FormVersion >= 78)
            {
                return DamageType.CreateFromBinary(frame, recordTypeConverter); 
            }
            else
            {
                return DamageTypeIndexed.CreateFromBinary(frame, recordTypeConverter);
            }
        }
    }

    namespace Internals
    {
        public partial class ADamageTypeBinaryOverlay
        {
            public static ADamageTypeBinaryOverlay ADamageTypeFactory(
                OverlayStream stream,
                BinaryOverlayFactoryPackage package,
                RecordTypeConverter recordTypeConverter)
            {
                var majorFrame = package.MetaData.Constants.MajorRecordFrame(stream.RemainingMemory);
                if (majorFrame.FormVersion >= 78)
                {
                    return DamageTypeBinaryOverlay.DamageTypeFactory(stream, package, recordTypeConverter);
                }
                else
                {
                    return  DamageTypeIndexedBinaryOverlay.DamageTypeIndexedFactory(stream, package, recordTypeConverter);
                }
            }
        }

        public partial class ADamageTypeBinaryCreateTranslation
        {
            public static partial void FillBinaryCustomLogicCustom(MutagenFrame frame, IADamageTypeInternal item)
            {
            }
        }

        public partial class ADamageTypeBinaryWriteTranslation
        {
            public static partial void WriteBinaryCustomLogicCustom(MutagenWriter writer, IADamageTypeGetter item)
            {
            }
        }
    }
}
