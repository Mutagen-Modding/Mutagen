using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

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
}

partial class ClimateBinaryOverlay
{
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