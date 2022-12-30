using Ionic.Zlib;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Noggog;
using System.Buffers.Binary;
using System.Collections;
using Loqui;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Assets;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal sealed class GroupListOverlay<T> : IReadOnlyList<T>
{
    private readonly IReadOnlyList<int> _locs;
    private readonly ReadOnlyMemorySlice<byte> _data;
    private readonly BinaryOverlayFactoryPackage _package;

    public int Count => _locs.Count;

    public T this[int index] => ConstructWrapper(_locs[index]);

    public GroupListOverlay(
        IReadOnlyList<int> locs,
        ReadOnlyMemorySlice<byte> data,
        BinaryOverlayFactoryPackage package)
    {
        _locs = locs;
        _data = data;
        _package = package;
    }

    private T ConstructWrapper(int pos)
    {
        ReadOnlyMemorySlice<byte> slice = _data.Slice(pos);
        var majorMeta = _package.MetaData.Constants.MajorRecordHeader(slice);
        if (majorMeta.IsCompressed)
        {
            uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(slice.Slice(majorMeta.HeaderLength));
            byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
            // Copy major meta bytes over
            slice.Span.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
            // Set length bytes
            BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HeaderLength), uncompressedLength);
            // Remove compression flag
            BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan().Slice(_package.MetaData.Constants.MajorConstants.FlagLocationOffset), majorMeta.MajorRecordFlags & ~Constants.CompressedFlag);
            // Copy uncompressed data over
            using (var stream = new ZlibStream(new ByteMemorySliceStream(slice.Slice(majorMeta.HeaderLength + 4)), CompressionMode.Decompress))
            {
                stream.Read(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
            }
            slice = new MemorySlice<byte>(buf);
        }
        return LoquiBinaryOverlayTranslation<T>.Create(
            stream: new OverlayStream(slice, _package),
            package: _package,
            recordTypeConverter: null);
    }

    public static GroupListOverlay<T> Factory(
        IBinaryReadStream stream,
        ReadOnlyMemorySlice<byte> data,
        BinaryOverlayFactoryPackage package,
        ObjectType objectType,
        int offset)
    {
        List<int> locations = new List<int>();

        stream.Position -= package.MetaData.Constants.GroupConstants.HeaderLength;
        var groupMeta = stream.GetGroupHeader(package);
        var finalPos = stream.Position + groupMeta.TotalLength;
        stream.Position += package.MetaData.Constants.GroupConstants.HeaderLength;
        // Parse locations
        while (stream.Position < finalPos)
        {
            VariableHeader meta = package.MetaData.Constants.VariableHeader(stream.RemainingMemory, objectType);
            locations.Add(checked((int)stream.Position - offset));
            stream.Position += checked((int)meta.TotalLength);
        }

        return new GroupListOverlay<T>(
            locations,
            data,
            package);
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var loc in _locs)
        {
            yield return ConstructWrapper(loc);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal abstract class AListGroupBinaryOverlay<TObject> : PluginBinaryOverlay, IListGroupGetter<TObject>
    where TObject : class, ILoquiObject
{
    protected GroupListOverlay<TObject>? _Records;
    public IReadOnlyList<TObject> Records => _Records!;
    IEnumerable<TObject> IGroupCommonGetter<TObject>.Records => Records;
    IReadOnlyList<ILoquiObject> IListGroupGetter.Records => Records;
    IEnumerable<ILoquiObject> IGroupCommonGetter.Records => Records;

    public ILoquiRegistration ContainedRecordRegistration => throw new NotImplementedException();

    protected AListGroupBinaryOverlay(
        PluginBinaryOverlay.MemoryPair memoryPair,
        BinaryOverlayFactoryPackage package)
        : base(memoryPair, package)
    {
    }

    public IEnumerator<TObject> GetEnumerator()
    {
        return Records.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Records).GetEnumerator();
    }

    IEnumerable<TObject> IListGroupGetter<TObject>.GetEnumerator() => Records;

    public abstract IEnumerable<IFormLinkGetter> EnumerateFormLinks();

    public abstract IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(AssetLinkQuery queryCategories, IAssetLinkCache? linkCache = null, Type? assetType = null);

    public int Count => Records.Count;

    public Type ContainedRecordType => typeof(TObject);

    public TObject this[int index] => Records[index];
}