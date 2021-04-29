using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Translations.Binary;

namespace Mutagen.Bethesda.Oblivion
{
    namespace Internals
    {
        public partial class ListGroupBinaryCreateTranslation<T>
        {
            public static partial void FillBinaryContainedRecordTypeCustom(
                MutagenFrame frame,
                IListGroup<T> item)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class ListGroupBinaryWriteTranslation
        {
            public static partial void WriteBinaryContainedRecordTypeCustom<T>(
                MutagenWriter writer,
                IListGroupGetter<T> item)
                where T : class, ICellBlockGetter, IBinaryItem
            {
                Int32BinaryTranslation<MutagenFrame, MutagenWriter>.Instance.Write(
                    writer,
                    GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE.TypeInt);
            }
        }

        public partial class ListGroupBinaryOverlay<T> : AListGroupBinaryOverlay<T>
        {
            partial void CustomFactoryEnd(
                OverlayStream stream,
                int finalPos,
                int offset)
            {
                _Records = GroupListOverlay<T>.Factory(
                    stream,
                    _data,
                    _package,
                    offset: offset,
                    objectType: ObjectType.Group);
            }
        }
    }
}
