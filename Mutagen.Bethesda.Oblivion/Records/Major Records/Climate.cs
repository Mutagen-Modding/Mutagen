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

        partial void CustomCtor()
        {
            this._SunriseBegin = NotifyingItem.Factory<DateTime>(
                converter: (d) => DateConverter(d));
            this._SunriseEnd = NotifyingItem.Factory<DateTime>(
                converter: (d) => DateConverter(d));
            this._SunsetBegin = NotifyingItem.Factory<DateTime>(
                converter: (d) => DateConverter(d));
            this._SunsetEnd = NotifyingItem.Factory<DateTime>(
                converter: (d) => DateConverter(d));
        }

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
        
        static partial void FillBinary_SunriseBegin_Custom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
            {
                item.SunriseBegin = date;
            }
        }

        static partial void WriteBinary_SunriseBegin_Custom(MutagenWriter writer, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            writer.Write(GetByte(item.SunriseBegin));
        }

        static partial void FillBinary_SunriseEnd_Custom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
            {
                item.SunriseEnd = date;
            }
        }

        static partial void WriteBinary_SunriseEnd_Custom(MutagenWriter writer, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            writer.Write(GetByte(item.SunriseEnd));
        }

        static partial void FillBinary_SunsetBegin_Custom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (GetDate(frame.Reader.ReadUInt8(), out var date, errorMask))
            {
                item.SunsetBegin = date;
            }
        }

        static partial void WriteBinary_SunsetBegin_Custom(MutagenWriter writer, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            writer.Write(GetByte(item.SunsetBegin));
        }

        static partial void FillBinary_SunsetEnd_Custom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            if (GetDate(frame.Reader.ReadUInt8 (), out var date, errorMask))
            {
                item.SunsetEnd = date;
            }
        }

        static partial void WriteBinary_SunsetEnd_Custom(MutagenWriter writer, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            writer.Write(GetByte(item.SunsetEnd));
        }

        static partial void FillBinary_Phase_Custom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
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

        static partial void WriteBinary_Phase_Custom(MutagenWriter writer, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        {
            var eInt = (byte)(((int)item.Phase) * 64);
            eInt += item.PhaseLength;
            writer.Write(eInt);
        }

        static partial void FillBinary_PhaseLength_Custom(MutagenFrame frame, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        { // Handled in Phase section
        }

        static partial void WriteBinary_PhaseLength_Custom(MutagenWriter writer, Climate item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
        { // Handled in Phase section
        }
    }
}
