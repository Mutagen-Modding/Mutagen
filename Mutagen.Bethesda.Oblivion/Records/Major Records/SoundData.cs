using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Oblivion;

public partial interface ISoundDataInternalGetter
{
    ReadOnlySpan<byte> Marker { get; }
}

public partial class SoundData
{
    [Flags]
    public enum Flag
    {
        RandomFrequencyShift = 0x01,
        PlayAtRandom = 0x02,
        EnvironmentIgnored = 0x04,
        RandomLocation = 0x08,
        Loop = 0x10,
        MenuSound = 0x20,
        TwoDimensional = 0x40,
        LFE360 = 0x80,
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
}