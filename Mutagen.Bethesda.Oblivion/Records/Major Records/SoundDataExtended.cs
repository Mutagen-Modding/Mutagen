using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class SoundDataExtended
    {
        private static byte[] _marker = new byte[] { 0 };
        public static ReadOnlySpan<byte> SoundDataExtendedMarker => _marker;
        public override ReadOnlySpan<byte> Marker => SoundDataExtendedMarker;
    }

    namespace Internals
    {
        public partial class SoundDataExtendedBinaryWriteTranslation
        {
            static partial void WriteBinaryStaticAttenuationCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                UInt16BinaryTranslation.Instance.Write(
                    writer,
                    (ushort)Math.Round(item.StaticAttenuation * 100));
            }

            static partial void WriteBinaryStartTimeCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                ByteBinaryTranslation.Instance.Write(
                    writer,
                    (byte)Math.Round(item.StartTime / 1440f * 256f));
            }

            static partial void WriteBinaryStopTimeCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                ByteBinaryTranslation.Instance.Write(
                    writer,
                    (byte)Math.Round(item.StopTime / 1440f * 256f));
            }
        }

        public partial class SoundDataExtendedBinaryCreateTranslation
        {
            public static float ConvertAttenuation(ushort i) => i / 100f;
            public static float ConvertTime(byte b) => b * 1440f / 256f;

            static partial void FillBinaryStaticAttenuationCustom(MutagenFrame frame, SoundDataExtended item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!UInt16BinaryTranslation.Instance.Parse(
                    frame,
                    out var i))
                {
                    return;
                }
                item.StaticAttenuation = ConvertAttenuation(i);
            }

            static partial void FillBinaryStartTimeCustom(MutagenFrame frame, SoundDataExtended item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!ByteBinaryTranslation.Instance.Parse(
                    frame,
                    out var b))
                {
                    return;
                }
                item.StartTime = ConvertTime(b);
            }

            static partial void FillBinaryStopTimeCustom(MutagenFrame frame, SoundDataExtended item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!ByteBinaryTranslation.Instance.Parse(
                    frame,
                    out var b))
                {
                    return;
                }
                item.StopTime = ConvertTime(b);
            }
        }
    
        public partial class SoundDataExtendedBinaryWrapper
        {
            public override ReadOnlySpan<byte> Marker => SoundDataExtended.SoundDataExtendedMarker;

            public float GetStaticAttenuationCustom(
                ReadOnlySpan<byte> span,
                int location,
                int? expectedLength,
                BinaryWrapperFactoryPackage package)
            {
                return SoundDataExtendedBinaryCreateTranslation.ConvertAttenuation(BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(location)));
            }

            public float GetStopTimeCustom(
                ReadOnlySpan<byte> span,
                int location,
                int? expectedLength,
                BinaryWrapperFactoryPackage package)
            {
                return SoundDataExtendedBinaryCreateTranslation.ConvertTime(span[location]);
            }

            public float GetStartTimeCustom(
                ReadOnlySpan<byte> span,
                int location,
                int? expectedLength,
                BinaryWrapperFactoryPackage package)
            {
                return SoundDataExtendedBinaryCreateTranslation.ConvertTime(span[location]);
            }
        }
    }
}
