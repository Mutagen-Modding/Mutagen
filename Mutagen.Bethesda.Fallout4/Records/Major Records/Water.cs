using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Fallout4;

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
        item.NoiseLayerOne.AmplitudeScale = frame.ReadFloat();
        item.NoiseLayerTwo.AmplitudeScale = frame.ReadFloat();
        item.NoiseLayerThree.AmplitudeScale = frame.ReadFloat();
        item.NoiseLayerOne.UvScale = frame.ReadFloat();
        item.NoiseLayerTwo.UvScale = frame.ReadFloat();
        item.NoiseLayerThree.UvScale = frame.ReadFloat();
        item.NoiseLayerOne.NoiseFalloff = frame.ReadFloat();
        item.NoiseLayerTwo.NoiseFalloff = frame.ReadFloat();
        item.NoiseLayerThree.NoiseFalloff = frame.ReadFloat();
    }

    public static partial ParseResult FillBinaryNoiseTextureParsingCustom(
            MutagenFrame frame,
        IWaterInternal item)
    {
        var subRecord = frame.ReadSubrecord();
        switch (subRecord.RecordTypeInt)
        {
            case RecordTypeInts.NAM2:
                item.NoiseLayerOne.Texture = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, frame.MetaData.Encodings.NonLocalized);
                return null;
            case RecordTypeInts.NAM3:
                item.NoiseLayerTwo.Texture = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, frame.MetaData.Encodings.NonLocalized);
                return null;
            case RecordTypeInts.NAM4:
                item.NoiseLayerThree.Texture = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, frame.MetaData.Encodings.NonLocalized);
                return null;
            default:
                throw new MalformedDataException($"Unexpected record type: {subRecord.RecordType}");
        }
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
        writer.Write(item.NoiseLayerOne.AmplitudeScale);
        writer.Write(item.NoiseLayerTwo.AmplitudeScale);
        writer.Write(item.NoiseLayerThree.AmplitudeScale);
        writer.Write(item.NoiseLayerOne.UvScale);
        writer.Write(item.NoiseLayerTwo.UvScale);
        writer.Write(item.NoiseLayerThree.UvScale);
        writer.Write(item.NoiseLayerOne.NoiseFalloff);
        writer.Write(item.NoiseLayerTwo.NoiseFalloff);
        writer.Write(item.NoiseLayerThree.NoiseFalloff);
    }

    public static partial void WriteBinaryNoiseTextureParsingCustom(
        MutagenWriter writer,
        IWaterGetter item)
    {
        if (item.NoiseLayerOne.Texture is { } oneTexture)
        {
            StringBinaryTranslation.Instance.Write(writer, oneTexture, RecordTypes.NAM2);
        }
        if (item.NoiseLayerTwo.Texture is { } twoTexture)
        {
            StringBinaryTranslation.Instance.Write(writer, twoTexture, RecordTypes.NAM3);
        }
        if (item.NoiseLayerThree.Texture is { } threeTexture)
        {
            StringBinaryTranslation.Instance.Write(writer, threeTexture, RecordTypes.NAM4);
        }
    }
}

partial class WaterBinaryOverlay
{
    private SubrecordFrame? _nam2;
    private SubrecordFrame? _nam3;
    private SubrecordFrame? _nam4;
    private int _dataLoc => _SpecularInteriorSpecularPowerLocation + 4;

    public IWaterNoisePropertiesGetter NoiseLayerOne => new WaterNoisePropertiesBinaryOverlay(_recordData.Slice(_dataLoc), _nam2, _package);

    public IWaterNoisePropertiesGetter NoiseLayerTwo => new WaterNoisePropertiesBinaryOverlay(_recordData.Slice(_dataLoc + 4), _nam3, _package);

    public IWaterNoisePropertiesGetter NoiseLayerThree => new WaterNoisePropertiesBinaryOverlay(_recordData.Slice(_dataLoc + 8), _nam4, _package);

    public partial ParseResult NoiseTextureParsingCustomParse(
        OverlayStream stream,
        int offset)
    {
        var rec = stream.ReadSubrecord();
        switch (rec.RecordTypeInt)
        {
            case RecordTypeInts.NAM2:
                _nam2 = rec;
                break;
            case RecordTypeInts.NAM3:
                _nam3 = rec;
                break;
            case RecordTypeInts.NAM4:
                _nam4 = rec;
                break;
            default:
                throw new MalformedDataException($"Unexpected record type: {rec.RecordType}");
        }
        return (int)Water_FieldIndex.ScreenSpaceReflections;
    }
}
