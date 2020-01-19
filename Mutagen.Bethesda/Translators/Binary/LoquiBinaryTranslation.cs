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
            RecordTypeConverter recordTypeConverter);
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
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferences)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
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
            IHasItem<T> item,
            MasterReferences masterReferences)
        {
            if (Parse(
                frame,
                item: out T subItem,
                masterReferences: masterReferences))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            MasterReferences masterReferences)
        {
            return Parse(
                frame: frame,
                item: out item,
                masterReferences: masterReferences,
                recordTypeConverter: null);
        }

        public void ParseInto(
            MutagenFrame frame,
            IHasItem<T> item,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter)
        {
            if (Parse(
                frame,
                out T subItem,
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

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter)
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
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter);
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
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(MasterReferences)))
                .Where((methodInfo) => methodInfo.GetParameters()[2].ParameterType.Equals(typeof(RecordTypeConverter)))
                .FirstOrDefault();
            if (method == null)
            {
                return null;
            }
            if (method.ReturnType.Equals(tType))
            {
                var wrap = LoquiBinaryTranslation<T>.CREATE;
                return async (MutagenFrame frame, MasterReferences master, RecordTypeConverter recConv) =>
                {
                    return wrap(frame, master, recConv);
                };
            }
            else
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
        }

        public async Task ParseInto(
            MutagenFrame frame,
            IHasItem<T> item,
            MasterReferences masterReferences)
        {
            var result = await Parse(
                frame,
                masterReferences: masterReferences).ConfigureAwait(false);
            if (result.Succeeded)
            {
                item.Item = result.Value;
            }
            else
            {
                item.Unset();
            }
        }

        [DebuggerStepThrough]
        public Task<TryGet<T>> Parse(
            MutagenFrame frame,
            MasterReferences masterReferences)
        {
            return Parse(
                frame: frame,
                masterReferences: masterReferences,
                recordTypeConverter: null);
        }

        public async Task ParseInto(
            MutagenFrame frame,
            IHasItem<T> item,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter)
        {
            var result = await Parse(
                frame,
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

        [DebuggerStepThrough]
        public async Task<TryGet<T>> Parse(
            MutagenFrame frame,
            MasterReferences masterReferences,
            RecordTypeConverter recordTypeConverter)
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
        public static void ParseInto<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            IHasItem<B> item,
            MasterReferences masterReferences)
            where T : ILoquiObjectGetter, B
        {
            if (loquiTrans.Parse(
                frame,
                out T subItem,
                masterReferences: masterReferences))
            {
                item.Item = subItem;
            }
            else
            {
                item.Unset();
            }
        }

        [DebuggerStepThrough]
        public static bool Parse<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            out B item,
            MasterReferences masterReferences)
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
