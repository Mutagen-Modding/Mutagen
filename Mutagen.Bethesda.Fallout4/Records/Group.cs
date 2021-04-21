using Mutagen.Bethesda.Records;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Mutagen.Bethesda.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Group<T> : AGroup<T>
    {
        public Group(IModGetter getter) : base(getter)
        {
        }

        public Group(IMod mod) : base(mod)
        {
        }

        protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
    }

    public partial interface IGroup<T> : IGroupCommon<T>
        where T : class, IFallout4MajorRecordInternal, IBinaryItem
    {
    }

    public partial interface IGroupGetter<out T> : IGroupCommonGetter<T>
        where T : class, IFallout4MajorRecordGetter, IBinaryItem
    {
    }

    namespace Internals
    {
        public partial class GroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
                MutagenWriter writer,
                IGroupGetter<T> item)
                where T : class, IFallout4MajorRecordGetter, IBinaryItem
            {
                Int32BinaryTranslation.Instance.Write(
                    writer,
                    GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }

        public partial class GroupBinaryCreateTranslation<T>
        {
            static partial void FillBinaryContainedRecordTypeParseCustom(
                MutagenFrame frame,
                IGroup<T> item)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class GroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
        {
            partial void CustomFactoryEnd(
                OverlayStream stream,
                int finalPos,
                int offset)
            {
                _RecordCache = GroupMajorRecordCacheWrapper<T>.Factory(
                    stream,
                    _data,
                    _package,
                    offset);
            }
        }
    }
}
