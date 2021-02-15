using Ionic.Zlib;
using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An abstract base class for Groups to inherit from for some common functionality
    /// </summary>
    public abstract class AGroup<TMajor> : IEnumerable<TMajor>, IGroupCommon<TMajor>
        where TMajor : class, IMajorRecordInternal, IBinaryItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract ICache<TMajor, FormKey> ProtectedCache { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ICache<TMajor, FormKey> InternalCache => this.ProtectedCache;

        /// <summary>
        /// An enumerable of all the records contained by the group.
        /// </summary>
        public IEnumerable<TMajor> Records => ProtectedCache.Items;

        /// <summary>
        /// Number of records contained in the group.
        /// </summary>
        public int Count => this.ProtectedCache.Count;

        /// <summary>
        /// The parent Mod object associated with the group.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IMod SourceMod { get; private set; }

        IReadOnlyCache<TMajor, FormKey> IGroupCommonGetter<TMajor>.RecordCache => InternalCache;

        /// <inheritdoc />
        public ICache<TMajor, FormKey> RecordCache => InternalCache;

        /// <inheritdoc />
        public IEnumerable<FormKey> FormKeys => InternalCache.Keys;

        /// <inheritdoc />
        public TMajor this[FormKey key] => InternalCache[key];

        private readonly GameRelease _release;

        protected AGroup()
        {
            this.SourceMod = null!;
        }

        protected AGroup(IModGetter getter)
        {
            this._release = getter.GameRelease;
            this.SourceMod = null!;
        }

        /// <summary>
        /// Constructor with parent Mod to be associated with
        /// </summary>
        public AGroup(IMod mod)
        {
            this._release = mod.GameRelease;
            this.SourceMod = mod;
        }

        /// <summary>
        /// Constructor with parent Mod to be associated with
        /// </summary>
        /// <returns>String in format: "Group(_record_count_)"</returns>
        public override string ToString()
        {
            return $"Group<{typeof(TMajor).Name}>({this.InternalCache.Count})";
        }

        /// <inheritdoc />
        public TMajor AddNew(FormKey formKey)
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(formKey, _release);
            InternalCache.Set(ret);
            return ret;
        }

        /// <inheritdoc />
        public TMajor AddNew()
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(SourceMod.GetNextFormKey(), _release);
            InternalCache.Set(ret);
            return ret;
        }

        /// <inheritdoc />
        public TMajor AddNew(string? editorID)
        {
            var ret = MajorRecordInstantiator<TMajor>.Activator(SourceMod.GetNextFormKey(editorID), _release);
            ret.EditorID = editorID;
            InternalCache.Set(ret);
            return ret;
        }

        /// <inheritdoc />
        public IEnumerator<TMajor> GetEnumerator()
        {
            return InternalCache.Items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalCache.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(TMajor item) => InternalCache.Add(item);

        /// <inheritdoc />
        public void Set(TMajor item) => InternalCache.Set(item);

        /// <inheritdoc />
        public void Set(IEnumerable<TMajor> items) => InternalCache.Set(items);

        /// <inheritdoc />
        public bool Remove(FormKey key) => InternalCache.Remove(key);

        /// <inheritdoc />
        public void Remove(IEnumerable<FormKey> keys) => InternalCache.Remove(keys);

        /// <inheritdoc />
        public void Clear() => InternalCache.Clear();

        /// <inheritdoc />
        public bool ContainsKey(FormKey key) => InternalCache.ContainsKey(key);
    }

    namespace Internals
    {
        public static class GroupRecordTypeGetter<T>
        {
            public static readonly RecordType GRUP_RECORD_TYPE;

            static GroupRecordTypeGetter()
            {
                var regis = LoquiRegistration.GetRegister(typeof(T));
                if (regis == null) throw new ArgumentException();
                GRUP_RECORD_TYPE = (RecordType)regis.ClassType.GetField(Mutagen.Bethesda.Internals.Constants.GrupRecordTypeMember)!.GetValue(null)!;
            }
        }

        public class GroupMajorRecordCacheWrapper<T> : IReadOnlyCache<T, FormKey>
        {
            private readonly IReadOnlyDictionary<FormKey, int> _locs;
            private readonly ReadOnlyMemorySlice<byte> _data;
            private readonly BinaryOverlayFactoryPackage _package;

            public GroupMajorRecordCacheWrapper(
                IReadOnlyDictionary<FormKey, int> locs,
                ReadOnlyMemorySlice<byte> data,
                BinaryOverlayFactoryPackage package)
            {
                this._locs = locs;
                this._data = data;
                this._package = package;
            }

            public T this[FormKey key]
            {
                get
                {
                    try
                    {
                        return ConstructWrapper(this._locs[key]);
                    }
                    catch (Exception ex)
                    {
                        throw RecordException.Factory(ex, key, edid: null);
                    }
                }
            }

            public int Count => this._locs.Count;

            public IEnumerable<FormKey> Keys => this._locs.Keys;

            public IEnumerable<T> Items => this.Select(kv => kv.Value);

            public bool ContainsKey(FormKey key) => this._locs.ContainsKey(key);

            public IEnumerator<IKeyValue<T, FormKey>> GetEnumerator()
            {
                foreach (var kv in this._locs)
                {
                    KeyValue<T, FormKey> item;
                    try
                    {
                        item = new KeyValue<T, FormKey>(kv.Key, ConstructWrapper(kv.Value));
                    }
                    catch (Exception ex)
                    {
                        throw RecordException.Factory(ex, kv.Key, edid: null);
                    }
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            private T ConstructWrapper(int pos)
            {
                ReadOnlyMemorySlice<byte> slice = this._data.Slice(pos);
                var majorMeta = _package.MetaData.Constants.MajorRecord(slice);
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
                   stream: new OverlayStream(this._data.Slice(pos), _package),
                   package: _package,
                   recordTypeConverter: null);
            }

            public static GroupMajorRecordCacheWrapper<T> Factory(
                IBinaryReadStream stream,
                ReadOnlyMemorySlice<byte> data,
                BinaryOverlayFactoryPackage package,
                int offset)
            {
                Dictionary<FormKey, int> locationDict = new Dictionary<FormKey, int>();

                stream.Position -= package.MetaData.Constants.GroupConstants.HeaderLength;
                var groupMeta = stream.GetGroup(package.MetaData);
                var finalPos = stream.Position + groupMeta.TotalLength;
                stream.Position += package.MetaData.Constants.GroupConstants.HeaderLength;
                // Parse MajorRecord locations
                ObjectType? lastParsed = default;
                while (stream.Position < finalPos)
                {
                    VariableHeader varMeta = package.MetaData.Constants.NextRecordVariableMeta(stream.RemainingMemory);
                    if (varMeta.IsGroup)
                    {
                        if (lastParsed != ObjectType.Record)
                        {
                            throw new DataMisalignedException("Unexpected Group encountered which was not after a major record: " + GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE);
                        }
                        stream.Position += checked((int)varMeta.TotalLength);
                        lastParsed = ObjectType.Group;
                    }
                    else
                    {
                        MajorRecordHeader majorMeta = package.MetaData.Constants.MajorRecord(stream.RemainingMemory);
                        if (majorMeta.RecordType != GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE)
                        {
                            throw new DataMisalignedException("Unexpected type encountered when parsing MajorRecord locations: " + majorMeta.RecordType);
                        }
                        var formKey = FormKey.Factory(package.MetaData.MasterReferences!, majorMeta.FormID.Raw);
                        try
                        {
                            locationDict.Add(formKey, checked((int)(stream.Position - offset)));
                        }
                        catch (ArgumentException)
                        {
                            throw new RecordCollisionException(formKey, typeof(T));
                        }
                        stream.Position += checked((int)majorMeta.TotalLength);
                        lastParsed = ObjectType.Record;
                    }
                }

                return new GroupMajorRecordCacheWrapper<T>(
                    locationDict,
                    data,
                    package);
            }
        }

        public class AGroupBinaryOverlay<TMajor> : BinaryOverlay, IGroupCommonGetter<TMajor>
            where TMajor : IMajorRecordCommonGetter, IBinaryItem
        {
            protected GroupMajorRecordCacheWrapper<TMajor>? _RecordCache;

            public TMajor this[FormKey key] => _RecordCache![key];
            public IReadOnlyCache<TMajor, FormKey> RecordCache => _RecordCache!;
            public IMod SourceMod => throw new NotImplementedException();
            public IEnumerable<TMajor> Records => RecordCache.Items;
            public int Count => this.RecordCache.Count;
            public IEnumerable<FormKey> FormKeys => _RecordCache!.Keys;
            public IEnumerable<TMajor> Items => _RecordCache!.Items;

            public bool ContainsKey(FormKey key)
            {
                return _RecordCache!.ContainsKey(key);
            }

            public IEnumerator<TMajor> GetEnumerator()
            {
                return _RecordCache!.Items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _RecordCache!.GetEnumerator();
            }

            protected AGroupBinaryOverlay(
                ReadOnlyMemorySlice<byte> bytes,
                BinaryOverlayFactoryPackage package)
                : base(bytes, package)
            {
            }
        }
    }
}
