using System.Collections;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class StarfieldListGroup<T> : AListGroup<T>
{
    public IEnumerator<T> GetEnumerator() => Records.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected override IExtendedList<T> ProtectedList => Records;
}

partial class StarfieldListGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeCustom(
        MutagenFrame frame,
        IStarfieldListGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

public partial interface IStarfieldListGroup<T> : IListGroup<T>
{
}

public partial interface IStarfieldListGroupGetter<out T> : IReadOnlyCollection<T>
{
}

partial class StarfieldListGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeCustom<T>(
        MutagenWriter writer,
        IStarfieldListGroupGetter<T> item)
        where T : class, ICellBlockGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class StarfieldListGroupBinaryOverlay<T> : AListGroupBinaryOverlay<T>
{
    partial void CustomFactoryEnd(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _Records = GroupListOverlay<T>.Factory(
            stream,
            _recordData,
            _package,
            offset: offset,
            objectType: ObjectType.Group);
    }

    public IEnumerator<T> GetEnumerator() => Records.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => Records.Count;
}