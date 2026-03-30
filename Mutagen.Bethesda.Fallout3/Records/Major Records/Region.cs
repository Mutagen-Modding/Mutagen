using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

partial class RegionBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryRegionAreaLogicCustom(MutagenFrame frame, IRegionInternal item, PreviousParse lastParsed)
    {
        var rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out _);
        while (rdat.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(frame, item);
            if (frame.Complete) break;
            rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out _);
        }

        return null;
    }

    public static bool IsExpected(RegionData.RegionDataType dataType, RecordType recordType)
    {
        return dataType switch
        {
            RegionData.RegionDataType.Object => recordType.Equals(RecordTypes.RDOT),
            RegionData.RegionDataType.Weather => recordType.Equals(RecordTypes.RDWT),
            RegionData.RegionDataType.Map => recordType.Equals(RecordTypes.RDMP),
            RegionData.RegionDataType.Grass => recordType.Equals(RecordTypes.RDGS),
            RegionData.RegionDataType.Sound => recordType.Equals(RecordTypes.RDSD)
                || recordType.Equals(RecordTypes.RDMD)
                || recordType.Equals(RecordTypes.RDMO)
                || recordType.Equals(RecordTypes.RDSI)
                || recordType.Equals(RecordTypes.RDSB),
            RegionData.RegionDataType.Imposter => recordType.Equals(RecordTypes.RDID),
            _ => false,
        };
    }

    static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
    {
        var rdatFrame = frame.GetSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
        int len = rdatFrame.TotalLength;
        var spawn = frame.SpawnAll();

        // Accumulate all following subrecords that belong to this RDAT entry
        while (spawn.Remaining > len)
        {
            var nextRec = frame.GetSubrecordHeader(offset: len);
            if (!IsExpected(dataType, nextRec.RecordType)) break;
            len += nextRec.TotalLength;
        }

        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                item.Objects = RegionObjects.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Map:
                item.MapName = RegionMap.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Grass:
                item.Grasses = RegionGrasses.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Sound:
                item.Sounds = RegionSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Weather:
                item.Weather = RegionWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Imposter:
                item.Imposters = RegionImposters.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            default:
                // Skip unknown region data types
                frame.Position += len;
                break;
        }
    }
}

partial class RegionBinaryWriteTranslation
{
    public static partial void WriteBinaryRegionAreaLogicCustom(MutagenWriter writer, IRegionGetter item)
    {
        item.Objects?.WriteToBinary(writer);
        item.Weather?.WriteToBinary(writer);
        item.MapName?.WriteToBinary(writer);
        item.Grasses?.WriteToBinary(writer);
        item.Sounds?.WriteToBinary(writer);
        item.Imposters?.WriteToBinary(writer);
    }
}

partial class RegionBinaryOverlay : IRegionGetter
{
    private ReadOnlyMemorySlice<byte>? _objectsSpan;
    public IRegionObjectsGetter? Objects => _objectsSpan.HasValue ? RegionObjectsBinaryOverlay.RegionObjectsFactory(new OverlayStream(_objectsSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _weatherSpan;
    public IRegionWeatherGetter? Weather => _weatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new OverlayStream(_weatherSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _mapSpan;
    public IRegionMapGetter? MapName => _mapSpan.HasValue ? RegionMapBinaryOverlay.RegionMapFactory(new OverlayStream(_mapSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _grassesSpan;
    public IRegionGrassesGetter? Grasses => _grassesSpan.HasValue ? RegionGrassesBinaryOverlay.RegionGrassesFactory(new OverlayStream(_grassesSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _soundsSpan;
    public IRegionSoundsGetter? Sounds => _soundsSpan.HasValue ? RegionSoundsBinaryOverlay.RegionSoundsFactory(new OverlayStream(_soundsSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _impostersSpan;
    public IRegionImpostersGetter? Imposters => _impostersSpan.HasValue ? RegionImpostersBinaryOverlay.RegionImpostersFactory(new OverlayStream(_impostersSpan.Value, _package), _package) : default;

    public partial ParseResult RegionAreaLogicCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        var rdat = stream.GetSubrecordHeader();
        while (rdat.RecordType.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(stream, offset);
            if (stream.Complete) break;
            rdat = stream.GetSubrecordHeader();
        }

        return null;
    }

    private void ParseRegionData(OverlayStream stream, int offset)
    {
        int loc = stream.Position - offset;
        var rdatFrame = stream.ReadSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
        var len = rdatFrame.TotalLength;

        // Accumulate all following subrecords that belong to this RDAT entry
        while (!stream.Complete)
        {
            var contentMeta = stream.GetSubrecordHeader();
            if (!RegionBinaryCreateTranslation.IsExpected(dataType, contentMeta.RecordType)) break;
            len += contentMeta.TotalLength;
            stream.Position += contentMeta.TotalLength;
        }

        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                _objectsSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Map:
                _mapSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Grass:
                _grassesSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Sound:
                _soundsSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Weather:
                _weatherSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Imposter:
                _impostersSpan = _recordData.Slice(loc, len);
                break;
            default:
                // Skip unknown region data types
                break;
        }
    }
}
