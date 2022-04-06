using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class ImageSpaceAdapter
{
    [Flags]
    public enum Flag
    {
        Animatable = 0x01
    }

    [Flags]
    public enum RadialBlurFlag
    {
        UseTarget = 0x01
    }

    [Flags]
    public enum DepthOfFieldFlag
    {
        UseTarget = 0x0001,
        ModeFront = 0x0100,
        ModeBack = 0x0200,
        NoSky = 0x0400,
        BlurRadiusBit2 = 0x0800,
        BlurRadiusBit1 = 0x1000,
        BlurRadiusBit0 = 0x2000,
    }
}

partial class ImageSpaceAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryCounts1Custom(MutagenFrame frame, IImageSpaceAdapterInternal item)
    {
        // Don't care about counts, currently
        frame.Position += 192;
    }

    public static partial void FillBinaryCounts2Custom(MutagenFrame frame, IImageSpaceAdapterInternal item)
    {
        // Don't care about counts, currently
        frame.Position += 12;
    }

    public static partial void FillBinaryCounts3Custom(MutagenFrame frame, IImageSpaceAdapterInternal item)
    {
        // Don't care about counts, currently
        frame.Position += 16;
    }
}

partial class ImageSpaceAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryCounts1Custom(MutagenWriter writer, IImageSpaceAdapterGetter item)
    {
        writer.Write(item.HdrEyeAdaptSpeedMult?.Count ?? 0);
        writer.Write(item.HdrEyeAdaptSpeedAdd?.Count ?? 0);
        writer.Write(item.HdrBloomBlurRadiusMult?.Count ?? 0);
        writer.Write(item.HdrBloomBlurRadiusAdd?.Count ?? 0);
        writer.Write(item.HdrBloomThresholdMult?.Count ?? 0);
        writer.Write(item.HdrBloomThresholdAdd?.Count ?? 0);
        writer.Write(item.HdrBloomScaleMult?.Count ?? 0);
        writer.Write(item.HdrBloomScaleAdd?.Count ?? 0);
        writer.Write(item.HdrTargetLumMinMult?.Count ?? 0);
        writer.Write(item.HdrTargetLumMinAdd?.Count ?? 0);
        writer.Write(item.HdrTargetLumMaxMult?.Count ?? 0);
        writer.Write(item.HdrTargetLumMaxAdd?.Count ?? 0);
        writer.Write(item.HdrSunlightScaleMult?.Count ?? 0);
        writer.Write(item.HdrSunlightScaleAdd?.Count ?? 0);
        writer.Write(item.HdrSkyScaleMult?.Count ?? 0);
        writer.Write(item.HdrSkyScaleAdd?.Count ?? 0);
        writer.Write(item.Unknown08?.Count ?? 0);
        writer.Write(item.Unknown48?.Count ?? 0);
        writer.Write(item.Unknown09?.Count ?? 0);
        writer.Write(item.Unknown49?.Count ?? 0);
        writer.Write(item.Unknown0A?.Count ?? 0);
        writer.Write(item.Unknown4A?.Count ?? 0);
        writer.Write(item.Unknown0B?.Count ?? 0);
        writer.Write(item.Unknown4B?.Count ?? 0);
        writer.Write(item.Unknown0C?.Count ?? 0);
        writer.Write(item.Unknown4C?.Count ?? 0);
        writer.Write(item.Unknown0D?.Count ?? 0);
        writer.Write(item.Unknown4D?.Count ?? 0);
        writer.Write(item.Unknown0E?.Count ?? 0);
        writer.Write(item.Unknown4E?.Count ?? 0);
        writer.Write(item.Unknown0F?.Count ?? 0);
        writer.Write(item.Unknown4F?.Count ?? 0);
        writer.Write(item.Unknown10?.Count ?? 0);
        writer.Write(item.Unknown50?.Count ?? 0);
        writer.Write(item.CinematicSaturationMult?.Count ?? 0);
        writer.Write(item.CinematicSaturationAdd?.Count ?? 0);
        writer.Write(item.CinematicBrightnessMult?.Count ?? 0);
        writer.Write(item.CinematicBrightnessAdd?.Count ?? 0);
        writer.Write(item.CinematicContrastMult?.Count ?? 0);
        writer.Write(item.CinematicContrastAdd?.Count ?? 0);
        writer.Write(item.Unknown14?.Count ?? 0);
        writer.Write(item.Unknown54?.Count ?? 0);
        writer.Write(item.TintColor?.Count ?? 0);
        writer.Write(item.BlurRadius?.Count ?? 0);
        writer.Write(item.DoubleVisionStrength?.Count ?? 0);
        writer.Write(item.RadialBlurStrength?.Count ?? 0);
        writer.Write(item.RadialBlurRampUp?.Count ?? 0);
        writer.Write(item.RadialBlurStart?.Count ?? 0);
    }

    public static partial void WriteBinaryCounts2Custom(MutagenWriter writer, IImageSpaceAdapterGetter item)
    {
        writer.Write(item.DepthOfFieldStrength?.Count ?? 0);
        writer.Write(item.DepthOfFieldDistance?.Count ?? 0);
        writer.Write(item.DepthOfFieldRange?.Count ?? 0);
    }

    public static partial void WriteBinaryCounts3Custom(MutagenWriter writer, IImageSpaceAdapterGetter item)
    {
        writer.Write(item.RadialBlurRampDown?.Count ?? 0);
        writer.Write(item.RadialBlurDownStart?.Count ?? 0);
        writer.Write(item.FadeColor?.Count ?? 0);
        writer.Write(item.MotionBlurStrength?.Count ?? 0);
    }
}

partial class ImageSpaceAdapterBinaryOverlay
{
    partial void Counts1CustomParse(OverlayStream stream, int offset)
    {
        // Don't care about counts, currently
        stream.Position += 192;
    }

    partial void Counts2CustomParse(OverlayStream stream, int offset)
    {
        // Don't care about counts, currently
        stream.Position += 12;
    }

    partial void Counts3CustomParse(OverlayStream stream, int offset)
    {
        // Don't care about counts, currently
        stream.Position += 16;
    }
}