using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Starfield.Internals;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class BoneModifierBinaryCreateTranslation
{
    public enum DataType
    {
        LookAtChain,
        MorphDriver,
        PoseDeformer,
        SpringBone,
    }
    
    public static partial void FillBinaryDataCustom(
        MutagenFrame frame,
        IBoneModifierInternal item,
        PreviousParse lastParsed)
    {
        frame.ReadSubrecordHeader(RecordTypes.DATA);
        var dataTypeStr = StringBinaryTranslation.Instance.Parse(frame, StringBinaryType.PrependLengthWithNullIfContent);
        var dataType = Enum.Parse<DataType>(dataTypeStr);
        ABoneModifierData data;
        switch (dataType)
        {
            case DataType.LookAtChain:
                data = BoneModifierLookAtChainData.CreateFromBinary(frame);
                break;
            case DataType.MorphDriver:
                data = BoneModifierMorphDriverData.CreateFromBinary(frame);
                break;
            case DataType.PoseDeformer:
                data = BoneModifierPoseDeformerData.CreateFromBinary(frame);
                break;
            case DataType.SpringBone:
                data = BoneModifierSpringBoneData.CreateFromBinary(frame);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        item.Data = data;
    }
}
partial class BoneModifierBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(
        MutagenWriter writer,
        IBoneModifierGetter item)
    {
        var data = item.Data;
        if (data == null) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            BoneModifierBinaryCreateTranslation.DataType dataType;
            switch (data)
            {
                case IBoneModifierLookAtChainDataGetter:
                    dataType = BoneModifierBinaryCreateTranslation.DataType.LookAtChain;
                    break;
                case IBoneModifierMorphDriverDataGetter:
                    dataType = BoneModifierBinaryCreateTranslation.DataType.MorphDriver;
                    break;
                case IBoneModifierPoseDeformerDataGetter:
                    dataType = BoneModifierBinaryCreateTranslation.DataType.PoseDeformer;
                    break;
                case IBoneModifierSpringBoneDataGetter:
                    dataType = BoneModifierBinaryCreateTranslation.DataType.SpringBone;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            StringBinaryTranslation.Instance.Write(writer, dataType.ToStringFast(), StringBinaryType.PrependLengthWithNullIfContent);
            data.WriteToBinary(writer);
        }
    }
}

partial class BoneModifierBinaryOverlay
{
    private int? _DataLocation;
    
    partial void DataCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _DataLocation = (stream.Position - offset);
    }

    public partial IABoneModifierDataGetter? GetDataCustom()
    {
        if (!_DataLocation.HasValue) return null;
        var bytes = HeaderTranslation.ExtractSubrecordMemory(_recordData, _DataLocation.Value,
            _package.MetaData.Constants);
        var dataTypeStr = BinaryStringUtility.ParsePrependedString(bytes, lengthLength: 4,
            encoding: _package.MetaData.Encodings.NonTranslated);
        var dataType = Enum.Parse<BoneModifierBinaryCreateTranslation.DataType>(dataTypeStr);
        bytes = bytes.Slice(5 + dataTypeStr.Length);
        IABoneModifierDataGetter data;
        switch (dataType)
        {
            case BoneModifierBinaryCreateTranslation.DataType.LookAtChain:
                data = BoneModifierLookAtChainDataBinaryOverlay.BoneModifierLookAtChainDataFactory(new OverlayStream(bytes, _package), _package);
                break;
            case BoneModifierBinaryCreateTranslation.DataType.MorphDriver:
                data = BoneModifierMorphDriverDataBinaryOverlay.BoneModifierMorphDriverDataFactory(new OverlayStream(bytes, _package), _package);
                break;
            case BoneModifierBinaryCreateTranslation.DataType.PoseDeformer:
                data = BoneModifierPoseDeformerDataBinaryOverlay.BoneModifierPoseDeformerDataFactory(new OverlayStream(bytes, _package), _package);
                break;
            case BoneModifierBinaryCreateTranslation.DataType.SpringBone:
                data = BoneModifierSpringBoneDataBinaryOverlay.BoneModifierSpringBoneDataFactory(new OverlayStream(bytes, _package), _package);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return data;
    }
}