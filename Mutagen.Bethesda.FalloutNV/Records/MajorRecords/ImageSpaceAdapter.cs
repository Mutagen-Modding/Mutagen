using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.FalloutNV;

public partial class ImageSpaceAdapter
{
    [Flags]
    public enum DepthOfFieldFlag
    {
        ModeFront = 0x01,
        ModeBack = 0x02,
        NoSky = 0x04,
    }
}

partial class ImageSpaceAdapterBinaryCreateTranslation
{
    public static partial void FillBinaryCounts1Custom(MutagenFrame frame, IImageSpaceAdapterInternal item)
    {
        frame.Position += 192;
    }

    public static partial void FillBinaryCounts2Custom(MutagenFrame frame, IImageSpaceAdapterInternal item)
    {
        frame.Position += 12;
    }

    public static partial void FillBinaryCounts3Custom(MutagenFrame frame, IImageSpaceAdapterInternal item)
    {
        frame.Position += 16;
    }
}

partial class ImageSpaceAdapterBinaryWriteTranslation
{
    public static partial void WriteBinaryCounts1Custom(MutagenWriter writer, IImageSpaceAdapterGetter item)
    {
        writer.Write(item.HdrEyeAdaptSpeedMult?.Count ?? 0);
        writer.Write(item.HdrEyeAdaptSpeedAdd?.Count ?? 0);
        writer.Write(item.HdrBlurRadiusMult?.Count ?? 0);
        writer.Write(item.HdrBlurRadiusAdd?.Count ?? 0);
        writer.Write(item.HdrSkinDimmerMult?.Count ?? 0);
        writer.Write(item.HdrSkinDimmerAdd?.Count ?? 0);
        writer.Write(item.HdrEmissiveMultMult?.Count ?? 0);
        writer.Write(item.HdrEmissiveMultAdd?.Count ?? 0);
        writer.Write(item.HdrTargetLumMult?.Count ?? 0);
        writer.Write(item.HdrTargetLumAdd?.Count ?? 0);
        writer.Write(item.HdrUpperLumClampMult?.Count ?? 0);
        writer.Write(item.HdrUpperLumClampAdd?.Count ?? 0);
        writer.Write(item.HdrBrightScaleMult?.Count ?? 0);
        writer.Write(item.HdrBrightScaleAdd?.Count ?? 0);
        writer.Write(item.HdrBrightClampMult?.Count ?? 0);
        writer.Write(item.HdrBrightClampAdd?.Count ?? 0);
        writer.Write(item.HdrLumRampNoTexMult?.Count ?? 0);
        writer.Write(item.HdrLumRampNoTexAdd?.Count ?? 0);
        writer.Write(item.HdrLumRampMinMult?.Count ?? 0);
        writer.Write(item.HdrLumRampMinAdd?.Count ?? 0);
        writer.Write(item.HdrLumRampMaxMult?.Count ?? 0);
        writer.Write(item.HdrLumRampMaxAdd?.Count ?? 0);
        writer.Write(item.HdrSunlightDimmerMult?.Count ?? 0);
        writer.Write(item.HdrSunlightDimmerAdd?.Count ?? 0);
        writer.Write(item.HdrGrassDimmerMult?.Count ?? 0);
        writer.Write(item.HdrGrassDimmerAdd?.Count ?? 0);
        writer.Write(item.HdrTreeDimmerMult?.Count ?? 0);
        writer.Write(item.HdrTreeDimmerAdd?.Count ?? 0);
        writer.Write(item.BloomBlurRadiusMult?.Count ?? 0);
        writer.Write(item.BloomBlurRadiusAdd?.Count ?? 0);
        writer.Write(item.BloomAlphaMultInteriorMult?.Count ?? 0);
        writer.Write(item.BloomAlphaMultInteriorAdd?.Count ?? 0);
        writer.Write(item.BloomAlphaMultExteriorMult?.Count ?? 0);
        writer.Write(item.BloomAlphaMultExteriorAdd?.Count ?? 0);
        writer.Write(item.CinematicSaturationMult?.Count ?? 0);
        writer.Write(item.CinematicSaturationAdd?.Count ?? 0);
        writer.Write(item.CinematicContrastMult?.Count ?? 0);
        writer.Write(item.CinematicContrastAdd?.Count ?? 0);
        writer.Write(item.CinematicContrastAvgLumMult?.Count ?? 0);
        writer.Write(item.CinematicContrastAvgLumAdd?.Count ?? 0);
        writer.Write(item.CinematicBrightnessMult?.Count ?? 0);
        writer.Write(item.CinematicBrightnessAdd?.Count ?? 0);
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
        stream.Position += 192;
    }

    partial void Counts2CustomParse(OverlayStream stream, int offset)
    {
        stream.Position += 12;
    }

    partial void Counts3CustomParse(OverlayStream stream, int offset)
    {
        stream.Position += 16;
    }
}
