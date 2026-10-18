using Loqui;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog.Utility;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

internal sealed class LoquiBinaryTranslation<T>
    where T : class, ILoquiObjectGetter
{
    public static readonly LoquiBinaryTranslation<T> Instance = new();
    public delegate T CreateFunc(
        MutagenFrame reader,
        TypedParseParams translationParams);
    public static readonly CreateFunc CREATE = GetCreateFunc();

    #region Parse
    private static CreateFunc GetCreateFunc()
    {
        var tType = typeof(T);
        var method = tType.GetMethods()
            .Where((methodInfo) => methodInfo.Name.Equals("CreateFromBinary"))
            .Where((methodInfo) => methodInfo.IsStatic
                                   && methodInfo.IsPublic)
            .Where((methodInfo) => methodInfo.ReturnType.Equals(tType))
            .Where((methodInfo) => methodInfo.GetParameters().Length == 2)
            .Where((methodInfo) => methodInfo.GetParameters()[0].ParameterType.Equals(typeof(MutagenFrame)))
            .Where((methodInfo) => methodInfo.GetParameters()[1].ParameterType.Equals(typeof(TypedParseParams)))
            .FirstOrDefault();
        if (method != null)
        {
            return DelegateBuilder.BuildDelegate<CreateFunc>(method);
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
        TypedParseParams translationParams)
    {
        var startPos = frame.Position;
        item = CREATE(
            reader: frame,
            translationParams: translationParams);
        return startPos != frame.Position;
    }
    #endregion
}

internal static class LoquiBinaryTranslationExt
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
        TypedParseParams translationParams)
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