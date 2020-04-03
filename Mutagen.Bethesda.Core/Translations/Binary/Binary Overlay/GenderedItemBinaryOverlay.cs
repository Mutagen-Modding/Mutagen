using Loqui;
using Mutagen.Bethesda.Internals;
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
            int? male,
            int? female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
            : base(bytes, package)
        {
            this._male = male;
            this._female = female;
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

    public static class GenderedItemBinaryOverlay
    {
        public static GenderedItemBinaryOverlay<T> FactorySkipMarkers<T>(
            BinaryMemoryReadStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            int offset,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
        {
            var initialPos = stream.Position;
            int? maleLoc = null, femaleLoc = null;
            for (int i = 0; i < 2; i++)
            {
                var recType = HeaderTranslation.ReadNextRecordType(stream, package.Meta.SubConstants.LengthLength, out var markerLen);
                stream.Position += markerLen;
                if (recType == male)
                {
                    maleLoc = (ushort)(stream.Position - offset);
                }
                else if (recType == female)
                {
                    femaleLoc = (ushort)(stream.Position - offset);
                }
                else
                {
                    break;
                }
                HeaderTranslation.ReadNextRecordType(stream, package.Meta.SubConstants.LengthLength, out var recLen);
                stream.Position += recLen;
            }
            var readLen = stream.Position - initialPos;
            if (readLen == 0)
            {
                throw new ArgumentException("Expected things to be read.");
            }
            return new GenderedItemBinaryOverlay<T>(
                stream.ReadBytes(readLen),
                package,
                maleLoc,
                femaleLoc,
                creator);
        }

        public static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
            BinaryMemoryReadStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<BinaryMemoryReadStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class
        {
            var initialPos = stream.Position;
            T? maleObj = null, femaleObj = null;
            for (int i = 0; i < 2; i++)
            {
                var recType = HeaderTranslation.ReadNextRecordType(stream, package.Meta.SubConstants.LengthLength, out var markerLen);
                stream.Position += markerLen;
                if (recType == male)
                {
                    var startPos = stream.Position;
                    maleObj = creator(stream, package, recordTypeConverter);
                    if (startPos == stream.Position)
                    {
                        maleObj = null;
                    }
                }
                else if (recType == female)
                {
                    var startPos = stream.Position;
                    femaleObj = creator(stream, package, recordTypeConverter);
                    if (startPos == stream.Position)
                    {
                        femaleObj = null;
                    }
                }
                else
                {
                    break;
                }
            }
            var readLen = stream.Position - initialPos;
            if (readLen == 0)
            {
                throw new ArgumentException("Expected things to be read.");
            }
            return new GenderedItem<T?>(maleObj, femaleObj);
        }

        public static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
            BinaryMemoryReadStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            RecordType marker,
            Func<BinaryMemoryReadStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
            RecordTypeConverter? recordTypeConverter = null,
            RecordTypeConverter? femaleRecordConverter = null)
            where T : class
        {
            var initialPos = stream.Position;
            T? maleObj = null, femaleObj = null;
            for (int i = 0; i < 2; i++)
            {
                // Skip marker
                var recType = HeaderTranslation.ReadNextRecordType(stream, package.Meta.SubConstants.LengthLength, out var markerLen);
                if (recType != marker) break;
                stream.Position += markerLen;

                // Read and skip gender marker
                recType = HeaderTranslation.ReadNextRecordType(stream, package.Meta.SubConstants.LengthLength, out markerLen);
                stream.Position += markerLen;
                if (recType == male)
                {
                    maleObj = creator(stream, package, recordTypeConverter);
                }
                else if (recType == female)
                {
                    femaleObj = creator(stream, package, femaleRecordConverter ?? recordTypeConverter);
                }
                else
                {
                    break;
                }
            }
            var readLen = stream.Position - initialPos;
            if (readLen == 0)
            {
                throw new ArgumentException("Expected things to be read.");
            }
            return new GenderedItem<T?>(maleObj, femaleObj);
        }

        public static GenderedItemBinaryOverlay<T> Factory<T>(
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
    }
}
