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
    private readonly ReadOnlyMemorySlice<byte>? _maleData;
    private readonly ReadOnlyMemorySlice<byte>? _femaleData;
    private readonly T _maleFallback;
    private readonly T _femaleFallback;
    private readonly Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> _creator;

    public T Male => _maleData.HasValue ? _creator(_maleData.Value, _package) : _maleFallback;
    public T Female => _femaleData.HasValue ? _creator(_femaleData.Value, _package) : _femaleFallback;

    public T this[MaleFemaleGender gender] => gender == MaleFemaleGender.Male ? Male : Female;

    public GenderedItemBinaryOverlay(
        ReadOnlyMemorySlice<byte> bytes,
        BinaryOverlayFactoryPackage package,
        int? male,
        int? female,
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
        T fallback,
        IGenderedItemGetter<T>? existing = null)
        : base(new MemoryPair(bytes, bytes), package)
    {
        _creator = creator;
        var prior = existing as GenderedItemBinaryOverlay<T>;
        if (male.HasValue)
        {
            _maleData = bytes.Slice(male.Value);
            _maleFallback = fallback;
        }
        else if (prior != null)
        {
            _maleData = prior._maleData;
            _maleFallback = prior._maleFallback;
        }
        else
        {
            _maleFallback = existing != null ? existing.Male : fallback;
        }
        if (female.HasValue)
        {
            _femaleData = bytes.Slice(female.Value);
            _femaleFallback = fallback;
        }
        else if (prior != null)
        {
            _femaleData = prior._femaleData;
            _femaleFallback = prior._femaleFallback;
        }
        else
        {
            _femaleFallback = existing != null ? existing.Female : fallback;
        }
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
        RecordTypeConverter? maleRecordConverter = null,
        bool shortCircuit = true,
        bool parseNonConvertedItems = false,
        IGenderedItemGetter<T?>? existing = null)
        where T : class
    {
        var initialPos = stream.Position;
        T? maleObj = existing?.Male, femaleObj = existing?.Female;
        for (int i = 0; i < 2; i++)
        {
            if (stream.Complete) break;
            var subHeader = stream.GetSubrecordHeader();
            var recType = subHeader.RecordType;
            if (maleRecordConverter != null && maleRecordConverter.ToConversions.TryGetValue(recType, out var _))
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
            else if (maleRecordConverter == null && i == 0)
            {
                maleObj = creator(stream, package, new TypedParseParams(
                    lengthOverride: null,
                    recordTypeConverter: maleRecordConverter,
                    doNotShortCircuit: !shortCircuit));
            }
            else if (parseNonConvertedItems && ((maleObj == null && i == 0) || femaleObj == null))
            {
                var male = maleObj == null && i == 0;
                var startPos = stream.Position;
                var item = creator(stream, package, new TypedParseParams(
                    lengthOverride: null,
                    recordTypeConverter: male ? maleRecordConverter : femaleRecordConverter,
                    doNotShortCircuit: !shortCircuit));
                if (stream.Position == startPos) break;
                if (male)
                {
                    maleObj = item;
                }
                else
                {
                    femaleObj = item;
                }
            }
            else
            {
                break;
            }
        }

        var readLen = stream.Position - initialPos;
        if (readLen == 0 && existing == null)
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
            var recType = HeaderTranslation.ReadNextRecordType(stream,
                package.MetaData.Constants.SubConstants.LengthLength, out var markerLen);
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

            HeaderTranslation.ReadNextRecordType(stream, package.MetaData.Constants.SubConstants.LengthLength,
                out var recLen);
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
        TypedParseParams femaleRecordConverter = default,
        IGenderedItemGetter<T?>? existing = null)
        where T : class
    {
        var initialPos = stream.Position;
        T? maleObj = existing?.Male, femaleObj = existing?.Female;
        for (int i = 0; i < 2; i++)
        {
            if (stream.Complete) break;
            var markerHeader = stream.GetSubrecordHeader();
            var recType = markerHeader.RecordType;
            if (recType != male && recType != female) break;
            stream.Position += markerHeader.TotalLength;
            var startPos = stream.Position;
            var item = creator(stream, package, recType == male ? maleRecordConverter : femaleRecordConverter);
            if (startPos == stream.Position) continue;
            if (recType == male)
            {
                maleObj = item;
            }
            else
            {
                femaleObj = item;
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
        TypedParseParams translationParams,
        IGenderedItemGetter<T?>? existing = null)
        where T : class
    {
        return FactorySkipMarkersPreRead<T>(
            stream,
            package,
            male,
            female,
            creator,
            maleRecordConverter: translationParams,
            femaleRecordConverter: translationParams,
            existing: existing);
    }

    internal static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        RecordType marker,
        Func<OverlayStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
        TypedParseParams translationParams)
        where T : class
    {
        return FactorySkipMarkersPreRead<T>(
            stream,
            package,
            male,
            female,
            marker,
            creator,
            recordTypeConverter: translationParams.RecordTypeConverter,
            femaleRecordConverter: translationParams.RecordTypeConverter);
    }

    internal static IGenderedItemGetter<T?> FactorySkipMarkersPreRead<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        RecordType marker,
        Func<OverlayStream, BinaryOverlayFactoryPackage, RecordTypeConverter?, T> creator,
        RecordTypeConverter? recordTypeConverter = null,
        RecordTypeConverter? femaleRecordConverter = null,
        IGenderedItemGetter<T?>? existing = null)
        where T : class
    {
        var initialPos = stream.Position;
        T? maleObj = existing?.Male, femaleObj = existing?.Female;
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
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
        IGenderedItemGetter<T?>? existing = null)
        where T : class
    {
        int? maleLoc = null, femaleLoc = null;
        var find = RecordSpanExtensions.TryFindNextSubrecords(stream.RemainingMemory, package.MetaData.Constants,
            out var lenParsed, male, female);
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
            default,
            existing);
        stream.Position += lenParsed;
        return ret;
    }

    internal static IGenderedItemGetter<T> Factory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType genderEnumRecord,
        Func<T> getDefault,
        Func<OverlayStream, BinaryOverlayFactoryPackage, T> creator,
        IGenderedItemGetter<T>? existing = null)
        where T : class
    {
        T? male = existing?.Male, female = existing?.Female;
        for (int i = 0; i < 2; i++)
        {
            if (!stream.TryReadSubrecord(genderEnumRecord, out var enumRec)) break;
            switch ((GenderedItemBinaryTranslation.GenderEnum)enumRec.AsInt32())
            {
                case GenderedItemBinaryTranslation.GenderEnum.Male:
                    male = creator(stream, package);
                    break;
                case GenderedItemBinaryTranslation.GenderEnum.Female:
                    female = creator(stream, package);
                    break;
            }
        }

        return new GenderedItem<T>(male ?? getDefault(), female ?? getDefault());
    }

    internal static GenderedItemBinaryOverlay<T> Factory<T>(
        OverlayStream stream,
        BinaryOverlayFactoryPackage package,
        RecordType male,
        RecordType female,
        Func<ReadOnlyMemorySlice<byte>, BinaryOverlayFactoryPackage, T> creator,
        T fallback,
        IGenderedItemGetter<T>? existing = null)
        where T : notnull
    {
        int? maleLoc = null, femaleLoc = null;
        var find = RecordSpanExtensions.TryFindNextSubrecords(stream.RemainingMemory, package.MetaData.Constants,
            out var lenParsed, male, female);
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
            fallback,
            existing);
        stream.Position += lenParsed;
        return ret;
    }
}