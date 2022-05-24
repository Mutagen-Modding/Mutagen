using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

public partial class ADamageType
{
    public static ADamageType CreateFromBinary(
        MutagenFrame frame,
        TypedParseParams? translationParams)
    {
        var majorMeta = frame.GetMajorRecord();
        if (majorMeta.FormVersion >= 78)
        {
            return DamageType.CreateFromBinary(frame, translationParams); 
        }
        else
        {
            return DamageTypeIndexed.CreateFromBinary(frame, translationParams);
        }
    }
}

partial class ADamageTypeBinaryOverlay
{
    public static IADamageTypeGetter ADamageTypeFactory(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        TypedParseParams? translationParams)
    {
        var majorFrame = package.MetaData.Constants.MajorRecord(stream.RemainingMemory);
        if (majorFrame.FormVersion >= 78)
        {
            return DamageTypeBinaryOverlay.DamageTypeFactory(stream, package, translationParams);
        }
        else
        {
            return  DamageTypeIndexedBinaryOverlay.DamageTypeIndexedFactory(stream, package, translationParams);
        }
    }
}

partial class ADamageTypeBinaryCreateTranslation
{
    public static partial void FillBinaryCustomLogicCustom(MutagenFrame frame, IADamageTypeInternal item)
    {
    }
}

partial class ADamageTypeBinaryWriteTranslation
{
    public static partial void WriteBinaryCustomLogicCustom(MutagenWriter writer, IADamageTypeGetter item)
    {
    }
}