using Loqui;
using Loqui.Internal;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Group<T> : GroupAbstract<T>
    {
        public Group(IModGetter getter) : base(getter)
        {
        }

        public Group(IMod mod) : base(mod)
        {
        }

        protected override ICache<T, FormKey> ProtectedCache => this.RecordCache;
    }

    public partial interface IGroupGetter<out T> : IGroupCommon<T>
        where T : class, ISkyrimMajorRecordGetter, IXmlItem, IBinaryItem
    {
    }

    public static class GroupExt
    {
        public static async Task CreateFromXmlFolder<T>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder? errorMask,
            int index)
            where T : SkyrimMajorRecord, ILoquiObject<T>, IFormKey, IBinaryItem
        {
            throw new NotImplementedException();
        }

        public static async Task WriteToXmlFolder<T, T_ErrMask>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder? errorMask,
            int index)
            where T : SkyrimMajorRecord, ILoquiObject<T>, IFormKey, IBinaryItem
            where T_ErrMask : MajorRecord.ErrorMask, IErrorMask<T_ErrMask>, new()
        {
            throw new NotImplementedException();
        }
    }


    namespace Internals
    {
        public partial class GroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
                MutagenWriter writer,
                IGroupGetter<T> item,
                MasterReferences masterReferences)
                where T : class, ISkyrimMajorRecordGetter, IXmlItem, IBinaryItem
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
                IGroup<T> item,
                MasterReferences masterReferences)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class GroupBinaryOverlay<T>
        {
            private GroupMajorRecordCacheWrapper<T>? _Cache;
            public IReadOnlyCache<T, FormKey> RecordCache => _Cache!;
            public IMod SourceMod => throw new NotImplementedException();
            public IEnumerable<T> Records => RecordCache.Items;
            public int Count => this.RecordCache.Count;

            partial void CustomCtor(
                IBinaryReadStream stream,
                int finalPos,
                int offset)
            {
                _Cache = GroupMajorRecordCacheWrapper<T>.Factory(
                    stream,
                    _data,
                    _package,
                    offset);
            }
        }
    }
}
