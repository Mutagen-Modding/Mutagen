using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class RecordConstants
    {
        public ObjectType ObjectType { get; }
        public GameMode GameMode { get; }
        public sbyte HeaderLength { get; }
        public sbyte LengthLength { get; }
        public sbyte LengthAfterLength { get; }
        public sbyte LengthAfterType { get; }
        public sbyte TypeAndLengthLength { get; }
        public bool HeaderIncludedInLength { get; }

        public RecordConstants(
            GameMode gameMode,
            ObjectType type,
            sbyte headerLength,
            sbyte lengthLength)
        {
            this.GameMode = gameMode;
            this.ObjectType = type;
            this.HeaderLength = headerLength;
            this.LengthLength = lengthLength;
            this.LengthAfterLength = (sbyte)(this.HeaderLength - Constants.HEADER_LENGTH - this.LengthLength);
            this.LengthAfterType = (sbyte)(this.HeaderLength - Constants.HEADER_LENGTH);
            this.TypeAndLengthLength = (sbyte)(Constants.HEADER_LENGTH + this.LengthLength);
            this.HeaderIncludedInLength = type == ObjectType.Group;
        }

        public VariableHeaderMeta VariableMeta(ReadOnlySpan<byte> span) => new VariableHeaderMeta(this, span);
        public VariableHeaderMeta GetVariableMeta(IBinaryReadStream stream, int offset = 0) => new VariableHeaderMeta(this, stream.GetSpan(this.HeaderLength, offset));
        public VariableHeaderMeta ReadVariableMeta(IBinaryReadStream stream) => new VariableHeaderMeta(this, stream.ReadSpan(this.HeaderLength));
    }

    public class MajorRecordConstants : RecordConstants
    {
        public sbyte FlagLocationOffset { get; }
        public sbyte FormIDLocationOffset { get; }

        public MajorRecordConstants(
            GameMode gameMode,
            ObjectType type,
            sbyte headerLength,
            sbyte lengthLength,
            sbyte flagsLoc,
            sbyte formIDloc)
            : base(gameMode, type, headerLength, lengthLength)
        {
            this.FlagLocationOffset = flagsLoc;
            this.FormIDLocationOffset = formIDloc;
        }
    }

    public class MetaDataConstants
    {
        public GameMode GameMode { get; private set; }
        public sbyte ModHeaderLength { get; private set; }
        public sbyte ModHeaderFluffLength { get; private set; }

        public RecordConstants GroupConstants { get; private set; }
        public MajorRecordConstants MajorConstants { get; private set; }
        public RecordConstants SubConstants { get; private set; }

        public static readonly MetaDataConstants Oblivion = new MetaDataConstants()
        {
            GameMode = GameMode.Oblivion,
            ModHeaderLength = 20,
            ModHeaderFluffLength = 12,
            GroupConstants = new RecordConstants(
                GameMode.Oblivion,
                ObjectType.Group,
                headerLength: 20,
                lengthLength: 4),
            MajorConstants = new MajorRecordConstants(
                GameMode.Oblivion,
                ObjectType.Record,
                headerLength: 20,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12),
            SubConstants = new RecordConstants(
                GameMode.Oblivion,
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2)
        };

        public static readonly MetaDataConstants Skyrim = new MetaDataConstants()
        {
            GameMode = GameMode.Skyrim,
            ModHeaderLength = 24,
            ModHeaderFluffLength = 16,
            GroupConstants = new RecordConstants(
                GameMode.Skyrim,
                ObjectType.Group,
                headerLength: 24,
                lengthLength: 4),
            MajorConstants = new MajorRecordConstants(
                GameMode.Skyrim,
                ObjectType.Record,
                headerLength: 24,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12),
            SubConstants = new RecordConstants(
                GameMode.Skyrim,
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2)
        };

        public ModHeaderMeta Header(ReadOnlySpan<byte> span) => new ModHeaderMeta(this, span);
        public ModHeaderMeta GetHeader(IBinaryReadStream stream) => new ModHeaderMeta(this, stream.GetSpan(this.ModHeaderLength));
        public ModHeaderMeta ReadHeader(IBinaryReadStream stream) => new ModHeaderMeta(this, stream.ReadSpan(this.ModHeaderLength));

        public GroupRecordMeta Group(ReadOnlySpan<byte> span) => new GroupRecordMeta(this, span);
        public GroupRecordFrame GroupRecordFrame(ReadOnlySpan<byte> span) => new GroupRecordFrame(this, span);
        public GroupRecordMemoryFrame GroupRecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new GroupRecordMemoryFrame(this, span);
        public GroupRecordMeta GetGroup(IBinaryReadStream stream, int offset = 0) => new GroupRecordMeta(this, stream.GetSpan(this.GroupConstants.HeaderLength, offset));
        public GroupRecordFrame GetGroupRecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetGroup(stream, offset);
            return new GroupRecordFrame(meta, stream.GetSpan(checked((int)meta.RecordLength), offset: offset + meta.HeaderLength));
        }
        public GroupRecordMemoryFrame GetGroupRecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetGroup(stream, offset);
            return new GroupRecordMemoryFrame(meta, stream.GetMemory(checked((int)meta.RecordLength), offset: offset + meta.HeaderLength));
        }
        public GroupRecordMeta ReadGroup(IBinaryReadStream stream, int offset = 0) => new GroupRecordMeta(this, stream.ReadSpan(this.GroupConstants.HeaderLength, offset));
        public GroupRecordFrame ReadGroupRecordFrame(IBinaryReadStream stream)
        {
            var meta = ReadGroup(stream);
            return new GroupRecordFrame(meta, stream.ReadSpan(checked((int)meta.RecordLength)));
        }
        public GroupRecordMemoryFrame ReadGroupRecordMemoryFrame(BinaryMemoryReadStream stream)
        {
            var meta = ReadGroup(stream);
            return new GroupRecordMemoryFrame(meta, stream.ReadMemory(checked((int)meta.RecordLength)));
        }

        public MajorRecordMeta MajorRecord(ReadOnlySpan<byte> span) => new MajorRecordMeta(this, span);
        public MajorRecordMetaWritable MajorRecordWritable(Span<byte> span) => new MajorRecordMetaWritable(this, span);
        public MajorRecordFrame MajorRecordFrame(ReadOnlySpan<byte> span) => new MajorRecordFrame(this, span);
        public MajorRecordMemoryFrame MajorRecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new MajorRecordMemoryFrame(this, span);
        public MajorRecordMeta GetMajorRecord(IBinaryReadStream stream, int offset = 0) => new MajorRecordMeta(this, stream.GetSpan(this.MajorConstants.HeaderLength, offset));
        public MajorRecordFrame GetMajorRecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetMajorRecord(stream, offset);
            return new MajorRecordFrame(meta, stream.GetSpan(checked((int)meta.RecordLength), offset: offset + meta.HeaderLength));
        }
        public MajorRecordMemoryFrame GetMajorRecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetMajorRecord(stream, offset);
            return new MajorRecordMemoryFrame(meta, stream.GetMemory(checked((int)meta.RecordLength), offset: offset + meta.HeaderLength));
        }
        public MajorRecordMeta ReadMajorRecord(IBinaryReadStream stream) => new MajorRecordMeta(this, stream.ReadSpan(this.MajorConstants.HeaderLength));
        public MajorRecordFrame ReadMajorRecordFrame(IBinaryReadStream stream)
        {
            var meta = ReadMajorRecord(stream);
            return new MajorRecordFrame(meta, stream.ReadSpan(checked((int)meta.RecordLength)));
        }
        public MajorRecordMemoryFrame ReadMajorRecordMemoryFrame(BinaryMemoryReadStream stream)
        {
            var meta = ReadMajorRecord(stream);
            return new MajorRecordMemoryFrame(meta, stream.ReadMemory(checked((int)meta.RecordLength)));
        }

        public SubRecordMeta SubRecord(ReadOnlySpan<byte> span) => new SubRecordMeta(this, span);
        public SubRecordFrame SubRecordFrame(ReadOnlySpan<byte> span) => new SubRecordFrame(this, span);
        public SubRecordMemoryFrame SubRecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new SubRecordMemoryFrame(this, span);
        public SubRecordMeta GetSubRecord(IBinaryReadStream stream, int offset = 0) => new SubRecordMeta(this, stream.GetSpan(this.SubConstants.HeaderLength, offset));
        public SubRecordFrame GetSubRecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetSubRecord(stream, offset);
            return new SubRecordFrame(meta, stream.GetSpan(meta.RecordLength, offset: offset + meta.HeaderLength));
        }
        public SubRecordMemoryFrame GetSubRecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetSubRecord(stream, offset);
            return new SubRecordMemoryFrame(meta, stream.GetMemory(meta.RecordLength, offset: offset + meta.HeaderLength));
        }
        public SubRecordMeta ReadSubRecord(IBinaryReadStream stream) => new SubRecordMeta(this, stream.ReadSpan(this.SubConstants.HeaderLength));
        public SubRecordFrame ReadSubRecordFrame(IBinaryReadStream stream)
        {
            var meta = ReadSubRecord(stream);
            return new SubRecordFrame(meta, stream.ReadSpan(meta.RecordLength));
        }
        public SubRecordMemoryFrame ReadSubRecordMemoryFrame(BinaryMemoryReadStream stream)
        {
            var meta = ReadSubRecord(stream);
            return new SubRecordMemoryFrame(meta, stream.ReadMemory(meta.RecordLength));
        }

        public VariableHeaderMeta NextRecordVariableMeta(ReadOnlySpan<byte> span)
        {
            RecordType rec = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span));
            if (rec == Mutagen.Bethesda.Constants.GRUP)
            {
                return this.GroupConstants.VariableMeta(span);
            }
            else
            {
                return this.MajorConstants.VariableMeta(span);
            }
        }
        public VariableHeaderMeta GetNextRecordVariableMeta(IBinaryReadStream stream)
        {
            RecordType rec = new RecordType(stream.GetInt32());
            if (rec == Mutagen.Bethesda.Constants.GRUP)
            {
                return this.GroupConstants.VariableMeta(stream.GetSpan(this.GroupConstants.HeaderLength));
            }
            else
            {
                return this.MajorConstants.VariableMeta(stream.GetSpan(this.MajorConstants.HeaderLength));
            }
        }
        public VariableHeaderMeta ReadNextRecordVariableMeta(IBinaryReadStream stream)
        {
            RecordType rec = new RecordType(stream.GetInt32());
            if (rec == Mutagen.Bethesda.Constants.GRUP)
            {
                return this.GroupConstants.VariableMeta(stream.ReadSpan(this.GroupConstants.HeaderLength));
            }
            else
            {
                return this.MajorConstants.VariableMeta(stream.ReadSpan(this.MajorConstants.HeaderLength));
            }
        }

        public RecordConstants Constants(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Subrecord:
                    return SubConstants;
                case ObjectType.Record:
                    return MajorConstants;
                case ObjectType.Group:
                    return GroupConstants;
                case ObjectType.Mod:
                default:
                    throw new NotImplementedException();
            }
        }

        public static MetaDataConstants Get(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Oblivion:
                    return Oblivion;
                case GameMode.Skyrim:
                    return Skyrim;
                default:
                    throw new NotImplementedException();
            }
        }

        public static implicit operator MetaDataConstants(GameMode mode)
        {
            return Get(mode);
        }
    }

    public ref struct ModHeaderMeta
    {
        public MetaDataConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public ModHeaderMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.ModHeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public bool HasContent => this.Span.Length >= this.HeaderLength;
        public sbyte HeaderLength => Meta.ModHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public long TotalLength => this.HeaderLength + this.RecordLength;
    }

    public ref struct VariableHeaderMeta
    {
        public ReadOnlySpan<byte> Span { get; }
        public RecordConstants Constants { get; }

        public VariableHeaderMeta(RecordConstants constants, ReadOnlySpan<byte> span)
        {
            this.Constants = constants;
            this.Span = span.Slice(0, constants.HeaderLength);
        }

        public sbyte HeaderLength => this.Constants.HeaderLength;
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        public uint RecordLength
        {
            get
            {
                switch (this.Constants.LengthLength)
                {
                    case 1:
                        return this.Span[4];
                    case 2:
                        return BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
                    case 4:
                        return BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public int TypeAndLengthLength => this.Constants.TypeAndLengthLength;
        public long TotalLength => this.Constants.HeaderIncludedInLength ? this.RecordLength : (this.HeaderLength + this.RecordLength);
        public bool IsGroup => this.Constants.ObjectType == ObjectType.Group;
        public long ContentLength => this.Constants.HeaderIncludedInLength ? this.RecordLength - this.HeaderLength : this.RecordLength;
    }

    public ref struct GroupRecordMeta
    {
        public MetaDataConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public GroupRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.GroupConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.GroupConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => this.Span.Slice(8, 4);
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.ContainedRecordTypeSpan));
        public int GroupType => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(12, 4));
        public ReadOnlySpan<byte> LastModifiedSpan => this.Span.Slice(16, 4);
        public long TotalLength => this.RecordLength;
        public bool IsGroup => this.RecordType == Constants.GRUP;
        public uint ContentLength => checked((uint)(this.TotalLength - this.HeaderLength));
        public int TypeAndLengthLength => Meta.GroupConstants.TypeAndLengthLength;
    }

    public ref struct GroupRecordFrame
    {
        public GroupRecordMeta Header { get; }
        public ReadOnlySpan<byte> ContentSpan { get; }

        public GroupRecordFrame(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.Group(span);
            this.ContentSpan = span.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));
        }

        public GroupRecordFrame(GroupRecordMeta meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.ContentSpan = span;
        }
    }

    public ref struct GroupRecordMemoryFrame
    {
        public GroupRecordMeta Header { get; }
        public ReadOnlyMemorySlice<byte> ContentSpan { get; }

        public GroupRecordMemoryFrame(MetaDataConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.Group(span);
            this.ContentSpan = span.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));
        }

        public GroupRecordMemoryFrame(GroupRecordMeta meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.ContentSpan = span;
        }
    }

    public ref struct MajorRecordMeta
    {
        public MetaDataConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public MajorRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.MajorConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(8, 4));
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(12, 4)));
        public long TotalLength => this.HeaderLength + this.RecordLength;
        public bool IsCompressed => (this.MajorRecordFlags & Mutagen.Bethesda.Constants.CompressedFlag) > 0;
    }

    public ref struct MajorRecordMetaWritable
    {
        public MetaDataConstants Meta { get; }
        public Span<byte> Span { get; }

        public MajorRecordMetaWritable(MetaDataConstants meta, Span<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.MajorConstants.HeaderLength;
        public RecordType RecordType
        {
            get => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.Span.Slice(0, 4), value.TypeInt);
        }
        public uint RecordLength
        {
            get => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
            set => BinaryPrimitives.WriteUInt32LittleEndian(this.Span.Slice(4, 4), value);
        }
        public int MajorRecordFlags
        {
            get => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(8, 4));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.Span.Slice(8, 4), value);
        }
        public FormID FormID
        {
            get => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(12, 4)));
            set => BinaryPrimitives.WriteUInt32LittleEndian(this.Span.Slice(12, 4), value.Raw);
        }
        public long TotalLength => this.HeaderLength + this.RecordLength;
        public bool IsCompressed
        {
            get => (this.MajorRecordFlags & Mutagen.Bethesda.Constants.CompressedFlag) > 0;
            set
            {
                if (value)
                {
                    this.MajorRecordFlags |= Mutagen.Bethesda.Constants.CompressedFlag;
                }
                else
                {
                    this.MajorRecordFlags &= ~Mutagen.Bethesda.Constants.CompressedFlag;
                }
            }
        }
    }

    public ref struct MajorRecordFrame
    {
        public MajorRecordMeta Header { get; }
        public ReadOnlySpan<byte> ContentSpan { get; }

        public MajorRecordFrame(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.MajorRecord(span);
            this.ContentSpan = span.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));
        }

        public MajorRecordFrame(MajorRecordMeta meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.ContentSpan = span;
        }
    }

    public ref struct MajorRecordMemoryFrame
    {
        public MajorRecordMeta Header { get; }
        public ReadOnlyMemorySlice<byte> ContentSpan { get; }

        public MajorRecordMemoryFrame(MetaDataConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.MajorRecord(span);
            this.ContentSpan = span.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));
        }

        public MajorRecordMemoryFrame(MajorRecordMeta meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.ContentSpan = span;
        }
    }

    public ref struct SubRecordMeta
    {
        public MetaDataConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public SubRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.SubConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.SubConstants.HeaderLength;
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        public ushort RecordLength => BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
        public int TotalLength => this.HeaderLength + this.RecordLength;
    }

    public ref struct SubRecordFrame
    {
        public SubRecordMeta Header { get; }
        public ReadOnlySpan<byte> ContentSpan { get; }

        public SubRecordFrame(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.SubRecord(span);
            this.ContentSpan = span.Slice(this.Header.HeaderLength, this.Header.RecordLength);
        }

        public SubRecordFrame(SubRecordMeta meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.ContentSpan = span;
        }
    }

    public ref struct SubRecordMemoryFrame
    {
        public SubRecordMeta Header { get; }
        public ReadOnlyMemorySlice<byte> ContentSpan { get; }

        public SubRecordMemoryFrame(MetaDataConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.SubRecord(span);
            this.ContentSpan = span.Slice(this.Header.HeaderLength, this.Header.RecordLength);
        }

        public SubRecordMemoryFrame(SubRecordMeta meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.ContentSpan = span;
        }
    }
}
