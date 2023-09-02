using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

public partial class StarfieldGroup<T> : AGroup<T>
{
    public StarfieldGroup(IModGetter getter) : base(getter)
    {
    }

    public StarfieldGroup(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface IStarfieldGroup<T> : IGroup<T>
    where T : class, IStarfieldMajorRecordInternal, IBinaryItem
{
}

public partial interface IStarfieldGroupGetter<out T> : IGroupGetter<T>
    where T : class, IStarfieldMajorRecordGetter, IBinaryItem
{
}

partial class StarfieldGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        IStarfieldGroupGetter<T> item)
        where T : class, IStarfieldMajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class StarfieldGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        IStarfieldGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

partial class StarfieldGroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
{
    partial void CustomFactoryEnd(
        OverlayStream stream,
        int finalPos,
        int offset)
    {
        _recordCache = GroupMajorRecordCacheWrapper<T>.Factory(
            stream,
            _recordData,
            _package,
            offset);
    }
}