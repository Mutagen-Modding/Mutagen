using System.Collections;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public partial class Fallout3ListGroup<T> : AListGroup<T>
{
    public IEnumerator<T> GetEnumerator() => Records.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected override IExtendedList<T> ProtectedList => Records;
}

partial class Fallout3ListGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeCustom(
        MutagenFrame frame,
        IFallout3ListGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

public partial interface IFallout3ListGroup<T> : IListGroup<T>
{
}

public partial interface IFallout3ListGroupGetter<out T> : IReadOnlyCollection<T>
{
}

partial class Fallout3ListGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeCustom<T>(
        MutagenWriter writer,
        IFallout3ListGroupGetter<T> item)
        where T : class, ICellBlockGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

internal partial class Fallout3ListGroupBinaryOverlay<T> : AListGroupBinaryOverlay<T>
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