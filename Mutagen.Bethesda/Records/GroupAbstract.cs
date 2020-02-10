using Ionic.Zlib;
using Loqui;
using Loqui.Xml;
using Mutagen.Bethesda.Binary;
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
    public abstract class GroupAbstract<T> : IEnumerable<T>, IGroupCommon<T>
        where T : IMajorRecordInternal, IXmlItem, IBinaryItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected abstract ICache<T, FormKey> ProtectedCache { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ICache<T, FormKey> InternalCache => this.ProtectedCache;
        public IEnumerable<T> Records => ProtectedCache.Items;
        public int Count => this.ProtectedCache.Count;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IMod SourceMod { get; private set; }

        protected GroupAbstract()
        {
            this.SourceMod = null!;
        }

        public GroupAbstract(IModGetter getter)
        {
            this.SourceMod = null!;
        }

        public GroupAbstract(IMod mod)
        {
            this.SourceMod = mod;
        }

        public override string ToString()
        {
            return $"Group<{typeof(T).Name}>({this.InternalCache.Count})";
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InternalCache.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalCache.GetEnumerator();
        }
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
                GRUP_RECORD_TYPE = (RecordType)regis.ClassType.GetField(Mutagen.Bethesda.Constants.GRUP_RECORDTYPE_MEMBER).GetValue(null);
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

            public T this[FormKey key] => ConstructWrapper(this._locs[key]);

            public int Count => this._locs.Count;

            public IEnumerable<FormKey> Keys => this._locs.Keys;

            public IEnumerable<T> Items => this.Select(kv => kv.Value);

            public bool ContainsKey(FormKey key) => this._locs.ContainsKey(key);

            public IEnumerator<IKeyValue<T, FormKey>> GetEnumerator()
            {
                foreach (var kv in this._locs)
                {
                    yield return new KeyValue<T, FormKey>(kv.Key, ConstructWrapper(kv.Value));
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            private T ConstructWrapper(int pos)
            {
                ReadOnlyMemorySlice<byte> slice = this._data.Slice(pos);
                var majorMeta = _package.Meta.MajorRecord(slice);
                if (majorMeta.IsCompressed)
                {
                    uint uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(slice.Slice(majorMeta.HeaderLength));
                    byte[] buf = new byte[majorMeta.HeaderLength + checked((int)uncompressedLength)];
                    // Copy major meta bytes over
                    slice.Span.Slice(0, majorMeta.HeaderLength).CopyTo(buf.AsSpan());
                    // Set length bytes
                    BinaryPrimitives.WriteUInt32LittleEndian(buf.AsSpan().Slice(Constants.HEADER_LENGTH), uncompressedLength);
                    // Remove compression flag
                    BinaryPrimitives.WriteInt32LittleEndian(buf.AsSpan().Slice(_package.Meta.MajorConstants.FlagLocationOffset), majorMeta.MajorRecordFlags & ~Constants.CompressedFlag);
                    // Copy uncompressed data over
                    using (var stream = new ZlibStream(new ByteMemorySliceStream(slice.Slice(majorMeta.HeaderLength + 4)), CompressionMode.Decompress))
                    {
                        stream.Read(buf, majorMeta.HeaderLength, checked((int)uncompressedLength));
                    }
                    slice = new MemorySlice<byte>(buf);
                }
                return LoquiBinaryOverlayTranslation<T>.Create(
                   stream: new BinaryMemoryReadStream(this._data.Slice(pos)),
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

                stream.Position -= package.Meta.GroupConstants.HeaderLength;
                var groupMeta = package.Meta.GetGroup(stream);
                var finalPos = stream.Position + groupMeta.TotalLength;
                stream.Position += package.Meta.GroupConstants.HeaderLength;
                // Parse MajorRecord locations
                ObjectType? lastParsed = default;
                while (stream.Position < finalPos)
                {
                    VariableHeader varMeta = package.Meta.NextRecordVariableMeta(stream.RemainingSpan);
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
                        MajorRecordHeader majorMeta = package.Meta.MajorRecord(stream.RemainingSpan);
                        if (majorMeta.RecordType != GroupRecordTypeGetter<T>.GRUP_RECORD_TYPE)
                        {
                            throw new DataMisalignedException("Unexpected type encountered when parsing MajorRecord locations: " + majorMeta.RecordType);
                        }
                        var formKey = FormKey.Factory(package.MasterReferences!, majorMeta.FormID.Raw);
                        locationDict.Add(formKey, checked((int)(stream.Position - offset)));
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
    }
}
