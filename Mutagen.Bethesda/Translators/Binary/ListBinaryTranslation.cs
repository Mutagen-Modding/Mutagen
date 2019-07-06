using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Noggog.Notifying;
using Mutagen.Bethesda.Binary;
using Loqui;
using System.IO;
using Loqui.Internal;
using DynamicData;
using CSharpExt.Rx;

namespace Mutagen.Bethesda.Binary
{
    public class ListBinaryTranslation<T>
    {
        public static readonly ListBinaryTranslation<T> Instance = new ListBinaryTranslation<T>();

        public delegate bool BinarySubParseDelegate(
            MutagenFrame reader,
            out T item);
        public delegate bool BinarySubParseErrDelegate(
            MutagenFrame reader,
            out T item,
            ErrorMaskBuilder errorMask);
        public delegate bool BinaryMasterParseErrDelegate(
            MutagenFrame reader,
            out T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);
        public delegate bool BinaryMasterParseDelegate(
            MutagenFrame reader,
            out T item,
            MasterReferences masterReferences);
        public delegate bool BinarySubParseRecordDelegate(
            MutagenFrame reader,
            RecordType header,
            out T item);
        public delegate bool BinarySubParseRecordErrDelegate(
            MutagenFrame reader,
            RecordType header,
            out T item,
            ErrorMaskBuilder errorMask);
        public delegate void BinarySubWriteDelegate(
            MutagenWriter writer,
            T item);
        public delegate void BinarySubWriteErrDelegate(
            MutagenWriter writer,
            T item,
            ErrorMaskBuilder errorMask);
        public delegate void BinaryMasterWriteDelegate(
            MutagenWriter writer,
            T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);

        public static readonly bool IsLoqui;

        static ListBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        #region Out Parameters
        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> item,
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
                    frame.Position += Constants.SUBRECORD_LENGTH;
                }
                var startingPos = frame.Position;
                if (transl(frame, out var subItem))
                {
                    ret.Add(subItem);
                }

