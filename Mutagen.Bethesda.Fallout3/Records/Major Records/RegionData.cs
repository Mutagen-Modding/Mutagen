using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Fallout3;

public partial class RegionData
{
    public enum RegionDataType
    {
        Object = 2,
        Weather = 3,
        Map = 4,
        Land = 5,
        Grass = 6,
        Sound = 7,
        Imposter = 8,
    }

    public abstract RegionDataType DataType { get; }
}

partial interface IRegionDataGetter
{
    RegionData.RegionDataType DataType { get; }
}

partial class RegionDataBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryHeaderLogicCustom(MutagenFrame frame, IRegionData item, PreviousParse lastParsed)
    {
        frame.ReadSubrecordHeader(RecordTypes.RDAT);
        frame.Position += 4;
        item.Override = frame.ReadUInt8() != 0;
        item.Priority = frame.ReadUInt8();
        frame.Position += 2;
        return (int)RegionData_FieldIndex.Priority;
    }
}

partial class RegionDataBinaryWriteTranslation
{
    public static partial void WriteBinaryHeaderLogicCustom(MutagenWriter writer, IRegionDataGetter item)
    {
        using (HeaderExport.Subrecord(writer, RecordTypes.RDAT))
        {
            EnumBinaryTranslation<RegionData.RegionDataType, MutagenFrame, MutagenWriter>.Instance.Write(
                writer,
                item.DataType,
                length: 4);
            writer.Write((byte)(item.Override ? 1 : 0));
            writer.Write(item.Priority);
            writer.WriteZeros(2);
        }
    }
}

partial class RegionDataBinaryOverlay
{
    public abstract RegionData.RegionDataType DataType { get; }
    public Boolean Override => HeaderTranslation.ExtractSubrecordMemory(_recordData, _rdatLocation, _package.MetaData.Constants).Span[0x4] != 0;
    public Byte Priority => HeaderTranslation.ExtractSubrecordMemory(_recordData, _rdatLocation, _package.MetaData.Constants).Span[0x5];
    private int _rdatLocation;

    public partial ParseResult HeaderLogicCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _rdatLocation = (stream.Position - offset);
        return (int)RegionData_FieldIndex.Priority;
    }
}

// Concrete type overrides
public partial class RegionObjects
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Object;
}

partial class RegionObjectsBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Object;
}

public partial class RegionWeather
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Weather;
}

partial class RegionWeatherBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Weather;
}

public partial class RegionMap
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Map;
}

partial class RegionMapBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Map;
}

public partial class RegionLand
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Land;
}

partial class RegionLandBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Land;
}

public partial class RegionGrasses
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Grass;
}

partial class RegionGrassesBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Grass;
}

public partial class RegionSounds
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Sound;
}

partial class RegionSoundsBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Sound;
}

public partial class RegionImposters
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Imposter;
}

partial class RegionImpostersBinaryOverlay
{
    public override RegionData.RegionDataType DataType => RegionData.RegionDataType.Imposter;
}
