using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4;

partial class ImageSpace
{
}

partial class ImageSpaceBinaryCreateTranslation
{
    public static partial void FillBinaryDepthOfFieldBlurRadiusCustom(MutagenFrame frame, IImageSpaceInternal item)
    {
        ParseSkyBlurRadius(frame.ReadUInt16(), out var radius, out var sky);
        item.DepthOfFieldBlurRadius = radius;
        item.DepthOfFieldSky = sky;
    }

    public static void ParseSkyBlurRadius(ushort val, out byte radius, out bool sky)
    {
        switch (val)
        {
            case 0x4000:
                radius = 0;
                sky = true;
                break;
            case 0x4120:
                radius = 1;
                sky = true;
                break;
            case 0x4190:
                radius = 2;
                sky = true;
                break;
            case 0x41D0:
                radius = 3;
                sky = true;
                break;
            case 0x4208:
                radius = 4;
                sky = true;
                break;
            case 0x4228:
                radius = 5;
                sky = true;
                break;
            case 0x4248:
                radius = 6;
                sky = true;
                break;
            case 0x4268:
                radius = 7;
                sky = true;
                break;
            case 0x40C0:
                radius = 0;
                sky = false;
                break;
            case 0x4160:
                radius = 1;
                sky = false;
                break;
            case 0x41B0:
                radius = 2;
                sky = false;
                break;
            case 0x41F0:
                radius = 3;
                sky = false;
                break;
            case 0x4218:
                radius = 4;
                sky = false;
                break;
            case 0x4238:
                radius = 5;
                sky = false;
                break;
            case 0x4258:
                radius = 6;
                sky = false;
                break;
            case 0x4278:
                radius = 7;
                sky = false;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static partial void FillBinaryDepthOfFieldSkyCustom(MutagenFrame frame, IImageSpaceInternal item)
    {
    }

    public static partial ParseResult FillBinaryENAMParsingCustom(MutagenFrame frame, IImageSpaceInternal item)
    {
        frame.ReadSubrecordHeader(RecordTypes.ENAM);
        item.HdrEyeAdaptSpeed = frame.ReadFloat();
        item.HdrTonemapE = frame.ReadFloat();
        item.HdrBloomThreshold = frame.ReadFloat();
        item.HdrBloomScale = frame.ReadFloat();
        item.HdrAutoExposureMin = frame.ReadFloat();
        item.HdrSunlightScale = frame.ReadFloat();
        item.HdrSkyScale = frame.ReadFloat();
        item.CinematicSaturation = frame.ReadFloat();
        item.CinematicBrightness = frame.ReadFloat();
        item.CinematicContrast = frame.ReadFloat();
        item.TintAmount = frame.ReadFloat();
        item.TintColor = frame.ReadColor(ColorBinaryType.NoAlphaFloat);
        return ParseResult.Stop;
    }
}

partial class ImageSpaceBinaryWriteTranslation
{
    public static partial void WriteBinaryDepthOfFieldBlurRadiusCustom(MutagenWriter writer, IImageSpaceGetter item)
    {
        writer.Write(GetBlurSkyValue(item));
    }

    public static ushort GetBlurSkyValue(IImageSpaceGetter item)
    {
        if (item.DepthOfFieldSky)
        {
            return item.DepthOfFieldBlurRadius switch
            {
                0 => 0x4000,
                1 => 0x4120,
                2 => 0x4190,
                3 => 0x41D0,
                4 => 0x4208,
                5 => 0x4228,
                6 => 0x4248,
                7 => 0x4268,
                _ => throw new NotImplementedException(),
            };
        }
        else
        {
            return item.DepthOfFieldBlurRadius switch
            {
                0 => 0x40C0,
                1 => 0x4160,
                2 => 0x41B0,
                3 => 0x41F0,
                4 => 0x4218,
                5 => 0x4238,
                6 => 0x4258,
                7 => 0x4278,
                _ => throw new NotImplementedException(),
            };
        }
    }

    public static partial void WriteBinaryDepthOfFieldSkyCustom(MutagenWriter writer, IImageSpaceGetter item)
    {
    }

    public static partial void WriteBinaryENAMParsingCustom(MutagenWriter writer, IImageSpaceGetter item)
    {
        if (writer.MetaData.FormVersion!.Value >= 16) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.ENAM))
        {
            writer.Write(item.HdrEyeAdaptSpeed);
            writer.Write(item.HdrTonemapE);
            writer.Write(item.HdrBloomThreshold);
            writer.Write(item.HdrBloomScale);
            writer.Write(item.HdrAutoExposureMin);
            writer.Write(item.HdrSunlightScale);
            writer.Write(item.HdrSkyScale);
            writer.Write(item.CinematicSaturation);
            writer.Write(item.CinematicBrightness);
            writer.Write(item.CinematicContrast);
            writer.Write(item.TintAmount);
            writer.Write(item.TintColor, ColorBinaryType.NoAlphaFloat);
        }
    }
}

partial class ImageSpaceBinaryOverlay
{
    public partial byte GetDepthOfFieldBlurRadiusCustom()
    {
        ImageSpaceBinaryCreateTranslation.ParseSkyBlurRadius(
            BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_DNAMLocation!.Value.Min + 0xE)),
            out var radius,
            out _);
        return radius;
    }

    public partial Boolean GetDepthOfFieldSkyCustom()
    {
        ImageSpaceBinaryCreateTranslation.ParseSkyBlurRadius(
            BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(_DNAMLocation!.Value.Min + 0xE)),
            out var _,
            out var sky);
        return sky;
    }

    public partial ParseResult ENAMParsingCustomParse(OverlayStream stream, int offset)
    {
        throw new NotImplementedException();
    }

    public static IImageSpaceGetter ImageSpaceFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams translationParams = default)
    {
        var header = stream.GetMajorRecordHeader();
        if (header.FormVersion < 16)
        {
            return ImageSpace.CreateFromBinary(new MutagenFrame(stream));
        }
        stream = Decompression.DecompressStream(stream);
        var ret = new ImageSpaceBinaryOverlay(
            bytes: HeaderTranslation.ExtractRecordMemory(stream.RemainingMemory, package.MetaData.Constants),
            package: package);
        var finalPos = checked((int)(stream.Position + stream.GetMajorRecordHeader().TotalLength));
        int offset = stream.Position + package.MetaData.Constants.MajorConstants.TypeAndLengthLength;
        ret._package.FormVersion = ret;
        stream.Position += 0x10 + package.MetaData.Constants.MajorConstants.TypeAndLengthLength;
        ret.CustomFactoryEnd(
            stream: stream,
            finalPos: finalPos,
            offset: offset);
        ret.FillSubrecordTypes(
            majorReference: ret,
            stream: stream,
            finalPos: finalPos,
            offset: offset,
            translationParams: translationParams,
            fill: ret.FillRecordType);
        return ret;
    }

}
