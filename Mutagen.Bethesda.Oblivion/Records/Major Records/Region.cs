using System.Buffers.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Oblivion;

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
            case RegionData.RegionDataType.Icon:
                if (!recordType.Equals(RecordTypes.ICON)) return false;
                break;
            case RegionData.RegionDataType.Grass:
                if (!recordType.Equals(RecordTypes.RDGS)) return false;
                break;
            case RegionData.RegionDataType.Sound:
                if (!recordType.Equals(RecordTypes.RDSD) && !recordType.Equals(RecordTypes.RDMD)) return false;
                break;
            default:
                return false;
        }
        return true;
    }

    static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
    {
        var rdatFrame = frame.GetSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
        var subMeta = frame.GetSubrecordHeader(offset: rdatFrame.TotalLength);
        int len = rdatFrame.TotalLength;
        if (IsExpected(dataType, subMeta.RecordType))
        {
            len += subMeta.TotalLength;
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
                if (frame.Reader.TryGetSubrecordHeader(out var nextRec, offset: len)
                    && (nextRec.RecordType.Equals(RecordTypes.RDSD) || nextRec.RecordType.Equals(RecordTypes.RDMD)))
                {
                    len += nextRec.TotalLength;
                }
                item.Sounds = RegionSounds.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Weather:
                item.Weather = RegionWeather.CreateFromBinary(frame.SpawnWithLength(len, checkFraming: false));
                break;
            case RegionData.RegionDataType.Icon:
                frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength + rdatFrame.TotalLength;
                len = len - frame.MetaData.Constants.SubConstants.HeaderLength - rdatFrame.TotalLength;
                if (StringBinaryTranslation.Instance.Parse(
                        frame.SpawnWithLength(len, checkFraming: false),
                        out var iconVal))
                {
                    item.Icon = iconVal;
                }
                else
                {
                    item.Icon = null;
                }
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
        item.MapName?.WriteToBinary(writer);
        item.Grasses?.WriteToBinary(writer);
        item.Sounds?.WriteToBinary(writer);
    }
}

partial class RegionBinaryOverlay : IRegionGetter
{
    #region Icon
    private int? _iconLocation;
    private int? _secondaryIconLocation;
    public partial string? GetIconCustom()
    {
        if (_iconLocation.HasValue)
        {
            return BinaryStringUtility.ProcessWholeToZString(HeaderTranslation.ExtractSubrecordMemory(_recordData, _iconLocation.Value, _package.MetaData.Constants), _package.MetaData.Encodings.NonLocalized);
        }
        if (_secondaryIconLocation.HasValue)
        {
            return BinaryStringUtility.ProcessWholeToZString(HeaderTranslation.ExtractSubrecordMemory(_recordData, _secondaryIconLocation.Value, _package.MetaData.Constants), _package.MetaData.Encodings.NonLocalized);
        }
        return default;
    }
    #endregion

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

    partial void IconCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _iconLocation = (ushort)(stream.Position - offset);
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
            if (RegionBinaryCreateTranslation.IsExpected(dataType, contentMeta.RecordType))
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
            case RegionData.RegionDataType.Sound:
                var nextRec = stream.GetSubrecordHeader();
                if (nextRec.RecordType.Equals(RecordTypes.RDSD) || nextRec.RecordType.Equals(RecordTypes.RDMD))
                {
                    len += nextRec.TotalLength;
                }
                _soundsSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Weather:
                _weatherSpan = _recordData.Slice(loc, len);
                break;
            case RegionData.RegionDataType.Icon:
                _secondaryIconLocation = loc + rdatFrame.TotalLength;
                break;
            default:
                throw new NotImplementedException();
        }
    }
}