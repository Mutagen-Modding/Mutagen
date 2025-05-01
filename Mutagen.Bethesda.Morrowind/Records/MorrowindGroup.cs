using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Morrowind;

public partial class MorrowindGroup<T> : AGroup<T>
{
    public MorrowindGroup(IModGetter getter) : base(getter)
    {
    }

    public MorrowindGroup(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface IMorrowindGroup<T> : IGroup<T>
    where T : class, IMorrowindMajorRecordInternal, IBinaryItem
{
}

public partial interface IMorrowindGroupGetter<out T> : IGroupGetter<T>
    where T : class, IMorrowindMajorRecordGetter, IBinaryItem
{
}

partial class MorrowindGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        IMorrowindGroupGetter<T> item)
        where T : class, IMorrowindMajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class MorrowindGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        IMorrowindGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

internal partial class MorrowindGroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
{
    partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
    {
        _recordCache = GroupMajorRecordCacheWrapper<T>.Factory(
            stream,
            _recordData,
            _package,
            offset);
    }
}