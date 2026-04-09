using System.Drawing;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class Weather
{
    public enum Classification
    {
        None = 0,
        Pleasant = 1,
        Cloudy = 2,
        Rainy = 4,
        Snow = 8,
    }

    internal const int CloudLayerCount = 4;
    internal const int TypedColorCount = 10;
    internal const int ColorBytes = 4;
    internal const int ColorSetFo3ByteLength = 4 * ColorBytes;  // 16
    internal const int ColorSetFnvByteLength = 6 * ColorBytes;  // 24
    internal const int PnamFo3ByteLength = CloudLayerCount * ColorSetFo3ByteLength;  // 64
    internal const int PnamFnvByteLength = CloudLayerCount * ColorSetFnvByteLength;  // 96
    internal const int Nam0Fo3ByteLength = TypedColorCount * ColorSetFo3ByteLength;  // 160
    internal const int Nam0FnvByteLength = TypedColorCount * ColorSetFnvByteLength;  // 240
}

partial class WeatherBinaryCreateTranslation
{
    private static WeatherColorSet ParseColorSet(MutagenFrame frame, bool fnvShape)
    {
        var set = new WeatherColorSet
        {
            Sunrise = frame.ReadColor(ColorBinaryType.Alpha),
            Day = frame.ReadColor(ColorBinaryType.Alpha),
            Sunset = frame.ReadColor(ColorBinaryType.Alpha),
            Night = frame.ReadColor(ColorBinaryType.Alpha),
        };
        if (fnvShape)
        {
            set.HighNoon = frame.ReadColor(ColorBinaryType.Alpha);
            set.Midnight = frame.ReadColor(ColorBinaryType.Alpha);
        }
        return set;
    }

    public static partial void FillBinaryCloudLayerColorsCustom(
        MutagenFrame frame,
        IWeatherInternal item,
        PreviousParse lastParsed)
    {
        var subHeader = frame.ReadSubrecordHeader(RecordTypes.PNAM);
        var dataFrame = frame.SpawnWithLength(subHeader.ContentLength);
        var fnvShape = subHeader.ContentLength == Weather.PnamFnvByteLength;
        item.CloudLayerColors = new WeatherCloudLayerColors
        {
            Layer0 = ParseColorSet(dataFrame, fnvShape),
            Layer1 = ParseColorSet(dataFrame, fnvShape),
            Layer2 = ParseColorSet(dataFrame, fnvShape),
            Layer3 = ParseColorSet(dataFrame, fnvShape),
        };
    }

    public static partial void FillBinaryColorsCustom(
        MutagenFrame frame,
        IWeatherInternal item,
        PreviousParse lastParsed)
    {
        var subHeader = frame.ReadSubrecordHeader(RecordTypes.NAM0);
        var dataFrame = frame.SpawnWithLength(subHeader.ContentLength);
        var fnvShape = subHeader.ContentLength == Weather.Nam0FnvByteLength;
        item.Colors = new WeatherColors
        {
            SkyUpper = ParseColorSet(dataFrame, fnvShape),
            Fog = ParseColorSet(dataFrame, fnvShape),
            UnusedA = ParseColorSet(dataFrame, fnvShape),
            Ambient = ParseColorSet(dataFrame, fnvShape),
            Sunlight = ParseColorSet(dataFrame, fnvShape),
            Sun = ParseColorSet(dataFrame, fnvShape),
            Stars = ParseColorSet(dataFrame, fnvShape),
            SkyLower = ParseColorSet(dataFrame, fnvShape),
            Horizon = ParseColorSet(dataFrame, fnvShape),
            UnusedB = ParseColorSet(dataFrame, fnvShape),
        };
    }
}

partial class WeatherBinaryWriteTranslation
{
    private static bool IsFnv(MutagenWriter writer)
        => writer.MetaData.ModHeaderVersion!.Value >= 1.32f;

    private static void WriteColorSet(MutagenWriter writer, IWeatherColorSetGetter? set, bool fnvShape)
    {
        writer.Write(set?.Sunrise ?? default, ColorBinaryType.Alpha);
        writer.Write(set?.Day ?? default, ColorBinaryType.Alpha);
        writer.Write(set?.Sunset ?? default, ColorBinaryType.Alpha);
        writer.Write(set?.Night ?? default, ColorBinaryType.Alpha);
        if (fnvShape)
        {
            writer.Write(set?.HighNoon ?? default, ColorBinaryType.Alpha);
            writer.Write(set?.Midnight ?? default, ColorBinaryType.Alpha);
        }
    }

