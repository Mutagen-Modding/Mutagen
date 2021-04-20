using Loqui;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Records.Binary.Streams;
using Mutagen.Bethesda.Records.Binary.Translations;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Records.Binary.Overlay
{
    public class GenderedItemBinaryOverlay<T> : BinaryOverlay, IGenderedItemGetter<T>
    {
        private int? _male;
        private int? _female;
        private T _fallback;
        private Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> _creator;

        public T Male => _male.HasValue ? _creator(_data.Slice(_male.Value), _package) : _fallback;
        public T Female => _female.HasValue ? _creator(_data.Slice(_female.Value), _package) : _fallback;

        public GenderedItemBinaryOverlay(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryOverlayFactoryPackage package,
            int? male,
            int? female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
            T fallback)
            : base(bytes, package)
        {
            this._male = male;
            this._female = female;
            this._creator = creator;
            this._fallback = fallback;
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
        public static IGenderedItemGetter<T?> Factory<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            Func<OverlayStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
            RecordTypeConverter femaleRecordConverter,
            RecordTypeConverter maleRecordConverter)
            where T : class
        {
            var initialPos = stream.Position;
            T? maleObj = null, femaleObj = null;
            for (int i = 0; i < 2; i++)
            {
                if (stream.Complete) break;
                var subHeader = stream.GetSubrecord();
                var recType = subHeader.RecordType;
                if (maleRecordConverter.ToConversions.TryGetValue(recType, out var _))
                {
                    maleObj = creator(stream, package, maleRecordConverter);
                }
                else if (femaleRecordConverter.ToConversions.TryGetValue(recType, out var _))
                {
                    femaleObj = creator(stream, package, femaleRecordConverter);
                }
            }
            var readLen = stream.Position - initialPos;
            if (readLen == 0)
            {
                throw new ArgumentException("Expected things to be read.");
            }
            return new GenderedItem<T?>(maleObj, femaleObj);
        }

        public static GenderedItemBinaryOverlay<T> FactorySkipMarkers<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            int offset,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
            T fallback)
        {
            var initialPos = stream.Position;
            int? maleLoc = null, femaleLoc = null;
            for (int i = 0; i < 2; i++)
            {
                if (stream.Complete) break;
                var recType = HeaderTranslation.ReadNextRecordType(stream, package.MetaData.Constants.SubConstants.LengthLength, out var markerLen);
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
                HeaderTranslation.ReadNextRecordType(stream, package.MetaData.Constants.SubConstants.LengthLength, out var recLen);
                stream.Position += recLen;
            }
            var readLen = stream.Position - initialPos;
            if (readLen == 0)
            {
                throw new ArgumentException("Expected things to be read.");
            }
            return new GenderedItemBinaryOverlay<T>(
                stream.ReadMemory(readLen),
                package,
                maleLoc,
                femaleLoc,
                creator,
                fallback);
        }

        public static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<OverlayStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
            RecordTypeConverter? recordTypeConverter = null)
            where T : class
        {
            var initialPos = stream.Position;
            T? maleObj = null, femaleObj = null;
            for (int i = 0; i < 2; i++)
            {
                if (stream.Complete) break;
                var recType = HeaderTranslation.ReadNextRecordType(stream, package.MetaData.Constants.SubConstants.LengthLength, out var markerLen);
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
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            RecordType marker,
            Func<OverlayStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
            RecordTypeConverter? recordTypeConverter = null,
            RecordTypeConverter? femaleRecordConverter = null)
            where T : class
        {
            var initialPos = stream.Position;
            T? maleObj = null, femaleObj = null;
            for (int i = 0; i < 2; i++)
            {
                if (stream.Complete) break;
                // Skip marker
                var recType = HeaderTranslation.ReadNextRecordType(stream, package.MetaData.Constants.SubConstants.LengthLength, out var markerLen);
                if (recType != marker) break;
                stream.Position += markerLen;

                // Read and skip gender marker
                recType = HeaderTranslation.ReadNextRecordType(stream, package.MetaData.Constants.SubConstants.LengthLength, out markerLen);
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

        public static GenderedItemBinaryOverlay<T?> Factory<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
            where T : class
        {
            int? maleLoc = null, femaleLoc = null;
            var find = UtilityTranslation.FindNextSubrecords(stream.RemainingMemory, package.MetaData.Constants, out var lenParsed, male, female);
            if (find[0] != null)
            {
                maleLoc = find[0];
            }
            if (find[1] != null)
            {
                femaleLoc = find[1];
            }
            var ret = new GenderedItemBinaryOverlay<T?>(
                stream.RemainingMemory.Slice(0, lenParsed),
                package,
                maleLoc,
                femaleLoc,
                creator,
                default);
            stream.Position += lenParsed;
            return ret;
        }

        public static GenderedItemBinaryOverlay<T> Factory<T>(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            RecordType male,
            RecordType female,
            Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
            T fallback)
            where T : notnull
        {
            int? maleLoc = null, femaleLoc = null;
            var find = UtilityTranslation.FindNextSubrecords(stream.RemainingMemory, package.MetaData.Constants, out var lenParsed, male, female);
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
                creator,
                fallback);
            stream.Position += lenParsed;
            return ret;
        }
    }
}
