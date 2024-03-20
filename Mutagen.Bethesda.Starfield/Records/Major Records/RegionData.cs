using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Starfield;

partial class RegionData
{
    [Flags]
    public enum RegionDataFlag
    {
        Override = 0x01
    }

    public enum RegionDataType
    {
        Weather = 3,
        Sound = 7,
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
        item.Flags = EnumBinaryTranslation<RegionData.RegionDataFlag, MutagenFrame, MutagenWriter>.Instance.Parse(
            reader: frame,
            length: 1);
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
            EnumBinaryTranslation<RegionData.RegionDataFlag, MutagenFrame, MutagenWriter>.Instance.Write(
                writer,
                item.Flags,
                length: 1);
            writer.Write(item.Priority);
            writer.WriteZeros(2);
        }
    }
}

partial class RegionDataBinaryOverlay
{
    public abstract RegionData.RegionDataType DataType { get; }
    public RegionData.RegionDataFlag Flags => (RegionData.RegionDataFlag)HeaderTranslation.ExtractSubrecordMemory(_recordData, _rdatLocation, _package.MetaData.Constants).Slice(0x4, 0x1)[0];
    public Byte Priority => HeaderTranslation.ExtractSubrecordMemory(_recordData, _rdatLocation, _package.MetaData.Constants).Span[0x5];
    private int _rdatLocation;

    public partial ParseResult HeaderLogicCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _rdatLocation = (stream.Position - offset);
        return (int)RegionData_FieldIndex.Priority;
    }
}