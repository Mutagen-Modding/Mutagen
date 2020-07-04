using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Reference for all the alignment and length constants related to a specific game
    /// </summary>
    public class GameConstants
    {
        /// <summary>
        /// Associated game type
        /// </summary>
        public GameMode GameMode { get; }
        
        /// <summary>
        /// Length of the Mod header's metadata, excluding content
        /// </summary>
        public sbyte ModHeaderLength { get; }
        
        /// <summary>
        /// Length of the Mod header's non-fundamental metadata
        /// </summary>
        public sbyte ModHeaderFluffLength { get; }

        /// <summary>
        /// Group constants
        /// </summary>
        public RecordHeaderConstants GroupConstants { get; }
        
        /// <summary>
        /// Major Record constants
        /// </summary>
        public MajorRecordConstants MajorConstants { get; }
        
        /// <summary>
        /// Sub Record constants
        /// </summary>
        public RecordHeaderConstants SubConstants { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameMode">GameMode to associate with the constants</param>
        /// <param name="modHeaderLength">Length of the ModHeader</param>
        /// <param name="modHeaderFluffLength">Length of the ModHeader excluding initial recordtype and length bytes.</param>
        /// <param name="groupConstants">Constants defining Groups</param>
        /// <param name="majorConstants">Constants defining Major Records</param>
        /// <param name="subConstants">Constants defining Sub Records</param>
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

        /// <summary>
        /// Readonly singleton of Oblivion game constants
        /// </summary>
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
                headerLength: 20,
                lengthLength: 4,
                flagsLoc: 8,
                formIDloc: 12),
            subConstants: new RecordHeaderConstants(
                GameMode.Oblivion,
                ObjectType.Subrecord,
                headerLength: 6,
                lengthLength: 2));

        /// <summary>
        /// Readonly singleton of Skyrim LE game constants
        /// </summary>
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
        public ModHeader Mod(ReadOnlySpan<byte> span) => new ModHeader(this, span);
        public ModHeader GetMod(IBinaryReadStream stream) => new ModHeader(this, stream.GetSpan(this.ModHeaderLength));
        public ModHeader ReadMod(IBinaryReadStream stream) => new ModHeader(this, stream.ReadSpan(this.ModHeaderLength));
        public bool TryGetMod(IBinaryReadStream stream, out ModHeader header)
        {
            if (stream.Remaining < this.ModHeaderLength)
            {
                header = default;
                return false;
            }
            header = new ModHeader(this, stream.GetSpan(this.ModHeaderLength));
            return true;
        }
        public bool TryReadMod(IBinaryReadStream stream, out ModHeader header)
        {
            if (stream.Remaining < this.ModHeaderLength)
            {
                header = default;
                return false;
            }
            header = new ModHeader(this, stream.ReadSpan(this.ModHeaderLength));
            return true;
        }

        public GroupHeader Group(ReadOnlySpan<byte> span) => new GroupHeader(this, span);
        public GroupFrame GroupFrame(ReadOnlySpan<byte> span) => new GroupFrame(this, span);
        public GroupMemoryFrame GroupMemoryFrame(ReadOnlyMemorySlice<byte> span) => new GroupMemoryFrame(this, span);
        public GroupHeader GetGroup(IBinaryReadStream stream, int offset = 0) => new GroupHeader(this, stream.GetSpan(this.GroupConstants.HeaderLength, offset));
        public bool TryGetGroup(IBinaryReadStream stream, out GroupHeader meta, int offset = 0, bool checkIsGroup = true)
        {
            if (stream.Remaining < GroupConstants.HeaderLength + offset)
            {
                meta = default;
                return false;
            }
            meta = GetGroup(stream, offset);
            return !checkIsGroup || meta.IsGroup;
        }
        public GroupFrame GetGroupFrame(IBinaryReadStream stream, int offset = 0)
        {
            var meta = GetGroup(stream, offset);
            return new GroupFrame(meta, stream.GetSpan(checked((int)meta.TotalLength), offset: offset));
        }
        public bool TryGetGroupFrame(IBinaryReadStream stream, out GroupFrame frame, int offset = 0, bool checkIsGroup = true)
        {
            if (!TryGetGroup(stream, out var meta, offset: offset, checkIsGroup: checkIsGroup))
            {
                frame = default;
                return false;
            }
            frame = new GroupFrame(meta, stream.GetSpan(checked((int)meta.TotalLength)));
            return true;
        }
        public GroupMemoryFrame GetGroupMemoryFrame(IBinaryReadStream stream, int offset = 0, bool readSafe = false)
        {
            var meta = GetGroup(stream, offset);
            return new GroupMemoryFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset, readSafe: readSafe));
        }
        public bool TryGetGroupMemoryFrame(IBinaryReadStream stream, out GroupMemoryFrame frame, int offset = 0, bool checkIsGroup = true, bool readSafe = false)
        {
            if (!TryGetGroup(stream, out var meta, offset: offset, checkIsGroup: checkIsGroup))
            {
                frame = default;
                return false;
            }
            frame = new GroupMemoryFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }
        public GroupHeader ReadGroup(IBinaryReadStream stream, int offset = 0) => new GroupHeader(this, stream.ReadSpan(this.GroupConstants.HeaderLength, offset));
        public bool TryReadGroup(IBinaryReadStream stream, out GroupHeader header, bool checkIsGroup = true)
        {
            if (stream.Remaining < GroupConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            header = ReadGroup(stream);
            return !checkIsGroup || header.IsGroup;
        }
        public GroupFrame ReadGroupFrame(IBinaryReadStream stream)
        {
            var meta = GetGroup(stream);
            return new GroupFrame(meta, stream.ReadSpan(checked((int)meta.TotalLength)));
        }
        public bool TryReadGroupFrame(IBinaryReadStream stream, out GroupFrame frame, bool checkIsGroup = true)
        {
            if (!TryGetGroup(stream, out var meta, checkIsGroup: checkIsGroup))
            {
                frame = default;
                return false;
            }
            frame = new GroupFrame(meta, stream.ReadSpan(checked((int)meta.TotalLength)));
            return true;
        }
        public GroupMemoryFrame ReadGroupMemoryFrame(IBinaryReadStream stream, bool readSafe = true)
        {
            var meta = GetGroup(stream);
            return new GroupMemoryFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe));
        }
        public bool TryReadGroupMemoryFrame(IBinaryReadStream stream, out GroupMemoryFrame frame, bool checkIsGroup = true, bool readSafe = true)
        {
            if (!TryGetGroup(stream, out var meta, checkIsGroup: checkIsGroup))
            {
                frame = default;
                return false;
            }
            frame = new GroupMemoryFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe));
            return true;
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
        public MajorRecordMemoryFrame ReadMajorRecordMemoryFrame(IBinaryReadStream stream, bool readSafe = false)
        {
            var meta = GetMajorRecord(stream);
            return new MajorRecordMemoryFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        public SubrecordHeader Subrecord(ReadOnlySpan<byte> span) => new SubrecordHeader(this, span);
        public SubrecordHeader Subrecord(ReadOnlySpan<byte> span, RecordType targetType)
        {
            var meta = new SubrecordHeader(this, span);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return meta;
        }
        public SubrecordFrame SubrecordFrame(ReadOnlySpan<byte> span) => new SubrecordFrame(this, span);
        public SubrecordFrame SubrecordFrame(ReadOnlySpan<byte> span, RecordType targetType)
        {
            var meta = new SubrecordHeader(this, span);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return new SubrecordFrame(meta, span);
        }
        public SubrecordMemoryFrame SubrecordMemoryFrame(ReadOnlyMemorySlice<byte> span) => new SubrecordMemoryFrame(this, span);
        public SubrecordHeader GetSubrecord(IBinaryReadStream stream, int offset = 0) => new SubrecordHeader(this, stream.GetSpan(this.SubConstants.HeaderLength, offset));
        public bool TryGetSubrecord(IBinaryReadStream stream, out SubrecordHeader meta, int offset = 0)
        {
            if (stream.Remaining < SubConstants.HeaderLength + offset)
            {
                meta = default;
                return false;
            }
            meta = GetSubrecord(stream, offset);
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
        public SubrecordMemoryFrame GetSubrecordMemoryFrame(IBinaryReadStream stream, int offset = 0, bool readSafe = true)
        {
            var meta = GetSubrecord(stream, offset);
            return new SubrecordMemoryFrame(meta, stream.GetMemory(meta.TotalLength, offset: offset, readSafe));
        }
        public bool TryGetSubrecordMemoryFrame(IBinaryReadStream stream, out SubrecordMemoryFrame frame, bool readSafe = true)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordMemoryFrame(meta, stream.GetMemory(meta.TotalLength, readSafe));
            return true;
        }
        public bool TryGetSubrecordMemoryFrame(IBinaryReadStream stream, RecordType targetType, out SubrecordMemoryFrame frame, bool readSafe = true)
        {
            if (!TryGetSubrecord(stream, targetType, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordMemoryFrame(meta, stream.GetMemory(meta.TotalLength, readSafe));
            return true;
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
        public SubrecordMemoryFrame ReadSubrecordMemoryFrame(IBinaryReadStream stream, bool readSafe = false)
        {
            var meta = GetSubrecord(stream);
            return new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
        }
        public SubrecordMemoryFrame ReadSubrecordMemoryFrame(IBinaryReadStream stream, RecordType targetType, bool readSafe = false)
        {
            var meta = GetSubrecord(stream);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
        }
        public bool TryReadSubrecordMemoryFrame(IBinaryReadStream stream, out SubrecordMemoryFrame frame, bool readSafe = false)
        {
            if (!TryGetSubrecord(stream, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }
        public bool TryReadSubrecordMemoryFrame(IBinaryReadStream stream, RecordType targetType, out SubrecordMemoryFrame frame, bool readSafe = false)
        {
            if (!TryGetSubrecord(stream, targetType, out var meta))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordMemoryFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        public VariableHeader NextRecordVariableMeta(ReadOnlySpan<byte> span)
        {
            RecordType rec = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span));
            if (rec == Mutagen.Bethesda.Internals.Constants.Group)
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
            if (rec == Mutagen.Bethesda.Internals.Constants.Group)
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
            if (rec == Mutagen.Bethesda.Internals.Constants.Group)
            {
                return this.GroupConstants.VariableMeta(stream.ReadSpan(this.GroupConstants.HeaderLength));
            }
            else
            {
                return this.MajorConstants.VariableMeta(stream.ReadSpan(this.MajorConstants.HeaderLength));
            }
        }
        #endregion

        /// <summary>
        /// Returns record constants related to a certain ObjectType
        /// </summary>
        /// <param name="type">ObjectType to query</param>
        /// <returns>Record Constants associated with type</returns>
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

        /// <summary>
        /// Returns GameConstant readonly singleton associated with a GameMode 
        /// </summary>
        /// <param name="mode">GameMode to query</param>
        /// <returns>GameConstant readonly singleton associated with mode</returns>
        public static GameConstants Get(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Oblivion:
                    return Oblivion;
                case GameMode.Skyrim:
                case GameMode.SkyrimSpecialEdition:
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
        public static ModHeader GetMod(this IMutagenReadStream stream) => stream.MetaData.Constants.GetMod(stream);
        public static ModHeader ReadMod(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadMod(stream);
        public static bool TryGetMod(this IMutagenReadStream stream, out ModHeader header) => stream.MetaData.Constants.TryGetMod(stream, out header);
        public static bool TryReadMod(this IMutagenReadStream stream, out ModHeader header) => stream.MetaData.Constants.TryReadMod(stream, out header);

        public static GroupHeader GetGroup(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.GetGroup(stream, offset);
        public static bool TryGetGroup(this IMutagenReadStream stream, out GroupHeader header, int offset = 0, bool checkIsGroup = true) => stream.MetaData.Constants.TryGetGroup(stream, out header, offset, checkIsGroup);
        public static GroupFrame GetGroupFrame(IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.GetGroupFrame(stream, offset);
        public static bool TryGetGroupFrame(this IMutagenReadStream stream, out GroupFrame header, int offset = 0, bool checkIsGroup = true) => stream.MetaData.Constants.TryGetGroupFrame(stream, out header, offset, checkIsGroup);
        public static GroupMemoryFrame GetGroupMemoryFrame(IMutagenReadStream stream, int offset = 0, bool readSafe = false) => stream.MetaData.Constants.GetGroupMemoryFrame(stream, offset, readSafe);
        public static bool TryGetGroupMemoryFrame(this IMutagenReadStream stream, out GroupMemoryFrame header, int offset = 0, bool checkIsGroup = true, bool readSafe = false) => stream.MetaData.Constants.TryGetGroupMemoryFrame(stream, out header, offset, checkIsGroup, readSafe);
        public static GroupHeader ReadGroup(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.ReadGroup(stream, offset);
        public static bool TryReadGroup(this IMutagenReadStream stream, out GroupHeader header, bool checkIsGroup = true) => stream.MetaData.Constants.TryReadGroup(stream, out header, checkIsGroup);
        public static GroupFrame ReadGroupFrame(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadGroupFrame(stream);
        public static bool TryReadGroupFrame(this IMutagenReadStream stream, out GroupFrame frame, bool checkIsGroup = true) => stream.MetaData.Constants.TryReadGroupFrame(stream, out frame, checkIsGroup);
        public static GroupMemoryFrame ReadGroupMemoryFrame(this IMutagenReadStream stream, bool readSafe = false) => stream.MetaData.Constants.ReadGroupMemoryFrame(stream, readSafe);
        public static bool TryReadGroupMemoryFrame(this IMutagenReadStream stream, out GroupMemoryFrame frame, bool checkIsGroup = true, bool readSafe = false) => stream.MetaData.Constants.TryReadGroupMemoryFrame(stream, out frame, checkIsGroup, readSafe);

        public static MajorRecordHeader GetMajorRecord(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.GetMajorRecord(stream, offset);
        public static MajorRecordFrame GetMajorRecordFrame(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.GetMajorRecordFrame(stream, offset);
        public static MajorRecordHeader ReadMajorRecord(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadMajorRecord(stream);
        public static MajorRecordFrame ReadMajorRecordFrame(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadMajorRecordFrame(stream);
        public static MajorRecordMemoryFrame ReadMajorRecordMemoryFrame(this IMutagenReadStream stream, bool readSafe = false) => stream.MetaData.Constants.ReadMajorRecordMemoryFrame(stream, readSafe: readSafe);

        public static SubrecordHeader GetSubrecord(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.GetSubrecord(stream, offset);
        public static bool TryGetSubrecord(this IMutagenReadStream stream, out SubrecordHeader meta, int offset = 0) => stream.MetaData.Constants.TryGetSubrecord(stream, out meta, offset);
        public static bool TryGetSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader meta) => stream.MetaData.Constants.TryGetSubrecord(stream, targetType, out meta);
        public static SubrecordFrame GetSubrecordFrame(this IMutagenReadStream stream, int offset = 0) => stream.MetaData.Constants.GetSubrecordFrame(stream, offset);
        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame) => stream.MetaData.Constants.TryGetSubrecordFrame(stream, out frame);
        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame) => stream.MetaData.Constants.TryGetSubrecordFrame(stream, targetType, out frame);
        public static SubrecordMemoryFrame GetSubrecordMemoryFrame(this IMutagenReadStream stream, int offset = 0, bool readSafe = false) => stream.MetaData.Constants.GetSubrecordMemoryFrame(stream, offset, readSafe);
        public static bool TryGetSubrecordMemoryFrame(this IMutagenReadStream stream, out SubrecordMemoryFrame frame, bool readSafe = false) => stream.MetaData.Constants.TryGetSubrecordMemoryFrame(stream, out frame, readSafe);
        public static bool TryGetSubrecordMemoryFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordMemoryFrame frame, bool readSafe = false) => stream.MetaData.Constants.TryGetSubrecordMemoryFrame(stream, targetType, out frame, readSafe);
        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadSubrecord(stream);
        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream, RecordType targetType) => stream.MetaData.Constants.ReadSubrecord(stream, targetType);
        public static bool TryReadSubrecord(this IMutagenReadStream stream, out SubrecordHeader meta) => stream.MetaData.Constants.TryReadSubrecord(stream, out meta);
        public static bool TryReadSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader meta) => stream.MetaData.Constants.TryReadSubrecord(stream, targetType, out meta);
        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadSubrecordFrame(stream);
        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType) => stream.MetaData.Constants.ReadSubrecordFrame(stream, targetType);
        public static SubrecordMemoryFrame ReadSubrecordMemoryFrame(this IMutagenReadStream stream, bool readSafe = false) => stream.MetaData.Constants.ReadSubrecordMemoryFrame(stream, readSafe);
        public static SubrecordMemoryFrame ReadSubrecordMemoryFrame(this IMutagenReadStream stream, RecordType targetType, bool readSafe = false) => stream.MetaData.Constants.ReadSubrecordMemoryFrame(stream, targetType, readSafe);
        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame) => stream.MetaData.Constants.TryReadSubrecordFrame(stream, out frame);
        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame) => stream.MetaData.Constants.TryReadSubrecordFrame(stream, targetType, out frame);
        public static bool TryReadSubrecordMemoryFrame(this IMutagenReadStream stream, out SubrecordMemoryFrame frame, bool readSafe = false) => stream.MetaData.Constants.TryReadSubrecordMemoryFrame(stream, out frame, readSafe: readSafe);
        public static bool TryReadSubrecordMemoryFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordMemoryFrame frame, bool readSafe = false) => stream.MetaData.Constants.TryReadSubrecordMemoryFrame(stream, targetType, out frame, readSafe: readSafe);

        public static VariableHeader GetNextRecordVariableMeta(this IMutagenReadStream stream) => stream.MetaData.Constants.GetNextRecordVariableMeta(stream);
        public static VariableHeader ReadNextRecordVariableMeta(this IMutagenReadStream stream) => stream.MetaData.Constants.ReadNextRecordVariableMeta(stream);
    }
}
