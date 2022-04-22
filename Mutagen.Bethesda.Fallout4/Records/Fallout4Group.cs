using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

public partial class Fallout4Group<T> : AGroup<T>
{
    public Fallout4Group(IModGetter getter) : base(getter)
    {
    }

    public Fallout4Group(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface IFallout4Group<T> : IGroup<T>
    where T : class, IFallout4MajorRecordInternal, IBinaryItem
{
}

public partial interface IFallout4GroupGetter<out T> : IGroupGetter<T>
    where T : class, IFallout4MajorRecordGetter, IBinaryItem
{
}

partial class Fallout4GroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        IFallout4GroupGetter<T> item)
        where T : class, IFallout4MajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class Fallout4GroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        IFallout4Group<T> item)
    {
        frame.Reader.Position += 4;
    }
}

partial class Fallout4GroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
{
    partial void CustomFactoryEnd(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _recordCache = GroupMajorRecordCacheWrapper<T>.Factory(
            stream,
            _data,
            _package,
            offset);
    }
}