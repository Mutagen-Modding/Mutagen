using System.Collections.Generic;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Translations.Binary;
using Noggog;

namespace Mutagen.Bethesda.Fallout76;

public partial class Fallout76Group<T> : AGroup<T>
{
    public Fallout76Group(IModGetter getter) : base(getter)
    {
    }

    public Fallout76Group(IMod mod) : base(mod)
    {
    }

    protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
}

public partial interface IFallout76Group<T> : IGroup<T>
    where T : class, IFallout76MajorRecordInternal, IBinaryItem
{
}

public partial interface IFallout76GroupGetter<out T> : IGroupGetter<T>
    where T : class, IFallout76MajorRecordGetter, IBinaryItem
{
}

partial class Fallout76GroupBinaryWriteTranslation
{
    public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
        MutagenWriter writer,
        IFallout76GroupGetter<T> item)
        where T : class, IFallout76MajorRecordGetter, IBinaryItem
    {
        Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
            writer,
            GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
    }
}

partial class Fallout76GroupBinaryCreateTranslation<T>
{
    public static partial void FillBinaryContainedRecordTypeParseCustom(
        MutagenFrame frame,
        IFallout76Group<T> item)
    {
        frame.Reader.Position += 4;
    }
}

partial class Fallout76GroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
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