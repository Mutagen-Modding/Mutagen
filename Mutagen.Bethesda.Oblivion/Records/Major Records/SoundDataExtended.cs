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

        static partial void FillBinary_StaticAttenuation_Custom(MutagenFrame frame, SoundDataExtended item, ErrorMaskBuilder errorMask)
        {
            if (!UInt16BinaryTranslation.Instance.Parse(
                frame.Spawn(snapToFinalPosition: false),
                out var i,
                errorMask))
            {
                return;
            }
            item.StaticAttenuation = i / 100f;
        }

        static partial void WriteBinary_StaticAttenuation_Custom(MutagenWriter writer, SoundDataExtended item, ErrorMaskBuilder errorMask)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)Math.Round(item.StaticAttenuation * 100),
                errorMask);
        }

        static partial void FillBinary_StartTime_Custom(MutagenFrame frame, SoundDataExtended item, ErrorMaskBuilder errorMask)
        {
            if (!ByteBinaryTranslation.Instance.Parse(
                frame.Spawn(snapToFinalPosition: false),
                out var b,
                errorMask))
            {
                return;
            }
            item.StartTime = b * 1440f / 256f;
        }

        static partial void WriteBinary_StartTime_Custom(MutagenWriter writer, SoundDataExtended item, ErrorMaskBuilder errorMask)
        {
            ByteBinaryTranslation.Instance.Write(
                writer,
                (byte)Math.Round(item.StartTime / 1440f * 256f),
                errorMask);
        }

        static partial void FillBinary_StopTime_Custom(MutagenFrame frame, SoundDataExtended item, ErrorMaskBuilder errorMask)
        {
            if (!ByteBinaryTranslation.Instance.Parse(
                frame.Spawn(snapToFinalPosition: false),
                out var b,
                errorMask))
            {
                return;
            }
            item.StopTime = b * 1440f / 256f;
        }

        static partial void WriteBinary_StopTime_Custom(MutagenWriter writer, SoundDataExtended item, ErrorMaskBuilder errorMask)
        {
            ByteBinaryTranslation.Instance.Write(
                writer,
                (byte)Math.Round(item.StopTime / 1440f * 256f),
                errorMask);
        }
    }
}
