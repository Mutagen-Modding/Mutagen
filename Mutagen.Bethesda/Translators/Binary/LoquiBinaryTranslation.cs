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
    public class LoquiBinaryTranslation<T>
        where T : ILoquiObjectGetter
    {
        public static readonly LoquiBinaryTranslation<T> Instance = new LoquiBinaryTranslation<T>();
        public delegate T CREATE_FUNC(
            MutagenFrame reader,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask);
        public static readonly Lazy<CREATE_FUNC> CREATE = new Lazy<CREATE_FUNC>(GetCreateFunc);
        public delegate void WRITE_FUNC(
            MutagenWriter writer,
            T item,
            RecordTypeConverter recordTypeConverter,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask);
        public static readonly Lazy<WRITE_FUNC> WRITE = new Lazy<WRITE_FUNC>(GetWriteFunc);

        #region Parse
        public static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var options = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("Create_Binary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.ReturnType.Equals(tType))
                .Where((methodInfo) => methodInfo.GetParameters().Length == 4)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferences)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
                .Where((methodInfo) => methodInfo.GetParameters()[3].ParameterType.Equals(typeof(ErrorMaskBuilder)))
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
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
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
            finally
            {
                errorMask?.PopIndex();
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
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
            finally
            {
                errorMask?.PopIndex();
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
            item = CREATE.Value(
                reader: frame,
                recordTypeConverter: recordTypeConverter,
                masterReferences: masterReferences,
                errorMask: errorMask);
            return true;
        }
        #endregion

        #region Write
        public static WRITE_FUNC GetWriteFunc()
        {
            var method = typeof(T).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where((methodInfo) => methodInfo.Name.Equals("Write_Binary_Internal"))
                .First();
            if (!method.IsGenericMethod)
            {
                var f = DelegateBuilder.BuildDelegate<Action<T, MutagenWriter, MasterReferences, RecordTypeConverter, ErrorMaskBuilder>>(method);
                return (MutagenWriter writer, T item, RecordTypeConverter recordTypeConverter, MasterReferences masterReferences, ErrorMaskBuilder errorMask) =>
                {
                    if (item == null)
                    {
                        errorMask.ReportExceptionOrThrow(
                            new NullReferenceException("Cannot write for a null item."));
                    }
                    f(item, writer, masterReferences, recordTypeConverter, errorMask);
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void Write(
            MutagenWriter writer,
            T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter)
        {
            WRITE.Value(
                writer: writer,
                item: item,
                masterReferences: masterReferences,
                recordTypeConverter: recordTypeConverter,
                errorMask: errorMask);
        }

        public void Write(
            MutagenWriter writer,
            T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
        {
            WRITE.Value(
                writer: writer,
                item: item,
                masterReferences: masterReferences,
                recordTypeConverter: null,
                errorMask: errorMask);
        }

        public void Write(
            MutagenWriter writer,
            T item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                WRITE.Value(
                    writer: writer,
                    item: item,
                    masterReferences: masterReferences,
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
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer: writer,
                item: item.Item,
                fieldIndex: fieldIndex,
                masterReferences: masterReferences,
                errorMask: errorMask,
                recordTypeConverter: recordTypeConverter);
        }

        public void Write(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                masterReferences: masterReferences,
                errorMask: errorMask,
                recordTypeConverter: recordTypeConverter);
        }
        #endregion
    }
    
    public class LoquiBinaryTranslation
    {
        public static readonly LoquiBinaryTranslation Instance = new LoquiBinaryTranslation();
        public delegate void WRITE_FUNC<T>(
            MutagenWriter writer,
            T item,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter,
            ErrorMaskBuilder errorMask);
        private static Dictionary<Type, object> writeDict = new Dictionary<Type, object>();

        public static WRITE_FUNC<T> GetWriteFunc<T>(Type t)
            where T : ILoquiObjectGetter
        {
            if (writeDict.TryGetValue(t, out var writeFunc))
            {
                return (WRITE_FUNC<T>)writeFunc;
            }
            var method = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where((methodInfo) => methodInfo.Name.Equals("Write_Binary_Internal"))
                .First();
            if (!method.IsGenericMethod)
            {
                var f = DelegateBuilder.BuildDelegate<Action<T, MutagenWriter, MasterReferences, RecordTypeConverter, ErrorMaskBuilder>>(method);
                WRITE_FUNC<T> ret = (MutagenWriter writer, T item, MasterReferences masterReferences, RecordTypeConverter recordTypeConverter, ErrorMaskBuilder errorMask) =>
                {
                    if (item == null)
                    {
                        errorMask.ReportExceptionOrThrow(
                            new NullReferenceException("Cannot write for a null item."));
                    }
                    f(item, writer, masterReferences, recordTypeConverter, errorMask);
                };
                writeDict[t] = ret;
                return ret;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region Write
        public void Write<T>(
            MutagenWriter writer,
            T item,
            ErrorMaskBuilder errorMask,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter)
            where T : ILoquiObjectGetter
        {
            GetWriteFunc<T>(item.GetType())(
                writer: writer,
                item: item,
                masterReferences: masterReferences,
                recordTypeConverter: recordTypeConverter,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            T item,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : ILoquiObjectGetter
        {
            GetWriteFunc<T>(item.GetType())(
                writer: writer,
                item: item,
                masterReferences: masterReferences,
                recordTypeConverter: null,
                errorMask: errorMask);
        }

        public void Write<T>(
            MutagenWriter writer,
            T item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where T : ILoquiObjectGetter
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
                GetWriteFunc<T>(item.GetType())(
                    writer: writer,
                    item: item,
                    masterReferences: masterReferences,
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

        public void Write<T>(
            MutagenWriter writer,
            IHasBeenSetItemGetter<T> item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where T : ILoquiObjectGetter
        {
            if (!item.HasBeenSet) return;
            this.Write(
                writer: writer,
                masterReferences: masterReferences,
                item: item.Item,
                fieldIndex: fieldIndex,
                errorMask: errorMask,
                recordTypeConverter: recordTypeConverter);
        }

        public void Write<T>(
            MutagenWriter writer,
            IHasItemGetter<T> item,
            int fieldIndex,
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask,
            RecordTypeConverter recordTypeConverter = null)
            where T : ILoquiObjectGetter
        {
            this.Write(
                writer,
                item.Item,
                fieldIndex,
                masterReferences: masterReferences,
                errorMask: errorMask,
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
            MasterReferences masterReferences,
            ErrorMaskBuilder errorMask)
            where T : ILoquiObjectGetter, B
        {
            try
            {
                errorMask?.PushIndex(fieldIndex);
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
