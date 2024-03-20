using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class Region
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
        var rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out _);
        while (rdat.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(frame, item);
            if (frame.Complete) break;
            rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out _);
        }

        return null;
    }

    private static readonly RecordType[] WeatherTypes = 
    {
        RecordTypes.ICON,
        RecordTypes.RDWT,
        RecordTypes.ANAM,
    };

    private static readonly RecordType[] SoundTypes = 
    {
        RecordTypes.ICON,
    };
    
    public static RecordType[] GetTypes(RegionData.RegionDataType type) => type switch
    {
        RegionData.RegionDataType.Weather => WeatherTypes,
        RegionData.RegionDataType.Sound => SoundTypes,
        _ => throw new ArgumentException($"Unexpected type {type}", nameof(type))
    };

    static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
    {
        var rdatFrame = frame.Reader.GetSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);

        frame = frame.SpawnAll();
        
        switch (dataType)
        {
            case RegionData.RegionDataType.Weather:
                item.Weather = RegionWeather.CreateFromBinary(frame);
                break;
            case RegionData.RegionDataType.Sound:
                item.Sounds = RegionSounds.CreateFromBinary(frame);
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
        item.Weather?.WriteToBinary(writer);
        item.Sounds?.WriteToBinary(writer);
    }
}

partial class RegionBinaryOverlay
{
    private ReadOnlyMemorySlice<byte>? _weatherSpan;
    public IRegionWeatherGetter? Weather => _weatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new OverlayStream(_weatherSpan.Value, _package), _package) : default;

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

    private void ParseRegionData(OverlayStream stream, int offset)
    {
        int loc = stream.Position - offset; 
        var rdatFrame = stream.ReadSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);
        
        switch (dataType)
        {
            case RegionData.RegionDataType.Weather:
                _weatherSpan = _recordData.Slice(loc);
                break;
            case RegionData.RegionDataType.Sound:
                _soundsSpan = _recordData.Slice(loc);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}