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

namespace Mutagen.Bethesda.Binary
{
    public abstract class ContainerBinaryTranslation<T> : IBinaryTranslation<IEnumerable<T>>
    {
        public static readonly bool IsLoqui;

        static ContainerBinaryTranslation()
        {
            IsLoqui = typeof(T).InheritsFrom(typeof(ILoquiObject));
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl,
            bool parseIndefinitely = false)
        {
            var safeFrame = frame.Spawn(snapToFinalPosition: false);
            var ret = new List<T>();
            int i = 0;
            while ((parseIndefinitely || !frame.Complete) && !frame.Reader.Complete)
            {
                try
                {
                    errorMask?.PushIndex(i++);
                    if (!HeaderTranslation.TryGetRecordType(safeFrame.Reader, lengthLength, triggeringRecord)) break;
                    if (!IsLoqui)
                    {
                        safeFrame.Position += Constants.SUBRECORD_LENGTH;
                    }
                    var startingPos = frame.Position;
                    if (transl(safeFrame, out var subItem, errorMask))
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
                finally
                {
                    errorMask?.PopIndex();
                }
            }
            item = ret;
            return true;
        }
        
        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl,
            bool parseIndefinitely = false)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (ParseRepeatedItem(
                    frame,
                    out var enumer,
                    triggeringRecord,
                    lengthLength,
                    errorMask: errorMask,
                    transl: transl,
                    parseIndefinitely: parseIndefinitely))
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            RecordType triggeringRecord,
            int lengthLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseDelegate<T> transl,
            bool parseIndefinitely = false)
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
                },
                parseIndefinitely: parseIndefinitely);
        }

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            var safeFrame = frame.Spawn(snapToFinalPosition: false);
            int i = 0;
            var ret = new List<T>();
            while (!frame.Complete)
            {
                try
                {
                    errorMask?.PushIndex(i++);
                    var nextRecord = HeaderTranslation.GetNextRecordType(frame.Reader);
                    if (!triggeringRecord?.Contains(nextRecord) ?? false) break;
                    if (!IsLoqui)
                    {
                        safeFrame.Position += Constants.SUBRECORD_LENGTH;
                    }
                    var startingPos = frame.Position;
                    if (transl(safeFrame, nextRecord, out var subIitem, errorMask))
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
                finally
                {
                    errorMask?.PopIndex();
                }
            }
            enumer = ret;
            return true;
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl,
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
            INotifyingCollection<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseDelegate<T> transl,
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

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl)
        {
            using (frame)
            {
                var safeFrame = frame.Spawn(snapToFinalPosition: false);
                var ret = new List<T>();
                int i = 0;
                while (!safeFrame.Complete)
                {
                    try
                    {
                        errorMask?.PushIndex(i++);
                        if (transl(safeFrame, out var subItem, errorMask))
                        {
                            ret.Add(subItem);
                        }
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                    finally
                    {
                        errorMask?.PopIndex();
                    }
                }
                enumer = ret;
                return true;
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            long lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            long lengthLength,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseDelegate<T> transl)
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

        public bool ParseRepeatedItem(
            MutagenFrame frame,
            out IEnumerable<T> enumer,
            int amount,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl)
        {
            var ret = new List<T>();
            var safeFrame = frame.Spawn(snapToFinalPosition: false);
            for (int i = 0; i < amount; i++)
            {
                try
                {
                    errorMask?.PushIndex(i);
                    if (transl(safeFrame, out var subItem, errorMask))
                    {
                        ret.Add(subItem);
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
            enumer = ret;
            return true;
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            int amount,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void ParseRepeatedItem(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingCollection<T> item,
            int amount,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            BinaryMasterParseDelegate<T> transl)
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

        #region NotifyingKeyedCollection Helpers
        public void ParseRepeatedItem<K>(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingKeyedCollection<K, T> item,
            RecordType triggeringRecord,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void ParseRepeatedItem<K>(
            MutagenFrame frame,
            int fieldIndex,
            INotifyingKeyedCollection<K, T> item,
            int lengthLength,
            ErrorMaskBuilder errorMask,
            BinarySubParseRecordDelegate<T> transl,
            ICollectionGetter<RecordType> triggeringRecord = null)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
                    item.Unset();
                }
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                errorMask.ReportException(ex);
            }
            finally
            {
                errorMask?.PopIndex();
            }
        }
        #endregion

        void IBinaryTranslation<IEnumerable<T>>.Write(
            MutagenWriter writer,
            IEnumerable<T> item,
            long length,
            ErrorMaskBuilder errorMask)
        {
            Write(writer, item, errorMask);
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            ErrorMaskBuilder errorMask)
        {
            var transl = BinaryTranslator<T>.Translator;
            if (transl.Item.Failed)
            {
                errorMask.ReportExceptionOrThrow(
                    new ArgumentException($"No binary Translator available for {typeof(T)}. {transl.Item.Reason}"));
            }
            this.Write(
                writer: writer,
                items: items,
                errorMask: errorMask,
                transl: (MutagenWriter subWriter, T item1, ErrorMaskBuilder errMask2)
                    => transl.Item.Value.Write(
                        writer: subWriter,
                        item: item1,
                        length: -1,
                        errorMask: errMask2));
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            int i = 0;
            foreach (var item in items)
            {
                try
                {
                    errorMask?.PushIndex(i++);
                    this.WriteSingleItem(writer, transl, item, errorMask);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
        }

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<IEnumerable<T>> item,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
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
            INotifyingCollection<T> items,
            int fieldIndex,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void WriteListOfRecords(
            MutagenWriter writer,
            IEnumerable<T> items,
            int fieldIndex,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> items,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        private void Write(
            MutagenWriter writer,
            INotifyingCollection<T> items,
            RecordType recordType,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        private void WriteRecordList(
            MutagenWriter writer,
            INotifyingCollection<T> items,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            if (!items.HasBeenSet) return;
            using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
            {
                int i = 0;
                foreach (var item in items)
                {
                    try
                    {
                        errorMask?.PushIndex(i++);
                        this.WriteSingleItem(writer, transl, item, errorMask);
                    }
                    catch (Exception ex)
                    when (errorMask != null)
                    {
                        errorMask.ReportException(ex);
                    }
                    finally
                    {
                        errorMask?.PopIndex();
                    }
                }
            }
        }

        private void WriteListOfRecords(
            MutagenWriter writer,
            IEnumerable<T> items,
            RecordType recordType,
            ErrorMaskBuilder errorMask,
            BinarySubWriteDelegate<T> transl)
        {
            int i = 0;
            foreach (var item in items)
            {
                try
                {
                    errorMask?.PushIndex(i++);
                    if (IsLoqui)
                    {
                        this.WriteSingleItem(writer, transl, item, errorMask);
                    }
                    else
                    {
                        using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                        {
                            this.WriteSingleItem(writer, transl, item, errorMask);
                        }
                    }
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
        }

        public abstract void WriteSingleItem(
            MutagenWriter writer,
            BinarySubWriteDelegate<T> transl,
            T item,
            ErrorMaskBuilder errorMask);

        bool IBinaryTranslation<IEnumerable<T>>.Parse(
            MutagenFrame reader,
            out IEnumerable<T> item,
            ErrorMaskBuilder errorMask)
        {
            throw new NotImplementedException();
        }
    }
}
