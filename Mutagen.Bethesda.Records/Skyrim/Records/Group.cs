using DynamicData;
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

        protected override IObservableCache<T, FormKey> InternalItems => throw new NotImplementedException();
    }

    public static class GroupExt
    {
        public static async Task CreateFromXmlFolder<T>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : SkyrimMajorRecord, ILoquiObject<T>, IFormKey, IXmlItem, IBinaryItem
        {
            throw new NotImplementedException();
        }

        public static async Task WriteToXmlFolder<T, T_ErrMask>(
            this Group<T> group,
            DirectoryPath dir,
            string name,
            ErrorMaskBuilder errorMask,
            int index)
            where T : SkyrimMajorRecord, ILoquiObject<T>, IFormKey, IXmlItem, IBinaryItem
            where T_ErrMask : MajorRecord_ErrorMask, IErrorMask<T_ErrMask>, new()
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
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
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
                MasterReferences masterReferences,
                ErrorMaskBuilder errorMask)
            {
                frame.Reader.Position += 4;
            }
        }

        public partial class GroupBinaryWrapper<T>
        {
            private GroupMajorRecordCacheWrapper<T> _Items;
            public IReadOnlyCache<T, FormKey> Items => _Items;

            partial void CustomCtor(
                IBinaryReadStream stream,
                int finalPos,
                int offset)
            {
                _Items = GroupMajorRecordCacheWrapper<T>.Factory(
                    stream,
                    _data,
                    _package,
                    offset);
            }
        }
    }
}
