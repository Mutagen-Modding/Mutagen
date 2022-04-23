using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda;

/// <summary>
/// Extension class to mix in convenience methods to header structs
/// </summary>
public static class HeaderExt
{
    /// <summary>
    /// Asserts that a subrecord's content is exactly a certain length
    /// </summary>
    /// <param name="frame">Frame to check</param>
    /// <param name="len">Length to assert on the content</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content length does not match parameter</exception>
    public static void AssertLength(this SubrecordFrame frame, int len)
    {
        if (frame.Content.Length != len)
        {
            throw new ArgumentException($"{frame.RecordType} Subrecord frame had unexpected length: {frame.Content.Length} != {len}");
        }
    }

    #region Primitive Extraction
    /// <summary>
    /// Interprets a subrecord's content as a byte.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 1</exception>
    /// <returns>Subrecord's content as a byte</returns>
    public static byte AsUInt8(this SubrecordFrame frame)
    {
        frame.AssertLength(1);
        return frame.Content[0];
    }

    /// <summary>
    /// Interprets a subrecord's content as a sbyte.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 1</exception>
    /// <returns>Subrecord's content as a sbyte</returns>
    public static sbyte AsInt8(this SubrecordFrame frame)
    {
        frame.AssertLength(1);
        return (sbyte)frame.Content[0];
    }

    /// <summary>
    /// Interprets a subrecord's content as a ushort.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 2</exception>
    /// <returns>Subrecord's content as a ushort</returns>
    public static ushort AsUInt16(this SubrecordFrame frame)
    {
        frame.AssertLength(2);
        return BinaryPrimitives.ReadUInt16LittleEndian(frame.Content);
    }

    /// <summary>
    /// Interprets a subrecord's content as a short.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 2</exception>
    /// <returns>Subrecord's content as a short</returns>
    public static short AsInt16(this SubrecordFrame frame)
    {
        frame.AssertLength(2);
        return BinaryPrimitives.ReadInt16LittleEndian(frame.Content);
    }

    /// <summary>
    /// Interprets a subrecord's content as a uint.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 4</exception>
    /// <returns>Subrecord's content as a uint</returns>
    public static uint AsUInt32(this SubrecordFrame frame)
    {
        frame.AssertLength(4);
        return BinaryPrimitives.ReadUInt32LittleEndian(frame.Content);
    }

    /// <summary>
    /// Interprets a subrecord's content as a int.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 4</exception>
    /// <returns>Subrecord's content as a int</returns>
    public static int AsInt32(this SubrecordFrame frame)
    {
        frame.AssertLength(4);
        return BinaryPrimitives.ReadInt32LittleEndian(frame.Content);
    }

    /// <summary>
    /// Interprets a subrecord's content as a ulong.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 8</exception>
    /// <returns>Subrecord's content as a ulong</returns>
    public static ulong AsUInt64(this SubrecordFrame frame)
    {
        frame.AssertLength(8);
        return BinaryPrimitives.ReadUInt64LittleEndian(frame.Content);
    }

    /// <summary>
    /// Interprets a subrecord's content as a long.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 8</exception>
    /// <returns>Subrecord's content as a long</returns>
    public static long AsInt64(this SubrecordFrame frame)
    {
        frame.AssertLength(8);
        return BinaryPrimitives.ReadInt64LittleEndian(frame.Content);
    }

    /// <summary>
    /// Interprets a subrecord's content as a float.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 4</exception>
    /// <returns>Subrecord's content as a float</returns>
    public static float AsFloat(this SubrecordFrame frame)
    {
        frame.AssertLength(4);
        return frame.Content.Float();
    }

    /// <summary>
    /// Interprets a subrecord's content as a double.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 8</exception>
    /// <returns>Subrecord's content as a double</returns>
    public static double AsDouble(this SubrecordFrame frame)
    {
        frame.AssertLength(8);
        return frame.Content.Double();
    }

