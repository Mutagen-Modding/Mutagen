using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System.Collections;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal sealed class GenderedItemBinaryOverlay<T> : PluginBinaryOverlay, IGenderedItemGetter<T>
{
    private readonly int? _male;
    private readonly int? _female;
    private readonly T _fallback;
    private readonly Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> _creator;

    public T Male => _male.HasValue ? _creator(_recordData.Slice(_male.Value), _package) : _fallback;
    public T Female => _female.HasValue ? _creator(_recordData.Slice(_female.Value), _package) : _fallback;

    public T this[MaleFemaleGender gender] => gender == MaleFemaleGender.Male ? Male : Female;

    public GenderedItemBinaryOverlay(
        ReadOnlyMemorySlice<byte> bytes,
        BinaryOverlayFactoryPackage package,
        int? male,
        int? female,
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
        T fallback)
        : base(new MemoryPair(bytes, bytes), package)
    {
        _male = male;
        _female = female;
        _creator = creator;
        _fallback = fallback;
    }

    public IEnumerator<T> GetEnumerator()
    {
        yield return Male;
        yield return Female;
    }

    public void Print(StructuredStringBuilder fg, string? name) => GenderedItem.Print(this, fg, name);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal static class GenderedItemBinaryOverlay
{
    internal static IGenderedItemGetter<T?> Factory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        Func<OverlayStream, BinaryOverlayFactoryPackage, TypedParseParams, T> creator,
        RecordTypeConverter femaleRecordConverter,
        RecordTypeConverter maleRecordConverter,
        bool shortCircuit = true)
        where T : class
    {
        var initialPos = stream.Position;
        T? maleObj = null, femaleObj = null;
        for (int i = 0; i < 2; i++)
        {
            if (stream.Complete) break;
            var subHeader = stream.GetSubrecordHeader();
            var recType = subHeader.RecordType;
            if (maleRecordConverter.ToConversions.TryGetValue(recType, out var _))
            {
                maleObj = creator(stream, package, new TypedParseParams(
                    lengthOverride: null,
                    recordTypeConverter: maleRecordConverter,
                    doNotShortCircuit: !shortCircuit));
            }
            else if (femaleRecordConverter.ToConversions.TryGetValue(recType, out var _))
            {
                femaleObj = creator(stream, package, new TypedParseParams(
                    lengthOverride: null,
                    recordTypeConverter: femaleRecordConverter,
                    doNotShortCircuit: !shortCircuit));
            }
            else if (i == 0)
            {
                maleObj = creator(stream, package, new TypedParseParams(
                    lengthOverride: null,
                    recordTypeConverter: maleRecordConverter,
                    doNotShortCircuit: !shortCircuit));
            }
            else if (i == 1)
            {
                femaleObj = creator(stream, package, new TypedParseParams(
                    lengthOverride: null,
                    recordTypeConverter: maleRecordConverter,
                    doNotShortCircuit: !shortCircuit));
            }
        }
        var readLen = stream.Position - initialPos;
        if (readLen == 0)
        {
            throw new ArgumentException("Expected things to be read.");
        }
        return new GenderedItem<T?>(maleObj, femaleObj);
    }

    internal static GenderedItemBinaryOverlay<T> FactorySkipMarkers<T>(
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

    internal static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        Func<OverlayStream, BinaryOverlayFactoryPackage, TypedParseParams, T> creator,
        TypedParseParams maleRecordConverter = default,
        TypedParseParams femaleRecordConverter = default)
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
                maleObj = creator(stream, package, maleRecordConverter);
                if (startPos == stream.Position)
                {
                    maleObj = null;
                }
            }
            else if (recType == female)
            {
                var startPos = stream.Position;
                femaleObj = creator(stream, package, femaleRecordConverter);
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

    internal static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        Func<OverlayStream, BinaryOverlayFactoryPackage, TypedParseParams, T> creator,
        TypedParseParams translationParams)
        where T : class
    {
        return FactorySkipMarkersPreRead<T>(
            stream,
            package,
            male,
            female,
            creator,
            maleRecordConverter: translationParams,
            femaleRecordConverter: translationParams);
    }

    internal static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
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
            var markerFrame = stream.GetSubrecord();
            if (markerFrame.RecordType != marker) break;
            stream.Position += markerFrame.TotalLength;

            // Read and skip gender marker
            var genderMarkerFrame = stream.GetSubrecord();
            var recType = genderMarkerFrame.RecordType;
            if (recType == male)
            {
                stream.Position += genderMarkerFrame.TotalLength;
                maleObj = creator(stream, package, recordTypeConverter);
            }
            else if (recType == female)
            {
                stream.Position += genderMarkerFrame.TotalLength;
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

    internal static GenderedItemBinaryOverlay<T?> Factory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator)
        where T : class
    {
        int? maleLoc = null, femaleLoc = null;
        var find = RecordSpanExtensions.TryFindNextSubrecords(stream.RemainingMemory, package.MetaData.Constants, out var lenParsed, male, female);
        if (find[0] is { } firstFind)
        {
            maleLoc = firstFind.Location;
        }
        if (find[1] is { } secondFind)
        {
            femaleLoc = secondFind.Location;
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

    internal static GenderedItemBinaryOverlay<T> Factory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
        T fallback)
        where T : notnull
    {
        int? maleLoc = null, femaleLoc = null;
        var find = RecordSpanExtensions.TryFindNextSubrecords(stream.RemainingMemory, package.MetaData.Constants, out var lenParsed, male, female);
        if (find[0] is { } firstFind)
        {
            maleLoc = firstFind.Location;
        }
        if (find[1] is { } secondFind)
        {
            femaleLoc = secondFind.Location;
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