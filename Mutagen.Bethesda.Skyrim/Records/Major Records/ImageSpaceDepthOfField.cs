using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Skyrim;

partial class ImageSpaceDepthOfFieldBinaryCreateTranslation
{
    public static partial void FillBinaryBlurRadiusCustom(MutagenFrame frame, IImageSpaceDepthOfField item)
    {
        ParseSkyBlurRadius(frame.ReadUInt16(), out var radius, out var sky);
        item.BlurRadius = radius;
        item.Sky = sky;
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

    public static partial void FillBinarySkyCustom(MutagenFrame frame, IImageSpaceDepthOfField item)
    {
    }
}

partial class ImageSpaceDepthOfFieldBinaryWriteTranslation
{
    public static partial void WriteBinaryBlurRadiusCustom(MutagenWriter writer, IImageSpaceDepthOfFieldGetter item)
    {
        writer.Write(GetBlurSkyValue(item));
    }

    public static ushort GetBlurSkyValue(IImageSpaceDepthOfFieldGetter item)
    {
        if (item.Sky)
        {
            return item.BlurRadius switch
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
            return item.BlurRadius switch
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

    public static partial void WriteBinarySkyCustom(MutagenWriter writer, IImageSpaceDepthOfFieldGetter item)
    {
    }
}

partial class ImageSpaceDepthOfFieldBinaryOverlay
{
    byte _radius;
    public partial Byte GetBlurRadiusCustom(int location) => _radius;

    bool _sky;
    public partial Boolean GetSkyCustom(int location) => _sky;

    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        if (this.Versioning.HasFlag(ImageSpaceDepthOfField.VersioningBreaks.Break0)) return;
        ImageSpaceDepthOfFieldBinaryCreateTranslation.ParseSkyBlurRadius(
            BinaryPrimitives.ReadUInt16LittleEndian(_data.Slice(0xE)),
            out _radius,
            out _sky);
    }
}