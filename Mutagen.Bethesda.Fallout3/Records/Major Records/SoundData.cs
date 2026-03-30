using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout3;

public partial interface ISoundDataInternalGetter
{
    ReadOnlySpan<byte> Marker { get; }
}

public partial class SoundData
{
    public enum Flag
    {
        RandomFrequencyShift = 0,
        PlayAtRandom = 1,
        EnvironmentIgnored = 2,
        RandomLocation = 3,
        Loop = 4,
        MenuSound = 5,
        TwoDimensional = 6,
        LFE360 = 7,
        DialogueSound = 8,
        EnvelopeFast = 9,
        EnvelopeSlow = 10,
        TwoDimensionalRadius = 11,
        MuteWhenSubmerged = 12,
    }

    #region MinimumAttenuationDistance
    private UInt16 _MinimumAttenuationDistance;
    public UInt16 MinimumAttenuationDistance
    {
        get => _MinimumAttenuationDistance;
        set
        {
            if (value % MinAttenuationDistanceMultiplier != 0)
            {
                throw new ArgumentException($"{nameof(MinimumAttenuationDistance)} must be divisible by {MinAttenuationDistanceMultiplier}");
            }
            _MinimumAttenuationDistance = value;
        }
    }
    #endregion
    #region MaximumAttenuationDistance
    private UInt16 _MaximumAttenuationDistance;
    public UInt16 MaximumAttenuationDistance
    {
        get => _MaximumAttenuationDistance;
        set
        {
            if (value % MaxAttenuationDistanceMultiplier != 0)
            {
                throw new ArgumentException($"{nameof(MaximumAttenuationDistance)} must be divisible by {MaxAttenuationDistanceMultiplier}");
            }
            _MaximumAttenuationDistance = value;
        }
    }
    #endregion

    public const int MinAttenuationDistanceMultiplier = 5;
    public const int MaxAttenuationDistanceMultiplier = 100;

    private static byte[] _marker = new byte[] { 1 };
    public static ReadOnlySpan<byte> SoundDataMarker => _marker;
    public virtual ReadOnlySpan<byte> Marker => SoundDataMarker;
}

partial class SoundDataBinaryWriteTranslation
{
    public static partial void WriteBinaryMinimumAttenuationDistanceCustom(MutagenWriter writer, ISoundDataInternalGetter item)
    {
        var val = (byte)(item.MinimumAttenuationDistance / SoundData.MinAttenuationDistanceMultiplier);
        ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer: writer,
            item: val);
    }

    public static partial void WriteBinaryMaximumAttenuationDistanceCustom(MutagenWriter writer, ISoundDataInternalGetter item)
    {
        var val = (byte)(item.MaximumAttenuationDistance / SoundData.MaxAttenuationDistanceMultiplier);
        ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer: writer,
            item: val);
    }

    public static partial void WriteBinaryStartTimeCustom(MutagenWriter writer, ISoundDataInternalGetter item)
    {
        ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            (byte)Math.Round(item.StartTime / 1440f * 256f));
    }

    public static partial void WriteBinaryStopTimeCustom(MutagenWriter writer, ISoundDataInternalGetter item)
    {
        ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            (byte)Math.Round(item.StopTime / 1440f * 256f));
    }
}

partial class SoundDataBinaryCreateTranslation
{
    public static partial void FillBinaryMinimumAttenuationDistanceCustom(MutagenFrame frame, ISoundDataInternal item)
    {
        if (ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(
                reader: frame,
                item: out var b))
        {
            item.MinimumAttenuationDistance = (ushort)(b * SoundData.MinAttenuationDistanceMultiplier);
        }
    }

    public static partial void FillBinaryMaximumAttenuationDistanceCustom(MutagenFrame frame, ISoundDataInternal item)
    {
        if (ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(
                reader: frame,
                item: out var b))
        {
            item.MaximumAttenuationDistance = (ushort)(b * SoundData.MaxAttenuationDistanceMultiplier);
        }
    }
    
    public static float ConvertAttenuation(ushort i) => i / 100f;
    public static float ConvertTime(byte b) => b * 1440f / 256f;

    public static partial void FillBinaryStartTimeCustom(MutagenFrame frame, ISoundDataInternal item)
    {
        if (!ByteBinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Parse(
                frame,
                out var b))
        {
            return;
        }
        item.StartTime = ConvertTime(b);
    }

    public static partial void FillBinaryStopTimeCustom(MutagenFrame frame, ISoundDataInternal item)
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

partial class SoundDataBinaryOverlay
{
    public virtual ReadOnlySpan<byte> Marker => SoundData.SoundDataMarker;
    
    #region MinimumAttenuationDistance
    public partial UInt16 GetMinimumAttenuationDistanceCustom(int location);
    public UInt16 MinimumAttenuationDistance => GetMinimumAttenuationDistanceCustom(location: 0x0);
    #endregion
    #region MaximumAttenuationDistance
    public partial UInt16 GetMaximumAttenuationDistanceCustom(int location);
    public UInt16 MaximumAttenuationDistance => GetMaximumAttenuationDistanceCustom(location: 0x1);
    #endregion

    public partial ushort GetMinimumAttenuationDistanceCustom(int location)
    {
        return (ushort)(_structData.Span[location] * SoundData.MinAttenuationDistanceMultiplier);
    }

    public partial ushort GetMaximumAttenuationDistanceCustom(int location)
    {
        return (ushort)(_structData.Span[location] * SoundData.MaxAttenuationDistanceMultiplier);
    }
    
    public partial float GetStopTimeCustom(int location)
    {
        return SoundDataExtendedBinaryCreateTranslation.ConvertTime(_structData.Span[location]);
    }

    public partial float GetStartTimeCustom(int location)
    {
        return SoundDataExtendedBinaryCreateTranslation.ConvertTime(_structData.Span[location]);
    }
}