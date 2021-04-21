using Mutagen.Bethesda.Records.Binary.Streams;
using System;
using static Mutagen.Bethesda.Records.Binary.Translations.UtilityTranslation;
using static Mutagen.Bethesda.Translations.Binary.UtilityTranslation;

namespace Mutagen.Bethesda.Records.Binary.Translations
{
    public class GenderedItemBinaryTranslation
    {
        public static GenderedItem<TItem> Parse<TItem>(
            MutagenFrame frame,
            BinarySubParseDelegate<MutagenFrame, TItem> transl)
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
            BinaryMasterParseDelegate<TItem> transl,
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
            BinaryMasterParseDelegate<TItem> transl,
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
            BinarySubParseDelegate<MutagenFrame, TItem> transl,
            bool skipMarker)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                if (frame.Reader.Complete) break;
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
                        continue;
                    }
                }
                else
                {
                    frame.Position += subHeader.HeaderLength;
                    if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out item))
                    {
                        continue;
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

        public static GenderedItem<TItem> Parse<TItem>(
            MutagenFrame frame,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinarySubParseDelegate<MutagenFrame, TItem> transl,
            bool skipMarker,
            TItem fallback)
        {
            TItem male = fallback, female = fallback;
            for (int i = 0; i < 2; i++)
            {
                if (frame.Reader.Complete) break;
                var subHeader = frame.GetSubrecord();
                RecordType type = subHeader.RecordType;
                if (type != maleMarker && type != femaleMarker)
                {
                    break;
                }
                TItem item;
                if (skipMarker)
                {
                    frame.Position += subHeader.TotalLength;
                    if (!transl(frame, out var translItem))
                    {
                        continue;
                    }
                    item = translItem;
                }
                else
                {
                    frame.Position += subHeader.HeaderLength;
                    if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out var translItem))
                    {
                        continue;
                    }
                    item = translItem;
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
            return new GenderedItem<TItem>(male, female);
        }

        public static GenderedItem<TItem?> Parse<TItem>(
            MutagenFrame frame,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinaryMasterParseDelegate<TItem> transl,
            RecordTypeConverter? recordTypeConverter = null)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                if (frame.Reader.Complete) break;
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
            BinarySubParseDelegate<MutagenFrame, TItem> transl,
            bool skipMarker)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                if (frame.Reader.Complete) break;
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
                        continue;
                    }
                }
                else
                {
                    frame.Position += subHeader.HeaderLength;
                    if (!transl(frame.SpawnWithLength(subHeader.ContentLength), out item))
                    {
                        continue;
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
            BinaryMasterParseDelegate<TItem> transl,
            RecordTypeConverter? femaleRecordConverter = null)
            where TItem : class
        {
            TItem? male = default, female = default;
            for (int i = 0; i < 2; i++)
            {
                if (frame.Reader.Complete) break;
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
                    continue;
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
            BinarySubWriteDelegate<MutagenWriter, T> transl)
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
            BinaryMasterWriteDelegate<T> transl,
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
            BinarySubWriteDelegate<MutagenWriter, T> transl)
        {
            if (item == null) return;
            try
            {
                var male = item.Male;
                if (male != null)
                {
                    using (HeaderExport.Subrecord(writer, maleMarker))
                    {
                        transl(writer, male);
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, maleMarker);
            }
            try
            {
                var female = item.Female;
                if (female != null)
                {
                    using (HeaderExport.Subrecord(writer, femaleMarker))
                    {
                        transl(writer, female);
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, femaleMarker);
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinaryMasterWriteDelegate<T> transl,
            bool markerWrap = true,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (item == null) return;
            try
            {
                var male = item.Male;
                using (HeaderExport.Subrecord(writer, maleMarker))
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
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, maleMarker);
            }
            try
            {
                var female = item.Female;
                using (HeaderExport.Subrecord(writer, femaleMarker))
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
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, femaleMarker);
            }
        }

        public static void Write<TMajor>(
            MutagenWriter writer,
            IGenderedItemGetter<IFormLinkNullableGetter<TMajor>>? item,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinaryMasterWriteDelegate<IFormLinkNullableGetter<TMajor>> transl,
            bool markerWrap = true,
            RecordTypeConverter? recordTypeConverter = null)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (item == null) return;
            try
            {
                var male = item.Male;
                if (male.FormKeyNullable != null)
                {
                    using (HeaderExport.Subrecord(writer, maleMarker))
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
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, maleMarker);
            }
            try
            {
                var female = item.Female;
                if (female.FormKeyNullable != null)
                {
                    using (HeaderExport.Subrecord(writer, femaleMarker))
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
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, femaleMarker);
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinarySubWriteDelegate<MutagenWriter, T> transl)
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, markerType))
            {
            }
            try
            {
                var male = item.Male;
                if (male != null)
                {
                    using (HeaderExport.Subrecord(writer, maleMarker))
                    {
                        transl(writer, male);
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, maleMarker);
            }
            try
            {
                var female = item.Female;
                if (female != null)
                {
                    using (HeaderExport.Subrecord(writer, femaleMarker))
                    {
                        transl(writer, female);
                    }
                }
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, femaleMarker);
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinaryMasterWriteDelegate<T> transl,
            bool markerWrap = true,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, markerType))
            {
            }
            try
            {
                var male = item.Male;
                if (male != null)
                {
                    using (HeaderExport.Subrecord(writer, maleMarker))
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
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, maleMarker);
            }
            try
            {
                var female = item.Female;
                if (female != null)
                {
                    using (HeaderExport.Subrecord(writer, femaleMarker))
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
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, femaleMarker);
            }
        }

        public static void WriteMarkerPerItem<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType markerType,
            RecordType maleMarker,
            RecordType femaleMarker,
            BinaryMasterWriteDelegate<T> transl,
            bool markerWrap = true,
            RecordTypeConverter? femaleRecordConverter = null)
        {
            if (item == null) return;
            try
            {
                var male = item.Male;
                if (male != null)
                {
                    using (HeaderExport.Subrecord(writer, markerType))
                    {
                    }
                    using (HeaderExport.Subrecord(writer, maleMarker))
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
            }
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, maleMarker);
            }
            try
            {
                var female = item.Female;
                if (female != null)
                {
                    using (HeaderExport.Subrecord(writer, markerType))
                    {
                    }
                    using (HeaderExport.Subrecord(writer, femaleMarker))
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
            catch (Exception ex)
            {
                throw SubrecordException.Enrich(ex, femaleMarker);
            }
        }

        public static void Write<T>(
            MutagenWriter writer,
            IGenderedItemGetter<T>? item,
            RecordType recordType,
            BinarySubWriteDelegate<MutagenWriter, T> transl)
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, recordType))
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
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (item == null) return;
            using (HeaderExport.Subrecord(writer, recordType))
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
