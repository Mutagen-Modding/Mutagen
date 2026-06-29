using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class Water
{
    [Flags]
    public enum Flag
    {
        CausesDamage = 0x01,
        Reflective = 0x02,
    }
}

partial class WaterBinaryCreateTranslation
{
    public static WaterData ParseDataSubrecord(MutagenFrame frame, WaterData? current)
    {
        var subHeader = frame.ReadSubrecordHeader();
        if (subHeader.RecordTypeInt == RecordTypeInts.DNAM)
        {
            var visuals = WaterData.CreateFromBinary(frame.SpawnWithLength(subHeader.ContentLength));
            visuals.Damage = current?.Damage ?? 0;
            return visuals;
        }

        if (subHeader.ContentLength > 2)
        {
            // Legacy format
            var data = WaterData.CreateFromBinary(frame.SpawnWithLength(subHeader.ContentLength - 2));
            data.Damage = frame.ReadUInt16();
            return data;
        }

        current ??= new WaterData();
        current.Damage = frame.ReadUInt16();
        return current;
    }

    public static partial ParseResult FillBinaryDataParseCustom(MutagenFrame frame, IWaterInternal item, PreviousParse lastParsed)
    {
        item.Data = ParseDataSubrecord(frame, item.Data);
        return (int)Water_FieldIndex.Data;
    }
}

partial class WaterBinaryWriteTranslation
{
    public static partial void WriteBinaryDataParseCustom(MutagenWriter writer, IWaterGetter item)
    {
        var data = item.Data;
        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            writer.Write(data.Damage);
        }
        using (HeaderExport.Subrecord(writer, RecordTypes.DNAM))
        {
            data.WriteToBinary(writer);
        }
    }
}

partial class WaterDataBinaryOverlay
{
    public ushort Damage => 0;
}

partial class WaterBinaryOverlay
{
    private WaterData? _data;

    public IWaterDataGetter Data => _data ?? new WaterData();

    public partial ParseResult DataParseCustomParse(OverlayStream stream, int offset, PreviousParse lastParsed)
    {
        _data = WaterBinaryCreateTranslation.ParseDataSubrecord(
            new MutagenFrame(new MutagenInterfaceReadStream(stream, _package.MetaData)),
            _data);
        return (int)Water_FieldIndex.Data;
    }
}
