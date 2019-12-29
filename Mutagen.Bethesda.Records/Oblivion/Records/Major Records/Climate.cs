using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
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
        public partial class ClimateBinaryWriteTranslation
        {
            private static byte GetByte(DateTime d)
            {
                var mins = d.Hour * 60 + d.Minute;
                return (byte)(mins / 10);
            }

            static partial void WriteBinarySunriseBeginCustom(MutagenWriter writer, IClimateGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunriseBegin));
            }

            static partial void WriteBinarySunriseEndCustom(MutagenWriter writer, IClimateGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunriseEnd));
            }

            static partial void WriteBinarySunsetBeginCustom(MutagenWriter writer, IClimateGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunsetBegin));
            }

            static partial void WriteBinarySunsetEndCustom(MutagenWriter writer, IClimateGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                writer.Write(GetByte(item.SunsetEnd));
            }

            static partial void WriteBinaryPhaseCustom(MutagenWriter writer, IClimateGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var eInt = (byte)(((int)item.Phase) * 64);
                eInt += item.PhaseLength;
                writer.Write(eInt);
            }

            static partial void WriteBinaryPhaseLengthCustom(MutagenWriter writer, IClimateGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            { // Handled in Phase section
            }
        }

        public partial class ClimateBinaryCreateTranslation
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

            public static DateTime GetDate(byte b)
            {
                if (b > 144)
                {
                    throw new ArgumentException("Cannot have a time value greater than 144");
                }
                return DateTime.MinValue.AddMinutes(b * 10);
            }

            static partial void FillBinarySunriseBeginCustom(MutagenFrame frame, IClimateInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunriseBegin = date;
                }
            }

            static partial void FillBinarySunriseEndCustom(MutagenFrame frame, IClimateInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunriseEnd = date;
                }
            }

            static partial void FillBinarySunsetBeginCustom(MutagenFrame frame, IClimateInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunsetBegin = date;
                }
            }

            static partial void FillBinarySunsetEndCustom(MutagenFrame frame, IClimateInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
                {
                    item.SunsetEnd = date;
                }
            }

            public static int GetPhaseInt(byte b) => b / 64;
            public static byte GetPhaseLen(byte b) => (byte)(b % 64);

            static partial void FillBinaryPhaseCustom(MutagenFrame frame, IClimateInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var b = frame.Reader.ReadUInt8();
                if (EnumExt.TryParse<Climate.MoonPhase>(GetPhaseInt(b), out var e))
                {
                    item.Phase = e;
                }
                else
                {
                    errorMask.ReportException(
                        new ArgumentException($"Unknown moon phase type: {b}"));
                }
                item.PhaseLength = GetPhaseLen(b);
            }

            static partial void FillBinaryPhaseLengthCustom(MutagenFrame frame, IClimateInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            { // Handled in Phase section
            }
        }

        public partial class ClimateBinaryOverlay
        {
            private bool GetSunriseBeginIsSetCustom() => _TNAMLocation.HasValue;
            private bool GetSunriseEndIsSetCustom() => _TNAMLocation.HasValue;
            private bool GetSunsetBeginIsSetCustom() => _TNAMLocation.HasValue;
            private bool GetSunsetEndIsSetCustom() => _TNAMLocation.HasValue;
            private bool GetPhaseIsSetCustom() => _TNAMLocation.HasValue;
            private bool GetPhaseLengthIsSetCustom() => _TNAMLocation.HasValue;
            private DateTime GetSunriseBeginCustom() => ClimateBinaryCreateTranslation.GetDate(_data.Span[_SunriseBeginLocation]);
            private DateTime GetSunriseEndCustom() => ClimateBinaryCreateTranslation.GetDate(_data.Span[_SunriseEndLocation]);
            private DateTime GetSunsetBeginCustom() => ClimateBinaryCreateTranslation.GetDate(_data.Span[_SunsetBeginLocation]);
            private DateTime GetSunsetEndCustom() => ClimateBinaryCreateTranslation.GetDate(_data.Span[_SunsetEndLocation]);
            private Climate.MoonPhase GetPhaseCustom() => (Climate.MoonPhase)ClimateBinaryCreateTranslation.GetPhaseInt(_data.Span[_PhaseLocation]);
            private byte GetPhaseLengthCustom() => ClimateBinaryCreateTranslation.GetPhaseLen(_data.Span[_PhaseLocation]);
        }
    }
}
