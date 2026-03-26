using Mutagen.Bethesda.Fallout76.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout76;

partial class Weather
{
    [Flags]
    public enum MajorFlag
    {
    }
}

partial class WeatherBinaryCreateTranslation
{
    public static ParseResult CustomRecordFallback(
        IWeatherInternal item,
        MutagenFrame frame,
        PreviousParse lastParsed,
        Dictionary<RecordType, int>? recordParseCount,
        RecordType nextRecordType,
        int contentLength,
        TypedParseParams translationParams = default)
    {
        if (nextRecordType == RecordTypes.EDID)
        {
            return Fallout76MajorRecordBinaryCreateTranslation.FillBinaryRecordTypes(
                item: item,
                frame: frame,
                lastParsed: lastParsed,
                recordParseCount: recordParseCount,
                nextRecordType: nextRecordType,
                contentLength: contentLength,
                translationParams: translationParams);
        }
        // Skip unknown texture subrecords (e.g. :0TX, ;0TX, <0TX)
        // that can't be represented in XML attributes
        frame.ReadSubrecord();
        return default(int?);
    }
}

partial class WeatherBinaryOverlay
{
    private ParseResult CustomRecordFallback(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        TypedParseParams translationParams = default)
    {
        if (type == RecordTypes.EDID)
        {
            return base.FillRecordType(
                stream: stream,
                finalPos: finalPos,
                offset: offset,
                type: type,
                recordParseCount: null,
                lastParsed: lastParsed,
                translationParams: translationParams);
        }
        // Skip unknown texture subrecords (e.g. :0TX, ;0TX, <0TX)
        stream.ReadSubrecord();
        return default(int?);
    }
}
