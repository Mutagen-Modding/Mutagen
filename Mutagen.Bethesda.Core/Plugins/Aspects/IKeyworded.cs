using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda
{
    namespace Plugins.Aspects
    {
        /// <summary>
        /// An interface implemented by Major Records that have keywords
        /// </summary>
        public interface IKeyworded<TKeyword> : IKeywordedGetter<TKeyword>, IMajorRecordQueryable
            where TKeyword : class, IKeywordCommonGetter
        {
            new ExtendedList<IFormLinkGetter<TKeyword>>? Keywords { get; set; }
        }

        /// <summary>
        /// An interface implemented by Major Records that have keywords
        /// </summary>
        public interface IKeywordedGetter : IMajorRecordQueryableGetter
        {
            IReadOnlyList<IFormLinkGetter<IKeywordCommonGetter>>? Keywords { get; }
        }

        /// <summary>
        /// An interface implemented by Major Records that have keywords
        /// </summary>
        public interface IKeywordedGetter<TKeyword> : IKeywordedGetter
            where TKeyword : class, IKeywordCommonGetter
        {
            new IReadOnlyList<IFormLinkGetter<TKeyword>>? Keywords { get; }
        }
    }
    
    public static class IKeywordedExt
    {
        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by FormKey.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordKey">FormKey of the Keyword record to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given FormKey</returns>
        public static bool HasKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            FormKey keywordKey)
            where TKeyword : class, IKeywordCommonGetter
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
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
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
            if (!HasKeyword(keyworded, keywordKey))
            {
                keyword = default;
                return false;
            }
            return cache.TryResolve(keywordKey, out keyword);
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by FormKey.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordLink">FormLink of the Keyword record to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given FormKey</returns>
        public static bool HasKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            IFormLinkGetter<TKeyword> keywordLink)
            where TKeyword : class, IKeywordCommonGetter
        {
            return HasKeyword(keyworded, keywordLink.FormKey);
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
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordLink">FormLink of the Keyword record to look for</param>
        /// <param name="cache">LinkCache to resolve against</param>
        /// <param name="keyword">Keyword record retrieved, if keyworded record does contain</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given FormKey</returns>
        public static bool TryResolveKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            IFormLinkGetter<TKeyword> keywordLink,
            ILinkCache cache,
            [MaybeNullWhen(false)] out TKeyword keyword)
            where TKeyword : class, IKeywordCommonGetter
        {
            return TryResolveKeyword(keyworded, keywordLink.FormKey, cache, out keyword);
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by FormKey.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keyword">Keyword record to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w the given Keyword record's FormKey</returns>
        public static bool HasKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            TKeyword keyword)
            where TKeyword : class, IKeywordCommonGetter
        {
            return keyworded.HasKeyword(keyword.FormKey);
        }

        /// <summary>
        /// Checks if a Keyworded record contains a specific Keyword, by EditorID.
        /// Also looks up that keyword in the given cache.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
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
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
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
        
        /// <summary>
        /// Checks if a Keyworded record contains any specific Keyword, by FormKey.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordKeys">FormKeys of the Keyword records to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w any of the given FormKeys</returns>
        public static bool HasAnyKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            IEnumerable<FormKey> keywordKeys)
            where TKeyword : class, IKeywordCommonGetter
        {
            return keyworded.Keywords?.IntersectBy(keywordKeys, x => x.FormKey).Any() ?? false;
        }
        
         /// <summary>
        /// Checks if a Keyworded record contains any specific Keyword, by FormKey.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywordLink">FormLinks of the Keyword records to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w any of the given FormKeys</returns>
        public static bool HasAnyKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            IEnumerable<IFormLinkGetter<TKeyword>> keywordLink)
            where TKeyword : class, IKeywordCommonGetter
        {
            return HasAnyKeyword(keyworded, keywordLink.Select(x => x.FormKey));
        }
        
         /// <summary>
        /// Checks if a Keyworded record contains any specific Keyword, by FormKey.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="keywords">Keyword records to look for</param>
        /// <returns>True if the Keyworded record contains a Keyword link /w any of the given Keyword records' FormKey</returns>
        public static bool HasAnyKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            IEnumerable<TKeyword> keywords)
            where TKeyword : class, IKeywordCommonGetter
        {
            return keyworded.HasAnyKeyword(keywords.Select(x => x.FormKey));
        }

        /// <summary>
        /// Checks if a Keyworded record contains any specific Keyword, by EditorID.
        /// <br />
        /// Aspects: IKeywordedGetter&lt;IKeywordCommonGetter&gt;
        /// </summary>
        /// <param name="keyworded">Keyworded record to check</param>
        /// <param name="editorIDs">EditorIDs of the Keywords to look for</param>
        /// <param name="cache">LinkCache to resolve against</param>
        /// <param name="stringComparison">
        /// What string comparison type to use.<br />
        /// By default EditorIDs are case insensitive.
        /// </param>
        /// <returns>True if the Keyworded record contains a Keyword link that points to a winning Keyword record /w any of the the given EditorIDs</returns>
        public static bool HasAnyKeyword<TKeyword>(
            this IKeywordedGetter<TKeyword> keyworded,
            IEnumerable<string> editorIDs,
            ILinkCache cache,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
            where TKeyword : class, IKeywordCommonGetter
        {
            // ToDo
            // Consider EDID link cache systems if/when available
            if (keyworded.Keywords == null)
            {
                return false;
            }
            foreach (var keywordForm in keyworded.Keywords.Select(link => link.FormKey))
            {
                if (cache.TryResolve<TKeyword>(keywordForm, out var keyword))
                {
                    var kwEditorID = keyword.EditorID;
                    if (editorIDs.Any(editorID => kwEditorID?.Equals(editorID, stringComparison) ?? false))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
