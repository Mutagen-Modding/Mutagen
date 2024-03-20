using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
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
    private static byte GetByte(TimeOnly d)
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
    private static bool GetTime(byte b, out TimeOnly date)
    {
        if (b > 144)
        {
            throw new ArgumentException("Cannot have a time value greater than 144");
            date = default(TimeOnly);
            return false;
        }
        date = new TimeOnly().AddMinutes(b * 10);
        return true;
    }

    public static TimeOnly GetTime(byte b)
    {
        if (b > 144)
        {
            throw new ArgumentException("Cannot have a time value greater than 144");
        }
        return new TimeOnly().AddMinutes(b * 10);
    }

    public static partial void FillBinarySunriseBeginCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseBegin = date;
        }
    }

    public static partial void FillBinarySunriseEndCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseEnd = date;
        }
    }

    public static partial void FillBinarySunsetBeginCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetBegin = date;
        }
    }

    public static partial void FillBinarySunsetEndCustom(MutagenFrame frame, IClimateData item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetEnd = date;
        }
    }

    public static int GetPhaseInt(byte b) => b / 64;
    public static byte GetPhaseLen(byte b) => (byte)(b % 64);

    public static partial void FillBinaryPhaseCustom(MutagenFrame frame, IClimateData item)
    {
        var b = frame.Reader.ReadUInt8();
        if (Enums<Climate.MoonPhase>.TryConvert(GetPhaseInt(b), out var e))
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
    public partial TimeOnly GetSunriseBeginCustom(int location) => ClimateDataBinaryCreateTranslation.GetTime(_structData.Span[0]);
    public partial TimeOnly GetSunriseEndCustom(int location) => ClimateDataBinaryCreateTranslation.GetTime(_structData.Span[1]);
    public partial TimeOnly GetSunsetBeginCustom(int location) => ClimateDataBinaryCreateTranslation.GetTime(_structData.Span[2]);
    public partial TimeOnly GetSunsetEndCustom(int location) => ClimateDataBinaryCreateTranslation.GetTime(_structData.Span[3]);
    public partial Climate.MoonPhase GetPhaseCustom(int location) => (Climate.MoonPhase)ClimateDataBinaryCreateTranslation.GetPhaseInt(_structData.Span[5]);
    public partial byte GetPhaseLengthCustom(int location) => ClimateDataBinaryCreateTranslation.GetPhaseLen(_structData.Span[5]);
}