using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Loqui;
using System.IO;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using static Mutagen.Bethesda.UtilityTranslation;

namespace Mutagen.Bethesda.Binary
{
    public class ListBinaryTranslation<T>
    {
        public static readonly ListBinaryTranslation<T> Instance = new ListBinaryTranslation<T>();

        public static readonly bool IsLoqui;

        static ListBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        #region Out Parameters
        public IEnumerable<T> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            BinarySubParseDelegate<T> transl)
        {
            var ret = new List<T>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, triggeringRecord)) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, out var subItem))
                {
                    ret.Add(subItem);
                }

                if (frame.Position == startingPos)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                    throw new ArgumentException($"Parsed item on the list consumed no data: {subItem}");
                }
            }
            return ret;
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            BinarySubParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType>? triggeringRecord = null)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                var nextRecord = HeaderTranslation.GetNextRecordType(frame.Reader);
                if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, nextRecord, out var subIitem))
                {
                    ret.Add(subIitem);
                }
                if (frame.Position == startingPos)
                {
                    throw new ArgumentException($"Parsed item on the list consumed no data: {subIitem}");
                }
            }
            return ret;
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            BinaryMasterParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType>? triggeringRecord = null,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                var nextRecord = HeaderTranslation.GetNextRecordType(frame.Reader);
                if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, nextRecord, out var subIitem, masterReferences, recordTypeConverter))
                {
                    ret.Add(subIitem);
                }
                if (frame.Position == startingPos)
                {
                    throw new ArgumentException($"Parsed item on the list consumed no data: {subIitem}");
                }
            }
            return ret;
        }
        #endregion

        #region Lengthed Triggering Record
        public IEnumerable<T> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, triggeringRecord)) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, out var subItem, masterReferences, recordTypeConverter))
                {
                    ret.Add(subItem);
                }

                if (frame.Position == startingPos)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                    throw new ArgumentException($"Parsed item on the list consumed no data: {subItem}");
                }
            }
            return ret;
        }
        #endregion

        #region Lengthed Triggering Records
        public IEnumerable<T> Parse(
            MutagenFrame frame,
            BinarySubParseDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord)
        {
            return this.Parse(
                frame: frame,
                triggeringRecord: triggeringRecord,
                transl: (MutagenFrame reader, RecordType header, out T subItem)
                    => transl(reader, out subItem));
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord,
            RecordTypeConverter? recordTypeConverter = null)
        {
            return this.Parse(
                frame: frame,
                triggeringRecord: triggeringRecord,
                masterReferences: masterReferences,
                transl: (MutagenFrame reader, RecordType header, out T subItem, MasterReferenceReader m, RecordTypeConverter? r)
                    => transl(reader, out subItem, m, r),
                recordTypeConverter: recordTypeConverter);
        }
        #endregion

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            BinarySubParseDelegate<T> transl)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                if (transl(frame, out var subItem))
                {
                    ret.Add(subItem);
                }
            }
            return ret;
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                if (transl(frame, out var subItem, masterReferences, recordTypeConverter))
                {
                    ret.Add(subItem);
                }
            }
            return ret;
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            int amount,
            BinarySubParseDelegate<T> transl)
        {
            var ret = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                if (transl(frame, out var subItem))
                {
                    ret.Add(subItem);
                }
            }
            return ret;
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            int amount,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                if (transl(frame, out var subItem, masterReferences, recordTypeConverter))
                {
                    ret.Add(subItem);
                }
            }
            return ret;
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            int amount,
            RecordType triggeringRecord,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var subHeader = frame.MetaData.GetSubRecord(frame);
            if (subHeader.RecordType != triggeringRecord)
            {
                throw new ArgumentException($"Unexpected record encountered.");
            }
            if (!IsLoqui)
            {
                frame.Position += frame.MetaData.SubConstants.HeaderLength;
            }
            var ret = new List<T>();
            var startingPos = frame.Position;
            for (int i = 0; i < amount; i++)
            {
                if (transl(frame, out var subIitem, masterReferences, recordTypeConverter))
                {
                    ret.Add(subIitem);
                }
            }
            if (frame.Position == startingPos)
            {
                throw new ArgumentException($"Parsed item on the list consumed no data.");
            }
            return ret;
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T>? items,
            BinarySubWriteDelegate<T> transl)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                transl(writer, item);
            }
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T>? items,
            MasterReferenceReader masterReferences,
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                transl(writer, item, masterReferences, recordTypeConverter);
            }
        }

        public void Write(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                foreach (var item in items)
                {
                    transl(writer, item);
                }
            }
        }

        public void Write(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType recordType,
            MasterReferenceReader masterReferences,
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                foreach (var item in items)
                {
                    transl(writer, item, masterReferences, recordTypeConverter);
                }
            }
        }

        public void WriteWithCounter(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType counterType,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count);
            }
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                foreach (var item in items)
                {
                    transl(writer, item);
                }
            }
        }

        public void WriteWithCounter(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType counterType,
            RecordType recordType,
            MasterReferenceReader masterReferences,
            BinaryMasterWriteDelegate<T> transl,
            bool subRecordPerItem = false,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count);
            }
            if (subRecordPerItem)
            {
                foreach (var item in items)
                {
                    using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                    {
                        transl(writer, item, masterReferences, recordTypeConverter);
                    }
                }
            }
            else
            {
                using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                {
                    foreach (var item in items)
                    {
                        transl(writer, item, masterReferences, recordTypeConverter);
                    }
                }
            }
        }

        public void WriteWithCounter(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType counterType,
            MasterReferenceReader masterReferences,
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count);
            }
            foreach (var item in items)
            {
                transl(writer, item, masterReferences, recordTypeConverter);
            }
        }
    }

    public class ListAsyncBinaryTranslation<T>
    {
        public static readonly ListAsyncBinaryTranslation<T> Instance = new ListAsyncBinaryTranslation<T>();

        public delegate Task<TryGet<T>> BinarySubParseDelegate(MutagenFrame reader);
        public delegate Task<TryGet<T>> BinaryMasterParseDelegate(
            MutagenFrame reader,
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter);
        public delegate Task<TryGet<T>> BinarySubParseRecordDelegate(
            MutagenFrame reader,
            RecordType header);
        public static readonly bool IsLoqui;

        static ListAsyncBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        public async Task<IEnumerable<T>> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            BinarySubParseDelegate transl)
        {
            var ret = new List<T>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, triggeringRecord)) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                var item = await transl(frame).ConfigureAwait(false);
                if (item.Succeeded)
                {
                    ret.Add(item.Value);
                }

                if (frame.Position == startingPos)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                    throw new ArgumentException($"Parsed item on the list consumed no data: {item.Value}");
                }
            }
            return ret;
        }

        public async Task<IEnumerable<T>> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, triggeringRecord)) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                var item = await transl(frame, masterReferences, recordTypeConverter).ConfigureAwait(false);
                if (item.Succeeded)
                {
                    ret.Add(item.Value);
                }

                if (frame.Position == startingPos)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                    throw new ArgumentException($"Parsed item on the list consumed no data: {item.Value}");
                }
            }
            return ret;
        }

        public async Task<IEnumerable<T>> ParseThreaded(
            MutagenFrame frame,
            RecordType triggeringRecord,
            BinarySubParseDelegate transl)
        {
            var tasks = new List<Task<TryGet<T>>>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                var nextRec = HeaderTranslation.GetNextSubRecordType(
                    reader: frame.Reader,
                    contentLength: out var contentLen);
                if (nextRec != triggeringRecord) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }

                var toDo = transl(frame);

                tasks.Add(Task.Run(() => toDo));
            }
            var ret = await Task.WhenAll(tasks).ConfigureAwait(false);
            return ret.Where(i => i.Succeeded)
                .Select(i => i.Value);
        }

        public async Task<IEnumerable<T>> ParseThreaded(
            MutagenFrame frame,
            RecordType triggeringRecord,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var tasks = new List<Task<TryGet<T>>>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                var nextRec = HeaderTranslation.GetNextSubRecordType(
                    reader: frame.Reader,
                    contentLength: out var contentLen);
                if (nextRec != triggeringRecord) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }

                var toDo = transl(frame, masterReferences, recordTypeConverter);

                tasks.Add(Task.Run(() => toDo));
            }
            var ret = await Task.WhenAll(tasks).ConfigureAwait(false);
            return ret.Where(i => i.Succeeded)
                .Select(i => i.Value);
        }

        #region Lengthed Triggering Record
        public async Task<ExtendedList<T>> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            BinarySubParseDelegate transl,
            bool thread = false)
        {
            IEnumerable<T> items;
            if (thread)
            {
                items = await ParseThreaded(
                    frame,
                    triggeringRecord,
                    transl: transl).ConfigureAwait(false);
            }
            else
            {
                items = await Parse(
                    frame,
                    triggeringRecord,
                    transl: transl).ConfigureAwait(false);
            }
            return new ExtendedList<T>(items);
        }

        public async Task<ExtendedList<T>> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate transl,
            bool thread,
            RecordTypeConverter? recordTypeConverter = null)
        {
            IEnumerable<T> items;
            if (thread)
            {
                items = await ParseThreaded(
                    frame,
                    triggeringRecord,
                    masterReferences: masterReferences,
                    transl: transl,
                    recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
            }
            else
            {
                items = await Parse(
                    frame,
                    triggeringRecord,
                    masterReferences: masterReferences,
                    transl: transl,
                    recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
            }
            return new ExtendedList<T>(items);
        }
        #endregion

        #region Cache Helpers
        public async Task Parse<K>(
            MutagenFrame frame,
            ICache<T, K> item,
            RecordType triggeringRecord,
            BinarySubParseDelegate transl)
        {
            item.SetTo(
                await Parse(
                    frame,
                    triggeringRecord,
                    transl: transl));
        }
        #endregion
    }
}
