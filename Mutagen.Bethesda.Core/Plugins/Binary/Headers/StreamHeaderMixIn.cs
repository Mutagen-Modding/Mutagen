using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System;

namespace Mutagen.Bethesda;

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
    public static ModHeader GetModHeader<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static ModHeader ReadModHeader<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryGetModHeader<TStream>(this TStream stream, GameConstants constants, out ModHeader header, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryReadModHeader<TStream>(this TStream stream, GameConstants constants, out ModHeader header, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static ModHeaderFrame GetModHeaderFrame<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static ModHeaderFrame ReadModHeaderFrame<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryGetModHeaderFrame<TStream>(this TStream stream, GameConstants constants, out ModHeaderFrame frame, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryReadModHeaderFrame<TStream>(this TStream stream, GameConstants constants, out ModHeaderFrame frame, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static GroupHeader GetGroup<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static GroupHeader ReadGroup<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static bool TryGetGroup<TStream>(this TStream stream, GameConstants constants, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static GroupFrame GetGroupFrame<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static bool TryGetGroupFrame<TStream>(this TStream stream, GameConstants constants, out GroupFrame frame, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static bool TryReadGroup<TStream>(this TStream stream, GameConstants constants, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static GroupFrame ReadGroupFrame<TStream>(this TStream stream, GameConstants constants, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static bool TryReadGroupFrame<TStream>(this TStream stream, GameConstants constants, out GroupFrame frame, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IBinaryReadStream
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
    public static MajorRecordHeader GetMajorRecord<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        return new MajorRecordHeader(constants, stream.GetMemory(constants.MajorConstants.HeaderLength, offset, readSafe: readSafe));
    }

    /// <summary>
    /// Attempts to retrieve a MajorRecordHeader struct from the stream, without progressing its position.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="constants">Constants to use for alignment and measurements</param>
    /// <param name="header">MajorRecordHeader struct if successfully retrieved</param>
    /// <param name="offset">Offset to the current position in the stream to read from</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if SubrecordHeader was retrieved</returns>
    public static bool TryGetMajorRecord<TStream>(this TStream stream, GameConstants constants, out MajorRecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (stream.Remaining < constants.MajorConstants.HeaderLength + offset)
        {
            header = default;
            return false;
        }
        header = GetMajorRecord(stream, constants, offset: offset, readSafe: readSafe);
        return true;
    }

    /// <summary>
    /// Attempts to retrieve a MajorRecordHeader struct from the stream, without progressing its position.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="constants">Constants to use for alignment and measurements</param>
    /// <param name="targetType">RecordType to require for a successful query</param>
    /// <param name="header">MajorRecordHeader struct if successfully retrieved</param>
    /// <param name="offset">Offset to the current position in the stream to read from</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if SubrecordHeader was retrieved</returns>
    public static bool TryGetMajorRecord<TStream>(this TStream stream, GameConstants constants, RecordType targetType, out MajorRecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (stream.Remaining < constants.MajorConstants.HeaderLength + offset)
        {
            header = default;
            return false;
        }
        header = GetMajorRecord(stream, constants, offset: offset, readSafe: readSafe);
        return header.RecordType == targetType;
    }

    /// <summary>
    /// Attempts to retrieve a MajorRecordHeader struct from the stream progressing its position.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="constants">Constants to use for alignment and measurements</param>
    /// <param name="header">MajorRecordHeader struct if successfully retrieved</param>
    /// <param name="offset">Offset to the current position in the stream to read from</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if SubrecordHeader was retrieved</returns>
    public static bool TryReadMajorRecord<TStream>(this TStream stream, GameConstants constants, out MajorRecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (stream.Remaining < constants.MajorConstants.HeaderLength + offset)
        {
            header = default;
            return false;
        }
        header = ReadMajorRecord(stream, constants, offset: offset, readSafe: readSafe);
        return true;
    }

    /// <summary>
    /// Attempts to retrieve a MajorRecordHeader struct from the stream progressing its position.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="constants">Constants to use for alignment and measurements</param>
    /// <param name="targetType">RecordType to require for a successful query</param>
    /// <param name="header">MajorRecordHeader struct if successfully retrieved</param>
    /// <param name="offset">Offset to the current position in the stream to read from</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if SubrecordHeader was retrieved</returns>
    public static bool TryReadMajorRecord<TStream>(this TStream stream, GameConstants constants,RecordType targetType,  out MajorRecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (stream.Remaining < constants.MajorConstants.HeaderLength + offset)
        {
            header = default;
            return false;
        }
        header = ReadMajorRecord(stream, constants, offset: offset, readSafe: readSafe);
        if (header.RecordType != targetType)
        {
            stream.Position -= header.HeaderLength;
            return false;
        }
        return true;
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
    /// <param name="automaticallyDecompress">Whether to automatically decompress when applicable</param>
    /// <returns>A MajorRecordFrame struct</returns>
    public static MajorRecordFrame GetMajorRecordFrame<TStream>(
        this TStream stream, 
        GameConstants constants,
        int offset = 0,
        bool readSafe = true,
        bool automaticallyDecompress = false)
        where TStream : IBinaryReadStream
    {
        var meta = GetMajorRecord(stream, constants, offset, readSafe: readSafe);
        var ret = new MajorRecordFrame(meta, stream.GetMemory(checked((int)meta.TotalLength), offset: offset, readSafe: readSafe));
        if (automaticallyDecompress && ret.IsCompressed)
        {
            return ret.Decompress(out _);
        }

        return ret;
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
    public static MajorRecordHeader ReadMajorRecord<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    /// <param name="automaticallyDecompress">Whether to automatically decompress when applicable</param>
    /// <returns>A MajorRecordFrame struct</returns>
    public static MajorRecordFrame ReadMajorRecordFrame<TStream>(
        this TStream stream, 
        GameConstants constants, 
        bool readSafe = true,
        bool automaticallyDecompress = false)
        where TStream : IBinaryReadStream
    {
        var meta = GetMajorRecord(stream, constants, offset: 0, readSafe: readSafe);
        var ret = new MajorRecordFrame(meta, stream.ReadMemory(checked((int)meta.TotalLength), readSafe: readSafe));
        if (automaticallyDecompress && ret.IsCompressed)
        {
            return ret.Decompress(out _);
        }

        return ret;
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
    public static SubrecordHeader GetSubrecord<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryGetSubrecord<TStream>(this TStream stream, GameConstants constants, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryGetSubrecord<TStream>(this TStream stream, GameConstants constants, RecordType targetType, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static SubrecordFrame GetSubrecordFrame<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        var meta = GetSubrecord(stream, constants, offset, readSafe: readSafe);
        return SubrecordFrame.FactoryNoTrim(meta, stream.GetMemory(meta.TotalLength, offset: offset, readSafe: readSafe));
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
    public static bool TryGetSubrecordFrame<TStream>(this TStream stream, GameConstants constants, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (!TryGetSubrecord(stream, constants, out var meta, readSafe: readSafe, offset: offset))
        {
            frame = default;
            return false;
        }
        frame = SubrecordFrame.FactoryNoTrim(meta, stream.GetMemory(meta.TotalLength, readSafe: readSafe));
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
    public static bool TryGetSubrecordFrame<TStream>(this TStream stream, GameConstants constants, RecordType targetType, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (!TryGetSubrecord(stream, constants, targetType, out var meta, readSafe: readSafe, offset: offset))
        {
            frame = default;
            return false;
        }
        frame = SubrecordFrame.FactoryNoTrim(meta, stream.GetMemory(meta.TotalLength, readSafe: readSafe));
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
    public static SubrecordHeader ReadSubrecord<TStream>(this TStream stream, GameConstants constants, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static SubrecordHeader ReadSubrecord<TStream>(this TStream stream, GameConstants constants, RecordType targetType, int offset = 0, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryReadSubrecord<TStream>(this TStream stream, GameConstants constants, out SubrecordHeader header, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static bool TryReadSubrecord<TStream>(this TStream stream, GameConstants constants, RecordType targetType, out SubrecordHeader header, bool readSafe = true)
        where TStream : IBinaryReadStream
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
    public static SubrecordFrame ReadSubrecordFrame<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        var meta = GetSubrecord(stream, constants, readSafe: readSafe, offset: 0);
        return SubrecordFrame.FactoryNoTrim(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
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
    public static SubrecordFrame ReadSubrecordFrame<TStream>(this TStream stream, GameConstants constants, RecordType targetType, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        var meta = GetSubrecord(stream, constants, readSafe: readSafe, offset: 0);
        if (meta.RecordType != targetType)
        {
            throw new ArgumentException($"Unexpected header type: {meta.RecordType}");
        }
        return SubrecordFrame.FactoryNoTrim(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
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
    public static bool TryReadSubrecordFrame<TStream>(this TStream stream, GameConstants constants, out SubrecordFrame frame, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (!TryGetSubrecord(stream, constants, out var meta, readSafe: readSafe, offset: 0))
        {
            frame = default;
            return false;
        }
        frame = SubrecordFrame.FactoryNoTrim(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
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
    public static bool TryReadSubrecordFrame<TStream>(this TStream stream, GameConstants constants, RecordType targetType, out SubrecordFrame frame, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        if (!TryGetSubrecord(stream, constants, targetType, out var meta, readSafe: readSafe, offset: 0))
        {
            frame = default;
            return false;
        }
        frame = SubrecordFrame.FactoryNoTrim(meta, stream.ReadMemory(meta.TotalLength, readSafe: readSafe));
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
    public static VariableHeader GetVariableHeader<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        RecordType rec = new RecordType(stream.GetInt32());
        if (rec == Constants.Group)
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
    public static VariableHeader ReadVariableHeader<TStream>(this TStream stream, GameConstants constants, bool readSafe = true)
        where TStream : IBinaryReadStream
    {
        RecordType rec = new RecordType(stream.GetInt32());
        if (rec == Constants.Group)
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
    public static ModHeader GetModHeader<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static ModHeader ReadModHeader<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetModHeader<TStream>(this TStream stream, out ModHeader header, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadModHeader<TStream>(this TStream stream, out ModHeader header, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static ModHeaderFrame GetModHeaderFrame<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static ModHeaderFrame ReadModHeaderFrame<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetModHeaderFrame<TStream>(this TStream stream, out ModHeaderFrame frame, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadModHeaderFrame<TStream>(this TStream stream, out ModHeaderFrame header, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static GroupHeader GetGroup<TStream>(this TStream stream, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetGroup<TStream>(this TStream stream, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static GroupFrame GetGroupFrame<TStream>(this TStream stream, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetGroupFrame<TStream>(this TStream stream, out GroupFrame frame, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static GroupHeader ReadGroup<TStream>(this TStream stream, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadGroup<TStream>(this TStream stream, out GroupHeader header, int offset = 0, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static GroupFrame ReadGroupFrame<TStream>(this TStream stream, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadGroupFrame<TStream>(this TStream stream, out GroupFrame frame, bool readSafe = true, bool checkIsGroup = true)
        where TStream : IMutagenReadStream
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
    public static MajorRecordHeader GetMajorRecord<TStream>(this TStream stream, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        return GetMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
    }

    /// <summary>
    /// Attempts to retrieve a MajorRecordHeader struct from the stream, without progressing its position.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="header">MajorRecordHeader struct if successfully retrieved</param>
    /// <param name="offset">Offset to the current position in the stream to read from</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if MajorRecordHeader was retrieved</returns>
    public static bool TryGetMajorRecord<TStream>(this TStream stream, out MajorRecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        if (stream.Remaining < stream.MetaData.Constants.MajorConstants.HeaderLength + offset)
        {
            header = default;
            return false;
        }
        header = GetMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
        return true;
    }

    /// <summary>
    /// Attempts to retrieve a MajorRecordHeader struct from the stream, without progressing its position.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="targetType">RecordType to require for a successful query</param>
    /// <param name="header">MajorRecordHeader struct if successfully retrieved</param>
    /// <param name="offset">Offset to the current position in the stream to read from</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if MajorRecordHeader was retrieved</returns>
    public static bool TryGetMajorRecord<TStream>(this TStream stream, RecordType targetType, out MajorRecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        return TryGetMajorRecord(stream, stream.MetaData.Constants, targetType, out header, offset: offset, readSafe: readSafe);
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
    public static MajorRecordHeader ReadMajorRecord<TStream>(this TStream stream, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        return ReadMajorRecord(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe);
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
    public static bool TryReadMajorRecord<TStream>(this TStream stream, out MajorRecordHeader header, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        return TryReadMajorRecord(stream, stream.MetaData.Constants, out header, readSafe: readSafe);
    }

    /// <summary>
    /// Attempts to retrieve a SubrecordHeader struct, progressing its position if successful.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <param name="targetType">RecordType to require for a successful query</param>
    /// <param name="header">SubrecordHeader struct if successfully retrieved</param>
    /// <param name="readSafe">
    /// Whether to prepare the underlying bytes to be safe in the case of future reads from the same stream.<br/>
    /// If false, future stream movement may corrupt and misalign underlying data the header references.<br/>
    /// If true, extra data copies may occur depending on the underling stream type.
    /// </param>
    /// <returns>True if SubrecordHeader was retrieved</returns>
    public static bool TryReadMajorRecord<TStream>(this TStream stream, RecordType targetType, out MajorRecordHeader header, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        return TryReadMajorRecord(stream, stream.MetaData.Constants, targetType, out header, readSafe: readSafe);
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
    /// <param name="automaticallyDecompress">Whether to automatically decompress when applicable</param>
    /// <returns>A MajorRecordFrame struct</returns>
    public static MajorRecordFrame GetMajorRecordFrame<TStream>(
        this TStream stream,
        int offset = 0,
        bool readSafe = true, 
        bool automaticallyDecompress = false)
        where TStream : IMutagenReadStream
    {
        return GetMajorRecordFrame(stream, stream.MetaData.Constants, offset: offset, readSafe: readSafe, automaticallyDecompress: automaticallyDecompress);
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
    /// <param name="automaticallyDecompress">Whether to automatically decompress when applicable</param>
    /// <returns>A MajorRecordFrame struct</returns>
    public static MajorRecordFrame ReadMajorRecordFrame<TStream>(
        this TStream stream,
        bool readSafe = true, 
        bool automaticallyDecompress = false)
        where TStream : IMutagenReadStream
    {
        return ReadMajorRecordFrame(stream, stream.MetaData.Constants, readSafe: readSafe, automaticallyDecompress: automaticallyDecompress);
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
    public static SubrecordHeader GetSubrecord<TStream>(this TStream stream, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetSubrecord<TStream>(this TStream stream, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetSubrecord<TStream>(this TStream stream, RecordType targetType, out SubrecordHeader header, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static SubrecordFrame GetSubrecordFrame<TStream>(this TStream stream, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetSubrecordFrame<TStream>(this TStream stream, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryGetSubrecordFrame<TStream>(this TStream stream, RecordType targetType, out SubrecordFrame frame, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static SubrecordHeader ReadSubrecord<TStream>(this TStream stream, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static SubrecordHeader ReadSubrecord<TStream>(this TStream stream, RecordType targetType, int offset = 0, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadSubrecord<TStream>(this TStream stream, out SubrecordHeader header, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadSubrecord<TStream>(this TStream stream, RecordType targetType, out SubrecordHeader header, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static SubrecordFrame ReadSubrecordFrame<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static SubrecordFrame ReadSubrecordFrame<TStream>(this TStream stream, RecordType targetType, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadSubrecordFrame<TStream>(this TStream stream, out SubrecordFrame frame, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static bool TryReadSubrecordFrame<TStream>(this TStream stream, RecordType targetType, out SubrecordFrame frame, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static VariableHeader GetVariableHeader<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
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
    public static VariableHeader ReadVariableHeader<TStream>(this TStream stream, bool readSafe = true)
        where TStream : IMutagenReadStream
    {
        return ReadVariableHeader(stream, stream.MetaData.Constants, readSafe: readSafe);
    }

    /// <summary>
    /// Locates the first encountered instances of all given subrecord types, and returns an array of their locations
    /// -1 represents a recordtype that was not found.
    /// 
    /// Not suggested to use with high numbers of record types, as it is an N^2 algorithm
    /// </summary>
    /// <param name="stream">Stream to read from</param>
    /// <param name="recordTypes">Record types to locate</param>
    /// <returns>Array of found record locations</returns>
    //public static int?[] ReadFirstSubrecords<TStream>(this TStream stream, params RecordType[] recordTypes)
    //    where TStream : IMutagenReadStream
    //{
    //    int?[] ret = new int?[recordTypes.Length];
    //    while (stream.Complete)
    //    {
    //        var subMeta = stream.GetSubrecord();
    //        var recType = subMeta.RecordType;
    //        for (int i = 0; i < recordTypes.Length; i++)
    //        {
    //            if (recordTypes[i] == recType && ret[i] == null)
    //            {
    //                ret[i] = lenParsed;
    //                bool breakOut = false;

    //                // Check to see if there's still more to find
    //                for (int j = 0; j < ret.Length; j++)
    //                {
    //                    if (ret[j] == null)
    //                    {
    //                        breakOut = true;
    //                        break;
    //                    }
    //                }

    //                if (breakOut)
    //                {
    //                    break;
    //                }

    //                // Found everything
    //                return ret;
    //            }
    //        }

    //        lenParsed += subMeta.TotalLength;
    //    }

    //    return ret;
    //}
    #endregion
}