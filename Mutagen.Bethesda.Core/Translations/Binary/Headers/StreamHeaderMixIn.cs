using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// Extension class to mix in header extraction functionality to streams
    /// </summary>
    public static class StreamHeaderMixIn
    {
        #region Normal Stream
        /// <summary>
        /// Retrieves a ModHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeader struct</returns>
        public static ModHeader GetModHeader(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
        {
            return new ModHeader(constants, stream.GetMemory(constants.ModHeaderLength, readSafe: readSafe));
        }

        /// <summary>
        /// Reads a ModHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeader struct</returns>
        public static ModHeader ReadModHeader(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
        {
            return new ModHeader(constants, stream.ReadMemory(constants.ModHeaderLength, readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a ModHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="header">ModHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeader struct was read in</returns>
        public static bool TryGetModHeader(this IBinaryReadStream stream, GameConstants constants, out ModHeader header, bool readSafe = true)
        {
            if (stream.Remaining < constants.ModHeaderLength)
            {
                header = default;
                return false;
            }
            header = new ModHeader(constants, stream.ReadMemory(constants.ModHeaderLength, readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a ModHeader struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="header">ModHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeader struct was read in</returns>
        public static bool TryReadModHeader(this IBinaryReadStream stream, GameConstants constants, out ModHeader header, bool readSafe = true)
        {
            if (stream.Remaining < constants.ModHeaderLength)
            {
                header = default;
                return false;
            }
            header = new ModHeader(constants, stream.ReadMemory(constants.ModHeaderLength, readSafe: readSafe));
            return true;
        }
        /// <summary>
        /// Retrieves a ModHeaderFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeaderFrame struct</returns>
        public static ModHeaderFrame GetModHeaderFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
        {
            var meta = GetModHeader(stream, constants, readSafe: readSafe);
            return new ModHeaderFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        /// <summary>
        /// Reads a ModHeaderFrame struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeaderFrame struct</returns>
        public static ModHeaderFrame ReadModHeaderFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
        {
            var meta = GetModHeader(stream, constants, readSafe: readSafe);
            return new ModHeaderFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a ModHeaderFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="frame">ModHeaderFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeaderFrame struct was read in</returns>
        public static bool TryGetModHeaderFrame(this IBinaryReadStream stream, GameConstants constants, out ModHeaderFrame frame, bool readSafe = true)
        {
            if (!TryGetModHeader(stream, constants, out var meta, readSafe: false))
            {
                frame = default;
                return false;
            }
            frame = new ModHeaderFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a ModHeaderFrame struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="frame">ModHeaderFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeaderFrame struct was read in</returns>
        public static bool TryReadModHeaderFrame(this IBinaryReadStream stream, GameConstants constants, out ModHeaderFrame frame, bool readSafe = true)
        {
            if (!TryGetModHeader(stream, constants, out var meta, readSafe: false))
            {
                frame = default;
                return false;
            }
            frame = new ModHeaderFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Retrieves a GroupHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupHeader struct</returns>
        public static GroupHeader GetGroup(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            var ret = new GroupHeader(constants, stream.GetMemory(constants.GroupConstants.HeaderLength, offset, readSafe: readSafe));
            if (checkIsGroup && !ret.IsGroup)
            {
                throw new ArgumentException("Read in data that was not a GRUP");
            }
            return ret;
        }
        
        /// <summary>
        /// Retrieves a GroupHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupHeader struct</returns>
        public static GroupHeader ReadGroup(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            var ret = new GroupHeader(constants, stream.ReadMemory(constants.GroupConstants.HeaderLength, offset, readSafe: readSafe));
            if (checkIsGroup && !ret.IsGroup)
            {
                throw new ArgumentException("Read in data that was not a GRUP");
            }
            return ret;
        }

        /// <summary>
        /// Attempts to retrieve a GroupHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="header">GroupHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupHeader was retrieved</returns>
        public static bool TryGetGroup(this IBinaryReadStream stream, GameConstants constants, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            if (stream.Remaining < constants.GroupConstants.HeaderLength + offset)
            {
                header = default;
                return false;
            }
            header = GetGroup(stream, constants, offset: offset, readSafe: readSafe, checkIsGroup: false);
            return !checkIsGroup || header.IsGroup;
        }

        /// <summary>
        /// Retrieves a GroupFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupFrame struct</returns>
        public static GroupFrame GetGroupFrame(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            var meta = GetGroup(stream, constants, offset: offset, readSafe: readSafe, checkIsGroup: checkIsGroup);
            return new GroupFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset, readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a GroupFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="frame">GroupFrame struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupFrame was retrieved</returns>
        public static bool TryGetGroupFrame(this IBinaryReadStream stream, GameConstants constants, out GroupFrame frame, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            if (!TryGetGroup(stream, constants, out var meta, offset: offset, checkIsGroup: checkIsGroup, readSafe: false))
            {
                frame = default;
                return false;
            }
            frame = new GroupFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a GroupHeader struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="header">GroupHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupHeader was retrieved</returns>
        public static bool TryReadGroup(this IBinaryReadStream stream, GameConstants constants, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
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

        /// <summary>
        /// Retrieves a GroupFrame struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupFrame struct</returns>
        public static GroupFrame ReadGroupFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true, bool checkIsGroup = true)
        {
            var meta = GetGroup(stream, constants, offset: 0, readSafe: readSafe, checkIsGroup: checkIsGroup);
            return new GroupFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a GroupHeader struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="frame">GroupFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupHeader was retrieved</returns>
        public static bool TryReadGroupFrame(this IBinaryReadStream stream, GameConstants constants, out GroupFrame frame, bool readSafe = true, bool checkIsGroup = true)
        {
            if (!TryGetGroup(stream, constants, out var meta, offset: 0, checkIsGroup: checkIsGroup, readSafe: false))
            {
                frame = default;
                return false;
            }
            frame = new GroupFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Retrieves a MajorRecordHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordHeader struct</returns>
        public static MajorRecordHeader GetMajorRecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        {
            return new MajorRecordHeader(constants, stream.GetMemory(constants.MajorConstants.HeaderLength, offset, readSafe: readSafe)); ;
        }

        /// <summary>
        /// Retrieves a MajorRecordFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordFrame struct</returns>
        public static MajorRecordFrame GetMajorRecordFrame(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        {
            var meta = GetMajorRecord(stream, constants, offset, readSafe: readSafe);
            return new MajorRecordFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset, readSafe: readSafe));
        }

        /// <summary>
        /// Retrieves a MajorRecordHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordHeader struct</returns>
        public static MajorRecordHeader ReadMajorRecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        {
            return new MajorRecordHeader(constants, stream.ReadMemory(constants.MajorConstants.HeaderLength, offset: offset, readSafe: readSafe));
        }

        /// <summary>
        /// Retrieves a MajorRecordFrame struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordFrame struct</returns>
        public static MajorRecordFrame ReadMajorRecordFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
        {
            var meta = GetMajorRecord(stream, constants, offset: 0, readSafe: readSafe);
            return new MajorRecordFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        }

        /// <summary>
        /// Retrieves a SubrecordHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A SubrecordHeader struct</returns>
        public static SubrecordHeader GetSubrecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        {
            return new SubrecordHeader(constants, stream.GetMemory(constants.SubConstants.HeaderLength, offset, readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved</returns>
        public static bool TryGetSubrecord(this IBinaryReadStream stream, GameConstants constants, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength + offset)
            {
                header = default;
                return false;
            }
            header = GetSubrecord(stream, constants, offset: offset, readSafe: readSafe);
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct of a specific type from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved, and is of target type</returns>
        public static bool TryGetSubrecord(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            header = GetSubrecord(stream, constants, offset: offset, readSafe: readSafe);
            return targetType == header.RecordType;
        }

        /// <summary>
        /// Retrieves a SubrecordFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A SubrecordFrame struct</returns>
        public static SubrecordFrame GetSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        {
            var meta = GetSubrecord(stream, constants, offset, readSafe: readSafe);
            return new SubrecordFrame(meta, stream.GetMemory(meta.TotalLength, offset: offset, readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved</returns>
        public static bool TryGetSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        {
            if (!TryGetSubrecord(stream, constants, out var meta, readSafe: readSafe, offset: offset))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.GetMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct of a specific type from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved, and is of target type</returns>
        public static bool TryGetSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        {
            if (!TryGetSubrecord(stream, constants, targetType, out var meta, readSafe: readSafe, offset: offset))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.GetMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Retrieves a SubrecordHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A SubrecordHeader struct</returns>
        public static SubrecordHeader ReadSubrecord(this IBinaryReadStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        {
            return new SubrecordHeader(constants, stream.ReadMemory(constants.SubConstants.HeaderLength, offset: offset, readSafe: readSafe));
        }

        /// <summary>
        /// Retrieves a SubrecordHeader struct of a specific type from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <exception cref="System.ArgumentException">Thrown when subrecord is not of target type</exception>
        /// <returns>A SubrecordHeader struct</returns>
        public static SubrecordHeader ReadSubrecord(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, int offset = 0, bool readSafe = true)
        {
            var meta = ReadSubrecord(stream, constants, offset: offset, readSafe: readSafe);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return meta;
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved</returns>
        public static bool TryReadSubrecord(this IBinaryReadStream stream, GameConstants constants, out SubrecordHeader header, bool readSafe = true)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            header = ReadSubrecord(stream, constants, readSafe: readSafe);
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct of a specific type from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved, and is of target type</returns>
        public static bool TryReadSubrecord(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordHeader header, bool readSafe = true)
        {
            if (stream.Remaining < constants.SubConstants.HeaderLength)
            {
                header = default;
                return false;
            }
            header = ReadSubrecord(stream, constants, readSafe: readSafe);
            if (header.RecordType != targetType)
            {
                stream.Position -= header.HeaderLength;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves a SubrecordFrame struct, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <exception cref="System.ArgumentException">Thrown when subrecord is not of target type</exception>
        /// <returns>A SubrecordFrame struct</returns>
        public static SubrecordFrame ReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
        {
            var meta = GetSubrecord(stream, constants, readSafe: readSafe, offset: 0);
            return new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
        }

        /// <summary>
        /// Retrieves a SubrecordFrame struct of a specific type from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <exception cref="System.ArgumentException">Thrown when subrecord is not of target type</exception>
        /// <returns>A SubrecordFrame struct</returns>
        public static SubrecordFrame ReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, bool readSafe = true)
        {
            var meta = GetSubrecord(stream, constants, readSafe: readSafe, offset: 0);
            if (meta.RecordType != targetType)
            {
                throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
            }
            return new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved</returns>
        public static bool TryReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, out SubrecordFrame frame, bool readSafe = true)
        {
            if (!TryGetSubrecord(stream, constants, out var meta, readSafe: readSafe, offset: 0))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct of a specific type from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved, and is of target type</returns>
        public static bool TryReadSubrecordFrame(this IBinaryReadStream stream, GameConstants constants, RecordType targetType, out SubrecordFrame frame, bool readSafe = true)
        {
            if (!TryGetSubrecord(stream, constants, targetType, out var meta, readSafe: readSafe, offset: 0))
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a VariableHeader struct, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A VariableHeader struct</returns>
        public static VariableHeader GetVariableHeader(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
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

        /// <summary>
        /// Attempts to retrieve a VariableHeader struct, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="constants">Constants to use for alignment and measurements</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A VariableHeader struct</returns>
        public static VariableHeader ReadVariableHeader(this IBinaryReadStream stream, GameConstants constants, bool readSafe = true)
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
        /// <summary>
        /// Retrieves a ModHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeader struct</returns>
        public static ModHeader GetModHeader(this IMutagenReadStream stream, bool readSafe = true)
        {
            return GetModHeader(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Reads a ModHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeader struct</returns>
        public static ModHeader ReadModHeader(this IMutagenReadStream stream, bool readSafe = true)
        {
            return ReadModHeader(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a ModHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">ModHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeader struct was read in</returns>
        public static bool TryGetModHeader(this IMutagenReadStream stream, out ModHeader header, bool readSafe = true)
        {
            return TryGetModHeader(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a ModHeader struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">ModHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeader struct was read in</returns>
        public static bool TryReadModHeader(this IMutagenReadStream stream, out ModHeader header, bool readSafe = true)
        {
            return TryReadModHeader(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a ModHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeader struct</returns>
        public static ModHeaderFrame GetModHeaderFrame(this IMutagenReadStream stream, bool readSafe = true)
        {
            return GetModHeaderFrame(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Reads a ModHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A ModHeader struct</returns>
        public static ModHeaderFrame ReadModHeaderFrame(this IMutagenReadStream stream, bool readSafe = true)
        {
            return ReadModHeaderFrame(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a ModHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="frame">ModHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeader struct was read in</returns>
        public static bool TryGetModHeaderFrame(this IMutagenReadStream stream, out ModHeaderFrame frame, bool readSafe = true)
        {
            return TryGetModHeaderFrame(stream, stream.MetaData.Constants, out frame, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a ModHeader struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">ModHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if a ModHeader struct was read in</returns>
        public static bool TryReadModHeaderFrame(this IMutagenReadStream stream, out ModHeaderFrame header, bool readSafe = true)
        {
            return TryReadModHeaderFrame(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a GroupHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupHeader struct</returns>
        public static GroupHeader GetGroup(this IMutagenReadStream stream, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            return GetGroup(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe, checkIsGroup: checkIsGroup);
        }

        /// <summary>
        /// Attempts to retrieve a GroupHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">GroupHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupHeader was retrieved</returns>
        public static bool TryGetGroup(this IMutagenReadStream stream, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            return TryGetGroup(stream, stream.MetaData.Constants, out header, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a GroupFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupFrame struct</returns>
        public static GroupFrame GetGroupFrame(IMutagenReadStream stream, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            return GetGroupFrame(stream, stream.MetaData.Constants, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a GroupFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="frame">GroupFrame struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupFrame was retrieved</returns>
        public static bool TryGetGroupFrame(this IMutagenReadStream stream, out GroupFrame frame, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            return TryGetGroupFrame(stream, stream.MetaData.Constants, out frame, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a GroupHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupHeader struct</returns>
        public static GroupHeader ReadGroup(this IMutagenReadStream stream, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            return ReadGroup(stream, stream.MetaData.Constants, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a GroupHeader struct from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">GroupHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupHeader was retrieved</returns>
        public static bool TryReadGroup(this IMutagenReadStream stream, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        {
            return TryReadGroup(stream, stream.MetaData.Constants, out header, offset: offset, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a GroupFrame struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to throw exception if header is aligned on top of bytes that are not a GRUP</param>
        /// <exception cref="System.ArgumentException">Thrown if checkIsGroup is on, and bytes not aligned on a GRUP.</exception>
        /// <returns>A GroupFrame struct</returns>
        public static GroupFrame ReadGroupFrame(this IMutagenReadStream stream, bool readSafe = true, bool checkIsGroup = true)
        {
            return ReadGroupFrame(stream, stream.MetaData.Constants, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a GroupHeader struct from the stream, its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="frame">GroupFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <param name="checkIsGroup">Whether to return false if header is aligned on top of bytes that are not a GRUP</param>
        /// <returns>True if GroupHeader was retrieved</returns>
        public static bool TryReadGroupFrame(this IMutagenReadStream stream, out GroupFrame frame, bool readSafe = true, bool checkIsGroup = true)
        {
            return TryReadGroupFrame(stream, stream.MetaData.Constants, out frame, checkIsGroup: checkIsGroup, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a MajorRecordHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordHeader struct</returns>
        public static MajorRecordHeader GetMajorRecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = true)
        {
            return GetMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a MajorRecordHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordHeader struct</returns>
        public static MajorRecordHeader ReadMajorRecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = true)
        {
            return ReadMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a MajorRecordFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordFrame struct</returns>
        public static MajorRecordFrame GetMajorRecordFrame(this IMutagenReadStream stream, int offset = 0, bool readSafe = true)
        {
            return GetMajorRecordFrame(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a MajorRecordFrame struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A MajorRecordFrame struct</returns>
        public static MajorRecordFrame ReadMajorRecordFrame(this IMutagenReadStream stream, bool readSafe = true)
        {
            return ReadMajorRecordFrame(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a SubrecordHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A SubrecordHeader struct</returns>
        public static SubrecordHeader GetSubrecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = true)
        {
            return GetSubrecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved</returns>
        public static bool TryGetSubrecord(this IMutagenReadStream stream, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        {
            return TryGetSubrecord(stream, stream.MetaData.Constants, out header, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct of a specific type from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved, and is of target type</returns>
        public static bool TryGetSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        {
            return TryGetSubrecord(stream, stream.MetaData.Constants, targetType, out header, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a SubrecordFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A SubrecordFrame struct</returns>
        public static SubrecordFrame GetSubrecordFrame(this IMutagenReadStream stream, int offset = 0, bool readSafe = true)
        {
            return GetSubrecordFrame(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved</returns>
        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        {
            return TryGetSubrecordFrame(stream, stream.MetaData.Constants, out frame, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct of a specific type from the stream, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved, and is of target type</returns>
        public static bool TryGetSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        {
            return TryGetSubrecordFrame(stream, stream.MetaData.Constants, targetType, out frame, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a SubrecordHeader struct from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A SubrecordHeader struct</returns>
        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream, int offset = 0, bool readSafe = true)
        {
            return ReadSubrecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a SubrecordHeader struct of a specific type from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="offset">Offset to the current position in the stream to read from</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <exception cref="System.ArgumentException">Thrown when subrecord is not of target type</exception>
        /// <returns>A SubrecordHeader struct</returns>
        public static SubrecordHeader ReadSubrecord(this IMutagenReadStream stream, RecordType targetType, int offset = 0, bool readSafe = true)
        {
            return ReadSubrecord(stream, stream.MetaData.Constants, targetType, offset: offset, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved</returns>
        public static bool TryReadSubrecord(this IMutagenReadStream stream, out SubrecordHeader header, bool readSafe = true)
        {
            return TryReadSubrecord(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordHeader struct of a specific type from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordHeader was retrieved, and is of target type</returns>
        public static bool TryReadSubrecord(this IMutagenReadStream stream, RecordType targetType, out SubrecordHeader header, bool readSafe = true)
        {
            return TryReadSubrecord(stream, stream.MetaData.Constants, targetType, out header, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a SubrecordFrame struct, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <exception cref="System.ArgumentException">Thrown when subrecord is not of target type</exception>
        /// <returns>A SubrecordFrame struct</returns>
        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream, bool readSafe = true)
        {
            return ReadSubrecordFrame(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Retrieves a SubrecordFrame struct of a specific type from the stream, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <exception cref="System.ArgumentException">Thrown when subrecord is not of target type</exception>
        /// <returns>A SubrecordFrame struct</returns>
        public static SubrecordFrame ReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, bool readSafe = true)
        {
            return ReadSubrecordFrame(stream, stream.MetaData.Constants, targetType, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved</returns>
        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, out SubrecordFrame frame, bool readSafe = true)
        {
            return TryReadSubrecordFrame(stream, stream.MetaData.Constants, out frame, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a SubrecordFrame struct of a specific type from the stream, progressing its position if successful.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="targetType">RecordType to require for a successful query</param>
        /// <param name="frame">SubrecordFrame struct if successfully retrieved</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>True if SubrecordFrame was retrieved, and is of target type</returns>
        public static bool TryReadSubrecordFrame(this IMutagenReadStream stream, RecordType targetType, out SubrecordFrame frame, bool readSafe = true)
        {
            return TryReadSubrecordFrame(stream, stream.MetaData.Constants, targetType, out frame, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a VariableHeader struct, without progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A VariableHeader struct</returns>
        public static VariableHeader GetVariableHeader(this IMutagenReadStream stream, bool readSafe = true)
        {
            return GetVariableHeader(stream, stream.MetaData.Constants, readSafe: readSafe);
        }

        /// <summary>
        /// Attempts to retrieve a VariableHeader struct, progressing its position.
        /// </summary>
        /// <param name="stream">Source stream</param>
        /// <param name="readSafe">
        /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
        /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
        /// If true, extra data copies may occur depending on the underling stream type.
        /// </param>
        /// <returns>A VariableHeader struct</returns>
        public static VariableHeader ReadVariableHeader(this IMutagenReadStream stream, bool readSafe = true)
        {
            return ReadVariableHeader(stream, stream.MetaData.Constants, readSafe: readSafe);
        }
        #endregion
    }
}
