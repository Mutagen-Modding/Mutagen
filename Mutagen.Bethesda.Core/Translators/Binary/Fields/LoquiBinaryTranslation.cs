using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
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
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter);
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
                .Where((methodInfo) => methodInfo.GetParameters().Length == 3)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferenceReader)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
                .FirstOrDefault();
            if (method != null)
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            MasterReferenceReader masterReferences)
        {
            return Parse(
                frame: frame,
                item: out item,
                masterReferences: masterReferences,
                recordTypeConverter: null);
        }

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter)
        {
            item = CREATE(
                reader: frame,
                recordTypeConverter: recordTypeConverter,
                masterReferences: masterReferences);
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
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter);
        public static readonly CREATE_FUNC CREATE = GetCreateFunc();

        #region Parse
        private static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var method = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("CreateFromBinary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.GetParameters().Length == 3)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferenceReader)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
                .FirstOrDefault();
            if (method == null)
            {
                throw new ArgumentException();
            }
            if (method.ReturnType == tType)
            {
                var wrap = LoquiBinaryTranslation<T>.CREATE;
                return async (MutagenFrame frame, MasterReferenceReader master, RecordTypeConverter? recConv) =>
                {
                    return wrap(frame, master, recConv);
                };
            }
            else
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
        }

        [DebuggerStepThrough]
        public Task<TryGet<T>> Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences)
        {
            return Parse(
                frame: frame,
                masterReferences: masterReferences,
                recordTypeConverter: null);
        }

        [DebuggerStepThrough]
        public async Task<TryGet<T>> Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences,
            RecordTypeConverter? recordTypeConverter)
        {
            var item = await CREATE(
                reader: frame,
                recordTypeConverter: recordTypeConverter,
                masterReferences: masterReferences).ConfigureAwait(false);
            return TryGet<T>.Succeed(item);
        }
        #endregion
    }

    public static class LoquiBinaryTranslationExt
    {
        [DebuggerStepThrough]
        public static bool Parse<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            out B item,
            MasterReferenceReader masterReferences)
            where T : ILoquiObjectGetter, B
        {
            if (loquiTrans.Parse(
                frame: frame,
                item: out T tItem,
                masterReferences: masterReferences))
            {
                item = tItem;
                return true;
            }
            item = default(B);
            return false;
        }
    }
}
