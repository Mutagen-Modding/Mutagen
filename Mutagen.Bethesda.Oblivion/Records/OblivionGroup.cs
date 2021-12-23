using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Translations.Binary;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class OblivionGroup<T> : AGroup<T>
    {
        public OblivionGroup(IModGetter getter) : base(getter)
        {
        }

        public OblivionGroup(IMod mod) : base(mod)
        {
        }

        protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
    }

    public partial interface IOblivionGroup<T> : IGroupCommon<T>
        where T : class, IOblivionMajorRecordInternal, IBinaryItem
    {
    }

    public partial interface IOblivionGroupGetter<out T> : IGroupCommonGetter<T>
        where T : class, IOblivionMajorRecordGetter, IBinaryItem
    {
    }

    namespace Internals
    {
        public partial class OblivionGroupBinaryWriteTranslation
        {
            public static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
                MutagenWriter writer,
                IOblivionGroupGetter<T> item)
                where T : class, IOblivionMajorRecordGetter, IBinaryItem
            {
                Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }

        public partial class OblivionGroupBinaryCreateTranslation<T>
        {
            public static partial void FillBinaryContainedRecordTypeParseCustom(
                MutagenFrame frame,
                IOblivionGroup<T> item)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class OblivionGroupBinaryOverlay<T> : AGroupBinaryOverlay<T>
        {
            partial void CustomFactoryEnd(OverlayStream stream, int finalPos, int offset)
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
