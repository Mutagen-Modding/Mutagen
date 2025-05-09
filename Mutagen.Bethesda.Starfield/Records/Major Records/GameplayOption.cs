using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

internal partial class AGameplayOptionDataBinaryOverlay
{
    public ParseResult FillRecordType(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed,
        Dictionary<RecordType, int>? recordParseCount,
        TypedParseParams translationParams = default)
    {
        throw new NotImplementedException();
    }
}

internal partial class GameplayOptionBinaryCreateTranslation
{
    public enum Type
    {
        Bool,
        Float
    }
    
    public static partial void FillBinaryDataCustom(
        MutagenFrame frame,
        IGameplayOptionInternal item,
        PreviousParse lastParsed)
    {
        var type = frame.GetSubrecord(RecordTypes.TNAM);
        frame = frame.SpawnAll();
        switch ((Type)type.AsUInt8())
        {
            case Type.Bool:
                item.Data = BoolGameplayOptionData.CreateFromBinary(frame);
                return;
            case Type.Float:
                item.Data = FloatGameplayOptionData.CreateFromBinary(frame);
                return;
            default:
                    throw new NotImplementedException();
        }
    }
}

internal partial class GameplayOptionBinaryOverlay
{
    private RangeInt32? _DataLocation;
    
    partial void DataCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _DataLocation = new RangeInt32((stream.Position - offset), finalPos - offset);
        stream.ReadSubrecord(RecordTypes.TNAM);
        stream.TryReadSubrecord(RecordTypes.VNAM, out _);
        stream.TryReadSubrecord(RecordTypes.WNAM, out _);
        stream.TryReadSubrecord(RecordTypes.GPOD, out _);
    }
    
    public partial IAGameplayOptionDataGetter? GetDataCustom()
    {
        if (!_DataLocation.HasValue) return null;
        
        GameplayOptionBinaryCreateTranslation.Type type = (GameplayOptionBinaryCreateTranslation.Type)HeaderTranslation.ExtractSubrecordMemory(_recordData, _DataLocation.Value.Min, _package.MetaData.Constants)[0];
        switch (type)
        {
            case GameplayOptionBinaryCreateTranslation.Type.Bool:
                return BoolGameplayOptionDataBinaryOverlay.BoolGameplayOptionDataFactory(
                    _recordData.Slice(_DataLocation.Value.Max), _package);
            case GameplayOptionBinaryCreateTranslation.Type.Float:
                return FloatGameplayOptionDataBinaryOverlay.FloatGameplayOptionDataFactory(
                    _recordData.Slice(_DataLocation.Value.Max), _package);
            default:
                throw new NotImplementedException();
        }
    }
}


public partial class GameplayOptionBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(
        MutagenWriter writer,
        IGameplayOptionGetter item)
    {
        if (item.Data is not {} data) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.TNAM))
        {
            GameplayOptionBinaryCreateTranslation.Type type;
            switch (data)
            {
                case IBoolGameplayOptionDataGetter:
                    type = GameplayOptionBinaryCreateTranslation.Type.Bool;
                    break;
                case IFloatGameplayOptionDataGetter:
                    type = GameplayOptionBinaryCreateTranslation.Type.Float;
                    break;
                default:
                    throw new NotImplementedException();
            }
        
            writer.Write((byte)type);
        }
        data.WriteToBinary(writer);
    }
}