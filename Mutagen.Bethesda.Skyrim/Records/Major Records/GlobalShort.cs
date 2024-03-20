using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class GlobalShort
{
    public const char TRIGGER_CHAR = 's';
    char IGlobalGetter.TypeChar => TRIGGER_CHAR;

    public override float? RawFloat
    {
        get => this.Data is {} data ? (float)data : default;
        set
        {
            if (value.HasValue)
            {
                this.Data = (short)value.Value;
            }
            else
            {
                this.Data = default;
            }
        }
    }
}

partial class GlobalShortBinaryCreateTranslation
{
    public static partial void FillBinaryDataCustom(MutagenFrame frame, IGlobalShortInternal item, PreviousParse lastParsed)
    {
    }
}

partial class GlobalShortBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalShortGetter item)
    {
        if (item.Data is not {} data) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
        {
            writer.Write((float)data);
        }
    }
}

partial class GlobalShortBinaryOverlay
{
    char IGlobalGetter.TypeChar => GlobalShort.TRIGGER_CHAR;
    public override float? RawFloat => this.Data.HasValue ? (float)this.Data : default;

    private int? _DataLocation;
    public bool GetDataIsSetCustom() => _DataLocation.HasValue;
    public partial short? GetDataCustom()
    {
        if (!_DataLocation.HasValue) return default;
        return (short)HeaderTranslation.ExtractSubrecordMemory(_recordData, _DataLocation.Value, _package.MetaData.Constants).Float();
    }
    partial void DataCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _DataLocation = (ushort)(stream.Position - offset);
    }
}