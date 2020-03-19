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

        private GenderedItemBinaryOverlay(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryOverlayFactoryPackage package,
            int? male,
            int? female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
            : base(bytes, package)
        {
            this._male = male;
            this._female = female;
            this._creator = creator;
        }

        public static GenderedItemBinaryOverlay<T> FactorySkipMarkers(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
        {
            var find = UtilityTranslation.FindFirstSubrecords(bytes, package.Meta, male, female);
            int? maleLoc = null, femaleLoc = null;
            if (find[0] != null)
            {
                var subMeta = package.Meta.SubRecord(bytes);
                maleLoc = find[0] + subMeta.TotalLength;
            }
            if (find[1] != null)
            {
                var subMeta = package.Meta.SubRecord(bytes);
                femaleLoc = find[1] + subMeta.TotalLength;
            }
            return new GenderedItemBinaryOverlay<T>(
                bytes,
                package,
                maleLoc,
                femaleLoc,
                creator);
        }

        public static GenderedItemBinaryOverlay<T> Factory(
            BinaryMemoryReadStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
        {
            int? maleLoc = null, femaleLoc = null;
            var find = UtilityTranslation.FindNextSubrecords(stream.RemainingSpan, package.Meta, out var lenParsed, male, female);
            if (find[0] != null)
            {
                maleLoc = find[0];
            }
            if (find[1] != null)
            {
                femaleLoc = find[1];
            }
            var ret = new GenderedItemBinaryOverlay<T>(
                stream.RemainingMemory.Slice(0, lenParsed),
                package,
                maleLoc,
                femaleLoc,
                creator);
            stream.Position += lenParsed;
            return ret;
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
