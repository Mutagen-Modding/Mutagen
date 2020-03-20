using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public class GenderedItemBinaryTranslation
    {
        public static GenderedItem<TItem> Parse<TItem>(
            MutagenFrame frame,
            UtilityTranslation.BinarySubParseDelegate<TItem> transl)
        {
            if (!transl(frame, out var male))
            {
                throw new ArgumentException();
            }
            if (!transl(frame, out var female))
            {
                throw new ArgumentException();
            }
            return new GenderedItem<TItem>(male, female);
        }

        public static GenderedItem<TItem> Parse<TItem>(
            MutagenFrame frame,
            UtilityTranslation.BinaryMasterParseDelegate<TItem> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (!transl(frame, out var male, recordTypeConverter))
            {
                throw new ArgumentException();
            }
            if (!transl(frame, out var female, recordTypeConverter))
            {
                throw new ArgumentException();
            }
            return new GenderedItem<TItem>(male, female);
        }

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinarySubParseDelegate<TItem> transl,
            bool skipMarker)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var subHeader = frame.MetaData.GetSubRecord(frame);
                RecordType type = subHeader.RecordType;
                if (type != maleMarker && type != femaleMarker)
                {
                    break;
                }
                TItem item;
                if (skipMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out item))
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    frame.Position += subHeader.HeaderLength;
                    if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out item))
                    {
                        throw new ArgumentException();
                    }
                }
                if (type == maleMarker)
                {
                    male = item;
                }
                else if (type == femaleMarker)
                {
                    female = item;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            return new GenderedItem<TItem?>(male, female);
        }

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinaryMasterParseDelegate<TItem> transl,
            RecordTypeConverter? recordTypeConverter = null)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var subHeader = frame.MetaData.GetSubRecord(frame);
                RecordType type = subHeader.RecordType;
                if (type == maleMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out male, recordTypeConverter))
                    {
                        throw new ArgumentException();
                    }
                }
                else if (type == femaleMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out female, recordTypeConverter))
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    break;
                }
            }
            return new GenderedItem<TItem?>(male, female);
        }

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            RecordType maleMarker,
            RecordType femaleMarker,
            RecordType contentMarker,
            UtilityTranslation.BinarySubParseDelegate<TItem> transl,
            bool skipMarker)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var genderedHeader = frame.MetaData.GetSubRecord(frame);
                RecordType type = genderedHeader.RecordType;
                if (type != maleMarker && type != femaleMarker)
                {
                    break;
                }
                frame.Position += genderedHeader.TotalLength;
                var subHeader = frame.MetaData.GetSubRecord(frame);
                if (contentMarker != subHeader.RecordType)
                {
                    break;
                }
                TItem item;
                if (skipMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out item))
                    {
                        throw new ArgumentException();
                    }
                }
                else
                {
                    frame.Position += subHeader.HeaderLength;
                    if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out item))
                    {
                        throw new ArgumentException();
                    }
                }
                if (type == maleMarker)
                {
                    male = item;
                }
                else if (type == femaleMarker)
                {
                    female = item;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            return new GenderedItem<TItem?>(male, female);
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            UtilityTranslation.BinarySubWriteDelegate<T> transl)
        {
            if (item == null) return;
            var male = item.Male;
            if (male != null)
            {
                transl(writer, male);
            }
            var female = item.Female;
            if (female != null)
            {
                transl(writer, female);
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinarySubWriteDelegate<T> transl)
        {
            if (item == null) return;
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, maleMarker))
                {
                    transl(writer, male);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, femaleMarker))
                {
                    transl(writer, female);
                }
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinaryMasterWriteDelegate<T> transl,
            bool markerWrap = true,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (item == null) return;
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, maleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, male, recordTypeConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, male, recordTypeConverter);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, femaleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, female, recordTypeConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, female, recordTypeConverter);
                }
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinarySubWriteDelegate<T> transl)
        {
            if (item == null) return;
            using (HeaderExport.ExportSubRecordHeader(writer, markerType))
            {
            }
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, maleMarker))
                {
                    transl(writer, male);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, femaleMarker))
                {
                    transl(writer, female);
                }
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinaryMasterWriteDelegate<T> transl,
            bool markerWrap = true,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (item == null) return;
            using (HeaderExport.ExportSubRecordHeader(writer, markerType))
            {
            }
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, maleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, male, recordTypeConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, male, recordTypeConverter);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubRecordHeader(writer, femaleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, female, recordTypeConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, female, recordTypeConverter);
                }
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType recordType,
            UtilityTranslation.BinarySubWriteDelegate<T> transl)
        {
            if (item == null) return;
            using (HeaderExport.ExportSubRecordHeader(writer, recordType))
            {
                var male = item.Male;
                if (male != null)
                {
                    transl(writer, male);
                }
                var female = item.Female;
                if (female != null)
                {
                    transl(writer, female);
                }
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType recordType,
            UtilityTranslation.BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (item == null) return;
            using (HeaderExport.ExportSubRecordHeader(writer, recordType))
            {
                var male = item.Male;
                if (male != null)
                {
                    transl(writer, male, recordTypeConverter);
                }
                var female = item.Female;
                if (female != null)
                {
                    transl(writer, female, recordTypeConverter);
                }
            }
        }
    }
}
