using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Noggog.Notifying;
using Mutagen.Binary;
using Loqui;
using System.IO;

namespace Mutagen.Binary
{
    public abstract class ContainerBinaryTranslation<T, M> : IBinaryTranslation<IEnumerable<T>, MaskItem<Exception, IEnumerable<M>>>
    {
        public TryGet<IEnumerable<T>> Parse(MutagenFrame reader, bool doMasks, out MaskItem<Exception, IEnumerable<M>> maskObj)
        {
            var transl = BinaryTranslator<T, M>.Translator;
            if (transl.Item.Failed)
            {
                throw new ArgumentException($"No XML Translator available for {typeof(T)}. {transl.Item.Reason}");
            }
            return Parse(
                reader,
                doMasks,
                out maskObj,
                transl: (MutagenFrame r, bool internalDoMasks, out M obj) => transl.Item.Value.Parse(reader: r, doMasks: internalDoMasks, maskObj: out obj));
        }

        public TryGet<IEnumerable<T>> Parse(
            MutagenFrame reader,
            bool doMasks,
            out MaskItem<Exception, IEnumerable<M>> maskObj,
            BinarySubParseDelegate<T, M> transl)
        {
            try
            {
                List<M> maskList = null;
                var ret = new List<T>();
                throw new NotImplementedException();
                maskObj = maskList == null ? null : new MaskItem<Exception, IEnumerable<M>>(null, maskList);
                return TryGet<IEnumerable<T>>.Succeed(ret);
            }
            catch (Exception ex)
            when (doMasks)
            {
                maskObj = new MaskItem<Exception, IEnumerable<M>>(ex, null);
                return TryGet<IEnumerable<T>>.Failure;
            }
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
                while (true)
                {
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

                    if (!HeaderTranslation.TryGetRecordType(safeFrame, objType, triggeringRecord)) break;
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
            RecordType triggeringRecord,
            ObjectType objType,
            bool doMasks,
            Func<Mask> errorMask,
            BinarySubParseDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                triggeringRecord: triggeringRecord,
                doMasks: doMasks,
                objType: objType,
                errorMask: out var err,
                transl: transl);
            ErrorMask.HandleErrorMask(
                errorMask,
                doMasks,
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
            bool doMasks,
            int amount,
            Func<Mask> errorMask,
            BinarySubParseDelegate<T, M> transl)
            where Mask : IErrorMask
        {
            var ret = this.ParseRepeatedItem(
                frame: frame,
                amount: amount,
                doMasks: doMasks,
                errorMask: out var err,
                transl: transl);
            ErrorMask.HandleErrorMask(
                errorMask,
                doMasks,
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
            bool doMasks,
            int fieldIndex,
            Func<Mask> errorMask,
            BinarySubWriteDelegate<T, M> transl)
            where Mask : IErrorMask
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
                    ErrorMask.HandleErrorMask(
                        errorMask,
                        doMasks,
                        fieldIndex,
                        new MaskItem<Exception, IEnumerable<M>>(null, maskList));
                }
            }
            catch (Exception ex)
            {
                ErrorMask.HandleException(
                    errorMask,
                    doMasks,
                    fieldIndex,
                    ex);
            }
        }

        public abstract void WriteSingleItem<ErrMask>(MutagenWriter writer, BinarySubWriteDelegate<T, ErrMask> transl, T item, bool doMasks, out ErrMask maskObj);
    }
}
