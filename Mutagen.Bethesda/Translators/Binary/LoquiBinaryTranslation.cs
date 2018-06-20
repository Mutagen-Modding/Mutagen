using Loqui;
using Loqui.Internal;
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
    public class LoquiBinaryTranslation<T> : IBinaryTranslation<T>
        where T : ILoquiObjectGetter
    {
        public static readonly LoquiBinaryTranslation<T> Instance = new LoquiBinaryTranslation<T>();
        private static readonly ILoquiRegistration Registration = LoquiRegistration.GetRegister(typeof(T));
        public delegate T CREATE_FUNC(MutagenFrame reader, RecordTypeConverter recordTypeConverter, ErrorMaskBuilder errorMask);
        public static readonly Lazy<CREATE_FUNC> CREATE = new Lazy<CREATE_FUNC>(GetCreateFunc);
        public delegate void WRITE_FUNC(MutagenWriter writer, T item, RecordTypeConverter recordTypeConverter, ErrorMaskBuilder errorMask);
        public static readonly Lazy<WRITE_FUNC> WRITE = new Lazy<WRITE_FUNC>(GetWriteFunc);

        private IEnumerable<KeyValuePair<ushort, object>> EnumerateObjects(
            ILoquiRegistration registration,
            MutagenFrame reader,
            bool skipProtected,
            ErrorMaskBuilder errorMask)
        {
            var ret = new List<KeyValuePair<ushort, object>>();
            errorMask.ReportExceptionOrThrow(
                new NotImplementedException());
            return ret;
        }

        #region Parse
        public static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var options = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("Create_Binary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.Equals(tType))
                .Where((methodInfo) => methodInfo.GetParameters().Length == 3)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(RecordTypeConverter)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(ErrorMaskBuilder)))
                .ToArray();
            var method = options
                .FirstOrDefault();
            if (method != null)
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            ErrorMaskBuilder errorMask)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame,
                    out T subItem,
                    errorMask))
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            ErrorMaskBuilder errorMask)
        {
            return Parse(
                frame: frame,
                item: out item,
                errorMask: errorMask,
                recordTypeConverter: null);
        }

        public void ParseInto(
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<T> item,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (Parse(
                    frame,
                    out T subItem,
                    errorMask,
                    recordTypeConverter))
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            item = CREATE.Value(
                reader: frame,
                recordTypeConverter: recordTypeConverter,
                errorMask: errorMask);
            return true;
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
                var f = DelegateBuilder.BuildDelegate<Action<T, MutagenWriter, RecordTypeConverter, ErrorMaskBuilder>>(method);
                return (MutagenWriter writer, T item, RecordTypeConverter recordTypeConverter, ErrorMaskBuilder errorMask) =>
                {
                    if (item == null)
                    {
                        errorMask.ReportExceptionOrThrow(
                            new NullReferenceException("Cannot write for a null item."));
                    }
                    f(item, writer, recordTypeConverter, errorMask);
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        void IBinaryTranslation<T>.Write(MutagenWriter writer, T item, long length, ErrorMaskBuilder errorMask)
        {
            throw new NotImplementedException();
        }

        public void Write(
            MutagenWriter writer,
            T item,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            WRITE.Value(
                writer: writer,
                item: item,
                recordTypeConverter: recordTypeConverter,
                errorMask: errorMask);
        }

        public void Write(
            MutagenWriter writer,
            T item,
            ErrorMaskBuilder errorMask)
        {
            WRITE.Value(
                writer: writer,
                item: item,
                recordTypeConverter: null,
                errorMask: errorMask);
        }

        public void Write(
            MutagenWriter writer,
            T item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                WRITE.Value(
                    writer: writer,
                    item: item,
                    recordTypeConverter: recordTypeConverter,
                    errorMask: errorMask);
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

        public void Write(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                errorMask,
                recordTypeConverter: recordTypeConverter);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            int fieldIndex,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
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
        public static void ParseInto<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            int fieldIndex,
            IHasItem<B> item,
            ErrorMaskBuilder errorMask)
            where T : ILoquiObjectGetter, B
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                if (loquiTrans.Parse(
                    frame,
                    out T subItem,
                    errorMask))
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
            finally
            {
                errorMask?.PopIndex();
            }
        }

        [DebuggerStepThrough]
        public static bool Parse<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            out B item,
            ErrorMaskBuilder errorMask)
            where T : ILoquiObjectGetter, B
        {
            if (loquiTrans.Parse(
                frame: frame,
                item: out T tItem,
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
