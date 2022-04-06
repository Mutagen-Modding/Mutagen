using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout4.Internals;

namespace Mutagen.Bethesda.Fallout4;

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
    public static readonly RecordType RDOT = new("RDOT");
    public static readonly RecordType RDWT = new("RDWT");
    public static readonly RecordType RDMP = new("RDMP");
    public static readonly RecordType RDGS = new("RDGS");
    public static readonly RecordType RDSA = new("RDSA");
    public static readonly RecordType RDMO = new("RDMO");

    public static partial ParseResult FillBinaryRegionAreaLogicCustom(MutagenFrame frame, IRegionInternal item)
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
                if (!recordType.Equals(RDOT)) return false;
                break;
            case RegionData.RegionDataType.Weather:
                if (!recordType.Equals(RDWT)) return false;
                break;
            case RegionData.RegionDataType.Map:
                if (!recordType.Equals(RDMP)) return false;
                break;
            case RegionData.RegionDataType.Grass:
                if (!recordType.Equals(RDGS)) return false;
                break;
            case RegionData.RegionDataType.Sound:
                if (!recordType.Equals(RDSA) && !recordType.Equals(RDMO)) return false;
                break;
            case RegionData.RegionDataType.Land:
            default:
                return false;
        }
        return true;
    }

    static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
    {
        var rdatFrame = frame.Reader.GetSubrecordFrame();
        int len = rdatFrame.TotalLength;
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);

        if (frame.Reader.TryGetSubrecord(out var subMeta, offset: len))
        {
            var recType = subMeta.RecordType;
            if (recType == RecordTypes.ICON)
            {
                len += subMeta.TotalLength;
                // Skip icon subrecord for now
                subMeta = frame.Reader.GetSubrecord(offset: rdatFrame.TotalLength + subMeta.TotalLength);
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
                if (frame.Reader.TryGetSubrecord(out var nextRec, offset: len)
                    && (nextRec.RecordType.Equals(RDSA) || nextRec.RecordType.Equals(RDMO)))
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

partial class RegionBinaryOverlay : IRegionGetter
{
    private ReadOnlyMemorySlice<byte>? _ObjectsSpan;
    public IRegionObjectsGetter? Objects => _ObjectsSpan.HasValue ? RegionObjectsBinaryOverlay.RegionObjectsFactory(new OverlayStream(_ObjectsSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _WeatherSpan;
    public IRegionWeatherGetter? Weather => _WeatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new OverlayStream(_WeatherSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _MapSpan;
    public IRegionMapGetter? Map => _MapSpan.HasValue ? RegionMapBinaryOverlay.RegionMapFactory(new OverlayStream(_MapSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _GrassesSpan;
    public IRegionGrassesGetter? Grasses => _GrassesSpan.HasValue ? RegionGrassesBinaryOverlay.RegionGrassesFactory(new OverlayStream(_GrassesSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _SoundsSpan;
    public IRegionSoundsGetter? Sounds => _SoundsSpan.HasValue ? RegionSoundsBinaryOverlay.RegionSoundsFactory(new OverlayStream(_SoundsSpan.Value, _package), _package) : default;

    private ReadOnlyMemorySlice<byte>? _LandSpan;
    public IRegionLandGetter? Land => _LandSpan.HasValue ? RegionLandBinaryOverlay.RegionLandFactory(new OverlayStream(_LandSpan.Value, _package), _package) : default;

    public partial ParseResult RegionAreaLogicCustomParse(
        OverlayStream stream,
        int offset)
    {
        var rdat = stream.GetSubrecord();
        while (rdat.RecordType.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(stream, offset);
            if (stream.Complete) break;
            rdat = stream.GetSubrecord();
        }

        return null;
    }

    private void ParseRegionData(OverlayStream stream, int offset)
    {
        int loc = stream.Position - offset;
        var rdatFrame = stream.ReadSubrecordFrame();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
        var len = rdatFrame.TotalLength;
        if (!stream.Complete)
        {
            var contentMeta = stream.GetSubrecord();
            var recType = contentMeta.RecordType;
            if (recType == RecordTypes.ICON)
            {
                var totalLen = contentMeta.TotalLength;
                len += totalLen;
                // Skip icon subrecord for now
                contentMeta = stream.GetSubrecord(offset: rdatFrame.TotalLength + totalLen);
            }
            if (RegionBinaryCreateTranslation.IsExpected(dataType, contentMeta.RecordType))
            {
                len += contentMeta.TotalLength;
                stream.Position += contentMeta.TotalLength;
            }
        }
        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                _ObjectsSpan = this._data.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Map:
                _MapSpan = this._data.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Grass:
                _GrassesSpan = this._data.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Land:
                _LandSpan = this._data.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Sound:
                if (stream.TryGetSubrecord(out var nextRec)
                    && (nextRec.RecordType.Equals(RegionBinaryCreateTranslation.RDSA)
                        || nextRec.RecordType.Equals(RegionBinaryCreateTranslation.RDMO)))
                {
                    len += nextRec.TotalLength;
                }
                _SoundsSpan = this._data.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Weather:
                _WeatherSpan = this._data.Slice(loc, len);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}