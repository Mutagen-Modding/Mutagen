using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Climate
    {
        public enum MoonPhase
        {
            NoMoon,
            Masser,
            Secunda,
            MasserSecunda,
        }
    }

    namespace Internals
    {
        public partial class ClimateBinaryTranslation
        {
            private static DateTime DateConverter(DateTime d)
            {
                return DateTime.MinValue
                    .AddHours(d.Hour)
                    .AddMinutes(d.Minute / 10 * 10);
            }

            private static bool GetDate(byte b, out DateTime date, ErrorMaskBuilder errorMask)
            {
                if (b > 144)
                {
                    errorMask.ReportExceptionOrThrow(new ArgumentException("Cannot have a time value greater than 144"));
                    date = default(DateTime);
                    return false;
                }
                date = DateTime.MinValue.AddMinutes(b * 10);
                return true;
            }

            private static byte GetByte(DateTime d)
            {
                var mins = d.Hour * 60 + d.Minute;
                return (byte)(mins / 10);
            }

            static partial void FillBinarySunriseBeginCustom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunriseBegin = date;
                }
            }

            static partial void WriteBinarySunriseBeginCustom(MutagenWriter writer, IClimateInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunriseBegin));
            }

            static partial void FillBinarySunriseEndCustom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunriseEnd = date;
                }
            }

            static partial void WriteBinarySunriseEndCustom(MutagenWriter writer, IClimateInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunriseEnd));
            }

            static partial void FillBinarySunsetBeginCustom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunsetBegin = date;
                }
            }

            static partial void WriteBinarySunsetBeginCustom(MutagenWriter writer, IClimateInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunsetBegin));
            }

            static partial void FillBinarySunsetEndCustom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunsetEnd = date;
                }
            }

            static partial void WriteBinarySunsetEndCustom(MutagenWriter writer, IClimateInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunsetEnd));
            }

            static partial void FillBinaryPhaseCustom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var b1 = frame.Reader.ReadUInt8();
                var eInt = b1 / 64;
                var phaseLen = (byte)(b1 % 64);
                if (EnumExt.TryParse<Climate.MoonPhase>(eInt, out var e))
                {
                    item.Phase = e;
                }
                else
                {
                    errorMask.ReportException(
                        new ArgumentException($"Unknown moon phase type: {b1}"));
                }
                item.PhaseLength = phaseLen;
            }

            static partial void WriteBinaryPhaseCustom(MutagenWriter writer, IClimateInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var eInt = (byte)(((int)item.Phase) * 64);
                eInt += item.PhaseLength;
                writer.Write(eInt);
            }

            static partial void FillBinaryPhaseLengthCustom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            { // Handled in Phase section
            }

            static partial void WriteBinaryPhaseLengthCustom(MutagenWriter writer, IClimateInternalGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            { // Handled in Phase section
            }
        }
    }
}
