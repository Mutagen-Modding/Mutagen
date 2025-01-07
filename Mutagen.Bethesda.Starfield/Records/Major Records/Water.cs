using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Starfield.Internals;

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
    // public static partial void FillBinaryNoiseLayerParsingCustom(
    //     MutagenFrame frame,
    //     IWaterInternal item)
    // {
    //     item.NoiseLayerOne.WindDirection = frame.ReadFloat();
    //     item.NoiseLayerTwo.WindDirection = frame.ReadFloat();
    //     item.NoiseLayerThree.WindDirection = frame.ReadFloat();
    //     item.NoiseLayerOne.WindSpeed = frame.ReadFloat();
    //     item.NoiseLayerTwo.WindSpeed = frame.ReadFloat();
    //     item.NoiseLayerThree.WindSpeed = frame.ReadFloat();
    // }
    //
    // public static partial ParseResult FillBinaryNoiseTextureParsingCustom(
    //     MutagenFrame frame,
    //     IWaterInternal item,
    //     PreviousParse lastParsed)
    // {
    //     var subRecord = frame.ReadSubrecord();
    //     switch (subRecord.RecordTypeInt)
    //     {
    //         case RecordTypeInts.NAM2:
    //             item.NoiseLayerOne.Texture = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, frame.MetaData.Encodings.NonLocalized);
    //             return null;
    //         case RecordTypeInts.NAM3:
    //             item.NoiseLayerTwo.Texture = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, frame.MetaData.Encodings.NonLocalized);
    //             return null;
    //         case RecordTypeInts.NAM4:
    //             item.NoiseLayerThree.Texture = BinaryStringUtility.ProcessWholeToZString(subRecord.Content, frame.MetaData.Encodings.NonLocalized);
    //             return null;
    //         default:
    //             throw new MalformedDataException($"Unexpected record type: {subRecord.RecordType}");
    //     }
    // }
}

partial class WaterBinaryWriteTranslation
{
    // public static partial void WriteBinaryNoiseLayerParsingCustom(
    //     MutagenWriter writer,
    //     IWaterGetter item)
    // {
    //     writer.Write(item.NoiseLayerOne.WindDirection);
    //     writer.Write(item.NoiseLayerTwo.WindDirection);
    //     writer.Write(item.NoiseLayerThree.WindDirection);
    //     writer.Write(item.NoiseLayerOne.WindSpeed);
    //     writer.Write(item.NoiseLayerTwo.WindSpeed);
    //     writer.Write(item.NoiseLayerThree.WindSpeed);
    // }
    //
    // public static partial void WriteBinaryNoiseTextureParsingCustom(
    //     MutagenWriter writer,
    //     IWaterGetter item)
    // {
    //     if (item.NoiseLayerOne.Texture is { } oneTexture)
    //     {
    //         StringBinaryTranslation.Instance.Write(writer, oneTexture, RecordTypes.NAM2, StringBinaryType.NullTerminate);
    //     }
    //     if (item.NoiseLayerTwo.Texture is { } twoTexture)
    //     {
    //         StringBinaryTranslation.Instance.Write(writer, twoTexture, RecordTypes.NAM3, StringBinaryType.NullTerminate);
    //     }
    //     if (item.NoiseLayerThree.Texture is { } threeTexture)
    //     {
    //         StringBinaryTranslation.Instance.Write(writer, threeTexture, RecordTypes.NAM4, StringBinaryType.NullTerminate);
    //     }
    // }
}

partial class WaterBinaryOverlay
{
    // private SubrecordFrame? _nam2;
    // private SubrecordFrame? _nam3;
    // private SubrecordFrame? _nam4;
    // private int _dataLoc => _SpecularInteriorSpecularPowerLocation + 4;
    //
    // public IWaterNoisePropertiesGetter NoiseLayerOne => new WaterNoisePropertiesBinaryOverlay(_recordData.Slice(_dataLoc), _nam2, _package);
    //
    // public IWaterNoisePropertiesGetter NoiseLayerTwo => new WaterNoisePropertiesBinaryOverlay(_recordData.Slice(_dataLoc + 4), _nam3, _package);
    //
    // public IWaterNoisePropertiesGetter NoiseLayerThree => new WaterNoisePropertiesBinaryOverlay(_recordData.Slice(_dataLoc + 8), _nam4, _package);
    //
    // public partial ParseResult NoiseTextureParsingCustomParse(
    //     OverlayStream stream,
    //     int offset, 
    //     PreviousParse lastParsed)
    // {
    //     var rec = stream.ReadSubrecord();
    //     switch (rec.RecordTypeInt)
    //     {
    //         case RecordTypeInts.NAM2:
    //             _nam2 = rec;
    //             break;
    //         case RecordTypeInts.NAM3:
    //             _nam3 = rec;
    //             break;
    //         case RecordTypeInts.NAM4:
    //             _nam4 = rec;
    //             break;
    //         default:
    //             throw new MalformedDataException($"Unexpected record type: {rec.RecordType}");
    //     }
    //     return (int)Water_FieldIndex.ScreenSpaceReflections;
    // }
}