    /// <summary>
    /// Interprets a subrecord's content as a string.
    /// </summary>
    /// <param name="frame">Frame to read from</param>
    /// <param name="encoding">Encoding to use</param>
    /// <returns>Subrecord's content as a string, null trimmed if applicable</returns>
    public static string AsString(this SubrecordFrame frame, IMutagenEncoding encoding)
    {
        return BinaryStringUtility.ProcessWholeToZString(frame.Content, encoding);
    }

    #region Pin Forwarding
    /// <summary>
    /// Interprets a subrecord's content as a byte.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 1</exception>
    /// <returns>Subrecord's content as a byte</returns>
    public static byte AsUInt8(this SubrecordPinFrame pin) => pin.Frame.AsUInt8();

    /// <summary>
    /// Interprets a subrecord's content as a sbyte.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 1</exception>
    /// <returns>Subrecord's content as a sbyte</returns>
    public static sbyte AsInt8(this SubrecordPinFrame pin) => pin.Frame.AsInt8();

    /// <summary>
    /// Interprets a subrecord's content as a ushort.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 2</exception>
    /// <returns>Subrecord's content as a ushort</returns>
    public static ushort AsUInt16(this SubrecordPinFrame pin) => pin.Frame.AsUInt16();

    /// <summary>
    /// Interprets a subrecord's content as a short.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 2</exception>
    /// <returns>Subrecord's content as a short</returns>
    public static short AsInt16(this SubrecordPinFrame pin) => pin.Frame.AsInt16();

    /// <summary>
    /// Interprets a subrecord's content as a uint.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 4</exception>
    /// <returns>Subrecord's content as a uint</returns>
    public static uint AsUInt32(this SubrecordPinFrame pin) => pin.Frame.AsUInt32();

    /// <summary>
    /// Interprets a subrecord's content as a int.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 4</exception>
    /// <returns>Subrecord's content as a int</returns>
    public static int AsInt32(this SubrecordPinFrame pin) => pin.Frame.AsInt32();

    /// <summary>
    /// Interprets a subrecord's content as a ulong.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 8</exception>
    /// <returns>Subrecord's content as a ulong</returns>
    public static ulong AsUInt64(this SubrecordPinFrame pin) => pin.Frame.AsUInt64();

    /// <summary>
    /// Interprets a subrecord's content as a long.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 8</exception>
    /// <returns>Subrecord's content as a long</returns>
    public static long AsInt64(this SubrecordPinFrame pin) => pin.Frame.AsInt64();

    /// <summary>
    /// Interprets a subrecord's content as a float.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 4</exception>
    /// <returns>Subrecord's content as a float</returns>
    public static float AsFloat(this SubrecordPinFrame pin) => pin.Frame.AsFloat();

    /// <summary>
    /// Interprets a subrecord's content as a double.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <exception cref="System.ArgumentException">Thrown if frame's content is not exactly 8</exception>
    /// <returns>Subrecord's content as a double</returns>
    public static double AsDouble(this SubrecordPinFrame pin) => pin.Frame.AsDouble();

    /// <summary>
    /// Interprets a subrecord's content as a string.
    /// </summary>
    /// <param name="pin">Frame to read from</param>
    /// <param name="encoding">Encoding to use</param>
    /// <returns>Subrecord's content as a string, null trimmed if applicable</returns>
    public static string AsString(this SubrecordPinFrame pin, IMutagenEncoding encoding) => pin.Frame.AsString(encoding);
    #endregion
    #endregion

