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
        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate<T> transl)
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
        #endregion

        #region Lengthed Triggering Record
        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            RecordType triggeringRecord,
            int lengthLength,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl)
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
                if (transl(frame, out var subItem, masterReferences))
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
        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            long lengthLength,
            BinarySubParseDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord)
        {
            return this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                lengthLength: lengthLength,
                transl: (MutagenFrame reader, RecordType header, out T subItem)
                    => transl(reader, out subItem));
        }
        #endregion

        public IEnumerable<T> ParseRepeatedItem(
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

        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                if (transl(frame, out var subItem, masterReferences))
                {
                    ret.Add(subItem);
                }
            }
            return ret;
        }

        public IEnumerable<T> ParseRepeatedItem(
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

        public IEnumerable<T> ParseRepeatedItem(
            MutagenFrame frame,
            int amount,
            RecordType triggeringRecord,
            MasterReferenceReader masterReferences,
            BinaryMasterParseDelegate<T> transl)
        {
            var ret = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, frame.MetaData.SubConstants.LengthLength, triggeringRecord)) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, out var subIitem, masterReferences))
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
            IReadOnlyList<T>? items,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
        {
            if (items == null) return;
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
            BinarySubWriteDelegate<T> transl)
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
            MasterReferenceReader masterReferences);
        public delegate Task<TryGet<T>> BinarySubParseRecordDelegate(
            MutagenFrame reader,
            RecordType header);
        public static readonly bool IsLoqui;

        static ListAsyncBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        public async Task<IEnumerable<T>> ParseRepeatedItem(
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
            return ret;
        }

        public async Task<IEnumerable<T>> ParseRepeatedItemThreaded(
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

        #region Lengthed Triggering Record
        public async Task<ExtendedList<T>> ParseRepeatedItem(
            MutagenFrame frame,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate transl,
            bool thread = false)
        {
            IEnumerable<T> items;
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
            return new ExtendedList<T>(items);
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
            item.SetTo(
                await ParseRepeatedItem(
                    frame,
                    triggeringRecord,
                    lengthLength,
                    transl: transl));
        }
        #endregion
    }
}
