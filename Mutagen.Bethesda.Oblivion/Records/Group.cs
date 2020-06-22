using Loqui;
using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Internals;
using Noggog;
using Loqui.Internal;
using System.Collections;
using Noggog.Utility;
using Mutagen.Bethesda.Folder;
using Loqui.Xml;
using Mutagen.Bethesda.Oblivion.Internals;
using System.Xml.Linq;

namespace Mutagen.Bethesda.Oblivion
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

    public partial interface IGroupGetter<out T> : IGroupCommon<T>
        where T : class, IOblivionMajorRecordGetter, IXmlItem, IBinaryItem
    {
    }

    namespace Internals
    {
        public partial class GroupBinaryWriteTranslation
        {
            static partial void WriteBinaryContainedRecordTypeParseCustom<T>(
                MutagenWriter writer,
                IGroupGetter<T> item)
                where T : class, IOblivionMajorRecordGetter, IXmlItem, IBinaryItem
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

        public partial class GroupBinaryOverlay<T>
        {
            private GroupMajorRecordCacheWrapper<T>? _RecordCache;
            public IReadOnlyCache<T, FormKey> RecordCache => _RecordCache!;
            public IMod SourceMod => throw new NotImplementedException();
            public IEnumerable<T> Records => RecordCache.Items;
            public int Count => this.RecordCache.Count;

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
