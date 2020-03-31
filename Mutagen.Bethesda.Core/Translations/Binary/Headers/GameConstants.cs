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

        #region Header Factories
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

        public SubrecordHeader Subrecord(ReadOnlySpan<byte> span) => new SubrecordHeader(this, span);
        public SubrecordFrame SubrecordFrame(ReadOnlySpan<byte> span) => new SubrecordFrame(this, span);
        public SubrecordMemoryFrame SubrecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new SubrecordMemoryFrame(this, span);
        public SubrecordHeader GetSubrecord(IBinaryReadStream stream, int offset = 0) => new SubrecordHeader(this, stream.GetSpan(this.SubConstants.HeaderLength, offset));
        public bool TryGetSubrecord(IBinaryReadStream stream, out SubrecordHeader meta)
        {
            if (stream.Remaining < SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = GetSubrecord(stream);
            return true;
        }
        public bool TryGetSubrecord(IBinaryReadStream stream, RecordType targetType, out SubrecordHeader meta)
        {
            if (stream.Remaining < SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = GetSubrecord(stream);
            return targetType == meta.RecordType;
        }
        public SubrecordFrame GetSubrecordFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetSubrecord(stream, offset);
            return new SubrecordFrame(meta, stream.GetSpan(meta.TotalLength, offset: offset));
        }
        public bool TryGetSubrecordFrame(IBinaryReadStream stream, out SubrecordFrame frame)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.GetSpan(meta.TotalLength));
            return true;
        }
        public bool TryGetSubrecordFrame(IBinaryReadStream stream, RecordType targetType, out SubrecordFrame frame)
        {
            if (!TryGetSubrecord(stream, targetType, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.GetSpan(meta.TotalLength));
            return true;
        }
        public SubrecordMemoryFrame GetSubrecordMemoryFrame(BinaryMemoryReadStream stream, int offset = 0)
        {
            var meta = GetSubrecord(stream, offset);
            return new SubrecordMemoryFrame(meta, stream.GetMemory(meta.TotalLength, offset: offset));
        }
        public SubrecordHeader ReadSubrecord(IBinaryReadStream stream) => new SubrecordHeader(this, stream.ReadSpan(this.SubConstants.HeaderLength));
        public SubrecordHeader ReadSubrecord(IBinaryReadStream stream, RecordType targetType)
        {
            var meta = ReadSubrecord(stream);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return meta;
        }
        public bool TryReadSubrecord(IBinaryReadStream stream, out SubrecordHeader meta)
        {
            if (stream.Remaining < SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = ReadSubrecord(stream);
            return true;
        }
        public bool TryReadSubrecord(IBinaryReadStream stream, RecordType targetType, out SubrecordHeader meta)
        {
            if (stream.Remaining < SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = ReadSubrecord(stream);
            if (meta.RecordType != targetType)
            {
                stream.Position -= meta.HeaderLength;
                return false;
            }
            return true;
        }
        public SubrecordFrame ReadSubrecordFrame(IBinaryReadStream stream)
        {
            var meta = GetSubrecord(stream);
            return new SubrecordFrame(meta, stream.ReadSpan(meta.TotalLength));
        }
        public SubrecordFrame ReadSubrecordFrame(IBinaryReadStream stream, RecordType targetType)
        {
            var meta = GetSubrecord(stream);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return new SubrecordFrame(meta, stream.ReadSpan(meta.TotalLength));
        }
        public bool TryReadSubrecordFrame(IBinaryReadStream stream, out SubrecordFrame frame)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.ReadSpan(meta.TotalLength));
            return true;
        }
        public bool TryReadSubrecordFrame(IBinaryReadStream stream, RecordType targetType, out SubrecordFrame frame)
        {
            if (!TryGetSubrecord(stream, targetType, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.ReadSpan(meta.TotalLength));
            return true;
        }
        public SubrecordMemoryFrame ReadSubrecordMemoryFrame(IBinaryReadStream stream)
        {
            var meta = GetSubrecord(stream);
            return new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength));
        }
        public SubrecordMemoryFrame ReadSubrecordMemoryFrame(IBinaryReadStream stream, RecordType targetType)
        {
            var meta = GetSubrecord(stream);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength));
        }
        public bool TryReadSubrecordMemoryFrame(IBinaryReadStream stream, out SubrecordMemoryFrame frame)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength));
            return true;
        }
        public bool TryReadSubrecordMemoryFrame(IBinaryReadStream stream, RecordType targetType, out SubrecordMemoryFrame frame)
        {
            if (!TryGetSubrecord(stream, targetType, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength));
            return true;
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
        #endregion

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

    public static class GameConstantsExt
    {
        public static ModHeader GetHeader(this IMutagenReadStream stream) => stream.MetaData.GetHeader(stream);
        public static ModHeader ReadHeader(this IMutagenReadStream stream) => stream.MetaData.ReadHeader(stream);

        public static GroupHeader GetGroup(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.GetGroup(stream, offset);
        public static GroupFrame GetGroupRecordFrame(IMutagenReadStream stream, int offset = 0) => stream.MetaData.GetGroupRecordFrame(stream, offset);
        public static GroupHeader ReadGroup(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.ReadGroup(stream, offset);
        public static GroupFrame ReadGroupRecordFrame(this IMutagenReadStream stream) => stream.MetaData.ReadGroupRecordFrame(stream);

        public static MajorRecordHeader GetMajorRecord(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.GetMajorRecord(stream, offset);
        public static MajorRecordFrame GetMajorRecordFrame(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.GetMajorRecordFrame(stream, offset);
        public static MajorRecordHeader ReadMajorRecord(this IMutagenReadStream stream) => stream.MetaData.ReadMajorRecord(stream);
        public static MajorRecordFrame ReadMajorRecordFrame(this IMutagenReadStream stream) => stream.MetaData.ReadMajorRecordFrame(stream);

        public static SubrecordHeader GetSubrecord(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.GetSubrecord(stream, offset);
        public static bool TryGetSubrecord(this IMutagenReadStream stream, out SubrecordHeader meta) => stream.MetaData.TryGetSubrecord(stream, out meta);
        public static bool TryGetSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader meta) => stream.MetaData.TryGetSubrecord(stream, targetType, out meta);
        public static SubrecordFrame GetSubrecordFrame(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.GetSubrecordFrame(stream, offset);
        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame) => stream.MetaData.TryGetSubrecordFrame(stream, out frame);
        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame) => stream.MetaData.TryGetSubrecordFrame(stream, targetType, out frame);
        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream) => stream.MetaData.ReadSubrecord(stream);
        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream, RecordType targetType) => stream.MetaData.ReadSubrecord(stream, targetType);
        public static bool TryReadSubrecord(this IMutagenReadStream stream, out SubrecordHeader meta) => stream.MetaData.TryReadSubrecord(stream, out meta);
        public static bool TryReadSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader meta) => stream.MetaData.TryReadSubrecord(stream, targetType, out meta);
        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream) => stream.MetaData.ReadSubrecordFrame(stream);
        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType) => stream.MetaData.ReadSubrecordFrame(stream, targetType);
        public static SubrecordMemoryFrame ReadSubrecordMemoryFrame(this IMutagenReadStream stream) => stream.MetaData.ReadSubrecordMemoryFrame(stream);
        public static SubrecordMemoryFrame ReadSubrecordMemoryFrame(this IMutagenReadStream stream, RecordType targetType) => stream.MetaData.ReadSubrecordMemoryFrame(stream, targetType);
        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame) => stream.MetaData.TryReadSubrecordFrame(stream, out frame);
        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame) => stream.MetaData.TryReadSubrecordFrame(stream, targetType, out frame);
        public static bool TryReadSubrecordMemoryFrame(this IMutagenReadStream stream, out SubrecordMemoryFrame frame) => stream.MetaData.TryReadSubrecordMemoryFrame(stream, out frame);
        public static bool TryReadSubrecordMemoryFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordMemoryFrame frame) => stream.MetaData.TryReadSubrecordMemoryFrame(stream, targetType, out frame);

        public static VariableHeader GetNextRecordVariableMeta(this IMutagenReadStream stream) => stream.MetaData.GetNextRecordVariableMeta(stream);
        public static VariableHeader ReadNextRecordVariableMeta(this IMutagenReadStream stream) => stream.MetaData.ReadNextRecordVariableMeta(stream);
    }
}
