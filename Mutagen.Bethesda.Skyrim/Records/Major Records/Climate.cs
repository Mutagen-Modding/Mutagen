using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Skyrim;

public partial class Climate
{
    [Flags]
    public enum Moon
    {
        Masser = 0x01,
        Secunda = 0x02
    }
}

partial class ClimateBinaryCreateTranslation
{
    public const byte MasserFlag = 64;
    public const byte SecundaFlag = 128;

    public static partial void FillBinaryMoonAndPhaseLengthCustom(MutagenFrame frame, IClimateInternal item)
    {
        var raw = frame.ReadUInt8();
        item.PhaseLength = (byte)(raw % 64);
        item.Moons = default(Climate.Moon);
        if (Enums.HasFlag(raw, MasserFlag))
        {
            item.Moons |= Climate.Moon.Masser;
        }
        if (Enums.HasFlag(raw, SecundaFlag))
        {
            item.Moons |= Climate.Moon.Secunda;
        }
    }
    
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

    public static partial void FillBinarySunriseBeginCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseBegin = date;
        }
    }

    public static partial void FillBinarySunriseEndCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseEnd = date;
        }
    }

    public static partial void FillBinarySunsetBeginCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetBegin = date;
        }
    }

    public static partial void FillBinarySunsetEndCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetEnd = date;
        }
    }
}

partial class ClimateBinaryWriteTranslation
{
    public static partial void WriteBinaryMoonAndPhaseLengthCustom(MutagenWriter writer, IClimateGetter item)
    {
        var raw = item.PhaseLength;
        if (item.Moons.HasFlag(Climate.Moon.Masser))
        {
            raw = (byte)Enums.SetFlag(raw, ClimateBinaryCreateTranslation.MasserFlag, true);
        }
        if (item.Moons.HasFlag(Climate.Moon.Secunda))
        {
            raw = (byte)Enums.SetFlag(raw, ClimateBinaryCreateTranslation.SecundaFlag, true);
        }
        writer.Write(raw);
    }
    
    private static byte GetByte(TimeOnly d)
    {
        var mins = d.Hour * 60 + d.Minute;
        return (byte)(mins / 10);
    }

    public static partial void WriteBinarySunriseBeginCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunriseBegin));
    }

    public static partial void WriteBinarySunriseEndCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunriseEnd));
    }

    public static partial void WriteBinarySunsetBeginCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunsetBegin));
    }

    public static partial void WriteBinarySunsetEndCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunsetEnd));
    }
}

partial class ClimateBinaryOverlay
{
    public partial TimeOnly GetSunriseBeginCustom() => _TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[_TNAMLocation.Value.Min + 0]) : default;
    public partial TimeOnly GetSunriseEndCustom() => _TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[_TNAMLocation!.Value.Min + 1]) : default;
    public partial TimeOnly GetSunsetBeginCustom() => _TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[_TNAMLocation!.Value.Min + 2]) : default;
    public partial TimeOnly GetSunsetEndCustom() => _TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[_TNAMLocation!.Value.Min + 3]) : default;

    public Climate.Moon Moons
    {
        get
        {
            if (!_TNAMLocation.HasValue) return default;
            var raw = _recordData[_TNAMLocation.Value.Min + 5];
            var ret = default(Climate.Moon);
            if (Enums.HasFlag(raw, ClimateBinaryCreateTranslation.MasserFlag))
            {
                ret |= Climate.Moon.Masser;
            }
            if (Enums.HasFlag(raw, ClimateBinaryCreateTranslation.SecundaFlag))
            {
                ret |= Climate.Moon.Secunda;
            }
            return ret;
        }
    }

    public byte PhaseLength
    {
        get
        {
            if (!_TNAMLocation.HasValue) return default;
            return (byte)(_recordData[_TNAMLocation.Value.Min + 5] % 64);
        }
    }
}