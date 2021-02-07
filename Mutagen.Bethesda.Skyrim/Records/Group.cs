using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
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
        where T : class, ISkyrimMajorRecordInternal, IBinaryItem
    {
    }

    public partial interface IGroupGetter<out T> : IGroupCommonGetter<T>
        where T : class, ISkyrimMajorRecordGetter, IBinaryItem
    {
    }

    namespace Internals
    {
        public partial class GroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
                MutagenWriter writer,
                IGroupGetter<T> item)
                where T : class, ISkyrimMajorRecordGetter, IBinaryItem
            {
                Mutagen.Bethesda.Binary.Int32BinaryTranslation.Instance.Write(
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
