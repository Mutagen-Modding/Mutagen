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

namespace Mutagen.Bethesda.Binary
{
    public abstract class ContainerBinaryTranslation<T, M> : IBinaryTranslation<IEnumerable<T>, MaskItem<Exception, IEnumerable<M>>>
    {
        TryGet<IEnumerable<T>> IBinaryTranslation<IEnumerable<T>, MaskItem<Exception, IEnumerable<M>>>.Parse(MutagenFrame reader, bool doMasks, out MaskItem<Exception, IEnumerable<M>> maskObj)
        {
            throw new NotImplementedException();
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem(
            MutagenFrame frame,
            bool doMasks,
            RecordType triggeringRecord,
            ObjectType objType,
            out MaskItem<Exception, IEnumerable<M>> errorMask,
            BinarySubParseDelegate<T, M> transl)
        {
            var safeFrame = frame.Spawn(snapToFinalPosition: false);
            try
            {
                List<M> maskList = null;
                var ret = new List<T>();
                while (!frame.Complete)
                {
                    if (!HeaderTranslation.TryGetRecordType(safeFrame, objType, triggeringRecord)) break;
                    var startingPos = frame.Position;
                    var get = transl(safeFrame, doMasks, out var subMaskObj);
                    if (get.Succeeded)
                    {
                        ret.Add(get.Value);
                    }
                    if (subMaskObj != null)
                    {
                        if (!doMasks)
                        { // This shouldn't actually throw, as subparse is expected to throw if doMasks is off
                            throw new ArgumentException("Error parsing list.  Could not parse subitem.");
                        }
                        if (maskList == null)
                        {
                            maskList = new List<M>();
                        }
                        maskList.Add(subMaskObj);
                    }

                    if (frame.Position == startingPos)
                    {
                        frame.Position += Constants.SUBRECORD_LENGTH;
                        throw new ArgumentException($"Parsed item on the list consumed no data: {get.Value}");
                    }
                }
                errorMask = maskList == null ? null : new MaskItem<Exception, IEnumerable<M>>(null, maskList);
                return TryGet<IEnumerable<T>>.Succeed(ret);
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, IEnumerable<M>>(ex, null);
                return TryGet<IEnumerable<T>>.Failure;
            }
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem(
            MutagenFrame frame,
            bool doMasks,
            ICollectionGetter<RecordType> triggeringRecord,
            ObjectType objType,
            out MaskItem<Exception, IEnumerable<M>> errorMask,
            BinarySubParseRecordDelegate<T, M> transl)
        {
            var safeFrame = frame.Spawn(snapToFinalPosition: false);
            try
            {
                List<M> maskList = null;
                var ret = new List<T>();
                while (!frame.Complete)
                {
                    var nextRecord = HeaderTranslation.GetNextRecordType(frame);
                    if (!triggeringRecord.Contains(nextRecord)) break;
                    var startingPos = frame.Position;
                    var get = transl(safeFrame, nextRecord, doMasks, out var subMaskObj);
                    if (get.Succeeded)
                    {
                        ret.Add(get.Value);
                    }
                    if (subMaskObj != null)
                    {
                        if (!doMasks)
                        { // This shouldn't actually throw, as subparse is expected to throw if doMasks is off
                            throw new ArgumentException("Error parsing list.  Could not parse subitem.");
                        }
                        if (maskList == null)
                        {
                            maskList = new List<M>();
                        }
                        maskList.Add(subMaskObj);
                    }

                    if (frame.Position == startingPos) throw new ArgumentException($"Parsed item on the list consumed no data: {get.Value}");
                }
                errorMask = maskList == null ? null : new MaskItem<Exception, IEnumerable<M>>(null, maskList);
                return TryGet<IEnumerable<T>>.Succeed(ret);
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, IEnumerable<M>>(ex, null);
                return TryGet<IEnumerable<T>>.Failure;
            }
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem(
            MutagenFrame frame,
            bool doMasks,
            ObjectType objType,
            out MaskItem<Exception, IEnumerable<M>> errorMask,
            BinarySubParseDelegate<T, M> transl)
        {
            try
            {
                using (frame)
                {
                    List<M> maskList = null;
                    var ret = new List<T>();
                    while (!frame.Complete)
                    {
                        var get = transl(frame, doMasks, out var subMaskObj);
                        if (get.Succeeded)
                        {
                            ret.Add(get.Value);
                        }
                        if (subMaskObj != null)
                        {
                            if (!doMasks)
                            { // This shouldn't actually throw, as subparse is expected to throw if doMasks is off
                                throw new ArgumentException("Error parsing list.  Could not parse subitem.");
                            }
                            if (maskList == null)
                            {
                                maskList = new List<M>();
                            }
                            maskList.Add(subMaskObj);
                        }
                    }
                    errorMask = maskList == null ? null : new MaskItem<Exception, IEnumerable<M>>(null, maskList);
                    return TryGet<IEnumerable<T>>.Succeed(ret);
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, IEnumerable<M>>(ex, null);
                return TryGet<IEnumerable<T>>.Failure;
            }
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem<Mask>(
            MutagenFrame frame,
            int fieldIndex,
            RecordType triggeringRecord,
            ObjectType objType,
            Func<Mask> errorMask,
            BinarySubParseDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                doMasks: errorMask != null,
                objType: objType,
                errorMask: out var err,
                transl: transl);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
            return ret;
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem<Mask>(
            MutagenFrame frame,
            int fieldIndex,
            ICollectionGetter<RecordType> triggeringRecord,
            ObjectType objType,
            Func<Mask> errorMask,
            BinarySubParseRecordDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                doMasks: errorMask != null,
                objType: objType,
                errorMask: out var err,
                transl: transl);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
            return ret;
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem<Mask>(
            MutagenFrame frame,
            int fieldIndex,
            ICollectionGetter<RecordType> triggeringRecord,
            ObjectType objType,
            Func<Mask> errorMask,
            BinarySubParseDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                doMasks: errorMask != null,
                objType: objType,
                errorMask: out var err,
                transl: (MutagenFrame reader, RecordType header, bool doMasks, out M maskObj) => transl(reader, doMasks, out maskObj));
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
            return ret;
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem<Mask>(
            MutagenFrame frame,
            int fieldIndex,
            ObjectType objType,
            Func<Mask> errorMask,
            BinarySubParseDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                doMasks: errorMask != null,
                objType: objType,
                errorMask: out var err,
                transl: transl);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
            return ret;
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem(
            MutagenFrame frame,
            bool doMasks,
            int amount,
            out MaskItem<Exception, IEnumerable<M>> errorMask,
            BinarySubParseDelegate<T, M> transl)
        {
            try
            {
                List<M> maskList = null;
                var ret = new List<T>();
                for (int i = 0; i < amount; i++)
                {
                    var get = transl(frame, doMasks, out var subMaskObj);
                    if (get.Succeeded)
                    {
                        ret.Add(get.Value);
                    }
                    else
                    {
                        if (!doMasks)
                        { // This shouldn't actually throw, as subparse is expected to throw if doMasks is off
                            throw new ArgumentException("Error parsing list.  Could not parse subitem.");
                        }
                        if (maskList == null)
                        {
                            maskList = new List<M>();
                        }
                        maskList.Add(subMaskObj);
                    }
                }
                errorMask = maskList == null ? null : new MaskItem<Exception, IEnumerable<M>>(null, maskList);
                return TryGet<IEnumerable<T>>.Succeed(ret);
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, IEnumerable<M>>(ex, null);
                return TryGet<IEnumerable<T>>.Failure;
            }
        }

        public TryGet<IEnumerable<T>> ParseRepeatedItem<Mask>(
            MutagenFrame frame,
            int fieldIndex,
            int amount,
            Func<Mask> errorMask,
            BinarySubParseDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                amount: amount,
                doMasks: errorMask != null,
                errorMask: out var err,
                transl: transl);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                err);
            return ret;
        }

        public abstract TryGet<T> ParseSingleItem(MutagenFrame frame, BinarySubParseDelegate<T, M> transl, bool doMasks, out M maskObj);

        void IBinaryTranslation<IEnumerable<T>, MaskItem<Exception, IEnumerable<M>>>.Write(MutagenWriter writer, IEnumerable<T> item, ContentLength length, bool doMasks, out MaskItem<Exception, IEnumerable<M>> maskObj)
        {
            Write(writer, item, doMasks, out maskObj);
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> item,
            bool doMasks,
            out MaskItem<Exception, IEnumerable<M>> maskObj)
        {
            try
            {
                var transl = BinaryTranslator<T, M>.Translator;
                if (transl.Item.Failed)
                {
                    throw new ArgumentException($"No XML Translator available for {typeof(T)}. {transl.Item.Reason}");
                }
                this.Write(
                    writer: writer,
                    item: item,
                    doMasks: doMasks,
                    maskObj: out maskObj,
                    transl: (T item1, bool internalDoMasks, out M obj) => transl.Item.Value.Write(writer: writer, item: item1, length: ContentLength.Invalid, doMasks: internalDoMasks, maskObj: out obj));
            }
            catch (Exception ex)
            when (doMasks)
            {
                maskObj = new MaskItem<Exception, IEnumerable<M>>(ex, null);
            }
        }

        public void Write(
            MutagenWriter writer,
            IEnumerable<T> item,
            bool doMasks,
            out MaskItem<Exception, IEnumerable<M>> maskObj,
            BinarySubWriteDelegate<T, M> transl)
        {
            try
            {
                List<M> maskList = null;
                foreach (var i in item)
                {
                    this.WriteSingleItem(writer, transl, i, doMasks, out var errObj);
                    if (errObj != null)
                    {
                        if (maskList == null)
                        {
                            maskList = new List<M>();
                        }
                        maskList.Add(errObj);
                    }
                }
                if (maskList != null)
                {
                    maskObj = new MaskItem<Exception, IEnumerable<M>>(null, maskList);
                }
                else
                {
                    maskObj = null;
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                maskObj = new MaskItem<Exception, IEnumerable<M>>(ex, null);
            }
        }

        public void Write<Mask>(
            MutagenWriter writer,
            IEnumerable<T> item,
            int fieldIndex,
            Func<Mask> errorMask,
            BinarySubWriteDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            bool doMasks = errorMask != null;
            try
            {
                List<M> maskList = null;
                foreach (var i in item)
                {
                    this.WriteSingleItem(writer, transl, i, doMasks, out var errObj);
                    if (errObj != null)
                    {
                        if (maskList == null)
                        {
                            maskList = new List<M>();
                        }
                        maskList.Add(errObj);
                    }
                }
                if (maskList != null)
                {
                    ErrorMask.HandleErrorMask(
                        errorMask,
                        fieldIndex,
                        new MaskItem<Exception, IEnumerable<M>>(null, maskList));
                }
            }
            catch (Exception ex)
            when (doMasks)
            {
                ErrorMask.HandleException(
                    errorMask,
                    fieldIndex,
                    ex);
            }
        }

        public void Write<Mask>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<IEnumerable<T>> item,
            int fieldIndex,
            RecordType recordType,
            Func<Mask> errorMask,
            BinarySubWriteDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer: writer,
                item: item.Item,
                fieldIndex: fieldIndex,
                recordType: recordType,
                errorMask: errorMask,
                transl: transl);
        }

        private void Write<Mask>(
            MutagenWriter writer,
            IEnumerable<T> item,
            int fieldIndex,
            RecordType recordType,
            Func<Mask> errorMask,
            BinarySubWriteDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            try
            {
                bool doMasks = errorMask != null;
                List<M> maskList = null;
                using (HeaderExport.ExportHeader(writer, recordType, ObjectType.Subrecord))
                {

                    foreach (var i in item)
                    {
                        this.WriteSingleItem(writer, transl, i, doMasks, out var errObj);
                        if (errObj != null)
                        {
                            if (maskList == null)
                            {
                                maskList = new List<M>();
                            }
                            maskList.Add(errObj);
                        }
                    }
                }
                if (maskList != null)
                {
                    ErrorMask.HandleErrorMask(
                        errorMask,
                        fieldIndex,
                        new MaskItem<Exception, IEnumerable<M>>(null, maskList));
                }
            }
            catch (Exception ex)
            {
                ErrorMask.HandleException(
                    errorMask,
                    fieldIndex,
                    ex);
            }
        }

        public abstract void WriteSingleItem<ErrMask>(MutagenWriter writer, BinarySubWriteDelegate<T, ErrMask> transl, T item, bool doMasks, out ErrMask maskObj);
    }
}
