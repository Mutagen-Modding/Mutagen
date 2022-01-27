using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda.Strings
{
    /// <summary>
    /// Interface for looking up TranslatedStrings contents from a folder source
    /// </summary>
    public interface IStringsFolderLookup
    {
        /// <summary>
        /// Retrieves the available languages present in a source
        /// </summary>
        /// <param name="source">Source to query</param>
        /// <returns>Enumerable of languages present for given source</returns>
        IReadOnlyCollection<Language> AvailableLanguages(StringsSource source);

        /// <summary>
        /// Attempts to retrieve a string given its index key
        /// </summary>
        /// <param name="source">Strings source to look to</param>
        /// <param name="language">Language to query</param>
        /// <param name="key">Index key to look up</param>
        /// <param name="str">String if retrieved</param>
        /// <returns>True if string was located</returns>
        bool TryLookup(StringsSource source, Language language, uint key, [MaybeNullWhen(false)] out string str);

        /// <summary>
        /// Attempts to retrieve a string given its index key
        /// </summary>
        /// <param name="source">Strings source to look to</param>
        /// <param name="language">Language to query</param>
        /// <param name="key">Index key to look up</param>
        /// <returns>String if located, otherwise null</returns>
        string? Lookup(StringsSource source, Language language, uint key)
        {
            if (TryLookup(source, language, key, out var str))
            {
                return str;
            }
            return null;
        }

        /// <summary>
        /// Retrieves related strings and constructs a TranslatedString out of the results
        /// </summary>
        /// <param name="source">Strings source to look to</param>
        /// <param name="key">Index key to look up</param>
        /// <param name="targetLanguage">Language to target by default</param>
        /// <returns>TranslatedString with located strings</returns>
        TranslatedString CreateString(StringsSource source, uint key, Language targetLanguage);
    }
}
