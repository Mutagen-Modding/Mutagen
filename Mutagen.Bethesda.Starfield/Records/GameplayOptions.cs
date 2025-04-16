using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

internal partial class AGameplayOptionsDataBinaryOverlay
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

internal partial class GameplayOptionsBinaryCreateTranslation
{
    public enum Type
    {
        Bool,
        Float
    }
    
    public static partial void FillBinaryDataCustom(
        MutagenFrame frame,
        IGameplayOptionsInternal item,
        PreviousParse lastParsed)
    {
        var type = frame.GetSubrecord(RecordTypes.TNAM);
        frame = frame.SpawnAll();
        switch ((Type)type.AsUInt8())
        {
            case Type.Bool:
                item.Data = BoolGameplayOptionsData.CreateFromBinary(frame);
                return;
            case Type.Float:
                item.Data = FloatGameplayOptionsData.CreateFromBinary(frame);
                return;
            default:
                    throw new NotImplementedException();
        }
    }
}

internal partial class GameplayOptionsBinaryOverlay
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
    }
    
    public partial IAGameplayOptionsDataGetter? GetDataCustom()
    {
        if (!_DataLocation.HasValue) return null;
        
        GameplayOptionsBinaryCreateTranslation.Type type = (GameplayOptionsBinaryCreateTranslation.Type)HeaderTranslation.ExtractSubrecordMemory(_recordData, _DataLocation.Value.Min, _package.MetaData.Constants)[0];
        switch (type)
        {
            case GameplayOptionsBinaryCreateTranslation.Type.Bool:
                return BoolGameplayOptionsDataBinaryOverlay.BoolGameplayOptionsDataFactory(
                    _recordData.Slice(_DataLocation.Value.Max), _package);
            case GameplayOptionsBinaryCreateTranslation.Type.Float:
                return FloatGameplayOptionsDataBinaryOverlay.FloatGameplayOptionsDataFactory(
                    _recordData.Slice(_DataLocation.Value.Max), _package);
            default:
                throw new NotImplementedException();
        }
    }
}


public partial class GameplayOptionsBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(
        MutagenWriter writer,
        IGameplayOptionsGetter item)
    {
        if (item.Data is not {} data) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.TNAM))
        {
            GameplayOptionsBinaryCreateTranslation.Type type;
            switch (data)
            {
                case IBoolGameplayOptionsDataGetter:
                    type = GameplayOptionsBinaryCreateTranslation.Type.Bool;
                    break;
                case IFloatGameplayOptionsDataGetter:
                    type = GameplayOptionsBinaryCreateTranslation.Type.Float;
                    break;
                default:
                    throw new NotImplementedException();
            }
        
            writer.Write((byte)type);
        }
        data.WriteToBinary(writer);
    }
}