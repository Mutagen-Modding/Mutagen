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
        public TryGet<IEnumerable<T>> Parse(BinaryReader reader, ContentLength length, bool doMasks, out MaskItem<Exception, IEnumerable<M>> maskObj)
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
                transl: (BinaryReader r, bool internalDoMasks, out M obj) => transl.Item.Value.Parse(reader: r, length: length, doMasks: internalDoMasks, maskObj: out obj));
        }

        public TryGet<IEnumerable<T>> Parse(
            BinaryReader reader,
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
            BinaryReader reader,
            bool doMasks,
            RecordType triggeringRecord,
            ObjectType objType,
            out MaskItem<Exception, IEnumerable<M>> maskObj,
            BinarySubParseDelegate<T, M> transl)
        {
            try
            {
                List<M> maskList = null;
                var ret = new List<T>();
                while (true)
                {
                    var get = transl(reader, doMasks, out var subMaskObj);
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

                    if (!HeaderTranslation.TryParseRecordType(reader, objType, triggeringRecord)) break;
                }
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
            BinaryReader reader,
            bool doMasks,
            int amount,
            out MaskItem<Exception, IEnumerable<M>> maskObj,
            BinarySubParseDelegate<T, M> transl)
        {
            try
            {
                List<M> maskList = null;
                var ret = new List<T>();
                for (int i = 0; i < amount; i++)
                {
                    var get = transl(reader, doMasks, out var subMaskObj);
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

        public abstract TryGet<T> ParseSingleItem(BinaryReader root, BinarySubParseDelegate<T, M> transl, bool doMasks, out M maskObj);
        
        void IBinaryTranslation<IEnumerable<T>, MaskItem<Exception, IEnumerable<M>>>.Write(BinaryWriter writer, IEnumerable<T> item, ContentLength length, bool doMasks, out MaskItem<Exception, IEnumerable<M>> maskObj)
        {
            Write(writer, item, doMasks, out maskObj);
        }

        public void Write(
            BinaryWriter writer,
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
                    transl: (T item1, bool internalDoMasks, out M obj) => transl.Item.Value.Write(writer: writer, item: item1, length: -1, doMasks: internalDoMasks, maskObj: out obj));
            }
            catch (Exception ex)
            when (doMasks)
            {
                maskObj = new MaskItem<Exception, IEnumerable<M>>(ex, null);
            }
        }

        public void Write(
            BinaryWriter writer,
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

        public abstract void WriteSingleItem<ErrMask>(BinaryWriter writer, BinarySubWriteDelegate<T, ErrMask> transl, T item, bool doMasks, out ErrMask maskObj);
    }
}
