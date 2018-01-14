using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class SoundDataExtended
    {
        private static byte[] _marker = new byte[] { 0 };
        public override byte[] Marker => _marker;

        static partial void FillBinary_StaticAttenuation_Custom(MutagenFrame frame, SoundDataExtended item, int fieldIndex, Func<SoundDataExtended_ErrorMask> errorMask)
        {
            var tryGet = UInt16BinaryTranslation.Instance.Parse(
                frame,
                fieldIndex,
                errorMask);
            if (!tryGet.Succeeded) return;
            item.StaticAttenuation = tryGet.Value / 100f;
        }

        static partial void WriteBinary_StaticAttenuation_Custom(MutagenWriter writer, SoundDataExtended item, int fieldIndex, Func<SoundDataExtended_ErrorMask> errorMask)
        {
            UInt16BinaryTranslation.Instance.Write(
                writer,
                (ushort)Math.Round(item.StaticAttenuation * 100),
                fieldIndex,
                errorMask);
        }

        static partial void FillBinary_StartTime_Custom(MutagenFrame frame, SoundDataExtended item, int fieldIndex, Func<SoundDataExtended_ErrorMask> errorMask)
        {
            var tryGet = ByteBinaryTranslation.Instance.Parse(
                frame,
                fieldIndex,
                errorMask);
            if (!tryGet.Succeeded) return;
            item.StartTime = tryGet.Value * 1440f / 256f;
        }

        static partial void WriteBinary_StartTime_Custom(MutagenWriter writer, SoundDataExtended item, int fieldIndex, Func<SoundDataExtended_ErrorMask> errorMask)
        {
            ByteBinaryTranslation.Instance.Write(
                writer,
                (byte)Math.Round(item.StartTime / 1440f * 256f),
                fieldIndex,
                errorMask);
        }

        static partial void FillBinary_StopTime_Custom(MutagenFrame frame, SoundDataExtended item, int fieldIndex, Func<SoundDataExtended_ErrorMask> errorMask)
        {
            var tryGet = ByteBinaryTranslation.Instance.Parse(
                frame,
                fieldIndex,
                errorMask);
            if (!tryGet.Succeeded) return;
            item.StopTime = tryGet.Value * 1440f / 256f;
        }

        static partial void WriteBinary_StopTime_Custom(MutagenWriter writer, SoundDataExtended item, int fieldIndex, Func<SoundDataExtended_ErrorMask> errorMask)
        {
            ByteBinaryTranslation.Instance.Write(
                writer,
                (byte)Math.Round(item.StopTime / 1440f * 256f),
                fieldIndex,
                errorMask);
        }
    }
}