    public static partial void WriteBinaryCloudLayerColorsCustom(
        MutagenWriter writer,
        IWeatherGetter item)
    {
        if (item.CloudLayerColors is not { } colors) return;
        var fnvShape = IsFnv(writer);
        using (HeaderExport.Subrecord(writer, RecordTypes.PNAM))
        {
            WriteColorSet(writer, colors.Layer0, fnvShape);
            WriteColorSet(writer, colors.Layer1, fnvShape);
            WriteColorSet(writer, colors.Layer2, fnvShape);
            WriteColorSet(writer, colors.Layer3, fnvShape);
        }
    }

    public static partial void WriteBinaryColorsCustom(
        MutagenWriter writer,
        IWeatherGetter item)
    {
        if (item.Colors is not { } colors) return;
        var fnvShape = IsFnv(writer);
        using (HeaderExport.Subrecord(writer, RecordTypes.NAM0))
        {
            WriteColorSet(writer, colors.SkyUpper, fnvShape);
            WriteColorSet(writer, colors.Fog, fnvShape);
            WriteColorSet(writer, colors.UnusedA, fnvShape);
            WriteColorSet(writer, colors.Ambient, fnvShape);
            WriteColorSet(writer, colors.Sunlight, fnvShape);
            WriteColorSet(writer, colors.Sun, fnvShape);
            WriteColorSet(writer, colors.Stars, fnvShape);
            WriteColorSet(writer, colors.SkyLower, fnvShape);
            WriteColorSet(writer, colors.Horizon, fnvShape);
            WriteColorSet(writer, colors.UnusedB, fnvShape);
        }
    }
}

partial class WeatherBinaryOverlay
{
    private IWeatherCloudLayerColorsGetter? _CloudLayerColors;
    private IWeatherColorsGetter? _Colors;

    private static WeatherColorSet ParseColorSet(ReadOnlySpan<byte> bytes, bool fnvShape)
    {
        var set = new WeatherColorSet
        {
            Sunrise = bytes.Slice(0, 4).ReadColor(ColorBinaryType.Alpha),
            Day = bytes.Slice(4, 4).ReadColor(ColorBinaryType.Alpha),
            Sunset = bytes.Slice(8, 4).ReadColor(ColorBinaryType.Alpha),
            Night = bytes.Slice(12, 4).ReadColor(ColorBinaryType.Alpha),
        };
        if (fnvShape)
        {
            set.HighNoon = bytes.Slice(16, 4).ReadColor(ColorBinaryType.Alpha);
            set.Midnight = bytes.Slice(20, 4).ReadColor(ColorBinaryType.Alpha);
        }
        return set;
    }

    partial void CloudLayerColorsCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        var sub = stream.ReadSubrecord(RecordTypes.PNAM);
        var fnvShape = sub.Content.Length == Weather.PnamFnvByteLength;
        var slotSize = fnvShape ? Weather.ColorSetFnvByteLength : Weather.ColorSetFo3ByteLength;
        var content = sub.Content.Span;
        _CloudLayerColors = new WeatherCloudLayerColors
        {
            Layer0 = ParseColorSet(content.Slice(0 * slotSize, slotSize), fnvShape),
            Layer1 = ParseColorSet(content.Slice(1 * slotSize, slotSize), fnvShape),
            Layer2 = ParseColorSet(content.Slice(2 * slotSize, slotSize), fnvShape),
            Layer3 = ParseColorSet(content.Slice(3 * slotSize, slotSize), fnvShape),
        };
    }

    public partial IWeatherCloudLayerColorsGetter? GetCloudLayerColorsCustom() => _CloudLayerColors;

    partial void ColorsCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        var sub = stream.ReadSubrecord(RecordTypes.NAM0);
        var fnvShape = sub.Content.Length == Weather.Nam0FnvByteLength;
        var slotSize = fnvShape ? Weather.ColorSetFnvByteLength : Weather.ColorSetFo3ByteLength;
        var content = sub.Content.Span;
        _Colors = new WeatherColors
        {
            SkyUpper = ParseColorSet(content.Slice(0 * slotSize, slotSize), fnvShape),
            Fog = ParseColorSet(content.Slice(1 * slotSize, slotSize), fnvShape),
            UnusedA = ParseColorSet(content.Slice(2 * slotSize, slotSize), fnvShape),
            Ambient = ParseColorSet(content.Slice(3 * slotSize, slotSize), fnvShape),
            Sunlight = ParseColorSet(content.Slice(4 * slotSize, slotSize), fnvShape),
            Sun = ParseColorSet(content.Slice(5 * slotSize, slotSize), fnvShape),
            Stars = ParseColorSet(content.Slice(6 * slotSize, slotSize), fnvShape),
            SkyLower = ParseColorSet(content.Slice(7 * slotSize, slotSize), fnvShape),
            Horizon = ParseColorSet(content.Slice(8 * slotSize, slotSize), fnvShape),
            UnusedB = ParseColorSet(content.Slice(9 * slotSize, slotSize), fnvShape),
        };
    }

    public partial IWeatherColorsGetter? GetColorsCustom() => _Colors;
}
