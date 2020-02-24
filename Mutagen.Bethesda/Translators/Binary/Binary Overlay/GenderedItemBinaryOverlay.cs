using Loqui;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class GenderedItemBinaryOverlay<T> : BinaryOverlay, IGenderedItemGetter<T>
    {
        private int? _male;
        private int? _female;
        private Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> _creator;

        public T Male => _male.HasValue ? _creator(_data.Slice(_male.Value), _package) : default!;
        public T Female => _female.HasValue ? _creator(_data.Slice(_female.Value), _package) : default!;

        public GenderedItemBinaryOverlay(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
            : base(bytes, package)
        {
            var find = UtilityTranslation.FindFirstSubrecords(bytes, _package.Meta, male, female);
            if (find[0] != null)
            {
                var subMeta = _package.Meta.SubRecord(bytes);
                this._male = find[0] + subMeta.TotalLength;
            }
            if (find[1] != null)
            {
                var subMeta = _package.Meta.SubRecord(bytes);
                this._female = find[1] + subMeta.TotalLength;
            }
            this._creator = creator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return Male;
            yield return Female;
        }

        public void ToString(FileGeneration fg, string? name) => GenderedItem.ToString(this, fg, name);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
