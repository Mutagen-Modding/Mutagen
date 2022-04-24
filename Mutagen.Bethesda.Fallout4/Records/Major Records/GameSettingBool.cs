using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout4;

public partial class GameSettingBool
{
    public override GameSettingType SettingType => GameSettingType.Bool;
}

partial class GameSettingBoolBinaryCreateTranslation
{
    public static partial void FillBinaryDataCustom(MutagenFrame frame, IGameSettingBoolInternal item)
    {
        var subFrame = frame.ReadSubrecord();
        item.Data = (bool)(BinaryPrimitives.ReadUInt32LittleEndian(subFrame.Content) != 0);
    }
}

partial class GameSettingBoolBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, IGameSettingBoolGetter item)
    {
        if (item.Data is not { } data) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            writer.Write(data ? 1 : 0);
        }
    }
}

partial class GameSettingBoolBinaryOverlay
{
    private int? _DataLocation;
    bool GetDataIsSetCustom() => _DataLocation.HasValue;
    public partial bool? GetDataCustom() => _DataLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_data, _DataLocation.Value, _package.MetaData.Constants)) != 0 : default;
    partial void DataCustomParse(OverlayStream stream, long finalPos, int offset)
    {
        _DataLocation = (ushort)(stream.Position - offset);
    }
}