    #region Locate
    /// <summary>
    /// Iterates a MajorRecordFrame's contents and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="header">SubrecordHeader if found</param>
    /// <returns>True if matching subrecord is found</returns>
    public static bool TryLocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, out SubrecordPinHeader header)
    {
        var find = PluginUtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, type);
        if (find == null)
        {
            header = default;
            return false;
        }
        header = new SubrecordPinHeader(majorFrame.Meta, majorFrame.Content.Slice(find.Value.Location), find.Value.Location + majorFrame.HeaderLength);
        return true;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="header">SubrecordHeader if found</param>
    /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
    /// <returns>True if matching subrecord is found</returns>
    public static bool TryLocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, int offset, out SubrecordPinHeader header)
    {
        var find = PluginUtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(offset - majorFrame.HeaderLength), majorFrame.Meta, type);
        if (find == null)
        {
            header = default;
            return false;
        }
        header = new SubrecordPinHeader(majorFrame.Meta, majorFrame.Content.Slice(find.Value.Location + offset - majorFrame.HeaderLength), find.Value.Location + offset);
        return true;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="pin">SubrecordPinFrame if found</param>
    /// <returns>True if matching subrecord is found</returns>
    public static bool TryLocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, out SubrecordPinFrame pin)
    {
        var find = PluginUtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, type);
        if (find == null)
        {
            pin = default;
            return false;
        }
        pin = new SubrecordPinFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value.Location), find.Value.Location + majorFrame.HeaderLength);
        return true;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="offset">Offset within the Major Record's contents to start searching</param>
    /// <param name="pin">SubrecordPinFrame if found</param>
    /// <returns>True if matching subrecord is found</returns>
    public static bool TryLocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, int offset, out SubrecordPinFrame pin)
    {
        var find = PluginUtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(offset - majorFrame.HeaderLength), majorFrame.Meta, type);
        if (find == null)
        {
            pin = default;
            return false;
        }
        pin = new SubrecordPinFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value.Location + offset - majorFrame.HeaderLength), find.Value.Location + offset);
        return true;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
    /// <returns>First encountered SubrecordHeader with the given type</returns>
    public static SubrecordPinHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type)
    {
        if (!TryLocateSubrecord(majorFrame, type, out var header))
        {
            throw new ArgumentException($"Could not locate subrecord of type: {type}");
        }
        return header;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="offset">Offset within the Major Record's contents to start searching</param>
    /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
    /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
    /// <returns>First encountered SubrecordHeader with the given type</returns>
    public static SubrecordPinHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, int offset)
    {
        if (!TryLocateSubrecord(majorFrame, type, offset, out var header))
        {
            throw new ArgumentException($"Could not locate subrecord of type: {type}");
        }
        return header;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
    /// <returns>First encountered SubrecordPin with the given type</returns>
    public static SubrecordPinFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type)
    {
        if (!TryLocateSubrecordFrame(majorFrame, type, out var pin))
        {
            throw new ArgumentException($"Could not locate subrecord of type: {type}");
        }
        return pin;
    }

    /// <summary>
    /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="offset">Offset within the Major Record's contents to start searching</param>
    /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
    /// <returns>First encountered SubrecordPin with the given type</returns>
    public static SubrecordPinFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, int offset)
    {
        if (!TryLocateSubrecordFrame(majorFrame, type, offset, out var pin))
        {
            throw new ArgumentException($"Could not locate subrecord of type: {type}");
        }
        return pin;
    }
    #endregion

    #region Iterate
    /// <summary>
    /// Finds and iterates subrecords of a given type
    /// </summary>
    /// <param name="majorFrame">Frame to read from</param>
    /// <param name="type">Type to search for</param>
    /// <param name="onlyFirstSet">
    /// If true, iteration will stop after finding the first non-applicable record after some applicable ones.<br/>
    /// If false, records will continue to be searched in their entirety for all matching subrecords.
    /// </param>
    /// <returns>First encountered SubrecordFrame with the given type</returns>
    public static IEnumerable<SubrecordPinFrame> FindEnumerateSubrecords(this MajorRecordFrame majorFrame, RecordType type, bool onlyFirstSet = false)
    {
        bool encountered = false;
        foreach (var subrecord in majorFrame)
        {
            if (subrecord.RecordType == type)
            {
                encountered = true;
                yield return subrecord;
            }
            else if (onlyFirstSet && encountered)
            {
                yield break;
            }
        }
    }

    internal static IEnumerable<SubrecordPinFrame> EnumerateSubrecords(ReadOnlyMemorySlice<byte> span, GameConstants meta, int loc, ICollection<RecordType>? lengthOverflowTypes = null)
    {
        while (loc < span.Length)
        {
            var subFrame = new SubrecordPinFrame(meta, span.Slice(loc), loc);
            if (lengthOverflowTypes?.Contains(subFrame.RecordType) ?? false)
            { // Length overflow record
                var nextLen = subFrame.AsUInt32();
                loc += subFrame.TotalLength;
                var nextSpan = span.Slice(loc, checked((int)(nextLen + meta.SubConstants.HeaderLength)));
                var subHeader = new SubrecordHeader(meta, nextSpan);
                yield return SubrecordPinFrame.FactoryNoTrim(subHeader, nextSpan, loc);
                loc += checked((int)(subHeader.HeaderLength + nextLen));
                continue;
            }
            yield return subFrame;
            loc += subFrame.TotalLength;
        }
    }

    /// <summary>
    /// Enumerates locations of the contained subrecords, while considering some specified RecordTypes as special length overflow subrecords.<br/>
    /// These length overflow subrecords will be skipped, and simply used to parse the next subrecord properly.<br />
    /// Locations are relative to the RecordType of the MajorRecordFrame.
    /// </summary>
    /// <param name="majorFrame">MajorRecordFrame to iterate</param>
    /// <param name="lengthOverflowTypes">Collection of known RecordTypes that signify a length overflow subrecord</param>
    public static IEnumerable<SubrecordPinFrame> EnumerateSubrecords(this MajorRecordFrame majorFrame, ICollection<RecordType>? lengthOverflowTypes = null)
    {
        return EnumerateSubrecords(majorFrame.HeaderAndContentData, majorFrame.Meta, majorFrame.HeaderLength, lengthOverflowTypes ?? _headerOverflow);
    }

    private static readonly ICollection<RecordType> _headerOverflow = new List<RecordType>() { RecordTypes.XXXX };

    /// <summary>
    /// Enumerates locations of the contained subrecords.<br/>
    /// Locations are relative to the RecordType of the ModHeaderFrame.
    /// <param name="lengthOverflowTypes">Collection of known RecordTypes that signify a length overflow subrecord</param>
    /// </summary>
    public static IEnumerable<SubrecordPinFrame> EnumerateSubrecords(this ModHeaderFrame modHeader, ICollection<RecordType>? lengthOverflowTypes = null)
    {
        return EnumerateSubrecords(modHeader.HeaderAndContentData, modHeader.Meta, modHeader.HeaderLength, lengthOverflowTypes ?? _headerOverflow);
    }

    public static IEnumerable<SubrecordPinFrame> Masters(this ModHeaderFrame modHeader, ICollection<RecordType>? lengthOverflowTypes = null)
    {
        foreach (var pin in EnumerateSubrecords(modHeader, lengthOverflowTypes: lengthOverflowTypes ?? _headerOverflow))
        {
            if (pin.RecordType == RecordTypes.MAST)
            {
                yield return pin;
            }
        }
    }

    // Not an extension method, as we don't want it to show up as intellisense, as it's already part of a GroupFrame's enumerator.
    /// <summary>
    /// Enumerates locations of the contained subrecords.<br/>
    /// Locations are relative to the RecordType of the MajorRecordFrame.
    /// </summary>
    public static IEnumerable<MajorRecordPinFrame> EnumerateRecords(GroupFrame group)
    {
        int loc = group.HeaderLength;
        while (loc < group.HeaderAndContentData.Length)
        {
            var subHeader = new MajorRecordPinFrame(group.Meta, group.HeaderAndContentData.Slice(loc), loc);
            yield return subHeader;
            loc = checked((int)(loc + subHeader.TotalLength));
        }
    }
    #endregion
}