using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public static class HeaderExt
    {
        public static void AssertLength(this SubrecordFrame frame, int len)
        {
            if (frame.Content.Length != len)
            {
                throw new ArgumentException($"{frame.RecordType} Subrecord frame had unexpected length: {frame.Content.Length} != {len}");
            }
        }

        #region Primitive Extraction
        public static byte AsUInt8(this SubrecordFrame frame)
        {
            frame.AssertLength(1);
            return frame.Content[0];
        }

        public static sbyte AsInt8(this SubrecordFrame frame)
        {
            frame.AssertLength(1);
            return (sbyte)frame.Content[0];
        }

        public static ushort AsUInt16(this SubrecordFrame frame)
        {
            frame.AssertLength(2);
            return BinaryPrimitives.ReadUInt16LittleEndian(frame.Content);
        }

        public static short AsInt16(this SubrecordFrame frame)
        {
            frame.AssertLength(2);
            return BinaryPrimitives.ReadInt16LittleEndian(frame.Content);
        }

        public static uint AsUInt32(this SubrecordFrame frame)
        {
            frame.AssertLength(4);
            return BinaryPrimitives.ReadUInt32LittleEndian(frame.Content);
        }

        public static int AsInt32(this SubrecordFrame frame)
        {
            frame.AssertLength(4);
            return BinaryPrimitives.ReadInt32LittleEndian(frame.Content);
        }

        public static ulong AsUInt64(this SubrecordFrame frame)
        {
            frame.AssertLength(8);
            return BinaryPrimitives.ReadUInt64LittleEndian(frame.Content);
        }

        public static long AsInt64(this SubrecordFrame frame)
        {
            frame.AssertLength(8);
            return BinaryPrimitives.ReadInt64LittleEndian(frame.Content);
        }

        public static float AsFloat(this SubrecordFrame frame)
        {
            frame.AssertLength(4);
            return frame.Content.Float();
        }

        public static double AsDouble(this SubrecordFrame frame)
        {
            frame.AssertLength(8);
            return frame.Content.Double();
        }

        public static string AsString(this SubrecordFrame frame)
        {
            return BinaryStringUtility.ProcessWholeToZString(frame.Content);
        }
        #endregion

        #region Locate
        public static bool TryLocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, out SubrecordHeader header)
        {
            return majorFrame.TryLocateSubrecord(type, header: out header, loc: out var _);
        }

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

        public static SubrecordHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type)
        {
            return majorFrame.LocateSubrecord(type, loc: out var _);
        }

        public static SubrecordHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, out int loc)
        {
            if (!TryLocateSubrecord(majorFrame, type, out var header, out loc))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return header;
        }

        public static SubrecordHeader LocateSubrecord(this MajorRecordFrame majorFrame, RecordType type, int offset, out int loc)
        {
            if (!TryLocateSubrecord(majorFrame, type, offset, out var header, out loc))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return header;
        }

        public static SubrecordFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type)
        {
            if (!TryLocateSubrecordFrame(majorFrame, type, out var frame))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return frame;
        }

        public static SubrecordFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, out int loc)
        {
            if (!TryLocateSubrecordFrame(majorFrame, type, out var frame, out loc))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return frame;
        }

        public static SubrecordFrame LocateSubrecordFrame(this MajorRecordFrame majorFrame, RecordType type, int offset, out int loc)
        {
            if (!TryLocateSubrecordFrame(majorFrame, type, offset, out var frame, out loc))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return frame;
        }

        public static SubrecordPinFrame LocateSubrecordPinFrame(this MajorRecordFrame majorFrame, RecordType type)
        {
            if (!TryLocateSubrecordPinFrame(majorFrame, type, out var pin))
            {
                throw new ArgumentException($"Could not locate subrecord of type: {type}");
            }
            return pin;
        }

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
