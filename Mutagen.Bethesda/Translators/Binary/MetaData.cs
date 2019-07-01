using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MetaDataConstants
    {
        public GameMode GameMode { get; private set; }
        public sbyte ModHeaderLength { get; private set; }
        public sbyte ModHeaderFluffLength { get; private set; }
        public sbyte GroupHeaderLength { get; private set; }
        public sbyte MajorRecordHeaderLength { get; private set; }
        public sbyte SubRecordHeaderLength { get; private set; }
        public sbyte RecordMetaLengthAfterRecordLength { get; private set; }

        public static readonly MetaDataConstants Oblivion = new MetaDataConstants()
        {
            GameMode = GameMode.Oblivion,
            ModHeaderLength = 20,
            ModHeaderFluffLength = 12,
            GroupHeaderLength = 20,
            MajorRecordHeaderLength = 20,
            SubRecordHeaderLength = 6,
            RecordMetaLengthAfterRecordLength = 12,
        };

        public static readonly MetaDataConstants Skyrim = new MetaDataConstants()
        {
            GameMode = GameMode.Skyrim,
            ModHeaderLength = 24,
            ModHeaderFluffLength = 16,
            GroupHeaderLength = 24,
            MajorRecordHeaderLength = 24,
            SubRecordHeaderLength = 6,
            RecordMetaLengthAfterRecordLength = 16,
        };

        public ModHeaderMeta Header(ReadOnlySpan<byte> span) => new ModHeaderMeta(this, span);
        public ModHeaderMeta Header(IMutagenReadStream stream) => new ModHeaderMeta(this, stream.GetSpan(this.ModHeaderLength));
        public GroupRecordMeta Group(ReadOnlySpan<byte> span) => new GroupRecordMeta(this, span);
        public GroupRecordMeta Group(IMutagenReadStream stream) => new GroupRecordMeta(this, stream.GetSpan(this.GroupHeaderLength));
        public MajorRecordMeta MajorRecord(ReadOnlySpan<byte> span) => new MajorRecordMeta(this, span);
        public MajorRecordMeta MajorRecord(IMutagenReadStream stream) => new MajorRecordMeta(this, stream.GetSpan(this.MajorRecordHeaderLength));
        public SubRecordMeta SubRecord(ReadOnlySpan<byte> span) => new SubRecordMeta(this, span);
        public SubRecordMeta SubRecord(IMutagenReadStream stream) => new SubRecordMeta(this, stream.GetSpan(this.MajorRecordHeaderLength));

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
        private ReadOnlySpan<byte> span;

        public ModHeaderMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.span = span.Slice(0, meta.GroupHeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public bool HasContent => span.Length >= this.HeaderLength;
        public sbyte HeaderLength => meta.ModHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
        public long TotalLength => this.HeaderLength + this.RecordLength;
    }

    public ref struct GroupRecordMeta
    {
        private MetaDataConstants meta;
        private ReadOnlySpan<byte> span;

        public GroupRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.span = span.Slice(0, meta.GroupHeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public sbyte HeaderLength => meta.GroupHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8, 4)));
        public int GroupFlags => BinaryPrimitives.ReadInt32LittleEndian(span.Slice(12, 4));
        public long TotalLength => this.RecordLength;
        public bool IsGroup => this.RecordType == Constants.GRUP;
        public uint ContentLength => checked((uint)(this.TotalLength - this.HeaderLength));
    }

    public ref struct MajorRecordMeta
    {
        private MetaDataConstants meta;
        private ReadOnlySpan<byte> span;

        public MajorRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.span = span.Slice(0, meta.MajorRecordHeaderLength);
        }

        public GameMode GameMode => meta.GameMode;
        public sbyte HeaderLength => meta.MajorRecordHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8, 4));
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(12, 4)));
        public long TotalLength => this.HeaderLength + this.RecordLength;
    }

    public ref struct SubRecordMeta
    {
        private MetaDataConstants meta;
        private ReadOnlySpan<byte> span;

        public SubRecordMeta(MetaDataConstants meta, ReadOnlySpan<byte> span)
        {
            this.meta = meta;
            this.span = span.Slice(0, 8);
        }

        public GameMode GameMode => meta.GameMode;
        public sbyte HeaderLength => meta.SubRecordHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4)));
        public ushort RecordLength => BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(4, 2));
        public long TotalLength => this.HeaderLength + this.RecordLength;
    }
}
