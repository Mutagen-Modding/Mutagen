using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Starfield;

partial class Water
{
    [Flags]
    public enum Flag
    {
        Dangerous = 0x01,
        DirectionalSound = 0x04,
    }
}

partial class WaterBinaryCreateTranslation
{
    public static partial void FillBinaryNoiseLayerParsingCustom(
        MutagenFrame frame,
        IWaterInternal item)
    {
        item.NoiseLayerOne.WindDirection = frame.ReadFloat();
        item.NoiseLayerTwo.WindDirection = frame.ReadFloat();
        item.NoiseLayerThree.WindDirection = frame.ReadFloat();
        item.NoiseLayerOne.WindSpeed = frame.ReadFloat();
        item.NoiseLayerTwo.WindSpeed = frame.ReadFloat();
        item.NoiseLayerThree.WindSpeed = frame.ReadFloat();
    }
}

partial class WaterBinaryWriteTranslation
{
    public static partial void WriteBinaryNoiseLayerParsingCustom(
        MutagenWriter writer,
        IWaterGetter item)
    {
        writer.Write(item.NoiseLayerOne.WindDirection);
        writer.Write(item.NoiseLayerTwo.WindDirection);
        writer.Write(item.NoiseLayerThree.WindDirection);
        writer.Write(item.NoiseLayerOne.WindSpeed);
        writer.Write(item.NoiseLayerTwo.WindSpeed);
        writer.Write(item.NoiseLayerThree.WindSpeed);
    }
}

partial class WaterBinaryOverlay
{
    private int _dataLoc => _SpecularInteriorSpecularPowerLocation + 4;

    public IWaterNoisePropertiesGetter NoiseLayerOne => WaterNoisePropertiesBinaryOverlay.WaterNoisePropertiesFactory(_recordData.Slice(_dataLoc), _package);

    public IWaterNoisePropertiesGetter NoiseLayerTwo => WaterNoisePropertiesBinaryOverlay.WaterNoisePropertiesFactory(_recordData.Slice(_dataLoc + 4), _package);

    public IWaterNoisePropertiesGetter NoiseLayerThree => WaterNoisePropertiesBinaryOverlay.WaterNoisePropertiesFactory(_recordData.Slice(_dataLoc + 8), _package);
}
