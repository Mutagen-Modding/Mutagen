using System;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

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
            public static partial void WriteBinaryStartTimeCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item)
            {
                ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    (byte)Math.Round(item.StartTime / 1440f * 256f));
            }

            public static partial void WriteBinaryStopTimeCustom(MutagenWriter writer, ISoundDataExtendedInternalGetter item)
            {
                ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    (byte)Math.Round(item.StopTime / 1440f * 256f));
            }
        }

        public partial class SoundDataExtendedBinaryCreateTranslation
        {
            public static float ConvertAttenuation(ushort i) => i / 100f;
            public static float ConvertTime(byte b) => b * 1440f / 256f;

            public static partial void FillBinaryStartTimeCustom(MutagenFrame frame, ISoundDataExtendedInternal item)
            {
                if (!ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(
                    frame,
                    out var b))
                {
                    return;
                }
                item.StartTime = ConvertTime(b);
            }

            public static partial void FillBinaryStopTimeCustom(MutagenFrame frame, ISoundDataExtendedInternal item)
            {
                if (!ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(
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
