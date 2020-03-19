using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class GameConstants
    {
        public GameMode GameMode { get; }
        public sbyte ModHeaderLength { get; }
        public sbyte ModHeaderFluffLength { get; }

        public RecordHeaderConstants GroupConstants { get; }
        public MajorRecordConstants MajorConstants { get; }
        public RecordHeaderConstants SubConstants { get; }

        public GameConstants(
            GameMode gameMode,
            sbyte modHeaderLength,
            sbyte modHeaderFluffLength,
            RecordHeaderConstants groupConstants,
            MajorRecordConstants majorConstants,
            RecordHeaderConstants subConstants)
        {
            GameMode = gameMode;
            ModHeaderLength = modHeaderLength;
            ModHeaderFluffLength = modHeaderFluffLength;
            GroupConstants = groupConstants;
            MajorConstants = majorConstants;
            SubConstants = subConstants;
        }

        public static readonly GameConstants Oblivion = new GameConstants(
            gameMode: GameMode.Oblivion,
            modHeaderLength: 20,
            modHeaderFluffLength: 12,
            groupConstants: new RecordHeaderConstants(
                GameMode.Oblivion,
                ObjectType.Group,
                headerLength: 20,
                lengthLength: 4),
            majorConstants: new MajorRecordConstants(
                GameMode.Oblivion,
                ObjectType.Record,
                headerLength: 20,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12),
            subConstants: new RecordHeaderConstants(
                GameMode.Oblivion,
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2));

        public static readonly GameConstants Skyrim = new GameConstants(
            gameMode: GameMode.Skyrim,
            modHeaderLength: 24,
            modHeaderFluffLength: 16,
            groupConstants: new RecordHeaderConstants(
                GameMode.Skyrim,
                ObjectType.Group,
                headerLength: 24,
                lengthLength: 4),
            majorConstants: new MajorRecordConstants(
                GameMode.Skyrim,
                ObjectType.Record,
                headerLength: 24,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12),
            subConstants: new RecordHeaderConstants(
                GameMode.Skyrim,
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2));

        public ModHeader Header(ReadOnlySpan<byte> span) => new ModHeader(this, span);
        public ModHeader GetHeader(IBinaryReadStream stream) => new ModHeader(this, stream.GetSpan(this.ModHeaderLength));
        public ModHeader ReadHeader(IBinaryReadStream stream) => new ModHeader(this, stream.ReadSpan(this.ModHeaderLength));

        public GroupHeader Group(ReadOnlySpan<byte> span) => new GroupHeader(this, span);
        public GroupFrame GroupRecordFrame(ReadOnlySpan<byte> span) => new GroupFrame(this, span);
        public GroupMemoryFrame GroupRecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new GroupMemoryFrame(this, span);
        public GroupHeader GetGroup(IBinaryReadStream stream, int offset = 0) => new GroupHeader(this, stream.GetSpan(this.GroupConstants.HeaderLength, offset));
        public GroupFrame GetGroupRecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetGroup(stream, offset);
            return new GroupFrame(meta, stream.GetSpan(checked((int)meta.TotalLength), offset: offset));
        }
        public GroupMemoryFrame GetGroupRecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetGroup(stream, offset);
            return new GroupMemoryFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset));
        }
        public GroupHeader ReadGroup(IBinaryReadStream stream, int offset = 0) => new GroupHeader(this, stream.ReadSpan(this.GroupConstants.HeaderLength, offset));
        public GroupFrame ReadGroupRecordFrame(IBinaryReadStream stream)
        {
            var meta = GetGroup(stream);
            return new GroupFrame(meta, stream.ReadSpan(checked((int)meta.TotalLength)));
        }
        public GroupMemoryFrame ReadGroupRecordMemoryFrame(BinaryMemoryReadStream stream)
        {
            var meta = GetGroup(stream);
            return new GroupMemoryFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength)));
        }

        public MajorRecordHeader MajorRecord(ReadOnlySpan<byte> span) => new MajorRecordHeader(this, span);
        public MajorRecordHeaderWritable MajorRecordWritable(Span<byte> span) => new MajorRecordHeaderWritable(this, span);
        public MajorRecordFrame MajorRecordFrame(ReadOnlySpan<byte> span) => new MajorRecordFrame(this, span);
        public MajorRecordMemoryFrame MajorRecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new MajorRecordMemoryFrame(this, span);
        public MajorRecordHeader GetMajorRecord(IBinaryReadStream stream, int offset = 0) => new MajorRecordHeader(this, stream.GetSpan(this.MajorConstants.HeaderLength, offset));
        public MajorRecordFrame GetMajorRecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetMajorRecord(stream, offset);
            return new MajorRecordFrame(meta, stream.GetSpan(checked((int)meta.TotalLength), offset: offset));
        }
        public MajorRecordMemoryFrame GetMajorRecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetMajorRecord(stream, offset);
            return new MajorRecordMemoryFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset));
        }
        public MajorRecordHeader ReadMajorRecord(IBinaryReadStream stream) => new MajorRecordHeader(this, stream.ReadSpan(this.MajorConstants.HeaderLength));
        public MajorRecordFrame ReadMajorRecordFrame(IBinaryReadStream stream)
        {
            var meta = GetMajorRecord(stream);
            return new MajorRecordFrame(meta, stream.ReadSpan(checked((int)meta.TotalLength)));
        }
        public MajorRecordMemoryFrame ReadMajorRecordMemoryFrame(BinaryMemoryReadStream stream)
        {
            var meta = GetMajorRecord(stream);
            return new MajorRecordMemoryFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength)));
        }

        public SubRecordHeader SubRecord(ReadOnlySpan<byte> span) => new SubRecordHeader(this, span);
        public SubRecordFrame SubRecordFrame(ReadOnlySpan<byte> span) => new SubRecordFrame(this, span);
        public SubRecordMemoryFrame SubRecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new SubRecordMemoryFrame(this, span);
        public SubRecordHeader GetSubRecord(IBinaryReadStream stream, int offset = 0) => new SubRecordHeader(this, stream.GetSpan(this.SubConstants.HeaderLength, offset));
        public bool TryGetSubrecord(IBinaryReadStream stream, out SubRecordHeader meta)
        {
            if (stream.Remaining < SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = GetSubRecord(stream);
            return true;
        }
        public SubRecordFrame GetSubRecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetSubRecord(stream, offset);
            return new SubRecordFrame(meta, stream.GetSpan(meta.TotalLength, offset: offset));
        }
        public bool TryGetSubrecordFrame(IBinaryReadStream stream, out SubRecordFrame frame)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubRecordFrame(meta, stream.GetSpan(meta.TotalLength));
            return true;
        }
        public SubRecordMemoryFrame GetSubRecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetSubRecord(stream, offset);
            return new SubRecordMemoryFrame(meta, stream.GetMemory(meta.TotalLength, offset: offset));
        }
        public SubRecordHeader ReadSubRecord(IBinaryReadStream stream) => new SubRecordHeader(this, stream.ReadSpan(this.SubConstants.HeaderLength));
        public bool TryReadSubrecord(IBinaryReadStream stream, out SubRecordHeader meta)
        {
            if (stream.Remaining < SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = ReadSubRecord(stream);
            return true;
        }
        public SubRecordFrame ReadSubRecordFrame(IBinaryReadStream stream)
        {
            var meta = GetSubRecord(stream);
            return new SubRecordFrame(meta, stream.ReadSpan(meta.TotalLength));
        }
        public bool TryReadSubrecordFrame(IBinaryReadStream stream, out SubRecordFrame frame)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubRecordFrame(meta, stream.ReadSpan(meta.TotalLength));
            return true;
        }
        public SubRecordMemoryFrame ReadSubRecordMemoryFrame(BinaryMemoryReadStream stream)
        {
            var meta = GetSubRecord(stream);
            return new SubRecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength));
        }

        public VariableHeader NextRecordVariableMeta(ReadOnlySpan<byte> span)
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
        public VariableHeader GetNextRecordVariableMeta(IBinaryReadStream stream)
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
        public VariableHeader ReadNextRecordVariableMeta(IBinaryReadStream stream)
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

        public RecordHeaderConstants Constants(ObjectType type)
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

        public static GameConstants Get(GameMode mode)
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

        public static implicit operator GameConstants(GameMode mode)
        {
            return Get(mode);
        }
    }
}
