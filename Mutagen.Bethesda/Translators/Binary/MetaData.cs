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
    }

    public class MajorRecordConstants : RecordConstants
    {
        public sbyte FlagLocation { get; }

        public MajorRecordConstants(
            GameMode gameMode,
            ObjectType type, 
            sbyte headerLength,
            sbyte lengthLength,
            sbyte flagsLoc) 
            : base(gameMode, type, headerLength, lengthLength)
        {
            this.FlagLocation = flagsLoc;
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
                flagsLoc: 8),
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
                flagsLoc: 8),
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
        public GroupRecordMeta GetGroup(IBinaryReadStream stream) => new GroupRecordMeta(this, stream.GetSpan(this.GroupConstants.HeaderLength));
        public GroupRecordMeta ReadGroup(IBinaryReadStream stream) => new GroupRecordMeta(this, stream.ReadSpan(this.GroupConstants.HeaderLength));
        public MajorRecordMeta MajorRecord(ReadOnlySpan<byte> span) => new MajorRecordMeta(this, span);
        public MajorRecordMeta GetMajorRecord(IBinaryReadStream stream) => new MajorRecordMeta(this, stream.GetSpan(this.MajorConstants.HeaderLength));
        public MajorRecordMeta ReadMajorRecord(IBinaryReadStream stream) => new MajorRecordMeta(this, stream.ReadSpan(this.MajorConstants.HeaderLength));
        public SubRecordMeta SubRecord(ReadOnlySpan<byte> span) => new SubRecordMeta(this, span);
        public SubRecordMeta GetSubRecord(IBinaryReadStream stream) => new SubRecordMeta(this, stream.GetSpan(this.SubConstants.HeaderLength));
        public SubRecordMeta ReadSubRecord(IBinaryReadStream stream) => new SubRecordMeta(this, stream.ReadSpan(this.SubConstants.HeaderLength));

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
        private MetaDataConstants meta;
        public ReadOnlySpan<byte> Span { get; }

        public ModHeaderMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.Span = span.Slice(0, meta.ModHeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public bool HasContent => this.Span.Length >= this.HeaderLength;
        public sbyte HeaderLength => meta.ModHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public long TotalLength => this.HeaderLength + this.RecordLength;
    }

    public ref struct GroupRecordMeta
    {
        private MetaDataConstants meta;
        public ReadOnlySpan<byte> Span { get; }

        public GroupRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.Span = span.Slice(0, meta.GroupConstants.HeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public sbyte HeaderLength => meta.GroupConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => this.Span.Slice(8, 4);
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.ContainedRecordTypeSpan));
        public int GroupType => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(12, 4));
        public ReadOnlySpan<byte> LastModifiedSpan => this.Span.Slice(16, 4);
        public long TotalLength => this.RecordLength;
        public bool IsGroup => this.RecordType == Constants.GRUP;
        public uint ContentLength => checked((uint)(this.TotalLength - this.HeaderLength));
        public int TypeAndLengthLength => meta.GroupConstants.TypeAndLengthLength;
    }

    public ref struct MajorRecordMeta
    {
        private MetaDataConstants meta;
        public ReadOnlySpan<byte> Span { get; }

        public MajorRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.Span = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public sbyte HeaderLength => meta.MajorConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(8, 4));
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(12, 4)));
        public long TotalLength => this.HeaderLength + this.RecordLength;
        public bool IsCompressed => (this.MajorRecordFlags & Mutagen.Bethesda.Constants.CompressedFlag) > 0;
    }

    public ref struct SubRecordMeta
    {
        private MetaDataConstants meta;
        public ReadOnlySpan<byte> Span { get; }

        public SubRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.Span = span.Slice(0, meta.SubConstants.HeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public sbyte HeaderLength => meta.SubConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public ushort RecordLength => BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
        public int TotalLength => this.HeaderLength + this.RecordLength;
    }
}
