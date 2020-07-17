using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
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
        /// <returns>Subrecord's content as a byte</return>
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
        /// <returns>Subrecord's content as a sbyte</return>
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
        /// <returns>Subrecord's content as a ushort</return>
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
        /// <returns>Subrecord's content as a short</return>
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
        /// <returns>Subrecord's content as a uint</return>
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
        /// <returns>Subrecord's content as a int</return>
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
        /// <returns>Subrecord's content as a ulong</return>
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
        /// <returns>Subrecord's content as a long</return>
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
        /// <returns>Subrecord's content as a float</return>
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
        /// <returns>Subrecord's content as a double</return>
        public static double AsDouble(this SubrecordFrame frame)
        {
            frame.AssertLength(8);
            return frame.Content.Double();
        }

        /// <summary>
        /// Interprets a subrecord's content as a string.
        /// </summary>
        /// <param name="frame">Frame to read from</param>
        /// <returns>Subrecord's content as a string, null trimmed if applicable</return>
        public static string AsString(this SubrecordFrame frame)
        {
            return BinaryStringUtility.ProcessWholeToZString(frame.Content);
        }
        #endregion

        #region Locate
        /// <summary>
        /// Iterates a MajorRecordFrame's contents and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="header">SubrecordHeader if found</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, out SubrecordHeader header)
        {
            return majorFrame.TryLocateSubrecord(type, header: out header, loc: out var _);
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's contents and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="header">SubrecordHeader if found</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, out SubrecordHeader header, out int loc)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                header = default;
                loc = default;
                return false;
            }
            header = new SubrecordHeader(majorFrame.Meta, majorFrame.Content.Slice(find.Value));
            loc = find.Value + majorFrame.HeaderLength;
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="header">SubrecordHeader if found</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, int offset, out SubrecordHeader header, out int loc)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(offset - majorFrame.HeaderLength), majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                header = default;
                loc = default;
                return false;
            }
            header = new SubrecordHeader(majorFrame.Meta, majorFrame.Content.Slice(find.Value + offset - majorFrame.HeaderLength));
            loc = find.Value + offset;
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="frame">SubrecordFrame if found</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, out SubrecordFrame frame)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                frame = default;
                return false;
            }
            frame = new SubrecordFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value));
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="frame">SubrecordFrame if found</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, out SubrecordFrame frame, out int loc)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                frame = default;
                loc = default;
                return false;
            }
            frame = new SubrecordFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value));
            loc = find.Value + majorFrame.HeaderLength;
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="offset">Offset within the Major Record's contents to start searching</param>
        /// <param name="frame">SubrecordFrame if found</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, int offset, out SubrecordFrame frame, out int loc)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(offset - majorFrame.HeaderLength), majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                frame = default;
                loc = default;
                return false;
            }
            frame = new SubrecordFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value + offset - majorFrame.HeaderLength));
            loc = find.Value + offset;
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="pin">SubrecordPinFrame if found</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecordPinFrame(this MajorRecordFrame majorFrame, RecordType type, out SubrecordPinFrame pin)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content, majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                pin = default;
                return false;
            }
            pin = new SubrecordPinFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value), find.Value + majorFrame.HeaderLength);
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="offset">Offset within the Major Record's contents to start searching</param>
        /// <param name="pin">SubrecordPinFrame if found</param>
        /// <returns>True if matching subrecord is found</return>
        public static bool TryLocateSubrecordPinFrame(this MajorRecordFrame majorFrame, RecordType type, int offset, out SubrecordPinFrame pin)
        {
            var find = UtilityTranslation.FindFirstSubrecord(majorFrame.Content.Slice(offset - majorFrame.HeaderLength), majorFrame.Meta, type, navigateToContent: false);
            if (find == null)
            {
                pin = default;
                return false;
            }
            pin = new SubrecordPinFrame(majorFrame.Meta, majorFrame.Content.Slice(find.Value + offset - majorFrame.HeaderLength), find.Value + offset);
            return true;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
        /// <returns>First encountered SubrecordHeader with the given type</return>
        public static SubrecordHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type)
        {
            return majorFrame.LocateSubrecord(type, loc: out var _);
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
        /// <returns>First encountered SubrecordHeader with the given type</return>
        public static SubrecordHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, out int loc)
        {
            if (!TryLocateSubrecord(majorFrame, type, out var header, out loc))
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
        /// <returns>First encountered SubrecordHeader with the given type</return>
        public static SubrecordHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, int offset, out int loc)
        {
            if (!TryLocateSubrecord(majorFrame, type, offset, out var header, out loc))
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
        /// <returns>First encountered SubrecordFrame with the given type</return>
        public static SubrecordFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type)
        {
            if (!TryLocateSubrecordFrame(majorFrame, type, out var frame))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return frame;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
        /// <returns>First encountered SubrecordFrame with the given type</return>
        public static SubrecordFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, out int loc)
        {
            if (!TryLocateSubrecordFrame(majorFrame, type, out var frame, out loc))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return frame;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <param name="offset">Offset within the Major Record's contents to start searching</param>
        /// <param name="loc">Location of the subrecord, relative to the parent record's RecordType data</param>
        /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
        /// <returns>First encountered SubrecordFrame with the given type</return>
        public static SubrecordFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, int offset, out int loc)
        {
            if (!TryLocateSubrecordFrame(majorFrame, type, offset, out var frame, out loc))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return frame;
        }

        /// <summary>
        /// Iterates a MajorRecordFrame's subrecords and locates the first occurance of the desired type
        /// </summary>
        /// <param name="majorFrame">Frame to read from</param>
        /// <param name="type">Type to search for</param>
        /// <exception cref="System.ArgumentException">Thrown if target type cannot be found.</exception>
        /// <returns>First encountered SubrecordPin with the given type</return>
        public static SubrecordPinFrame LocateSubrecordPinFrame(this MajorRecordFrame majorFrame, RecordType type)
        {
            if (!TryLocateSubrecordPinFrame(majorFrame, type, out var pin))
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
        /// <returns>First encountered SubrecordPin with the given type</return>
        public static SubrecordPinFrame LocateSubrecordPinFrame(this MajorRecordFrame majorFrame, RecordType type, int offset)
        {
            if (!TryLocateSubrecordPinFrame(majorFrame, type, offset, out var pin))
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
        /// <returns>First encountered SubrecordFrame with the given type</return>
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
        #endregion
    }
}
