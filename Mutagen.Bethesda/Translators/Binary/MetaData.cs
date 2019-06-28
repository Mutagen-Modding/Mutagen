using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class MetaDataConstants
    {
        public sbyte ModHeaderFluffLength { get; private set; }
        public sbyte GroupHeaderLength { get; private set; }
        public sbyte MajorRecordHeaderLength { get; private set; }
        public sbyte GroupMetaLengthAfterType { get; private set; }
        public sbyte RecordMetaLengthAfterRecordLength { get; private set; }

        public static readonly MetaDataConstants Oblivion = new MetaDataConstants()
        {
            ModHeaderFluffLength = 12,
            GroupHeaderLength = 20,
            MajorRecordHeaderLength = 20,
            GroupMetaLengthAfterType = 4,
            RecordMetaLengthAfterRecordLength = 12,
        };

        public static readonly MetaDataConstants Skyrim = new MetaDataConstants()
        {
            ModHeaderFluffLength = 16,
            GroupHeaderLength = 24,
            MajorRecordHeaderLength = 20,
            GroupMetaLengthAfterType = 8,
            RecordMetaLengthAfterRecordLength = 16,
        };

        public GroupRecordMeta Group(ReadOnlySpan<byte> span) => new GroupRecordMeta(this, span);
        public MajorRecordMeta MajorRecord(ReadOnlySpan<byte> span) => new MajorRecordMeta(this, span);
        public GroupRecordMeta Group(IMutagenReadStream stream) => new GroupRecordMeta(this, stream.GetSpan(this.GroupHeaderLength));
        public MajorRecordMeta MajorRecord(IMutagenReadStream stream) => new MajorRecordMeta(this, stream.GetSpan(this.MajorRecordHeaderLength));

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

        public sbyte HeaderLength => meta.GroupHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8, 4)));
        public int GroupFlags => BinaryPrimitives.ReadInt32LittleEndian(span.Slice(12, 4));
        public uint TotalLength => this.RecordLength;
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

        public sbyte HeaderLength => meta.MajorRecordHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(span.Slice(8, 4));
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(12, 4)));
        public uint TotalLength => checked((uint)(this.HeaderLength + this.RecordLength));
    }
}
