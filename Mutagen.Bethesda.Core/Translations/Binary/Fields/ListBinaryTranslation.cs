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
                var startingPos = frame.Position;
                MutagenFrame subFrame;
                if (!IsLoqui)
                {
                    var subHeader = frame.ReadSubrecord();
                    subFrame = frame.ReadAndReframe(subHeader.ContentLength);
                }
                else
                {
                    subFrame = frame;
                }
                if (transl(subFrame, out var subItem))
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
            BinaryMasterParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType>? triggeringRecord = null,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                var nextRecord = HeaderTranslation.GetNextRecordType(frame.Reader);
                nextRecord = recordTypeConverter.ConvertToStandard(nextRecord);
                if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, nextRecord, out var subIitem, recordTypeConverter))
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
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            triggeringRecord = recordTypeConverter.ConvertToCustom(triggeringRecord);
            while (!frame.Complete && !frame.Reader.Complete)
            {
                if (!HeaderTranslation.TryGetRecordType(frame.Reader, triggeringRecord)) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                var startingPos = frame.Position;
                if (transl(frame, out var subItem, recordTypeConverter))
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

        public IEnumerable<T> ParseParallel(
            MutagenFrame frame,
            RecordType triggeringRecord,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var frames = new List<MutagenFrame>();
            triggeringRecord = recordTypeConverter.ConvertToCustom(triggeringRecord);
            while (!frame.Complete && !frame.Reader.Complete)
            {
                var header = frame
                    .MetaData.GetNextRecordVariableMeta(frame.Reader);
                if (header.RecordType != triggeringRecord) break;
                if (!IsLoqui)
                {
                    throw new NotImplementedException();
                }
                var totalLen = header.TotalLength;
                var subFrame = new MutagenFrame(frame.ReadAndReframe(checked((int)totalLen)));
                frames.Add(subFrame);
            }
            var ret = new TryGet<T>[frames.Count];
            Parallel.ForEach(frames, (subFrame, state, count) =>
            {
                if (transl(subFrame, out var subItem, recordTypeConverter))
                {
                    ret[count] = TryGet<T>.Succeed(subItem);
                }
                else
                {
                    ret[count] = TryGet<T>.Failure;
                }
            });
            return ret.Where(i => i.Succeeded)
                .Select(i => i.Value);
        }

        public IEnumerable<T> Parse(
            MutagenFrame frame,
            RecordType triggeringRecord,
            bool thread,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (thread)
            {
                return ParseParallel(
                    frame,
                    triggeringRecord,
                    transl,
                    recordTypeConverter);
            }
            else
            {
                return Parse(
                    frame,
                    triggeringRecord,
                    transl,
                    recordTypeConverter);
            }
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
            BinaryMasterParseDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord,
            RecordTypeConverter? recordTypeConverter = null)
        {
            return this.Parse(
                frame: frame,
                triggeringRecord: triggeringRecord,
                transl: (MutagenFrame reader, RecordType header, out T subItem, RecordTypeConverter? r)
                    => transl(reader, out subItem, r),
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
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            while (!frame.Complete)
            {
                if (transl(frame, out var subItem, recordTypeConverter))
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
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                if (transl(frame, out var subItem, recordTypeConverter))
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
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var subHeader = frame.GetSubrecord();
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
                if (transl(frame, out var subIitem, recordTypeConverter))
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

        public IEnumerable<T> ParsePerItem(
            MutagenFrame frame,
            int amount,
            RecordType triggeringRecord,
            BinaryMasterParseDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            var startingPos = frame.Position;
            for (int i = 0; i < amount; i++)
            {
                var subHeader = frame.GetSubrecord();
                if (subHeader.RecordType != triggeringRecord)
                {
                    throw new ArgumentException($"Unexpected record encountered.");
                }
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                if (transl(frame, out var subIitem, recordTypeConverter))
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

        public IEnumerable<T> ParsePerItem(
            MutagenFrame frame,
            int amount,
            BinaryMasterParseDelegate<T> transl,
            ICollectionGetter<RecordType>? triggeringRecord = null,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new List<T>();
            var startingPos = frame.Position;
            for (int i = 0; i < amount; i++)
            {
                var nextRecord = HeaderTranslation.GetNextRecordType(frame.Reader);
                if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }
                if (transl(frame, out var subIitem, recordTypeConverter))
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
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                transl(writer, item, recordTypeConverter);
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
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                foreach (var item in items)
                {
                    transl(writer, item, recordTypeConverter);
                }
            }
        }

        public void Write(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType recordType,
            int countLengthLength,
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                switch (countLengthLength)
                {
                    case 2:
                        writer.Write(checked((ushort)items.Count));
                        break;
                    case 4:
                        writer.Write(items.Count);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                foreach (var item in items)
                {
                    transl(writer, item, recordTypeConverter);
                }
            }
        }

        public void Write(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            int countLengthLength,
            BinaryMasterWriteDelegate<T> transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            switch (countLengthLength)
            {
                case 2:
                    writer.Write(checked((ushort)items.Count));
                    break;
                case 4:
                    writer.Write(items.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
            foreach (var item in items)
            {
                transl(writer, item, recordTypeConverter);
            }
        }

        public void Write(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            int countLengthLength,
            BinarySubWriteDelegate<T> transl)
        {
            if (items == null) return;
            switch (countLengthLength)
            {
                case 2:
                    writer.Write(checked((ushort)items.Count));
                    break;
                case 4:
                    writer.Write(items.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
            foreach (var item in items)
            {
                transl(writer, item);
            }
        }

        public void WritePerItem(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
        {
            if (items == null) return;
            foreach (var item in items)
            {
                using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
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
            BinarySubWriteDelegate<T> transl,
            byte counterLength)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count, counterLength);
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
            BinaryMasterWriteDelegate<T> transl,
            byte counterLength,
            bool subRecordPerItem = false,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count, counterLength);
            }
            if (subRecordPerItem)
            {
                foreach (var item in items)
                {
                    using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                    {
                        transl(writer, item, recordTypeConverter);
                    }
                }
            }
            else
            {
                using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                {
                    foreach (var item in items)
                    {
                        transl(writer, item, recordTypeConverter);
                    }
                }
            }
        }

        public void WriteWithCounter(
            MutagenWriter writer,
            IReadOnlyList<T>? items,
            RecordType counterType,
            BinaryMasterWriteDelegate<T> transl,
            byte counterLength,
            RecordTypeConverter? recordTypeConverter = null)
        {
            if (items == null) return;
            using (HeaderExport.ExportHeader(writer, counterType, ObjectType.Subrecord))
            {
                writer.Write(items.Count, counterLength);
            }
            foreach (var item in items)
            {
                transl(writer, item, recordTypeConverter);
            }
        }

        #region Cache Helpers
        public void Parse<K>(
            MutagenFrame frame,
            ICache<T, K> item,
            RecordType triggeringRecord,
            BinarySubParseDelegate<T> transl)
        {
            item.SetTo(
                Parse(
                    frame,
                    triggeringRecord,
                    transl: transl));
        }
        #endregion
    }

    public class ListAsyncBinaryTranslation<T>
    {
        public static readonly ListAsyncBinaryTranslation<T> Instance = new ListAsyncBinaryTranslation<T>();

        public delegate Task<TryGet<T>> BinarySubParseDelegate(MutagenFrame reader);
        public delegate Task<TryGet<T>> BinaryMasterParseDelegate(
            MutagenFrame reader,
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
                var item = await transl(frame, recordTypeConverter).ConfigureAwait(false);
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
                var nextRec = HeaderTranslation.GetNextSubrecordType(
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
            BinaryMasterParseDelegate transl,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var tasks = new List<Task<TryGet<T>>>();
            while (!frame.Complete && !frame.Reader.Complete)
            {
                var nextRec = HeaderTranslation.GetNextSubrecordType(
                    reader: frame.Reader,
                    contentLength: out var contentLen);
                if (nextRec != triggeringRecord) break;
                if (!IsLoqui)
                {
                    frame.Position += frame.MetaData.SubConstants.HeaderLength;
                }

                var toDo = transl(frame, recordTypeConverter);

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
                    transl: transl,
                    recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
            }
            else
            {
                items = await Parse(
                    frame,
                    triggeringRecord,
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
