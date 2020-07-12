using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public static class StreamHeaderMixIn
    {
        #region Normal Stream
        public static ModHeader GetMod(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false)
        {
            return new ModHeader(constants, stream.GetMemory(constants.ModHeaderLength, readSafe: readSafe));
        }

        public static ModHeader ReadMod(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false)
        {
            return new ModHeader(constants, stream.ReadMemory(constants.ModHeaderLength, readSafe: readSafe));
        }

        public static bool TryGetMod(this IBinaryReadStream stream, GameConstants constants, out ModHeader header, bool readSafe = false)
        {
            if (stream.Remaining < constants.ModHeaderLength)
            {
                header = default;
                return false;
            }
            header = new ModHeader(constants, stream.ReadMemory(constants.ModHeaderLength, readSafe: readSafe));
            return true;
        }

        public static bool TryReadMod(this IBinaryReadStream stream, GameConstants constants, out ModHeader header, bool readSafe = false)
        {
            if (stream.Remaining < constants.ModHeaderLength)
            {
                header = default;
                return false;
            }
            header = new ModHeader(constants, stream.ReadMemory(constants.ModHeaderLength, readSafe: readSafe));
            return true;
        }

        public static GroupHeader GetGroup(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            var ret = new GroupHeader(constants, stream.GetMemory(constants.GroupConstants.HeaderLength, offset, readSafe: readSafe));
            if (checkIsGroup && !ret.IsGroup)
            {
                throw new ArgumentException("Read in data that was not a GRUP");
            }
            return ret;
        }

        public static bool TryGetGroup(this IBinaryReadStream stream, GameConstants constants, out GroupHeader header, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            if (stream.Remaining < constants.GroupConstants.HeaderLength + offset)
            {
                header = default;
                return false;
            }
            header = GetGroup(stream, constants, offset: offset, readSafe: readSafe, checkIsGroup: false);
            return !checkIsGroup || header.IsGroup;
        }

        public static GroupFrame GetGroupFrame(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            var meta = GetGroup(stream, constants, offset: offset, readSafe: readSafe, checkIsGroup: checkIsGroup);
            return new GroupFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset, readSafe: readSafe));
        }

        public static bool TryGetGroupFrame(this IBinaryReadStream stream, GameConstants constants, out GroupFrame frame, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            if (!TryGetGroup(stream, constants, out var meta, offset: offset, checkIsGroup: checkIsGroup, readSafe: false))
            {
                frame = default;
                return false;
            }
            frame = new GroupFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }

        public static GroupHeader ReadGroup(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            var ret = new GroupHeader(constants, stream.ReadMemory(constants.GroupConstants.HeaderLength, offset, readSafe: readSafe));
            if (checkIsGroup && !ret.IsGroup)
            {
                throw new ArgumentException("Read in data that was not a GRUP");
            }
            return ret;
        }

        public static bool TryReadGroup(this IBinaryReadStream stream, GameConstants constants, out GroupHeader header, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            if (stream.Remaining < constants.GroupConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            header = ReadGroup(stream, constants, offset: offset, readSafe: readSafe, checkIsGroup: false);
            var ret = !checkIsGroup || header.IsGroup;
            if (!ret)
            {
                stream.Position -= header.HeaderLength;
            }
            return ret;
        }

        public static GroupFrame ReadGroupFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false, bool checkIsGroup = true)
        {
            var meta = GetGroup(stream, constants, offset: 0, readSafe: readSafe, checkIsGroup: checkIsGroup);
            return new GroupFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        public static bool TryReadGroupFrame(this IBinaryReadStream stream, GameConstants constants, out GroupFrame frame, bool readSafe = false, bool checkIsGroup = true)
        {
            if (!TryGetGroup(stream, constants, out var meta, offset: 0, checkIsGroup: checkIsGroup, readSafe: false))
            {
                frame = default;
                return false;
            }
            frame = new GroupFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }

        public static MajorRecordHeader GetMajorRecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false)
        {
            return new MajorRecordHeader(constants, stream.GetMemory(constants.MajorConstants.HeaderLength, offset, readSafe: readSafe)); ;
        }

        public static MajorRecordFrame GetMajorRecordFrame(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false)
        {
            var meta = GetMajorRecord(stream, constants, offset, readSafe: readSafe);
            return new MajorRecordFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset, readSafe: readSafe));
        }

        public static MajorRecordHeader ReadMajorRecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false)
        {
            return new MajorRecordHeader(constants, stream.ReadMemory(constants.MajorConstants.HeaderLength, offset: offset, readSafe: readSafe));
        }

        public static MajorRecordFrame ReadMajorRecordFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false)
        {
            var meta = GetMajorRecord(stream, constants, offset: 0, readSafe: readSafe);
            return new MajorRecordFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        public static SubrecordHeader GetSubrecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false)
        {
            return new SubrecordHeader(constants, stream.GetMemory(constants.SubConstants.HeaderLength, offset, readSafe: readSafe));
        }

        public static bool TryGetSubrecord(this IBinaryReadStream stream, GameConstants constants, out SubrecordHeader meta, int offset = 0, bool readSafe = false)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength + offset)
            {
                meta = default;
                return false;
            }
            meta = GetSubrecord(stream, constants, offset: offset, readSafe: readSafe);
            return true;
        }

        public static bool TryGetSubrecord(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordHeader meta, int offset = 0, bool readSafe = false)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = GetSubrecord(stream, constants, offset: offset, readSafe: readSafe);
            return targetType == meta.RecordType;
        }

        public static SubrecordFrame GetSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false)
        {
            var meta = GetSubrecord(stream, constants, offset, readSafe: readSafe);
            return new SubrecordFrame(meta, stream.GetMemory(meta.TotalLength, offset: offset, readSafe: readSafe));
        }

        public static bool TryGetSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, out SubrecordFrame frame, int offset = 0, bool readSafe = false)
        {
            if (!TryGetSubrecord(stream, constants, out var meta, readSafe: readSafe, offset: offset))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.GetMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        public static bool TryGetSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordFrame frame, int offset = 0, bool readSafe = false)
        {
            if (!TryGetSubrecord(stream, constants, targetType, out var meta, readSafe: readSafe, offset: offset))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.GetMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        public static SubrecordHeader ReadSubrecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = false)
        {
            return new SubrecordHeader(constants, stream.ReadMemory(constants.SubConstants.HeaderLength, offset: offset, readSafe: readSafe));
        }

        public static SubrecordHeader ReadSubrecord(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, int offset = 0, bool readSafe = false)
        {
            var meta = ReadSubrecord(stream, constants, offset: offset, readSafe: readSafe);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return meta;
        }

        public static bool TryReadSubrecord(this IBinaryReadStream stream, GameConstants constants, out SubrecordHeader meta, bool readSafe = false)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = ReadSubrecord(stream, constants, readSafe: readSafe);
            return true;
        }

        public static bool TryReadSubrecord(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordHeader meta, bool readSafe = false)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength)
            {
                meta = default;
                return false;
            }
            meta = ReadSubrecord(stream, constants, readSafe: readSafe);
            if (meta.RecordType != targetType)
            {
                stream.Position -= meta.HeaderLength;
                return false;
            }
            return true;
        }

        public static SubrecordFrame ReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false)
        {
            var meta = GetSubrecord(stream, constants, readSafe: readSafe, offset: 0);
            return new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
        }

        public static SubrecordFrame ReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, bool readSafe = false)
        {
            var meta = GetSubrecord(stream, constants, readSafe: readSafe, offset: 0);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
        }

        public static bool TryReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, out SubrecordFrame frame, bool readSafe = false)
        {
            if (!TryGetSubrecord(stream, constants, out var meta, readSafe: readSafe, offset: 0))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        public static bool TryReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordFrame frame, bool readSafe = false)
        {
            if (!TryGetSubrecord(stream, constants, targetType, out var meta, readSafe: readSafe, offset: 0))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        public static VariableHeader GetNextRecordVariableMeta(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false)
        {
            RecordType rec = new RecordType(stream.GetInt32());
            if (rec == Mutagen.Bethesda.Internals.Constants.Group)
            {
                return constants.GroupConstants.VariableMeta(stream.GetMemory(constants.GroupConstants.HeaderLength, readSafe: readSafe));
            }
            else
            {
                return constants.MajorConstants.VariableMeta(stream.GetMemory(constants.MajorConstants.HeaderLength, readSafe: readSafe));
            }
        }

        public static VariableHeader ReadNextRecordVariableMeta(this IBinaryReadStream stream, GameConstants constants, bool readSafe = false)
        {
            RecordType rec = new RecordType(stream.GetInt32());
            if (rec == Mutagen.Bethesda.Internals.Constants.Group)
            {
                return constants.GroupConstants.VariableMeta(stream.ReadMemory(constants.GroupConstants.HeaderLength, readSafe: readSafe));
            }
            else
            {
                return constants.MajorConstants.VariableMeta(stream.ReadMemory(constants.MajorConstants.HeaderLength, readSafe: readSafe));
            }
        }
        #endregion
        #region Mutagen Stream
        public static ModHeader GetMod(this IMutagenReadStream stream, bool readSafe = false)
        {
            return GetMod(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        public static ModHeader ReadMod(this IMutagenReadStream stream, bool readSafe = false)
        {
            return ReadMod(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        public static bool TryGetMod(this IMutagenReadStream stream, out ModHeader header, bool readSafe = false)
        {
            return TryGetMod(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        public static bool TryReadMod(this IMutagenReadStream stream, out ModHeader header, bool readSafe = false)
        {
            return TryReadMod(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        public static GroupHeader GetGroup(this IMutagenReadStream stream, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            return GetGroup(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe, checkIsGroup: checkIsGroup);
        }

        public static bool TryGetGroup(this IMutagenReadStream stream, out GroupHeader header, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            return TryGetGroup(stream, stream.MetaData.Constants, out header, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static GroupFrame GetGroupFrame(IMutagenReadStream stream, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            return GetGroupFrame(stream, stream.MetaData.Constants, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static bool TryGetGroupFrame(this IMutagenReadStream stream, out GroupFrame frame, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            return TryGetGroupFrame(stream, stream.MetaData.Constants, out frame, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static GroupHeader ReadGroup(this IMutagenReadStream stream, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            return ReadGroup(stream, stream.MetaData.Constants, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static bool TryReadGroup(this IMutagenReadStream stream, out GroupHeader header, int offset = 0, bool readSafe = false, bool checkIsGroup = true)
        {
            return TryReadGroup(stream, stream.MetaData.Constants, out header, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static GroupFrame ReadGroupFrame(this IMutagenReadStream stream, bool readSafe = false, bool checkIsGroup = true)
        {
            return ReadGroupFrame(stream, stream.MetaData.Constants, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static bool TryReadGroupFrame(this IMutagenReadStream stream, out GroupFrame frame, bool readSafe = false, bool checkIsGroup = true)
        {
            return TryReadGroupFrame(stream, stream.MetaData.Constants, out frame, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        public static MajorRecordHeader GetMajorRecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = false)
        {
            return GetMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        public static MajorRecordHeader ReadMajorRecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = false)
        {
            return ReadMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        public static MajorRecordFrame GetMajorRecordFrame(this IMutagenReadStream stream, int offset = 0, bool readSafe = false)
        {
            return GetMajorRecordFrame(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        public static MajorRecordFrame ReadMajorRecordFrame(this IMutagenReadStream stream, bool readSafe = false)
        {
            return ReadMajorRecordFrame(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        public static SubrecordHeader GetSubrecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = false)
        {
            return GetSubrecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        public static bool TryGetSubrecord(this IMutagenReadStream stream, out SubrecordHeader header, int offset = 0, bool readSafe = false)
        {
            return TryGetSubrecord(stream, stream.MetaData.Constants, out header, offset: offset, readSafe: readSafe);
        }

        public static bool TryGetSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader header, int offset = 0, bool readSafe = false)
        {
            return TryGetSubrecord(stream, stream.MetaData.Constants, targetType, out header, offset: offset, readSafe: readSafe);
        }

        public static SubrecordFrame GetSubrecordFrame(this IMutagenReadStream stream, int offset = 0, bool readSafe = false)
        {
            return GetSubrecordFrame(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame, int offset = 0, bool readSafe = false)
        {
            return TryGetSubrecordFrame(stream, stream.MetaData.Constants, out frame, offset: offset, readSafe: readSafe);
        }

        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame, int offset = 0, bool readSafe = false)
        {
            return TryGetSubrecordFrame(stream, stream.MetaData.Constants, targetType, out frame, offset: offset, readSafe: readSafe);
        }

        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = false)
        {
            return ReadSubrecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream, RecordType targetType, int offset = 0, bool readSafe = false)
        {
            return ReadSubrecord(stream, stream.MetaData.Constants, targetType, offset: offset, readSafe: readSafe);
        }

        public static bool TryReadSubrecord(this IMutagenReadStream stream, out SubrecordHeader header, bool readSafe = false)
        {
            return TryReadSubrecord(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        public static bool TryReadSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader header, bool readSafe = false)
        {
            return TryReadSubrecord(stream, stream.MetaData.Constants, targetType, out header, readSafe: readSafe);
        }

        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream, bool readSafe = false)
        {
            return ReadSubrecordFrame(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, bool readSafe = false)
        {
            return ReadSubrecordFrame(stream, stream.MetaData.Constants, targetType, readSafe: readSafe);
        }

        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame, bool readSafe = false)
        {
            return TryReadSubrecordFrame(stream, stream.MetaData.Constants, out frame, readSafe: readSafe);
        }

        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame, bool readSafe = false)
        {
            return TryReadSubrecordFrame(stream, stream.MetaData.Constants, targetType, out frame, readSafe: readSafe);
        }

        public static VariableHeader GetNextRecordVariableMeta(this IMutagenReadStream stream, bool readSafe = false)
        {
            return GetNextRecordVariableMeta(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        public static VariableHeader ReadNextRecordVariableMeta(this IMutagenReadStream stream, bool readSafe = false)
        {
            return ReadNextRecordVariableMeta(stream, stream.MetaData.Constants, readSafe: readSafe);
        }
        #endregion
    }
}
