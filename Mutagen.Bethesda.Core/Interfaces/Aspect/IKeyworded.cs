using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeyworded<TKeyword> : IKeywordedGetter<TKeyword>
        where TKeyword : IKeywordCommonGetter
    {
        new ExtendedList<IFormLink<TKeyword>>? Keywords { get; set; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeywordedGetter
    {
        IReadOnlyList<IFormLink<IKeywordCommonGetter>>? Keywords { get; }
    }

    /// <summary>
    /// An interface implemented by Major Records that have keywords
    /// </summary>
    public interface IKeywordedGetter<TKeyword> : IKeywordedGetter
        where TKeyword : IKeywordCommonGetter
    {
        new IReadOnlyList<IFormLink<TKeyword>>? Keywords { get; }
    }
    
    public static class IKeywordedExt
    {
        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by FormKey.
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordKey">FormKey of the Keyword record to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given FormKey</returns>
        public static bool HasKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            FormKey keywordKey)
            where TKeyword : IKeywordCommonGetter
        {
            return keyworded.Keywords?.Any(x => x.FormKey == keywordKey) ?? false;
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by FormKey.
        /// Also looks up that keyword in the given cache. <br />
        /// <br />
        /// Note that this function only succeeds if the record contains the keyword,
        /// and the cache found it as well. <br />
        /// <br />
        /// It is possible that the record contains the keyword, but it could not be found
        /// by the cache.
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordKey">FormKey of the Keyword record to look for</param>
        /// <param name="cache">LinkCache to resolve against</param>
        /// <param name="keyword">Keyword record retrieved, if keyworded record does contain</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given FormKey</returns>
        public static bool TryResolveKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            FormKey keywordKey,
            ILinkCache cache,
            [MaybeNullWhen(false)] out TKeyword keyword)
            where TKeyword : class, IKeywordCommonGetter
        {
            if (!keyworded.Keywords?.Any(x => x.FormKey == keywordKey) ?? true)
            {
                keyword = default;
                return false;
            }
            return cache.TryResolve(keywordKey, out keyword);
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by FormKey.
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keyword">Keyword record to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given Keyword record's FormKey</returns>
        public static bool HasKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            TKeyword keyword)
            where TKeyword : IKeywordCommonGetter
        {
            return keyworded.HasKeyword(keyword.FormKey);
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by EditorID.
        /// Also looks up that keyword in the given cache. <br />
        /// <br />
        /// Note that this function only succeeds if the record contains the keyword,
        /// and the cache found it as well. <br />
        /// <br />
        /// It is possible that the record contains the keyword, but it could not be found
        /// by the cache.
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="editorID">EditorID of the Keyword to look for</param>
        /// <param name="cache">LinkCache to resolve against</param>
        /// <param name="keyword">Keyword record retrieved, if keyworded record does contain</param>
        /// <param name="stringComparison">
        /// What string comparison type to use.<br />
        /// By default EditorIDs are case insensitive.
        /// </param>
        /// <returns>True if the Keyworded record contains a Keyword link that points to a winning Keyword record /w the given EditorID</returns>
        public static bool TryResolveKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            string editorID,
            ILinkCache cache,
            [MaybeNullWhen(false)] out TKeyword keyword,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
            where TKeyword : class, IKeywordCommonGetter
        {
            // ToDo
            // Consider EDID link cache systems if/when available
            if (keyworded.Keywords == null)
            {
                keyword = default;
                return false;
            }
            foreach (var keywordForm in keyworded.Keywords.Select(link => link.FormKey))
            {
                if (cache.TryResolve<TKeyword>(keywordForm, out keyword)
                    && (keyword.EditorID?.Equals(editorID, stringComparison) ?? false))
                {
                    return true;
                }
            }

            keyword = default;
            return false;
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by EditorID.
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="editorID">EditorID of the Keyword to look for</param>
        /// <param name="cache">LinkCache to resolve against</param>
        /// <param name="stringComparison">
        /// What string comparison type to use.<br />
        /// By default EditorIDs are case insensitive.
        /// </param>
        /// <returns>True if the Keyworded record contains a Keyword link that points to a winning Keyword record /w the given EditorID</returns>
        public static bool HasKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            string editorID,
            ILinkCache cache,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
            where TKeyword : class, IKeywordCommonGetter
        {
            return TryResolveKeyword(keyworded, editorID, cache, out _, stringComparison);
        }
    }
}
