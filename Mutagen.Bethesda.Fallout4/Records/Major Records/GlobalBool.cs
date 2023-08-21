using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Fallout4.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

public partial class GlobalBool
{
    public const char TRIGGER_CHAR = 'b';
    char IGlobalGetter.TypeChar => TRIGGER_CHAR;

    public override float? RawFloat
    {
        get => this.Data ?? default ? 1 : 0;
        set
        {
            if (value.HasValue)
            {
                this.Data = value.Value != 0;
            }
            else
            {
                this.Data = default;
            }
        }
    }
}

partial class GlobalBoolBinaryCreateTranslation
{
    public static partial void FillBinaryDataCustom(MutagenFrame frame, IGlobalBoolInternal item)
    {
    }
}

partial class GlobalBoolBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, IGlobalBoolGetter item)
    {
        if (item.Data is not {} data) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.FLTV))
        {
            writer.Write(data);
        }
    }
}

partial class GlobalBoolBinaryOverlay
{
    char IGlobalGetter.TypeChar => GlobalInt.TRIGGER_CHAR;
    public override float? RawFloat => this.Data is {} data ? (data ? 1 : 0) : default;

    private int? _DataLocation;
    public bool GetDataIsSetCustom() => _DataLocation.HasValue;
    public partial bool? GetDataCustom()
    {
        if (!_DataLocation.HasValue) return default;
        return HeaderTranslation
            .ExtractSubrecordMemory(_recordData, _DataLocation.Value, _package.MetaData.Constants).Float() != 0;
    }

    partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _DataLocation = (ushort)(stream.Position - offset);
    }
}
