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

namespace Mutagen.Bethesda.Binary
{
    public class ListBinaryTranslation<T>
    {
        public static readonly ListBinaryTranslation<T> Instance = new ListBinaryTranslation<T>();

        public delegate bool BinarySubParseDelegate(
            MutagenFrame reader,
            out T item);
        public delegate bool BinaryMasterParseDelegate(
            MutagenFrame reader,
            out T item,
            MasterReferences masterReferences);
        public delegate bool BinarySubParseRecordDelegate(
            MutagenFrame reader,
            RecordType header,
            out T item);
        public delegate void BinarySubWriteDelegate(
            MutagenWriter writer,
            T item);
        public delegate void BinaryMasterWriteDelegate(
            MutagenWriter writer,
            T item,
            MasterReferences masterReferences);

        public static readonly bool IsLoqui;

        static ListBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        #region Out Parameters
        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate transl)
        {
            var ret = new List<T>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, lengthLength, triggeringRecord)) break;
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

        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            long lengthLength,
            BinarySubParseRecordDelegate transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
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
        #endregion

        #region Lengthed Triggering Record
        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate transl)
        {
            var enumer = ParseRepeatedItem(
                frame,
                triggeringRecord,
                lengthLength,
                transl: transl);
            if (item is ISetList<T> setList)
            {
                setList.SetTo(enumer);
            }
            else
            {
                item.SetTo(enumer);
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            MasterReferences masterReferences,
            BinaryMasterParseDelegate transl)
        {
            ParseRepeatedItem(
                frame: frame,
                item: item,
                triggeringRecord: triggeringRecord,
                lengthLength: lengthLength,
                transl: (MutagenFrame r, out T i) =>
                {
                    return transl(r, out i, masterReferences);
                });
        }
        #endregion

        #region Lengthed Triggering Records
        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            long lengthLength,
            BinarySubParseDelegate transl,
            ICollectionGetter<RecordType> triggeringRecord)
        {
            this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                item: item,
                lengthLength: lengthLength,
                transl: (MutagenFrame reader, RecordType header, out T subItem)
                    => transl(reader, out subItem));
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            long lengthLength,
            BinarySubParseRecordDelegate transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            var enumer = ParseRepeatedItem(
                frame,
                lengthLength,
                transl: transl,
                triggeringRecord: triggeringRecord);
            if (item is ISetList<T> setList)
            {
                setList.SetTo(enumer);
            }
            else
            {
                item.SetTo(enumer);
            }
        }
        #endregion

        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            BinarySubParseDelegate transl)
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

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            BinarySubParseDelegate transl)
        {
            var enumer = ParseRepeatedItem(
                frame,
                transl: transl);
            if (item is ISetList<T> setList)
            {
                setList.SetTo(enumer);
            }
            else
            {
                item.SetTo(enumer);
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            MasterReferences masterReferences,
            BinaryMasterParseDelegate transl)
        {
            ParseRepeatedItem(
                frame: frame,
                item: item,
                transl: (MutagenFrame r, out T i) =>
                {
                    return transl(r, out i, masterReferences);
                });
        }

        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            int amount,
            BinarySubParseDelegate transl)
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

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            int amount,
            BinarySubParseDelegate transl)
        {
            var enumer = ParseRepeatedItem(
                frame,
                amount: amount,
                transl: transl);
            if (item is ISetList<T> setList)
            {
                setList.SetTo(enumer);
            }
            else
            {
                item.SetTo(enumer);
            }
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            BinarySubWriteDelegate transl)
        {
            foreach (var item in items)
            {
                transl(writer, item);
            }
        }

        public void Write(
            MutagenWriter writer,
            IReadOnlyList<T> items,
            RecordType recordType,
            BinarySubWriteDelegate transl)
        {
            this.WriteRecordList(
                writer: writer,
                items: items,
                recordType: recordType,
                transl: transl);
        }

        private void WriteRecordList(
            MutagenWriter writer,
            IReadOnlyList<T> items,
            RecordType recordType,
            BinarySubWriteDelegate transl)
        {
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                foreach (var item in items)
                {
                    transl(writer, item);
                }
            }
        }
    }

    public class ListAsyncBinaryTranslation<T>
    {
        public static readonly ListAsyncBinaryTranslation<T> Instance = new ListAsyncBinaryTranslation<T>();

        public delegate Task<TryGet<T>> BinarySubParseDelegate(MutagenFrame reader);
        public delegate Task<TryGet<T>> BinaryMasterParseDelegate(
            MutagenFrame reader,
            MasterReferences masterReferences);
        public delegate Task<TryGet<T>> BinarySubParseRecordDelegate(
            MutagenFrame reader,
            RecordType header);
        public static readonly bool IsLoqui;

        static ListAsyncBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        #region Out Parameters
        public async Task<TryGet<IEnumerable<T>>> ParseRepeatedItem(
            MutagenFrame frame,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate transl)
        {
            var ret = new List<T>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, lengthLength, triggeringRecord)) break;
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
            return TryGet<IEnumerable<T>>.Succeed(ret);
        }

        public async Task<TryGet<IEnumerable<T>>> ParseRepeatedItemThreaded(
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
            return TryGet<IEnumerable<T>>.Succeed(
                ret.Where(i => i.Succeeded)
                    .Select(i => i.Value));
        }
        #endregion

        #region Lengthed Triggering Record
        public async Task ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate transl,
            bool thread = false)
        {
            TryGet<IEnumerable<T>> items;
            if (thread)
            {
                items = await ParseRepeatedItemThreaded(
                    frame,
                    triggeringRecord,
                    transl: transl).ConfigureAwait(false);
            }
            else
            {
                items = await ParseRepeatedItem(
                    frame,
                    triggeringRecord,
                    lengthLength,
                    transl: transl).ConfigureAwait(false);
            }
            if (items.Succeeded)
            {
                item.SetTo(items.Value);
            }
            else
            {
                item.Clear();
            }
        }
        #endregion

        #region Cache Helpers
        public async Task ParseRepeatedItem<K>(
            MutagenFrame frame,
            ICache<T, K> item,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate transl)
        {
            var items = await ParseRepeatedItem(
                frame,
                triggeringRecord,
                lengthLength,
                transl: transl).ConfigureAwait(false);
            if (items.Succeeded)
            {
                item.SetTo(items.Value);
            }
            else
            {
                item.Clear();
            }
        }
        #endregion
    }
}
