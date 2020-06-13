using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
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
            static partial void WriteBinaryStartTimeCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item)
            {
                ByteBinaryTranslation.Instance.Write(
                    writer,
                    (byte)Math.Round(item.StartTime / 1440f * 256f));
            }

            static partial void WriteBinaryStopTimeCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item)
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

            static partial void FillBinaryStartTimeCustom(MutagenFrame frame, ISoundDataExtendedInternal item)
            {
                if (!ByteBinaryTranslation.Instance.Parse(
                    frame,
                    out var b))
                {
                    return;
                }
                item.StartTime = ConvertTime(b);
            }

            static partial void FillBinaryStopTimeCustom(MutagenFrame frame, ISoundDataExtendedInternal item)
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
    
        public partial class SoundDataExtendedBinaryOverlay
        {
            public override ReadOnlySpan<byte> Marker => SoundDataExtended.SoundDataExtendedMarker;

            public float GetStopTimeCustom(int location)
            {
                return SoundDataExtendedBinaryCreateTranslation.ConvertTime(_data.Span[location]);
            }

            public float GetStartTimeCustom(int location)
            {
                return SoundDataExtendedBinaryCreateTranslation.ConvertTime(_data.Span[location]);
            }
        }
    }
}
