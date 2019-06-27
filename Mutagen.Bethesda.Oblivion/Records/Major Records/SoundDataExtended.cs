using System;
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
        public override byte[] Marker => _marker;
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
            static partial void FillBinaryStaticAttenuationCustom(MutagenFrame frame, SoundDataExtended item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!UInt16BinaryTranslation.Instance.Parse(
                    frame,
                    out var i))
                {
                    return;
                }
                item.StaticAttenuation = i / 100f;
            }

            static partial void FillBinaryStartTimeCustom(MutagenFrame frame, SoundDataExtended item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!ByteBinaryTranslation.Instance.Parse(
                    frame,
                    out var b))
                {
                    return;
                }
                item.StartTime = b * 1440f / 256f;
            }

            static partial void FillBinaryStopTimeCustom(MutagenFrame frame, SoundDataExtended item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!ByteBinaryTranslation.Instance.Parse(
                    frame,
                    out var b))
                {
                    return;
                }
                item.StopTime = b * 1440f / 256f;
            }
        }
    }
}
