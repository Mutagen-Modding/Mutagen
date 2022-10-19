using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Assets;

namespace Noggog;

public static class AssetLinkExt
{
    public static bool IsNumeric(this IAssetLinkGetter s, bool floatingPt = true) =>
        s.RawPath.IsNumeric(floatingPt: floatingPt);

    public static IEnumerable<string> Split(this IAssetLinkGetter line, string delim, char escapeChar) =>
        line.RawPath.Split(delim: delim, escapeChar: escapeChar);

    public static bool TrySubstringFromStart(this IAssetLinkGetter src, string item, out string result) =>
        src.RawPath.TrySubstringFromStart(item: item, result: out result);

    public static string SubstringFromStart(this IAssetLinkGetter src, string item) =>
        src.RawPath.SubstringFromStart(item: item);

    public static bool TrySubstringFromEnd(this IAssetLinkGetter src, string item, out string result) =>
        src.RawPath.TrySubstringFromEnd(item: item, result: out result);

    public static string SubstringFromEnd(this IAssetLinkGetter src, string item) =>
        src.RawPath.SubstringFromEnd(item: item);

    public static bool TryTrimStart(this IAssetLinkGetter src, string item, out string result) =>
        src.RawPath.TryTrimStart(item: item, result: out result);

    public static string TrimStart(this IAssetLinkGetter src, string item) =>
        src.RawPath.TrimStart(item: item);

    public static bool TryTrimEnd(this IAssetLinkGetter src, string item, out string result) =>
        src.RawPath.TryTrimEnd(item: item, result: out result);

    public static string TrimEnd(this IAssetLinkGetter src, string item) =>
        src.RawPath.TrimEnd(item: item);

    public static byte[] ToBytes(this IAssetLinkGetter str) =>
        str.RawPath.ToBytes();

#if NETSTANDARD2_0
#else
    public static bool ContainsInsensitive(this IAssetLinkGetter str, string rhs)
    {
        return str.RawPath.ContainsInsensitive(rhs);
    }
#endif

    /// <summary>
    /// Takes in a nullable string, and applies a string converter if it is not null or empty.
    /// </summary>
    /// <param name="src">String to process</param>
    /// <param name="decorator">String decorator if source not null or empty</param>
    /// <returns>Decorated string, or null/empty if source was null/empty</returns>
    [return: NotNullIfNotNull("src")]
    public static string? Decorate(this IAssetLinkGetter? src, Func<string, string> decorator) =>
        src?.RawPath.Decorate(decorator);

    public static bool IsNullOrWhitespace([NotNullWhen(false)] this IAssetLinkGetter? src)
    {
        return string.IsNullOrWhiteSpace(src?.RawPath);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this IAssetLinkGetter? src)
    {
        return string.IsNullOrEmpty(src?.RawPath);
    }
}