using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class Fallout3Group<T> : AGroup<T>
{
    public Fallout3Group(IModGetter getter) : base(getter)
    {
    }

    public Fallout3Group(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface IFallout3Group<T> : IGroup<T>
    where T : class, IFallout3MajorRecordInternal, IBinaryItem
{
}

public partial interface IFallout3GroupGetter<out T> : IGroupGetter<T>
    where T : class, IFallout3MajorRecordGetter, IBinaryItem
{
}

partial class Fallout3GroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        IFallout3GroupGetter<T> item)
        where T : class, IFallout3MajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class Fallout3GroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        IFallout3Group<T> item)
    {
        frame.Reader.Position += 4;
    }
}

internal partial class Fallout3GroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
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