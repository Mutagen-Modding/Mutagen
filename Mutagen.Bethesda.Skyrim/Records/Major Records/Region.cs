using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class Region
{
    [Flags]
    public enum MajorFlag
    {
        BorderRegion = 0x40
    }
}

partial class RegionBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryRegionAreaLogicCustom(MutagenFrame frame, IRegionInternal item, PreviousParse lastParsed)
    {
        var rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out var rdatType);
        while (rdat.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(frame, item);
            if (frame.Complete) break;
            rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out rdatType);
        }

        return null;
    }

    public static bool IsExpected(RegionData.RegionDataType dataType, RecordType recordType)
    {
        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                if (!recordType.Equals(RecordTypes.RDOT)) return false;
                break;
            case RegionData.RegionDataType.Weather:
                if (!recordType.Equals(RecordTypes.RDWT)) return false;
                break;
            case RegionData.RegionDataType.Map:
                if (!recordType.Equals(RecordTypes.RDMP)) return false;
                break;
            case RegionData.RegionDataType.Grass:
                if (!recordType.Equals(RecordTypes.RDGS)) return false;
                break;
            case RegionData.RegionDataType.Sound:
                if (!recordType.Equals(RecordTypes.RDSA) && !recordType.Equals(RecordTypes.RDMO)) return false;
                break;
            case RegionData.RegionDataType.Land:
            default:
                return false;
        }
        return true;
    }

    static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
    {
        var rdatFrame = frame.Reader.GetSubrecord();
        int len = rdatFrame.TotalLength;
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);

        if (frame.Reader.TryGetSubrecordHeader(out var subMeta, offset: len))
        {
            var recType = subMeta.RecordType;
            if (recType == RecordTypes.ICON)
            {
                len += subMeta.TotalLength;
                // Skip icon subrecord for now
                subMeta = frame.Reader.GetSubrecordHeader(offset: rdatFrame.TotalLength + subMeta.TotalLength);
            }
            if (IsExpected(dataType, recType))
            {
                len += subMeta.TotalLength;
            }
        }
                
        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                item.Objects = RegionObjects.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Map:
                item.Map = RegionMap.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Grass:
                item.Grasses = RegionGrasses.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Sound:
                if (frame.Reader.TryGetSubrecordHeader(out var nextRec, offset: len)
                    && (nextRec.RecordType.Equals(RecordTypes.RDSA) || nextRec.RecordType.Equals(RecordTypes.RDMO)))
                {
                    len += nextRec.TotalLength;
                }
                item.Sounds = RegionSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Weather:
                item.Weather = RegionWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Land:
                item.Land = RegionLand.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class RegionBinaryWriteTranslation
{
    public static partial void WriteBinaryRegionAreaLogicCustom(MutagenWriter writer, IRegionGetter item)
    {
        item.Objects?.WriteToBinary(writer);
        item.Weather?.WriteToBinary(writer);
        item.Map?.WriteToBinary(writer);
        item.Land?.WriteToBinary(writer);
        item.Grasses?.WriteToBinary(writer);
        item.Sounds?.WriteToBinary(writer);
    }
}

partial class RegionBinaryOverlay
{
    private ReadOnlyMemorySlice<byte>? _objectsSpan;
    public IRegionObjectsGetter? Objects => _objectsSpan.HasValue ? RegionObjectsBinaryOverlay.RegionObjectsFactory(new OverlayStream(_objectsSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _weatherSpan;
    public IRegionWeatherGetter? Weather => _weatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new OverlayStream(_weatherSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _mapSpan;
    public IRegionMapGetter? Map => _mapSpan.HasValue ? RegionMapBinaryOverlay.RegionMapFactory(new OverlayStream(_mapSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _grassesSpan;
    public IRegionGrassesGetter? Grasses => _grassesSpan.HasValue ? RegionGrassesBinaryOverlay.RegionGrassesFactory(new OverlayStream(_grassesSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _soundsSpan;
    public IRegionSoundsGetter? Sounds => _soundsSpan.HasValue ? RegionSoundsBinaryOverlay.RegionSoundsFactory(new OverlayStream(_soundsSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _landSpan;
    public IRegionLandGetter? Land => _landSpan.HasValue ? RegionLandBinaryOverlay.RegionLandFactory(new OverlayStream(_landSpan.Value, _package), _package) : default;

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
        if (!stream.Complete)
        {
            var contentMeta = stream.GetSubrecordHeader();
            var recType = contentMeta.RecordType;
            if (recType == RecordTypes.ICON)
            {
                var totalLen = contentMeta.TotalLength;
                len += totalLen;
                // Skip icon subrecord for now
                contentMeta = stream.GetSubrecordHeader(offset: rdatFrame.TotalLength + totalLen);
                stream.ReadSubrecord();
            }
            else if (RegionBinaryCreateTranslation.IsExpected(dataType, contentMeta.RecordType))
            {
                len += contentMeta.TotalLength;
                stream.Position += contentMeta.TotalLength;
            }
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
            case RegionData.RegionDataType.Land:
                _landSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Sound:
                if (stream.TryGetSubrecordHeader(out var nextRec)
                    && (nextRec.RecordType.Equals(RecordTypes.RDSA)
                        || nextRec.RecordType.Equals(RecordTypes.RDMO)))
                {
                    len += nextRec.TotalLength;
                    stream.Position += nextRec.TotalLength;
                }
                _soundsSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Weather:
                _weatherSpan = _recordData.Slice(loc, len);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}