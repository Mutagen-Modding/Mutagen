using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Noggog.Utility;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Plugins.Binary.Translations
{
    public class LoquiBinaryTranslation<T>
        where T : class, ILoquiObjectGetter
    {
        public static readonly LoquiBinaryTranslation<T> Instance = new LoquiBinaryTranslation<T>();
        public delegate T CREATE_FUNC(
            MutagenFrame reader,
            TypedParseParams? translationParams);
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
                .Where((methodInfo) => methodInfo.GetParameters().Length == 2)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(Nullable<TypedParseParams>)))
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
            out T item)
        {
            return Parse(
                frame: frame,
                item: out item,
                translationParams: null);
        }

        [DebuggerStepThrough]
        public bool Parse(
            MutagenFrame frame,
            out T item,
            TypedParseParams? translationParams)
        {
            var startPos = frame.Position;
            item = CREATE(
                reader: frame,
                translationParams: translationParams);
            return startPos != frame.Position;
        }
        #endregion
    }

    public class LoquiBinaryAsyncTranslation<T>
        where T : class, ILoquiObjectGetter
    {
        public static readonly LoquiBinaryAsyncTranslation<T> Instance = new LoquiBinaryAsyncTranslation<T>();
        public delegate Task<T> CREATE_FUNC(
            MutagenFrame reader,
            TypedParseParams? parseParams);
        public static readonly CREATE_FUNC CREATE = GetCreateFunc();

        #region Parse
        private static CREATE_FUNC GetCreateFunc()
        {
            var tType = typeof(T);
            var method = tType.GetMethods()
                .Where((methodInfo) => methodInfo.Name.Equals("CreateFromBinary"))
                .Where((methodInfo) => methodInfo.IsStatic
                    && methodInfo.IsPublic)
                .Where((methodInfo) => methodInfo.GetParameters().Length == 2)
                .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
                .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(TypedParseParams)))
                .FirstOrDefault();
            if (method == null)
            {
                throw new ArgumentException();
            }
            if (method.ReturnType == tType)
            {
                var wrap = LoquiBinaryTranslation<T>.CREATE;
                return async (MutagenFrame frame, TypedParseParams? recConv) =>
                {
                    return wrap(frame, recConv);
                };
            }
            else
            {
                return DelegateBuilder.BuildDelegate<CREATE_FUNC>(method);
            }
        }

        [DebuggerStepThrough]
        public Task<TryGet<T>> Parse(MutagenFrame frame)
        {
            return Parse(
                frame: frame,
                parseParams: null);
        }

        [DebuggerStepThrough]
        public async Task<TryGet<T>> Parse(
            MutagenFrame frame,
            TypedParseParams? parseParams)
        {
            var item = await CREATE(
                reader: frame,
                parseParams: parseParams).ConfigureAwait(false);
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
            [MaybeNullWhen(false)] out B item)
            where T : class, ILoquiObjectGetter, B
        {
            if (loquiTrans.Parse(
                frame: frame,
                item: out T tItem))
            {
                item = tItem;
                return true;
            }
            item = default;
            return false;
        }

        [DebuggerStepThrough]
        public static bool Parse<T, B>(
            this LoquiBinaryTranslation<T> loquiTrans,
            MutagenFrame frame,
            [MaybeNullWhen(false)] out B item,
            TypedParseParams? translationParams = null)
            where T : class, ILoquiObjectGetter, B
        {
            if (loquiTrans.Parse(
                frame: frame,
                item: out T tItem,
                translationParams: translationParams))
            {
                item = tItem;
                return true;
            }
            item = default;
            return false;
        }
    }
}
