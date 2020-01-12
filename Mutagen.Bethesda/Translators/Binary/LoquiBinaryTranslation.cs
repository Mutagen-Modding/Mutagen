using Loqui;
using Loqui.Internal;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class LoquiBinaryTranslation<T>
        where T : ILoquiObjectGetter
    {
        public static readonly LoquiBinaryTranslation<T> Instance = new LoquiBinaryTranslation<T>();
        public delegate T CREATE_FUNC(
            MutagenFrame reader,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask);
        public static readonly CREATE_FUNC CREATE = GetCreateFunc();

        #region Parse
        private static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var method = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("CreateFromBinary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.Equals(tType))
                .Where((methodInfo) => methodInfo.GetParameters().Length == 4)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferences)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
                .Where((methodInfo) => methodInfo.GetParameters()[3].ParameterType.Equals(typeof(ErrorMaskBuilder)))
                .FirstOrDefault();
            if (method != null)
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
            else
            {
                return null;
            }
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(
                        frame,
                        item: out T subItem,
                        masterReferences: masterReferences,
                        errorMask: errorMask))
                    {
                        item.Item = subItem;
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

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            return Parse(
                frame: frame,
                item: out item,
                errorMask: errorMask,
                masterReferences: masterReferences,
                recordTypeConverter: null);
        }

        public bool Parse(
            MutagenFrame frame,
            out T item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    return Parse(
                        frame: frame,
                        item: out item,
                        errorMask: errorMask,
                        masterReferences: masterReferences,
                        recordTypeConverter: null);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                    item = default;
                    return false;
                }
            }
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (Parse(
                        frame,
                        out T subItem,
                        errorMask: errorMask,
                        masterReferences: masterReferences,
                        recordTypeConverter: recordTypeConverter))
                    {
                        item.Item = subItem;
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

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            item = CREATE(
                reader: frame,
                recordTypeConverter: recordTypeConverter,
                masterReferences: masterReferences,
                errorMask: errorMask);
            return true;
        }
        #endregion
    }

    public class LoquiBinaryAsyncTranslation<T>
        where T : ILoquiObjectGetter
    {
        public static readonly LoquiBinaryAsyncTranslation<T> Instance = new LoquiBinaryAsyncTranslation<T>();
        public delegate Task<T> CREATE_FUNC(
            MutagenFrame reader,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask);
        public static readonly CREATE_FUNC CREATE = GetCreateFunc();

        #region Parse
        private static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var method = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("CreateFromBinary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.GetParameters().Length == 4)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferences)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
                .Where((methodInfo) => methodInfo.GetParameters()[3].ParameterType.Equals(typeof(ErrorMaskBuilder)))
                .FirstOrDefault();
            if (method == null)
            {
                return null;
            }
            if (method.ReturnType.Equals(tType))
            {
                var wrap = LoquiBinaryTranslation<T>.CREATE;
                return async (MutagenFrame frame, MasterReferences master, RecordTypeConverter recConv, ErrorMaskBuilder errMask) =>
                {
                    return wrap(frame, master, recConv, errMask);
                };
            }
            else
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
        }

        public async Task ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    var result = await Parse(
                        frame,
                        masterReferences: masterReferences,
                        errorMask: errorMask).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        item.Item = result.Value;
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

        [DebuggerStepThrough]
        public Task<TryGet<T>> Parse(
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            return Parse(
                frame: frame,
                errorMask: errorMask,
                masterReferences: masterReferences,
                recordTypeConverter: null);
        }

        public async Task<TryGet<T>> Parse(
            MutagenFrame frame,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    return await Parse(
                        frame: frame,
                        errorMask: errorMask,
                        masterReferences: masterReferences,
                        recordTypeConverter: null).ConfigureAwait(false);
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                    return TryGet<T>.Failure;
                }
            }
        }

        public async Task ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    var result = await Parse(
                        frame,
                        errorMask: errorMask,
                        masterReferences: masterReferences,
                        recordTypeConverter: recordTypeConverter).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        item.Item = result.Value;
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

        [DebuggerStepThrough]
        public async Task<TryGet<T>> Parse(
            MutagenFrame frame,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            var item = await CREATE(
                reader: frame,
                recordTypeConverter: recordTypeConverter,
                masterReferences: masterReferences,
                errorMask: errorMask).ConfigureAwait(false);
            return TryGet<T>.Succeed(item);
        }
        #endregion
    }

    public static class LoquiBinaryTranslationExt
    {
        public static void ParseInto<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<B> item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : ILoquiObjectGetter, B
        {
            using (errorMask.PushIndex(fieldIndex))
            {
                try
                {
                    if (loquiTrans.Parse(
                        frame,
                        out T subItem,
                        masterReferences: masterReferences,
                        errorMask: errorMask))
                    {
                        item.Item = subItem;
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

        [DebuggerStepThrough]
        public static bool Parse<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            out B item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : ILoquiObjectGetter, B
        {
            if (loquiTrans.Parse(
                frame: frame,
                item: out T tItem,
                masterReferences: masterReferences,
                errorMask: errorMask))
            {
                item = tItem;
                return true;
            }
            item = default(B);
            return false;
        }
    }
}
