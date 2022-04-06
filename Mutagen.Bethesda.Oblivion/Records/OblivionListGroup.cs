using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Oblivion;

partial class OblivionListGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeCustom(
        MutagenFrame frame,
        IOblivionListGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

partial class OblivionListGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeCustom<T>(
        MutagenWriter writer,
        IOblivionListGroupGetter<T> item)
        where T : class, ICellBlockGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

internal partial class OblivionListGroupBinaryOverlay<T> : AListGroupBinaryOverlay<T>
{
    partial void CustomFactoryEnd(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _Records = GroupListOverlay<T>.Factory(
            stream,
            _data,
            _package,
            offset: offset,
            objectType: ObjectType.Group);
    }
}