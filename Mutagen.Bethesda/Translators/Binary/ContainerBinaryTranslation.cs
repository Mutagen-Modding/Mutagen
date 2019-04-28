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
    public delegate bool BinarySubParseDelegate<T>(
        MutagenFrame reader,
        out T item);
    public delegate bool BinarySubParseErrDelegate<T>(
        MutagenFrame reader,
        out T item,
        ErrorMaskBuilder errorMask);
    public delegate bool BinaryMasterParseErrDelegate<T>(
        MutagenFrame reader,
        out T item,
        MasterReferences masterReferences,
        ErrorMaskBuilder errorMask);
    public delegate bool BinaryMasterParseDelegate<T>(
        MutagenFrame reader,
        out T item,
        MasterReferences masterReferences);
    public delegate bool BinarySubParseRecordDelegate<T>(
        MutagenFrame reader,
        RecordType header,
        out T item);
    public delegate bool BinarySubParseRecordErrDelegate<T>(
        MutagenFrame reader,
        RecordType header,
        out T item,
        ErrorMaskBuilder errorMask);
    public delegate void BinarySubWriteDelegate<in T>(
        MutagenWriter writer,
        T item);
    public delegate void BinarySubWriteErrDelegate<in T>(
        MutagenWriter writer,
        T item,
        ErrorMaskBuilder errorMask);
    public delegate void BinaryMasterWriteDelegate<in T>(
        MutagenWriter writer,
        T item,
        MasterReferences masterReferences,
        ErrorMaskBuilder errorMask);

    public abstract class ContainerBinaryTranslation<T>
    {
        public static readonly bool IsLoqui;

        static ContainerBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        #region Out Parameters
        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> item,
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
            BinarySubParseErrDelegate<T> transl)
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
            BinarySubParseRecordErrDelegate<T> transl,
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
            BinarySubParseRecordDelegate<T> transl,
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
            ISourceSetList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            BinarySubParseDelegate<T> transl)
        {
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                triggeringRecord,
                lengthLength,
                transl: transl))
            {
                item.SetTo(enumer);
            }
            else
            {
                item.Clear();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            ISourceSetList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate<T> transl)
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
                        item.SetTo(enumer);
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
            int fieldIndex,
            ISourceSetList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseErrDelegate<T> transl)
        {
            ParseRepeatedItem(
                frame: frame,
                fieldIndex: fieldIndex,
                item: item,
                triggeringRecord: triggeringRecord,
                lengthLength: lengthLength,
                errorMask: errorMask,
                transl: (MutagenFrame r, out T i, ErrorMaskBuilder err) =>
                {
                    return transl(r, out i, masterReferences, err);
                });
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            ISourceSetList<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            MasterReferences masterReferences,
            BinaryMasterParseDelegate<T> transl)
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
            ISourceSetList<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate<T> transl,
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
            ISourceSetList<T> item,
            long lengthLength,
            BinarySubParseDelegate<T> transl,
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
            int fieldIndex,
            ISourceSetList<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordErrDelegate<T> transl,
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
                        item.SetTo(enumer);
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
            ISourceSetList<T> item,
            long lengthLength,
            BinarySubParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                lengthLength,
                transl: transl,
                triggeringRecord: triggeringRecord))
            {
                item.SetTo(enumer);
            }
            else
            {
                item.Clear();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            ISourceSetList<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            BinaryMasterParseDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            this.ParseRepeatedItem(
                frame: frame,
                item: item,
                lengthLength: lengthLength,
                triggeringRecord: triggeringRecord,
                transl: (MutagenFrame r, out T i) =>
                {
                    return transl(r, out i, masterReferences);
                });
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            ISourceSetList<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseErrDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            this.ParseRepeatedItem(
                frame: frame,
                fieldIndex: fieldIndex,
                item: item,
                lengthLength: lengthLength,
                errorMask: errorMask,
                triggeringRecord: triggeringRecord,
                transl: (MutagenFrame r, out T i, ErrorMaskBuilder err) =>
                {
                    return transl(r, out i, masterReferences, err);
                });
        }
        #endregion

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate<T> transl)
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
            enumer = ret;
            return true;
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            ISourceSetList<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate<T> transl)
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
                        item.SetTo(enumer);
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
            ISourceSetList<T> item,
            long lengthLength,
            BinarySubParseDelegate<T> transl)
        {
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                lengthLength,
                transl: transl))
            {
                item.SetTo(enumer);
            }
            else
            {
                item.Clear();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            ISourceSetList<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseErrDelegate<T> transl)
        {
            ParseRepeatedItem(
                frame: frame,
                fieldIndex: fieldIndex,
                item: item,
                lengthLength: lengthLength,
                errorMask: errorMask,
                transl: (MutagenFrame r, out T i, ErrorMaskBuilder err) =>
                {
                    return transl(r, out i, masterReferences, err);
                });
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            ISourceSetList<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            BinaryMasterParseDelegate<T> transl)
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
            BinarySubParseErrDelegate<T> transl)
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
            enumer = ret;
            return true;
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            ISourceSetList<T> item,
            int amount,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate<T> transl)
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
                        item.SetTo(enumer);
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
            ISourceSetList<T> item,
            int amount,
            BinarySubParseDelegate<T> transl)
        {
            if (ParseRepeatedItem(
                frame,
                out var enumer,
                amount: amount,
                transl: transl))
            {
                item.SetTo(enumer);
            }
            else
            {
                item.Clear();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            ISourceSetList<T> item,
            int amount,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseErrDelegate<T> transl)
        {
            this.ParseRepeatedItem(
                frame: frame,
                fieldIndex: fieldIndex,
                item: item,
                amount: amount,
                errorMask: errorMask,
                transl: (MutagenFrame r, out T i, ErrorMaskBuilder err) =>
                {
                    return transl(r, out i, masterReferences, err);
                });
        }

        #region Cache Helpers
        public void ParseRepeatedItem<K>(
            MutagenFrame frame,
            int fieldIndex,
            CSharpExt.Rx.ISourceSetCache<T, K> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseErrDelegate<T> transl)
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
                        item.SetTo(enumer);
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

        public void ParseRepeatedItem<K>(
            MutagenFrame frame,
            int fieldIndex,
            CSharpExt.Rx.ISourceSetCache<T, K> item,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordErrDelegate<T> transl,
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
                        item.SetTo(enumer);
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

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
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
            BinarySubWriteDelegate<T> transl)
        {
            foreach (var item in items)
            {
                transl(writer, item);
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<IEnumerable<T>> item,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                items: item.Item,
                errorMask: errorMask,
                transl: transl);
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<IEnumerable<T>> item,
            BinarySubWriteDelegate<T> transl)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                items: item.Item,
                transl: transl);
        }

        public void Write(
            MutagenWriter writer,
            ISourceSetList<T> items,
            int fieldIndex,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
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
            ISourceSetList<T> items,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
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
            BinarySubWriteErrDelegate<T> transl)
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

        private void Write(
            MutagenWriter writer,
            ISourceSetList<T> items,
            RecordType recordType,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
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

        private void WriteRecordList(
            MutagenWriter writer,
            ISourceSetList<T> items,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
        {
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                int i = 0;
                foreach (var item in items.Items)
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
            ISourceSetList<T> items,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
        {
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                foreach (var item in items.Items)
                {
                    transl(writer, item);
                }
            }
        }

        public void WriteListOfRecords(
            MutagenWriter writer,
            IEnumerable<T> items,
            int fieldIndex,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    this.WriteListOfRecords(
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

        private void WriteListOfRecords(
            MutagenWriter writer,
            IEnumerable<T> items,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteErrDelegate<T> transl)
        {
            int i = 0;
            foreach (var item in items)
            {
                using (errorMask.PushIndex(i++))
                {
                    try
                    {
                        if (IsLoqui)
                        {
                            transl(writer, item, errorMask);
                        }
                        else
                        {
                            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                            {
                                transl(writer, item, errorMask);
                            }
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                }
            }
        }

        private void WriteListOfRecords(
            MutagenWriter writer,
            IEnumerable<T> items,
            RecordType recordType,
            BinarySubWriteDelegate<T> transl)
        {
            foreach (var item in items)
            {
                if (IsLoqui)
                {
                    transl(writer, item);
                }
                else
                {
                    using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                    {
                        transl(writer, item);
                    }
                }
            }
        }
    }
}
