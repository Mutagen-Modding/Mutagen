using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public partial class FalloutNVGroup<T> : AGroup<T>
{
    public FalloutNVGroup(IModGetter getter) : base(getter)
    {
    }

    public FalloutNVGroup(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface IFalloutNVGroup<T> : IGroup<T>
    where T : class, IFalloutNVMajorRecordInternal, IBinaryItem
{
}

public partial interface IFalloutNVGroupGetter<out T> : IGroupGetter<T>
    where T : class, IFalloutNVMajorRecordGetter, IBinaryItem
{
}

partial class FalloutNVGroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        IFalloutNVGroupGetter<T> item)
        where T : class, IFalloutNVMajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class FalloutNVGroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        IFalloutNVGroup<T> item)
    {
        frame.Reader.Position += 4;
    }
}

internal partial class FalloutNVGroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
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