                if (frame.Position == startingPos)
                {
                    frame.Position += Constants.SUBRECORD_LENGTH;
                    throw new ArgumentException($"Parsed item on the list consumed no data: {subItem}");
                }
            }
            item = ret;
            return true;
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            var ret = new List<T>();
            int i = 0;
            while (!frame.Complete && !frame.Reader.Complete)
            {
                using (errorMask.PushIndex(i++))
                {
                    try
                    {
                        if (!HeaderTranslation.TryGetRecordType(frame.Reader, lengthLength, triggeringRecord)) break;
                        if (!IsLoqui)
                        {
                            frame.Position += Constants.SUBRECORD_LENGTH;
                        }
                        var startingPos = frame.Position;
                        if (transl(frame, out var subItem, errorMask))
                        {
                            ret.Add(subItem);
                        }

                        if (frame.Position == startingPos)
                        {
                            frame.Position += Constants.SUBRECORD_LENGTH;
                            throw new ArgumentException($"Parsed item on the list consumed no data: {subItem}");
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
            item = ret;
            return true;
        }


        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordErrDelegate transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            var ret = new List<T>();
            int i = 0;
            while (!frame.Complete)
            {
                using (errorMask.PushIndex(i++))
                {
                    try
                    {
                        var nextRecord = HeaderTranslation.GetNextRecordType(frame.Reader);
                        if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
                        if (!IsLoqui)
                        {
                            frame.Position += Constants.SUBRECORD_LENGTH;
                        }
                        var startingPos = frame.Position;
                        if (transl(frame, nextRecord, out var subIitem, errorMask))
                        {
                            ret.Add(subIitem);
                        }
                        if (frame.Position == startingPos)
                        {
                            errorMask.ReportExceptionOrThrow(
                                new ArgumentException($"Parsed item on the list consumed no data: {subIitem}"));
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
            enumer = ret;
            return true;
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
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
                    frame.Position += Constants.SUBRECORD_LENGTH;
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
            enumer = ret;
            return true;
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
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                triggeringRecord,
                lengthLength,
                transl: transl))
            {
                if (item is ISetList<T> setList)
                {
                    setList.SetTo(enumer);
                }
                else
                {
                    item.SetTo(enumer);
                }
            }
            else
            {
                item.Clear();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            IList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (ParseRepeatedItem(
                        frame,
                        out var enumer,
                        triggeringRecord,
                        lengthLength,
                        errorMask: errorMask,
                        transl: transl))
                    {
                        if (item is ISetList<T> setList)
                        {
                            setList.SetTo(enumer);
                        }
                        else
                        {
                            item.SetTo(enumer);
                        }
                    }
                    else
                    {
                        item.Clear();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
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
            int fieldIndex,
            IList<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl,
            ICollectionGetter<RecordType> triggeringRecord)
        {
            this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                fieldIndex: fieldIndex,
                item: item,
                lengthLength: lengthLength,
                errorMask: errorMask,
                transl: (MutagenFrame reader, RecordType header, out T subItem, ErrorMaskBuilder subErrMask)
                    => transl(reader, out subItem, subErrMask));
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            IList<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordErrDelegate transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (ParseRepeatedItem(
                        frame,
                        out var enumer,
                        lengthLength,
                        errorMask: errorMask,
                        transl: transl,
                        triggeringRecord: triggeringRecord))
                    {
                        if (item is ISetList<T> setList)
                        {
                            setList.SetTo(enumer);
                        }
                        else
                        {
                            item.SetTo(enumer);
                        }
                    }
                    else
                    {
                        item.Clear();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }
        #endregion

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            var ret = new List<T>();
            int i = 0;
            while (!frame.Complete)
            {
                using (errorMask.PushIndex(i++))
                {
                    try
                    {
                        if (transl(frame, out var subItem, errorMask))
                        {
                            ret.Add(subItem);
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
            enumer = ret;
            return true;
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            long lengthLength,
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
            enumer = ret;
            return true;
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            IList<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (ParseRepeatedItem(
                        frame,
                        out var enumer,
                        lengthLength,
                        errorMask: errorMask,
                        transl: transl))
                    {
                        if (item is ISetList<T> setList)
                        {
                            setList.SetTo(enumer);
                        }
                        else
                        {
                            item.SetTo(enumer);
                        }
                    }
                    else
                    {
                        item.Clear();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            long lengthLength,
            BinarySubParseDelegate transl)
        {
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                lengthLength,
                transl: transl))
            {
                if (item is ISetList<T> setList)
                {
                    setList.SetTo(enumer);
                }
                else
                {
                    item.SetTo(enumer);
                }
            }
            else
            {
                item.Clear();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            BinaryMasterParseDelegate transl)
        {
            ParseRepeatedItem(
                frame: frame,
                item: item,
                lengthLength: lengthLength,
                transl: (MutagenFrame r, out T i) =>
                {
                    return transl(r, out i, masterReferences);
                });
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            int amount,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            var ret = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                using (errorMask.PushIndex(i))
                {
                    try
                    {
                        if (transl(frame, out var subItem, errorMask))
                        {
                            ret.Add(subItem);
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
            enumer = ret;
            return true;
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
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
            enumer = ret;
            return true;
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            IList<T> item,
            int amount,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (ParseRepeatedItem(
                        frame,
                        out var enumer,
                        amount: amount,
                        errorMask: errorMask,
                        transl: transl))
                    {
                        if (item is ISetList<T> setList)
                        {
                            setList.SetTo(enumer);
                        }
                        else
                        {
                            item.SetTo(enumer);
                        }
                    }
                    else
                    {
                        item.Clear();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            IList<T> item,
            int amount,
            BinarySubParseDelegate transl)
        {
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                amount: amount,
                transl: transl))
            {
                if (item is ISetList<T> setList)
                {
                    setList.SetTo(enumer);
                }
                else
                {
                    item.SetTo(enumer);
                }
            }
            else
            {
                item.Clear();
            }
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate transl)
        {
            int i = 0;
            foreach (var item in items)
            {
                using (errorMask.PushIndex(i++))
                {
                    try
                    {
                        transl(writer, item, errorMask);
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
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
            int fieldIndex,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    this.WriteRecordList(
                        writer: writer,
                        items: items,
                        recordType: recordType,
                        errorMask: errorMask,
                        transl: transl);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask?.ReportException(ex);
                }
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

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    this.Write(
                        writer: writer,
                        items: items,
                        errorMask: errorMask,
                        transl: transl);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask?.ReportException(ex);
                }
            }
        }

        private void WriteRecordList(
            MutagenWriter writer,
            IReadOnlyList<T> items,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate transl)
        {
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                int i = 0;
                foreach (var item in items)
                {
                    using (errorMask.PushIndex(i++))
                    {
                        try
                        {
                            transl(writer, item, errorMask);
                        }
                        catch (Exception ex)
                        when (errorMask != null)
                        {
                            errorMask.ReportException(ex);
                        }
                    }
                }
            }
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
        public delegate Task<TryGet<T>> BinarySubParseErrDelegate(
            MutagenFrame reader,
            ErrorMaskBuilder errorMask);
        public delegate Task<TryGet<T>> BinaryMasterParseErrDelegate(
            MutagenFrame reader,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);
        public delegate Task<TryGet<T>> BinaryMasterParseDelegate(
            MutagenFrame reader,
            MasterReferences masterReferences);
        public delegate Task<TryGet<T>> BinarySubParseRecordDelegate(
            MutagenFrame reader,
            RecordType header);
        public delegate Task<TryGet<T>> BinarySubParseRecordErrDelegate(
            MutagenFrame reader,
            RecordType header,
            ErrorMaskBuilder errorMask);
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
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            var ret = new List<T>();
            int i = 0;
            while (!frame.Complete && !frame.Reader.Complete)
            {
                using (errorMask.PushIndex(i++))
                {
                    try
                    {
                        if (!HeaderTranslation.TryGetRecordType(frame.Reader, lengthLength, triggeringRecord)) break;
                        if (!IsLoqui)
                        {
                            frame.Position += Constants.SUBRECORD_LENGTH;
                        }
                        var startingPos = frame.Position;
                        var item = await transl(frame, errorMask).ConfigureAwait(false);
                        if (item.Succeeded)
                        {
                            ret.Add(item.Value);
                        }

                        if (frame.Position == startingPos)
                        {
                            frame.Position += Constants.SUBRECORD_LENGTH;
                            throw new ArgumentException($"Parsed item on the list consumed no data: {item.Value}");
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
            return TryGet<IEnumerable<T>>.Succeed(ret);
        }

        public async Task<TryGet<IEnumerable<T>>> ParseRepeatedItemThreaded(
            MutagenFrame frame,
            RecordType triggeringRecord,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            if (errorMask != null)
            {
                throw new NotImplementedException();
            }
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

                var toDo = transl(frame, errorMask);

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
            int fieldIndex,
            IList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl,
            bool thread = false)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    TryGet<IEnumerable<T>> items;
                    if (thread)
                    {
                        items = await ParseRepeatedItemThreaded(
                            frame,
                            triggeringRecord,
                            errorMask: errorMask,
                            transl: transl).ConfigureAwait(false);
                    }
                    else
                    {
                        items = await ParseRepeatedItem(
                            frame,
                            triggeringRecord,
                            lengthLength,
                            errorMask: errorMask,
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
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }
        #endregion

        #region Cache Helpers
        public async Task ParseRepeatedItem<K>(
            MutagenFrame frame,
            int fieldIndex,
            CSharpExt.Rx.ISourceSetCache<T, K> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    var items = await ParseRepeatedItem(
                        frame,
                        triggeringRecord,
                        lengthLength,
                        errorMask: errorMask,
                        transl: transl).ConfigureAwait(false);
                    if (items.Succeeded)
                    {
                        item.SetTo(items.Value);
                    }
                    else
                    {
                        item.Unset();
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
            }
        }
        #endregion
    }
}
