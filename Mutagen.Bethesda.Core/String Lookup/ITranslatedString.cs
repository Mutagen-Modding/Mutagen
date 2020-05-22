using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A readonly string that can be represented in different languages
    /// </summary>
    public interface ITranslatedStringGetter : IEnumerable<KeyValuePair<Language, string>>
    {
        /// <summary>
        /// Retrieves or sets the string for the language stored in TranslatedString.DefaultLanguage
        /// </summary>
        string String { get; set; }

        /// <summary>
        /// Attempts to retrieve a string for a specific language
        /// Default language will always at least succeed and return string.Empty, never null.
        /// </summary>
        /// <param name="language">Language to query</param>
        /// <param name="str">String if located</param>
        /// <returns>True if string was located for given language</returns>
        bool Lookup(Language language, [MaybeNullWhen(false)] out string str);

        /// <summary>
        /// Attempts to retrieve a string for a specific language.
        /// Default language will always at least return string.Empty, never null.
        /// </summary>
        /// <param name="language">Language to query</param>
        /// <returns>String if located, otherwise null</returns>
        public virtual string? Lookup(Language language)
        {
            if (Lookup(language, out var str))
            {
                return str;
            }
            if (language == TranslatedString.DefaultLanguage) return string.Empty;
            return null;
        }
    }

    /// <summary>
    /// A string that can be represented in different languages
    /// </summary>
    public interface ITranslatedString : ITranslatedStringGetter
    {
        /// <summary>
        /// Sets a string for a specific language
        /// </summary>
        /// <param name="language">Language to register string under</param>
        /// <param name="str">String to register</param>
        void Set(Language language, string str);

        /// <summary>
        /// Removes a non-default language.
        /// If the default language is provided, no change will be applied.
        /// </summary>
        /// <param name="language">Language to remove</param>
        /// <returns>True if an item was removed</returns>
        bool RemoveNonDefault(Language language);

        /// <summary>
        /// Clears all non-default language string registrations
        /// </summary>
        void ClearNonDefault();
    }
}
