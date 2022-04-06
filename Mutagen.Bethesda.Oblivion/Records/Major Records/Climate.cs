using System;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Oblivion;

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

partial class ClimateDataBinaryWriteTranslation
{
    private static byte GetByte(DateTime d)
    {
        var mins = d.Hour * 60 + d.Minute;
        return (byte)(mins / 10);
    }

    public static partial void WriteBinarySunriseBeginCustom(MutagenWriter writer, IClimateDataGetter item)
    {
        writer.Write(GetByte(item.SunriseBegin));
    }

    public static partial void WriteBinarySunriseEndCustom(MutagenWriter writer, IClimateDataGetter item)
    {
        writer.Write(GetByte(item.SunriseEnd));
    }

    public static partial void WriteBinarySunsetBeginCustom(MutagenWriter writer, IClimateDataGetter item)
    {
        writer.Write(GetByte(item.SunsetBegin));
    }

    public static partial void WriteBinarySunsetEndCustom(MutagenWriter writer, IClimateDataGetter item)
    {
        writer.Write(GetByte(item.SunsetEnd));
    }

    public static partial void WriteBinaryPhaseCustom(MutagenWriter writer, IClimateDataGetter item)
    {
        var eInt = (byte)(((int)item.Phase) * 64);
        eInt += item.PhaseLength;
        writer.Write(eInt);
    }

    public static partial void WriteBinaryPhaseLengthCustom(MutagenWriter writer, IClimateDataGetter item)
    { // Handled in Phase section
    }
}

partial class ClimateDataBinaryCreateTranslation
{
    private static DateTime DateConverter(DateTime d)
    {
        return DateTime.MinValue
            .AddHours(d.Hour)
            .AddMinutes(d.Minute / 10 * 10);
    }

    private static bool GetDate(byte b, out DateTime date)
    {
        if (b > 144)
        {
            throw new ArgumentException("Cannot have a time value greater than 144");
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

    public static partial void FillBinarySunriseBeginCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetDate(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseBegin = date;
        }
    }

    public static partial void FillBinarySunriseEndCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetDate(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseEnd = date;
        }
    }

    public static partial void FillBinarySunsetBeginCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetDate(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetBegin = date;
        }
    }

    public static partial void FillBinarySunsetEndCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetDate(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetEnd = date;
        }
    }

    public static int GetPhaseInt(byte b) => b / 64;
    public static byte GetPhaseLen(byte b) => (byte)(b % 64);

    public static partial void FillBinaryPhaseCustom(MutagenFrame frame, IClimateData item)
    {
        var b = frame.Reader.ReadUInt8();
        if (EnumExt.TryParse<Climate.MoonPhase>(GetPhaseInt(b), out var e))
        {
            item.Phase = e;
        }
        else
        {
            throw new ArgumentException($"Unknown moon phase type: {b}");
        }
        item.PhaseLength = GetPhaseLen(b);
    }

    public static partial void FillBinaryPhaseLengthCustom(MutagenFrame frame, IClimateData item)
    { // Handled in Phase section
    }
}

partial class ClimateDataBinaryOverlay
{
    public partial DateTime GetSunriseBeginCustom(int location) => ClimateDataBinaryCreateTranslation.GetDate(_data.Span[0]);
    public partial DateTime GetSunriseEndCustom(int location) => ClimateDataBinaryCreateTranslation.GetDate(_data.Span[1]);
    public partial DateTime GetSunsetBeginCustom(int location) => ClimateDataBinaryCreateTranslation.GetDate(_data.Span[2]);
    public partial DateTime GetSunsetEndCustom(int location) => ClimateDataBinaryCreateTranslation.GetDate(_data.Span[3]);
    public partial Climate.MoonPhase GetPhaseCustom(int location) => (Climate.MoonPhase)ClimateDataBinaryCreateTranslation.GetPhaseInt(_data.Span[5]);
    public partial byte GetPhaseLengthCustom(int location) => ClimateDataBinaryCreateTranslation.GetPhaseLen(_data.Span[5]);
}