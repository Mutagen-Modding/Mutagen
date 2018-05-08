using Loqui;
using Noggog;
using Noggog.Notifying;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class LoquiBinaryTranslation<T, M> : IBinaryTranslation<T, M>
        where T : ILoquiObjectGetter
        where M : class, IErrorMask, new()
    {
        public static readonly LoquiBinaryTranslation<T, M> Instance = new LoquiBinaryTranslation<T, M>();
        private static readonly ILoquiRegistration Registration = LoquiRegistration.GetRegister(typeof(T));
        public delegate T CREATE_FUNC(MutagenFrame reader, RecordTypeConverter recordTypeConverter, bool doMasks, out M errorMask);
        public static readonly Lazy<CREATE_FUNC> CREATE = new Lazy<CREATE_FUNC>(GetCreateFunc);
        public delegate void WRITE_FUNC(MutagenWriter writer, T item, RecordTypeConverter recordTypeConverter, bool doMasks, out M errorMask);
        public static readonly Lazy<WRITE_FUNC> WRITE = new Lazy<WRITE_FUNC>(GetWriteFunc);

        private IEnumerable<KeyValuePair<ushort, object>> EnumerateObjects(
            ILoquiRegistration registration,
            MutagenFrame reader,
            bool skipProtected,
            bool doMasks,
            Func<IErrorMask> mask)
        {
            var ret = new List<KeyValuePair<ushort, object>>();
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                if (doMasks)
                {
                    mask().Overall = ex;
                }
                else
                {
                    throw;
                }
            }
            return ret;
        }

        public void CopyIn<C>(
            MutagenFrame frame,
            C item,
            bool skipProtected,
            bool doMasks,
            out M mask,
            NotifyingFireParameters cmds)
            where C : T, ILoquiObject
        {
            var maskObj = default(M);
            Func<IErrorMask> maskGet;
            if (doMasks)
            {
                maskGet = () =>
                {
                    if (maskObj == null)
                    {
                        maskObj = new M();
                    }
                    return maskObj;
                };
            }
            else
            {
                maskGet = null;
            }
            var fields = EnumerateObjects(
                item.Registration,
                frame,
                skipProtected,
                doMasks,
                maskGet);
            var copyIn = LoquiRegistration.GetCopyInFunc<C>();
            copyIn(fields, item);
            mask = maskObj;
        }

        #region Parse
        public static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var mType = typeof(M);
            var options = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("Create_Binary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.InheritsFrom(typeof(ValueTuple<,>)))
                .Where((methodInfo) => methodInfo.ReturnType.GenericTypeArguments[0].Equals(tType))
                .ToArray();
            var method = options
                .Where((methodInfo) => mType.InheritsFrom(methodInfo.ReturnType.GenericTypeArguments[1]))
                .Where((methodInfo) => methodInfo.ReturnType.Equals(typeof(ValueTuple<T, M>)))
                .FirstOrDefault();
            if (method != null)
            {
                var func = DelegateBuilder.BuildDelegate<Func<MutagenFrame, RecordTypeConverter, bool, (T item, M mask)>>(method);
                return (MutagenFrame reader, RecordTypeConverter converterDictionary, bool doMasks, out M errorMask) =>
                {
                    var ret = func(reader, converterDictionary, doMasks);
                    errorMask = ret.mask;
                    return ret.item;
                };
            }
            method = options
                .Where((methodInfo) => typeof(M).InheritsFrom(methodInfo.ReturnType.GenericTypeArguments[1], couldInherit: true)).First();
            var f = DelegateBuilder.BuildGenericDelegate<Func<MutagenFrame, RecordTypeConverter, bool, (T item, M mask)>>(tType, new Type[] { mType.GenericTypeArguments[0] }, method);
            return (MutagenFrame reader, RecordTypeConverter converterDictionary, bool doMasks, out M errorMask) =>
            {
                var ret = f(reader, converterDictionary, doMasks);
                errorMask = ret.mask;
                return ret.item;
            };
        }

        [DebuggerStepThrough]
        public TryGet<T> Parse(
            MutagenFrame frame,
            bool doMasks,
            out MaskItem<Exception, M> errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            try
            {
                var ret = TryGet<T>.Succeed(CREATE.Value(
                    reader: frame,
                    doMasks: doMasks,
                    recordTypeConverter: recordTypeConverter,
                    errorMask: out var subMask));
                errorMask = subMask == null ? null : new MaskItem<Exception, M>(null, subMask);
                return ret;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, M>(ex, default(M));
                return TryGet<T>.Failure;
            }
        }

        [DebuggerStepThrough]
        public TryGet<T> Parse<Mask>(
            MutagenFrame frame,
            int fieldIndex,
            Func<Mask> errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where Mask : IErrorMask
        {
            var ret = this.Parse(
                frame,
                errorMask != null,
                out MaskItem<Exception, M> ex,
                recordTypeConverter: recordTypeConverter);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                ex);
            return ret;
        }

        public TryGet<T> Parse(MutagenFrame reader, bool doMasks, out M mask)
        {
            var ret = Parse(reader, doMasks, out MaskItem<Exception, M> subMask);
            if (subMask?.Overall != null)
            {
                throw subMask.Overall;
            }
            mask = subMask?.Specific;
            return ret;
        }

        public TryGet<T> Parse(MutagenFrame reader, long length, bool doMasks, out M maskObj)
        {
            throw new NotImplementedException();
        }

        public TryGet<T> Parse(MutagenFrame reader, RecordType header, long lengthLength, bool doMasks, out M maskObj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        [DebuggerStepThrough]
        public static WRITE_FUNC GetWriteFunc()
        {
            var method = typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where((methodInfo) => methodInfo.Name.Equals("Write_Binary_Internal"))
                .First();
            if (!method.IsGenericMethod)
            {
                var f = DelegateBuilder.BuildDelegate<Func<T, MutagenWriter, RecordTypeConverter, bool, object>>(method);
                return (MutagenWriter writer, T item, RecordTypeConverter recordTypeConverter, bool doMasks, out M errorMask) =>
                {
                    if (item == null)
                    {
                        throw new NullReferenceException("Cannot write for a null item.");
                    }
                    errorMask = (M)f(item, writer, recordTypeConverter, doMasks);
                };
            }
            else
            {
                var f = DelegateBuilder.BuildGenericDelegate<Func<T, MutagenWriter, RecordTypeConverter, bool, object>>(typeof(T), new Type[] { typeof(M).GenericTypeArguments[0] }, method);
                return (MutagenWriter writer, T item, RecordTypeConverter recordTypeConverter, bool doMasks, out M errorMask) =>
                {
                    if (item == null)
                    {
                        throw new NullReferenceException("Cannot write for a null item.");
                    }
                    errorMask = (M)f(item, writer, recordTypeConverter, doMasks);
                };
            }
        }

        void IBinaryTranslation<T, M>.Write(MutagenWriter writer, T item, long length, bool doMasks, out M mask)
        {
            throw new NotImplementedException();
        }

        public void Write(
            MutagenWriter writer,
            T item,
            bool doMasks,
            out MaskItem<Exception, M> errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            try
            {
                WRITE.Value(
                    writer: writer,
                    item: item,
                    recordTypeConverter: recordTypeConverter,
                    doMasks: doMasks,
                    errorMask: out var subMask);
                errorMask = subMask == null ? null : new MaskItem<Exception, M>(null, subMask);
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, M>(ex, default(M));
            }
        }

        public void Write(
            MutagenWriter writer,
            T item,
            int fieldIndex,
            Func<M> errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            try
            {
                WRITE.Value(
                    writer: writer,
                    item: item,
                    recordTypeConverter: recordTypeConverter,
                    doMasks: errorMask != null,
                    errorMask: out var subMask);
                ErrorMask.HandleErrorMask(
                    errorMask,
                    fieldIndex,
                    subMask == null ? null : new MaskItem<Exception, M>(null, subMask));
            }
            catch (Exception ex)
            when (errorMask != null)
            {
                ErrorMask.HandleException(
                    errorMask,
                    fieldIndex,
                    ex);
            }
        }

        public void Write<Mask>(
            MutagenWriter writer,
            T item,
            int fieldIndex,
            Func<Mask> errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where Mask : IErrorMask
        {
            this.Write(
                writer,
                item,
                errorMask != null,
                out var subMask,
                recordTypeConverter: recordTypeConverter);
            ErrorMask.HandleErrorMask(
                errorMask,
                fieldIndex,
                subMask);
        }

        public void Write<Mask>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            int fieldIndex,
            Func<Mask> errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where Mask : IErrorMask
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask,
                recordTypeConverter: recordTypeConverter);
        }

        public void Write<Mask>(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            int fieldIndex,
            Func<Mask> errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where Mask : IErrorMask
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask,
                recordTypeConverter: recordTypeConverter);
        }
        #endregion
    }

    public static class LoquiBinaryTranslationExt
    {
        [DebuggerStepThrough]
        public static TryGet<T> Parse<T, M, C>(
            this LoquiBinaryTranslation<T, M> loqTrans,
            MutagenFrame frame,
            bool doMasks,
            out MaskItem<Exception, C> errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where T : ILoquiObjectGetter
            where C : class
            where M : class, C, IErrorMask, new()
        {
            try
            {
                var ret = TryGet<T>.Succeed(LoquiBinaryTranslation<T, M>.CREATE.Value(
                    reader: frame,
                    doMasks: doMasks,
                    recordTypeConverter: recordTypeConverter,
                    errorMask: out var subMask));
                errorMask = subMask == null ? null : new MaskItem<Exception, C>(null, subMask);
                return ret;
            }
            catch (Exception ex)
            when (doMasks)
            {
                errorMask = new MaskItem<Exception, C>(ex, default(C));
                return TryGet<T>.Failure;
            }
        }
    }
}
