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

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            UtilityTranslation.BinaryMasterParseDelegate<TItem> transl,
            RecordTypeConverter femaleRecordConverter,
            RecordTypeConverter? maleRecordConverter = null)
            where TItem : class
        {
            if (!transl(frame, out var male, maleRecordConverter))
            {
                male = null;
            }
            if (!transl(frame, out var female, femaleRecordConverter))
            {
                female = null;
            }
            return new GenderedItem<TItem?>(male, female);
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
                var subHeader = frame.GetSubrecord();
                RecordType type = subHeader.RecordType;
                if (type != maleMarker && type != femaleMarker)
                {
                    break;
                }
                TItem? item;
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
                var subHeader = frame.GetSubrecord();
                RecordType type = subHeader.RecordType;
                if (type == maleMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out male, recordTypeConverter))
                    {
                        male = null;
                    }
                }
                else if (type == femaleMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out female, recordTypeConverter))
                    {
                        female = null;
                    }
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
            RecordType contentMarker,
            UtilityTranslation.BinarySubParseDelegate<TItem> transl,
            bool skipMarker)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var genderedHeader = frame.GetSubrecord();
                RecordType type = genderedHeader.RecordType;
                if (type != maleMarker && type != femaleMarker)
                {
                    break;
                }
                frame.Position += genderedHeader.TotalLength;
                var subHeader = frame.GetSubrecord();
                if (contentMarker != subHeader.RecordType)
                {
                    break;
                }
                TItem? item;
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

        public static GenderedItem<TItem?> ParseMarkerPerItem<TItem>(
            MutagenFrame frame,
            RecordType marker,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinaryMasterParseDelegate<TItem> transl,
            RecordTypeConverter? femaleRecordConverter = null)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                var markerHeader = frame.GetSubrecord();
                if (markerHeader.RecordType != marker) break;
                frame.Position += markerHeader.TotalLength;

                var genderedHeader = frame.GetSubrecord();
                RecordType type = genderedHeader.RecordType;
                if (type != maleMarker && type != femaleMarker)
                {
                    break;
                }
                frame.Position += genderedHeader.TotalLength;
                if (!transl(frame, out var item, type == maleMarker ? null : femaleRecordConverter))
                {
                    throw new ArgumentException();
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
            UtilityTranslation.BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter femaleRecordConverter,
            RecordTypeConverter? maleRecordConverter = null)
        {
            if (item == null) return;
            var male = item.Male;
            if (male != null)
            {
                transl(writer, male, maleRecordConverter);
            }
            var female = item.Female;
            if (female != null)
            {
                transl(writer, female, femaleRecordConverter);
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
                using (HeaderExport.ExportSubrecordHeader(writer, maleMarker))
                {
                    transl(writer, male);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, femaleMarker))
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
            using (HeaderExport.ExportSubrecordHeader(writer, maleMarker))
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
            var female = item.Female;
            using (HeaderExport.ExportSubrecordHeader(writer, femaleMarker))
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

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinarySubWriteDelegate<T> transl)
        {
            if (item == null) return;
            using (HeaderExport.ExportSubrecordHeader(writer, markerType))
            {
            }
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, maleMarker))
                {
                    transl(writer, male);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, femaleMarker))
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
            using (HeaderExport.ExportSubrecordHeader(writer, markerType))
            {
            }
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, maleMarker))
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
                using (HeaderExport.ExportSubrecordHeader(writer, femaleMarker))
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

        public static void WriteMarkerPerItem<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            UtilityTranslation.BinaryMasterWriteDelegate<T> transl,
            bool markerWrap = true,
            RecordTypeConverter? femaleRecordConverter = null)
        {
            if (item == null) return;
            var male = item.Male;
            if (male != null)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, markerType))
                {
                }
                using (HeaderExport.ExportSubrecordHeader(writer, maleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, male, recordTypeConverter: null);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, male, recordTypeConverter: null);
                }
            }
            var female = item.Female;
            if (female != null)
            {
                using (HeaderExport.ExportSubrecordHeader(writer, markerType))
                {
                }
                using (HeaderExport.ExportSubrecordHeader(writer, femaleMarker))
                {
                    if (markerWrap)
                    {
                        transl(writer, female, femaleRecordConverter);
                    }
                }
                if (!markerWrap)
                {
                    transl(writer, female, femaleRecordConverter);
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
            using (HeaderExport.ExportSubrecordHeader(writer, recordType))
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
            using (HeaderExport.ExportSubrecordHeader(writer, recordType))
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
