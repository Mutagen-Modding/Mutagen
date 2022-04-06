using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Skyrim;

public partial class SkyrimGroup<T> : AGroup<T>
{
    public SkyrimGroup(IModGetter getter) : base(getter)
    {
    }

    public SkyrimGroup(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface ISkyrimGroup<T> : IGroup<T>
    where T : class, ISkyrimMajorRecordInternal, IBinaryItem
{
}

public partial interface ISkyrimGroupGetter<out T> : IGroupGetter<T>
    where T : class, ISkyrimMajorRecordGetter, IBinaryItem
{
}

partial class SkyrimGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        ISkyrimGroupGetter<T> item)
        where T : class, ISkyrimMajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class SkyrimGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        ISkyrimGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

partial class SkyrimGroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
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